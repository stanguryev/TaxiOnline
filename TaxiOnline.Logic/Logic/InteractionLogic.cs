using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Logic.Common.Exceptions;
using TaxiOnline.Logic.Models;
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Collections.Special;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Logic
{
    internal class InteractionLogic
    {
        private readonly InteractionModel _model;
        private readonly MapLogic _map;
        private readonly SettingsLogic _settings;
        private readonly AdaptersExtender _adaptersExtender;
        private readonly UpdatableCollectionLoadDecorator<CityLogic, ICityInfo> _cities;
        private ProfileLogic _currentProfile;

        public ProfileLogic CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                _model.CurrentProfile = value.ProfileModel;
            }
        }

        public SettingsLogic Settings
        {
            get { return _settings; }
        } 

        public MapLogic Map
        {
            get { return _map; }
        } 

        public InteractionLogic(InteractionModel model, AdaptersExtender adaptersExtender)
        {
            _model = model;
            _adaptersExtender = adaptersExtender;
            model.CurrentCityChanged += Model_CurrentCityChanged;
            model.EnumerateCitiesDelegate = EnumerateCities;
            _settings = new SettingsLogic(new SettingsModel(_adaptersExtender.ServicesFactory.GetCurrentSettingsService()), _adaptersExtender, this);
            _settings.Model.LoadSettings();
            _map = new MapLogic(new MapModel(_adaptersExtender.ServicesFactory.GetCurrentMapService()), _adaptersExtender, this);
            _adaptersExtender.ServicesFactory.GetCurrentDataService().ConnectionStateChanged += InteractionLogic_ConnectionStateChanged;
            _cities = new UpdatableCollectionLoadDecorator<CityLogic, ICityInfo>(RetriveCities, CompareCityInfo, c => true, CreateCityLogic);
            _cities.RequestFailed += Cities_RequestFailed;
            UpdateConnectionStateInfo();
        }

        private ActionResult<IEnumerable<CityLogic>> EnumerateCities()
        {
            if (_cities.Items == null)
                _cities.FillItemsList();
            return _cities.Items == null ? ActionResult<IEnumerable<CityLogic>>.GetErrorResult(new DataServiceException(new Exception())): ActionResult<IEnumerable<CityLogic>>.GetValidResult(_cities.Items);
        }

        private ActionResult<IEnumerable<CityLogic>> RetriveCities()
        {
            ActionResult<IEnumerable<ICityInfo>> requestResult = _adaptersExtender.ServicesFactory.GetCurrentDataService().EnumerateCities(CultureInfo.CurrentCulture.Name);
            return requestResult.IsValid ? ActionResult<IEnumerable<CityLogic>>.GetValidResult(requestResult.Result.Select(c => CreateCityLogic(c)).ToArray())
                : ActionResult<IEnumerable<CityLogic>>.GetErrorResult(requestResult); ;
        }

        private bool CompareCityInfo(CityLogic logic, ICityInfo slo)
        {
            return logic.Model.Id == slo.Id;
        }

        private CityLogic CreateCityLogic(ICityInfo citySLO)
        {
            return new CityLogic(new CityModel(_map.Model)
            {
                Id = citySLO.Id,
                Name = citySLO.Name,
                InitialCenter = new ClientInfrastructure.Data.MapPoint(citySLO.InitialLatitude, citySLO.InitialLongitude),
                InitialZoom = citySLO.InitialZoom
            }, _adaptersExtender, this);
        }

        private void UpdateConnectionStateInfo()
        {
            _model.ConnectionState = _adaptersExtender.ServicesFactory.GetCurrentDataService().ConnectionState;
        }

        private void InteractionLogic_ConnectionStateChanged(object sender, EventArgs e)
        {
            UpdateConnectionStateInfo();
        }

        private void Cities_RequestFailed(object sender, ActionResultEventArgs e)
        {
            _model.NotifyEnumrateCitiesFailed(e.Result);
        }

        private void Model_CurrentCityChanged(object sender, EventArgs e)
        {
            CityModel currentCity = _model.CurrentCity;
            if (currentCity != null)
                currentCity.SetMapInitials();
        }
    }
}
