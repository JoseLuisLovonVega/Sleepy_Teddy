using SleepyTeddy.Resources;
using SleepyTeddy.Views;
using SleepyTeddy.Views.PatientViews;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WindesHeartSDK;
using WindesHeartSDK.Models;
using Xamarin.Forms;

namespace SleepyTeddy.ViewModel
{
    public class MiCuentaPacienteViewModel : INotifyPropertyChanged
    {
        private int _battery;
        private int _heartrate;
        private string _batteryImage = "";
        private bool _isLoading;
        public bool toggle;
        public string SleepWakeDiary = "";
        private string _bandnameLabel;
        private float _fetchProgress;
        private bool _fetchProgressVisible;
        public event PropertyChangedEventHandler PropertyChanged;

        public MiCuentaPacienteViewModel()
        {
            if (Windesheart.PairedDevice != null)
            {
                ReadCurrentBattery();
                BandNameLabel = Windesheart.PairedDevice.Name;
            }

            toggle = false;
        }

        public async Task ReadCurrentBattery()
        {
            //catch!!
            var battery = await Windesheart.PairedDevice.GetBattery();
            UpdateBattery(battery);
        }

        public void UpdateBattery(BatteryData battery)
        {
            if (battery.Percentage == 0)
            {
                BatteryImage = "";
                return;
            };
            Battery = battery.Percentage;
            if (battery.Status == BatteryStatus.Charging)
            {
                BatteryImage = "BatteryCharging.png";
                return;
            }

            if (battery.Percentage >= 0 && battery.Percentage < 11)
            {
                BatteryImage = "BatteryLow.png";
            }
            else if (battery.Percentage >= 11 && battery.Percentage < 31)
            {
                BatteryImage = "BatteryQuart.png";
            }
            else if (battery.Percentage >= 31 && battery.Percentage < 51)
            {
                BatteryImage = "BatteryHalf.png";
            }
            else if (battery.Percentage >= 51 && battery.Percentage < 71)
            {
                BatteryImage = "BatteryTwoQuarts.png";
            }
            else if (battery.Percentage >= 71 && battery.Percentage < 91)
            {
                BatteryImage = "BatteryThreeQuarts.png";
            }
            else if (battery.Percentage >= 91)
            {
                BatteryImage = "BatteryFull.png";
            }
        }


        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string BatteryImage
        {
            get => _batteryImage;
            set
            {
                _batteryImage = value;
                OnPropertyChanged();
            }
        }

        public int Battery
        {
            get => _battery;
            set
            {
                _battery = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayBattery));
            }
        }
        public int Heartrate
        {
            get => _heartrate;
            set
            {
                _heartrate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayHeartRate));
            }
        }

        public string BandNameLabel
        {
            get => _bandnameLabel;
            set
            {
                _bandnameLabel = value;
                OnPropertyChanged();
            }
        }

        public float FetchProgress
        {
            get => _fetchProgress;
            set
            {
                _fetchProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayFetchProgress));


            }
        }

        public bool FetchProgressVisible
        {
            get => _fetchProgressVisible;
            set
            {
                _fetchProgressVisible = value;
                OnPropertyChanged();
            }
        }
        public string DisplayHeartRate => Heartrate != 0 ? $"Last Heartbeat: {Heartrate.ToString()}" : "";

        public string DisplayBattery => Battery != 0 ? $"{Battery.ToString()}%" : "";

        public float DisplayFetchProgress => FetchProgress != 0 ? FetchProgress : 0;

        public async void MicuentaPaciente(object sender, EventArgs args)
        {
            EnableDisableButtons(false);
            IsLoading = true;
            await Application.Current.MainPage.Navigation.PushAsync(new UpdateAccPatient());
            IsLoading = false;
            EnableDisableButtons(true);
        }
        public async void Weareable(object sender, EventArgs args)
        {
            EnableDisableButtons(false);
            IsLoading = true;
            await Application.Current.MainPage.Navigation.PushAsync(new Wearable()
            {
                BindingContext = Globals.DevicePageViewModel
            });
            IsLoading = false;
            EnableDisableButtons(true);
        }
        public async void CerrarSesion(object sender, EventArgs args)
        {
            EnableDisableButtons(false);
            IsLoading = true;
            await Application.Current.MainPage.Navigation.PushAsync(new MainPageLogin());
            IsLoading = false;
            EnableDisableButtons(true);
        }

        public void EnableDisableButtons(bool enable)
        {
            MiCuentaPaciente.UpdatePatientButton.IsEnabled = enable;
            MiCuentaPaciente.WearableButton.IsEnabled = enable;
            MiCuentaPaciente.CerrarSesionButton.IsEnabled = enable;
        }

        public void ShowFetchProgress(float progress)
        {
            FetchProgress = progress;
            if (progress == 1f)
            {
                FetchProgressVisible = false;
            }
            else
            {
                FetchProgressVisible = true;
            }
        }
    }
}
