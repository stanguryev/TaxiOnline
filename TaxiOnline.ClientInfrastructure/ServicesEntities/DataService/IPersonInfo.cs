using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IPersonInfo
    {
        Guid PersonId { get; }
        MapPoint CurrentLocation { get; }
        string PhoneNumber { get; }
        string SkypeNumber { get; }
        bool IsOnline { get; }
    }
}
