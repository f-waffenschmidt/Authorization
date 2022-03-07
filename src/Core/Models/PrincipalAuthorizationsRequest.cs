using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Florez4Code.Authorization.Core.Models
{
    /// <summary>
    /// Principal Authorization Request
    /// </summary>
    public class PrincipalAuthorizationsRequest : HttpRequestMessage
    {
        /// <summary>
        /// PrincipalAuthorizationsRequest
        /// </summary>
        /// <param name="address"></param>
        public PrincipalAuthorizationsRequest(string address)
        {
            Address = address;
            Headers.Accept.Clear();
            Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Address 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the client assertion.
        /// </summary>
        /// <value>
        /// The assertion.
        /// </value>
        public void Prepare()
        {
            if (!string.IsNullOrEmpty(Address))
            {
                RequestUri = new Uri(Address ?? "");
            }
        }
    }
}