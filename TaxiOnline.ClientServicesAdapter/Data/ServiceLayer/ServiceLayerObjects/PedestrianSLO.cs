using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ClientServicesAdapter.Data.DataObjects;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class PedestrianSLO : PersonSLO, IPedestrianInfo
    {
        public PedestrianSLO(PedestrianDataContract dataContract)
            : base(dataContract)
        {

        }

        public PedestrianSLO(PedestrianDTO dto)
            : base(new PersonDataContract
            {
                CurrentLocationLatidude = dto.Latitude.Value,
                CurrentLocationLongitude = dto.Longitude.Value,
                IsOnline = true,
                PersonId = Guid.Parse(dto.Id),
                PhoneNumber = dto.PhoneNumber,
                SkypeNumber = dto.SkypeNumber
            })
        {

        }
    }
}
