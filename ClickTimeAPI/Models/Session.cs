using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ClickTimeAPI.Models
{
    public class Session
    {
        /*
        {
	        "CompanyID":"2mwn-8y6ivGa",
	        "Token":"mNAOFfkgJwUkeOGwHfQSLI1x5USZdJxeErdiHzl7mPa=",
	        "UserEmail":"jim@acme.com",
	        "UserID":"2jMuCaFZyw7S",
	        "UserName":"Jim Hobbs", 
	        "SecurityLevel":"MANAGER"NEW
        }
         */

        [JsonProperty(PropertyName = "CompanyID")]
        public string CompanyId { get; set; }
        
        public string Token { get; set; }

        public string UserEmail { get; set; }

        [JsonProperty(PropertyName = "UserID")]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string SecurityLevel { get; set; }
    }
}
