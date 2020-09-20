using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiltersPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        DALHome dal_Home;
        string SelectedVehicle;
        ParkedVehiclesFilter objFilter;
        List<ApplicationType> objSelectedAppTypes;
        List<Status> objSelectedStatus;
        public FiltersPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            objFilter = new ParkedVehiclesFilter();
            objSelectedAppTypes = new List<ApplicationType>();
            objSelectedStatus = new List<Status>();
            dal_Home = new DALHome();
            GetAllApplicationTypes();
            GetAllStatus();
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            try
            {
                
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];

                    objFilter.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                    objFilter.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    objFilter.ApplicationTypeCode = objSelectedAppTypes;
                    objFilter.StatusCode = objSelectedStatus;
                    objFilter.VehicleTypeCode = SelectedVehicle;
                    var filerpage = new MasterHomePage(objFilter);
                     await Navigation.PushAsync(filerpage);
                }
            }
            catch (Exception ex)
            {
            }

        }

        #region ApplicationTypes
        public void GetAllApplicationTypes()
        {

            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    List<ApplicationType> lstApplicationType = dal_Home.GetAllApplicationTypes(Convert.ToString(App.Current.Properties["apitoken"]));
                    LstApplicationTypes.ItemsSource = lstApplicationType;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "GetAllApplicationTypes");
            }
        }
        private void SwitchApplicationType_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                var swithch = (Switch)sender;
                ApplicationType ob = swithch.BindingContext as ApplicationType;
                if (swithch.IsToggled)
                {
                    objSelectedAppTypes.Add(ob);
                }
                else
                {
                    if (objSelectedAppTypes.Count > 0)
                    {
                        var item = objSelectedAppTypes.SingleOrDefault(x => x.ApplicationTypeID == ob.ApplicationTypeID);
                        if (item != null)
                        {
                            objSelectedAppTypes.Remove(ob);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "SwitchApplicationType_Toggled");
            }
        }
        #endregion

        #region Status
        public void GetAllStatus()
        {

            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    List<Status> lstApplicationType = dal_Home.GetAllStatus(Convert.ToString(App.Current.Properties["apitoken"]));
                    LstStatus.ItemsSource = lstApplicationType;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "GetAllApplicationTypes");
            }
        }
        private void SwitchStatus_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                var swithch = (Switch)sender;
                Status obj = swithch.BindingContext as Status;
                if (swithch.IsToggled)
                {
                    if(obj.StatusCode.ToUpper()=="C")
                    {
                        objFilter.IsClamped = true;
                    }
                    else
                    {
                        objSelectedStatus.Add(obj);
                    }
                    
                }
                else
                {
                    if (obj.StatusCode.ToUpper() == "C")
                    {
                        objFilter.IsClamped = false;
                    }
                    else
                    {
                        if (objSelectedStatus.Count > 0)
                        {
                            var item = objSelectedStatus.SingleOrDefault(x => x.StatusID == obj.StatusID);
                            if (item != null)
                            {
                                objSelectedStatus.Remove(obj);
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "SwitchStatus_Toggled");
            }

        }

        #endregion

        #region Vehicle Type Selection

        private void ImgBtnTwoWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {

                SelectedVehicle = "2W";
                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle_ticked.png");
                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "ImgBtnTwoWheeler_Clicked");
            }
        }

        private void ImgBtnFourWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                SelectedVehicle = "4W";
                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle_ticked.png");
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "ImgBtnFourWheeler_Clicked");
            }

        }

        #endregion


    }
}