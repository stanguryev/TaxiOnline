using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ServerInfrastructure.EntitiesInterfaces
{
    public interface IPersonInfo
    {
        Guid Id { get; }
        bool IsOnline { get; }
        double CurrentLocationLatidude { get; set; }
        double CurrentLocationLongidude { get; set; }
        string PhoneNumber { get; set; }
        string SkypeNumber { get; set; }
    }
}
