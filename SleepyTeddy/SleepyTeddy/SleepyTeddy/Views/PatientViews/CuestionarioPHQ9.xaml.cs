using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.PatientViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CuestionarioPHQ9 : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        string documentID;
        public class Option
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        List<Option> opts;

        public CuestionarioPHQ9(string key_questionnaire)
        {
            id_questionnaire = key_questionnaire;
            InitializeComponent();
            //InitApp();
            NavigationPage.SetHasBackButton(this, false);
            getQuestionnaire();
            LoadItems();
        }

        private async void getQuestionnaire()
        {
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Questionnaire_ID", id_questionnaire)
                                       .GetAsync();
            questionnaire = document.Documents.ElementAt(0).ToObject<Questionnaire>();
            documentID = document.Documents.ElementAt(0).Id;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private void LoadItems()
        {
            opts = new List<Option>();
            opts.Add(new Option { Name = "Para nada", Number = 0 });
            opts.Add(new Option { Name = "Varios días", Number = 1 });
            opts.Add(new Option { Name = "Más de la mitad de los días", Number = 2 });
            opts.Add(new Option { Name = "Casi todos los días", Number = 3 });

            picker1.ItemsSource = opts;
            picker2.ItemsSource = opts;
            picker3.ItemsSource = opts;
            picker4.ItemsSource = opts;
            picker5.ItemsSource = opts;
            picker6.ItemsSource = opts;
            picker7.ItemsSource = opts;
            picker8.ItemsSource = opts;
            picker9.ItemsSource = opts;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            var answer1 = (Option)picker1.SelectedItem;
            var answer2 = (Option)picker2.SelectedItem;
            var answer3 = (Option)picker3.SelectedItem;
            var answer4 = (Option)picker4.SelectedItem;
            var answer5 = (Option)picker5.SelectedItem;
            var answer6 = (Option)picker6.SelectedItem;
            var answer7 = (Option)picker7.SelectedItem;
            var answer8 = (Option)picker8.SelectedItem;
            var answer9 = (Option)picker9.SelectedItem;
            if (answer1==null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 1.", new TimeSpan(3));
            }
            else if (answer2 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 2.", new TimeSpan(3));
            }
            else if (answer3 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 3.", new TimeSpan(3));
            }
            else if (answer4 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 4.", new TimeSpan(3));
            }
            else if (answer5 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5.", new TimeSpan(3));
            }
            else if (answer6 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 6.", new TimeSpan(3));
            }
            else if (answer7 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 7.", new TimeSpan(3));
            }
            else if (answer8 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 8.", new TimeSpan(3));
            }
            else if (answer9 == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 9.", new TimeSpan(3));
            }
            else
            {
              questionnaire.N_Result = answer1.Number+answer2.Number+ answer3.Number + answer4.Number 
                    + answer5.Number + answer6.Number + answer7.Number + answer8.Number + answer9.Number;
              questionnaire.D_Completed_Date = DateTime.Now.AddHours(-5);
               
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection("Questionnaires")
                    .Document(documentID)
                    .UpdateAsync(questionnaire);
                await DisplayAlert("", "Cuestionario realizado correctamente", "OK");
                await Navigation.PushAsync(new MisCuestionarios());
            }
        }
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {

            await Navigation.PopAsync();
        }
    }
}