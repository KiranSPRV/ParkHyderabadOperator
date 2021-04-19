using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParkHyderabadOperator.Model
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<VehicleCheckIn>().Wait();
            db.CreateTableAsync<DeSyncVehicleCheckIn>().Wait();

        }
        public bool IsTableExists(SQLiteAsyncConnection connection, string tableName)
        {
            try
            {
                var tableInfo = connection.GetTableInfoAsync(tableName);
                if (tableInfo.Result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        //Insert and Update new record
        public Task<int> SaveOfflineCheckInAsync(VehicleCheckIn objNewCheckIn)
        {
            Task<int> output = null;
            try
            {

                if (objNewCheckIn.CustomerParkingSlotID != 0)
                {

                    return db.UpdateAsync(objNewCheckIn);

                }
                else
                {
                    output = db.InsertAsync(objNewCheckIn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public Task<List<VehicleCheckIn>> GetAllVehicleAsync()
        {
            try
            {
                return db.Table<VehicleCheckIn>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VehicleCheckIn GetIVehicleDetailsAsync(string RegistrationNumber)
        {
            VehicleCheckIn resultvehicle = new VehicleCheckIn();
            try
            {
                var rstdata = db.Table<VehicleCheckIn>().Where(i => i.RegistrationNumber == RegistrationNumber).FirstOrDefaultAsync();
                resultvehicle = rstdata.Result;
            }
            catch (Exception ex) { throw ex; }
            return resultvehicle;

        }
        public Task<int> DeleteItemAsync(VehicleCheckIn objdelCheckIn)
        {
            Task<int> deleted = null;
            try
            {
                deleted = db.ExecuteAsync("DELETE FROM VehicleCheckIn WHERE [RegistrationNumber] = ?;", objdelCheckIn.RegistrationNumber);
            }
            catch (Exception ex) { throw ex; }
            return deleted;
        }
        public Task<int> DeleteAllItemAsync()
        {
            Task<int> deleted = null;
            try
            {
                deleted = db.ExecuteAsync("DELETE FROM VehicleCheckIn ;");
            }
            catch (Exception ex) { throw ex; }
            return deleted;
        }
        #region DeSync Items
        public Task<int> SaveDeSyncCheckInAsync(DeSyncVehicleCheckIn objDeSyncCheckIn)
        {
            Task<int> output = null;
            try
            {
                output = db.InsertAsync(objDeSyncCheckIn);
            }
            catch (Exception ex) { throw ex; }
            return output;
        }
        public Task<List<DeSyncVehicleCheckIn>> GetDeSyncCheckInAsync()
        {
            try
            {
                return db.Table<DeSyncVehicleCheckIn>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<int> DeleteDesycItemAsync(DeSyncVehicleCheckIn objdelCheckIn)
        {
            Task<int> deleted = null;
            try
            {
                deleted = db.ExecuteAsync("DELETE FROM DeSyncVehicleCheckIn WHERE [RegistrationNumber] = ?;", objdelCheckIn.RegistrationNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return deleted;
        }
        public Task<int> DeleteAllDesycItemAsync()
        {
            Task<int> deleted = null;
            try
            {
                deleted = db.ExecuteAsync("DELETE FROM DeSyncVehicleCheckIn ;");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return deleted;
        }
        #endregion

        #region Load VehicleTypes on Login
        public Task<int> SaveAllVehicleTypesInSQLLite(string apitoken, int LocationID)
        {

            Task<int> output = null;
            DALPass objDALPass = new DALPass();
            try
            {
                bool istableexist = IsTableExists(db, "VehicleType");
                if (!istableexist)
                {
                    db.CreateTableAsync<VehicleType>();
                }
                Task<int> deleted = db.ExecuteAsync("DELETE FROM VehicleType");
                var lstVehicleType = objDALPass.GetAllVehicleTypes(apitoken, LocationID);
                for (var item = 0; item < lstVehicleType.Count; item++)
                {
                    output = db.InsertAsync(lstVehicleType[item]);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public Task<List<VehicleType>> GetAllVehicleTypesInSQLLite()
        {
            try
            {
                return db.Table<VehicleType>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        #endregion

        #region Vehicle Parking Fees On Login
        public Task<int> SaveVehiclesParkingFeesDetailOnLogin(string accessToken, int LocationParkingLotID)
        {

            Task<int> output = null;
            DALCheckIn objdalCheckIn = new DALCheckIn();
            try
            {
                bool istableexist = IsTableExists(db, "VehicleParkingFee");
                if (!istableexist)
                {
                    db.CreateTableAsync<VehicleParkingFee>();
                }
                Task<int> deleted = db.ExecuteAsync("DELETE FROM VehicleParkingFee");
                var lstfees = objdalCheckIn.GetLotVehiclesParkingFeesDetailOnLogin(accessToken, LocationParkingLotID);
                for (var item = 0; item < lstfees.Count; item++)
                {
                    output = db.InsertAsync(lstfees[item]);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;


        }
        public Task<List<VehicleParkingFee>> GetLotVehiclesParkingFeesSQLLite()
        {
            try
            {
                return db.Table<VehicleParkingFee>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
