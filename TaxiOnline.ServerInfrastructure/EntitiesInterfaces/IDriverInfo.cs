using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface IDriverInfo: IPersonInfo
    {
        string CarColor { get; set; }
    }
}
