using System;
using Florez4Code.Authorization.Core.Models;

namespace Florez4Code.Authorization.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]

    public class ProvideDefaultReaderPolicyAttribute : ProvidePolicyAttribute
    {
        public ProvideDefaultReaderPolicyAttribute(string categoryName) : base(categoryName, AuthorizationConstants.ReaderPolicyName, AuthorizationConstants.ReadAccess)
        {
        }
    }
}