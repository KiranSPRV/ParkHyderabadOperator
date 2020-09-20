using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
namespace ParkHyderabadOperator.ViewModel
{
  public  class VMMultipleStations
    {
        public VMMultipleStations()
        {
            Station = new Stations();
        }
        public Stations Station { get; set; } 
        public bool IsSelected { get; set; }
    }
}
