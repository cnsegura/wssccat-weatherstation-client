using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
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
using Windows.Web.Http.Headers;
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
        public ObservableCollection<NameValueItem> items = new ObservableCollection<NameValueItem>();
        //Production private Uri baseUri = new Uri("http://wssccatiot.westus.cloudapp.azure.com:8080/topic");

        public MainPage()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            this.InitializeComponent();
            
            //Create timeer based ThreadPool task to get data from sensors and update UI
            ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await ClearScreen();
                var item = await GetGraphData();

                if (item != null)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => items.Add(item));
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => UpdateScreen());
                await PostDataAsync();

            }, TimeSpan.FromSeconds(2));


        }


        private async Task ClearScreen(CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
        {
            await Dispatcher.RunAsync(priority, () => { Status.Text = ""; });
        }
        async Task<NameValueItem> GetGraphData()
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
                        return null;
                    }
                    else
                    {
                        data.FahrenheitTemperature = _random.Next(75, 100);
                        data.TimeStamp = DateTimeOffset.Now.ToLocalTime().ToString();
                        data.BarometricPressure = _random.Next(20, 100);
                        data.Humidity = _random.Next(20, 100);
                        return new NameValueItem { Name = data.TimeStamp, Value = data.FahrenheitTemperature };
                       
                    }
                }
            }
            catch (Exception e)
            {
                //TODO LOG TO SCREEN
                return null;
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
        private void UpdateScreen()
        {
            LogToScreen("Fake temp is: " + data.FahrenheitTemperature + "\tTime Stamp is: " + data.TimeStamp + "\tBarometric Pressure is: " + data.BarometricPressure);

            if (items.Count > 4)
            {
                int i = items.Count;
                int j = 0;
                while (i > 4) //limit number of points graphed to screen to 4
                {
                    --i;
                    items.RemoveAt(j++); //remove all items starting from the oldest and moving up until we have an array the size we want
                }
                 
            }
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
            topGauge.Value = data.BarometricPressure;
            bottomGauge.Value = data.Humidity;
        }
        private async Task PostDataAsync()
        {
            string topicString = "/SensorData";
            UriBuilder u1 = new UriBuilder();
            u1.Host = "localhost";
            u1.Port = 8080;
            u1.Path = "topic" + topicString;
            u1.Scheme = "http";
            Uri topicUri = u1.Uri;
            string json = JsonConvert.SerializeObject(data, Formatting.None);
            //Currently focused on REST API surface for Confluent.io Kafka deployment. We can make this more generic in the future
            HttpClient httpClient = new HttpClient();
            try
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/vnd.kafka.v1+json"));
                HttpResponseMessage postResponse = await httpClient.PostAsync(topicUri, new HttpStringContent(json, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/vnd.kfka.v1+json"));
            }
            catch
            {
                LogToScreen("POST failure");
            }

        }
        public class NameValueItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public partial class SensorData
        {
            public SensorData()
            {
                TimeStamp = DateTimeOffset.Now.ToLocalTime().ToString();
            }
            public int BarometricPressure { get; set; }
            public float CelciusTemperature { get; set; }
            public int FahrenheitTemperature { get; set; } 
            public int Humidity { get; set; }
            public string TimeStamp { get; set; }
        }

        private void Status_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
