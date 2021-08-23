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

        List<SleepRecordsView> listSleepRecords;
        List<SleepRecordsView> listSleepRecords2;
        List<SleepRecordsView> listSleepRecordsDateFilter;
        int unicaVerficiacion = 0;
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
            sleepWakeDiary = new SleepWakeDiary();
            listSleepRecords = new List<SleepRecordsView>();
            listSleepRecords2 = new List<SleepRecordsView>();
            listSleepRecordsDateFilter = new List<SleepRecordsView>();
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
                    unicaVerficiacion = 0;
                    await objData.GetSleepRecordsDateFilterViewAsync(contador);
                    //Chequear si existen sleep records del día a evaluar
                    if (objData.ListSleepRecordsDateFilter.Count > 0)
                    {
                        listSleepRecordsDateFilter = objData.ListSleepRecordsDateFilter.OrderByDescending(o => o.DateTimeHour).ToList();
                        //Chequear si existen sleep records en la mañana del día a evaluar y
                        // si tiene significa que se durmió el dia anterior por tanto hay data para
                        // crear un diario de sueño-vigilia
                        foreach (var sleepRecord in listSleepRecordsDateFilter)
                        {
                            if (sleepRecord.DateTimeHour.Hour < 16 && sleepRecord.Kind > 0 && unicaVerficiacion == 0)
                            {
                                //Solo verificar una vez si hay data para registrar el atributo de 
                                unicaVerficiacion = 1;
                                //Por tanto se define la fecha de creación del diario de sueño como un día antes del día de
                                //de creación del sleep record verificado que se creó en la mañana
                                sleepWakeDiary.CreatedDate = sleepRecord.DateTimeHour.Date.AddDays(-1);
                            }
                        }
                        //Crear una lista de sleep records con cada sleep record del rango 4 pm del día anterior
                        //hasta 4 pm del día siguiente y tienen que ser sleeprecords tipo 1 y 2
                        await objData.GetSleepRecordsViewAsync();
                        listSleepRecords = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();
                        foreach (var sleepRecord2 in listSleepRecords)
                        {
                            if ((sleepRecord2.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).ToString("dd/MM/yy") && sleepRecord2.DateTimeHour.Hour<16) 
                                || (sleepRecord2.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador-1).ToString("dd/MM/yy") && sleepRecord2.DateTimeHour.Hour > 16)
                                && sleepRecord2.Kind > 0)
                                {
                                            listSleepRecords2.Add(sleepRecord2);
                                }
                        }
                        //Verificar si la lista creada no es nula
                        if (listSleepRecords2.Count > 0)
                        {
                        listSleepRecords2= listSleepRecords2.OrderByDescending(o => o.DateTimeHour).ToList();
                        //Calcular las horas dormidas restando las fechas del primer con el último elemento de la lista
                        sleepWakeDiary.HoursTotal = (listSleepRecords2.Last().DateTimeHour - listSleepRecords2.First().DateTimeHour).TotalHours;
                        foreach (var SleepRecord in listSleepRecords2)
                        {
                            if (SleepRecord.Kind == 2 && count == 0)
                                {
                                sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                                count = 1;
                                }
                            }
                        count = 0;
                        //Ordenar la lista descendentemente osea de fecha más actual a más antigua
                        //listSleepRecords2 = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();

                        //Para así calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                        //el sleep record registrado antes de este es 1 o 2
                        for (int i = 0; i < listSleepRecordsDateFilter.Count; i++)
                        {
                            if (listSleepRecordsDateFilter.ElementAt(i).Kind == 0 && listSleepRecordsDateFilter.ElementAt(i - 1).Kind > 0 && count == 0)
                            {
                                sleepWakeDiary.WakeUpTime = listSleepRecordsDateFilter.ElementAt(i).DateTimeHour;
                                count = 1;
                            }
                        }
                        for (int i = 0; i < listSleepRecords2.Count; i++)
                        {
                            amountMinutes = listSleepRecords2.ElementAt(i + 1).DateTimeHour.Subtract(listSleepRecords2.ElementAt(i).DateTimeHour).TotalMinutes;
                            sum += amountMinutes;
                        }
                        //Los minutos se pasan a horas
                        sum = sum / 60;
                        //Se calcula las horas dormidas
                        sleepWakeDiary.HoursSlept = sum;
                        //Y con ell la eficiencia del sueño del diario de sueño-vigilia
                        sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
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
                                  count = 0;
                        }
                        else
                        {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No existe data para registrar los diarios de sueño-vigilia", new TimeSpan(3));
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
