using System;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace MyGlimpse.Core
{
    public class HttpHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var cookie = context.Request.Cookies["myGlimpse"];

            if (cookie == null)
            {
                return;
            }

            var data = Json.Decode<RequestFormatCollection>(cookie.Value);
            var resp = new StringBuilder();

            foreach (var trackedRequest in data.TrackedRequests)
            {
                resp.Append(string.Format("<strong>Request Guid: {0}</strong> from {1} starting {2} until {3}<br/>", trackedRequest.Id, trackedRequest.Uri, trackedRequest.Start, trackedRequest.End));
            }
            context.Response.Write(resp.ToString());
        }

        public bool IsReusable { get; private set; }
    }
}
