using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ParkHyderabadOperator.Droid.DependencyServices;
using ParkHyderabadOperator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LaunchActivity))]
namespace ParkHyderabadOperator.Droid.DependencyServices
{
    public class LaunchActivity : ILaunchActivity
    {
        [Obsolete]
        public void LaunchActivityInAndroid(string packageName)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionDelete);
                intent.SetData(Android.Net.Uri.Parse("package:"+ packageName));
                Forms.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                string exmsg = ex.Message;
            }
        }


    }
}