using Florez4Code.Authorization.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florez4Code.Authorization.Core.Attributes
{
    public class AuthorizeWriterAttribute : AuthorizeAttribute
    {
        public AuthorizeWriterAttribute(string name) : base(TransformName(name))
        {
        }

        private static string TransformName(string name)
        {
            var policyName = name.Contains(nameof(Controller))
                ? name.Replace(nameof(Controller), string.Empty).ToLower()
                : name;

            return string.Join(".", policyName, AuthorizationConstants.WriterPolicyName.ToLower());
        }
    }

    public class AuthorizeReaderAttribute : AuthorizeAttribute
    {
        public AuthorizeReaderAttribute(string name) : base(TransformName(name))
        {
        }

        private static string TransformName(string name)
        {
            var policyName = name.Contains(nameof(Controller))
                ? name.Replace(nameof(Controller), string.Empty).ToLower()
                : name.ToLower();

            return string.Join(".", policyName, AuthorizationConstants.ReaderPolicyName.ToLower());
        }
    }
}