using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ServerInfrastructure;

namespace TaxiOnline.Server.Core.Logic
{
    internal class LogicExtender
    {
        private readonly Lazy<ITaxiOnlineStorage> _storage;
        private Func<ITaxiOnlineServer, ITaxiOnlineStorage> _storageInitDelegate;

        public ITaxiOnlineStorage Storage
        {
            get { return _storage.Value; }
        }

        public LogicExtender(TaxiOnlineServer server)
        {
            _storage = new Lazy<ITaxiOnlineStorage>(() => _storageInitDelegate(server), true);
        }
        
        public void InitStorage(Func<ITaxiOnlineServer, ITaxiOnlineStorage> storageInitDelegate)
        {
            _storageInitDelegate = storageInitDelegate;
        }
    }
}
