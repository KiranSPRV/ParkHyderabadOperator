using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestingPage : ContentPage
    {
        public TestingPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            Accelerometer.ShakeDetected += Accelerometer_OnShaked;
            Accelerometer.Start(SensorSpeed.Fastest);
            base.OnAppearing();
        }

        void Accelerometer_OnShaked(object sender, EventArgs e)
        {
            labelDeviceChange.Text = $"Shake detected: {DateTime.Now.ToLongTimeString()}";
        }

        protected override void OnDisappearing()
        {
            Accelerometer.Stop();
            Accelerometer.ShakeDetected -= Accelerometer_OnShaked;
            base.OnDisappearing();
        }
    }
}