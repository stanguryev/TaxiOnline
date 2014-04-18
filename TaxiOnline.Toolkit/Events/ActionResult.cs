using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace TaxiOnline.Toolkit.Events
{
    public struct ActionResult
    {
        private readonly bool _isValid;
        private readonly string _failMessage;
        private readonly Exception _failException;
        private Exception _fakeFailException;
        private readonly ExceptionDispatchInfo _exceptionCapture;

        private static readonly ActionResult s_validResult = new ActionResult(true, null, null);

        public bool IsValid
        {
            get { return _isValid; }
        }

        public string FailMessage
        {
            get
            {
                ActionResult.ThrowExceptionIfIsValid(this);
                return _failMessage;
            }
        }

        public Exception FailException
        {
            get
            {
                ActionResult.ThrowExceptionIfIsValid(this);
                return _failException ?? GetFakeFailException();
            }
        }

        public static ActionResult ValidResult
        {
            get { return s_validResult; }
        }

        private ActionResult(bool isValid, string failMessage, Exception failException)
        {
            _isValid = isValid;
            _failMessage = failMessage;
            _failException = failException;
            _fakeFailException = null;
            _exceptionCapture = failException == null ? null : ExceptionDispatchInfo.Capture(failException);
        }

        public static ActionResult GetErrorResult(string failMessage)
        {
            return new ActionResult(false, failMessage, null);
        }

        public static ActionResult GetErrorResult(Exception failException)
        {
            return new ActionResult(false, failException.Message, failException);
        }

        public static ActionResult GetErrorResult(string failMessage, Exception failException)
        {
            return new ActionResult(false, failException.Message, failException);
        }

        public static ActionResult GetErrorResult(IEnumerable<ActionResult> otherActionResults)
        {
            foreach (ActionResult otherActionResult in otherActionResults)
                ThrowExceptionIfIsValid(otherActionResult);
            return new ActionResult(false, string.Join(Environment.NewLine, otherActionResults.Select(r => r.FailMessage)), new AggregateException(otherActionResults.Select(r => r.FailException)));
        }

        public static ActionResult GetErrorResult<TOther>(ActionResult<TOther> otherActionResult)
        {
            ThrowExceptionIfIsValid<TOther>(otherActionResult);
            return new ActionResult(false, otherActionResult.FailMessage, otherActionResult.FailException);
        }

        public static ActionResult GetErrorResult<TOther>(string additionalInfoTemplate, ActionResult<TOther> otherActionResult)
        {
            ThrowExceptionIfIsValid<TOther>(otherActionResult);
            return new ActionResult(false, string.Format(additionalInfoTemplate, otherActionResult.FailMessage), otherActionResult.FailException);
        }

        public static ActionResult GetErrorResult(string additionalInfoTemplate, ActionResult otherActionResult)
        {
            ThrowExceptionIfIsValid(otherActionResult);
            return new ActionResult(false, string.Format(additionalInfoTemplate, otherActionResult.FailMessage), otherActionResult.FailException);
        }

        public bool TryThrowException()
        {
            if (_exceptionCapture == null)
                return false;
            _exceptionCapture.Throw();
            return true;
        }

        public static explicit operator bool(ActionResult actionResult)
        {
            return actionResult._isValid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Exception GetFakeFailException()
        {
            if (_fakeFailException == null)
                _fakeFailException = new Exception(_failMessage);
            return _fakeFailException;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowExceptionIfIsValid(ActionResult actionResult)
        {
            if (actionResult.IsValid)
                throw new InvalidOperationException("Правильный результат выполнения не содержит сообщения об ошибке");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowExceptionIfIsValid<TResult>(ActionResult<TResult> actionResult)
        {
            if (actionResult.IsValid)
                throw new InvalidOperationException("Правильный результат выполнения не содержит сообщения об ошибке");
        }
    }
}
