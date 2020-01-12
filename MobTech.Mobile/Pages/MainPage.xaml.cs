using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobTech.Mobile.MenuItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobTech.Mobile.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : MasterDetailPage
	{
        public List<MasterPageItem> menuList { get; set; }
		public MainPage ()
		{
			InitializeComponent ();
            menuList = new List<MasterPageItem>();
            menuList.Add(new MasterPageItem() {Title = "Home", Icon = "" , TargetType = typeof(HomePage)});
            menuList.Add(new MasterPageItem() {Title = "Add AED", Icon = "" , TargetType = typeof(AddAED)});
            menuList.Add(new MasterPageItem() {Title = "Login", Icon = "" , TargetType = typeof(Login)});
            menuList.Add(new MasterPageItem() {Title = "Edit AED", Icon = "" , TargetType = typeof(EditAED)});
            menuList.Add(new MasterPageItem() {Title = "Video", Icon = "" , TargetType = typeof(Video)});

            navigationDrawerList.ItemsSource = menuList;

            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(HomePage)));

            this.BindingContext = new
            {
                Footer = "MobTech Assignment"
            };
        }

        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            var item = (MasterPageItem)e.SelectedItem;
            Type page = item.TargetType;

            Detail = new NavigationPage((Page)Activator.CreateInstance(page));
            IsPresented = false;
        }
	}
}