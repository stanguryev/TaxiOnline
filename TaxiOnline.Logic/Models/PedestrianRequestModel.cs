using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Logic.Helpers;
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
        private string _comment;
        private bool _isCancelled;

        public Guid AuthorId
        {
            get { return _requestAuthor.PersonId; }
        }

        public PedestrianModel RequestAuthor
        {
            get { return _requestAuthor; }
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

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public bool IsCancelled
        {
            get { return _isCancelled; }
            internal set { _isCancelled = value; }
        }

        internal Func<ActionResult<Logic.DriverProfileResponseLogic>> InitResponseDelegate { get; set; }

        internal PedestrianRequestModel(PedestrianModel requestAuthor)
        {
            _requestAuthor = requestAuthor;
        }

        public ActionResult<DriverProfileResponseModel> InitResponse()
        {
            return UpdateHelper.GetModel<DriverProfileResponseModel, Logic.DriverProfileResponseLogic>(InitResponseDelegate, l => l.Model);
        }
    }
}
