using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class DriverInfo : PersonInfo, IDriverInfo
    {
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;

        public string PersonName
        {
            get { return _personName; }
            set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            set { _carNumber = value; }
        }

        public DriverInfo(Guid id)
            : base(id)
        {
            
        }
    }
}
