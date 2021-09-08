using Acr.UserDialogs;
using SleepyTeddy.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SleepyTeddy.Resources;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SleepyTeddy.Data.Repository;
using SleepyTeddy.Data;
using System.Diagnostics;

namespace SleepyTeddy
{
    public partial class App : Application
    {
        public static Task Navigation { get; internal set; }

        public App()
        {
            InitializeComponent();
            var database = new Database();
            Debug.WriteLine("Fecha actual: " + DateTime.Now);
            Debug.WriteLine("Fecha actual UTC: " + DateTime.UtcNow);
            Debug.WriteLine("Fecha actual menos 5 horas: " + DateTime.Now.AddHours(-5));
            Globals.BuildGlobals(new HeartrateRepository(database), new SleepRepository(database), new StepsRepository(database), database);
            //database.EmptyDatabase();
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
        public static async void RequestLocationPermission()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (permissionStatus != PermissionStatus.Granted)
            {
                await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            }
        }
        public void CreateDatabase()
        {

        }
    }
}
