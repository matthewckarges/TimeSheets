using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickTimeAPI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClickTimeAPI.Models
{
    public class TimeSheet
    {
        /*
        {
		    "DayTotals":[2,10,9,7,10,6,0],
		    "EndDate":"20150609",
		    "IncompleteDays":["20150604"],
		    "StartDate":"20150603",
		    "Status":"open",
		    "TimesheetID":"2PUTiLQYt3jc", 
		    "UserName": "Jim Hobbs"
		    "UserID": "2jMuCaFZyw7S"
	    }
         */

        public List<decimal> DayTotals { get; set; }
        
        public DateTime EndDate { get; set; }

        public List<DateTime> IncompleteDays { get; set; }
        
        public DateTime StartDate { get; set; }

        public TimeSheetStatus Status { get; set; }

        [JsonProperty(PropertyName = "TimesheetID")]
        public string TimeSheetId { get; set; }

        public string UserName { get; set; }

        [JsonProperty(PropertyName = "UserID")]
        public string UserId { get; set; }

    }
}
