using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace TaxiOnline.Toolkit.Events
{
    public struct ActionResult<T>
    {
        private readonly T _result;
        private readonly bool _isValid;
        private readonly string _failMessage;
        private readonly Exception _failException;
        private Exception _fakeFailException;
        private readonly ExceptionDispatchInfo _exceptionCapture;

        public bool IsValid
        {
            get { return _isValid; }
        }

        public T Result
        {
            get
            {
                if (!_isValid)
                    throw new InvalidOperationException("При наличии ошибки результат недоступен");
                return _result;
            }
        }

        public string FailMessage
        {
            get
            {
                ActionResult<T>.ThrowExceptionIfIsValid(this);
                return _failMessage;
            }
        }

        public Exception FailException
        {
            get
            {
                ActionResult<T>.ThrowExceptionIfIsValid(this);
                return _failException ?? GetFakeFailException();
            }
        }

        private ActionResult(T result, bool isValid, string failMessage, Exception failException)
        {
            _result = result;
            _isValid = isValid;
            _failMessage = failMessage;
            _failException = failException;
            _fakeFailException = null;
            _exceptionCapture = failException == null ? null : ExceptionDispatchInfo.Capture(failException);
        }

        public static ActionResult<T> GetValidResult(T result)
        {
            return new ActionResult<T>(result, true, null, null);
        }

        public static ActionResult<T> GetErrorResult(string failMessage)
        {
            return new ActionResult<T>(default(T), false, failMessage, new Exception(failMessage));
        }

        public static ActionResult<T> GetErrorResult(Exception failException)
        {
            return new ActionResult<T>(default(T), false, failException.Message, failException);
        }

        public static ActionResult<T> GetErrorResult(string failMessage, Exception failException)
        {
            return new ActionResult<T>(default(T), false, failException.Message, failException);
        }

        public static ActionResult<T> GetErrorResult<TOther>(ActionResult<TOther> otherActionResult)
        {
            ThrowExceptionIfIsValid<TOther>(otherActionResult);
            return new ActionResult<T>(default(T), false, otherActionResult.FailMessage, otherActionResult.FailException);
        }

        public static ActionResult<T> GetErrorResult<TOther>(string additionalInfoTemplate, ActionResult<TOther> otherActionResult)
        {
            ThrowExceptionIfIsValid<TOther>(otherActionResult);
            return new ActionResult<T>(default(T), false, string.Format(additionalInfoTemplate, otherActionResult.FailMessage), null);
        }

        public static ActionResult<T> GetErrorResult(string additionalInfoTemplate, ActionResult otherActionResult)
        {
            ThrowExceptionIfIsValid(otherActionResult);
            return new ActionResult<T>(default(T), false, string.Format(additionalInfoTemplate, otherActionResult.FailMessage), null);
        }

        public static ActionResult<T> GetErrorResult(ActionResult otherActionResult)
        {
            ThrowExceptionIfIsValid(otherActionResult);
            return new ActionResult<T>(default(T), false, otherActionResult.FailMessage, otherActionResult.FailException);
        }

        public bool TryThrowException()
        {
            if (_exceptionCapture == null)
                return false;
            _exceptionCapture.Throw();
            return true;
        }

        public static explicit operator bool(ActionResult<T> actionResult)
        {
            return actionResult._isValid;
        }

        public static explicit operator ActionResult(ActionResult<T> actionResult)
        {
            return actionResult.IsValid ? ActionResult.ValidResult : ActionResult.GetErrorResult(actionResult);
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