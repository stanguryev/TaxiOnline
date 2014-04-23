using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class PedestrianRequestModel
    {
        private readonly PedestrianModel _requestAuthor;
        private Guid _requestId;
        private decimal _paymentAmount;
        private string _currency = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
        private MapLocationModel _target = new MapLocationModel();
        private bool _isCancelled;

        public Guid AuthorId
        {
            get { return _requestAuthor.PersonId; }
        }

        public Guid RequestId
        {
            get { return _requestId; }
            internal set { _requestId = value; }
        }

        public MapLocationModel Target
        {
            get { return _target; }
        }

        public decimal PaymentAmount
        {
            get { return _paymentAmount; }
            set { _paymentAmount = value; }
        }

        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        public bool IsCancelled
        {
            get { return _isCancelled; }
            internal set { _isCancelled = value; }
        }

        internal PedestrianRequestModel(PedestrianModel requestAuthor)
        {
            _requestAuthor = requestAuthor;
        }
    }
}
