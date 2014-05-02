using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TaxiOnline.Android.Decorators
{
    public class ProgressDialogDecorator
    {
        private readonly string _title;
        private readonly string _message;
        private readonly Context _context;
        private ProgressDialog _dialog;
        private Action _cancelCallback;
        private System.Threading.CancellationTokenSource _cancellaction;

        public ProgressDialogDecorator(Context context, string title, string message)
        {
            _context = context;
            _title = title;
            _message = message;
        }

        public void Show()
        {
            if (_dialog == null)
                _dialog = ProgressDialog.Show(_context, _title, _message);
        }

        public void Show(Action cancelCallback)
        {
            if (_dialog == null)
            {
                _cancelCallback = cancelCallback;
                _dialog = ProgressDialog.Show(_context, _title, _message, true, true);
                _dialog.CancelEvent += Dialog_CancelEvent;
            }
        }

        public System.Threading.CancellationToken ShowWithCancel()
        {
            if (_cancellaction == null)
            {
                _cancellaction = new System.Threading.CancellationTokenSource();
                Show(() =>
                {
                    _cancellaction.Cancel();
                    _cancellaction.Dispose();
                    _cancellaction = null;
                });
                return _cancellaction.Token;
            }
            return default(System.Threading.CancellationToken);
        }

        public void Hide()
        {
            if (_dialog != null)
            {
                _dialog.CancelEvent -= Dialog_CancelEvent;
                _dialog.Dismiss();
                _dialog.Dispose();
                _dialog = null;
                if (_cancellaction != null)
                {
                    _cancellaction.Dispose();
                    _cancellaction = null;
                }
            }
        }

        private void Dialog_CancelEvent(object sender, EventArgs e)
        {
            if (_dialog != null && _cancelCallback != null)
            {
                _cancelCallback();
                _dialog.CancelEvent -= Dialog_CancelEvent;
            }
        }
    }
}