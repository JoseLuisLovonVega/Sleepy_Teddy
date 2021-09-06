using SleepyTeddy.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuscarPaciente : ContentPage
    {
        public GetDataFromSearchLastNames objTableBinding { get; set; }
        public GetDataFromLoginUser objPatients { get; set; }

        public BuscarPaciente()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            LoadItems();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private async void LoadItems()
        {
            objPatients = new GetDataFromLoginUser();
            await objPatients.GetPatientsAdministratorViewAsync();
            list_patients.ItemsSource = objPatients.ListPatientsAdministrator;
            apPatient.Text = "";
        }

        private async void btnBuscar_clicked(object sender, EventArgs e)
        {
            string id_admin = LoginViewModel.Administrator_ID;
            if (string.IsNullOrWhiteSpace(apPatient.Text))
            {
                list_patients.ItemsSource = objPatients.ListPatientsAdministrator;
                Acr.UserDialogs.UserDialogs.Instance.Toast("Complete el campo de búsqueda", new TimeSpan(3));
            }
            else
            {
                try
                {
                    objTableBinding = new GetDataFromSearchLastNames();
                    await objTableBinding.GetPatientsAdministratorViewAsync(apPatient.Text, id_admin);
                    list_patients.ItemsSource = objTableBinding.ListPatient;
                    if ((list_patients.ItemsSource as ListPatients).Count == 0)
                    {
                        list_patients.ItemsSource = objPatients.ListPatientsAdministrator;
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                    }
                }
                catch (Exception ex) { }
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var dataItem = e.Item as PatientsView;

            await Navigation.PushAsync(new UpdatePatient(dataItem.Key));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void btnRegisterPacient_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegPaciente());
            //Nos envía a cuestionario PSQ-9
            //await Navigation.PushAsync(new CuestionarioPSQ9());
        }

        private void apPatient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apPatient.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apPatient.Text = apPatient.Text.Remove(apPatient.Text.Length - 1);
                }
            }
        }
    }
}