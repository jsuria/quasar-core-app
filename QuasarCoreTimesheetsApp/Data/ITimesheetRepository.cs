using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Data
{
    // interface for TimesheetRepository
    public interface ITimesheetRepository
    {
        void CreateTimesheet(Timesheet timesheet);
        void UpdateTimesheet(Timesheet timesheet);
        Timesheet GetTimesheet(int id);                     // retrieving a specific entry by id
        Timesheet GetTimesheet(int userId, DateTime date);  // retrieving by userId and date

        IEnumerable<Timesheet> GetTimesheets(int userId, DateTime startDate, DateTime endDate); // list of entries by specific user
        IEnumerable<UserTimesheet> GetTimesheets(DateTime startDate, DateTime endDate);             // list of entries for all users

        void DeleteTimesheet(int id);

    }
}
