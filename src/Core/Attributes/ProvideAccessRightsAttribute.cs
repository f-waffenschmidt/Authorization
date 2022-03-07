using System;
using System.Collections.Generic;
using System.Linq;

namespace Florez4Code.Authorization.Core.Attributes
{
    public class ProvideAccessRightsAttribute : Attribute
    {
        public string CategoryKey { get; }
        private List<string> _accessRights;

        public ProvideAccessRightsAttribute(string categoryKey, params string[] accessRights)
        {
            CategoryKey = categoryKey;
            _accessRights = accessRights.ToList();
        }

        public bool ProvidesAccessRights
        {
            get => _accessRights != null && _accessRights.Any();
        }

        public List<string> GetAccessRights()
        {
            return _accessRights;
        }
    }
}