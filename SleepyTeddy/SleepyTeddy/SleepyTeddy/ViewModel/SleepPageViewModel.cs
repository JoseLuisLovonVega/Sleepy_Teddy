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

        //SleepRecordsView sleepRecords1;
        //leepRecordsView sleepRecords2;

        List<SleepRecordsView> listSleepRecords12;
        List<SleepRecordsView> listSleepRecords22;

        int count = 0;
        double amountMinutes = 0;
        double sum = 0;
        int verificacion = 0;
        int verificacion2 = 0;

        int dia;

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

            //sleepRecords1 = new SleepRecordsView();
            //sleepRecords2 = new SleepRecordsView();

            listSleepRecords12 = new List<SleepRecordsView>();
            listSleepRecords22 = new List<SleepRecordsView>();
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
                                    DateTimeHour = data[j].DateTime.AddHours(-5),
                                    Kind = (int)data[j].SleepType
                                });
                            }
                        }
                    }
                }
            }
        }

        /*public async void CreateSleepRecords()
        {
            await objData.GetSleepRecordsViewAsync();
            objData.ListSleepRecords.OrderBy(o => o.DateTimeHour).ToList();

            for (int i = 0; i < _sleepRepository.GetAll().ToList().Count; i++)
            {
                verificacion2 = 0;
                for (int j = 1; i < objData.ListSleepRecords.Count; i++)
                {
                    if(_sleepRepository.GetAll().ToList().ElementAt(i).Id == objData.ListSleepRecords.ElementAt(j).Key)
                    {
                        verificacion2 = 1;
                    }
                }
                if (verificacion2 == 0)
                {
                    if (_sleepRepository.GetAll().ToList().ElementAt(i) != null)
                    {
                        await CrossCloudFirestore.Current
                        .Instance
                        .Collection("SleepRecords")
                        .AddAsync(new SleepRecord
                        {
                            SleepRecord_ID = _sleepRepository.GetAll().ToList().ElementAt(i).Id,
                            Patient_ID = LoginViewModel.Patient_ID,
                            DateTimeHour = _sleepRepository.GetAll().ToList().ElementAt(i).DateTime.AddHours(-5),
                            Kind = (int) _sleepRepository.GetAll().ToList().ElementAt(i).SleepType
                        });
                    }
                }
            }
        }*/

            public async void CreateCompletedSleepWakeDiaries()
        {
            try
            {
                await objData.GetSleepWakeDiariesViewAsync(LoginViewModel.Patient_ID);
                //CreateSleepRecords();
                for (int contador = 0; contador > -6; contador--)
                {
                    verificacion = 0;

                    if (objData.ListSleepWakeDiaries.Count > 0)
                    {
                        foreach (var sleepWakeDiary in objData.ListSleepWakeDiaries)
                        {
                            if (sleepWakeDiary.CreatedDate.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).ToString("dd/MM/yy"))
                            {
                                verificacion = 1;
                            }
                        }
                    }
                    Debug.WriteLine("Evaluando Día " + contador);
                    Debug.WriteLine("La verificación del Día " + contador + "es: " + verificacion);
                    if (verificacion == 0)
                    {
                        sleepWakeDiary = new SleepWakeDiary();
                        amountMinutes = 0;
                        sum = 0;
                        count = 0;

                        listSleepRecords1 = new List<SleepRecordsView>();
                        listSleepRecords2 = new List<SleepRecordsView>();
                        listSleepRecords3 = new List<SleepRecordsView>();
                        listSleepRecordsSleepKinds = new List<SleepRecordsView>();

                        //sleepRecords1 = new SleepRecordsView();
                        //sleepRecords2 = new SleepRecordsView();

                        listSleepRecords12 = new List<SleepRecordsView>();
                        listSleepRecords22 = new List<SleepRecordsView>();

                        dia = contador;

                        await objData.GetSleepRecordsViewAsync();
                        Debug.WriteLine("Se logró obtener todos los sleep records del paciente del día: " + DateTime.Today.AddDays(contador));
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecordsObjData = objData.ListSleepRecords.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("Se logró ordenar descendientemente todos los sleep records del paciente");

                        foreach (var sleepRecord in listSleepRecordsObjData)
                        {
                            if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador - 1).ToString("dd/MM/yy") && sleepRecord.DateTimeHour.Hour > 18)
                            {
                                listSleepRecords1.Add(sleepRecord);
                            }
                        }
                        Debug.WriteLine("Cantidad de sleeprecords1: " + listSleepRecords1.Count);
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecords1 = listSleepRecords1.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("El primer sleeprecord de la lista sleeprecords1: " + listSleepRecords1.First().DateTimeHour);
                        Debug.WriteLine("El último sleeprecord de la lista sleeprecords1: " + listSleepRecords1.Last().DateTimeHour);

                        foreach (var sleepRecord in listSleepRecordsObjData)
                        {
                            if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).ToString("dd/MM/yy") && sleepRecord.DateTimeHour.Hour < 16)
                            {
                                listSleepRecords2.Add(sleepRecord);
                            }
                        }
                        Debug.WriteLine("Cantidad de sleeprecords2: " + listSleepRecords2.Count);
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecords2 = listSleepRecords2.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("El primer sleeprecord de la lista sleeprecords2: " + listSleepRecords2.First().DateTimeHour);
                        Debug.WriteLine("El último sleeprecord de la lista sleeprecords2: " + listSleepRecords2.Last().DateTimeHour);

                        foreach (var sleepRecord in listSleepRecords1)
                        {
                            if (sleepRecord.Kind != 0)
                            {
                                listSleepRecords12.Add(sleepRecord);
                            }
                        }

                        Debug.WriteLine("Cantidad de sleeprecords12: " + listSleepRecords12.Count);
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecords2 = listSleepRecords2.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("El primer sleeprecord de la lista sleeprecords12: " + listSleepRecords12.First().DateTimeHour);
                        Debug.WriteLine("El último sleeprecord de la lista sleeprecords12: " + listSleepRecords12.Last().DateTimeHour);

                        foreach (var sleepRecord in listSleepRecords2)
                        {
                            if (sleepRecord.Kind != 0)
                            {
                                listSleepRecords22.Add(sleepRecord);
                            }
                        }
                        Debug.WriteLine("Cantidad de sleeprecords22: " + listSleepRecords22.Count);
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecords2 = listSleepRecords2.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("El primer sleeprecord de la lista sleeprecords22: " + listSleepRecords22.First().DateTimeHour);
                        Debug.WriteLine("El último sleeprecord de la lista sleeprecords22: " + listSleepRecords22.Last().DateTimeHour);

                        if (listSleepRecords12.Count > 0 && listSleepRecords22.Count > 0)
                        {
                            
                            
                            Debug.WriteLine("Se logró registrar las listas de los sleep records del paciente de 6 pm del dia anterior a 4 pm del dia a evaluar");
                            Debug.WriteLine("Fecha de día a evaluar: " + DateTime.Today.AddDays(contador - 1));

                            /*foreach (var sleepRecord in listSleepRecords1)
                            {
                                listSleepRecords3.Add(sleepRecord);
                            }
                            foreach (var sleepRecord in listSleepRecords2)
                            {
                                listSleepRecords3.Add(sleepRecord);
                            }
                            Debug.WriteLine("Cantidad de sleeprecords3: " + listSleepRecords3.Count);
                            //Ordenar de la más antigua a la más reciente
                            listSleepRecords3 = listSleepRecords3.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("El primer sleeprecord de la lista sleeprecords3: " + listSleepRecords3.First().DateTimeHour);
                            Debug.WriteLine("El último sleeprecord de la lista sleeprecords3: " + listSleepRecords3.Last().DateTimeHour);
                            */

                            Debug.WriteLine("Se logró obtener los sleep records desde las 4 pm del dia anterior hasta antes de las 4 pm del dia a evaluar");

                            //Se define la fecha de creación del diario de sueño como un día antes del día a evaluar
                            sleepWakeDiary.CreatedDate = listSleepRecords12.First().DateTimeHour.AddHours(-5);
                            Debug.WriteLine("Se registro la fecha de creación del diario de sueño-vigilia");

                            //Calcular cuántos minutos le tomó dormirse al paciente
                            sleepWakeDiary.GoToSleepTime = listSleepRecords12.First().DateTimeHour.Subtract(listSleepRecords1.First().DateTimeHour).TotalMinutes;

                            Debug.WriteLine("Se calculó cuántos minutos le tomó dormirse al paciente para el diario de sueño-vigilia");

                            //Calcular las horas en la cama restando las fechas del primer con el último elemento de la lista
                            sleepWakeDiary.HoursTotal = listSleepRecords2.Last().DateTimeHour.Subtract(listSleepRecords1.First().DateTimeHour).TotalHours;
                            Debug.WriteLine("Se calcularon las horas totales del diario de sueño-vigilia");

                            //Calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                            //el sleep record registrado antes de éste es 1 o 2
                            for (int i = 0; i < listSleepRecords2.Count - 1; i++)
                            {
                                if (listSleepRecords2.ElementAt(i).Kind > 0 && listSleepRecords2.ElementAt(i + 1).Kind == 0)
                                {
                                    sleepWakeDiary.WakeUpTime = listSleepRecords2.ElementAt(i + 1).DateTimeHour;
                                }
                            }
                            Debug.WriteLine("Se calculó la hora a la que se despertó del diario de sueño-vigilia");

                            //Se crea la lista con los sleep records sólo kind 1 o 2
                            foreach (var sleepRecord in listSleepRecords12)
                            {
                                listSleepRecordsSleepKinds.Add(sleepRecord);
                            }
                            foreach (var sleepRecord in listSleepRecords22)
                            {
                                listSleepRecordsSleepKinds.Add(sleepRecord);
                            }

                            Debug.WriteLine("Cantidad de sleeprecordsSleepKinds: " + listSleepRecordsSleepKinds.Count);
                            //Ordenar de la más antigua a la más reciente
                            listSleepRecordsSleepKinds = listSleepRecordsSleepKinds.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("El primer sleeprecord de la lista sleeprecordsSleepKinds: " + listSleepRecordsSleepKinds.First().DateTimeHour);
                            Debug.WriteLine("El último sleeprecord de la lista sleeprecordsSleepKinds: " + listSleepRecordsSleepKinds.Last().DateTimeHour);

                            Debug.WriteLine("Se creó la lista con los sleep records kind 1 o 2 para el diario de sueño-vigilia");

                            count = 0;
                            //Verificar si la lista creada no está vacía
                            if (listSleepRecordsSleepKinds.Count > 0)
                            {
                                //Calcular a qué hora se durmió el paciente el día anterior al día a evaluar
                                foreach (var SleepRecord in listSleepRecordsSleepKinds)
                                {
                                    if (count == 0)
                                    {
                                        sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                                        count = 1;
                                    }
                                }
                                Debug.WriteLine("Se calculó la hora a la que se durmió del diario de sueño-vigilia");

                                //Se calcula los minutos que el paciente estuvo dormido
                                for (int i = 1; i < listSleepRecordsSleepKinds.Count; i++)
                                {
                                    amountMinutes = listSleepRecordsSleepKinds.ElementAt(i).DateTimeHour.Subtract(listSleepRecordsSleepKinds.ElementAt(i - 1).DateTimeHour).TotalMinutes;
                                    sum = sum + amountMinutes;
                                }

                                //Los minutos se pasan a horas
                                sum = sum / 60;

                                //Se calculan las horas dormidas
                                sleepWakeDiary.HoursSlept = sum;
                                sleepWakeDiary.HoursSlept = Math.Round(sleepWakeDiary.HoursSlept, 2);
                                Debug.WriteLine("Se calculó las horas horas dormidas del diario de sueño-vigilia");

                                //Y con ello se calcula la eficiencia del sueño del diario de sueño-vigilia
                                sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
                                sleepWakeDiary.SleepEfficiency = Math.Round(sleepWakeDiary.SleepEfficiency, 2);
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
                                              SleepTime = sleepWakeDiary.SleepTime.AddHours(-5),
                                              WakeUpTime = sleepWakeDiary.WakeUpTime.AddHours(-5),
                                              HoursTotal = sleepWakeDiary.HoursTotal,
                                              HoursSlept = sleepWakeDiary.HoursSlept,
                                              SleepEfficiency = sleepWakeDiary.SleepEfficiency
                                          });
                                Debug.WriteLine("Se registró el diario de sueño-vigilia del día a evaluar: Día: " + contador);
                            }
                            else
                            {
                                Debug.WriteLine("No existen sleep records de kind 1 o 2 en los días en cuestión. : Día: " + contador);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No existe data de sleep records de los días en cuestión.: Día: " + contador);
                        }
                    }
                    if (contador == -5)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.", new TimeSpan(3));
                        Debug.WriteLine("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.");
                    }
                }
                //await Application.Current.MainPage.DisplayAlert("Sincronización Exitosa", "Registros de diarios de sueño-vigilia completados", "OK");
                //Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.", new TimeSpan(3));
            }
            catch (Exception e)
            {
                
                Debug.WriteLine(e.Message);
                Debug.WriteLine("ERROR al completar los diarios de sueño-vigilia. Día: " + dia);
                Acr.UserDialogs.UserDialogs.Instance.Toast("ERROR al completar los diarios de sueño-vigilia.", new TimeSpan(3));
                return;
            }
        }
    }
}
