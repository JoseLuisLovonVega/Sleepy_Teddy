using System;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateAccTherapist : ContentPage
    {
        String Therapist_ID;
        String documentID;
        Therapist therapist;

        public UpdateAccTherapist(String key_terapist)
        {
            Therapist_ID = key_terapist;
            InitializeComponent();
            getTerapeuta();

        }

        private async void getTerapeuta()
        {
            String role_id = "3";
            String UID = LoginViewModel.Therapist_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Therapist_ID", Therapist_ID)
                                       .GetAsync();
            therapist = document.Documents.ElementAt(0).ToObject<Therapist>();
            documentID = document.Documents.ElementAt(0).Id;
            nmTpt.Text = therapist.Names;
            apTpt.Text = therapist.Last_Names;
            espTpt.Text = therapist.Especiality;
            txEmail.Text = therapist.Email;
            txPsw.Text = therapist.Password;
            txPsw2.Text = therapist.Password;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            if (txPsw.Text.Length < 8 || txPsw.Text.Length > 16)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener entre 8 y 16 caracteres.", new TimeSpan(3));
            }
            else if (!txPsw.Text.Any(char.IsUpper))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos una mayúscula.", new TimeSpan(3));
            }
            else if (!txPsw.Text.Any(char.IsDigit))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un número.", new TimeSpan(3));
            }
            else if (txPsw.Text != txPsw2.Text)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Las contraseñas no coinciden.", new TimeSpan(3));
            }
            else
            {
                therapist.Password = txPsw.Text;
                await CrossCloudFirestore.Current
                     .Instance
                     .Collection("Users")
                     .Document(documentID)
                     .UpdateAsync(therapist);
                await DisplayAlert("Actualización Exitosa", "Cuenta actualizada correctamente", "OK");
                //await Navigation.PushAsync(new PaginaPrincipalTerapeuta());
            }
        }
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        /*
        private void nmTpt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmTpt.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmTpt.Text = nmTpt.Text.Remove(nmTpt.Text.Length - 1);
                }
            }
        }

        private void apTpt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apTpt.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apTpt.Text = apTpt.Text.Remove(apTpt.Text.Length - 1);
                }
            }
        }

        private void espTpt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(espTpt.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    espTpt.Text = espTpt.Text.Remove(espTpt.Text.Length - 1);
                }
            }
        }
    */
    }
 }