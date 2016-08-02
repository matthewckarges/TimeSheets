using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClickTimeAPI.Models;
using ClickTimeAPI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClickTimeAPI
{
    public static class ClickTime
    {
        private static string _email;
        private static string _password;
        private static string _companyId;
        private static string _userId;

        public static bool Initialize(string email, string password)
        {
            _email = email;
            _password = password;
            try
            {
                var session = GetSession();
                if (session == null) return false;
                _companyId = session.CompanyId;
                _userId = session.UserId;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<TimeSheet> GetPendingTimeSheets()
        {
            return Send<List<TimeSheet>>($"Companies/{_companyId}/Users/{_userId}/TimesheetsToApprove");
        }

        public static Session GetSession()
        {
            return Send<Session>("Session");
        }

        public static List<TimeEntryGroup> GetTimeEntries(TimeSheet sheet)
        {
            var response = Send<List<TimeEntryGroup>>($"Companies/{_companyId}/Users/{sheet.UserId}/TimeEntries?startdate={sheet.StartDate.ToString("yyyyMMdd")}&enddate={sheet.EndDate.ToString("yyyyMMdd")}");

            return response;
        }

        public static bool Approve(TimeSheet sheet)
        {
            sheet.Status = TimeSheetStatus.Approved;
            var data = JsonConvert.SerializeObject(new
            {
                sheet.DayTotals,
                sheet.EndDate,
                sheet.IncompleteDays,
                sheet.StartDate,
                sheet.Status,
                sheet.TimeSheetId
            }, new IsoDateTimeConverter {DateTimeFormat = "yyyyMMdd"}, new StringEnumConverter());
            object ignore;
            return Send($"Companies/{_companyId}/Users/{sheet.UserId}/Timesheets/{sheet.TimeSheetId}?comment=", out ignore, "POST", data);
        }

        public static List<User> GetUsersToApprove()
        {
            return Send<List<User>>($"Companies/{_companyId}/Users/{_userId}/Approve/Users");
        }

        public static T Send<T>(string call, string method = "GET", string serializedData = null) where T : class
        {
            T result = null;
            if (Send(call, out result, method, serializedData))
            {
                return result;
            }
            return null;
        }

        public static bool Send<T>(string call, out T result, string method = "GET", string serializedData = null) where T : class
        {
            result = null;
            var request = WebRequest.Create($"https://app.clicktime.com/API/1.3/{call}");
            request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(new ASCIIEncoding().GetBytes($"{_email}:{_password}"))}");
            request.Method = method;

            request.ContentType = "application/json";

            if (method.ToUpper() == "POST" && serializedData != null)
            {
                var bytes = new ASCIIEncoding().GetBytes(serializedData);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            var json = reader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<T>(json, new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" });
                        }
                    }
                }
                return true;
            }
            catch (WebException webEx)
            {
                var response = "";
                using (var stream = webEx.Response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            response = reader.ReadToEnd();
                        }
                    }
                }
                return false;
            }
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }
}
