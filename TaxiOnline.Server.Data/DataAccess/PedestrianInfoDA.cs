using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class PedestrianInfoDA
    {
        public virtual int Id { get; set; }
        public virtual PedestrianAccountDA PedestrianAccount { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }
}
