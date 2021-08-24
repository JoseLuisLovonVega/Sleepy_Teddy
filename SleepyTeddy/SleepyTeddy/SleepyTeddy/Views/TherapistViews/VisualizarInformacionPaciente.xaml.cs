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
        List<SleepWakeDiariesView> listDataSWDiary;
        List<QuestionnairesView> listresultsQuestionnairePatientSearched;
        List<SleepWakeDiariesView> listSleepWakeDiariesPatientSearched;
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
            objSearch = new GetDataFromLoginUser();
            if (cbxPatientInfo.SelectedItem==null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe seleccionar un tipo de registro de monitoreo del sueño", new TimeSpan(3));
            } else { 
                if (cbxPatientInfo.SelectedItem.ToString() == "Diario de Sueño-Vigilia") {
                    btnFiltrar2.IsVisible = false;
                    list_questionnaireResults.IsVisible = false;
                    listSleepWakeDiariesPatientSearched = new List<SleepWakeDiariesView>();
                    await objSearch.GetSleepWakeDiariesViewAsync(id_patient);
                    objSearch.ListSleepWakeDiaries=objSearch.ListSleepWakeDiaries.OrderByDescending(o => o.CreatedDate).ToList();
                    if (objSearch.ListSleepWakeDiaries.Count == 0)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No se obtuvieron resultados", new TimeSpan(3));
                    }
                    else
                    {
                        list_questionnaireResults.IsVisible = false;
                        btnFiltrar2.IsVisible = false;
                        cbxResultsSleepWakeDiary.ItemsSource = objSearch.ListResultsSleepWakeDiary;
                        cbxResultsSleepWakeDiary.SelectedIndex = 0; //.SelectedItem = "Hora a la que durmió";
                        cbxResultsSleepWakeDiary.IsVisible = true;
                        btnBuscar2.IsVisible = true;
                        for (int i = 0; i < objSearch.ListSleepWakeDiaries.Count; i++)
                        {
                            listSleepWakeDiariesPatientSearched.Add(objSearch.ListSleepWakeDiaries.ElementAt(i)); 
                        }
                        list_sleepWakeDiariesSleepTime.ItemsSource = listSleepWakeDiariesPatientSearched;
                        if ((list_sleepWakeDiariesSleepTime.ItemsSource as List<SleepWakeDiariesView>).Count >= 7)
                        {
                            btnFiltrar.IsVisible = true;
                        }
                    }
                } else {
                    btnFiltrar.IsVisible = false;
                    cbxResultsSleepWakeDiary.IsVisible = false;
                    btnBuscar2.IsVisible = false;
                    list_sleepWakeDiariesHoursSlept.IsVisible = false;
                    list_sleepWakeDiariesWakeUpTime.IsVisible = false;
                    list_sleepWakeDiariesSleepTime.IsVisible = false;
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
                        list_questionnaireResults.ItemsSource = listresultsQuestionnairePatientSearched;
                        list_questionnaireResults.IsVisible = true;
                        if ((list_questionnaireResults.ItemsSource as List<QuestionnairesView>).Count >= 7)
                        {
                            btnFiltrar2.IsVisible = true;
                        }
                    }   
                }
            }
        }

        private void btnBuscar2_clicked(object sender, EventArgs e)
        {
            if(cbxResultsSleepWakeDiary.SelectedItem.ToString()=="Hora a la que durmió")
            {
                btnFiltrar.IsVisible = false;
                list_sleepWakeDiariesHoursSlept.IsVisible = false;
                list_sleepWakeDiariesWakeUpTime.IsVisible = false;
                list_sleepWakeDiariesSleepTime.IsVisible = true;
                if ((list_sleepWakeDiariesSleepTime.ItemsSource as List<SleepWakeDiariesView>).Count >= 7)
                {
                    btnFiltrar.IsVisible = true;
                }

            }
            else if(cbxResultsSleepWakeDiary.SelectedItem.ToString() == "Hora a la que despertó el día siguiente")
            {
                btnFiltrar.IsVisible = false;
                list_sleepWakeDiariesHoursSlept.IsVisible = false;
                list_sleepWakeDiariesSleepTime.IsVisible = false;
                list_sleepWakeDiariesWakeUpTime.ItemsSource = listSleepWakeDiariesPatientSearched;
                list_sleepWakeDiariesWakeUpTime.IsVisible = true;
                if ((list_sleepWakeDiariesWakeUpTime.ItemsSource as List<SleepWakeDiariesView>).Count >= 7)
                {
                    btnFiltrar.IsVisible = true;
                }

            }
            else if (cbxResultsSleepWakeDiary.SelectedItem.ToString() == "Cantidad de horas que durmió")
            {
                btnFiltrar.IsVisible = false;
                list_sleepWakeDiariesWakeUpTime.IsVisible = false;
                list_sleepWakeDiariesSleepTime.IsVisible = false;
                list_sleepWakeDiariesHoursSlept.ItemsSource = listSleepWakeDiariesPatientSearched;
                list_sleepWakeDiariesHoursSlept.IsVisible = true;
                if ((list_sleepWakeDiariesHoursSlept.ItemsSource as List<SleepWakeDiariesView>).Count >= 7)
                {
                    btnFiltrar.IsVisible = true;
                }
            }
        }

        private void btnFiltrar_clicked(object sender, EventArgs e)
        {
            int count=-1;
            if (btnFiltrar.Text == "Filtrar Según: 7 Recientes Diarios Registrados")
            {
                btnFiltrar.Text = "Retirar el Filtrado";
                lineGraphView.IsVisible = true;
                listColorResults.Add("#50A0F0");
                listColorResults.Add("#EBF038");
                listColorResults.Add("#B455B6");
                listColorResults.Add("#3498DB");
                listColorResults.Add("#AA0CA5");
                listColorResults.Add("#21EF30");
                listColorResults.Add("#F3352C");
                list_questionnaireResults.IsVisible = false;
                DataLineGraph = new List<ChartEntry>();
                listDataSWDiary = new List<SleepWakeDiariesView>();
                listDataSWDiary = listSleepWakeDiariesPatientSearched.OrderBy(o => o.CreatedDate).ToList();
                listDataSWDiary = listDataSWDiary.GetRange(0, 7);
                listDataSWDiary = listDataSWDiary.OrderByDescending(o => o.CreatedDate).ToList();
                if (list_sleepWakeDiariesSleepTime.IsVisible == true)
                {
                    count = 0;
                    list_sleepWakeDiariesSleepTime.IsVisible = false;
                    for (int i = 0; i < listDataSWDiary.Count; i++)
                    {
                        DataLineGraph.Add(new ChartEntry(i)
                        {
                            Label = listDataSWDiary.ElementAt(i).CreatedDate_S,
                            ValueLabel = listDataSWDiary.ElementAt(i).SleepTime_S,
                            Color = SKColor.Parse(listColorResults.ElementAt(i))
                        });
                    }
                }
                else if (list_sleepWakeDiariesWakeUpTime.IsVisible == true)
                {
                    count = 1;
                    list_sleepWakeDiariesWakeUpTime.IsVisible = false;
                    for (int i = 0; i < listDataSWDiary.Count; i++)
                    {
                        DataLineGraph.Add(new ChartEntry(i)
                        {
                            Label = listDataSWDiary.ElementAt(i).CreatedDate_S,
                            ValueLabel = listDataSWDiary.ElementAt(i).WakeUpTime_S,
                            Color = SKColor.Parse(listColorResults.ElementAt(i))
                        });
                    }
                }
                else if (list_sleepWakeDiariesHoursSlept.IsVisible == true)
                {
                    count = 2;
                    list_sleepWakeDiariesHoursSlept.IsVisible = false;
                    for (int i = 0; i < listDataSWDiary.Count; i++)
                    {
                        DataLineGraph.Add(new ChartEntry(i)
                        {
                            Label = listDataSWDiary.ElementAt(i).CreatedDate_S,
                            ValueLabel = listDataSWDiary.ElementAt(i).HoursSlept.ToString(),
                            Color = SKColor.Parse(listColorResults.ElementAt(i))
                        });
                    }
                }
                
                var lineGraph = new LineChart() { Entries = DataLineGraph, LabelTextSize = 35, ValueLabelOrientation = Orientation.Horizontal };
                lineGraphView.HeightRequest = 300;
                lineGraphView.Chart = lineGraph;
            }
            else
            {
                btnFiltrar.Text = "Filtrar Según: 7 Recientes Diarios Registrados";
                lineGraphView.IsVisible = false;
                if (count == 0)
                {
                    list_sleepWakeDiariesSleepTime.IsVisible = true;
                }
                else if (count == 1)
                {
                    list_sleepWakeDiariesWakeUpTime.IsVisible = true;
                }
                else if (count == 2)
                {
                    list_sleepWakeDiariesHoursSlept.IsVisible = true;
                }
            }
        }

        private void btnFiltrar2_clicked(object sender, EventArgs e)
        {
            if (btnFiltrar2.Text == "Filtrar Según: 7 Recientes Cuestionarios Realizados")
            {
                btnFiltrar2.Text = "Retirar el Filtrado";
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
                listData = listresultsQuestionnairePatientSearched.OrderBy(o => o.D_Completed_Date).ToList();
                listData = listData.GetRange(0, 7);
                listData = listData.OrderByDescending(o => o.D_Completed_Date).ToList();
                for (int i = 0; i < listData.Count; i++)
                {
                    DataLineGraph.Add(new ChartEntry(i)
                    {
                        Label = listData.ElementAt(i).D_Completed_Date_S,
                        ValueLabel = listData.ElementAt(i).N_Result.ToString(),
                        Color = SKColor.Parse(listColorResults.ElementAt(i))
                    });
                }
                var lineGraph = new LineChart() { Entries = DataLineGraph, LabelTextSize = 35, ValueLabelOrientation= Orientation.Horizontal };
                lineGraphView.HeightRequest = 300;
                lineGraphView.Chart = lineGraph;
            } else {
                btnFiltrar2.Text = "Filtrar Según: 7 Recientes Cuestionarios Realizados";
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