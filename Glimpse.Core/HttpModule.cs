using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyGlimpse.Core
{
    public class HttpModule : IHttpModule
    {
        List<string> ignoreList = new List<string>();
 
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (ctx, e) => BeginRequest(ctx);
            context.PostAcquireRequestState += (ctx, e) => BeginSessionAccess(ctx);
            context.PostRequestHandlerExecute += (ctx, e) => EndSessionAccess(ctx);
            context.PostReleaseRequestState += (ctx, e) => EndRequest(ctx);
            context.PreSendRequestHeaders += (ctx, e) => SendHeaders(ctx);
            AppDomain.CurrentDomain.DomainUnload += UnloadDomain;

            ignoreList.Add("/myglimpse.axd");
            ignoreList.Add("/favicon.ico");
        }

        private void UnloadDomain(object sender, EventArgs e)
        {
        }

        private void SendHeaders(object sender)
        {
        }

        private void EndRequest(object sender)
        {
            var context = sender as HttpApplication;
            if (Track(context))
            {
                var requestId = context.Response.Headers["RequestId"];

                var data = this.GetData(context.Response.Cookies);
                var trackedRequest = data.TrackedRequests.FirstOrDefault(p => p.Id == requestId);

                if (trackedRequest != null)
                {
                    data.TrackedRequests.Remove(trackedRequest);
                    trackedRequest.End = DateTime.Now.Ticks.ToString();
                    data.TrackedRequests.Add(trackedRequest);
                    this.SetData(context, data);
                }
            }
        }

        private void EndSessionAccess(object sender)
        {
        }

        private void BeginSessionAccess(object sender)
        {
        }

        private void BeginRequest(object sender)
        {
            var context = sender as HttpApplication;
            if (Track(context))
            {
                var requestId = Guid.NewGuid().ToString();
                context.Response.AddHeader("RequestId", requestId);

                var data = this.GetData(context.Request.Cookies);
                data.TrackedRequests.Add(new RequestFormat { Id = requestId, Uri = context.Request.RawUrl, Start = DateTime.Now.Ticks.ToString() });

                this.SetData(context, data);
            }
        }

        private bool Track(HttpApplication context)
        {
            return context != null && ignoreList.All(p => p != context.Request.RawUrl);
        }

        private RequestFormatCollection GetData(HttpCookieCollection cookies)
        {
            var cookie = cookies["myGlimpse"] ?? new HttpCookie("myGlimpse");

            return cookie.Value != null ? Json.Decode<RequestFormatCollection>(cookie.Value) : new RequestFormatCollection();
        }

        private void SetData(HttpApplication context, RequestFormatCollection data)
        {

            context.Response.SetCookie(new HttpCookie("myGlimpse", Json.Encode(data)));
        }

        public void Dispose()
        {

        }
    }
}
