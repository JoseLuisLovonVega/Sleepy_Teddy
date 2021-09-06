using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuscarTerapeuta : ContentPage
    {
        public GetDataFromSearchLastNames objTableBinding { get; set; }
        public GetDataFromLoginUser objTherapists { get; set; }
        public BuscarTerapeuta()
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
            objTherapists = new GetDataFromLoginUser();
            await objTherapists.GetTherapistsAdministratorViewAsync();
            list_therapist.ItemsSource = objTherapists.ListTherapistsAdministrator;
            apTherapist.Text = "";
        }

        private async void btnBuscar_clicked(object sender, EventArgs e)
        {
            string id_admin = LoginViewModel.Administrator_ID;
            if (string.IsNullOrWhiteSpace(apTherapist.Text))
            {
                list_therapist.ItemsSource = objTherapists.ListTherapistsAdministrator;
                Acr.UserDialogs.UserDialogs.Instance.Toast("Complete el campo de búsqueda", new TimeSpan(3));
            }
            else
            {
                try
                {
                    objTableBinding = new GetDataFromSearchLastNames();
                    await objTableBinding.GetTherapistsAdministratorViewAsync(apTherapist.Text, id_admin);
                    list_therapist.ItemsSource = objTableBinding.ListTherapist;
                    if ((list_therapist.ItemsSource as ListTherapists).Count == 0)
                    {
                        list_therapist.ItemsSource = objTherapists.ListTherapistsAdministrator;
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

            var dataItem = e.Item as TherapistsView;

            await Navigation.PushAsync(new UpdateTherapist(dataItem.Key));
            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        private async void btnRegisterTherapist_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegTerapeuta());
        }

        private void apTherapist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apTherapist.Text) == false)
            {
                if (char.IsDigit(e.NewTextValue.Last()))
                {
                    apTherapist.Text = apTherapist.Text.Remove(apTherapist.Text.Length - 1);
                }
            }
        }
    }
}