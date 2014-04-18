using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Logic.Common.Enums;
using TaxiOnline.Logic.Common.Exceptions;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Threading.Lock;

namespace TaxiOnline.Logic.Decorators
{
    public class RequestDecorator
    {
        private enum CancelRequestAction { CancelPending, CancelConfirmed }

        private readonly ReadWriteBox _stateLocker = new ReadWriteBox();
        private readonly Func<RequestState> _getStateDelegate;
        private readonly Action<RequestState> _setStateDelegate;
        private readonly Func<ActionResult> _confirmDeleage;
        private readonly Action _cancelPendingDeleage;
        private readonly Func<ActionResult> _cancelConfirmedDeleage;

        public RequestDecorator(Func<RequestState> getStateDelegate, Action<RequestState> setStateDelegate, Func<ActionResult> confirmDeleage, Action cancelPendingDeleage, Func<ActionResult> cancelConfirmedDeleage)
        {
            _getStateDelegate = getStateDelegate;
            _setStateDelegate = setStateDelegate;
            _confirmDeleage = confirmDeleage;
            _cancelPendingDeleage = cancelPendingDeleage;
            _cancelConfirmedDeleage = cancelConfirmedDeleage;
        }

        public ActionResult Confirm()
        {
            using (_stateLocker.EnterUpgradeableReadLock())
            {
                RequestState currentState = _getStateDelegate();
                ActionResult checkResult = CheckCanConfirm(currentState);
                if (!checkResult.IsValid)
                    return checkResult;
                using (_stateLocker.EnterWriteLock())
                {
                    ActionResult deepCheckResult = CheckCanConfirm(currentState);
                    if (!deepCheckResult.IsValid)
                        return deepCheckResult;
                    _setStateDelegate(RequestState.Confirming);
                }
                ActionResult confirmResult = _confirmDeleage();
                if (confirmResult.IsValid)
                    using (_stateLocker.EnterWriteLock())
                        _setStateDelegate(RequestState.Comfirmed);
                else
                    using (_stateLocker.EnterWriteLock())
                        _setStateDelegate(RequestState.ConfirmFailed);
                return confirmResult;
            }
        }

        public ActionResult Cancel()
        {
            using (_stateLocker.EnterUpgradeableReadLock())
            {
                RequestState currentState = _getStateDelegate();
                ActionResult checkResult = CheckCanCancel(currentState);
                if (!checkResult.IsValid)
                    return checkResult;
                CancelRequestAction action = GetCancelAction(currentState);
                switch (action)
                {
                    case CancelRequestAction.CancelPending:
                        {
                            using (_stateLocker.EnterWriteLock())
                            {
                                ActionResult deepCheckResult = CheckCanCancel(currentState);
                                if (!deepCheckResult.IsValid)
                                    return deepCheckResult;
                                _setStateDelegate(RequestState.Canceling);
                            }
                            _cancelPendingDeleage();
                            using (_stateLocker.EnterWriteLock())
                                _setStateDelegate(RequestState.Canceled);
                            return ActionResult.ValidResult;
                        }
                    case CancelRequestAction.CancelConfirmed:
                        {
                            using (_stateLocker.EnterWriteLock())
                            {
                                ActionResult deepCheckResult = CheckCanCancel(currentState);
                                if (!deepCheckResult.IsValid)
                                    return deepCheckResult;
                                _setStateDelegate(RequestState.Canceling);
                            }
                            ActionResult cancelResult = _cancelConfirmedDeleage();
                            if (cancelResult.IsValid)
                                using (_stateLocker.EnterWriteLock())
                                    _setStateDelegate(RequestState.Canceled);
                            else
                                using (_stateLocker.EnterWriteLock())
                                    _setStateDelegate(RequestState.CancelFailed);
                            return cancelResult;
                        }
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        private ActionResult CheckCanConfirm(RequestState currentState)
        {
            switch (currentState)
            {
                case RequestState.Created:
                    return ActionResult.ValidResult;
                case RequestState.Confirming:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyConfirming));
                case RequestState.Comfirmed:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyConfirmed));
                case RequestState.ConfirmFailed:
                    return ActionResult.ValidResult;
                case RequestState.Canceling:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyCancelling));
                case RequestState.CancelFailed:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyConfirmed));
                case RequestState.Canceled:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyCalnceled));
                default:
                    throw new NotImplementedException();
            }
        }

        private ActionResult CheckCanCancel(RequestState currentState)
        {
            switch (currentState)
            {
                case RequestState.Created:
                    return ActionResult.ValidResult;
                case RequestState.Confirming:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyConfirming));
                case RequestState.Comfirmed:
                    return ActionResult.ValidResult;
                case RequestState.ConfirmFailed:
                    return ActionResult.ValidResult;
                case RequestState.Canceling:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyCancelling));
                case RequestState.CancelFailed:
                    return ActionResult.ValidResult;
                case RequestState.Canceled:
                    return ActionResult.GetErrorResult(new InvalidRequestException(RequestErrors.AlreadyCalnceled));
                default:
                    throw new NotImplementedException();
            }
        }

        private CancelRequestAction GetCancelAction(RequestState currentState)
        {
            switch (currentState)
            {
                case RequestState.Created:
                    return CancelRequestAction.CancelPending;
                case RequestState.Comfirmed:
                    return CancelRequestAction.CancelConfirmed;
                case RequestState.ConfirmFailed:
                    return CancelRequestAction.CancelPending;
                case RequestState.CancelFailed:
                    return CancelRequestAction.CancelConfirmed;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
