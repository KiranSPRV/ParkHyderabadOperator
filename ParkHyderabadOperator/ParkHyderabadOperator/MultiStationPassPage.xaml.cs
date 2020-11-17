using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
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
    public partial class MultiStationPassPage : ContentPage
    {
        string PassType = string.Empty;
        PassPrice objResultVMPass = null;
        DALExceptionManagment dal_Exceptionlog;
        List<Location> lstSelectedLocations;
        public MultiStationPassPage()
        {
            InitializeComponent();
            labelSelectedStations.Text = "You have selected MULTI STATION monthly pass.You can park your vehicle at";
            lstSelectedLocations = new List<Location>();
            GetAllStations();
        }
        public MultiStationPassPage(string NewOrReNew, PassPrice objmultiVMPass)
        {
            InitializeComponent();
            labelSelectedStations.Text = "You have selected MULTI STATION monthly pass.You can park your vehicle at ";
            PassType = NewOrReNew;
            objResultVMPass = objmultiVMPass;
            lstSelectedLocations = new List<Location>();
            GetAllStations();
        }
        private void GetAllStations()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALHome dal_Home = new DALHome();
                    if (PassType == "ReNew Pass")
                    {
                        if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                        {
                            var objReNewVehicle = (CustomerVehiclePass)App.Current.Properties["ReNewPassCustomerVehicle"];
                            List<VMMultiLocations> renewPassLocations = dal_Home.GetAllPassLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), objResultVMPass.VehicleTypeID.VehicleTypeCode, objReNewVehicle.CustomerVehiclePassID);
                            lstStations.ItemsSource = renewPassLocations;
                            //if (renewPassLocations.Count > 0)
                            //{
                            //    for (int l = 0; l < renewPassLocations.Count; l++)
                            //    {
                            //        Location objselected = new Location();
                            //        objselected.LocationID = renewPassLocations[l].LocationID;
                            //        objselected.LocationName = renewPassLocations[l].LocationName;
                            //        labelSelectedStations.Text = labelSelectedStations.Text + "," + renewPassLocations[l].LocationName.ToUpper();
                            //        lstSelectedLocations.Add(objselected);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        lstStations.ItemsSource = dal_Home.GetAllLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), objResultVMPass.VehicleTypeID.VehicleTypeCode);
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MultiStationPassPage.xaml.cs", "", "GetAllStations");
            }
        }
        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {

                var resultbox = (CheckBox)sender;
                VMMultiLocations ob = resultbox.BindingContext as VMMultiLocations;
                if (resultbox.IsChecked)
                {
                    Location objselected = new Location();
                    objselected.LocationID = ob.LocationID;
                    objselected.LocationName = ob.LocationName;
                    if (lstSelectedLocations.Count < 3)
                    {
                        labelSelectedStations.Text = labelSelectedStations.Text + "," + ob.LocationName.ToUpper();
                        lstSelectedLocations.Add(objselected);
                    }
                    else
                    {
                        resultbox.IsChecked = false;
                        await DisplayAlert("Alert", "You can only select a maximum of 3 stations", "Ok");
                    }
                }
                else
                {

                    if (lstSelectedLocations.Count > 0)
                    {
                        var item = lstSelectedLocations.SingleOrDefault(x => x.LocationID == ob.LocationID);
                        if (item != null)
                        {
                            lstSelectedLocations.Remove(item);
                            if (labelSelectedStations.Text.Contains(item.LocationName.ToUpper()))
                            {
                                labelSelectedStations.Text = labelSelectedStations.Text.Replace("," + item.LocationName.ToUpper(), string.Empty);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MultiStationPassPage.xaml.cs", "", "CheckBox_CheckedChanged");
            }
        }
        private async void BtnContinue_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (lstSelectedLocations.Count > 0 && lstSelectedLocations.Count==3)
                {
                    App.Current.Properties["MultiSelectionLocations"] = lstSelectedLocations;
                    await Navigation.PushAsync(new MonthlyPassPage(PassType, objResultVMPass));
                }
                else
                {
                    await DisplayAlert("Alert", "Please select stations", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MultiStationPassPage.xaml.cs", "", "BtnGneratePass_Clicked");
            }
        }
    }
}