using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Logic.Models
{
    public abstract class ProfileModel
    {
        private MapModel _map;
        private Guid _personId;
        private string _skypeId;
        private string _phoneNumber;

        public Guid PersonId
        {
            get { return _personId; }
            internal set { _personId = value; }
        }

        public string SkypeId
        {
            get { return _skypeId; }
            set { _skypeId = value; }
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
    }
}
