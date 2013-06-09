using System.Collections.Generic;

namespace MyGlimpse.Core
{
    class RequestFormatCollection
    {
        public RequestFormatCollection()
        {
            TrackedRequests = new List<RequestFormat>();
        }

        public List<RequestFormat> TrackedRequests { get; set; }
    }
}