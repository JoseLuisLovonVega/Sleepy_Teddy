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
using Microcharts;
using SkiaSharp;
using System.Diagnostics;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualizarResultadosCuestionariosPaciente : ContentPage
    {
        string id_patient;
        int count;
        Patient patient = new Patient();
        List<ChartEntry> DataLineGraph;
        List<QuestionnairesView> listData;
        List<QuestionnairesView> listresultsQuestionnairePatientSearched;
        List<string> listColorResults = new List<string>();
        public GetDataFromLoginUser objSearch { get; set; }
        //

        public VisualizarResultadosCuestionariosPaciente(string key_patient)
        {
            id_patient = key_patient;
            InitializeComponent();
            getPatient();
            listColorResults.Add("#50A0F0");
            listColorResults.Add("#EBF038");
            listColorResults.Add("#B455B6");
            listColorResults.Add("#3498DB");
            listColorResults.Add("#AA0CA5");
            listColorResults.Add("#21EF30");
            listColorResults.Add("#F20A0A");
            /*PatientInfo.Text = "Paciente: " + patient.Names + " " + patient.Last_Names;*/
        }
        private async void getPatient()
        {
            String role_id = "2";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Patient_ID", id_patient)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            PatientInfo.Text = "Paciente: " + patient.Names + " " + patient.Last_Names;
            objSearch = new GetDataFromLoginUser();
            cbxPatientInfo.ItemsSource = objSearch.ListPatientInfo;
        }
        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private void LoadItems()
        {
            objSearch = new GetDataFromLoginUser();
            BindingContext = objSearch;
            cbxPatientInfo.ItemsSource = objSearch.ListPatientInfo;
        }*/

        private async void btnBuscar_clicked(object sender, EventArgs e)
        {
            objSearch = new GetDataFromLoginUser();
            if (cbxPatientInfo.SelectedItem == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe seleccionar un tipo de registro de monitoreo del sueño", new TimeSpan(3));
            }
            else if (cbxPatientInfo.SelectedItem.ToString() == "PHQ-9" || cbxPatientInfo.SelectedItem.ToString() == "ISI"
                  || cbxPatientInfo.SelectedItem.ToString() == "PSQI")
            {
                lineGraphView.IsVisible = false;
                btnFiltrar2.Text = "Filtrar Según un Gráfico";
                btnFiltrar2.IsVisible = false;
                listresultsQuestionnairePatientSearched = new List<QuestionnairesView>();
                await objSearch.GetQuestionnaireResultsViewAsync(cbxPatientInfo.SelectedItem.ToString(), id_patient);
                //Ordenar de la más antigua a la más reciente
                objSearch.ListQuestionnaireData = objSearch.ListQuestionnaireData.OrderByDescending(o => o.D_Assigned_Date).ToList();
                if (objSearch.ListQuestionnaireData.Count == 0)
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                }
                else
                {
                    for (int i = 0; i < objSearch.ListQuestionnaireData.Count; i++)
                    {
                        if (objSearch.ListQuestionnaireData.ElementAt(i).D_Completed_Date.ToString("dd/MM/yyyy") != DateTime.MinValue.ToString("dd/MM/yyyy"))
                        {
                            listresultsQuestionnairePatientSearched.Add(objSearch.ListQuestionnaireData.ElementAt(i));
                        }
                    }
                    if (listresultsQuestionnairePatientSearched.Count == 0)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                    }
                    else
                    {
                        list_questionnaireResults.ItemsSource = listresultsQuestionnairePatientSearched;
                        list_questionnaireResults.IsVisible = true;
                        if ((list_questionnaireResults.ItemsSource as List<QuestionnairesView>).Count > 1)
                        {
                            btnFiltrar2.IsVisible = true;
                        }
                    }
                }
            }
        }

        private void btnFiltrar2_clicked(object sender, EventArgs e)
        {
            if (btnFiltrar2.Text == "Filtrar Según un Gráfico")
            {
                btnFiltrar2.Text = "Retirar el Filtrado";
                lineGraphView.IsVisible = true;
                list_questionnaireResults.IsVisible = false;
                DataLineGraph = new List<ChartEntry>();
                listData = new List<QuestionnairesView>();
                //Ordenar de la más reciente a la más antigua
                listData = listresultsQuestionnairePatientSearched.OrderByDescending(o => o.D_Assigned_Date).ToList();
                if (listData.Count() <= 7)
                {
                    listData = listData.GetRange(0, listresultsQuestionnairePatientSearched.Count);
                }
                else
                {
                    listData = listData.GetRange(0, 7);
                }
                //Ordenar de la más antigua a la más reciente
                listData = listData.OrderBy(o => o.D_Assigned_Date).ToList();
                for (int i = 0; i < listData.Count; i++)
                {
                    DataLineGraph.Add(new ChartEntry(listData.ElementAt(i).N_Result)
                    {
                        Label = listData.ElementAt(i).D_Completed_Date_S,
                        ValueLabel = listData.ElementAt(i).N_Result.ToString(),
                        Color = SKColor.Parse(listColorResults.ElementAt(i))
                    });
                }
                var lineGraph = new LineChart() { Entries = DataLineGraph, LabelTextSize = 35, ValueLabelOrientation = Orientation.Horizontal };
                lineGraphView.HeightRequest = 300;
                lineGraphView.Chart = lineGraph;
            }
            else
            {
                btnFiltrar2.Text = "Filtrar Según un Gráfico";
                lineGraphView.IsVisible = false;
                list_questionnaireResults.IsVisible = true;
            }

        }
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var dataItem = e.Item as QuestionnairesView;

            if (dataItem.Type == "ISI")
            {
                await Application.Current.MainPage.Navigation.PushAsync(new VisualizarCuestionarioISIPaciente(dataItem.Key));
            }
            else if (dataItem.Type == "PHQ-9")
            {
                await Application.Current.MainPage.Navigation.PushAsync(new VisualizarCuestionarioPHQ9Paciente(dataItem.Key));
            }
            else if (dataItem.Type == "PSQI")
            {

                await Application.Current.MainPage.Navigation.PushAsync(new VisualizarCuestionarioPSQIPaciente(dataItem.Key));
            }

           //Deselect Item
           ((ListView)sender).SelectedItem = null;
        }

        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}