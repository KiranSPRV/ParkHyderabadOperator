using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.ViewModel;

namespace ParkHyderabadOperator.DAL
{
  public  class ParkingVechiles
    {
        public List<Vehicle> GetParkedVehicles(string Location, string Lot)
        {
            List<Vehicle> lstParkedVehicles = new List<Vehicle>();
            try
            {


                Vehicle objVehicleV = new Vehicle();

                objVehicleV.RegistrationNumber = "TS 08 FL 0960";
                objVehicleV.VehicleImage = "bike_red.png";
                objVehicleV.BayNumber = "01-10";


                objVehicleV.VehicleStaus = "V";
                objVehicleV.VehicleStatusColor = "#FF0000";
                objVehicleV.BayNumberColor = "#FF0000";

                objVehicleV.VehicleLoginType = "V";
                objVehicleV.VehicleLoginTypeColor = "#F39C12";


                objVehicleV.VehicleClampImage = "clamp.png";


                Vehicle objVehicleA = new Vehicle();
                objVehicleA.RegistrationNumber = "TS 08 FL 0960";
                objVehicleA.VehicleImage = "car_orange.png";
                objVehicleA.BayNumber = "20-30";

                objVehicleA.VehicleStaus = "A";
                objVehicleA.VehicleStatusColor = "#F39C12";
                objVehicleA.BayNumberColor = "#F39C12";

                objVehicleA.VehicleLoginType = "A";
                objVehicleA.VehicleLoginTypeColor = "#008000";

                objVehicleA.VehicleClampImage = "clock_orange.png";


                Vehicle objVehicleP = new Vehicle();
                objVehicleP.VehicleImage = "bike_black.png";
                objVehicleP.RegistrationNumber = "TS 08 FL 0960";
                objVehicleP.BayNumber = "300-400";
                objVehicleP.VehicleStaus = "P";
                objVehicleP.VehicleStatusColor = "#3293fa";
                objVehicleP.BayNumberColor = "#444444";

                objVehicleP.VehicleLoginType = "P";
                objVehicleP.VehicleLoginTypeColor = "#0000FF";



                lstParkedVehicles.Add(objVehicleV);
                lstParkedVehicles.Add(objVehicleA);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);
                lstParkedVehicles.Add(objVehicleP);

            }
            catch (Exception ex) { }
            return lstParkedVehicles;
        }
        public List<Vehicle> GetAllVehicle(string Location, string SearchVehicle)
        {
            List<Vehicle> lstVehicle = new List<Vehicle>();
            try
            {

                

                Vehicle objVehicleO = new Vehicle();
                objVehicleO.RegistrationNumber = "TS 08 FL 0960";
                objVehicleO.VehicleType = "Two Wheeler";

                Vehicle objVehicleV = new Vehicle();
                objVehicleV.RegistrationNumber = "TS 08 FL 0961";
                objVehicleV.VehicleType = "Four Wheeler";

                Vehicle objVehicleClamp = new Vehicle();
                objVehicleClamp.RegistrationNumber = "AP 08 FL 0962";
                objVehicleClamp.VehicleType = "Two Wheeler";

                Vehicle objVehicleMP = new Vehicle();
                objVehicleMP.RegistrationNumber = "MP 08 FL 0962";
                objVehicleMP.VehicleType = "Four Wheeler";

                Vehicle objVehicleUP = new Vehicle();
                objVehicleUP.RegistrationNumber = "UP 08 FL 0962";
                objVehicleUP.VehicleType = "Four Wheeler";

                lstVehicle.Add(objVehicleO);
                lstVehicle.Add(objVehicleV);
                lstVehicle.Add(objVehicleClamp);
                lstVehicle.Add(objVehicleMP);
                lstVehicle.Add(objVehicleUP);

            }
            catch (Exception ex) { }
            return lstVehicle;
        }
        public VMVehicleParking GetParkingVehicleDetails(string Vehicle, string CheckINID)
        {
            VMVehicleParking objVMVehicleParking = new VMVehicleParking();
            try
            {

                objVMVehicleParking.VehicleNumber = "TS 08 MN 0147";
                objVMVehicleParking.VehicleType = "Bike";
                objVMVehicleParking.VehicleImage = "bike_black.png";
                objVMVehicleParking.ParkingFromTime = DateTime.Now.ToString("dd MMMM yyyy hh:mm tt");
                objVMVehicleParking.ParkingToTime = DateTime.Now.ToString("dd MMMM yyyy hh:mm tt");
                objVMVehicleParking.ParkingFees = "10";
                objVMVehicleParking.BayNumber = "Bay Number " + "01-10";
                objVMVehicleParking.Location = "KPHB Colony - A2";
                objVMVehicleParking.PaymentType = "Cash";
            }
            catch (Exception ex)
            {
            }
            return objVMVehicleParking;

        }
    }
}
