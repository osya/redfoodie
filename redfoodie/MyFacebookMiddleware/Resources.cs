using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace redfoodie.MyFacebookMiddleware
{
    [CompilerGenerated]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode]
    internal class Resources
    {
        private static ResourceManager _resourceMan;
        private static CultureInfo _resourceCulture;

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(_resourceMan, null))
                    _resourceMan = new ResourceManager("Microsoft.Owin.Security.Facebook.Resources", typeof(Resources).Assembly);
                return _resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => _resourceCulture;
            set => _resourceCulture = value;
        }

        /// <summary>
        ///   Looks up a localized string similar to An ICertificateValidator cannot be specified at the same time as an HttpMessageHandler unless it is a WebRequestHandler..
        /// </summary>
        internal static string ExceptionValidatorHandlerMismatch => ResourceManager.GetString("Exception_ValidatorHandlerMismatch", _resourceCulture);
    }
}