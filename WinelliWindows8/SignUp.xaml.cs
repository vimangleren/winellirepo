using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarmPenguin.StorageBroker;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SignUp : PenguinDeals.Common.LayoutAwarePage
    {
        public SignUp()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private async void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var error = false;
            if (string.IsNullOrWhiteSpace(tbName.Text)) {
                tbError.Text = "* Name";
                error = true;
            }

            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                tbError.Text = "* Email";
                error = true;
            }

            if (string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                tbError.Text = "* Address";
                error = true;
            }

            if (string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                tbError.Text = "* Password";
                error = true;
            }

            
            if (!error) {
                User user = new User();
                user.Address = tbAddress.Text;
                user.Email = tbEmail.Text;
                user.ScreenName = tbName.Text;
                user.Password = tbPassword.Password;

                if (!await UsersData.UserExists(user.Email))
                {
                    await UsersData.CreateUser(user);

                    this.Frame.GoBack();
                }
                else {
                    tbError.Text = "* Email already exists";
                }
            }
        }
    }
}
