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
    public partial class VisualizarCuestionarioISIPaciente : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        public VisualizarCuestionarioISIPaciente(string questionnaire_ID)
        {
            id_questionnaire = questionnaire_ID;
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
            answer1a.Text = questionnaire.ISI1a;
            answer1b.Text = questionnaire.ISI1b;
            answer1c.Text = questionnaire.ISI1c;
            answer2.Text = questionnaire.ISI2;
            answer3.Text = questionnaire.ISI3;
            answer4.Text = questionnaire.ISI4;
            answer5.Text = questionnaire.ISI5;
        }
        private async void btnRegresar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}