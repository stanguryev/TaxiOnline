using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public abstract class AuthenticationRequestModel
    {
        private string _deviceId;
        private MapPoint _currentLocation;
        private string _skypeNumber;
        private string _phoneNumber;

        public abstract ParticipantTypes ParticipantType { get; }

        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; }
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
        
        internal AuthenticationRequestModel()
        {

        }
    }
}
