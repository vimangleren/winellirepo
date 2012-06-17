using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarmPenguin.StorageBroker;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace PenguinDeals
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static DealBasket _DealBasket = new DealBasket();
     
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(FrontPage));

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var loginCmd = new SettingsCommand("login", "Login", new Windows.UI.Popups.UICommandInvokedHandler(x =>
            {
                ShowLoginScreen();
            }));

            var signUpCmd = new SettingsCommand("sign up", "Sign Up", new Windows.UI.Popups.UICommandInvokedHandler(x =>
            {
                ShowSignUpScreen();
            }));

            args.Request.ApplicationCommands.Clear();
            args.Request.ApplicationCommands.Add(loginCmd);
            args.Request.ApplicationCommands.Add(signUpCmd);
        }

        void ShowSignUpScreen()
        {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(typeof(SignUp));
        }

        void ShowLoginScreen()
        {
            var frame = (Frame)Window.Current.Content;
            frame.Navigate(typeof(Login));
        }


        public static User LoggedInUser { get; set; }
        public static bool IsUserLoggedIn { get; set; }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }
    }
}
