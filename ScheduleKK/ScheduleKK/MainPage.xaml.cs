using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace ScheduleKK
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public class TitledPage
        { 
            public Page page { get; set; }
            public string Title { get; set; }

            public TitledPage(Page p)
            {
                page = p;
                Title = p.Title == null ? "" : p.Title;
            }

        }
        public MainPage()
        {
            InitializeComponent();

            List<TitledPage> pages = new List<TitledPage>() { new TitledPage(new SchedulePage()), new TitledPage(new SettingsPage()) };
            
            var template = new DataTemplate(() =>
            {
                var grid = new Grid();
                var optionLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 20, TextColor = Color.Black };

                optionLabel.SetBinding(Label.TextProperty, "Title");
                grid.Children.Add(optionLabel);

                return new ViewCell { View = grid };

            });
            var titles = new List<string>() { "Расписание", "Настройки" };
            menuList.ItemsSource = titles;
            menuList.ItemTapped += (s, e) =>
            {
                master.IsPresented = false;
                Detail = new NavigationPage(pages[e.ItemIndex].page) { BarTextColor = Color.White, BarBackgroundColor = Color.DarkKhaki };
            };
            menuList.BackgroundColor = Color.Khaki;
            menuList.Header = new Label() { Text = "Меню", FontSize = 30, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Color.DarkKhaki, TextColor = Color.White, Padding = new Thickness(5) };
            Detail = new NavigationPage(pages[0].page) { BarTextColor = Color.White, BarBackgroundColor = Color.DarkKhaki };

        }
    }
}
