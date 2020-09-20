using ParkHyderabadOperator.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParkHyderabadOperator.ViewModel
{
    public class VMLocationParkingSummaryReport : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public VMLocationParkingSummaryReport()
        {
            LocationParkingReportID = new List<LocationParkingReport>();
        }

        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string ParkingHours { get; set; }
        public string TotalIn { get; set; }
        public string TotalOut { get; set; }
        public string TotalCash { get; set; }
        public string TotalEpay { get; set; }
        public string TotalFOC { get; set; }
        public List<LocationParkingReport> LocationParkingReportID { get; set; }

        private bool _isExpandVisible { get; set; }
        private bool _isVisible { get; set; }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isExpandVisible = value;
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }
    }

}
