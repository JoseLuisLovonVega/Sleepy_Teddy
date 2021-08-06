using AutoMapper;
using Plugin.CloudFirestore;
using SleepyTeddy.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepyTeddy.ViewModel
{

    public class GetDataFromSearchLastNames
    {
        //public ObservableCollection<PatientsView> ListPatient {get;set;}
        public ListPatients ListPatient { get; set; }
        public ListPatients ListPatientsTherapist { get; set; }
        public ListTherapists ListTherapist { get; set; }


        public GetDataFromSearchLastNames(){
            ListPatient = new ListPatients();//new ObservableCollection<PatientsView>();//new List<PatientsView>();
            ListPatientsTherapist = new ListPatients();
            ListTherapist = new ListTherapists();
        }      
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        //Obtener los pacientes por su apellido en base al administrador que inició sesión
        public async Task GetPatientsAdministratorViewAsync(string apellido, string administrador_ID)
        {
            ListPatient = new ListPatients(); //new List<PatientsView>();
            string patient_role = "2";

            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", patient_role)
                                       .WhereEqualsTo("Last_Names", apellido)
                                       .WhereEqualsTo("administrator_ID", administrador_ID)
                                       .GetAsync();

            var resModel = document.ToObjects<Patient>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Patient, PatientsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Patient_ID))
                .ForMember(d => d.nombre_completo, o => o.MapFrom(c => c.Names + " " + c.Last_Names));
            });

            resModel.ForEach(a => ListPatient.Add(config.CreateMapper().Map<Patient, PatientsView>(a)));
            return;
        }

        //Obtener los terapeutas por su apellido en base al administrador que inició sesión
        public async Task GetTherapistsAdministratorViewAsync(string apellido, string administrador_ID)
        {
            ListTherapist = new ListTherapists(); //new List<PatientsView>();
            string terapeuta_role = "3";

            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", terapeuta_role)
                                       .WhereEqualsTo("Last_Names", apellido)
                                       .WhereEqualsTo("administrator_ID", administrador_ID)
                                       .GetAsync();

            var resModel = document.ToObjects<Therapist>().ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Therapist, TherapistsView>()
                .ForMember(d => d.Key, o => o.MapFrom(c => c.Therapist_ID))
                .ForMember(d => d.nombre_completo, o => o.MapFrom(c => c.Names + " " + c.Last_Names));
            });

            resModel.ForEach(a => ListTherapist.Add(config.CreateMapper().Map<Therapist, TherapistsView>(a)));
            return;
        }

        //Obtener los pacientes por su apellido en base al terapeuta que inició sesión
        public async Task GetPatientsTherapistViewAsync(string apellido, string therapist_ID)
        {
            ListPatient = new ListPatients(); //new List<PatientsView>();
            string patient_role = "2";

            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", patient_role)
                                       .WhereEqualsTo("Last_Names", apellido)
                                       .WhereEqualsTo("Therapist_ID", therapist_ID)
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
    }

    public class ListPatients : ObservableCollection<PatientsView> { }

    public class ListTherapists : ObservableCollection<TherapistsView> { }

}
