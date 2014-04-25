using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Settings;

namespace TaxiOnline.Logic.Models
{
    public class SettingsModel
    {
        private ISettingsService _settingsService;

        public ISettings CurrentSettings
        {
            get { return _settingsService.CurrentSettings; }
        }

        public SettingsModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void LoadSettings()
        {
            _settingsService.LoadSettings();
        }

        public void SaveSettings()
        {
            _settingsService.SaveSettings();
        }
    }
}
