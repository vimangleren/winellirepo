using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PenguinDeals
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class Login : PenguinDeals.Common.LayoutAwarePage
	{
		public Login()
		{
			this.InitializeComponent();
		}

		public async void FacebookLogin()
		{
			var facebookURL = "https://www.facebook.com/dialog/oauth?client_id=394984243875558";
			var callbackURL = "https://www.facebook.com/connect/login_success.html";
			facebookURL += "&redirect_uri=" + Uri.EscapeUriString(callbackURL) + "&display=popup&response_type=token";

			var startURI = new Uri(facebookURL);
			var endURI = new Uri(callbackURL);

			try
			{
				var callback = await WebAuthenticationBroker.AuthenticateAsync(
									Windows.Security.Authentication.Web.WebAuthenticationOptions.None,
									startURI,
									endURI);

				var response = callback.ResponseData;

				var searchFor = "access_token=";
				var findEqual = response.IndexOf(searchFor, 0) + searchFor.Length;
				var findAmper = response.IndexOf("&", findEqual);
				var length = findAmper - findEqual;
				var accessToken = response.Substring(findEqual, length);

				HttpClient client = new HttpClient();
				var me = await client.GetStringAsync("https://graph.facebook.com/me?access_token=" + accessToken);

				JObject json = JObject.Parse(me);

				var user = await UsersData.GetByToken(json["id"].ToString());
				if (user != null)
				{
					App.IsUserLoggedIn = true;
					App.LoggedInUser = user;

					this.Frame.GoBack();
				}
			}
			catch (Exception e) { }
		}


		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			FacebookLogin();
		}
	}
}
