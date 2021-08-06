using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.CloudFirestore;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Models;
using System.Text.RegularExpressions;

namespace SleepyTeddy.Views.PatientViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateAccPatient : ContentPage
    {
        Patient patient;
        string documentId;
        string Patient_ID;
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

        public UpdateAccPatient(string key_pacient)
        {
            Patient_ID = key_pacient;
            InitializeComponent();
            getPatient();
        }

        private async void getPatient()
        {
            String role_id = "2";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Patient_ID", Patient_ID)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            documentId = document.Documents.ElementAt(0).Id;
            nmPct.Text = patient.Names;
            apPct.Text = patient.Last_Names;
            dtNacimiento.Date = patient.Birthday;
            txEmail.Text = patient.Email;
            txPsw.Text = patient.Password;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmPct.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
            }
            else if (!nmPct.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los nombres no pueden contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(apPct.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
            }
            else if (!apPct.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los apellidos no pueden contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el email.", new TimeSpan(3));
            }
            else if (!rgxEMAIL.IsMatch(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña.", new TimeSpan(3));
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
                patient.Names = nmPct.Text;
                patient.Last_Names = apPct.Text;
                patient.Birthday = dtNacimiento.Date;
                patient.Email = txEmail.Text;
                patient.Password = txPsw.Text;
                await CrossCloudFirestore.Current
                     .Instance
                     .Collection("Users")
                     .Document(documentId)
                     .UpdateAsync(patient);
                await DisplayAlert("", "Cuenta actualizada correctamente", "OK");
                await Navigation.PushAsync(new PaginaPrincipalPaciente());
            }  
        }

        /*private Task<bool> ValidarFormularioAsync()
        {
            //Regex rgxLETRAS = new Regex(REGEX_LETRAS, RegexOptions.IgnoreCase);
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);
            Regex rgxCONTRASENA = new Regex(REGEX_CONTRASENA, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmPct.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
            }
            else if (!nmPct.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los nombres no pueden contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(apPct.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
            }
            else if (!apPct.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los apellidos no pueden contener números.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el email.", new TimeSpan(3));
            }
            else if (!rgxEMAIL.IsMatch(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña.", new TimeSpan(3));
            }
            else if (txPsw.Text.Length < 8 || txPsw.Text.Length > 16)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener entre 8 y 16 caracteres.", new TimeSpan(3));
            }
            else if (!rgxCONTRASENA.IsMatch(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un dígito, una minúscula y una mayúscula.", new TimeSpan(4));
            }
            return Task.FromResult(true);
        }*/
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void nmPct_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmPct.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmPct.Text = nmPct.Text.Remove(nmPct.Text.Length - 1);
                }
            }
        }

        private void apPct_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apPct.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apPct.Text = apPct.Text.Remove(apPct.Text.Length - 1);
                }
            }
        }
    }
}