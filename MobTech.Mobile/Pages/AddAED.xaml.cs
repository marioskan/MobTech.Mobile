using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Android;
using Android.Content.PM;
using Android.Hardware;
using Java.IO;
using Permission = Plugin.Permissions.Abstractions.Permission;
using Plugin.Permissions.Abstractions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Text;
using Windows.Media.Audio;
using Windows.UI.Xaml.Media.Imaging;
using Android.Provider;
using MobTech.Mobile.Entities;
using File = Java.IO.File;

namespace MobTech.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddAED : ContentPage
	{
        public string FilePath { get; set; }
        public string PreviousName { get; set; }
        public Stream Stream { get; set; }
        public string NewFilePath { get; set; }
        public int id { get; set; }
        public bool Changed { get; set; } = false;

		public  AddAED()
		{
			InitializeComponent();
        }

        public AddAED(Defibrillator def)
        {
            InitializeComponent();
            var client2 = new HttpClient();
            PreviousName = def.Name;
            var url = "https://newmobtech.azurewebsites.net/api/AED/GetID?name=" + def.Name;
            var result = client2.GetStringAsync(url);
            id = Int32.Parse(result.Result);
            Changed = true;
            aedName.Text = def.Name;
            aedDesc.Text = def.Description;
            lon.Text = def.Longitude.ToString();
            lat.Text = def.Latitude.ToString();
            PhotoImage.Source = "https://mobtechdef.blob.core.windows.net/mobtechcontainer/"+def.Photograph;
        }

        public async void CameraButton(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            var cameraStatus2 = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
            var storageStatus2 = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted && storageStatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Message", "Unavailable", "OK");
                return;
            }
            else
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    SaveToAlbum = true,
                    Directory = "MobTech"
                });
                PhotoImage.Source = ImageSource.FromStream(() => file.GetStream());
                Stream = file.GetStream();
                FilePath = file.Path;
            }
            /// HERE ^
        }

        public async void CalculatePosition(object sender, EventArgs e)
        {
            load.IsRunning = true;
            await RetreiveLocation();
            load.IsRunning = false;
        }

        private async void SaveAED(object sender, EventArgs e)
        {
            var account = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=mobtechdef;AccountKey=q6g5XNLUjRzXETkx9KMUXg+erf7ELKsW0PhmvasP0rS+ZFhUHTjZQCJ/oCng05CR7hgvBbwpyYpRPbr/5gVcXA==;EndpointSuffix=core.windows.net");
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("mobtechcontainer");
            var blockBlob = container.GetBlockBlobReference(PreviousName +".png"); 
            var def = new Defibrillator();            
          
            blockBlob = container.GetBlockBlobReference(aedName.Text+".png");
            await UploadImage(blockBlob);
            def.Name = aedName.Text.ToString();
            def.Description = aedDesc.Text.ToString();
            def.Latitude = (float) Convert.ToDouble(lat.Text.ToString());
            def.Longitude = (float) Convert.ToDouble(lon.Text.ToString());
            def.Photograph = aedName.Text.ToString()+".png";
            var client2 = new HttpClient();
            if (Changed)
            {
                UpdatedAED newAed = new UpdatedAED();
                newAed.Name = def.Name;
                newAed.Description = def.Description;
                newAed.Latitude = def.Latitude;
                newAed.Longitude = def.Longitude;
                newAed.Photograph = def.Photograph;
                newAed.SearchedID = id;
                var json2 = JsonConvert.SerializeObject(newAed);
                var content2 = new StringContent(json2, Encoding.UTF8, "application/json");
                var result2 = await client2.PostAsync("https://newmobtech.azurewebsites.net/api/AED/Update", content2);
                if (result2.IsSuccessStatusCode)
                {
                    await DisplayAlert("Message", "Success", "OK");
                    await Navigation.PushAsync(new HomePage());                   
                }
                else
                {
                    await DisplayAlert("Message", "Something went wrong!", "OK");
                    await Navigation.PushAsync(new HomePage());
                }
            }
            else
            {
                var json = JsonConvert.SerializeObject(def);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client2.PostAsync("https://newmobtech.azurewebsites.net/api/AED/Add", content);
                if (result.IsSuccessStatusCode)
                {
                    await DisplayAlert("Message", "Success", "OK");
                }
                else
                {
                    await DisplayAlert("Message", "Something went wrong!", "OK");
                }
            }
            
        }

        public  async Task UploadImage(CloudBlockBlob blob)
        {
            
                load.IsRunning = true;
                await blob.UploadFromStreamAsync(Stream);
                load.IsRunning = false;
            
        }

        private async Task RetreiveLocation()
        {
            if (CrossGeolocator.IsSupported)
            {
                var locator = CrossGeolocator.Current;
                

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5),null,true);

                lat.Text = position.Latitude.ToString();
                lon.Text = position.Longitude.ToString();
            } 
         
        }

	}
}