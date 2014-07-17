using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.Decorators
{
    public interface IUpdateInfo
    {
        DateTimeOffset? UpdatedAt { get; }
    }
}
