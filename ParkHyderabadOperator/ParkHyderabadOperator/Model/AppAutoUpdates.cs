using ParkHyderabadOperator.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;


namespace ParkHyderabadOperator.Model
{
    public class AppAutoUpdates
    {
        private readonly ILaunchActivity _launchActivity;

        public  AppAutoUpdates()
        {
            _launchActivity = DependencyService.Get<ILaunchActivity>();

        }
      
    }
}
