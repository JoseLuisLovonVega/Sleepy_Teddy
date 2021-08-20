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
    public partial class CuestionarioISI : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        string documentID;

        public class Option
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }
        public class Option2
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }
        public class Option3
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        List<Option> opts;
        List<Option2> opts2;
        List<Option3> opts3;
        public CuestionarioISI(string key_questionnaire)
        {
            id_questionnaire= key_questionnaire;
            InitializeComponent();
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
            opts.Add(new Option { Name = "Nada", Number = 0 });
            opts.Add(new Option { Name = "Leve", Number = 1 });
            opts.Add(new Option { Name = "Moderado", Number = 2 });
            opts.Add(new Option { Name = "Grave", Number = 3 });
            opts.Add(new Option { Name = "Muy Grave", Number = 4 });

            opts2 = new List<Option2>();
            opts2.Add(new Option2 { Name = "Muy satisfecho", Number = 0 });
            opts2.Add(new Option2 { Name = "Satisfecho", Number = 1 });
            opts2.Add(new Option2 { Name = "Moderadamente satisfecho", Number = 2 });
            opts2.Add(new Option2 { Name = "Insatisfecho", Number = 3 });
            opts2.Add(new Option2 { Name = "Muy insatisfecho", Number = 4 });

            opts3 = new List<Option3>();
            opts3.Add(new Option3 { Name = "Nada", Number = 0 });
            opts3.Add(new Option3 { Name = "Un poco", Number = 1 });
            opts3.Add(new Option3 { Name = "Algo", Number = 2 });
            opts3.Add(new Option3 { Name = "Mucho", Number = 3 });
            opts3.Add(new Option3 { Name = "Muchísimo", Number = 4 });

            picker1a.ItemsSource = opts;
            picker1b.ItemsSource = opts;
            picker1c.ItemsSource = opts;
            picker2.ItemsSource = opts2;
            picker3.ItemsSource = opts3;
            picker4.ItemsSource = opts3;
            picker5.ItemsSource = opts3;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            {
                var answer1a = (Option)picker1a.SelectedItem;
                var answer1b = (Option)picker1b.SelectedItem;
                var answer1c = (Option)picker1c.SelectedItem;
                var answer2 = (Option2)picker2.SelectedItem;
                var answer3 = (Option3)picker3.SelectedItem;
                var answer4 = (Option3)picker4.SelectedItem;
                var answer5 = (Option3)picker5.SelectedItem;
                if (answer1a == null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 1a.", new TimeSpan(3));
                }
                else if (answer1b == null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 1b.", new TimeSpan(3));
                }
                else if (answer1c == null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 1c.", new TimeSpan(3));
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
                else
                {
                    questionnaire.N_Result = answer1a.Number + answer1b.Number + answer1c.Number + answer2.Number
                          + answer3.Number + answer4.Number + answer5.Number;
                    questionnaire.D_Completed_Date = DateTime.Now.AddHours(-5);

                    await CrossCloudFirestore.Current
                        .Instance
                        .Collection("Questionnaires")
                        .Document(documentID)
                        .UpdateAsync(questionnaire);
                    await DisplayAlert("Registro Exitoso", "Cuestionario realizado correctamente", "OK");
                    await Navigation.PushAsync(new MisCuestionarios());
                }
            }
        }

        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}