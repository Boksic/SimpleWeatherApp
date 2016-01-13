using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Locations;
using Android.Util;
using Android.Runtime;
using Android.Content;

using System;
using System.Runtime.Remoting.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace AppExpert1.Droid
{
	[Activity (Label = "AppExpert1", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity, ILocationListener
	{
		LocationManager lManager;
		Location location;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);

			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			EditText codePostale = FindViewById<EditText>(Resource.Id.codePostale);
			Button buttonCodePostale = FindViewById<Button>(Resource.Id.buttonCodePostale);

			buttonCodePostale.Click += async (sender, e) =>  
			{
				string queryStringCodePostale = "https://query.yahooapis.com/v1/public/yql?q=select+*+from+weather.forecast+where+location=" + codePostale.Text + "%20and+u='c'&format=json";

				Core.queryString = queryStringCodePostale;

				Weather weather = await Core.GetWeather(codePostale.Text);

				writeDataOnPage(weather);
			};

			AutoCompleteTextView villeName = FindViewById<AutoCompleteTextView>(Resource.Id.villeName);
			Button buttonVilleName = FindViewById<Button>(Resource.Id.buttonVilleName);

			ArrayAdapter adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleSpinnerItem, VilleName.cities);

			villeName.Adapter = adapter;

			buttonVilleName.Click += async (sender, e) =>  
			{
				string queryStringVilleName = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + villeName.Text + "%22)and+u='c'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

				Core.queryString = queryStringVilleName;

				Weather weather = await Core.GetWeather(villeName.Text);

				writeDataOnPage(weather);
			};

			Button buttonGeolocalisation = FindViewById<Button>(Resource.Id.buttonGeo);
			buttonGeolocalisation.Enabled = false;

			InitializeLocationManager();

			if (this.location != null) 
			{
				buttonGeolocalisation.Enabled = true;
			}

			buttonGeolocalisation.Click += async (sender, e) =>
			{
				string queryStringGeolocalisation = "http://query.yahooapis.com/v1/public/yql?q=select+*+from+geo.placefinder+where+text=%22" + location.Latitude.ToString() + "," + location.Longitude.ToString() + "%22+and+gflags=%22R%22&format=json";

				System.Diagnostics.Debug.WriteLine("String = " + queryStringGeolocalisation);

				Core.queryString = queryStringGeolocalisation;

				Weather weather = await Core.GetWeatherByPosition();

				System.Diagnostics.Debug.WriteLine("Ville = " + weather.Line2);

				string queryStringVilleName = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22" + weather.Line2.ToString() + "%22)and+u='c'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

				Core.queryString = queryStringVilleName; 

				Weather weather2 = await Core.GetWeather(weather.Line2.ToString());

				writeDataOnPage(weather2);
			};
		}

		void InitializeLocationManager()
		{
			lManager = (LocationManager) GetSystemService(LocationService);

			lManager.RequestSingleUpdate (LocationManager.GpsProvider, this, null);
			lManager.RequestLocationUpdates (LocationManager.GpsProvider, 2000, 0, this);
		}

		//Met à jour les TextViews de la page
		private void writeDataOnPage(Weather weather)
		{
			TextView title = FindViewById<TextView> (Resource.Id.ResultsTilte);
			TextView temperature = FindViewById<TextView> (Resource.Id.textViewTemp);
			TextView vent = FindViewById<TextView> (Resource.Id.textViewWent);
			TextView humidite = FindViewById<TextView> (Resource.Id.textViewhumidite);
			TextView visibilite = FindViewById<TextView> (Resource.Id.textViewWent);
			TextView leveSoleil = FindViewById<TextView> (Resource.Id.textViewSunRise);
			TextView coucheSoleil = FindViewById<TextView> (Resource.Id.textViewSunset);
			ImageView weatherImage = FindViewById<ImageView> (Resource.Id.weatherImage);

			if (weather != null) 
			{
				title.Text = weather.Title;
				temperature.Text = weather.Temperature;
				vent.Text = weather.Wind;
				humidite.Text = weather.Humidity;
				visibilite.Text = weather.Visibility;
				leveSoleil.Text = weather.Sunset;
				coucheSoleil.Text = weather.Sunrise;

				string imageURL = weather.ImagePath;

				Square.Picasso.Picasso.With (this).Load (imageURL).Into (weatherImage);
			} 
			else 
			{
				title.Text = "Pas de resultats !!!";
			}
		}

		public void OnLocationChanged(Location location) 
		{
			this.location = location;
			Button buttonGeolocalisation = FindViewById<Button>(Resource.Id.buttonGeo);
			buttonGeolocalisation.Enabled = true;
		}

		public void OnProviderDisabled(string provider) {}

		public void OnProviderEnabled(string provider) {}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {}
	}
}