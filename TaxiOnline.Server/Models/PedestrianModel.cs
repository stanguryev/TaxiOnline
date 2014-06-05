﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Models
{
    public class PedestrianModel 
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }

        public string SkypeNumber { get; set; }

        public Guid CityId { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Altitude { get; set; }

        public CityModel City { get; set; }
    }
}
