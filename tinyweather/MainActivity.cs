using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace tinyweather
{
    [Activity(Label = "tinyweather", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.MyButton);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }

        [Java.Interop.Export("onClickTest")]
        public void onClickTest(View v)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string key = "01f9bf4a3d561a37";
            string URL = "http://api.wunderground.com/api/" + key + "/conditions/lang:DL/q/Germany/Leipzig.json";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;


            string json_data = "";
            if(response != null)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json_data = reader.ReadToEnd();
                }
            }
            watch.Stop();
            long timeForHTTPRequest = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();

            TextView textv = FindViewById<TextView>(Resource.Id.textView_weatherdata);
            
            if (json_data.Length > 0)
            {
                dynamic weather_data = JsonConvert.DeserializeObject<dynamic>(json_data);
                if (weather_data != null)
                {
                    string city     = weather_data.current_observation?.display_location?.city;
                    float temp_c    = weather_data.current_observation?.temp_c;
                    float wind = weather_data.current_observation?.wind_kph;
                    //float wind      = weather_data.current_observation?.wind_kph;

                    textv.Text = "Aktuelle Temperatur in " + city + ": " + temp_c + "°C";
                    textv.Text += "\n" + weather_data.current_observation?.weather;
                    textv.Text += "\n\nWind: " + wind + "km/h";
                    if(wind > 0.0)
                        textv.Text += " aus Richtung " + weather_data.current_observation?.wind_dir;
                    textv.Text += "\nLuftfeuchtigkeit: " + weather_data.current_observation?.relative_humidity;
                    //textv.Text += "\nData that doesnt exist: " + weather_data.currenure?.temp_c + "°C";

                    watch.Stop();
                    textv.Text += "\n\n\n\n\n\nHttp request took: " + timeForHTTPRequest + "ms";
                    textv.Text += "\nJSON parsing took: " + watch.ElapsedMilliseconds + "ms";
                }
                else
                    textv.Text = "JSON deserialization went horribly wrong!";
            }
            else
            {
                textv.Text = "Current temperature data could not be received.";
            }
        }
    }
}

