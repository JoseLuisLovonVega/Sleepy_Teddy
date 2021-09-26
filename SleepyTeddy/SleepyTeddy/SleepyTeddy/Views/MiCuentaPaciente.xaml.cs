using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.Views.PatientViews;
using SleepyTeddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WindesHeartSDK;
using FormsControls.Base;
using SleepyTeddy.Resources;
using SleepyTeddy.Services;

namespace SleepyTeddy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MiCuentaPaciente : IAnimationPage
    {
        string patient_id = LoginViewModel.Patient_ID;
        Patient patient = new Patient();
        public static Button UpdatePatientButton;
        public static Button WearableButton;
        public static Button CerrarSesionButton;
        private readonly string _propertyKey = "LastConnectedDevice";
        string NombreCompleto;

        GetDataFromLoginUser objData { get; set; }
        public MiCuentaPaciente()
        {
            InitializeComponent();
            BindingContext = Globals.MiCuentaPacienteViewModel;
            //getPatient();
            BuildPage();
        }

        protected override void OnAppearing()
        {
            //App.RequestLocationPermission();
            if (Windesheart.PairedDevice == null)
                return;
        }

        //Set UUID in App-properties
        private void SetApplicationProperties()
        {
            if (Windesheart.PairedDevice != null)
            {
                App.Current.Properties[_propertyKey] = Windesheart.PairedDevice.Uuid;
            }
        }

        //Handle Auto-connect to the last connected device with App-properties
        private async Task HandleAutoConnect()
        {
            var knownGuid = App.Current.Properties[_propertyKey].ToString();
            if (!string.IsNullOrEmpty(knownGuid))
            {
                var knownDevice = await Windesheart.GetKnownDevice(Guid.Parse(knownGuid));
                knownDevice.Connect(CallbackHandler.OnConnect);
            }
        }

        private async Task getPatient()
        {
            string role_id = "2";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Patient_ID", patient_id)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            NombreCompleto = patient.Names + " " + patient.Last_Names;
        }
        public static void BuildPageBasics(AbsoluteLayout layout, object sender)
        {
            layout.BackgroundColor = Color.White;
            ((ContentPage)sender).Content = layout;
        }
        private async void BuildPage()
        {
            absoluteLayout = new AbsoluteLayout();
            BuildPageBasics(absoluteLayout, this);
            await getPatient();
            PageBuilder.AddLabel(absoluteLayout, NombreCompleto, 0.5, 0.07, Color.Black, "", 22);
            PageBuilder.AddAccountImage(absoluteLayout);
            #region define fetch progressbar 
            ProgressBar fetchProgressBar = new ProgressBar
            {
                ProgressColor = Color.Red,
                HeightRequest = 20
            };

            fetchProgressBar.SetBinding(ProgressBar.ProgressProperty, new Binding("FetchProgress"));
            fetchProgressBar.SetBinding(ProgressBar.IsVisibleProperty, new Binding("FetchProgressVisible"));

            AbsoluteLayout.SetLayoutBounds(fetchProgressBar, new Rectangle(0.5, 0.40, 0.95, -1));
            AbsoluteLayout.SetLayoutFlags(fetchProgressBar, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            absoluteLayout.Children.Add(fetchProgressBar);
            #endregion

            #region define battery and hr Label
            Image batteryImage = new Image { HeightRequest = 30 };
            batteryImage.SetBinding(Image.SourceProperty, new Binding("BatteryImage"));
            AbsoluteLayout.SetLayoutBounds(batteryImage, new Rectangle(0.95, 0.24, 30, 30));
            AbsoluteLayout.SetLayoutFlags(batteryImage, AbsoluteLayoutFlags.PositionProportional);

            var bandNameLabel = PageBuilder.AddLabel(absoluteLayout, "", 0.95, 0.155, Color.Black, "BandNameLabel", 9);
            bandNameLabel.FontAttributes = FontAttributes.Bold;
            bandNameLabel.FontAttributes = FontAttributes.Italic;

            var batteryLabel = PageBuilder.AddLabel(absoluteLayout, "", 0.95, 0.18, Color.Black, "DisplayBattery", 12);
            batteryLabel.FontAttributes = FontAttributes.Bold;
            absoluteLayout.Children.Add(batteryImage);

            Label hrLabel = new Label { FontSize = 10, FontAttributes = FontAttributes.Bold };
            hrLabel.SetBinding(Label.TextProperty, new Binding("DisplayHeartRate"));
            AbsoluteLayout.SetLayoutBounds(hrLabel, new Rectangle(0.10, 0.155, 60, 60));
            AbsoluteLayout.SetLayoutFlags(hrLabel, AbsoluteLayoutFlags.PositionProportional);
            absoluteLayout.Children.Add(hrLabel);
            #endregion

            PageBuilder.AddActivityIndicator(absoluteLayout, "IsLoading", 0.50, 0.42, 80, 80, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#999999"));

            UpdatePatientButton = PageBuilder.AddButton(absoluteLayout, "Mi Cuenta", Globals.MiCuentaPacienteViewModel.MicuentaPaciente, 0.50, 0.60, 180, 50, 12, 20, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#E0DADA"));
            WearableButton = PageBuilder.AddButton(absoluteLayout, "Wearable", Globals.MiCuentaPacienteViewModel.Weareable, 0.50, 0.75, 180, 50, 12, 20, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#E0DADA"));
            CerrarSesionButton = PageBuilder.AddButton(absoluteLayout, "Cerrar Sesión", Globals.MiCuentaPacienteViewModel.CerrarSesion, 0.50, 0.90, 180, 50, 12, 20, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#E0DADA"));

        }

        public IPageAnimation PageAnimation { get; } = new SlidePageAnimation { Duration = AnimationDuration.Short, Subtype = AnimationSubtype.FromTop };

        private async void MicuentaPaciente(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new UpdateAccPatient());
        }
        private async void Weareable(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new Wearable()
            {
                BindingContext = Globals.DevicePageViewModel
            });
        }
        private async void CerrarSesion(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new MainPageLogin());
        }
        public void OnAnimationStarted(bool isPopAnimation)
        {
            // Put your code here but leaving empty works just fine
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            // Put your code here but leaving empty works just fine
        }

    }
}