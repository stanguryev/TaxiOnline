using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Exceptions;
using TaxiOnline.ClientInfrastructure.Exceptions.Enums;
using TaxiOnline.ServiceContract;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.Patterns;

#warning вопрос о DuplexChannel не решён

namespace TaxiOnline.ClientServicesAdapter.Data.Service
{
    internal class ServiceProxy : RemoteProxy<ITaxiOnlineService>
    {
        private readonly ChannelFactory<ITaxiOnlineService> _channelFactory;
        //private readonly InstanceContext _callbackInstance;
        private readonly CallbackWrapper _callbackWrapper;
        private readonly string _serverEndpointAddress;

        public CallbackWrapper CallbackWrapper
        {
            get { return _callbackWrapper; }
        }

        public ServiceProxy(string serverEndpointAddress)
            : base()
        {
            _serverEndpointAddress = serverEndpointAddress;
            _callbackWrapper = new CallbackWrapper();
            //_callbackInstance = new InstanceContext(_callbackWrapper);
            _channelFactory = CreateChannelFactory();
            BeginConnect();
        }

        private ChannelFactory<ITaxiOnlineService> CreateChannelFactory()
        {
            System.ServiceModel.Channels.Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.None); //NetTcpBinding(SecurityMode.None);
            EndpointAddress address = new EndpointAddress(_serverEndpointAddress);
            return new ChannelFactory<ITaxiOnlineService>(binding, address); //new DuplexChannelFactory<ITaxiOnlineService>(_callbackInstance, binding, address);
        }

        protected override ITaxiOnlineService CreateChannel()
        {
            return _channelFactory.CreateChannel();
        }

        protected override void OpenChannel(ITaxiOnlineService channel)
        {
            ((IClientChannel)channel).Open();
        }

        protected override ActionResult BuildErrorInfo(RemoteProxy<ITaxiOnlineService>.ErrorType errorType)
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
            CommunicationException communicationException = exception as CommunicationException;
            if (communicationException != null)
            {
                errorInfo = ActionResult.GetErrorResult(communicationException);
                return true;
            }
            TimeoutException timeoutException = exception as TimeoutException;
            if (timeoutException != null)
            {
                errorInfo = ActionResult.GetErrorResult(timeoutException);
                return true;
            }
            WebException webException = exception as WebException;
            if (webException != null)
            {
                errorInfo = ActionResult.GetErrorResult(webException);
                return true;
            }
            errorInfo = ActionResult.ValidResult;
            return false;
        }

        protected override bool ProceedConnectionException(Exception exception, out Toolkit.Events.ActionResult errorInfo)
        {
            CommunicationException communicationException = exception as CommunicationException;
            if (communicationException != null)
            {
                errorInfo = ActionResult.GetErrorResult(communicationException);
                return true;
            }
            TimeoutException timeoutException = exception as TimeoutException;
            if (timeoutException != null)
            {
                errorInfo = ActionResult.GetErrorResult(timeoutException);
                return true;
            }
            WebException webException = exception as WebException;
            if (webException != null)
            {
                errorInfo = ActionResult.GetErrorResult(webException);
                return true;
            }
            errorInfo = ActionResult.ValidResult;
            return false;
        }

        protected override bool IsConnectionAvailable()
        {
            return ((IClientChannel)_channel).State == CommunicationState.Opened;
        }

        protected override void NotifyConnectionStateChanged()
        {

        }

        protected override Tuple<RemoteProxy<ITaxiOnlineService>.ConnectionCheckDecision, Toolkit.Events.ActionResult> CheckConnectionCore(ITaxiOnlineService channel)
        {
            switch (((IClientChannel)channel).State)
            {
                case CommunicationState.Closed:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.NotifyError, ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.ConnectionIsClosedError)));
                case CommunicationState.Closing:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.NotifyError, ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.ConnectionIsClosingError)));
                case CommunicationState.Created:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.Reconnect, ActionResult.ValidResult);
                case CommunicationState.Faulted:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.Reconnect, ActionResult.ValidResult);
                case CommunicationState.Opened:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.Use, ActionResult.ValidResult);
                case CommunicationState.Opening:
                    return new Tuple<ConnectionCheckDecision, ActionResult>(ConnectionCheckDecision.NotifyError, ActionResult.GetErrorResult(new RemoteProxyException(RemoteProxyErrors.ConnectionIsNotEstablishedError)));
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        protected override void HookChannel(ITaxiOnlineService channel)
        {

        }

        protected override void UnhookChannel(ITaxiOnlineService channel)
        {

        }

        protected override void DisposeCore()
        {
            IClientChannel channel = (IClientChannel)_channel;
            UnhookChannel((ITaxiOnlineService)channel);
            try
            {
                if (channel.State != CommunicationState.Faulted)
                    channel.Close();
                else
                    channel.Abort();
            }
            catch (CommunicationException)
            {
                channel.Abort();
            }
            catch (TimeoutException)
            {
                channel.Abort();
            }
            catch (Exception)
            {
                channel.Abort();
                throw;
            }
            finally
            {
                channel = null;
                _channel = null;
            }
            // _callbackInstance.Close();
        }

        protected override void AbortCore(ITaxiOnlineService channel)
        {
            if (channel != null)
            {
                UnhookChannel(channel);
                ((IClientChannel)channel).Abort();
            }
        }
    }
}
