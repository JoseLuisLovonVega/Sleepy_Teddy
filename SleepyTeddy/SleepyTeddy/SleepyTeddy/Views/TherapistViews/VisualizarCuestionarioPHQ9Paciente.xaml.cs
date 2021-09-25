using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualizarCuestionarioPHQ9Paciente : ContentPage
    {
        string id_questionnaire;
        Questionnaire questionnaire;
        public VisualizarCuestionarioPHQ9Paciente(string questionnaireID)
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
            answer1.Text = questionnaire.PHQ91;
            answer2.Text = questionnaire.PHQ92;
            answer3.Text = questionnaire.PHQ93;
            answer4.Text = questionnaire.PHQ94;
            answer5.Text = questionnaire.PHQ95;
            answer6.Text = questionnaire.PHQ96;
            answer7.Text = questionnaire.PHQ97;
            answer8.Text = questionnaire.PHQ98;
            answer9.Text = questionnaire.PHQ99;
        }
        private async void btnRegresar_clicked(object sender, EventArgs e)
        {

            await Navigation.PopAsync();
        }
    }
}