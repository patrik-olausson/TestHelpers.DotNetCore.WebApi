using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.DotNetCore.WebApi
{
    public static class AuthenticationOverrideExtensions
    {
        internal const string TestingAuthenticationScheme = "OverrideAuthForTestingScheme"; 
        
        /// <summary>
        /// Adds authentication that should override the "real" authentication registered by the
        /// application (in Startup.cs).
        /// </summary>
        /// <param name="services"></param>
        /// <param name="userId">The userId that will be used for the claim NameIdentifier</param>
        /// <param name="simulateAnonymous">Indication if you want to simulate a failed authentication, like you
        /// are an anonymous user. Useful for testing that the operation really is protected.</param>
        /// <param name="claims">Claims you want to add to the authenticated user (ClaimsPrincipal)</param>
        /// <returns></returns>
        public static IServiceCollection OverrideAuthentication(
            this IServiceCollection services,
            string userId = "DefaultUserId",
            bool simulateAnonymous = false,
            IReadOnlyCollection<Claim> claims = null)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestingAuthenticationScheme;
                    options.DefaultChallengeScheme = TestingAuthenticationScheme;
                })
                .AddScheme<AuthenticationOverrideOptions, AuthenticationOverrideHandler>(
                    TestingAuthenticationScheme, 
                    "Overrides authentication to enable testing",
                    options =>
                    {
                        options.UserId = userId;
                        options.SimulateAnonymous = simulateAnonymous;
                        options.AddClaims(claims);
                    });

            return services;
        }
    }

    public class AuthenticationOverrideHandler : AuthenticationHandler<AuthenticationOverrideOptions>
    {
        public AuthenticationOverrideHandler(
            IOptionsMonitor<AuthenticationOverrideOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(Options.SimulateAnonymous)
                return Task.FromResult(AuthenticateResult.Fail("Simulation of failed authentication i.e. an anonymous user"));
            
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        Options.GetClaims(),
                        "Test")),
                new AuthenticationProperties(),
                AuthenticationOverrideExtensions.TestingAuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }

    public class AuthenticationOverrideOptions : AuthenticationSchemeOptions
    {
        private readonly List<Claim> _claims = new List<Claim>();
        public string UserId { get; set; }
        public bool SimulateAnonymous { get; set; }

        public IReadOnlyList<Claim> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            };
            claims.AddRange(_claims);

            return claims;
        }

        public void AddClaim(Claim claim)
        {
            if (claim == null) return;
            if (claim.Type == ClaimTypes.NameIdentifier) throw new ArgumentException("The NameIdentifier claim is already added. If you want to change the value just change the UserId property!");
            
            _claims.Add(claim);
        }

        public void AddClaims(IReadOnlyCollection<Claim> claims)
        {
            if(claims == null) return;

            foreach (var claim in claims)
            {
                AddClaim(claim);
            }
        }
    }
}