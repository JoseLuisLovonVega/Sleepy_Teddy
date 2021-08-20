using Acr.UserDialogs;
using Firebase.Auth;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegAdministrador : ContentPage
    {
        //public string WebAPIkey = "AIzaSyCn23mEfbZVvw9Y7RTuCqmhpI5fcwQRL5o";
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        public RegAdministrador()
        {
            InitializeComponent();
        }
        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            if (await ValidarFormularioAsync())
            {
                try
                {
                    await CrossCloudFirestore.Current
                         .Instance
                         .Collection("Users")
                         .AddAsync(new Administrator
                         {
                             Administrator_ID = Guid.NewGuid().ToString().Replace("-", ""),
                             Names = nmAdmin.Text,
                             Last_Names = apAdmin.Text,
                             Institution = apInstitucion.Text,
                             Email = txEmail.Text,
                             Password = txPsw.Text.ToString(),
                             Role_ID = "1"
                         });

                    await App.Current.MainPage.DisplayAlert("Registro Exitoso", "Cuenta registrada correctamente", "OK");
                    await Navigation.PushAsync(new MainPageLogin());
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

            if (string.IsNullOrWhiteSpace(nmAdmin.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(apAdmin.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(apInstitucion.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el nombre de la institución", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el email", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (!rgxEMAIL.IsMatch(txEmail.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El email es incorrecto", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(txPsw.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la contraseña", new TimeSpan(3));
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
                Acr.UserDialogs.UserDialogs.Instance.Toast("La contraseña debe tener al menos un dígito.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPageLogin());
        }

        private void nmAdmin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmAdmin.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmAdmin.Text = nmAdmin.Text.Remove(nmAdmin.Text.Length - 1);
                }
            }
        }

        private void apAdmin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apAdmin.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apAdmin.Text = apAdmin.Text.Remove(apAdmin.Text.Length - 1);
                }
            }
        }

        private void apInstitucion_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apInstitucion.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apInstitucion.Text = apInstitucion.Text.Remove(apInstitucion.Text.Length - 1);
                }
            }
        }
    }
}