using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class DriverModel : PersonModel
    {
        private string _personName;
        private string _carColor;
        private string _carBrand;
        private string _carNumber;
        private bool _hasAcceptedRequest;

        public string PersonName
        {
            get { return _personName; }
            internal set { _personName = value; }
        }

        public string CarColor
        {
            get { return _carColor; }
            internal set { _carColor = value; }
        }

        public string CarBrand
        {
            get { return _carBrand; }
            internal set { _carBrand = value; }
        }

        public string CarNumber
        {
            get { return _carNumber; }
            internal set { _carNumber = value; }
        }

        public bool HasAcceptedRequest
        {
            get { return _hasAcceptedRequest; }
            internal set
            {
                if (_hasAcceptedRequest != value)
                {
                    _hasAcceptedRequest = value;
                    OnHasAcceptedRequestChanged();
                }
            }
        }

        public event EventHandler HasAcceptedRequestChanged;

        internal DriverModel(IDriverInfo info)
            : base(info)
        {
            _personName = info.PersonName;
            _carColor = info.CarColor;
            _carBrand = info.CarBrand;
            _carNumber = info.CarNumber;
        }

        protected virtual void OnHasAcceptedRequestChanged()
        {
            EventHandler handler = HasAcceptedRequestChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
