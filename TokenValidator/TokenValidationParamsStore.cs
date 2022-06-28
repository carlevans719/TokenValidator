using System;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Timers;

namespace TokenValidator
{
    public class TokenValidationParamsStore
    {
        private static TokenValidationParameters Params;
        private static System.Timers.Timer Timer = null;
        private static bool HasFetched = false;
        private static DateTime LastFetch;

        public static void Start()
        {
            if (Timer == null)
            {
                RefreshCache();
                Timer = new System.Timers.Timer(5 * 1000 * 60);
                Timer.Elapsed += RefreshCacheFromTimer;
                Timer.AutoReset = true;
                Timer.Enabled = true;
            }
        }

        public static void Stop()
        {
            Timer.Stop();
            Timer.Dispose();
            Timer = null;
        }

        public static TokenValidationParameters GetParams()
        {
            return Params;
        }

        private static void RefreshCacheFromTimer(Object source, ElapsedEventArgs e)
        {
            RefreshCache();
        }

        public static async void RefreshCache()
        {
            if (HasFetched && DateTime.UtcNow.Subtract(LastFetch).TotalMinutes < 1)
            {
                Console.WriteLine("Not refreshing because the last fetch was <5 minutes ago");
                return;
            }

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                Environment.GetEnvironmentVariable("JWKS_URI"),
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever()
            );

            CancellationToken ct = default(CancellationToken);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            var discoveryDocument = await configurationManager.GetConfigurationAsync(ct);
            var signingKeys = discoveryDocument.SigningKeys;
            Console.WriteLine(signingKeys);
            Params = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = discoveryDocument.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
            };
            LastFetch = DateTime.UtcNow;
            HasFetched = true;
        }
    }
}
