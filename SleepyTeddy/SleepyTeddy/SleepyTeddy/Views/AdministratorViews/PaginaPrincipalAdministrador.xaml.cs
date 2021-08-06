using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaPrincipalAdministrador : TabbedPage
    {

        public PaginaPrincipalAdministrador()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }
        private async void AccountSelect(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MiCuenta());
        }

        //Método para bloquear boton retroceder
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                    await Navigation.PushAsync(new PaginaPrincipalAdministrador());
            });
            return true;
        }



    }

}