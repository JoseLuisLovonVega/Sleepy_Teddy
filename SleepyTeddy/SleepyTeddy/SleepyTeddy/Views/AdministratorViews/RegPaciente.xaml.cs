using FirebaseAdmin;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegPaciente : ContentPage
    {
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        public RegPaciente()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            string id_admin = LoginViewModel.Administrator_ID;
            if (await ValidarFormularioAsync())
            {

                try
                {
                    await CrossCloudFirestore.Current
                             .Instance
                             .Collection("Users")
                             .AddAsync(new Patient
                             {
                                 Patient_ID = Guid.NewGuid().ToString().Replace("-", ""),
                                 administrator_ID = id_admin,
                                 Names = nmPaciente.Text,
                                 Last_Names = apPaciente.Text,
                                 Email = txEmail.Text,
                                 Password = txPsw.Text.ToString(),
                                 Birthday = dtNacimiento.Date.AddHours(-5),
                                 Role_ID = "2"
                             });

                    await App.Current.MainPage.DisplayAlert("Registro Exitoso", "Paciente registrado correctamente", "OK");
                    await Navigation.PushAsync(new BuscarPaciente());
                    /*nmTerapeuta.Text = "";
                    apTerapeuta.Text = "";
                    nmEspecialidad.Text = "";
                    txEmail.Text = "";
                    txPsw.Text = "";*/
                    //await Navigation.PopAsync();
                    //await Navigation.PushAsync(new MainPageLogin());
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("", ex.Message, "OK");

                }
            }

        }
        private Task<bool> ValidarFormularioAsync()
        {
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmPaciente.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(apPaciente.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el email.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!rgxEMAIL.IsMatch(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (txPsw.Text.Length < 8 || txPsw.Text.Length > 16)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener entre 8 y 16 caracteres.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!txPsw.Text.Any(char.IsUpper))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos una mayúscula.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!txPsw.Text.Any(char.IsDigit))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un número.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (txPsw.Text != txPsw2.Text)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Las contraseñas no coinciden.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        private void dtNacimiento_DateSelected(object sender, DateChangedEventArgs e)
        {

        }
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
        await Navigation.PopAsync();
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