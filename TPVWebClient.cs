using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TarkovPriceViewer
{
    class TPVWebClient : WebClient
    {
        public TPVWebClient()
        {
            this.Encoding = Encoding.UTF8;
            this.Proxy = null;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            request.Timeout = 5000;

            return request;
        }
    }
}
