using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClickTimeAPI.Models
{
    public class TimeOffEntry
    {
        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        [JsonProperty(PropertyName = "TimeOffEntryID")]
        public string TimeOffEntryId { get; set; }

        [JsonProperty(PropertyName = "TimeOffTypeID")]
        public string TimeOffTypeId { get; set; }
    }
}
