/*
 *
 * (c) Copyright Ascensio System Limited 2010-2015
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.MessagingSystem;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using AjaxPro;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using Resources;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    [AjaxNamespace("PwdTool")]
    public partial class PwdTool : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/PwdTool.ascx"; }
        }

        public Guid UserID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _pwdRemainderContainer.Options.IsPopup = true;
            _pwdRemainderContainer.Options.InfoMessageText = "";
            _pwdRemainderContainer.Options.InfoType = InfoType.Info;

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/UserProfile/js/pwdtool.js"));

            AjaxPro.Utility.RegisterTypeForAjax(GetType());
        }

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse RemindPwd(string email)
        {
            var response = new AjaxResponse {rs1 = "0"};

            if (!email.TestEmailRegex())
            {
                response.rs2 = "<div>" + Resource.ErrorNotCorrectEmail + "</div>";
                return response;
            }

            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant != null)
            {
                var settings = SettingsManager.Instance.LoadSettings<IPRestrictionsSettings>(tenant.TenantId);
                if (settings.Enable && !IPSecurity.IPSecurity.Verify(tenant.TenantId))
                {
                    response.rs2 = "<div>" + Resource.ErrorAccessRestricted + "</div>";
                    return response;
                }
            }

            try
            {
                UserManagerWrapper.SendUserPassword(email);

                response.rs1 = "1";
                response.rs2 = String.Format(Resource.MessageYourPasswordSuccessfullySendedToEmail, "<b>" + email + "</b>");
                var userInfo = CoreContext.UserManager.GetUserByEmail(email);

                if (userInfo.Sid != null)
                {
                    response.rs2 = "<div>" + Resource.CouldNotRecoverPasswordForLdapUser + "</div>";
                    return response;
                }

                string displayUserName = userInfo.DisplayUserName(false);

                MessageService.Send(HttpContext.Current.Request, MessageAction.UserSentPasswordChangeInstructions, displayUserName);
            }
            catch(Exception ex)
            {
                response.rs2 = "<div>" + HttpUtility.HtmlEncode(ex.Message) + "</div>";
            }

            return response;
        }
    }
}