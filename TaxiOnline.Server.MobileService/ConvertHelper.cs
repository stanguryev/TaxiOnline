using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.Server.MobileService
{
    internal class ConvertHelper
    {
        public static CityDataContract CreateCityDataContract(ICityInfo city)
        {
            return new CityDataContract
            {
                Id = city.Id,
                Name = city.Name,
                InitialLatitude = city.InitialLatitude,
                InitialLongitude = city.InitialLongitude,
                InitialZoom = city.InitialZoom
            };
        }

        public static PedestrianDataContract CreatePedestrianDataContract(IPedestrianInfo pedestrianInfo)
        {
            return new PedestrianDataContract
            {
                PersonId = pedestrianInfo.Id,
                PhoneNumber = pedestrianInfo.PhoneNumber,
                SkypeNumber = pedestrianInfo.SkypeNumber,
                CurrentLocationLatidude = pedestrianInfo.CurrentLocationLatidude,
                CurrentLocationLongitude = pedestrianInfo.CurrentLocationLongidude,
                IsOnline = pedestrianInfo.IsOnline
            };
        }

        public static DriverDataContract CreateDriverDataContract(IDriverInfo driverInfo)
        {
            return new DriverDataContract
            {
                PersonId = driverInfo.Id,
                PhoneNumber = driverInfo.PhoneNumber,
                SkypeNumber = driverInfo.SkypeNumber,
                CarColor = driverInfo.CarColor,
                CurrentLocationLatidude = driverInfo.CurrentLocationLatidude,
                CurrentLocationLongitude = driverInfo.CurrentLocationLongidude,
                IsOnline = driverInfo.IsOnline
            };
        }

        public static void FillPedestrianAuthenticationRequestInfo(IPedestrianInfo pedestrianInfo, PedestrianAuthenticationRequestDataContract request)
        {
            pedestrianInfo.PhoneNumber = request.PhoneNumber;
            pedestrianInfo.SkypeNumber = request.SkypeNumber;
        }

        public static void FillDriverAuthenticationRequestInfo(IDriverInfo driverInfo, DriverAuthenticationRequestDataContract request)
        {
            driverInfo.PhoneNumber = request.PhoneNumber;
            driverInfo.SkypeNumber = request.SkypeNumber;
            driverInfo.CarColor = request.CarColor;
        }
    }
}
