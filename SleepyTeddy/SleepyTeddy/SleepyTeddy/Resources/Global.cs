using System.Collections.Generic;
using SleepyTeddy.Services;
using SleepyTeddy.ViewModel;
using Xamarin.Forms;

namespace SleepyTeddy.Resources
{
    public static class Global
    {
        public static SamplesService SamplesService { get; private set; }
        public static MiCuentaPacienteViewModel MiCuentaPacienteViewModel;
        public static DevicePageViewModel DevicePageViewModel;

        public static void BuildGlobal()
        {
            MiCuentaPacienteViewModel = new MiCuentaPacienteViewModel();
            DevicePageViewModel = new DevicePageViewModel();
            SamplesService = new SamplesService();
        }
    }
}