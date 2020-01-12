using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobTech.Mobile.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobTech.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditAED : ContentPage
	{      
        public EditAED ()
		{
            InitializeComponent();
            
        }

        public async void Refresh_Clicked(object sender, EventArgs e)
        {
            ObservableCollection<Defibrillator> defs = new ObservableCollection<Defibrillator>();
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://newmobtech.azurewebsites.net/api/AED/ReturnAED");
            defs = JsonConvert.DeserializeObject<ObservableCollection<Defibrillator>>(response);
            lv.ItemsSource = defs;
        }


        private async void Lv_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Defibrillator def = new Defibrillator();
            def = e.SelectedItem as Defibrillator;
            await Navigation.PushAsync(new AddAED(def));
        }
    }
}