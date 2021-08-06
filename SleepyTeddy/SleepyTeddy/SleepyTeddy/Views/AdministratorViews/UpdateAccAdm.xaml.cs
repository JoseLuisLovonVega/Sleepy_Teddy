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

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateAccAdm : ContentPage
    {
        Administrator admin;
        string documentId;
        string Administrator_ID;
        //String REGEX_LETRAS = "[^a-zA-Z ]{2,254}";
        String REGEX_EMAIL = "^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

        public UpdateAccAdm(string key_administrator)
        {
            Administrator_ID = key_administrator;
            InitializeComponent();
            getAdmin();
        }

        private async void getAdmin()
        {
            String role_id = "1";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Administrator_ID", Administrator_ID)
                                       .GetAsync();
            admin = document.Documents.ElementAt(0).ToObject<Administrator>();
            documentId = document.Documents.ElementAt(0).Id;
            nmAdm.Text = admin.Names;
            apAdm.Text = admin.Last_Names;
            nmInstitucion.Text = admin.Institution;
            txEmail.Text = admin.Email;
            txPsw.Text = admin.Password;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            Regex rgxEMAIL = new Regex(REGEX_EMAIL, RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(nmAdm.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los nombres. ", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(apAdm.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar los apellidos.", new TimeSpan(3));
            }
            else if (string.IsNullOrWhiteSpace(nmInstitucion.Text))
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar el nombre de la institución.", new TimeSpan(3));
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
                admin.Names = nmAdm.Text;
                admin.Last_Names = apAdm.Text;
                admin.Institution = nmInstitucion.Text;
                admin.Email = txEmail.Text;
                admin.Password = txPsw.Text;
               await CrossCloudFirestore.Current
                    .Instance
                    .Collection("Users")
                    .Document(documentId)
                    .UpdateAsync(admin);
                await DisplayAlert("", "Cuenta actualizada correctamente", "OK");
                await Navigation.PushAsync(new PaginaPrincipalAdministrador());
                //await Navigation.PopAsync();
            }
        }
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void nmAdm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmAdm.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmAdm.Text = nmAdm.Text.Remove(nmAdm.Text.Length - 1);
                }
            }
        }

        private void apAdm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apAdm.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apAdm.Text = apAdm.Text.Remove(apAdm.Text.Length - 1);
                }
            }
        }

        private void nmInstitucion_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nmInstitucion.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    nmInstitucion.Text = nmInstitucion.Text.Remove(nmInstitucion.Text.Length - 1);
                }
            }
        }
    }
}