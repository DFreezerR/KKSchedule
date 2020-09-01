using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ScheduleKK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchedulePage : ContentPage
    {
        public static List<string> groupList;
        public static int neededRow = 2;
        public static DataTable dataTable;
        
        class Para
        {
            public string name;
            public string cabinet;
            public string time;

        }
        class Day
        {
            public string date;
            public string group;
            public string place;
            public List<Para> paras;
            public string building;
        }
        public Point FindColRow(DataTable table, string find)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j].ToString() == find)
                    {
                        return new Point(i, j);

                    }
                }
            }
            return new Point(0, 0);
        }
        
        private async Task<bool> GetStorageAccess()
        {
            bool result = false;
            var response = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            if (response == PermissionStatus.Granted)
            {
                result = true;
                Device.BeginInvokeOnMainThread(() => ScheduleStart());
            }
            return result;
        }
        
        public async void GetPermissions()
        {
            var res = await GetStorageAccess();
            if (!res)
            {
                System.Environment.Exit(0);
            }
        }
        public async void ScheduleStart()
        {
            if (CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>().Result == PermissionStatus.Granted)
            {
                await Task.Factory.StartNew(() => SettingsPage.SettingsInit());
                var res = await Task.Factory.StartNew(()=> Downloader.GetSchedule());

                if (res.IsCompleted && Downloader.dataSet != null)
                {
                    dataTable = Downloader.dataSet.Tables["Речная"];
                    groupList = Downloader.GetGroups(dataTable);
                    int neededCol;
                    SettingsPage.groupL = groupList;
                    SettingsPage.picker.ItemsSource = SettingsPage.groupL;
                    
                    if (SettingsPage.lastGroup != null && SettingsPage.lastGroup != "")
                    {
                        neededCol = (int)FindColRow(dataTable, SettingsPage.lastGroup).Y;
                        title.Text = $"Расписание {SettingsPage.lastGroup}";
                        SettingsPage.picker.SelectedItem = SettingsPage.lastGroup;
                    }
                    else
                    {
                        neededCol = (int)FindColRow(dataTable, groupList[0]).Y;
                        title.Text = $"Расписание {groupList[0]}";
                        SettingsPage.picker.SelectedItem = groupList[0];
                    }
                    
                    title.FontSize = 17;
                    title.TextColor = Color.White;

                    List<Day> days = new List<Day>();
                    List<Para> _paras = new List<Para>();
                    List<string> times = new List<string>();
                    List<string> paras = new List<string>();
                    List<string> dates = new List<string>();
                    List<Grid> grids = new List<Grid>();
                    List<string> places = new List<string>();
                    stack.Children.Clear();

                    for (int j = 0; j < dataTable.Rows.Count; j++)
                    {

                        if (dataTable.Rows[j][0].ToString() != "")
                        {
                            dates.Add(dataTable.Rows[j][0].ToString());
                        }

                    }
                    for (int j = 5; j < dataTable.Rows.Count; j++)
                    {

                        if (dataTable.Rows[j][2].ToString() != "")
                        {
                            times.Add(dataTable.Rows[j][2].ToString());
                        }

                    }
                    for (int day = 0; day < 5; day++)
                    {

                        if (day == 0)
                        {
                            places.Add(dataTable.Rows[neededRow + 2][neededCol].ToString());
                            for (int i = neededRow; i < neededRow + 3 + 8; i++)
                            {

                                if (i == neededRow + 1)
                                {
                                    continue;
                                }
                                if (dataTable.Rows[i][neededCol].ToString() != "" && i > neededRow + 2)
                                {
                                    paras.Add(dataTable.Rows[i][neededCol].ToString().Replace("\n", " ") + "%" +
                                        dataTable.Rows[i][2].ToString() + "%" +
                                        dataTable.Rows[i][neededCol + 1]);
                                }
                                else
                                {
                                    paras.Add(dataTable.Rows[i][neededCol].ToString() + "%" +
                                        dataTable.Rows[i][2].ToString());
                                }
                            }
                        }
                        else
                        {
                            places.Add(dataTable.Rows[neededRow + (10 * day) + 2][neededCol].ToString());
                            for (int i = neededRow + (10 * day) + 1; i < neededRow + (10 * (day + 1)) + 1; i++)
                            {
                                if (dataTable.Rows[i][neededCol].ToString() != "" && i > neededRow + (10 * day) + 2)
                                {
                                    if (dataTable.Rows[i][neededCol + 1].ToString() == "")
                                    {
                                        paras.Add(dataTable.Rows[i][neededCol].ToString().Replace("\n", " ") + "%" +
                                        dataTable.Rows[i][2].ToString() + "%" + "_NOCABINET_");
                                    }
                                    else
                                    {

                                        paras.Add(dataTable.Rows[i][neededCol].ToString().Replace("\n", " ") + "%" +
                                        dataTable.Rows[i][2].ToString() + "%" +
                                        dataTable.Rows[i][neededCol + 1]);
                                    }
                                }
                                else
                                {
                                    paras.Add(dataTable.Rows[i][neededCol].ToString() + "%" +
                                        dataTable.Rows[i][2].ToString());
                                }
                            }
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 2 + 10 * i; j < 10 + 10 * i; j++)
                        {
                            string full_para = paras[j];
                            string[] para_parts = full_para.Split(new char[] { '%' });
                            if (para_parts[0] != "")
                            {
                                string name = para_parts[0];
                                string time = para_parts[1];
                                string cab = para_parts[2] == "_NOCABINET_" ? "" : para_parts[2];

                                _paras.Add(new Para()
                                {
                                    cabinet = cab,
                                    name = name,
                                    time = time

                                });
                            }
                            else
                            {
                                _paras.Add(new Para()
                                {
                                    cabinet = "",
                                    name = "",
                                    time = para_parts[1]

                                });
                            }
                        }
                        days.Add(new Day()
                        {
                            group = paras[0 + 10 * i],
                            place = paras[1 + 10 * i],
                            paras = new List<Para>(_paras),
                            date = dates[i],
                            building = places[i]

                        });
                        _paras.Clear();
                    }
                    for (int i = 0; i < days.Count; i++)
                    {
                        grids.Add(new Grid()
                        {
                            RowDefinitions =
                        {
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)},
                            new RowDefinition(){ Height = new GridLength(.5, GridUnitType.Star)}
                        },
                            ColumnDefinitions =
                        {
                            new ColumnDefinition(){ Width = new GridLength(.5, GridUnitType.Star)},
                            new ColumnDefinition(){ Width = new GridLength(1, GridUnitType.Star)},
                            new ColumnDefinition(){ Width = new GridLength(.5, GridUnitType.Star)}
                        }
                        });
                        Label header = new Label() { BackgroundColor = Color.Khaki, FontSize = 20, Padding = new Thickness(5), Text = days[i].date + "\n" + days[i].building, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.Black };
                        grids[i].Children.Add(header, 0, 0);
                        grids[i].Children.Add(new Label() { BackgroundColor = Color.Khaki, FontSize = 20, Padding = new Thickness(5), Text = "Время", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.Black }, 0, 1);
                        grids[i].Children.Add(new Label() { BackgroundColor = Color.Khaki, FontSize = 20, Padding = new Thickness(5), Text = "Пара", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.Black }, 1, 1);
                        grids[i].Children.Add(new Label() { BackgroundColor = Color.Khaki, FontSize = 20, Padding = new Thickness(5), Text = "Кабинет", HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.Black }, 2, 1);
                        Grid.SetColumnSpan(header, 3);
                        grids[i].ColumnSpacing = 3;
                        grids[i].Margin = new Thickness(10);
                        for (int t = 2; t < grids[i].RowDefinitions.Count; t++)
                        {
                            for (int j = 0; j < grids[i].ColumnDefinitions.Count; j++)
                            {
                                if (j == 0)
                                {
                                    grids[i].Children.Add(new Label() { BackgroundColor = Color.FromRgba(0, 0, 0, .05), Padding = new Thickness(5), Text = days[i].paras[t - 2].time, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.Black }, j, t);
                                }
                                if (j == 1)
                                {
                                    grids[i].Children.Add(new Label() { BackgroundColor = Color.FromRgba(0, 0, 0, .05), Padding = new Thickness(5), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = days[i].paras[t - 2].name, TextColor = Color.Black }, j, t);
                                }
                                if (j == 2)
                                {
                                    grids[i].Children.Add(new Label() { BackgroundColor = Color.FromRgba(0, 0, 0, .05), Padding = new Thickness(5), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = days[i].paras[t - 2].cabinet, TextColor = Color.Black }, j, t);
                                }
                            }
                        }

                        stack.Children.Add(grids[i]);
                    }
                    DependencyService.Get<IMessage>().ShortAlert("Расписание обновлено");
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("Расписание не скачано");
                }
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert("Нет доступа");
            }

        }
        public SchedulePage()
        {
            InitializeComponent();
            refreshView.Command = new Command(() =>
            {
                ScheduleStart();
                refreshView.IsRefreshing = false;
            });
            Task.Run(()=>GetPermissions());
        }
    }
}
