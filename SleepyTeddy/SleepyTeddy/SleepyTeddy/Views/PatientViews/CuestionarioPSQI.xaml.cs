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
    public partial class CuestionarioPSQI : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        string documentID;
        int Component1, Component2, Component3, Component4, Component5, Component6, Component7;

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
        public CuestionarioPSQI(string key_questionnaire)
        {
            id_questionnaire = key_questionnaire;
            InitializeComponent();
            getQuestionnaire();
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
            opts.Add(new Option { Name = "No me ha ocurrido durante el último mes", Number = 0 });
            opts.Add(new Option { Name = "Menos de una vez a la semana", Number = 1 });
            opts.Add(new Option { Name = "Una o dos veces a la semana", Number = 2 });
            opts.Add(new Option { Name = "Tres o más veces a la semana", Number = 3 });

            opts2 = new List<Option2>();
            opts2.Add(new Option2 { Name = "Muy buena", Number = 0 });
            opts2.Add(new Option2 { Name = "Bastante buena", Number = 1 });
            opts2.Add(new Option2 { Name = "Bastante mala", Number = 2 });
            opts2.Add(new Option2 { Name = "Muy mala", Number = 3 });

            opts3 = new List<Option3>();
            opts3.Add(new Option3 { Name = "No ha resultado problemático en absoluto", Number = 0 });
            opts3.Add(new Option3 { Name = "Sólo ligeramente problemático", Number = 1 });
            opts3.Add(new Option3 { Name = "Moderadamente problemático", Number = 2 });
            opts3.Add(new Option3 { Name = "Muy problemático", Number = 3 });

            picker5a.ItemsSource = opts;
            picker5b.ItemsSource = opts;
            picker5c.ItemsSource = opts;
            picker5d.ItemsSource = opts;
            picker5e.ItemsSource = opts;
            picker5f.ItemsSource = opts;
            picker5g.ItemsSource = opts;
            picker5h.ItemsSource = opts;
            picker5i.ItemsSource = opts;
            picker5j.ItemsSource = opts;
            picker6.ItemsSource = opts2;
            picker7.ItemsSource = opts;
            picker8.ItemsSource = opts;
            picker9.ItemsSource = opts3;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            var answer5a= (Option)picker5a.SelectedItem;
            var answer5b = (Option)picker5b.SelectedItem;
            var answer5c = (Option)picker5c.SelectedItem;
            var answer5d = (Option)picker5d.SelectedItem;
            var answer5e = (Option)picker5e.SelectedItem;
            var answer5f = (Option)picker5f.SelectedItem;
            var answer5g = (Option)picker5g.SelectedItem;
            var answer5h = (Option)picker5h.SelectedItem;
            var answer5i = (Option)picker5i.SelectedItem;
            var answer5j = (Option)picker5j.SelectedItem;
            var answer6 = (Option2)picker6.SelectedItem;
            var answer7 = (Option)picker7.SelectedItem;
            var answer8 = (Option)picker8.SelectedItem;
            var answer9 = (Option3)picker9.SelectedItem;
            if (answer5a == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5a.", new TimeSpan(3));
            }
            else if (answer5b == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5b.", new TimeSpan(3));
            }
            else if (answer5c == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5c.", new TimeSpan(3));
            }
            else if (answer5d == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5d.", new TimeSpan(3));
            }
            else if (answer5e == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5e.", new TimeSpan(3));
            }
            else if (answer5f == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5f.", new TimeSpan(3));
            }
            else if (answer5g == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5g.", new TimeSpan(3));
            }
            else if (answer5h == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5h.", new TimeSpan(3));
            }
            else if (answer5i == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5i.", new TimeSpan(3));
            }
            else if (answer5j == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe completar la pregunta 5j.", new TimeSpan(3));
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
                //Componente 1
                Component1 = answer6.Number;
                //Componente 2
                if (int.Parse(answer2.Text) <=15)
                {
                    Component2 = 0;
                }
                else if (int.Parse(answer2.Text) >= 16 && int.Parse(answer2.Text) <= 30)
                {
                    Component2 = 1;
                }
                else if (int.Parse(answer2.Text) >= 31 && int.Parse(answer2.Text) <= 60)
                {
                    Component2 = 2;
                }
                else if (int.Parse(answer2.Text) < 60)
                {
                    Component2 = 3;
                }
                Component2 = Component2+answer5a.Number;
                if (Component2 == 0)
                {
                    Component2 = 0;
                }
                else if (Component2 >= 1 && Component2 <= 2)
                {
                    Component2 = 1;
                }
                else if (Component5 >= 3 && Component2 <= 4)
                {
                    Component2 = 2;
                }
                else if (Component2 >= 5 && Component2 <= 6)
                {
                    Component2 = 3;
                }
                //Componente 3
                if (int.Parse(answer4.Text) > 7)
                {
                    Component3 = 0;
                }
                else if (int.Parse(answer4.Text) >=6 && int.Parse(answer4.Text) <= 7)
                {
                    Component3 = 1;
                }
                else if (int.Parse(answer4.Text) >= 5 && int.Parse(answer4.Text) <= 6)
                {
                    Component3 = 2;
                }
                else if (int.Parse(answer4.Text) <5)
                {
                    Component3 = 3;
                }
                //Componente 4
                float HoursSlept = float.Parse(answer4.Text);
                float HoursSpentOnBed = float.Parse(answer3.Text) - float.Parse(answer1.Text);
                float SleepEfficiency = (HoursSlept / HoursSpentOnBed) * 100;
                if (SleepEfficiency > 85)
                {
                    Component4 = 0;
                }
                else if (SleepEfficiency >= 75 && SleepEfficiency <= 84)
                {
                    Component4 = 1;
                }
                else if (SleepEfficiency >= 65 && SleepEfficiency <= 74)
                {
                    Component4 = 2;
                }
                else if (SleepEfficiency < 65)
                {
                    Component4 = 3;
                }
                //Componente 5
                Component5 = answer5b.Number + answer5c.Number + answer5d.Number + answer5e.Number + answer5f.Number + 
                    answer5g.Number + answer5h.Number + answer5i.Number + answer5j.Number;
                if (Component5 == 0)
                {
                    Component5 = 0;
                } 
                else if (Component5>=1 && Component5 <= 9)
                {
                    Component5 = 1;
                }
                else if (Component5 >= 10 && Component5 <= 18)
                {
                    Component5 = 2;
                }
                else if (Component5 >= 19 && Component5 <= 27)
                {
                    Component5 = 3;
                }
                //Componente 6
                Component6 = answer7.Number;
                //Componente 7
                Component7 = answer8.Number + answer9.Number;
                if (Component7 == 0)
                {
                    Component7 = 0;
                }
                else if (Component7 >= 1 && Component7 <= 2)
                {
                    Component7 = 1;
                }
                else if (Component7 >= 3 && Component7 <= 4)
                {
                    Component7 = 2;
                }
                else if (Component7 >= 5 && Component7 <= 6)
                {
                    Component7 = 3;
                }
                //Global PSQI Score
                questionnaire.N_Result = Component1 + Component2 + Component3 + Component4 + Component5 + Component6 + Component7;
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