using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaxiOnline.Server.DataAccess;

namespace TaxiOnline.Server.Models
{
    public class CityModel
    {
        private City _cityDA;
        public Guid Id
        {
            get { return _cityDA.Id; }
        }

        public double InitialLatitude
        {
            get { return _cityDA.InitialLatitude; }
        }

        public double InitialLongitude
        {
            get { return _cityDA.InitialLongitude; }
        }

        public double InitialZoom
        {
            get { return _cityDA.InitialZoom; }
        }

        public string PhoneConstraintPattern
        {
            get { return _cityDA.PhoneConstraintPattern; }
        }

        public string PhoneCorrectionPattern
        {
            get { return _cityDA.PhoneCorrectionPattern; }
        }

        public CityModel(City cityDA)
        {
            _cityDA = cityDA;
        }

        public void LoadDb()
        {
            throw new NotImplementedException();
        }
    }
}