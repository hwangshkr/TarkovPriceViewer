using System;
using System.Net;
using System.Text;

namespace TarkovPriceViewer
{
    class TPVWebClient : WebClient
    {
        private int timeout = 5000;

        public TPVWebClient()
        {
            this.Encoding = Encoding.UTF8;
            this.Proxy = null;
        }
        public TPVWebClient(int _timeout)
        {
            this.Encoding = Encoding.UTF8;
            this.Proxy = null;
            this.timeout = _timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = timeout;

            return request;
        }
    }
}
