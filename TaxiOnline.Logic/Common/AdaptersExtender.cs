using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Factories;

namespace TaxiOnline.Logic.Common
{
    public abstract class AdaptersExtender
    {
        private readonly IServicesFactory _servicesFactory;

        public IServicesFactory ServicesFactory
        {
            get { return _servicesFactory; }
        }

        public AdaptersExtender(IServicesFactory servicesFactory)
        {
            _servicesFactory = servicesFactory;
        }
    }
}
