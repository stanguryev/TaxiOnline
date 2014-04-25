using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Settings;

namespace TaxiOnline.ClientServicesAdapter.Settings
{
    public abstract class SettingsAdapter : ISettingsService
    {
        protected SettingsObject _settingsObject = new SettingsObject();

        public ISettings CurrentSettings
        {
            get { return _settingsObject; }
        }

        public abstract void LoadSettings();

        public abstract void SaveSettings();
    }
}
