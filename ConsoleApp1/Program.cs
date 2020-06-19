using Core.Entities;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sslCredentials = new SslCredentials(GetRootCertificates());
            var channel = new Channel("localhost", 5001, sslCredentials);
            var client = new Core.Services.Config.ConfigClient(channel);

            Console.WriteLine("gRPC ConfigService");
            Console.WriteLine();
            Console.WriteLine("Press a key:");
            Console.WriteLine("0: Authenticate");
            Console.WriteLine("1: Get Setting");
            Console.WriteLine("2: Get GroupConfig");
            Console.WriteLine("3: Get UserConfig");
            Console.WriteLine("4: Reset Cache");
            Console.WriteLine("5: Exit");
            Console.WriteLine("-: show jwt");
            Console.WriteLine();

            string token = null;

            var exiting = false;
            while (!exiting)
            {
                var consoleKeyInfo = Console.ReadKey(intercept: true);
                switch (consoleKeyInfo.KeyChar)
                {
                    case '-':
                        Console.WriteLine($"Token Value: {token}");
                        break;
                    case '0':
                        token = await Authenticate();
                        break;
                    case '1':
                        await GetSetting(client, token);
                        break;
                    case '2':
                        await GetGroupConfig(client, token);
                        break;
                    case '3':
                        await GetUserConfig(client, token);
                        break;
                    case '4':
                        await ResetCache(client, token);
                        break;
                    case '5':
                        exiting = true;
                        break;
                    case '6':
                        await GetGroupConfigRude(client, token);
                        break;
                    case 'a':
                        await AddEntry(client, token);
                        break;
                }
            }

            Console.WriteLine("Exiting");
        }


        public static string GetRootCertificates()
        {
            StringBuilder builder = new StringBuilder();
            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                builder.AppendLine(
                    "# Issuer: " + mCert.Issuer.ToString() + "\n" +
                    "# Subject: " + mCert.Subject.ToString() + "\n" +
                    "# Label: " + mCert.FriendlyName.ToString() + "\n" +
                    "# Serial: " + mCert.SerialNumber.ToString() + "\n" +
                    "# SHA1 Fingerprint: " + mCert.GetCertHashString().ToString() + "\n" +
                    ExportToPEM(mCert) + "\n");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Export a certificate to a PEM format string
        /// </summary>
        /// <param name="cert">The certificate to export</param>
        /// <returns>A PEM encoded string</returns>
        public static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        private static async Task<string> Authenticate()
        {
            Console.WriteLine($"Authenticating as {Environment.UserName}...");

            var sessionInfo = new SessionRequest
            {
                Application = "TestConsole",
                ApplicationVersion = new Version(1, 7),
                DomainName = Environment.UserDomainName,
                UserName = Environment.UserName,
                MachineName = Environment.MachineName,
                Department = "Main"
            };

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"https://localhost:5001/token"),
                    Method = HttpMethod.Post,
                    Version = new Version(1, 1),
                    Content = new ByteArrayContent(SerializeToUtf8Bytes(sessionInfo))
                };

                var tokenResponse = await httpClient.SendAsync(request);
                tokenResponse.EnsureSuccessStatusCode();

                var token = await tokenResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Successfully authenticated.");

                return token;
            }
        }

        private static byte[] SerializeToUtf8Bytes(SessionRequest sessionInfo)
        {
            byte[] result;

            var serializer = JsonSerializer.CreateDefault();
            serializer.Converters.Add(new JsonVersionConverter());
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms, Encoding.UTF8))
            using (var jw = new JsonTextWriter(sw))
            {
                serializer.Serialize(jw, sessionInfo);
                jw.Flush();
                sw.Flush();
                ms.Position = 0;
                result = ms.ToArray();
            }

            return result;
        }

        private static Metadata GetHeaders(string token)
        {
            Metadata headers = null;
            if (!string.IsNullOrEmpty(token))
            {
                headers = new Metadata
                {
                    {"Authorization", $"Bearer {token}"}
                };
            }

            return headers;
        }

        private static async Task GetSetting(Core.Services.Config.ConfigClient client, string token)
        {
            Console.WriteLine("Getting a single setting...");
            try
            {
                var headers = GetHeaders(token);
                Console.Write("Key? ");
                var keyName = Console.ReadLine();
                var setting = await client.GetSettingAsync(keyName, headers);
                Console.WriteLine($"{setting.Key} [{setting.Value}({setting.Type})]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setting.{Environment.NewLine}{ex}");
            }
        }

        private static async Task GetGroupConfig(Core.Services.Config.ConfigClient client, string token)
        {
            Console.WriteLine("Getting group key setting...");
            try
            {
                var headers = GetHeaders(token);
                Console.Write("Group? ");
                var keyName = Console.ReadLine();
                var resp = await client.GetGroupConfigAsync(keyName, headers);
                foreach (var (index, setting) in resp.Settings.Select((p, i) => (i, p)))
                    Console.WriteLine($"{index} => {setting.Key} [{setting.Value}({setting.Type})]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setting.{Environment.NewLine}{ex}");
            }
        }

        private static async Task GetGroupConfigRude(Core.Services.Config.ConfigClient client, string token)
        {
            Console.WriteLine("Getting group key setting...");
            try
            {
                var headers = GetHeaders(token);
                Console.Write("Group? ");
                var keyName = Console.ReadLine();
                Console.Write("Rudness? ");
                var rudeness = Console.ReadLine();
                if (!int.TryParse(rudeness, out var loops))
                {
                    loops = 10;
                }
                var sw = System.Diagnostics.Stopwatch.StartNew();
                for (int i = 0; i < loops; i++)
                {
                    await client.GetGroupConfigAsync(keyName, headers);
                    Console.Write('.');
                }
                sw.Stop();
                Console.WriteLine($"\n {sw.Elapsed} Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setting.{Environment.NewLine}{ex}");
            }
        }

        private static async Task GetUserConfig(Core.Services.Config.ConfigClient client, string token)
        {
            Console.WriteLine("Getting user's setting...");
            try
            {
                var headers = GetHeaders(token);
                var resp = await client.GetUserConfigAsync(headers);
                foreach (var (index, setting) in resp.Settings.Select((p, i) => (i, p)))
                    Console.WriteLine($"{index} => {setting.Key} [{setting.Value}({setting.Type})]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting setting.{Environment.NewLine}{ex}");
            }
        }

        private static async Task ResetCache(Core.Services.Config.ConfigClient client, string token)
        {
            Console.WriteLine("resetting cache...");
            try
            {
                var headers = GetHeaders(token);
                var resp = await client.ResetCacheAsync(headers);
                Console.WriteLine($"Reset Cache");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting cache.{Environment.NewLine}{ex}");
            }
        }

        private static async Task AddEntry(Core.Services.Config.ConfigClient client, string token)
        {
            try
            {
                var headers = GetHeaders(token);
                var key = Prompt("Key? ");
                var value = Prompt("Value? ");
                var encryptStr = Prompt("Encrypt? ");
                var encrypt = encryptStr.ToUpperInvariant()[0] == 'Y';
                var typeStr = Prompt("Type? ");
                if (!Enum.TryParse<SettingType>(typeStr, out var type))
                {
                    type = SettingType.String;
                };
                var app = Prompt("App? ");
                var env = Prompt("Env? ");
                var domain = Prompt("Domain? ");
                var user = Prompt("User? ");
                var newSetting = new SetSetting
                {
                    Application = app,
                    Domain = domain,
                    Encrypt = encrypt,
                    Environment = env,
                    Key = key,
                    Type = type,
                    UserName = user,
                    Value = value
                };
                var resp = await client.AddSettingAsync(newSetting, headers);
                Console.WriteLine("Success? {0}", resp.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in adding entry.{Environment.NewLine}{ex}");
            }
        }

        private static string Prompt(string message)
        {
            Console.Write(message);
            var userInput = Console.ReadLine();
            return userInput;
        }

        private class JsonVersionConverter : JsonConverter<Version>
        {
            public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var versionString = reader.ReadAsString();
                return Version.Parse(versionString);
            }

            public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}
