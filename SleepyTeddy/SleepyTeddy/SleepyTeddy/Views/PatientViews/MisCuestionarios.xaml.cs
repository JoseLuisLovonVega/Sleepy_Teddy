using SleepyTeddy.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.PatientViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MisCuestionarios : ContentPage
    {
        public GetDataFromLoginUser objQuestionnaires { get; set; }

        public MisCuestionarios()
        {
            InitializeComponent();
            LoadItems();
        }

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }*/

        private async void LoadItems()
        {
            objQuestionnaires = new GetDataFromLoginUser();
            await objQuestionnaires.GetQuestionnairesViewAsync();
            list_questionnaires.ItemsSource = objQuestionnaires.ListQuestionnairesPatient.OrderByDescending(o => o.D_Assigned_Date).ToList();
        }
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var dataItem = e.Item as QuestionnairesView;

            if (dataItem.Type == "PHQ-9")
            {
                await Navigation.PushAsync(new CuestionarioPHQ9(dataItem.Key));
            }
            else
            {
                if (dataItem.Type == "ISI")
                {
                    await Navigation.PushAsync(new CuestionarioISI(dataItem.Key));
                }
                else
                {
                    if (dataItem.Type == "PSQI")
                    {
                        await Navigation.PushAsync(new CuestionarioPSQI(dataItem.Key));
                    }

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
                }
            }
        }
        protected override bool OnBackButtonPressed()
        {
            //Deselect Item
            list_questionnaires.SelectedItem = null;
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new MisCuestionarios());
            });
            return true;
        }
    }
}