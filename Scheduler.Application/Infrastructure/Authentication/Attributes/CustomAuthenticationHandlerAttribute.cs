using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler.Application.Infrastructure.Authorization.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Scheduler.Application.Infrastructure.Authentication.Attributes
{
    public class CustomAuthenticationHandlerAttribute(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseApp firebaseApp,
        IUserSessionSet userSession) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
    {
        private static readonly string BearerPrefix = "Bearer ";
        private readonly FirebaseApp _firebaseApp = firebaseApp;
        private readonly IUserSessionSet _userSession = userSession;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            IHeaderDictionary headers = Context.Request.Headers;
            if (!headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
                return AuthenticateResult.NoResult();

            string bearerToken = value!;

            if (bearerToken == null || !bearerToken.StartsWith(BearerPrefix))
                return AuthenticateResult.Fail("Invalid Authorization token");

            string token = bearerToken[BearerPrefix.Length..];
            try
            {
                FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);
                return AuthenticateResult.Success(GetAuthenticationTicket(firebaseToken, token));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private AuthenticationTicket GetAuthenticationTicket(FirebaseToken firebaseToken, string bearerToken)
        {
            var claims = ToClaims(firebaseToken.Claims, bearerToken);
            return new AuthenticationTicket(new ClaimsPrincipal(new List<ClaimsIdentity>
            {
                new ClaimsIdentity(claims, nameof(CustomAuthenticationHandlerAttribute))
            }), JwtBearerDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim>? ToClaims(IReadOnlyDictionary<string, object> claims, string bearerToken)
        {
            var email = claims["email"].ToString()!;
            var externalId = claims["user_id"].ToString()!;

            var displayName = claims["name"].ToString()!;
            var nameAndPermissionArray = displayName.Split('-');
            var name = nameAndPermissionArray[0].Trim();
            var isAdmin = int.Parse(nameAndPermissionArray[1].Trim()) == 1;

            _userSession.SetUserSession(externalId, name, email, isAdmin);

            return
            [
                new Claim("id", externalId),
                new Claim("email", email),
                new Claim("name", name)
            ];
        }
    }
}
