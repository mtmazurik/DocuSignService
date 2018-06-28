using ECA.Services.Document.Signature.Config;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System;

namespace ECA.Services.Document.Signature.Security
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsonConfiguration _config;
        public AuthenticationMiddleware(RequestDelegate next, IJsonConfiguration config)
        {
            _next = next;
            _config = config;
        }
        
        public async Task Invoke(HttpContext context)
        {
            var bearer = "Bearer ";
            string authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith(bearer))
            {
                var tokenString = authHeader.Substring(bearer.Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyByteArray = TextEncodings.Base64Url.Decode(_config.JwtSecretKey);
                var signingKey = new SymmetricSecurityKey(keyByteArray);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(1),
                    ValidateAudience = false,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config.JwtIssuer,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };

                try
                {
                    // grab the principal for later use for endpoint security
                    var claimsPrincipal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out var rawValidatedToken);
                }
                catch (SecurityTokenValidationException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
                await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
    }
}
