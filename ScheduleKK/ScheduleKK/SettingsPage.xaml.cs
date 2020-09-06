
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Picker = Xamarin.Forms.Picker;
using PCLStorage;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Entry = Xamarin.Forms.Entry;
using System;

namespace ScheduleKK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public static Picker picker;
        public static List<string> groupL;
        public static string lastGroup;
        public static Entry urlEntry;
        public SettingsPage()
        {
            InitializeComponent();
            Label groupsLabel = new Label() { Text = "Группы:", FontSize = 20, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 5.0)};
            Label urlLabel = new Label() { Text = "Ссылка:", FontSize = 20, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 5.0) };
            urlEntry = new Entry();
            picker = new Picker();
            refreshSettings.Command = new Command(async () =>
            {
                /*await Downloader.GetSchedule();
                groupL = Downloader.GetGroups(Downloader.dataSet.Tables["Речная"]);
                picker.ItemsSource = groupL;*/
                refreshSettings.IsRefreshing = false;
            });
            picker.Title = "Выберите группу";
            urlEntry.Placeholder = "Введите ссылку на расписание";
            accept.Margin = new Thickness(0, 0, 10, 0);
            stackSettings.Children.Add(groupsLabel);
            stackSettings.Children.Add(picker);
            stackSettings.Children.Add(urlLabel);
            stackSettings.Children.Add(urlEntry);
        }
        public static async Task SettingsInit()
        {
            var fileCheck = await FileSystem.Current.LocalStorage.CheckExistsAsync("Settings");
            if (fileCheck == ExistenceCheckResult.FolderExists)
            {
                var file = await (await FileSystem.Current.LocalStorage.GetFolderAsync("Settings")).GetFileAsync("config.json");
                string json = "";
                JObject items;
                using (StreamReader r = new StreamReader(file.Path))
                {
                    json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<JObject>(json);
                    
                }
                if (items.ContainsKey("lastGroup"))
                {
                    lastGroup = items["lastGroup"].ToString();
                    picker.SelectedItem = lastGroup;
                }
                if (items.ContainsKey("url"))
                {
                    Downloader.url = items["url"].ToString();
                    urlEntry.Text = items["url"].ToString();
                }
            }
            else
            {
                lastGroup = "";
                Downloader.url = "";
                picker.SelectedIndex = -1;
                DependencyService.Get<IMessage>().ShortAlert("Настройте программу.");
            }
        }
        private static async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            if (picker.SelectedIndex == -1 && (urlEntry.Text == "" || urlEntry.Text == null))
            {
                DependencyService.Get<IMessage>().ShortAlert("Нечего сохранять");
                return;
            }
            string url = "";
            string notify = "";
            bool result = false;
            Uri uriResult;
            var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync("config.json", CreationCollisionOption.OpenIfExists);
            var obj = new JObject();
            if (picker.SelectedIndex != null && picker.SelectedIndex > -1)
            {
                lastGroup = picker.SelectedItem.ToString();
                obj["lastGroup"] = lastGroup;
            }
            if (urlEntry.Text != "" && urlEntry.Text != null)
            {
                url = urlEntry.Text.Trim();
                result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result && uriResult.Host == Downloader.host)
                {
                    obj["url"] = url;
                }
                else
                {
                    notify = "Проверьте URL.";
                }
            }
            string json = JsonConvert.SerializeObject(obj);
            await file.WriteAllTextAsync(json);
            notify = notify == "" ? "Настройки сохранены." : notify;
            DependencyService.Get<IMessage>().ShortAlert(notify);
        }
    }
    public interface IMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);

    }

}