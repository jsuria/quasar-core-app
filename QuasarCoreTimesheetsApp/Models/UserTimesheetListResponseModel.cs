using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Models
{
    public class UserTimesheetListResponseModel
    {
        public string DisplayName { get; set; }
        public TimeSpan FlexTime { get; set; }
        public TimesheetResponseModel[] Timesheets { get; set; }
    }
}
