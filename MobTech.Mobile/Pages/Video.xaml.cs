
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobTech.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Video : ContentPage
	{
		public Video ()
		{
			InitializeComponent ();
            
        }

        protected override void OnAppearing()
        {
            Uri myUri = new Uri("https://www.youtube.com/watch?v=fb29LCjX4-E&t=81s", UriKind.Absolute);
            base.OnAppearing();
            Device.OpenUri(myUri);
        }
    }
}