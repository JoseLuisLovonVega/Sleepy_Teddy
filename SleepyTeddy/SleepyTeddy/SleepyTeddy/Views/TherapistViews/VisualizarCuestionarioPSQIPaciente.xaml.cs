using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualizarCuestionarioPSQIPaciente : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        public VisualizarCuestionarioPSQIPaciente(string questionnaireID)
        {
            id_questionnaire = questionnaireID;
            getQuestionnaire();
            InitializeComponent();
        }
        private async void getQuestionnaire()
        {
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Questionnaire_ID", id_questionnaire)
                                       .GetAsync();
            questionnaire = document.Documents.ElementAt(0).ToObject<Questionnaire>();
            answer1.Text = questionnaire.PSQI1;
            answer2.Text = questionnaire.PSQI2;
            answer3.Text = questionnaire.PSQI3;
            answer4.Text = questionnaire.PSQI4;
            answer5a.Text = questionnaire.PSQI5a;
            answer5b.Text = questionnaire.PSQI5b;
            answer5c.Text = questionnaire.PSQI5c;
            answer5d.Text = questionnaire.PSQI5d;
            answer5e.Text = questionnaire.PSQI5e;
            answer5f.Text = questionnaire.PSQI5f;
            answer5g.Text = questionnaire.PSQI5g;
            answer5h.Text = questionnaire.PSQI5h;
            answer5i.Text = questionnaire.PSQI5i;
            answer5j.Text = questionnaire.PSQI5j;
            answer6.Text = questionnaire.PSQI6;
            answer7.Text = questionnaire.PSQI7;
            answer8.Text = questionnaire.PSQI8;
            answer9.Text = questionnaire.PSQI9;
        }
        private async void btnRegresar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}