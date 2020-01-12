using MobTech.Mobile.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MobTech.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		public HomePage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.7333304,26.83333),Distance.FromKilometers(20)));
            GetPins();
          
        }

        public async void GetPins()
        {
            ObservableCollection<Defibrillator> defs = new ObservableCollection<Defibrillator>();
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://newmobtech.azurewebsites.net/api/AED/ReturnAED");
            defs = JsonConvert.DeserializeObject<ObservableCollection<Defibrillator>>(response);
            foreach (var def in defs)
            {
                var pin = new Pin
                {
                    Position =  new Position(def.Latitude,def.Longitude),
                    Label =  def.Name,
                    Address = def.Description,                  
                };
                MainMap.Pins.Add(pin);
                pin.InfoWindowClicked += async (sender, e) => {
                  var test = await DisplayActionSheet("Επιλογή", "Ακύρωση", null, "Αναφορά προβ",
                      "Οδηγίες");
                    var action = await DisplayAlert(null,"Αναφορά προβλήματος","Ναι","Όχι");
                  if (action)
                  {
                      string type = await DisplayActionSheet("Πρόβλημα", "Ακύρωση", null, "Eκτός λειτουργίας",
                          "Aπουσία απινιδωτή");
                      string result = await DisplayPromptAsync("Σχόλιο", null);
                      var report = new Report();
                      report.ProblemType = type;
                      report.Comment = result;
                      report.Name = pin.Label;
                      var json = JsonConvert.SerializeObject(report);
                      var content = new StringContent(json, Encoding.UTF8, "application/json");
                      var response2 = await httpClient.PostAsync("https://newmobtech.azurewebsites.net/api/AED/Report",content);
                      if (response2.IsSuccessStatusCode)
                      {
                          await DisplayAlert("Μύνημα", "Επιτυχία", "OK");
                      }
                      else
                      {
                          await DisplayAlert("Μύνημα", "Κάτι πήγε λάθος!", "OK");
                      }
                    }
                };
            }

          
        }
    }
}