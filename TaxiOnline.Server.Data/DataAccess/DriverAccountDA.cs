using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class DriverAccountDA
    {
        public virtual int Id { get; set; }
        public virtual PersonAccountDA Person { get; set; }
        public virtual string PersonName { get; set; }
        public virtual string CarColor { get; set; }
        public virtual string CarBrand { get; set; }
        public virtual string CarNumber { get; set; }
    }
}
