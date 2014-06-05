using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.Authentication
{
    public class TaxiOnlineAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null || actionContext.Request.Headers.Authorization.Scheme != TaxiOnlineIdentity.AuthenticationTypeName || string.IsNullOrEmpty(actionContext.Request.Headers.Authorization.Parameter))
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }
            AuthenticationRequestDTO request = JsonConvert.DeserializeObject<AuthenticationRequestDTO>(actionContext.Request.Headers.Authorization.Parameter);
            System.Threading.Thread.CurrentPrincipal = new TaxiOnlineUser(request);
        }
    }
}