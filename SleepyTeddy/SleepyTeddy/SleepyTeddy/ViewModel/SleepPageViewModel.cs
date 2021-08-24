using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SleepyTeddy.Data.Interfaces;
using SleepyTeddy.Models;
using Xamarin.Forms;
using Plugin.CloudFirestore;
using SleepyTeddy.Resources;
using SleepyTeddy.ViewModel;

namespace SleepyTeddy.ViewModel
{
    public class SleepPageViewModel : INotifyPropertyChanged
    {
        public static IEnumerable<Sleep> SleepInfo = new List<Sleep>();
        public DateTime StartDate { get; }
        public DateTime SelectedDate;

        SleepWakeDiary sleepWakeDiary;
        GetDataFromLoginUser objData { get; set; }

        List<SleepRecordsView> listSleepRecordsDateFilter;
        List<SleepRecordsView> listSleepRecordsDaysToEvaluate;
        List<SleepRecordsView> listSleepRecordsSleepKindsDatesToEvaluate;
        

        int unicaVerificiacion = 0;
        int count = 0;
        double amountMinutes = 0;
        double sum = 0;

        private ISleepRepository _sleepRepository;

        public event PropertyChangedEventHandler PropertyChanged;

        public SleepPageViewModel(ISleepRepository sleepRepository)
        {
            _sleepRepository = sleepRepository;
            StartDate = DateTime.Today;
            //SelectedDate = StartDate;

            objData = new GetDataFromLoginUser();
            sleepWakeDiary = new SleepWakeDiary();

            listSleepRecordsDateFilter = new List<SleepRecordsView>();
            listSleepRecordsDaysToEvaluate = new List<SleepRecordsView>();
            listSleepRecordsSleepKindsDatesToEvaluate = new List<SleepRecordsView>(); 
        }

        private List<Sleep> GetCurrentSleep()
        {
            return SleepInfo.Where(s => s.DateTime.Year == SelectedDate.Year &&
            s.DateTime.Month == SelectedDate.Month &&
            s.DateTime > SelectedDate.AddHours(-4) &&
            s.DateTime < SelectedDate.AddHours(12)).
            OrderBy(x => x.DateTime).ToList();
        }

        public async void CreateSleepRecords()
        {
            SleepInfo = _sleepRepository.GetAll();

            for (int k = 0; k > -7; k--)
            {

                SelectedDate = StartDate.AddDays(k);
                List<Sleep> sleepData = GetCurrentSleep();

                if (sleepData.Count() > 0)
                {

                    //For each hour
                    for (int i = 20; i < 36; i++)
                    {
                        int hour = i;
                        if (i >= 24) hour -= 24;

                        //Get sleep data for that hour
                        List<Sleep> data = sleepData.Where(x => x.DateTime.Hour == hour).ToList();
                        for (int j = 0; j < 60; j++)
                        {
                            if (data.ElementAtOrDefault(j) != null)
                            {
                                await CrossCloudFirestore.Current
                                .Instance
                                .Collection("SleepRecords")
                                .AddAsync(new SleepRecord
                                {
                                    SleepRecord_ID = data[j].Id,
                                    Patient_ID= LoginViewModel.Patient_ID,
                                    DateTimeHour = data[j].DateTime,
                                    Kind = (int)data[j].SleepType
                                });
                            }
                        }
                    }
                }
            }
            Acr.UserDialogs.UserDialogs.Instance.Toast("Registros de información del sueño completados.", new TimeSpan(4));
        }

