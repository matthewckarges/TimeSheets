using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClickTimeAPI.Models
{
    public class TimeEntry
    {
        public decimal BreakTime { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        [JsonProperty(PropertyName = "ISOEndTime")]
        public TimeSpan? IsoEndTime { get; set; }

        [JsonProperty(PropertyName = "ISOStartTime")]
        public TimeSpan? IsoStartTime { get; set; }

        [JsonProperty(PropertyName = "JobID")]
        public string JobId { get; set; }

        [JsonProperty(PropertyName = "PhaseID")]
        public string PhaseId { get; set; }

        [JsonProperty(PropertyName = "SubPhaseID")]
        public string SubPhaseId { get; set; }

        [JsonProperty(PropertyName = "TaskID")]
        public string TaskId { get; set; }

        [JsonProperty(PropertyName = "TimeEntryID")]
        public string TimeEntryId { get; set; }

        public List<OptionalData> OptionalData { get; set; }
    }
}
