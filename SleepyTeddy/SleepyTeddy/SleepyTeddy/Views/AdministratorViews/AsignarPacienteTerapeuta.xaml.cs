using Plugin.CloudFirestore;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views.AdministratorViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AsignarPacienteTerapeuta : ContentPage
    {
        string documentID;
        string documentID2;
        string Therapist_ID;
        string Patient_ID;
        Patient patient;
        Questionnaire questionnaire;
        public GetDataFromLoginUser objTableBinding { get; set; }
        public AsignarPacienteTerapeuta()
        {
            InitializeComponent();
            LoadItems();
            cbxPacients.SelectedItem = null;
            cbxTherapists.SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new AsignarPacienteTerapeuta());
            });
            return true;
        }

        /*protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }*/
        private async void LoadItems()
        {
            objTableBinding = new GetDataFromLoginUser();
            await objTableBinding.GetTherapistsAdministratorViewAsync();
            await objTableBinding.GetPatientsAdministratorViewAsync();
            cbxTherapists.ItemsSource = objTableBinding.ListTherapistsAdministrator;
            cbxPacients.ItemsSource = objTableBinding.ListPatientsAdministrator;
        }

        private async void btnAceptar_clicked(object sender, EventArgs e)
        {
            var ObjPatientSel = (PatientsView)cbxPacients.SelectedItem;
            var ObjTherapistSel = (TherapistsView)cbxTherapists.SelectedItem;

            if (ObjTherapistSel == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar un terapeuta.", new TimeSpan(3));
            }
            else if (ObjPatientSel == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("Debe ingresar un paciente.", new TimeSpan(3));
            }
            else if (ObjPatientSel.TherapistID != null && ObjPatientSel.TherapistID == ObjTherapistSel.Key)
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("El terapeuta ingresado ya se encuentra asignado al paciente.", new TimeSpan(3));
            }
            else
            {
                Patient_ID = ObjPatientSel.Key;
                Therapist_ID = ObjTherapistSel.Key;

                if (ObjPatientSel.TherapistID != Therapist_ID)
                {
                    await objTableBinding.GetAllQuestionnairesViewAsync(Patient_ID);
                    if (objTableBinding.ListAllQuestionnairesPatient.Count > 0)
                    {
                        for(int k=0; k < objTableBinding.ListAllQuestionnairesPatient.Count; k++)
                        {
                            var document2 = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Questionnaire_ID", objTableBinding.ListAllQuestionnairesPatient.ElementAt(k).Key)
                                       .GetAsync();
                            questionnaire = document2.Documents.ElementAt(0).ToObject<Questionnaire>();
                            documentID2 = document2.Documents.ElementAt(0).Id;
                            questionnaire.Therapist_ID = Therapist_ID;

                            await CrossCloudFirestore.Current
                            .Instance
                            .Collection("Questionnaires")
                            .Document(documentID2)
                            .UpdateAsync(questionnaire);
                        }
                    }
                }
                //Obtener el documento del paciente seleccionado
                var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", "2")
                                       .WhereEqualsTo("Patient_ID", Patient_ID)
                                       .GetAsync();
                patient= document.Documents.ElementAt(0).ToObject<Patient>();
                documentID = document.Documents.ElementAt(0).Id;
                patient.Therapist_ID = Therapist_ID;

                await CrossCloudFirestore.Current
                    .Instance
                    .Collection("Users")
                    .Document(documentID)
                    .UpdateAsync(patient);
                await DisplayAlert("Asignación Exitosa", "Paciente asignado al terapeuta correctamente", "OK");
                //await Navigation.PopAsync();
                cbxPacients.SelectedItem = null;
                cbxTherapists.SelectedItem = null;
            }
        }
    }
}