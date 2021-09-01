using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using SleepyTeddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SleepyTeddy.ViewModel.GetDataFromLoginUser;

namespace SleepyTeddy.Views.TherapistViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AsignarCuestionarioPaciente : ContentPage
    {
        //SearchPatientView aa = new SearchPatientView();      
        public GetDataFromLoginUser objSearch { get; set; }
        List<SleepWakeDiariesView> listSleepWakeDiaries;

        public AsignarCuestionarioPaciente()
        {
            InitializeComponent();
            LoadItems();
        }
        //Método para bloquear boton retroceder
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new AsignarCuestionarioPaciente());
            });
            return true;
        }

        private async void AccountSelectTerapeuta(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MiCuenta());
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();          
            LoadItems();            
        }

        private async void LoadItems()
        {
            objSearch = new GetDataFromLoginUser();
            await objSearch.GetPatientsTherapistViewAsync();
            BindingContext = objSearch;
        }
     

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            var ObjPatientSel =(PatientsView)lista_pacientes.SelectedItem;
            var questionnaireSel = lista_cuestionarios.SelectedItem;

            if (questionnaireSel == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar un tipo de cuestionario.", new TimeSpan(3));
            }else if (ObjPatientSel == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar un paciente.", new TimeSpan(3));
            }
            else if (questionnaireSel.ToString() == "PSQI")
            {
                await objSearch.GetSleepWakeDiariesViewAsync(ObjPatientSel.Key);
                listSleepWakeDiaries = new List<SleepWakeDiariesView>();
                foreach (var SWDiary in objSearch.ListSleepWakeDiaries)
                {
                    if (SWDiary.CreatedDate.Month == DateTime.Now.AddHours(-5).Month-1 && SWDiary.CreatedDate.Year == DateTime.Now.AddHours(-5).Year)
                    {
                        listSleepWakeDiaries.Add(SWDiary);
                    }
                }
                Debug.WriteLine("listSleepWakeDiaries.Count: "+ listSleepWakeDiaries.Count);
                if (listSleepWakeDiaries.Count > 0)
                {
                    await CrossCloudFirestore.Current
                             .Instance
                             .Collection("Questionnaires")
                             .AddAsync(new Questionnaire
                             {
                                 Questionnaire_ID = Guid.NewGuid().ToString(),
                                 Patient_ID = ObjPatientSel.Key,
                                 Therapist_ID = LoginViewModel.Therapist_ID,
                                 Type = questionnaireSel.ToString(),
                                 N_Result = 0,
                                 D_Assigned_Date = DateTime.Now.AddHours(-5),
                                 D_Completed_Date = DateTime.MinValue
                             });
                    await DisplayAlert("Asignación Exitosa", "Cuestionario asignado al paciente correctamente", "OK");
                    lista_cuestionarios.SelectedItem = null;
                    lista_pacientes.SelectedItem = null;
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("No existen diarios de sueño-vigilia del último mes del paciente ingresado para incorporar al cuestionario.", new TimeSpan(15));
                    lista_cuestionarios.SelectedItem = null;
                }
            } else {
                await CrossCloudFirestore.Current
                             .Instance
                             .Collection("Questionnaires")
                             .AddAsync(new Questionnaire
                             {
                                 Questionnaire_ID = Guid.NewGuid().ToString(),
                                 Patient_ID = ObjPatientSel.Key,
                                 Therapist_ID = LoginViewModel.Therapist_ID,
                                 Type = questionnaireSel.ToString(),
                                 N_Result = 0,
                                 D_Assigned_Date = DateTime.Now.AddHours(-5),
                                 D_Completed_Date = DateTime.MinValue
                             });
                await DisplayAlert("Asignación Exitosa", "Cuestionario asignado al paciente correctamente", "OK");
                lista_cuestionarios.SelectedItem = null;
                lista_pacientes.SelectedItem=null;
            }
        }
    }
}