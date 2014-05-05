using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public abstract class ProfileModel
    {
        private MapModel _map;
        private Guid _personId;
        private MapPoint _currentLocation;
        private string _skypeNumber;
        private string _phoneNumber;

        public Guid PersonId
        {
            get { return _personId; }
            internal set { _personId = value; }
        }

        public string SkypeNumber
        {
            get { return _skypeNumber; }
            set { _skypeNumber = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public MapModel Map
        {
            get { return _map; }
            internal set { _map = value; }
        }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            internal set
            {
                _currentLocation = value;
                OnCurrentLocationChanged();
            }
        }

        public event EventHandler CurrentLocationChanged;

        public ProfileModel(MapModel map)
        {
            _map = map;
        }

        protected virtual void OnCurrentLocationChanged()
        {
            EventHandler handler = CurrentLocationChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
