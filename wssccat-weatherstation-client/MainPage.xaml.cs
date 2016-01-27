using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls;
using WinRTXamlToolkit.Controls.DataVisualization;
using WinRTXamlToolkit.Controls.Common;
using WinRTXamlToolkit.Controls.Extensions;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace wssccat_weatherstation_client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            UpdateCharts();
        }
        private Random _random = new Random();

        private void UpdateCharts()
        {
            List<NameValueItem> items = new List<NameValueItem>();
            items.Add(new NameValueItem { Name = "Test1", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test2", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test3", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test4", Value = _random.Next(10, 100) });
            items.Add(new NameValueItem { Name = "Test5", Value = _random.Next(10, 100) });

            ((LineSeries)LineChartWithAxes.Series[0]).ItemsSource = items;
            ((LineSeries)LineChartWithAxes.Series[0]).DependentRangeAxis =
                new LinearAxis
                {
                    Minimum = 0,
                    Maximum = 100,
                    Orientation = AxisOrientation.Y,
                    Interval = 20,
                    ShowGridLines = true
                };
        }

        public class NameValueItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
