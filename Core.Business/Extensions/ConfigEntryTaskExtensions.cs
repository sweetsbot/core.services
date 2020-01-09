using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Core.Business.Extensions
{
    public static class ConfigEntryTaskExtensions
    {
        public static Task<TResult> Decrypt<TResult>(this Task<TResult> task, IEncryptionProvider provider, ILogger logger) where TResult : IConfigEntry
            => task.ContinueWith(t => DecryptValue(t.Result, provider, logger));

        public static Task<IEnumerable<TResult>> Decrypt<TResult>(this Task<IEnumerable<TResult>> task, IEncryptionProvider provider, ILogger logger) where TResult : IConfigEntry
            => task.ContinueWith(t => t.Result.Select(p => DecryptValue(p, provider, logger)));

        private static TConfigEntry DecryptValue<TConfigEntry>(
            TConfigEntry entry,
            IEncryptionProvider provider,
            ILogger logger)
            where TConfigEntry : IConfigEntry
        {
            if (entry == null || !entry.IsEncrypted)
                return entry;

            if (provider.TryDecrypt(entry.ConfigValue, out var decryptedValue))
            {
                entry.ConfigValue = decryptedValue;
            }
            else
            {
                logger?.LogError("Failed to decrypt entry that is marked as encrypted!");
            }

            return entry;
        }
    }
}