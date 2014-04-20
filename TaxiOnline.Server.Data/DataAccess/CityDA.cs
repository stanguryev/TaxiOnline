using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class CityDA
    {
        public virtual Guid Id { get; set; }
        public virtual double InitialLatitude { get; set; }
        public virtual double InitialLongitude { get; set; }
        public virtual double InitialZoom { get; set; }
    }
}
