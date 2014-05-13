using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class DriverResponseDA
    {
        public virtual Guid Id { get; set; }
        public virtual PedestrianRequestDA Request { get; set; }
        public virtual bool IsAccepted { get; set; }
    }
}
