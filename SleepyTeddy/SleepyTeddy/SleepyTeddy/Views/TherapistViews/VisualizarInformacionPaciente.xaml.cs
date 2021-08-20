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

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualizarInformacionPaciente : ContentPage
    {
        string id_patient;
        Patient patient=new Patient();
        List<ChartEntry> DataLineGraph;
        List<QuestionnairesView> listData;
        List<QuestionnairesView> listresultsQuestionnairePatientSearched;
        List<string> listColorResults = new List<string>();
        public GetDataFromLoginUser objSearch { get; set; }
        //

        public VisualizarInformacionPaciente(string key_patient)
        {
            id_patient = key_patient;
            InitializeComponent();
            getPatient();
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
            if (cbxPatientInfo.SelectedItem==null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe seleccionar un tipo de registro de monitoreo del sueño", new TimeSpan(3));
            } else { 
                if (cbxPatientInfo.SelectedItem.ToString() == "Diario de Sueño-Vigilia") {
                    //
                } else {
                    objSearch = new GetDataFromLoginUser();
                    listresultsQuestionnairePatientSearched = new List<QuestionnairesView>();
                    await objSearch.GetQuestionnaireResultsViewAsync(cbxPatientInfo.SelectedItem.ToString(), id_patient);
                    objSearch.ListQuestionnaireData = objSearch.ListQuestionnaireData.OrderByDescending(o => o.D_Assigned_Date).ToList();
                    if (objSearch.ListQuestionnaireData.Count == 0)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                    } else {
                        for (int i = 0; i < objSearch.ListQuestionnaireData.Count; i++)
                        {
                            if (objSearch.ListQuestionnaireData.ElementAt(i).D_Completed_Date.ToString("dd/MM/yyyy") != "01/01/0001")
                            {
                                listresultsQuestionnairePatientSearched.Add(objSearch.ListQuestionnaireData.ElementAt(i));
                            }
                        }
                        
                        //listresultsQuestionnairePatientSearched.OrderByDescending(o => o.D_Assigned_Date.Ticks).ToList();
                        list_questionnaireResults.ItemsSource = listresultsQuestionnairePatientSearched;
                        if ((list_questionnaireResults.ItemsSource as List<QuestionnairesView>).Count >= 7)
                        {
                            btnFiltrar.IsVisible = true;
                            
                        }
                    }   
                }
            }
        }

        private void btnFiltrar_clicked(object sender, EventArgs e)
        {
            if (btnFiltrar.Text == "Filtrar los 7 Primeros Registros según F. Realización")
            {
                btnFiltrar.Text = "Retirar el Filtrado";
                lineGraphView.IsVisible = true;
                list_questionnaireResults.IsVisible = false;
                listColorResults.Add("#50A0F0");
                listColorResults.Add("#EBF038");
                listColorResults.Add("#B455B6");
                listColorResults.Add("#3498DB");
                listColorResults.Add("#AA0CA5");
                listColorResults.Add("#21EF30");
                listColorResults.Add("#F3352C");
                DataLineGraph = new List<ChartEntry>();
                listData = new List<QuestionnairesView>();
                listData = listresultsQuestionnairePatientSearched.OrderByDescending(o => o.D_Completed_Date).ToList();
                listData = listData.GetRange(0, 7);
                listData = listData.OrderBy(o => o.D_Completed_Date).ToList();
                for (int i = 0; i < listData.Count; i++)
                {
                    DataLineGraph.Add(new ChartEntry(listData.ElementAt(i).N_Result)
                    {
                        Label = listData.ElementAt(i).D_Completed_Date.ToString("dd/MM/yy"),
                        ValueLabel = listData.ElementAt(i).N_Result.ToString(),
                        Color = SKColor.Parse(listColorResults.ElementAt(i))
                    });
                }
                var lineGraph = new LineChart() { Entries = DataLineGraph, LabelTextSize = 35, ValueLabelOrientation= Orientation.Horizontal };
                lineGraphView.HeightRequest = 300;
                lineGraphView.Chart = lineGraph;
            } else {
                btnFiltrar.Text = "Filtrar los 7 Primeros Registros según F. Realización";
                lineGraphView.IsVisible = false;
                list_questionnaireResults.IsVisible = true;
            }
            
        }

        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}