using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.Authentication
{
    public class TaxiOnlineUser : System.Security.Principal.IPrincipal
    {
        private readonly TaxiOnlineIdentity _identity;

        public System.Security.Principal.IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            return role == TaxiOnlineIdentity.AuthenticationTypeName;
        }

        public TaxiOnlineUser(AuthenticationRequestDTO request)
        {
            _identity = new TaxiOnlineIdentity(request);
        }
    }
}