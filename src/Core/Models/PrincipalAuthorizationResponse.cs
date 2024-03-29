﻿using System.Collections.Generic;
using IdentityModel;
using IdentityModel.Client;

namespace Fwaqo.Authorization.Core.Models
{
    public class PrincipalAuthorizationResponse : ProtocolResponse
    {
        /// <summary>
        /// Try get enumerable value from response
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private IEnumerable<string> TryGetArray(string name) => Json.TryGetStringArray(name);

        /// <summary>
        /// Gets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription);


        /// <summary>
        /// Gets the permissions
        /// </summary>
        public IEnumerable<string> Permissions =>
            TryGetArray(AuthorizationConstants.PrincipalAuthorizationResponse.Permissions);

        /// <summary>
        /// Gets the roles
        /// </summary>
        public IEnumerable<string> Roles =>
            TryGetArray(AuthorizationConstants.PrincipalAuthorizationResponse.Roles);


        /// <summary>
        /// Gets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        public int ExpiresIn
        {
            get
            {
                var value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

                if (value == null) return 0;
                return int.TryParse(value, out var theValue) ? theValue : 0;
            }
        }
    }
}