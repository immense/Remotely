using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string ChangePassword => "ChangePassword";

        public static string DownloadPersonalData => "DownloadPersonalData";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";


        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);

        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);
        public static string OptionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, "Options");
        public static string OrganizationNavClass(ViewContext viewContext) => PageNavClass(viewContext, "Organization");
        public static string ApiTokensNavClass(ViewContext viewContext) => PageNavClass(viewContext, "ApiTokens");
        public static string ServerLogsNavClass(ViewContext viewContext) => PageNavClass(viewContext, "ServerLogs");

        public static string ServerConfigNavClass(ViewContext viewContext) => PageNavClass(viewContext, "ServerConfig");
        public static string BrandingNavClass(ViewContext viewContext) => PageNavClass(viewContext, "Branding");

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
