using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.ClientServicesAdapter.Data.DataObjects
{
    [DataTable("Pedestrians")]
    public class PedestrianDTO
    {
        public string Id { get; set; }

        [UpdatedAt]
        public DateTimeOffset? UpdatedAt { get; set; }

        public string PhoneNumber { get; set; }

        public string SkypeNumber { get; set; }

        public Guid CityId { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Altitude { get; set; }
    }
}
