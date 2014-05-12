using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class PedestrianRequestDA
    {
        public virtual Guid Id { get; set; }
        public virtual PedestrianInfoDA Author { get; set; }
        public virtual DriverInfoDA Target { get; set; }
        public virtual string Comment { get; set; }
    }
}
