using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal abstract class PersonSLO : IPersonInfo
    {
        private Guid _personId;
        private bool _isOnline;
        private MapPoint _currentLocation;
        private string _phoneNumber;
        private string _skypeNumber;

        public Guid PersonId
        {
            get { return _personId; }
            internal set { _personId = value; }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
            internal set { _isOnline = value; }
        }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            internal set { _currentLocation = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            internal set { _phoneNumber = value; }
        }

        public string SkypeNumber
        {
            get { return _skypeNumber; }
            internal set { _skypeNumber = value; }
        }

        public PersonSLO(PersonDataContract dataContract)
        {
            PersonId = dataContract.PersonId;
            IsOnline = dataContract.IsOnline;
            CurrentLocation = new MapPoint(dataContract.CurrentLocationLatidude, dataContract.CurrentLocationLongitude);
            PhoneNumber = dataContract.PhoneNumber;
            SkypeNumber = dataContract.SkypeNumber;
        }
    }
}
