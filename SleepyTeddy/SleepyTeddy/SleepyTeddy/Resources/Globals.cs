using System.Collections.Generic;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Services;
using Xamarin.Forms;
using SleepyTeddy.Data;
using SleepyTeddy.Data.Interfaces;

namespace SleepyTeddy.Resources
{
    public static class Globals
    {
        public static SamplesService SamplesService { get; private set; }
        public static MiCuentaPacienteViewModel MiCuentaPacienteViewModel;
        public static DevicePageViewModel DevicePageViewModel;
        public static SleepPageViewModel SleepPageViewModel;
       // public static List<SleepRecordsView> listSleepRecordsGlobal;
        public static double ScreenHeight { get; set; }
        public static double ScreenWidth { get; set; }
        public static string patientID;
        public static Color PrimaryColor { get; set; } = Color.FromHex("#96d1ff");
        public static Color SecondaryColor { get; set; } = Color.FromHex("#53b1ff");
        public static Color LightTextColor { get; set; } = Color.FromHex("#999999");

        public static IStepsRepository StepsRepository { get; set; }
        public static ISleepRepository SleepRepository { get; set; }
        public static IHeartrateRepository HeartrateRepository { get; set; }

        public static Database Database;

        public static void BuildGlobals(IHeartrateRepository heartrateRepository, ISleepRepository sleepRepository, IStepsRepository stepsRepository, Database database)
        {
            StepsRepository = stepsRepository;
            SleepRepository = sleepRepository;
            HeartrateRepository = heartrateRepository;
            Database = database;
            SamplesService = new SamplesService(HeartrateRepository, StepsRepository, SleepRepository);
            DevicePageViewModel = new DevicePageViewModel();
            MiCuentaPacienteViewModel = new MiCuentaPacienteViewModel();
            //listSleepRecordsGlobal = new List<SleepRecordsView>();
            SleepPageViewModel = new SleepPageViewModel(SleepRepository);
        }
    }
}