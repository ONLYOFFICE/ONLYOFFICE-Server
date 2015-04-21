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
using System.Diagnostics;
using System.Web;
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using ASC.Core;
using log4net;

namespace ASC.Specific.GloabalFilters.Analytics
{
    public class MethodAccessCsvLogFilter : ApiCallFilter
    {
        private readonly log4net.ILog _loger;

        public MethodAccessCsvLogFilter()
        {
            _loger = LogManager.GetLogger("ASC.Api.Analytics");
        }


        internal class LogEntry
        {
            internal enum Actions
            {
                BeforeCall,
                AfterCall,
                ErrorCall
            }

            internal string HostAddress { get; set; }
            internal Uri Referer { get; set; }
            internal string HttpMethod { get; set; }
            internal Uri Url { get; set; }
            internal string ApiRoute { get; set; }
            internal int TenantId { get; set; }
            internal Guid UserId { get; set; }
            internal Actions Action { get; set; }
            internal double ExecutionTime { get; set; }
            internal Exception Error { get; set; }

            internal LogEntry(Actions action)
            {
                Action = action;
            }
        }

        public Stopwatch Sw
        {
            get
            {
                var sw = new Stopwatch();
                if (HttpContext.Current!=null)
                {
                    sw = HttpContext.Current.Items["apiSwProfile"] as Stopwatch;
                    if (sw==null)
                    {
                        sw = new Stopwatch();
                        HttpContext.Current.Items["apiSwProfile"] = sw;
                    }
                }
                return sw;
            }
        }

        public override void PreMethodCall(Api.Interfaces.IApiMethodCall method, ASC.Api.Impl.ApiContext context, System.Collections.Generic.IEnumerable<object> arguments)
        {
            //Log method call
            TryLog(LogEntry.Actions.BeforeCall,method,context, null);
            base.PreMethodCall(method, context, arguments);
            Sw.Start();
        }

        private void TryLog(LogEntry.Actions action, IApiMethodCall method, ApiContext context, Exception exception)
        {
            try
            {
                ThreadContext.Properties["HostAddress"] = context.RequestContext.HttpContext.Request.UserHostAddress;
                ThreadContext.Properties["Referer"] = context.RequestContext.HttpContext.Request.UrlReferrer;
                ThreadContext.Properties["HttpMethod"] = method.HttpMethod;
                ThreadContext.Properties["ApiRoute"] = method.FullPath;
                ThreadContext.Properties["Url"] = context.RequestContext.HttpContext.Request.GetUrlRewriter();
                ThreadContext.Properties["TenantId"] = CoreContext.TenantManager.GetCurrentTenant(false).TenantId;
                ThreadContext.Properties["UserId"] = SecurityContext.CurrentAccount.ID;
                ThreadContext.Properties["ExecutionTime"] = Sw.ElapsedMilliseconds;
                ThreadContext.Properties["Error"] = exception;
                ThreadContext.Properties["Action"] = action;
                _loger.Debug("log");
            }
            catch
            {
            }
        }

        public override void PostMethodCall(IApiMethodCall method, ASC.Api.Impl.ApiContext context, object methodResponce)
        {
            Sw.Stop();
            TryLog(LogEntry.Actions.AfterCall, method, context, null);
            base.PostMethodCall(method, context, methodResponce);
        }

        public override void ErrorMethodCall(IApiMethodCall method, ASC.Api.Impl.ApiContext context, Exception e)
        {
            Sw.Stop();
            TryLog(LogEntry.Actions.ErrorCall, method, context,e);
            base.ErrorMethodCall(method, context, e);
        }
    }
}