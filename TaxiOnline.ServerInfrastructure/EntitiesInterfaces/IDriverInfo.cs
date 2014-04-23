using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface IDriverInfo : IPersonInfo
    {
        string PersonName { get; set; }
        string CarColor { get; set; }
        string CarBrand { get; set; }
        string CarNumber { get; set; }
    }
}
