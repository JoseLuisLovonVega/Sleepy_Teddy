using SleepyTeddy.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuscarPacienteTerapeuta : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public GetDataFromSearchLastNames objTableBinding { get; set; }
        public GetDataFromLoginUser objPatientsTherapist { get; set; }

        public BuscarPacienteTerapeuta()
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
            objPatientsTherapist = new GetDataFromLoginUser();
            await objPatientsTherapist.GetPatientsTherapistViewAsync();
            list_patients.ItemsSource = objPatientsTherapist.ListPatientsTherapist;
            apPatient.Text = "";
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var dataItem = e.Item as PatientsView;
            //Dar clic en el paciente y mostrar los detalles del paciente con respecto a su sueño
            await Navigation.PushAsync(new VisualizarInformacionPaciente(dataItem.Key));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
        private async void btnBuscar_clicked(object sender, EventArgs e)
        {
            string id_therapist = LoginViewModel.Therapist_ID;
            if (string.IsNullOrWhiteSpace(apPatient.Text))
            {
                list_patients.ItemsSource = objPatientsTherapist.ListPatientsTherapist;
                Acr.UserDialogs.UserDialogs.Instance.Toast("Complete el campo de búsqueda", new TimeSpan(3));
            }
            else
            {
                try
                {
                    objTableBinding = new GetDataFromSearchLastNames();
                    await objTableBinding.GetPatientsTherapistViewAsync(apPatient.Text, id_therapist);
                    list_patients.ItemsSource = objTableBinding.ListPatientsTherapist;
                    if ((list_patients.ItemsSource as ListPatients).Count == 0)
                    {
                        list_patients.ItemsSource = objPatientsTherapist.ListPatientsTherapist;
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                    }
                }
                catch (Exception ex) { }
            }
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