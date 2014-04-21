using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Data.DataAccess
{
    public class PersonAccountDA
    {
        public virtual Guid Id { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string SkypeNumber { get; set; }
    }
}
