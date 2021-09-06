using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Text.RegularExpressions;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdatePatient : ContentPage
    {
        Patient patient;
        string documentId;
        string Patient_ID;

        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

        public UpdatePatient(string key_patient)
        {
            Patient_ID = key_patient;
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
                                       .WhereEqualsTo("Patient_ID",Patient_ID)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            documentId = document.Documents.ElementAt(0).Id;
            nmPaciente.Text = patient.Names;
            apPaciente.Text = patient.Last_Names;
            dtNacimiento.Date = patient.Birthday;
            txEmail.Text = patient.Email;
            txPsw.Text = patient.Password;
            txPsw2.Text = patient.Password;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmPaciente.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(apPaciente.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
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
            else if (txPsw.Text != txPsw2.Text)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Las contraseñas no coinciden.", new TimeSpan(3));
            }
            else
            {
                patient.Names = nmPaciente.Text;
                patient.Last_Names = apPaciente.Text;
                patient.Birthday = dtNacimiento.Date;
                patient.Email = txEmail.Text;
                patient.Password = txPsw.Text;
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection("Users")
                    .Document(documentId)
                    .UpdateAsync(patient);
                await DisplayAlert("Actualización Exitosa", "Paciente actualizado correctamente", "OK");
                await Navigation.PushAsync(new BuscarPaciente());
                    //await Navigation.PopAsync();
                }
        }

        /*private Task<bool> ValidarFormularioAsync()
        {
            //Regex rgxLETRAS = new Regex(REGEX_LETRAS, RegexOptions.IgnoreCase);
            //Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);
            Regex rgxCONTRASENA = new Regex(REGEX_CONTRASENA, RegexOptions.IgnoreCase);

            if (nmPaciente.Text.Length == 0)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el nombre del paciente.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (apPaciente.Text.Length == 0)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el apellido del paciente.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (txEmail.Text.Length == 0)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el correo del paciente.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (txPsw.Text.Length == 0)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña del paciente.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!IsValidEmail(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (txPsw.Text.Length < 8 || txPsw.Text.Length > 16)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener entre 8 y 16 caracteres.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!nmPaciente.Text.ToCharArray().All(Char.IsLetter))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Los nombres no pueden contener números", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!apPaciente.Text.ToCharArray().All(Char.IsLetter))
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Los apellidos no pueden contener números", new TimeSpan(3));
                return Task.FromResult(false);
                }
             if (!rgxCONTRASENA.IsMatch(txPsw.Text))
                {
                            Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un dígito, una minúscula y una mayúscula.", new TimeSpan(6));
                return Task.FromResult(false);
                }
             return Task.FromResult(true);
        }*/
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void dtNacimiento_DateSelected(object sender, DateChangedEventArgs e)
        {

        }

        private void nmPaciente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmPaciente.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmPaciente.Text = nmPaciente.Text.Remove(nmPaciente.Text.Length - 1);
                }
            }
        }

        private void apPaciente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apPaciente.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apPaciente.Text = apPaciente.Text.Remove(apPaciente.Text.Length - 1);
                }
            }
        }
    }
}