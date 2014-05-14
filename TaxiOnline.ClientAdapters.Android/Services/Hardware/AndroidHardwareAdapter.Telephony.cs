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
using TaxiOnline.Toolkit.Events;
using Android.Telephony;
using System.Collections.Concurrent;

namespace TaxiOnline.ClientAdapters.Android.Services.Hardware
{
    public partial class AndroidHardwareAdapter
    {
        private class IncomingCallsListener : PhoneStateListener
        {
            private readonly Action<string> _incomingCallDelegate;

            public IncomingCallsListener(Action<string> incomingCallDelegate)
            {
                _incomingCallDelegate = incomingCallDelegate;
            }

            public override void OnCallStateChanged(CallState state, string incomingNumber)
            {
                base.OnCallStateChanged(state, incomingNumber);
                if (state == CallState.Ringing && !string.IsNullOrWhiteSpace(incomingNumber))
                    _incomingCallDelegate(incomingNumber);
            }
        }

        private readonly ConcurrentDictionary<Delegate, IncomingCallsListener> _incomingCallsSubscriptions = new ConcurrentDictionary<Delegate, IncomingCallsListener>();

        public override event EventHandler<ValueEventArgs<string>> IncomingCallArrived
        {
            add { SubscribeToIncomingCall(value); }
            remove { UnsubscribeFromIncomingCall(value); }
        }

        public override ActionResult PhoneCall(string number)
        {
            try
            {
                string unifiedNumber = string.Format("tel:{0}", new string(number.Where(c => char.IsDigit(c) || c == '+').ToArray()));
                Intent callIntent = new Intent(Intent.ActionCall);
                callIntent.AddFlags(ActivityFlags.NewTask);
                callIntent.SetData(global::Android.Net.Uri.Parse(unifiedNumber));
                Application.Context.StartActivity(callIntent);
                return ActionResult.ValidResult;
            }
            catch (global::Android.Util.AndroidRuntimeException ex)
            {
                return ActionResult.GetErrorResult(ex);
            }
        }

        private void SubscribeToIncomingCall(EventHandler<ValueEventArgs<string>> handler)
        {
            using (TelephonyManager telephonyManager = (TelephonyManager)Application.Context.GetSystemService(Application.TelephonyService))
            {
                IncomingCallsListener listener = new IncomingCallsListener(phoneNumber =>
                {
                    if (handler != null && phoneNumber != null)
                        handler(this, new ValueEventArgs<string>(phoneNumber));
                });
                telephonyManager.Listen(listener, PhoneStateListenerFlags.CallState);
                _incomingCallsSubscriptions.AddOrUpdate(handler, listener, (h, l) => listener);
            }
        }

        private void UnsubscribeFromIncomingCall(EventHandler<ValueEventArgs<string>> handler)
        {
            IncomingCallsListener listener;
            if (!_incomingCallsSubscriptions.TryGetValue(handler, out listener) || listener == null)
                return;
            listener.Dispose();
            _incomingCallsSubscriptions.TryRemove(handler, out listener);
        }
    }
}