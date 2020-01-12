using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MobTech.Mobile.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MobTech.Mobile.Pages
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void AuthenticateButton(object sender, EventArgs e)
        {
            var user = new User();
            if (string.IsNullOrWhiteSpace(r_email.Text) && string.IsNullOrWhiteSpace(r_password.Text))
            {
                
                user.Email = l_email.Text;
                user.Password = l_password.Text;

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("http://192.168.1.65:7500/api/User/Login", content);

                if (result.IsSuccessStatusCode)
                {
                    ui_result.Text = "Success";
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    ui_result.Text = "Wrong credentials.";
                }               
            }
            else if (string.IsNullOrWhiteSpace(l_email.Text) && string.IsNullOrWhiteSpace(l_password.Text))
            {
                user.Email = r_email.Text;
                user.Password = r_password.Text;

                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync("http://192.168.1.65:7500/api/User/Register", content);

                if (result.IsSuccessStatusCode)
                {
                    ui_result.Text = "Success";
                }
                else
                {
                    ui_result.Text = "Account already exists.";
                }
            }
        }
    }
}
