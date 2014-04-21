using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.Logic.Helpers;
using TaxiOnline.Logic.Logic;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;

namespace TaxiOnline.Logic.Models
{
    public class CityModel
    {
        private readonly MapModel _map;
        private readonly SimpleCollectionLoadDecorator<PersonModel> _persons;
        private Guid _id;
        private string _name;
        private MapPoint _initialCenter;
        private double _initialZoom;

        public Guid Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        public MapPoint InitialCenter
        {
            get { return _initialCenter; }
            internal set { _initialCenter = value; }
        }

        public double InitialZoom
        {
            get { return _initialZoom; }
            internal set { _initialZoom = value; }
        }

        public MapModel Map
        {
            get { return _map; }
        }

        public IEnumerable<PersonModel> Persons
        {
            get { return _persons.Items; }
        }

        public event ActionResultEventHandler AuthenticationFailed;

        public event EventHandler PersonsChanged
        {
            add { _persons.ItemsChanged += value; }
            remove { _persons.ItemsChanged -= value; }
        }

        public event NotifyCollectionChangedEventHandler PersonsCollectionChanged
        {
            add { _persons.ItemsCollectionChanged += value; }
            remove { _persons.ItemsCollectionChanged -= value; }
        }

        public event ActionResultEventHandler PersonsRequestFailed
        {
            add { _persons.RequestFailed += value; }
            remove { _persons.RequestFailed -= value; }
        }

        internal Func<AuthenticationRequestModel, ActionResult<ProfileLogic>> AuthenticateDelegate;

        internal Func<ActionResult<IEnumerable<Logic.PersonLogic>>> EnumeratePersonsDelegate;

        public CityModel(MapModel map)
        {
            _map = map;
            _persons = new SimpleCollectionLoadDecorator<PersonModel>(EnumeratePersons);
        }

        public AuthenticationRequestModel CreateAuthenticationRequest(ParticipantTypes requestType)
        {
            switch (requestType)
            {
                case ParticipantTypes.Driver:
                    return new DriverAuthenticationRequestModel();
                    break;
                case ParticipantTypes.Pedestrian:
                    return new PedestrianAuthenticationRequestModel();
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void BeginAuthenticate(AuthenticationRequestModel request)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Func<AuthenticationRequestModel, ActionResult<ProfileLogic>> handler = AuthenticateDelegate;
                if (handler != null)
                {
                    ActionResult<ProfileLogic> authResult = handler(request);
                    if (!authResult.IsValid)
                        OnAuthenticationFailed(ActionResult.GetErrorResult(authResult));
                }
            });
        }

        public void BeginLoadPersons()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => _persons.FillItemsList());
        }

        private ActionResult<IEnumerable<PersonModel>> EnumeratePersons()
        {
            return UpdateHelper.EnumerateModels(EnumeratePersonsDelegate, l => l.PersonModel);
        }

        protected virtual void OnAuthenticationFailed(ActionResult errorResult)
        {
            ActionResultEventHandler handler = AuthenticationFailed;
            if (handler != null)
                handler(this, new ActionResultEventArgs(errorResult));
        }
    }
}
