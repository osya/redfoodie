using System;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace redfoodie.MyFacebookMiddleware
{
    public class MyFacebookAuthenticationMiddleware: FacebookAuthenticationMiddleware
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public MyFacebookAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, FacebookAuthenticationOptions options): base(next, app, options)
        {
            _logger = app.CreateLogger<MyFacebookAuthenticationMiddleware>();
            _httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 10485760L
            };
        }

        protected override AuthenticationHandler<FacebookAuthenticationOptions> CreateHandler()
        {
            return new MyFacebookAuthenticationHandler(_httpClient, _logger);
        }

        private static HttpMessageHandler ResolveHttpMessageHandler(FacebookAuthenticationOptions options)
        {
            var httpMessageHandler = options.BackchannelHttpHandler ?? new WebRequestHandler();
            if (options.BackchannelCertificateValidator == null) return httpMessageHandler;
            var webRequestHandler = httpMessageHandler as WebRequestHandler;
            if (webRequestHandler == null)
                throw new InvalidOperationException(Resources.ExceptionValidatorHandlerMismatch);
            webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            return httpMessageHandler;
        }
    }
}