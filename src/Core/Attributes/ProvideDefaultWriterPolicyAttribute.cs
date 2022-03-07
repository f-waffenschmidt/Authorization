using System;
using Florez4Code.Authorization.Core.Models;

namespace Florez4Code.Authorization.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ProvideDefaultWriterPolicyAttribute : ProvidePolicyAttribute
    {

        public ProvideDefaultWriterPolicyAttribute(string categoryName) : base(categoryName, AuthorizationConstants.WriterPolicyName, AuthorizationConstants.WriteAccess)
        {
        }
    }
}