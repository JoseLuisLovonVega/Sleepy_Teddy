using System;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SleepyTeddy.ViewModel;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateTherapist : ContentPage
    {
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        string Therapist_ID;
        string documentID;
        Therapist therapist;

        public UpdateTherapist(String key_terapist)
        {
            Therapist_ID = key_terapist;
            InitializeComponent();
            getTherapist();

        }
        private async void getTherapist()
        {
            String role_id = "3";
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
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmTpt.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
            }
            else if (!nmTpt.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los nombres no puede contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(apTpt.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
            }
            else if (!apTpt.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los apellidos no puede contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(espTpt.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la especialidad del terapeuta.", new TimeSpan(3));
            }
            else if (!espTpt.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El nombre de la especialidad no puede contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el email.", new TimeSpan(3));
            }
            else if (!rgxEMAIL.IsMatch(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña", new TimeSpan(3));
            }
            else if (txPsw.Text.Length < 8 || txPsw.Text.Length > 16)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener entre 8 y 16 caracteres.", new TimeSpan(3));
            }
            else if (!txPsw.Text.Any(char.IsUpper))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos una mayúscula.", new TimeSpan(3));
            }
            else if (!txPsw.Text.Any(char.IsDigit))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un dígito.", new TimeSpan(3));
            }
            else
            {
                therapist.Names = nmTpt.Text;
                therapist.Last_Names = apTpt.Text;
                therapist.Especiality = espTpt.Text;
                therapist.Email = txEmail.Text;
                therapist.Password = txPsw.Text;

                await CrossCloudFirestore.Current
                                     .Instance
                                     .Collection("Users")
                                     .Document(documentID)
                                     .UpdateAsync(therapist);

                await DisplayAlert("", "Terapeuta actualizado correctamente", "OK");
                await Navigation.PushAsync(new BuscarTerapeuta());

            }
        }
            private async void btnCancelar_clicked(object sender, EventArgs e)
            {
                await Navigation.PopAsync();
            }

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
    }
}