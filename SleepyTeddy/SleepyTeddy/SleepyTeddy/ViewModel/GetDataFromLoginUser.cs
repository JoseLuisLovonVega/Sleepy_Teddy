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
        //public List<string> ListResultsSleepWakeDiary { get; set; }
        public List<QuestionnairesView> ListQuestionnairesPatient { get; set; }
        public List<QuestionnairesView> ListAllQuestionnairesPatient { get; set; }
        public List<QuestionnairesView> ListQuestionnaireData { get; set; }
        public List<SleepRecordsView> ListSleepRecords { get; set; }
        public List<SleepWakeDiariesView> ListSleepWakeDiaries { get; set; }


        public GetDataFromLoginUser()
        {
            ListQuestionnaires = new List<string>();
            ListQuestionnaires.Add("PHQ-9");
            ListQuestionnaires.Add("ISI");
            ListQuestionnaires.Add("PSQI");
            ListPatientInfo = new List<string>();
            ListPatientInfo.Add("PHQ-9");
            ListPatientInfo.Add("ISI");
            ListPatientInfo.Add("PSQI");
            /*ListResultsSleepWakeDiary = new List<string>();
            ListResultsSleepWakeDiary.Add("Hora a la que durmió");
            ListResultsSleepWakeDiary.Add("Hora a la que despertó el día siguiente");
            ListResultsSleepWakeDiary.Add("Cantidad de horas que durmió");*/
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
                .ForMember(d => d.TherapistID, o => o.MapFrom(c => c.Therapist_ID))
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

        //Obtener los cuestionarios no completados del paciente que inicio sesión
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
        //Obtener todos los cuestionarios del paciente que inicio sesión
        public async Task GetAllQuestionnairesViewAsync(string patient_ID)
        {
            ListAllQuestionnairesPatient = new List<QuestionnairesView>();
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Patient_ID", patient_ID)
                                       .GetAsync();

            var resModel = document.ToObjects<Questionnaire>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Questionnaire_ID))
                .ForMember(d => d.Type, o => o.MapFrom(c => c.Type));
            });

            resModel.ForEach(a => ListAllQuestionnairesPatient.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
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
                .ForMember(d => d.N_Result, o => o.MapFrom(c => Math.Round(c.N_Result, 2)));
            });

            resModel.ForEach(a => ListQuestionnaireData.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
            return;
        }
        //Obtener datos de los cuestionarios del paciente seleccionado por el terapeuta que inició sesión
        public async Task GetQuestionnaireResults2ViewAsync(string questionnaireID, string type)
        {
            ListQuestionnaireData = new List<QuestionnairesView>();
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Questionnaires")
                                       .WhereEqualsTo("Questionnaire_ID", questionnaireID)
                                       .GetAsync();

            var resModel = document.ToObjects<Questionnaire>().ToList();
            if (type == "ISI")
            {
                var config = new MapperConfiguration(cfg => { 
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.ISI1a, o => o.MapFrom(c => c.ISI1a))
                .ForMember(d => d.ISI1b, o => o.MapFrom(c => c.ISI1b))
                .ForMember(d => d.ISI1c, o => o.MapFrom(c => c.ISI1c))
                .ForMember(d => d.ISI2, o => o.MapFrom(c => c.ISI2))
                .ForMember(d => d.ISI3, o => o.MapFrom(c => c.ISI3))
                .ForMember(d => d.ISI4, o => o.MapFrom(c => c.ISI4))
                .ForMember(d => d.ISI5, o => o.MapFrom(c => c.ISI5));
            });
                resModel.ForEach(a => ListQuestionnaireData.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
                return;
            }
            else if (type == "PHQ-9")
            {
                var config = new MapperConfiguration(cfg => { 
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.PHQ91, o => o.MapFrom(c => c.PHQ91))
                .ForMember(d => d.PHQ92, o => o.MapFrom(c => c.PHQ92))
                .ForMember(d => d.PHQ93, o => o.MapFrom(c => c.PHQ93))
                .ForMember(d => d.PHQ94, o => o.MapFrom(c => c.PHQ94))
                .ForMember(d => d.PHQ95, o => o.MapFrom(c => c.PHQ95))
                .ForMember(d => d.PHQ96, o => o.MapFrom(c => c.PHQ96))
                .ForMember(d => d.PHQ97, o => o.MapFrom(c => c.PHQ97))
                .ForMember(d => d.PHQ98, o => o.MapFrom(c => c.PHQ98))
                .ForMember(d => d.PHQ99, o => o.MapFrom(c => c.PHQ99));
            });
                resModel.ForEach(a => ListQuestionnaireData.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
                return;
            }
            else if (type == "PSQI")
            {
                var config = new MapperConfiguration(cfg => { 
                cfg.CreateMap<Questionnaire, QuestionnairesView>()
                .ForMember(d => d.PSQI1, o => o.MapFrom(c => c.PSQI1))
                .ForMember(d => d.PSQI2, o => o.MapFrom(c => c.PSQI2))
                .ForMember(d => d.PSQI3, o => o.MapFrom(c => c.PSQI3))
                .ForMember(d => d.PSQI4, o => o.MapFrom(c => c.PSQI4))
                .ForMember(d => d.PSQI5a, o => o.MapFrom(c => c.PSQI5a))
                .ForMember(d => d.PSQI5b, o => o.MapFrom(c => c.PSQI5b))
                .ForMember(d => d.PSQI5c, o => o.MapFrom(c => c.PSQI5c))
                .ForMember(d => d.PSQI5d, o => o.MapFrom(c => c.PSQI5d))
                .ForMember(d => d.PSQI5e, o => o.MapFrom(c => c.PSQI5e))
                .ForMember(d => d.PSQI5f, o => o.MapFrom(c => c.PSQI5f))
                .ForMember(d => d.PSQI5g, o => o.MapFrom(c => c.PSQI5g))
                .ForMember(d => d.PSQI5h, o => o.MapFrom(c => c.PSQI5h))
                .ForMember(d => d.PSQI5i, o => o.MapFrom(c => c.PSQI5i))
                .ForMember(d => d.PSQI5j, o => o.MapFrom(c => c.PSQI5j))
                .ForMember(d => d.PSQI6, o => o.MapFrom(c => c.PSQI6))
                .ForMember(d => d.PSQI7, o => o.MapFrom(c => c.PSQI7))
                .ForMember(d => d.PSQI8, o => o.MapFrom(c => c.PSQI8))
                .ForMember(d => d.PSQI9, o => o.MapFrom(c => c.PSQI9));
            });
                resModel.ForEach(a => ListQuestionnaireData.Add(config.CreateMapper().Map<Questionnaire, QuestionnairesView>(a)));
                return;
            }       
        }
    //Obtener los sleep records del paciente que inició sesión
    public async Task GetSleepRecordsViewAsync(string patient_ID)
    {
        ListSleepRecords = new List<SleepRecordsView>();
        var document = await CrossCloudFirestore.Current
                                   .Instance
                                   .Collection("SleepRecords")
                                   .WhereEqualsTo("Patient_ID", patient_ID)
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

    //Obtener los diarios de sueño del paciente que inició sesión o fue seleccionado por el terapeuta que inició sesión
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
            .ForMember(d => d.CreatedDate_S, o => o.MapFrom(c => c.CreatedDate.ToString("dd/MM/yy")))
            .ForMember(d => d.SleepTime, o => o.MapFrom(c => c.SleepTime))
            .ForMember(d => d.SleepTime_S, o => o.MapFrom(c => c.SleepTime.ToString("HH:mm")))
            .ForMember(d => d.WakeUpTime, o => o.MapFrom(c => c.WakeUpTime))
            .ForMember(d => d.GoToSleepTime, o => o.MapFrom(c => c.GoToSleepTime))
            .ForMember(d => d.WakeUpTime_S, o => o.MapFrom(c => c.WakeUpTime.ToString("HH:mm")))
            .ForMember(d => d.HoursTotal, o => o.MapFrom(c => c.HoursTotal))
            .ForMember(d => d.TimeToFallSleep, o => o.MapFrom(c => c.TimeToFallSleep))
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
    public string TherapistID { get; set; }
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
    //Cuestionario ISI
    public string ISI1a { get; set; }
    public string ISI1b { get; set; }
    public string ISI1c { get; set; }
    public string ISI2 { get; set; }
    public string ISI3 { get; set; }
    public string ISI4 { get; set; }
    public string ISI5 { get; set; }

    //Cuestionario PHQ9
    public string PHQ91 { get; set; }
    public string PHQ92 { get; set; }
    public string PHQ93 { get; set; }
    public string PHQ94 { get; set; }
    public string PHQ95 { get; set; }
    public string PHQ96 { get; set; }
    public string PHQ97 { get; set; }
    public string PHQ98 { get; set; }
    public string PHQ99 { get; set; }

    //Cuestionario PSQI
    public string PSQI1 { get; set; }
    public string PSQI2 { get; set; }
    public string PSQI3 { get; set; }
    public string PSQI4 { get; set; }
    public string PSQI5a { get; set; }
    public string PSQI5b { get; set; }
    public string PSQI5c { get; set; }
    public string PSQI5d { get; set; }
    public string PSQI5e { get; set; }
    public string PSQI5f { get; set; }
    public string PSQI5g { get; set; }
    public string PSQI5h { get; set; }
    public string PSQI5i { get; set; }
    public string PSQI5j { get; set; }
    public string PSQI6 { get; set; }
    public string PSQI7 { get; set; }
    public string PSQI8 { get; set; }
    public string PSQI9 { get; set; }
}
public class SleepRecordsView
{
    public int Key { get; set; }
    public string Patient_ID { get; set; }
    public DateTime DateTimeHour { get; set; }
    public int Kind { get; set; }
}
public class SleepWakeDiariesView
{
    public string Key { get; set; }
    public string SleepWakeDiary_ID { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedDate_S { get; set; }
    public DateTime SleepTime { get; set; }
    public string SleepTime_S { get; set; }
    public DateTime WakeUpTime { get; set; }
    public DateTime GoToSleepTime { get; set; }
    public string WakeUpTime_S { get; set; }
    public double HoursSlept { get; set; }
    public double TimeToFallSleep { get; set; }
    public double HoursTotal { get; set; }
    public double SleepEfficiency { get; set; }
}

public class ListQuestionnaireResultsPatient : ObservableCollection<QuestionnairesView> { }
}
