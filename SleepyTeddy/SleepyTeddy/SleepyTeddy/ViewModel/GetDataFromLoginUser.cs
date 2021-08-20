using AutoMapper;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Resources;

namespace SleepyTeddy.ViewModel
{
    public class GetDataFromLoginUser : INotifyPropertyChanged
    {
        //public ObservableCollection<MyModel> Items { get; set; }
        public List<PatientsView> ListPatientsTherapist { get; set; }
        public List<TherapistsView> ListTherapistsAdministrator { get; set; }
        public List<PatientsView> ListPatientsAdministrator { get; set; }
        public List<string> ListQuestionnaires { get; set; }
        public List<string> ListPatientInfo { get; set; }
        public List<QuestionnairesView> ListQuestionnairesPatient { get; set; }
        public List<QuestionnairesView> ListQuestionnaireData { get; set; }
        public List<SleepRecordsView> ListSleepRecords { get; set; }
        public List<SleepRecordsView> ListSleepRecordsDateFilter { get; set; }
        public List<SleepWakeDiariesView> ListSleepWakeDiaries { get; set; }

        public GetDataFromLoginUser()
        {
            ListQuestionnaires = new List<string>();
            ListQuestionnaires.Add("PHQ-9");
            ListQuestionnaires.Add("ISI");
            ListQuestionnaires.Add("PSQI");
            ListPatientInfo = new List<string>();
            ListPatientInfo.Add("Diario de Sueño-Vigilia");
            ListPatientInfo.Add("PHQ-9");
            ListPatientInfo.Add("ISI");
            ListPatientInfo.Add("PSQI");
        }



        public event PropertyChangedEventHandler PropertyChanged;

