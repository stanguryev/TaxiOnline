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

        public void Hide()
        {
            if (_dialog != null)
            {
                _dialog.Dismiss();
                _dialog.Dispose();
                _dialog = null;
            }
        }
    }
}