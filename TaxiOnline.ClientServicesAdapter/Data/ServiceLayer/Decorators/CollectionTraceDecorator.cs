using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientServicesAdapter.Data.Service;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.Patterns;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.Decorators
{

    internal class CollectionTraceDecorator<TDataObject, TResult> where TDataObject : IUpdateInfo
    {
        private readonly AzureMobileServicesProxy _proxy;
        private readonly Func<MobileServiceClient, IMobileServiceTable<TDataObject>> _requestDelegate;
        private readonly Predicate<TDataObject> _filterDelegate;
        private readonly Func<TDataObject, TResult> _convertDelegate;
        private readonly MessagesLoopDecorator<TResult> _resultsLoop;
        private DateTimeOffset? _lastRequest;
        private volatile bool _isActive = true;


        public event EventHandler<ValueEventArgs<TResult>> GotUpdate;

        public CollectionTraceDecorator(AzureMobileServicesProxy proxy, Func<MobileServiceClient, IMobileServiceTable<TDataObject>> requestDelegate, Predicate<TDataObject> filterDelegate, Func<TDataObject, TResult> convertDelegate)
        {
            _proxy = proxy;
            _requestDelegate = requestDelegate;
            _filterDelegate = filterDelegate;
            _convertDelegate = convertDelegate;
            _resultsLoop = new MessagesLoopDecorator<TResult>(r => OnGotUpdate(r), deferreStart: true);
            _resultsLoop.GoingToAwait += ResultsLoop_GoingToAwait;
        }

        public ActionResult<IEnumerable<TResult>> Init()
        {
            ActionResult<IEnumerable<TResult>> outResult = RequestCore();
            _resultsLoop.Start();
            return outResult;
        }

        private ActionResult<IEnumerable<TResult>> RequestCore()
        {
            _proxy.WaitConnectionCompleted();
            MobileServiceClient channel = _proxy.Channel;
            Func<IEnumerable<TDataObject>> requestDelegate = _lastRequest.HasValue ? new Func<IEnumerable<TDataObject>>(() => GetAsyncResult<List<TDataObject>>(() => _requestDelegate(channel).Where(dto => _filterDelegate(dto) && dto.UpdatedAt > _lastRequest).ToListAsync()))
                : new Func<IEnumerable<TDataObject>>(() => GetAsyncResult<List<TDataObject>>(() => _requestDelegate(channel).Where(dto => _filterDelegate(dto)).ToListAsync()));
            ActionResult<IEnumerable<TDataObject>> remoteResult = _proxy.RunRequestSafe(() => requestDelegate(), channel);
            if (remoteResult.IsValid && remoteResult.Result.Any(dto => dto.UpdatedAt.HasValue))
                _lastRequest = remoteResult.Result.Where(dto => dto.UpdatedAt.HasValue).Max(dto => dto.UpdatedAt.Value);
            return remoteResult.IsValid ? ActionResult<IEnumerable<TResult>>.GetValidResult(remoteResult.Result.Select(d => _convertDelegate(d)).ToArray()) : ActionResult<IEnumerable<TResult>>.GetErrorResult(remoteResult);
        }

        private T GetAsyncResult<T>(Func<Task<T>> function)
        {
            Task<T> task = function();
            task.Wait();
            return task.Result;
        }

        protected virtual void OnGotUpdate(TResult update)
        {
            EventHandler<ValueEventArgs<TResult>> handler = GotUpdate;
            if (handler != null)
                handler(this, new ValueEventArgs<TResult>(update));
        }

        private void ResultsLoop_GoingToAwait(object sender, EventArgs e)
        {
            IEnumerable<TResult> result = null;
            while (result == null)
            {
                ActionResult<IEnumerable<TResult>> requestResult = RequestCore();
                if (requestResult.IsValid)
                    result = requestResult.Result;
                else
                    Task.Delay(3000);
            }
            _resultsLoop.PushMessageRange(result);
        }
    }

}
