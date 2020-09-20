using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using ParkHyderabadOperator.ViewModel.VMPass;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPassPage : ContentPage
    {
        public string PassCategory = string.Empty;
        string SelectedVehicle = string.Empty;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objReNewVehicle = null;
        public NewPassPage(string pass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();

            PassCategory = pass;
            if (PassCategory == "New Pass")
            {
                PassTypePageHeading.Text = "NEW PASS";
                slPassTypePageHeadingImages.IsVisible = true;
                LoadPasseTypesAndPriceDetails("2W");
            }
            else if (PassCategory == "ReNew Pass")
            {
                PassTypePageHeading.Text = "CHOOSE PASS";
                PassTypePageHeading.Margin = new Thickness(0, 0, 100, 0);
                slPassTypePageHeadingImages.IsVisible = false;
                if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                {
                    objReNewVehicle = (CustomerVehiclePass)App.Current.Properties["ReNewPassCustomerVehicle"];
                    if (objReNewVehicle != null && objReNewVehicle.CustomerVehiclePassID != 0)
                    {
                        if (objReNewVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                        {
                            slFourWheelerImage.IsVisible = false;
                        }
                        if (objReNewVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                        {
                            slTwoWheelerImage.IsVisible = false;
                        }
                        LoadPasseTypesAndPriceDetails(objReNewVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode);
                    }

                }


            }

        }
        private void LoadPasseTypesAndPriceDetails(string SelectedVehicle)
        {
            try
            {
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    VMPass vm_Pass = new VMPass();
                    DALPass dal_Pass = new DALPass();

                    VehicleLotPassPrice objVehicleLotPassPrice = new VehicleLotPassPrice();
                    objVehicleLotPassPrice.VehicleTypeCode = SelectedVehicle;
                    objVehicleLotPassPrice.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;

                    List<PassPrice> lstVMPass = dal_Pass.GetPassPriceDetails(Convert.ToString(App.Current.Properties["apitoken"]), objVehicleLotPassPrice);

                    if(objReNewVehicle!=null&& objReNewVehicle.CustomerVehiclePassID!=0)
                    {
                        List<PassPrice> renewSelectedPass = lstVMPass.Where(p => (p.PassTypeID.PassTypeCode.ToUpper() == objReNewVehicle.PassPriceID.PassTypeID.PassTypeCode.ToUpper()) && p.StationAccess.ToUpper()== objReNewVehicle.PassPriceID.StationAccess.ToUpper()).ToList();
                        LstVehiclePasses.ItemsSource = renewSelectedPass;
                    }
                    else
                    {
                        LstVehiclePasses.ItemsSource = lstVMPass;
                    }
                    

                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "LoadPasseTypesAndPriceDetails");
            }
        }
        private async void LstVehiclePasses_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                PassPrice selectedPass = (PassPrice)e.SelectedItem;
                if (selectedPass != null)
                {
                    if (selectedPass.PassTypeID.PassTypeCode == "DP" || selectedPass.PassTypeID.PassTypeCode == "EP")//Day Pass
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new DailyPass(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "WP")//Weekly Pass
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new WeeklyPassPage(PassCategory, selectedPass));


                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "Single Station".ToUpper()))//Montly Pass -Single Station
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new MonthlyPassPage(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "Multi Station".ToUpper() || selectedPass.StationAccess.ToUpper() == "Multi Stations".ToUpper()))//Montly Pass -Multiple Station
                    {
                        await Navigation.PushAsync(new MultiStationPassPage(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "All Station".ToUpper() || selectedPass.StationAccess.ToUpper() == "All Stations".ToUpper()))//Montly Pass -Multiple Station
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new MonthlyPassPage(PassCategory, selectedPass));
                    }
                }
                  ((ListView)LstVehiclePasses).SelectedItem = null;

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "LstVehiclePasses_ItemSelected");
            }
        }

        #region Vehicle Type Selection

        private void SlTwoWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                SelectedVehicle = "2W";
                imgTwoWheeler.Source = "Twowheeler_circle_ticked.png";
                imgFourWheeler.Source = "Fourwheeler_circle.png";
                LoadPasseTypesAndPriceDetails(SelectedVehicle);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "ImgBtnTwoWheeler_Clicked");
            }
        }
        private void SlFourWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                SelectedVehicle = "4W";
                imgTwoWheeler.Source = "Twowheeler_circle.png";
                imgFourWheeler.Source = "Fourwheeler_circle_ticked.png";

                LoadPasseTypesAndPriceDetails(SelectedVehicle);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "ImgBtnFourWheeler_Clicked");
            }
        }

        #endregion
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutNewPassPage.Opacity = 0.5;
            }
            else
            {
                absLayoutNewPassPage.Opacity = 1;
            }

        }
    }
}