        public async void CreateCompletedSleepWakeDiaries()
        {
            try
            { 
                for (int contador = 0; contador > -7; contador--)
                {
                    unicaVerificiacion = 0;
                    count = 0;
                    await objData.GetSleepRecordsDateFilterViewAsync(contador);
                    Debug.WriteLine("Se logró obtener los sleep records desde las 0 am hasta antes de las 4 pm del dia a evaluar");
                    //Chequear si existen sleep records del día a evaluar
                    if (objData.ListSleepRecordsDateFilter.Count > 0)
                    {
                        listSleepRecordsDateFilter = objData.ListSleepRecordsDateFilter.OrderByDescending(o => o.DateTimeHour).ToList();
                        //Chequear si existen sleep records en la mañana del día a evaluar y
                        // si tiene significa que se durmió el dia anterior por tanto hay data para
                        // crear un diario de sueño-vigilia
                        foreach (var sleepRecord in listSleepRecordsDateFilter)
                        {
                            if (sleepRecord.Kind > 0 && unicaVerificiacion == 0)
                            {
                                //Por tanto se define la fecha de creación del diario de sueño como un día antes del día de
                                //de creación del sleep record verificado que se creó en la mañana
                                sleepWakeDiary.CreatedDate = sleepRecord.DateTimeHour.Date.AddDays(-1);

                                //Solo verificar una vez si hay data para registrar el atributo de 
                                unicaVerificiacion = 1; 
                            }
                        }

                        //Crear una lista de sleep records con cada sleep record del rango 4 pm del día anterior
                        //hasta 4 pm del día siguiente y tienen que ser sleeprecords tipo 1 y 2
                        await objData.GetSleepRecordsViewAsync(contador);
                        Debug.WriteLine("Se logró obtener los sleep records desde las 4 pm del dia anterior hasta antes de las 4 pm del dia a evaluar");

                        listSleepRecordsDaysToEvaluate = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();
                        foreach (var sleepRecord in listSleepRecordsDaysToEvaluate)
                        {
                            if (sleepRecord.Kind > 0)
                            {
                                listSleepRecordsSleepKindsDatesToEvaluate.Add(sleepRecord);
                            }
                        }
                        Debug.WriteLine("Se creó la lista con el rango de los días para el diario de sueño-vigilia");
                        //Verificar si la lista creada no es nula
                        if (listSleepRecordsSleepKindsDatesToEvaluate.Count > 0)
                        {
                        //listSleepRecordsSleepKindsDateEvaluate = listSleepRecordsSleepKindsDateEvaluate.OrderByDescending(o => o.DateTimeHour).ToList();
                        //Calcular las horas dormidas restando las fechas del primer con el último elemento de la lista
                        sleepWakeDiary.HoursTotal = (listSleepRecordsSleepKindsDatesToEvaluate.Last().DateTimeHour - listSleepRecordsSleepKindsDatesToEvaluate.First().DateTimeHour).TotalHours;
                        Debug.WriteLine("Se calcularon las horas totales del diario de sueño-vigilia");
                        //Calcular a qué hora se durmió el paciente el día anterior al día a evaluar
                        foreach (var SleepRecord in listSleepRecordsSleepKindsDatesToEvaluate)
                        {
                            if (SleepRecord.Kind == 2 && count == 0)
                                {
                                sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                                count = 1;
                                }
                        }
                        Debug.WriteLine("Se calculó la hora a la que se durmió del diario de sueño-vigilia");
                        count = 0;
                        //Calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                        //el sleep record registrado antes de éste es 1 o 2
                        for (int i = 0; i < listSleepRecordsSleepKindsDatesToEvaluate.Count; i++)
                        {
                            if (listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i).DateTimeHour.Date== DateTime.Today.AddDays(contador).Date && listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i).Kind > 0 && listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i+1).Kind == 0 && count == 0)
                            {
                                sleepWakeDiary.WakeUpTime = listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i).DateTimeHour;
                                count = 1;
                            }
                        }
                        Debug.WriteLine("Se calculó la hora a la que se despertó del diario de sueño-vigilia");
                        for (int i = 0; i < listSleepRecordsSleepKindsDatesToEvaluate.Count; i++)
                        {
                            amountMinutes = listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i + 1).DateTimeHour.Subtract(listSleepRecordsSleepKindsDatesToEvaluate.ElementAt(i).DateTimeHour).TotalMinutes;
                            sum += amountMinutes;
                        }
                        //Los minutos se pasan a horas
                        sum = sum / 60;
                        //Se calcula las horas dormidas
                        sleepWakeDiary.HoursSlept = sum;
                        Debug.WriteLine("Se calculó las horas horas dormidas del diario de sueño-vigilia");
                        //Y con ell la eficiencia del sueño del diario de sueño-vigilia
                        sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
                        Debug.WriteLine("Se calculó la eficiencia del sueño del diario de sueño-vigilia");
                        //Se crea el diario de sueño-vigilia
                        await CrossCloudFirestore.Current
                                  .Instance
                                  .Collection("SleepWakeDiaries")
                                  .AddAsync(new SleepWakeDiary
                                  {
                                  SleepWakeDiary_ID = Guid.NewGuid().ToString().Replace("-", ""),
                                  Patient_ID = LoginViewModel.Patient_ID,
                                  CreatedDate = sleepWakeDiary.CreatedDate,
                                  SleepTime = sleepWakeDiary.SleepTime,
                                  WakeUpTime = sleepWakeDiary.WakeUpTime,
                                  HoursTotal = sleepWakeDiary.HoursTotal,
                                  HoursSlept = sleepWakeDiary.HoursSlept,
                                  SleepEfficiency = sleepWakeDiary.SleepEfficiency
                                  });
                        Debug.WriteLine("Se registró el diario de sueño-vigilia del día a evaluar: Día: " + contador);
                        }
                        else
                        {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No existe data del día en cuestión", new TimeSpan(3));
                        }
                    }
                }
                await Application.Current.MainPage.DisplayAlert("Sincronización Exitosa", "Registros de diarios de sueño-vigilia completados", "OK");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("ERROR al completar los diarios de sueño-vigilia");
                Acr.UserDialogs.UserDialogs.Instance.Toast("ERROR al completar los diarios de sueño-vigilia.", new TimeSpan(3));
                return;
            }
        }
    }
}
