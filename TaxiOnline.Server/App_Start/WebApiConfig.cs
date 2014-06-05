using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Server.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using TaxiOnline.Server.App_Start;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            AutoMapper.Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<City, CityModel>();
                cfg.CreateMap<PedestriansInfo, PedestrianDTO>()
                    .ForMember(dst => dst.Id, map => map.MapFrom(c => c.PersonsInfo.PersonId.ToString()))
                    .ForMember(dst => dst.CityId, map => map.MapFrom(c => c.PersonsInfo.CityId))
                    .ForMember(dst => dst.Latitude, map => map.MapFrom(c => c.PersonsInfo.Latitude))
                    .ForMember(dst => dst.Longitude, map => map.MapFrom(c => c.PersonsInfo.Longitude))
                    .ForMember(dst => dst.Altitude, map => map.MapFrom(c => c.PersonsInfo.Altitude))
                    .ForMember(dst => dst.PhoneNumber, map => map.MapFrom(c => c.PersonsInfo.PersonAccount.PhoneNumber))
                    .ForMember(dst => dst.SkypeNumber, map => map.MapFrom(c => c.PersonsInfo.PersonAccount.SkypeNumber));
            });

            Database.SetInitializer(new DatabaseModelInitializer());
        }
    }
}

