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
            SleepPageViewModel = new SleepPageViewModel(SleepRepository);
        }
    }
}