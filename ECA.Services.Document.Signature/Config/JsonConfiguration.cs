using ECA.Services.Document.Signature.DocuSign.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Config
{
    public class JsonConfiguration : IJsonConfiguration
    {
        private IConfiguration _configuration;
        public JsonConfiguration()              // ctor
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = configBuilder.Build();
        }

        public string ApiUrl {
            get
            {
                string apiUrl = _configuration["ApiUrl"];
                if (apiUrl is null) throw new ConfigFileReadError("Check appsettings.json; ApiUrl not found.");
                return apiUrl;
            }
        }
        public string IntegrationKey
        {
            get
            {
                string integrationKey = _configuration["IntegrationKey"];
                if (integrationKey is null) throw new ConfigFileReadError("Check appsettings.json; IntegrationKey not found.");
                return integrationKey;
            }
        }

        public string ConnectionString
        {
            get
            {
                string connectionString = _configuration.GetSection("ConnectionStrings")["SigSvcRepository"];
                if (connectionString is null) throw new ConfigFileReadError("Check appsettings.json; ConnectionString not found.");
                return connectionString;
            }
        }

        public double TaskManagerIntervalSeconds
        {
            get
            {
                string intervalString = _configuration["TaskManagerIntervalSeconds"];
                if (intervalString is null) throw new ConfigFileReadError("Check appsettings.json; TaskManagerIntervalSeconds not found.");
                return Convert.ToDouble(intervalString);
            }
        }

        public string JwtSecretKey
        {
            get
            {
                string key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                    throw new ConfigFileReadError("Check appsettings.json; JwtSecretkey not found.");

                return key;
            }
        }

        public string JwtIssuer
        {
            get
            {
                string issuer = _configuration["Jwt:Issuer"];
                if (string.IsNullOrEmpty(issuer))
                    throw new ConfigFileReadError("Check appsettings.json; JwtIssuer not found.");

                return issuer;
            }
        }
    }
}
