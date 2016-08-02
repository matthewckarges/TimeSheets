using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ClickTimeAPI.Models;

namespace ClickTimeAPI.Approval
{
    class Program
    {
        private static string _email;
        private static string _password;

        static void Main(string[] args)
        {
            var loggedIn = false;
            while (!loggedIn)
            {
                var email = string.Empty;
                while (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("ClickTime Email Address:");
                    email = Console.ReadLine();
                }
                Console.WriteLine("ClickTime Password");
                var password = ReadPassword();
                loggedIn = ClickTime.Initialize(email, password);
                if (!loggedIn)
                {
                    Console.WriteLine("ClickTime Authentication error");
                }
            }

            Console.WriteLine("Office365 Email Address:");
            _email = Console.ReadLine();
            Console.WriteLine("Office365 Password:");
            _password = ReadPassword();
            Console.WriteLine();

            Process();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void Process()
        {
            var taskRunning = false;
            while (true)
            {
                Console.WriteLine("Start reoccurring task (r) or just one time (o)?");
                var op = Console.ReadLine();
                switch (op)
                {
                    case "r":
                        if (!taskRunning)
                        {
                            Console.WriteLine("Running task to check for approvals every dat at 5pm. Close this window to stop task.");
                            taskRunning = true;
                            ScheduleReocurringTask(false);
                        }
                        else
                        {
                            Console.WriteLine("The task is already running");
                        }
                        break;
                    case "o":
                        CheckApprovals();
                        break;
                    case "q":
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Unrecognized input.");
                        break;
                }
            }
        }

        private static void ScheduleReocurringTask(bool runNow)
        {
            var now = DateTime.Now;
            TimeSpan delay;
            var fivepm = new TimeSpan(0 /*days*/, 13 /*hours*/, 50 /*minutes*/, 0 /*seconds*/);
            var day = new TimeSpan(1 /*days*/, 0 /*hours*/, 0 /*minutes*/, 0 /*seconds*/);
            if (now.TimeOfDay <= fivepm)
            {
                delay = fivepm - now.TimeOfDay;
            }
            else
            {
                delay = day + (fivepm - now.TimeOfDay);
            }

            Task.Delay(delay).ContinueWith(t =>
            {
                ScheduleReocurringTask(true);
            });
            if (runNow)
            {
                CheckApprovals();
            }
        }
        
        private static void CheckApprovals()
        {
            var timesheets = ClickTime.GetPendingTimeSheets();

            if (!timesheets.Any())
            {
                Console.WriteLine("There were no timesheets to aprove");
                return;
            }

            foreach (var timesheet in timesheets)
            {
                if (ConfigurationManager.AppSettings["Interns"].Split(',').Contains(timesheet.UserName))
                {
                    if (timesheet.DayTotals.Sum() < 20)
                    {
                        var message = $"Warning. Under Intern hours: {timesheet?.UserName}";
                        //send email of error
                        Email(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Approve(timesheet);
                    }
                }
                else if (ConfigurationManager.AppSettings["Unbillable"].Split(',').Contains(timesheet.UserName))
                {
                    if (timesheet.DayTotals.Sum() < 40)
                    {
                        var message = $"Warning. Under full hours as unbillable: {timesheet.UserName}";
                        //send email of warning
                        Email(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Approve(timesheet);
                    }
                }
                else
                {
                    if (GetBillableHours(timesheet) < 40)
                    {
                        var message = $"Warning. Under billable hours: {timesheet.UserName}";
                        //send email of invalid timesheet
                        Email(message);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Approve(timesheet);
                    }
                }
            }
        }

        private static void Approve(TimeSheet timesheet)
        {
            if (!ClickTime.Approve(timesheet))
            {
                //send email of error
                Console.WriteLine($"There was an issue trying to approve the time sheet. {timesheet.TimeSheetId} - {timesheet.UserName}");
            }
            else
            {
                Console.WriteLine($"Time sheet approved. {timesheet.TimeSheetId} - {timesheet.UserName}");
            }
        }

        private static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }

        private static decimal GetBillableHours(TimeSheet timeSheet)
        {
            var timeEntyGroups = ClickTime.GetTimeEntries(timeSheet);
            decimal billable = 0;
            foreach (var timeEntryGroup in timeEntyGroups)
            {
                billable += timeEntryGroup.TimeEntries
                    .Where(te => !te.OptionalData
                        .Any(od => (od.Key == "JobName" && od.Value == "INT Xpanxion Internal") || (od.Key == "TaskName" && od.Value.Contains("Unbillable"))))
                    .Sum(timeEntry => timeEntry.Hours);
                billable += timeEntryGroup.TimeOffEntries.Sum(ptoEntry => ptoEntry.Hours);
            }
            return billable;
        }

        private static void Email(string message)
        {
            var msg = new MailMessage();
            msg.To.Add(new MailAddress(_email));
            msg.From = new MailAddress(_email);
            msg.Subject = "Automated ClickTime Approval Error";
            msg.Body = message;
            msg.IsBodyHtml = false;
            var client = new SmtpClient();
            client.Host = "smtp.office365.com";
            client.Credentials = new System.Net.NetworkCredential(_email, _password);
            client.Port = 587;
            client.EnableSsl = true;
            client.Send(msg);
        }
    }
}
