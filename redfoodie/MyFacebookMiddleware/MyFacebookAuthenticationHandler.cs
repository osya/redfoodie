using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Helpers;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json.Linq;

namespace redfoodie.MyFacebookMiddleware
{
    internal class MyFacebookAuthenticationHandler : AuthenticationHandler<FacebookAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public MyFacebookAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var properties = (AuthenticationProperties)null;
            try
            {
                var accessToken = Request.Query["accessToken"];
                var expires = Request.Query["expiresIn"];
                string text;
                if (null == accessToken)
                {
                    var code = (string) null;
                    var state = (string) null;
                    var query = Request.Query;
                    var values = query.GetValues("error");
                    if (values != null && values.Count >= 1)
                        _logger.WriteVerbose("Remote server returned an error: " + Request.QueryString);
                    values = query.GetValues("code");
                    if (values != null && values.Count == 1)
                        code = values[0];
                    values = query.GetValues("state");
                    if (values != null && values.Count == 1)
                        state = values[0];
                    properties = Options.StateDataFormat.Unprotect(state);
                    if (properties == null)
                        return null;
                    if (!ValidateCorrelationId(properties, _logger))
                        return new AuthenticationTicket(null, properties);
                    if (code == null)
                        return new AuthenticationTicket(null, properties);
                    var requestPrefix = Request.Scheme + "://" + Request.Host;
                    var redirectUri = requestPrefix + Request.PathBase + Options.CallbackPath;
                    var tokenRequest = "grant_type=authorization_code&code=" + Uri.EscapeDataString(code) +
                                       "&redirect_uri=" + Uri.EscapeDataString(redirectUri) + "&client_id=" +
                                       Uri.EscapeDataString(Options.AppId) + "&client_secret=" +
                                       Uri.EscapeDataString(Options.AppSecret);
                    var tokenResponse =
                        await _httpClient.GetAsync(Options.TokenEndpoint + "?" + tokenRequest, Request.CallCancelled);
                    tokenResponse.EnsureSuccessStatusCode();
                    text = await tokenResponse.Content.ReadAsStringAsync();
                    var form = WebHelpers.ParseForm(text);
                    accessToken = form["access_token"];
                    expires = form["expires"];
                }
                else
                {
                    properties = new AuthenticationProperties()
                    {
                        RedirectUri = "/Account/ExternalLoginCallback"
                    };
                }
                var graphAddress = Options.UserInformationEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken);
                if (Options.SendAppSecretProof)
                {
                    graphAddress += "&appsecret_proof=" + GenerateAppSecretProof(accessToken);
                }
                var graphResponse = await _httpClient.GetAsync(graphAddress, Request.CallCancelled);
                graphResponse.EnsureSuccessStatusCode();
                text = await graphResponse.Content.ReadAsStringAsync();
                var user = JObject.Parse(text);
                var context = new FacebookAuthenticatedContext(Context, user, accessToken, expires)
                {
                    Identity =
                        new ClaimsIdentity(Options.AuthenticationType,
                            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                };
                if (!string.IsNullOrEmpty(context.Id))
                    context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", context.Id, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                if (!string.IsNullOrEmpty(context.UserName))
                    context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", context.UserName, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                if (!string.IsNullOrEmpty(context.Email))
                    context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", context.Email, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                if (!string.IsNullOrEmpty(context.Name))
                {
                    context.Identity.AddClaim(new Claim("urn:facebook:name", context.Name, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                    if (string.IsNullOrEmpty(context.UserName))
                        context.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", context.Name, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Link))
                    context.Identity.AddClaim(new Claim("urn:facebook:link", context.Link, "http://www.w3.org/2001/XMLSchema#string", Options.AuthenticationType));
                context.Properties = properties;
                await Options.Provider.Authenticated(context);
                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError("Authentication failed", ex);
                return new AuthenticationTicket(null, properties);
            }
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (!Options.CallbackPath.HasValue || !(Options.CallbackPath == Request.Path))
                return false;
            var ticket = await AuthenticateAsync();
            if (ticket == null)
            {
                _logger.WriteWarning("Invalid return state, unable to redirect.");
                Response.StatusCode = 500;
                return true;
            }
            var context = new FacebookReturnEndpointContext(Context, ticket)
            {
                SignInAsAuthenticationType = Options.SignInAsAuthenticationType,
                RedirectUri = ticket.Properties.RedirectUri
            };
            await Options.Provider.ReturnEndpoint(context);
            if (context.SignInAsAuthenticationType != null && context.Identity != null)
            {
                var claimsIdentity = context.Identity;
                if (!string.Equals(claimsIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                    claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, context.SignInAsAuthenticationType, claimsIdentity.NameClaimType, claimsIdentity.RoleClaimType);
                Context.Authentication.SignIn(context.Properties, claimsIdentity);
            }
            if (context.IsRequestCompleted || context.RedirectUri == null) return context.IsRequestCompleted;
            var str = context.RedirectUri;
            if (context.Identity == null)
                str = WebUtilities.AddQueryString(str, "error", "access_denied");
            Response.Redirect(str);
            context.RequestCompleted();
            return context.IsRequestCompleted;
        }

        private string GenerateAppSecretProof(string accessToken)
        {
            using (var hmacshA256 = new HMACSHA256(Encoding.ASCII.GetBytes(Options.AppSecret)))
            {
                var hash = hmacshA256.ComputeHash(Encoding.ASCII.GetBytes(accessToken));
                var stringBuilder = new StringBuilder();
                foreach (var t in hash)
                    stringBuilder.Append(t.ToString("x2", CultureInfo.InvariantCulture));
                return stringBuilder.ToString();
            }
        }
    }
}