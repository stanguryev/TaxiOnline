using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiOnline.Server.DataObjects
{
    public class PedestrianDTO : EntityData
    {
        public string PhoneNumber { get; set; }

        public string SkypeNumber { get; set; }

        public Guid CityId { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Altitude { get; set; }
    }
}