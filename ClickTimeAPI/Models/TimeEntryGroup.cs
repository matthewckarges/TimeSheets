using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickTimeAPI.Models
{
    public class TimeEntryGroup
    {
        public DateTime Date { get; set; }

        public bool? Locked { get; set; }

        public List<TimeEntry> TimeEntries { get; set; }

        public List<TimeOffEntry> TimeOffEntries { get; set; }
    }
}
