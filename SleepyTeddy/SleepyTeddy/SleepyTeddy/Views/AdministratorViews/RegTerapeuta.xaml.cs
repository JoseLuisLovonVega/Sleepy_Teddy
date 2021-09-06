using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegTerapeuta : ContentPage
    {
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        
        public RegTerapeuta()
        {
            InitializeComponent();
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
                             .AddAsync(new Therapist
                             {
                                 Therapist_ID = Guid.NewGuid().ToString().Replace("-", ""),
                                 administrator_ID = id_admin,
                                 Names = nmTerapeuta.Text,
                                 Last_Names = apTerapeuta.Text,
                                 Especiality = nmEspecialidad.Text,
                                 Email = txEmail.Text,
                                 Password = txPsw.Text.ToString(),
                                 Role_ID = "3"
                             });

                    await App.Current.MainPage.DisplayAlert("Registro Exitoso", "Terapeuta registrado correctamente", "OK");
                    await Navigation.PushAsync(new BuscarTerapeuta());
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

            if (string.IsNullOrWhiteSpace(nmTerapeuta.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(apTerapeuta.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
                return Task.FromResult(false);
            }
            else if (string.IsNullOrWhiteSpace(nmEspecialidad.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar la especialidad del terapeuta.", new TimeSpan(3));
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

        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void nmTerapeuta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmTerapeuta.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmTerapeuta.Text = nmTerapeuta.Text.Remove(nmTerapeuta.Text.Length - 1);
                }
            }
        }

        private void apTerapeuta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apTerapeuta.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apTerapeuta.Text = apTerapeuta.Text.Remove(apTerapeuta.Text.Length - 1);
                }
            }
        }

        private void nmEspecialidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmEspecialidad.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmEspecialidad.Text = nmEspecialidad.Text.Remove(nmEspecialidad.Text.Length - 1);
                }
            }
        }
    }
}