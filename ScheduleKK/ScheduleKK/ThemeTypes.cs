using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace ScheduleKK
{
    public class ThemeTypes
    {
        public static class NormalType
        {
            public static GridLength GridHeaderHeight = new GridLength(.55, GridUnitType.Star);
            public static GridLength GridRowHeight = new GridLength(.7, GridUnitType.Star);
            public static GridLength GridColumnWidth = new GridLength(.5, GridUnitType.Star);
            public static Grid GetGrid(int headers, int rows, int columns)
            {
                var grid = new Grid();
                for (int i = 0; i < headers; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() {Height = GridHeaderHeight});
                }
                for (int i = 0; i < rows; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridRowHeight });
                }
                for (int i = 0; i < columns; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridColumnWidth });
                }
                return grid;
            }


        }
        public static class CompactType
        {

        }
        public static class HugeType
        {

        }
    }
}
