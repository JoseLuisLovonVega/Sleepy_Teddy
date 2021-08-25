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

        List<SleepRecordsView> listSleepRecordsObjData;
        List<SleepRecordsView> listSleepRecords1;
        List<SleepRecordsView> listSleepRecords2;
        List<SleepRecordsView> listSleepRecords3;
        List<SleepRecordsView> listSleepRecordsSleepKinds;

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

            listSleepRecordsObjData = new List<SleepRecordsView>();
            listSleepRecords1 = new List<SleepRecordsView>();
            listSleepRecords2 = new List<SleepRecordsView>();
            listSleepRecords3 = new List<SleepRecordsView>();
            listSleepRecordsSleepKinds = new List<SleepRecordsView>();
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
                                    Patient_ID = LoginViewModel.Patient_ID,
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
                //CreateSleepRecords();
                for (int contador = 0; contador > -7; contador--)
                {
                    amountMinutes = 0;
                    sum = 0;
                    count = 0;
                    await objData.GetSleepRecordsViewAsync();
                    Debug.WriteLine("Se logró obtener todos los sleep records del paciente");
                    listSleepRecordsObjData = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();
                    Debug.WriteLine("Se logró ordenar descendientemente todos los sleep records del paciente");
                    if (contador != -6)
                    {
                        foreach (var sleepRecord in listSleepRecordsObjData)
                        {
                            if (sleepRecord.DateTimeHour.Date == DateTime.Today.AddDays(contador - 1).Date && sleepRecord.DateTimeHour.Hour > 16)
                            {
                                listSleepRecords1.Add(sleepRecord);
                            }
                            else if (sleepRecord.DateTimeHour.Date == DateTime.Today.AddDays(contador).Date && sleepRecord.DateTimeHour.Hour < 16)
                            {
                                listSleepRecords2.Add(sleepRecord);
                            }
                        }
                        Debug.WriteLine("Se logró registrar las listas de los sleep records del paciente de 4 pm del dia anterior a 4 pm del dia a evaluar");
                        //Verificar si existen sleep records del día anterior y el día a evaluar
                        //if (listSleepRecords1.Count > 0 && listSleepRecords2.Count > 0)
                        //{
                        foreach (var sleepRecord in listSleepRecords1)
                        {
                            listSleepRecords3.Add(sleepRecord);
                        }
                        foreach (var sleepRecord in listSleepRecords2)
                        {
                            listSleepRecords3.Add(sleepRecord);
                        }
                        //listSleepRecords3 = listSleepRecords3.OrderByDescending(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("Se logró obtener los sleep records desde las 4 pm del dia anterior hasta antes de las 4 pm del dia a evaluar");

                        //Se define la fecha de creación del diario de sueño como un día antes del día a evaluar
                        sleepWakeDiary.CreatedDate = DateTime.Today.AddDays(contador - 1);

                        //Calcular las horas en la cama restando las fechas del primer con el último elemento de la lista
                        sleepWakeDiary.HoursTotal = (listSleepRecords3.Last().DateTimeHour - listSleepRecords3.First().DateTimeHour).TotalHours;
                        Debug.WriteLine("Se calcularon las horas totales del diario de sueño-vigilia");

                        //Calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                        //el sleep record registrado antes de éste es 1 o 2
                        listSleepRecords2 = listSleepRecords2.OrderByDescending(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("Se ordenó la lista de listSleepRecords2");
                        for (int i = 0; i < listSleepRecords2.Count; i++)
                        {
                            if (listSleepRecords2.ElementAt(i).Kind > 0 && listSleepRecords2.ElementAt(i + 1).Kind == 0 && count == 0)
                            {
                                sleepWakeDiary.WakeUpTime = listSleepRecords2.ElementAt(i + 1).DateTimeHour;
                                count = 1;
                            }
                        }
                        Debug.WriteLine("Se calculó la hora a la que se despertó del diario de sueño-vigilia");
                        count = 0;

                        //Se crea la lista con los sleep records sólo kind 1 o 2
                        foreach (var sleepRecord in listSleepRecords3)
                        {
                            if (sleepRecord.Kind > 0)
                            {
                                listSleepRecordsSleepKinds.Add(sleepRecord);
                            }
                        }
                    }
                    Debug.WriteLine("Se creó la lista con los sleep records kind 1 o 2 para el diario de sueño-vigilia");
                    //Verificar si la lista creada no está vacía
                    if (listSleepRecordsSleepKinds.Count > 0)
                    {
                        //Calcular a qué hora se durmió el paciente el día anterior al día a evaluar
                        foreach (var SleepRecord in listSleepRecordsSleepKinds)
                        {
                            if (SleepRecord.Kind == 2 && count == 0)
                            {
                                sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                                count = 1;
                            }
                        }
                        Debug.WriteLine("Se calculó la hora a la que se durmió del diario de sueño-vigilia");
                        count = 0;

                        //Se calcula los minutos que el paciente estuvo dormido
                        for (int i = 0; i <= listSleepRecordsSleepKinds.Count; i++)
                        {
                            if (i != 0)
                            {
                                amountMinutes = listSleepRecordsSleepKinds.ElementAt(i).DateTimeHour.Subtract(listSleepRecordsSleepKinds.ElementAt(i - 1).DateTimeHour).TotalMinutes;
                                sum = sum + amountMinutes;
                            }
                        }
                        //Los minutos se pasan a horas
                        sum = sum / 60;
                        //Se calcula las horas dormidas
                        sleepWakeDiary.HoursSlept = sum;
                        Debug.WriteLine("Se calculó las horas horas dormidas del diario de sueño-vigilia");
                        //Y con ello se calcula la eficiencia del sueño del diario de sueño-vigilia
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
                        Debug.WriteLine("No existe data de sueño de los días en cuestión");
                    }
                    //}
                    //else
                    //{
                    //    Debug.WriteLine("No existe data de sleep records de los días en cuestión");
                    //}
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
