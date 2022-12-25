using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Models
{
    public class TimesheetResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Comment { get; set; }
        public bool Absence { get; set; }
        public TimeSpan? FlexTime { get; set; }  // computed property

    }
}
