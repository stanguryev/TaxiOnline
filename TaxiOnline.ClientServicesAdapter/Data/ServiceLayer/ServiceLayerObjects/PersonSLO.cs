using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal abstract class PersonSLO : IPersonInfo
    {
        private Guid _personId;
        private bool _isOnline;

        public Guid PersonId
        {
            get { return _personId; }
            internal set { _personId = value; }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
            internal set { _isOnline = value; }
        }
    }
}