        //Obtener los pacientes en base al terapeuta que inicio sesión
        public async Task GetPatientsTherapistViewAsync()
        {
            ListPatientsTherapist = new List<PatientsView>();
            string role_id = "2";
            string therapistId = LoginViewModel.Therapist_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Therapist_ID", therapistId)
                                       .GetAsync();

            var resModel = document.ToObjects<Patient>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Patient, PatientsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Patient_ID))
                .ForMember(d => d.nombre_completo, o => o.MapFrom(c => c.Names + " " + c.Last_Names));
            });

            resModel.ForEach(a => ListPatientsTherapist.Add(config.CreateMapper().Map<Patient, PatientsView>(a)));
            return;
        }

        //Obtener los pacientes del administrador que inicio sesión
        public async Task GetPatientsAdministratorViewAsync()
        {
            ListPatientsAdministrator = new List<PatientsView>();
            string role_id = "2";
            string administratorId = LoginViewModel.Administrator_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("administrator_ID", administratorId)
                                       .GetAsync();

            var resModel = document.ToObjects<Patient>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Patient, PatientsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Patient_ID))
                .ForMember(d => d.nombre_completo, o => o.MapFrom(c => c.Names + " " + c.Last_Names));
            });

            resModel.ForEach(a => ListPatientsAdministrator.Add(config.CreateMapper().Map<Patient, PatientsView>(a)));
            return;
        }

        //Obtener los terapeutas del administrador que inicio sesión
        public async Task GetTherapistsAdministratorViewAsync()
        {
            ListTherapistsAdministrator = new List<TherapistsView>();
            string role_id = "3";
            string administratorId = LoginViewModel.Administrator_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("administrator_ID", administratorId)
                                       .GetAsync();

            var resModel = document.ToObjects<Therapist>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Therapist, TherapistsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Therapist_ID))
                .ForMember(d => d.nombre_completo, o => o.MapFrom(c => c.Names + " " + c.Last_Names));
            });

            resModel.ForEach(a => ListTherapistsAdministrator.Add(config.CreateMapper().Map<Therapist, TherapistsView>(a)));
            return;
        }

        //Obtener los cuestionarios del paciente que inicio sesión
        public async Task GetQuestionnairesViewAsync()
        {
            ListQuestionnairesPatient = new List<QuestionnairesView>();
            string patientId = LoginViewModel.Patient_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Patient_ID", patientId)
                                       .WhereEqualsTo("D_Completed_Date", DateTime.MinValue)
                                       .GetAsync();

            var resModel = document.ToObjects<Questionnaire>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Questionnaire_ID))
                .ForMember(d => d.Type, o => o.MapFrom(c => c.Type));
            });

            resModel.ForEach(a => ListQuestionnairesPatient.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
            return;
        }
        //Obtener datos de los cuestionarios del paciente seleccionado por el terapeuta que inició sesión
        public async Task GetQuestionnaireResultsViewAsync(string type_searched, string patientId)
        {
            ListQuestionnaireData = new List<QuestionnairesView>();
            string therapistId = LoginViewModel.Therapist_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Therapist_ID", therapistId)
                                       .WhereEqualsTo("Patient_ID", patientId)
                                       .WhereEqualsTo("Type", type_searched)
                                       .GetAsync();

            var resModel = document.ToObjects<Questionnaire>().ToList();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Questionnaire_ID))
                .ForMember(d => d.Type, o => o.MapFrom(c => c.Type))
                .ForMember(d => d.D_Assigned_Date, o => o.MapFrom(c => c.D_Assigned_Date))
                .ForMember(d => d.D_Assigned_Date_S, o => o.MapFrom(c => c.D_Assigned_Date.ToString("dd/MM/yy")))
                .ForMember(d => d.D_Completed_Date, o => o.MapFrom(c => c.D_Completed_Date))
                .ForMember(d => d.D_Completed_Date_S, o => o.MapFrom(c => c.D_Completed_Date.ToString("dd/MM/yy")))
                .ForMember(d => d.N_Result, o => o.MapFrom(c => c.N_Result));
            });

            resModel.ForEach(a => ListQuestionnaireData.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
            return;
        }
        //Obtener los sleep records del paciente que inició sesión para completar los campos del SleepWakeDiary
        public async Task GetSleepRecordsViewAsync()
        {
            ListSleepRecords = new List<SleepRecordsView>();
            string patientId = LoginViewModel.Patient_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("SleepRecords")
                                       .WhereEqualsTo("Patient_ID", patientId)
                                       .GetAsync();

            var resModel = document.ToObjects<SleepRecord>().ToList();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SleepRecord, SleepRecordsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.SleepRecord_ID))
                .ForMember(d => d.DateTimeHour, o => o.MapFrom(c => c.DateTimeHour))
                .ForMember(d => d.Kind, o => o.MapFrom(c => c.Kind));
            });

            resModel.ForEach(a => ListSleepRecords.Add(config.CreateMapper().Map<SleepRecord, SleepRecordsView>(a)));
            return;
        }
        //Obtener los sleep records del paciente que inició sesión según una fecha en específico
        public async Task GetSleepRecordsDateFilterViewAsync(int contador)
        {
            ListSleepRecordsDateFilter = new List<SleepRecordsView>();
            string patientId = LoginViewModel.Patient_ID;
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("SleepRecords")
                                       .WhereEqualsTo("Patient_ID", patientId)
                                       .WhereEqualsTo("DateTimeHour", DateTime.Today.AddDays(contador))
                                       .GetAsync();

            var resModel = document.ToObjects<SleepRecord>().ToList();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SleepRecord, SleepRecordsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.SleepRecord_ID))
                .ForMember(d => d.DateTimeHour, o => o.MapFrom(c => c.DateTimeHour))
                .ForMember(d => d.Kind, o => o.MapFrom(c => c.Kind));
            });

            resModel.ForEach(a => ListSleepRecordsDateFilter.Add(config.CreateMapper().Map<SleepRecord, SleepRecordsView>(a)));
            return;
        }

        //Obtener los diarios de sueño del paciente seleccionado por el terapeuta que inició sesión
        public async Task GetSleepWakeDiariesViewAsync(string patientId)
        {
            ListSleepWakeDiaries = new List<SleepWakeDiariesView>();
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("SleepWakeDiaries")
                                       .WhereEqualsTo("Patient_ID", patientId)
                                       .GetAsync();

            var resModel = document.ToObjects<SleepWakeDiary>().ToList();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SleepWakeDiary, SleepWakeDiariesView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.SleepWakeDiary_ID))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(c => c.CreatedDate))
                .ForMember(d => d.SleepTime, o => o.MapFrom(c => c.SleepTime))
                .ForMember(d => d.WakeUpTime, o => o.MapFrom(c => c.WakeUpTime))
                .ForMember(d => d.HoursSlept, o => o.MapFrom(c => c.HoursSlept))
                .ForMember(d => d.SleepEfficiency, o => o.MapFrom(c => c.SleepEfficiency));
            });

            resModel.ForEach(a => ListSleepWakeDiaries.Add(config.CreateMapper().Map<SleepWakeDiary, SleepWakeDiariesView>(a)));
            return;
        }
    }
    public class PatientsView
    {
        public string Key { get; set; }
        public string Names { get; set; }
        public string LastNames { get; set; }
        public string nombre_completo { get; set; }
    }

    public class TherapistsView
    {
        public string Key { get; set; }
        public string Names { get; set; }
        public string LastNames { get; set; }
        public string nombre_completo { get; set; }
    }

    public class QuestionnairesView
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public DateTime D_Assigned_Date { get; set; }
        public string D_Assigned_Date_S { get; set; }
        public DateTime D_Completed_Date { get; set; }
        public string D_Completed_Date_S { get; set; }
        public float N_Result { get; set; }
    }
    public class SleepRecordsView
    {
        public string Key { get; set; }
        public DateTime DateTimeHour { get; set; }
        public int Kind { get; set; }
    }
    public class SleepWakeDiariesView
    {
        public string Key { get; set; }
        public string SleepWakeDiary_ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SleepTime { get; set; }
        public DateTime WakeUpTime { get; set; }
        public Double HoursSlept { get; set; }
        public Double SleepEfficiency { get; set; }
    }

    public class ListQuestionnaireResultsPatient : ObservableCollection<QuestionnairesView> { }
}
