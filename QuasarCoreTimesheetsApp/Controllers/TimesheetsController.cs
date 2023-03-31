using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using QuasarCoreTimesheetsApp.Data;
using QuasarCoreTimesheetsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Limits access to authenticated users
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetsController : BaseController
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IMapper _mapper;

        public TimesheetsController(ITimesheetRepository timesheetRepository, IMapper mapper)
        {
            _timesheetRepository = timesheetRepository;
            _mapper = mapper;
        }

        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpPut("start")]
        public IActionResult Start()
        {
            var userId = GetUserId();
            var startTime = DateTime.Now;

            // Checking for existing timesheet for this user
            var existingTimesheet = _timesheetRepository.GetTimesheet(userId, startTime.Date);

            if (existingTimesheet != null)
            {
                return BadRequest();
            }

            var timesheet = new Timesheet
            {
                UserId = userId,
                Date = startTime.Date,
                StartTime = startTime.TimeOfDay
            };

            _timesheetRepository.CreateTimesheet(timesheet);
            var result = _mapper.Map<TimesheetResponseModel>(timesheet);

            CalculateFlexTime(result);

            return Ok(result);
        }

        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpPut("{id}/end")]      // or end/{id}
        public IActionResult End(int id)
        {
            // Checking for existing timesheet for this user
            var timesheet = _timesheetRepository.GetTimesheet(id);

            if(timesheet == null || timesheet.UserId != GetUserId() || timesheet.Absence)
            {
                return BadRequest();
            }

            timesheet.EndTime = DateTime.Now.TimeOfDay;

            _timesheetRepository.UpdateTimesheet(timesheet);
            var result = _mapper.Map<TimesheetResponseModel>(timesheet);
            CalculateFlexTime(result);
            return Ok(result);
        }


        // Listing for specific user
        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpGet("")]
        public IActionResult List(DateTime startDate, DateTime endDate)
        {
            // Checking for existing timesheet for this user
            var timesheets = _timesheetRepository.GetTimesheets(GetUserId(), startDate.Date, endDate.Date);

            var result = new UserTimesheetListResponseModel
            {
                Timesheets = _mapper.Map<TimesheetResponseModel[]>(timesheets)
            };

            CalculateFlexTime(result);
            return Ok(result);
         }

        // Listing for admin/supervisor
        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpGet("all")]
        [Authorize(Roles ="admin")]
        public IActionResult ListAll(DateTime startDate, DateTime endDate)
        {
            // Checking for existing timesheets within this range
            var timesheets = _timesheetRepository.GetTimesheets(startDate.Date, endDate.Date);
            // Group the result userid and displayname, sort by displayname
            var result = timesheets.GroupBy(ts => (ts.UserId, ts.DisplayName))
                                   .OrderBy(grp => grp.Key.DisplayName)
                                   .Select(uts => new UserTimesheetListResponseModel
                                   {
                                       DisplayName = uts.Key.DisplayName,
                                       Timesheets = _mapper.Map<TimesheetResponseModel[]>(uts)
                                   }).ToArray();

            // Calculate by group
            foreach(var group in result)
            {
                CalculateFlexTime(group);
            }

            return Ok(result);
        }

        // Endpoint for absence
        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpGet("absence")]
        public IActionResult CreateAbsence([FromBody]AbsenceRequestModel absenceRequest)
        {
            var userId = GetUserId();

            if (_timesheetRepository.GetTimesheet(userId, DateTime.Today) != null)
            {
                return BadRequest();
            }

            var absence = new Timesheet
            {
                UserId = userId,
                Date = absenceRequest.Date.Date,
                Absence = true,
                Comment = absenceRequest.Comment
            };

            _timesheetRepository.CreateTimesheet(absence);
            var result = _mapper.Map<TimesheetResponseModel>(absence);
            return Ok(result);
        }

        [EnableCors("PolicyLocalhostWithOrigins")]
        [HttpGet("absence/{id}")]
        public IActionResult DeleteAbsence(int id)
        {
            var timesheet = _timesheetRepository.GetTimesheet(id);

            if(timesheet == null || timesheet.UserId != GetUserId() || !timesheet.Absence)
            {
                return BadRequest();
            }

            _timesheetRepository.DeleteTimesheet(id);
            return Ok();
        }

        // Calculation for flextime on listing
        private void CalculateFlexTime(params UserTimesheetListResponseModel[] timesheetLists)
        {
            foreach (var list in timesheetLists)
            {
                CalculateFlexTime(list.Timesheets);

                var timesheets = list.Timesheets.Where(t => t.FlexTime.HasValue);
                list.FlexTime = TimeSpan.FromSeconds(timesheets.Sum(t => t.FlexTime.Value.TotalSeconds));
            }
        }

        // in C#, params allows variable number of arguments
        // Calculation for flextime on normal circumstances
        private void CalculateFlexTime(params TimesheetResponseModel[] timesheets)
        {
            // only process non-absence entries and those with valid endtime
            foreach(var timesheet in timesheets.Where(ts => !ts.Absence && ts.EndTime.HasValue))
            {
                if (!timesheet.Absence && timesheet.EndTime.HasValue)
                {
                    var workingHours = timesheet.EndTime.Value - timesheet.StartTime;

                    // Flextime calculations
                    timesheet.FlexTime = workingHours - TimeSpan.FromHours(7.5);

                    if (workingHours.TotalHours > 6)
                    {
                        timesheet.FlexTime -= timesheet.FlexTime - TimeSpan.FromMinutes(30);
                    }

                }
            }
        }

/*
        private void CalculateFlexTime(params TimesheetResponseModel[] timesheets)
        {
            foreach (var timesheet in timesheets.Where(t => !t.Absence && t.EndTime.HasValue))
            {
                var workingHours = timesheet.EndTime.Value - timesheet.StartTime;
                timesheet.FlexTime = workingHours - TimeSpan.FromHours(7.5);
                if (workingHours.TotalHours > 6)
                {
                    timesheet.FlexTime = timesheet.FlexTime - TimeSpan.FromMinutes(30);
                }
            }
        }

        private void CalculateFlexTime(params UserTimesheetListResponseModel[] timesheetLists)
        {
            foreach (var list in timesheetLists)
            {
                CalculateFlexTime(list.Timesheets);

                var timesheets = list.Timesheets.Where(t => t.FlexTime.HasValue);
                list.FlexTime = TimeSpan.FromSeconds(timesheets.Sum(t => t.FlexTime.Value.TotalSeconds));
            }
        }

*/


    }
}
