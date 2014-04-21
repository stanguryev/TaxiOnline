using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public abstract class PersonModel
    {
        private Guid _personId;
        private MapPoint _currentLocation;
        private string _skypeId;
        private string _phoneNumber;

        public Guid PersonId
        {
            get { return _personId; }
            internal set { _personId = value; }
        }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            internal set { _currentLocation = value; }
        }

        public string SkypeNumber
        {
            get { return _skypeId; }
            internal set { _skypeId = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            internal set { _phoneNumber = value; }
        }

        public PersonModel()
        {
            
        }
    }
}
