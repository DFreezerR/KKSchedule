using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScheduleKK
{
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            //TODO--------------------------------------------------------------------------------
            //1:URL AUTO DETECTOR-----------------------------------------------------------------
            //2:THEMES----------------------------------------------------------------------------
            //#:RANDOMIZERS-----------------------------------------------------------------------
            List<Page> pages = new List<Page> { new SchedulePage(), new SettingsPage() };
            menuList.ItemsSource = pages;
            menuList.ItemTapped += async(s, e) =>
            {
                master.IsPresented = false;
                await Task.Delay(200);
                Detail = new NavigationPage(pages[e.ItemIndex]) { BarTextColor = Color.White, BarBackgroundColor = Color.DarkKhaki };
            };
            menuList.BackgroundColor = Color.Khaki;
            menuList.Header = new Label() { Text = "Меню", FontSize = 30, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Color.DarkKhaki, TextColor = Color.White, Padding = new Thickness(5) };
            Detail = new NavigationPage(pages[0]) { BarTextColor = Color.White, BarBackgroundColor = Color.DarkKhaki };

        }
    }
}
