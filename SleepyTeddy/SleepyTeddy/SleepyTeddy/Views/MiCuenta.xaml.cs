using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.Views.AdministratorViews;
using SleepyTeddy.Views.TherapistViews;
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
    public partial class MiCuenta : ContentPage
    {
        string therapist_id = LoginViewModel.Therapist_ID;
        string administrator_id = LoginViewModel.Administrator_ID;
        Administrator admin;
        Therapist therapist;
        public MiCuenta()
        {
            //accountViewModel = new AccountViewModel();
            InitializeComponent();
            //BindingContext = accountViewModel;
            if (LoginViewModel.Role_ID == "1")
            {
                getAdmin();
            }
            else
            {
                if (LoginViewModel.Role_ID == "3")
                {
                    getTherapist();
                }
            }

        }
        private async void getAdmin()
        {
            string role_id = "1";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Administrator_ID", administrator_id)
                                       .GetAsync();
            admin = document.Documents.ElementAt(0).ToObject<Administrator>();
            NombreCompleto.Text = admin.Names + " " + admin.Last_Names;
        }

        private async void getTherapist()
        {
            string role_id = "3";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Therapist_ID", therapist_id)
                                       .GetAsync();
            therapist = document.Documents.ElementAt(0).ToObject<Therapist>();
            NombreCompleto.Text = therapist.Names + " " + therapist.Last_Names;
        }
        private async void Micuenta(object sender, EventArgs args)
        {
            if(LoginViewModel.Role_ID=="1")
            {
                await Navigation.PushAsync(new UpdateAccAdm(administrator_id));
            }else
                if(LoginViewModel.Role_ID == "3")
            {
                await Navigation.PushAsync(new UpdateAccTherapist(therapist_id));
            }
        }
        private async void CerrarSesion(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new MainPageLogin());         
        }

    }
}