using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using System.Diagnostics;
using Xamarin.Forms;

namespace ScheduleKK
{
    public static class Downloader
    {
        public static List<string> groupList;
        public static DataSet dataSet;
        public static DataTable dataTable;
        static IFolder folder;
        static string path;
        static string file = "schedule.xls";
        public static string url;

        public static async Task GetSchedule()
        {
            folder = await FileSystem.Current.LocalStorage.CreateFolderAsync("Schedule", CreationCollisionOption.ReplaceExisting);
            path = folder.Path;
            try
            {
                if (url != null && url != "")
                {
                    var webClient = new WebClient();
                    webClient.DownloadFile(new Uri(url), PortablePath.Combine(path, file));
                    ReadFile();
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("Нет URL");
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine("ERROR:" + ex.Message);
            }
        }
        public static void ReadFile()
        {
            using (var stream = File.OpenRead(PortablePath.Combine(path, file)))
            {
                var reader = ExcelReaderFactory.CreateReader(stream);
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };
                dataSet = reader.AsDataSet(conf);
                reader.Close();
            }

        }
        public static List<string> GetGroups(DataTable data)
        {
            bool flag = false;
            var res = new List<string>();
            for (int i = 3; i > 0; i++)
            {
                if (data.Rows[2][i].ToString() != "")
                {
                    res.Add(data.Rows[2][i].ToString());
                    flag = true;
                }
                else
                {
                    if (flag)
                    {
                        flag = false;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return res;
        }
    }
}
