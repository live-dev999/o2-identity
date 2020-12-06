using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace O2.Identity.Web.Views.Manage
{
    public static class ManageNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Index => "Index";
        public static string GetUsers=> "GetUsers";

        public static string ChangePassword => "ChangePassword";

        public static string ExternalLogins => "ExternalLogins";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";
        public static string PaymentsAndBulling => "PaymentsAndBulling";
        public static string ServicesAndSubscriptions { get; set; }
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);
    
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetUsers);

        public static string PaymentAndBillingNavClass(ViewContext viewContext) => PageNavClass(viewContext, PaymentsAndBulling);

        public static object ServicesAndSubscriptionsNavClass(ViewContext viewContext)=> PageNavClass(viewContext, ServicesAndSubscriptions);
        
    }
}
