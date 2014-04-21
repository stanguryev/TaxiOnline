using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class PedestrianInfo : PersonInfo, IPedestrianInfo
    {
        public PedestrianInfo(Guid id)
            : base(id)
        {
            
        }
    }
}
