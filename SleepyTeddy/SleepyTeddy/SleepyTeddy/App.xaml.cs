using Acr.UserDialogs;
using SleepyTeddy.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WindesHeartApp.Data;
using WindesHeartApp.Data.Repository;
using WindesHeartApp.Resources;

namespace SleepyTeddy
{
    public partial class App : Application
    {
        public static Task Navigation { get; internal set; }

        public App()
        {
            InitializeComponent();
            var database = new Database();
            Globals.BuildGlobals(new HeartrateRepository(database), new SleepRepository(database), new StepsRepository(database), database);
            MainPage = new NavigationPage(new MainPageLogin());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
