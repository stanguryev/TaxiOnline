using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public class MapLocationModel
    {
        private MapPoint _location;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public MapPoint Location
        {
            get { return _location; }
            set { _location = value; }
        }
    }
}
