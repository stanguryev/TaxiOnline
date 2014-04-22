using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class PersonInfoDA
    {
        public virtual int Id { get; set; }
        public virtual PersonAccountDA Person { get; set; }
        public virtual CityDA City { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }
}
