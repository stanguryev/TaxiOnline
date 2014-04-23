using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class DriverInfoDA
    {
        public virtual int Id { get; set; }
        public virtual PersonInfoDA PersonInfo { get; set; }
    }
}
