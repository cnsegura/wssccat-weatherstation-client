using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
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
        private SensorData data = new SensorData();
        private HttpClient sensorClient;
        private HttpBaseProtocolFilter sensorFilter = new HttpBaseProtocolFilter();
        private Random _random = new Random();
        private Uri sensorUri = new Uri("http://wssccat:1038");
        public List<NameValueItem> items = new List<NameValueItem>();
        //public Collection<NameValueItem> items = new Collection<NameValueItem>();
        public MainPage()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            this.InitializeComponent();
            
            //Create timeer based ThreadPool task to get data from sensors and update UI
            ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await ClearScreen();
                GetData();

                await UpdateScreen(data.TimeStamp, data.FahrenheitTemperature);

            }, TimeSpan.FromSeconds(2));


        }


        private async Task ClearScreen(CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
        {
            await Dispatcher.RunAsync(priority, () => { Status.Text = ""; });
        }
        async void GetData()
        {
            try
            {
                //using (HttpResponseMessage response = await sensorClient.GetAsync(sensorUri))
                bool TEMP = false;
                {
                    if (TEMP == true)
                        //(response.IsSuccessStatusCode)
                    {
                        //TODO PARSE JSON ETC.
                    }
                    else
                    {
                        data.FahrenheitTemperature = _random.Next(95, 100);
                        data.TimeStamp = DateTimeOffset.Now.ToLocalTime().ToString();
                        
                        items.Add(new NameValueItem { Name = data.TimeStamp, Value = data.FahrenheitTemperature });
                        /*
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
                            */
                    }
                }
            }
            catch (Exception e)
            {
                //TODO LOG TO SCREEN
            }
            finally
            {
                //TODO LOG TO SCREEN
            }
        }

        private async void LogToScreen (string text, CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
        {
            await Dispatcher.RunAsync(priority, () => { Status.Text += text + "\n"; });
        }
        private async Task UpdateScreen(string xValue, int yValue, CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
        {
            await Dispatcher.RunAsync(priority, () =>
            {
                int i;
                i = items.Count();
                LogToScreen("Fake temp is: " + data.FahrenheitTemperature + "\tTime Stamp is: " + data.TimeStamp + "\tCount is " +i);
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
            });
        }
        public class NameValueItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public sealed class SensorData
        {
            public SensorData()
            {
                TimeStamp = DateTimeOffset.Now.ToLocalTime().ToString();
            }
            public float BarometricPressure { get; set; }
            public float CelciusTemperature { get; set; }
            public int FahrenheitTemperature { get; set; } 
            public float Humidity { get; set; }
            public string TimeStamp { get; set; }
        }

        private void Status_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
