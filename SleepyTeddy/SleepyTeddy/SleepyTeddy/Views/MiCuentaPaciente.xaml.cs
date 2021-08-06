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

namespace SleepyTeddy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MiCuentaPaciente : ContentPage
    {
        string patient_id = LoginViewModel.Patient_ID;
        Patient patient;
        public MiCuentaPaciente()
        {
            //accountViewModel = new AccountViewModel();
            InitializeComponent();
            //BindingContext = accountViewModel;
            getPatient();

        }

        private async void getPatient()
        {
            string role_id = "2";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Patient_ID", patient_id)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            NombreCompleto.Text = patient.Names + " " + patient.Last_Names;
        }
        private async void MicuentaPaciente(object sender, EventArgs args)
        {
                await Navigation.PushAsync(new UpdateAccPatient(patient_id));
        }
        private async void Weareable(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new Wearable(patient_id));
        }
        private async void CerrarSesion(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new MainPageLogin());         
        }

    }
}