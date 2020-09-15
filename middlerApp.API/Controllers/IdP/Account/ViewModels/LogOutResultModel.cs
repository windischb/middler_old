using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace middlerApp.API.Controllers.IdP.Account.ViewModels
{
    public class LogOutResultModel
    {
        public string LogoutId { get; set; }
        public bool ShowLogoutPrompt { get; set; }

        public LogOutStatus Status { get; set; }

        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; }
    }

    public enum LogOutStatus
    {
        None,
        Prompt,
        LoggedOut
    }
}
