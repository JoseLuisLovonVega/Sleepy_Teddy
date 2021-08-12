using System.Collections.Generic;
using SleepyTeddy.ViewModel;
using Xamarin.Forms;

namespace SleepyTeddy.Resources
{
    public static class Globals
    {
        public static DevicePageViewModel DevicePageViewModel;
        public static MiCuentaPacienteViewModel MiCuentaPacienteViewModel;
        //public static HomePageViewModel HomePageViewModel;

        public static void BuildGlobals()
        {
            DevicePageViewModel = new DevicePageViewModel();
            MiCuentaPacienteViewModel = new MiCuentaPacienteViewModel();
            //HomePageViewModel = new HomePageViewModel();
        }
    }
}