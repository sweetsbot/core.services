﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Core.Services
{
    public class Startup
    {
        private readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();

        private readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        private readonly Random Random = new Random();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(CoreClaimTypes.UserName);
                    policy.RequireClaim(CoreClaimTypes.Session);
                });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateActor = false,
                            ValidateLifetime = true,
                            IssuerSigningKey = SecurityKey,
                            NameClaimType = CoreClaimTypes.UserName,
                            RoleClaimType = CoreClaimTypes.Role
                        };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ConfigService>();

                endpoints.MapGet("/",async context =>
                    {
                        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });

                endpoints.MapGet("/token", async context =>
                {
                    var sessionRequest = await JsonSerializer.DeserializeAsync<SessionRequest>(context.Request.Body);
                    await context.Response.WriteAsync(GenerateJwtToken(sessionRequest));
                });
            });
        }

        private string GenerateJwtToken(SessionRequest sessionRequest)
        {
            var claims = new[]
            {
                new Claim(CoreClaimTypes.UserName, sessionRequest.ToBlameString()),
                new Claim(CoreClaimTypes.Application, sessionRequest.VersionString()),
                new Claim(CoreClaimTypes.Department, sessionRequest.Department),
                new Claim(CoreClaimTypes.MachineName, sessionRequest.MachineName),
                new Claim(CoreClaimTypes.Session, Random.Next().ToString(), ClaimValueTypes.Integer32),
                new Claim(CoreClaimTypes.Role, "Developer"), 
                new Claim(CoreClaimTypes.Role, "User"), 
            };
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("Core.Services", "Client", claims, expires: DateTime.Now.AddHours(18), signingCredentials: credentials);
            return JwtTokenHandler.WriteToken(token);
        }
    }
}