using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NFCPopup : PopupPage
    {
		public NFCPopup ()
		{
			InitializeComponent ();
		}

        

        private async void ImgbtnClose_Clicked(object sender, EventArgs e)
        {
            try
            {
               await PopupNavigation.PopAsync(true);

            }
            catch (Exception ex)
            {
            }
        }
    }
}