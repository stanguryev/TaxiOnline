using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Exceptions;
using TaxiOnline.ClientInfrastructure.Exceptions.Enums;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.Patterns;

namespace TaxiOnline.ClientServicesAdapter.Data.Service
{
    public class AzureMobileServicesProxy : RemoteProxy<MobileServiceClient>
    {
        private readonly string _serverEndpointAddress;

        public AzureMobileServicesProxy(string serverEndpointAddress)
        {
            _serverEndpointAddress = serverEndpointAddress;
            BeginConnect();
        }

        protected override MobileServiceClient CreateChannel()
        {
            return new MobileServiceClient(_serverEndpointAddress, @"bQxZxfjJWTeAdBmIWsOtHLoUqsgLjK47");
        }

        protected override void OpenChannel(MobileServiceClient channel)
        {

        }

        protected override Toolkit.Events.ActionResult BuildErrorInfo(RemoteProxy<MobileServiceClient>.ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.SessionIsClosedError:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.SessionIsClosedError));
                case ErrorType.FailedToReconnect:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.FailedToReconnect));
                case ErrorType.ConnectionWasChangedError:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.ConnectionWasChangedError));
                case ErrorType.Timeout:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.Timeout));
                case ErrorType.IsConnecting:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.IsConnecting));
                case ErrorType.ConnectionIsClosingError:
                    return ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.IsConnecting));
                default:
                    throw new NotImplementedException();
            }
        }

        protected override bool ProceedInvokeException(Exception exception, out Toolkit.Events.ActionResult errorInfo)
        {
            MobileServiceInvalidOperationException mobileServiceInvalidOperationException = exception as MobileServiceInvalidOperationException;
            if (mobileServiceInvalidOperationException != null)
            {
                errorInfo = ActionResult.GetErrorResult(mobileServiceInvalidOperationException);
                return true;
            }
            MobileServicePreconditionFailedException mobileServicePreconditionFailedException = exception as MobileServicePreconditionFailedException;
            if (mobileServicePreconditionFailedException != null)
            {
                errorInfo = ActionResult.GetErrorResult(mobileServicePreconditionFailedException);
                return true;
            }
            errorInfo = ActionResult.ValidResult;
            return false;
        }

        protected override bool ProceedConnectionException(Exception exception, out Toolkit.Events.ActionResult errorInfo)
        {
            errorInfo = ActionResult.GetErrorResult(exception);
            return true;
        }

        protected override bool IsConnectionAvailable()
        {
            return true;
        }

        protected override void NotifyConnectionStateChanged()
        {
            
        }

        protected override Tuple<RemoteProxy<MobileServiceClient>.ConnectionCheckDecision, Toolkit.Events.ActionResult> CheckConnectionCore(MobileServiceClient channel)
        {
            return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.Use, ActionResult.ValidResult);
        }

        protected override void HookChannel(MobileServiceClient channel)
        {
            
        }

        protected override void UnhookChannel(MobileServiceClient channel)
        {
            
        }

        protected override void DisposeCore()
        {
            _channel.Dispose();
        }

        protected override void AbortCore(MobileServiceClient channel)
        {
            
        }
    }
}
