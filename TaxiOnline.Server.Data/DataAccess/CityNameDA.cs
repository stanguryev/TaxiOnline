using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class CityNameDA
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string CultureName { get; set; }
        public virtual CityDA City { get; set; }
    }
}
