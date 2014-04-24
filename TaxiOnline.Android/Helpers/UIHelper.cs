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
using TaxiOnline.Android.Activities;

namespace TaxiOnline.Android.Helpers
{
    public static class UIHelper
    {
        public const string UpperActivityBundleId = "Upper";

        public static Intent GetIntent(Activity sourceActivity, Type targetActivityType)
        {
            Intent intent = new Intent(sourceActivity, targetActivityType);
            intent.PutExtra(UpperActivityBundleId, sourceActivity.Handle.ToInt64());
            return intent;
        }

        public static void GoActivity(Activity sourceActivity, Type targetActivityType)
        {
            Intent intent = new Intent(sourceActivity, targetActivityType);
            intent.PutExtra(UpperActivityBundleId, sourceActivity.Handle.ToInt64());
            sourceActivity.StartActivity(intent);
        }

        public static void GoResultActivity(Activity sourceActivity, Type targetActivityType, int requestCode)
        {
            Intent intent = new Intent(sourceActivity, targetActivityType);
            intent.PutExtra(UpperActivityBundleId, sourceActivity.Handle.ToInt64());
            sourceActivity.StartActivityForResult(intent, requestCode);
        }

        public static TActivity GetUpperActivity<TActivity>(Activity currentActivity, Bundle bundle) where TActivity : Activity
        {
            IntPtr upperHandle = new IntPtr(currentActivity.Intent.GetLongExtra(UpperActivityBundleId, 0));
            return upperHandle == IntPtr.Zero ? null : Activity.GetObject<TActivity>(upperHandle, JniHandleOwnership.DoNotTransfer);
        }
    }
}