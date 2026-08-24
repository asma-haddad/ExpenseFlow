using ExpenseFlow.Application.Abstraction;

namespace ExpenseFlow.Application.Features.Dashboard.Auth.Login
{
    public class LoginCommand
    {
        public class Request : ICommand<Response>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public SessionDto Session { get; set; }

        }
        public class Response
        {
            public string Token { get; set; }


        }
        public class SessionDto
        {
            public string IP { get; set; }
            public string BrowserInfo { get; set; }

            public double Lat { get; set; }
            public double Long { get; set; }
            public int Network { get; set; }
            public string OsVersion { get; set; }
            public string Brand { get; set; }
            public string TimeZone { get; set; }
            public string FcmToken { get; set; }
            public string DeviceId { get; set; }
            public string Device { get; set; }
        }
    }
}
