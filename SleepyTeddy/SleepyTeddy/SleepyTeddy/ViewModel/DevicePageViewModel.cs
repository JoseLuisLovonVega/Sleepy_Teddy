using Plugin.BluetoothLE;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WindesHeartApp.Pages;
using WindesHeartApp.Resources;
using WindesHeartApp.Services;
using WindesHeartSDK;
using WindesHeartSDK.Models;
using Xamarin.Forms;

namespace SleepyTeddy.ViewModel
{
    internal class DevicePageViewModel
    {
        private bool _isLoading;
        private string _statusText;
        private BLEScanResult _selectedDevice;
        private ObservableCollection<BLEScanResult> _deviceList;
        private string _scanbuttonText;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string _propertyKey = "LastConnectedDevice";

        public DevicePageViewModel()
        {
            if (DeviceList == null)
                DeviceList = new ObservableCollection<BLEScanResult>();
            if (Windesheart.PairedDevice == null)
                StatusText = "Desconectado";
            ScanButtonText = "Escanear";
        }
        public void DisconnectButtonClicked(object sender, EventArgs args)
        {
            IsLoading = true;
            Windesheart.PairedDevice?.Disconnect();
            IsLoading = false;
            StatusText = "Desconectado";
            DeviceList = new ObservableCollection<BLEScanResult>();
        }
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    string err = e.InnerException.Message;
                    Trace.WriteLine(err);
                }
            }
        }

        public ObservableCollection<BLEScanResult> DeviceList
        {
            get { return _deviceList; }
            set
            {
                _deviceList = value;
                OnPropertyChanged();
            }
        }
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public string ScanButtonText
        {
            get { return _scanbuttonText; }
            set
            {
                _scanbuttonText = value;
                OnPropertyChanged();
            }
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
        public BLEScanResult SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;

                if (_selectedDevice == null)
                    return;
                DeviceSelected(_selectedDevice.Device);
                DevicePage.Devicelist.SelectedItem = null;
            }
        }

        public async void ScanButtonClicked(object sender, EventArgs args)
        {
            //Console.WriteLine("scan button clicked");
            DeviceList = new ObservableCollection<BLEScanResult>();
            DisconnectButtonClicked(sender, EventArgs.Empty);
            try
            {
                //If already scanning, stop scanning
                if (CrossBleAdapter.Current.IsScanning)
                {
                    Windesheart.StopScanning();
                    ScanButtonText = "Escanear";
                    IsLoading = false;
                }
                else
                {

                    if (CrossBleAdapter.Current.Status == AdapterStatus.PoweredOff)
                    {
                        await Application.Current.MainPage.DisplayAlert("Bluetooth apagado",
                            "El Bluetooth está apagado. Por favor habilite el Bluetooth para iniciar con el escaneo de los dispositivos", "OK");
                        StatusText = "Bluetooth apagado";
                        return;
                    }

                    //If started scanning
                    if (Windesheart.StartScanning(OnDeviceFound))
                    {
                        ScanButtonText = "Detener escaneo";
                        StatusText = "Escaneando...";
                        IsLoading = true;
                    }
                    else
                    {
                        StatusText = "No se pudo iniciar el escaneo.";
                        ScanButtonText = "Escanear";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary> 
        /// Called when leaving the page
        /// </summary>
        public void OnDisappearing()
        {
            Console.WriteLine("Deteniendo escaneo...");
            Windesheart.StopScanning();
            IsLoading = false;
            StatusText = "";
            ScanButtonText = "Escanear";
            DeviceList = new ObservableCollection<BLEScanResult>();
        }

        private void OnDeviceFound(BLEScanResult result)
        {
            DeviceList.Add(result);
        }

        private void DeviceSelected(BLEDevice device)
        {
            if (device == null)
            {
                return;
            }

            try
            {
                Windesheart.StopScanning();

                StatusText = "Connecting...";
                IsLoading = true;

                if (App.Current.Properties.ContainsKey(_propertyKey))
                {
                    var knownGuidString = App.Current.Properties[_propertyKey].ToString();

                    if (!string.IsNullOrEmpty(knownGuidString))
                    {
                        var knownGuid = Guid.Parse(knownGuidString);
                        if (device.IDevice.Uuid == knownGuid)
                        {
                            device.NeedsAuthentication = false;
                        }
                    }
                }
                device.Connect(CallbackHandler.OnConnect);
                SelectedDevice = null;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


    }
}
