using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.DataService
{
    public interface IPedestrianRequest
    {
        Guid Id { get; }
        Guid PedestrianId { get; }
        string TargetName { get; set; }
        MapPoint TargetLocation { get; set; }
        decimal PaymentAmount { get; set; }
        string Currency { get; set; }
        bool IsCanceled { get; set; }
    }
}
