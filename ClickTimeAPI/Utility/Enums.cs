using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClickTimeAPI.Utility
{
    public enum TimeSheetStatus
    {
        [EnumMember(Value = "approved")]
        Approved,
        [EnumMember(Value = "rejected")]
        Rejected,
        [EnumMember(Value = "waiting")]
        Waiting,
        [EnumMember(Value = "open")]
        Open
    }
}
