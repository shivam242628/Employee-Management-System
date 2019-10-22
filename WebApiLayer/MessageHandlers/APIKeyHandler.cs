using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiLayer.MessageHandlers
{
    public class APIKeyHandler : DelegatingHandler
    {
        #region Data Members
        private string yourApiKey = ConfigurationManager.AppSettings["APIKey"].ToString();
        #endregion

        #region APIValidation Function
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isValidAPIKey = false;
            IEnumerable<string> lsHeaders;
            var checkApiKeyExists = request.Headers.TryGetValues("API_KEY", out lsHeaders);
            if (checkApiKeyExists)
            {
                if (lsHeaders.FirstOrDefault().Equals(yourApiKey))
                { 
                    isValidAPIKey = true;
                }
            }
            if (!isValidAPIKey)
                return request.CreateResponse(HttpStatusCode.Forbidden, "Bad API Key");
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        #endregion
    }
}