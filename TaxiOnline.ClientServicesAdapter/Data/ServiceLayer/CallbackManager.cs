using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer
{
    public class CallbackManager
    {
        private readonly ConcurrentDictionary<Type, Delegate> _callbacksCache = new ConcurrentDictionary<Type, Delegate>();

        public Action<TDataContract> GetContractUpdatedDelegate<TDataContract, TInterface>(EventHandler<ValueEventArgs<TInterface>> handler, Func<TDataContract, TInterface> convertDelegate)
        {
            return (Action<TDataContract>)_callbacksCache.GetOrAdd(typeof(TInterface), new Func<Type, Delegate>(t => CreateCallback<TDataContract, TInterface>(handler, convertDelegate)));
        }

        private Action<TDataContract> CreateCallback<TDataContract, TInterface>(EventHandler<ValueEventArgs<TInterface>> handler, Func<TDataContract, TInterface> convertDelegate)
        {
            return data => BeginInvoke(() => handler(this, new ValueEventArgs<TInterface>(convertDelegate(data))));
        }

        private void BeginInvoke(Action delegateToInvoke)
        {
            Task.Factory.StartNew(delegateToInvoke);
        }
    }
}
