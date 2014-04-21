using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class DriverInfo : PersonInfo, IDriverInfo
    {
        private string _carColor;

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        public DriverInfo(Guid id)
            : base(id)
        {

        }

    }
}
