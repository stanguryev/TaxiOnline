using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.Authentication
{
    public class TaxiOnlineIdentity : System.Security.Principal.IIdentity
    {
        public const string AuthenticationTypeName = "TaxiOnline";
        private readonly string _name;

        public string AuthenticationType
        {
            get { return AuthenticationTypeName; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return _name; }
        }

        public TaxiOnlineIdentity(AuthenticationRequestDTO request)
        {
            _name = request.Id;
        }
    }
}