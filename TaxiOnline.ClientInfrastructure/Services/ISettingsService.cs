using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Settings;

namespace TaxiOnline.ClientInfrastructure.Services
{
    public interface ISettingsService
    {
        ISettings CurrentSettings { get; }
        void LoadSettings();
        void SaveSettings();
    }
}
