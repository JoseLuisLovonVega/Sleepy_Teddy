using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WindesHeartApp.Models;
using SleepyTeddy.Resources;
using WindesHeartApp.Resources;
using WindesHeartSDK;
using WindesHeartSDK.Models;
using Xamarin.Forms;

namespace SleepyTeddy.Services
{
    public static class CallbackHandler
    {
        private static readonly string _key = "LastConnectedDeviceGuid";

        public static void OnHeartrateUpdated(WindesHeartSDK.Models.HeartrateData heartrate)
        {
            if (heartrate.Heartrate == 0)
                return;
            Global.MiCuentaPacienteViewModel.Heartrate = heartrate.Heartrate;
        }

        public static void OnBatteryUpdated(BatteryData battery)
        {
            Global.MiCuentaPacienteViewModel.UpdateBattery(battery);
        }

        public static void OnStepsUpdated(StepData stepsInfo)
        {
            var count = stepsInfo.StepCount;
            Debug.WriteLine($"Stepcount updated: {count}");
            Globals.StepsPageViewModel.OnStepsUpdated(count);

        }

        public static void OnConnect(ConnectionResult result)
        {
            if (result == ConnectionResult.Succeeded)
            {
                try
                {
                    //Sync settings
                    Windesheart.PairedDevice.SetTime(DateTime.Now);
                    Windesheart.PairedDevice.SetDateDisplayFormat(DeviceSettings.DateFormatDMY);
                    Windesheart.PairedDevice.SetLanguage(DeviceSettings.DeviceLanguage);
                    Windesheart.PairedDevice.SetTimeDisplayFormat(DeviceSettings.TimeFormat24Hour);
                    Windesheart.PairedDevice.SetActivateOnLiftWrist(DeviceSettings.WristRaiseDisplay);
                    Windesheart.PairedDevice.SetStepGoal(DeviceSettings.DailyStepsGoal);
                    Windesheart.PairedDevice.EnableFitnessGoalNotification(true);
                    Windesheart.PairedDevice.EnableSleepTracking(true);
                    Windesheart.PairedDevice.SetHeartrateMeasurementInterval(1);

                    //Callbacks
                    Windesheart.PairedDevice.EnableRealTimeHeartrate(OnHeartrateUpdated);
                    Windesheart.PairedDevice.EnableRealTimeBattery(OnBatteryUpdated);
                    Windesheart.PairedDevice.EnableRealTimeSteps(OnStepsUpdated);
                    Windesheart.PairedDevice.SubscribeToDisconnect(OnDisconnect);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine("Algo salió mal mientras se conectaba el wearable, desconectando...");
                    Windesheart.PairedDevice.Disconnect();
                    Global.DevicePageViewModel.IsLoading = false;
                }

                Global.DevicePageViewModel.StatusText = "Conectado";
                Global.DevicePageViewModel.DeviceList = new ObservableCollection<BLEScanResult>();
                Global.DevicePageViewModel.IsLoading = false;
                Global.MiCuentaPacienteViewModel.SleepWakeDiary = Guid.NewGuid().ToString().Replace("-", "");

                Global.MiCuentaPacienteViewModel.ReadCurrentBattery();
                Global.MiCuentaPacienteViewModel.BandNameLabel = Windesheart.PairedDevice.Name;
                Global.SamplesService.AddSleepWakeDiary();

                Device.BeginInvokeOnMainThread(delegate { Application.Current.MainPage.Navigation.PopAsync(); });
                Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización exitosa - Registro de diario de sueño-vigilia iniciado", new TimeSpan(4));
                Globals.SamplesService.StartFetching();
                Global.SamplesService.StartFetching();
            }
            else if (result == ConnectionResult.Failed)
            {
                Debug.WriteLine("ERROR");
                Acr.UserDialogs.UserDialogs.Instance.Toast("ERROR - Sincronización fallida", new TimeSpan(3));
                return;
            }
        }

        public static void OnDisconnect(Object obj)
        {
            Global.DevicePageViewModel.StatusText = "Desconectado"; 
            Global.MiCuentaPacienteViewModel.BandNameLabel = "";
            Global.MiCuentaPacienteViewModel.BatteryImage = "";
            /*if (Global.MiCuentaPacienteViewModel.SleepWakeDiary != "")
            {
                Global.SamplesService.CompleteSleepWakeDiary();
            }*/
            Global.MiCuentaPacienteViewModel.SleepWakeDiary = "";
        }
    }
}