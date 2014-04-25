using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Logic
{
    internal abstract class ProfileLogic
    {
        private readonly ProfileModel _profileModel;
        protected readonly AdaptersExtender _adaptersExtender;
        protected readonly CityLogic _city;

        public ProfileModel ProfileModel
        {
            get { return _profileModel; }
        }

        public ProfileLogic(ProfileModel model, AdaptersExtender adaptersExtender, CityLogic city)
        {
            _profileModel = model;
            _adaptersExtender = adaptersExtender;
            _city = city;
            UpdateCurrentLocation();
        }

        private void UpdateCurrentLocation()
        {
            ActionResult<MapPoint> locationResult = _adaptersExtender.ServicesFactory.GetCurrentHardwareService().GetCurrentLocation();
            if (locationResult.IsValid)
                _profileModel.CurrentLocation = locationResult.Result;
        }
    }
}
