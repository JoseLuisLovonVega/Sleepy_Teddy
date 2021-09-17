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

        List<SleepRecordsView> listSleepRecordsLocalDB;
        List<SleepRecordsView> listSleepRecords1;
        List<SleepRecordsView> listSleepRecords2;
        List<SleepRecordsView> listSleepRecords3;

        SleepRecordsView sleepRecord;

        List<SleepRecordsView> listSleepRecords12;
        List<SleepRecordsView> listSleepRecords22;

        int count = 0;
        double amountMinutes = 0;
        double sum = 0;
        int verificacion = 0;
        int verificacion2 = 0;

        int dia = 0;

        private ISleepRepository _sleepRepository;

        public event PropertyChangedEventHandler PropertyChanged;

        public SleepPageViewModel(ISleepRepository sleepRepository)
        {
            _sleepRepository = sleepRepository;
            StartDate = DateTime.Today;
            Debug.WriteLine("La fecha de hoy es: " + StartDate);
            //SelectedDate = StartDate;

            objData = new GetDataFromLoginUser();
            sleepWakeDiary = new SleepWakeDiary();

            listSleepRecordsLocalDB = new List<SleepRecordsView>();
            listSleepRecords1 = new List<SleepRecordsView>();
            listSleepRecords2 = new List<SleepRecordsView>();
            listSleepRecords3 = new List<SleepRecordsView>();

            listSleepRecords12 = new List<SleepRecordsView>();
            listSleepRecords22 = new List<SleepRecordsView>();

            sleepRecord = new SleepRecordsView();
        }

        private List<Sleep> GetCurrentSleep()
        {
            return SleepInfo.Where(s => s.DateTime.Year == SelectedDate.Year &&
            s.DateTime.Month == SelectedDate.Month &&
            s.DateTime > SelectedDate.AddHours(-4) &&
            s.DateTime < SelectedDate.AddHours(14)).
            OrderBy(x => x.DateTime).ToList();
        }

        public void CreateSleepRecords()
        {
            Debug.WriteLine("Se inicia el proceso para agregar los sleeprecords a la lista designada");
            SleepInfo = _sleepRepository.GetAll();
            listSleepRecordsLocalDB.Clear();
            //listSleepRecordsLocalDB = new List<SleepRecordsView>();
            for (int k = 0; k > -7; k--)
            {
                SelectedDate = StartDate.AddDays(k);
                List<Sleep> sleepData = GetCurrentSleep();

                if (sleepData.Count() > 0)
                {
                    Debug.WriteLine("Se analiza el Día: " + StartDate.AddDays(k));
                    //For each hour
                    for (int i = 20; i < 38; i++)
                    {
                        int hour = i;
                        if (i >= 24) hour -= 24;

                        //Get sleep data for that hour
                        List<Sleep> data = sleepData.Where(x => x.DateTime.Hour == hour).ToList();
                        for (int j = 0; j < 60; j++)
                        {
                            if (data.ElementAt(j) != null) //ElementAtOrDefault
                            {
                                sleepRecord = new SleepRecordsView();
                                sleepRecord.Key = data[j].Id;
                                sleepRecord.Patient_ID = LoginViewModel.Patient_ID;
                                sleepRecord.DateTimeHour = data[j].DateTime;
                                sleepRecord.Kind = (int)data[j].SleepType;
                                listSleepRecordsLocalDB.Add(sleepRecord);
                            }
                        }
                    }
                }
            }
            Debug.WriteLine("Cantidad de sleep records agregados a la lista: " + listSleepRecordsLocalDB.Count);
            Debug.WriteLine("Se completó la carga de sleep records a la lista asignada.");
        }

        /*public void CreateSleepRecords2()
        {
            Debug.WriteLine("Se inicia el proceso para agregar los sleeprecords a la lista designada");
            SleepInfo2 = _sleepRepository.GetAll().ToList();

            if (SleepInfo2.Count() > 0)
            {
                for (int i = 0; i < SleepInfo2.Count; i++)
                {
                    if (SleepInfo2.ElementAt(i) != null)
                    {
                        sleepRecord = new SleepRecordsView();
                        sleepRecord.Key = SleepInfo2.ElementAt(i).Id;
                        sleepRecord.Patient_ID = LoginViewModel.Patient_ID;
                        sleepRecord.DateTimeHour = SleepInfo2.ElementAt(i).DateTime;
                        sleepRecord.Kind = (int)SleepInfo2.ElementAt(i).SleepType;
                        listSleepRecordsLocalDB.Add(sleepRecord);
                    }
                }
            }
        }*/

        /*public async Task TransferToFirebaseSleepRecords()
        {
            Debug.WriteLine("Iniciando la subida de elementos de la lista de sleeprecords al Firebase");
            foreach (var sleepRecord in listSleepRecordsLocalDB)
            {
                await CrossCloudFirestore.Current
                                     .Instance
                                     .Collection("SleepRecords")
                                     .AddAsync(new SleepRecord
                                     {
                                         SleepRecord_ID = sleepRecord.Key,
                                         Patient_ID = sleepRecord.Patient_ID,
                                         DateTimeHour = sleepRecord.DateTimeHour.AddHours(-5),
                                         Kind = sleepRecord.Kind
                                     });
            }
            Debug.WriteLine("Se completó la subida de sleeprecords al Firebase");
            Debug.WriteLine("Iniciando el registro de diarios de sueño-vigilia");
            await CreateCompletedSleepWakeDiaries();
        }*/

        public async Task CreateSleepWakeDiaries()
        {
            verificacion2 = 0;
            try
            {
                await objData.GetSleepWakeDiariesViewAsync(LoginViewModel.Patient_ID);
                for (int contador = 0; contador > -7; contador--)
                {
                    verificacion = 0;

                    //Verificar que no se registren diarios de sueño-vigilia con la misma fecha
                    if (objData.ListSleepWakeDiaries.Count > 0)
                    {
                        foreach (var SWDiary in objData.ListSleepWakeDiaries)
                        {
                            if (DateTime.Now.AddDays(contador - 1).Date.ToString("dd/MM/yy") == SWDiary.CreatedDate_S)
                            {
                                verificacion = 1;
                            }
                        }
                    }
                    Debug.WriteLine("Evaluando Día " + (contador - 1));
                    Debug.WriteLine("La verificación del Día " + (contador - 1) + " es: " + verificacion);
                    if (verificacion == 0)
                    {
                        sleepWakeDiary = new SleepWakeDiary();
                        sleepWakeDiary.SleepTime = DateTime.MinValue;
                        sleepWakeDiary.CreatedDate = DateTime.MinValue;
                        sleepWakeDiary.GoToSleepTime = DateTime.MinValue;
                        sleepWakeDiary.WakeUpTime = DateTime.MinValue;
                        sleepWakeDiary.TimeToFallSleep = 0;
                        sleepWakeDiary.HoursSlept = 0;
                        sleepWakeDiary.HoursTotal = 0;
                        sleepWakeDiary.SleepEfficiency = 0;

                        amountMinutes = 0;
                        sum = 0;
                        count = 0;

                        listSleepRecords1.Clear();
                        //listSleepRecords1 = new List<SleepRecordsView>();

                        listSleepRecords2.Clear();
                        //listSleepRecords2 = new List<SleepRecordsView>();

                        listSleepRecords3.Clear();
                        //listSleepRecords3 = new List<SleepRecordsView>();

                        listSleepRecords12.Clear();
                        //listSleepRecords12 = new List<SleepRecordsView>();

                        listSleepRecords22.Clear();
                        //listSleepRecords22 = new List<SleepRecordsView>();

                        dia = contador - 1;

                        //await objData.GetSleepRecordsViewAsync();
                        Debug.WriteLine("Se logró obtener todos los sleep records del paciente del día: " + DateTime.Now.AddDays(contador - 1).ToString("dd/MM/yy"));
                        //Ordenar de la más antigua a la más reciente
                        if (listSleepRecordsLocalDB.Count > 0)
                        {
                            listSleepRecordsLocalDB = listSleepRecordsLocalDB.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("Se logró ordenar ascendentemente todos los sleep records del paciente.");
                        }

                        foreach (var sleepRecord in listSleepRecordsLocalDB)
                        {
                            if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).AddHours(-4).ToString("dd/MM/yy") && sleepRecord.DateTimeHour > DateTime.Today.AddDays(contador).AddHours(-4))
                            {
                                listSleepRecords1.Add(sleepRecord);
                            }
                            else if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).AddHours(14).ToString("dd/MM/yy") && sleepRecord.DateTimeHour < DateTime.Today.AddDays(contador).AddHours(14))
                            {
                                listSleepRecords2.Add(sleepRecord);
                            }
                        }

                        if ((listSleepRecords1.Count > 0 && listSleepRecords2.Count > 0) || listSleepRecords2.Count > 0)
                        {
                            //Ordenar de la más antigua a la más reciente
                            if (listSleepRecords1.Count > 0)
                            {
                                Debug.WriteLine("Cantidad de sleeprecords1: " + listSleepRecords1.Count);
                                listSleepRecords1 = listSleepRecords1.OrderBy(o => o.DateTimeHour).ToList();
                                Debug.WriteLine("El primer sleeprecord de la lista sleeprecords1: " + listSleepRecords1.First().DateTimeHour);
                                Debug.WriteLine("El último sleeprecord de la lista sleeprecords1: " + listSleepRecords1.Last().DateTimeHour);
                            }
                            //Ordenar de la más antigua a la más reciente
                            Debug.WriteLine("Cantidad de sleeprecords2: " + listSleepRecords2.Count);
                            listSleepRecords2 = listSleepRecords2.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("El primer sleeprecord de la lista sleeprecords2: " + listSleepRecords2.First().DateTimeHour);
                            Debug.WriteLine("El último sleeprecord de la lista sleeprecords2: " + listSleepRecords2.Last().DateTimeHour);

                            foreach (var sleepRecord in listSleepRecords1)
                            {
                                if (sleepRecord.Kind == 1 || sleepRecord.Kind == 2)
                                {
                                    listSleepRecords12.Add(sleepRecord);
                                }
                            }
                            foreach (var sleepRecord in listSleepRecords2)
                            {
                                if (sleepRecord.Kind == 1 || sleepRecord.Kind == 2)
                                {
                                    listSleepRecords22.Add(sleepRecord);
                                }
                            }

                            if ((listSleepRecords12.Count > 0 && listSleepRecords22.Count > 0) || listSleepRecords22.Count > 0)
                            {
                                //Ordenar de la más antigua a la más reciente
                                if (listSleepRecords12.Count > 0)
                                {
                                    listSleepRecords12 = listSleepRecords12.OrderBy(o => o.DateTimeHour).ToList();
                                    Debug.WriteLine("Cantidad de sleeprecords12: " + listSleepRecords12.Count);
                                    Debug.WriteLine("El primer sleeprecord de la lista sleeprecords12: " + listSleepRecords12.First().DateTimeHour);
                                    Debug.WriteLine("El último sleeprecord de la lista sleeprecords12: " + listSleepRecords12.Last().DateTimeHour);
                                }
                                //Ordenar de la más antigua a la más reciente
                                listSleepRecords22 = listSleepRecords22.OrderBy(o => o.DateTimeHour).ToList();
                                Debug.WriteLine("Cantidad de sleeprecords22: " + listSleepRecords22.Count);
                                Debug.WriteLine("El primer sleeprecord de la lista sleeprecords22: " + listSleepRecords22.First().DateTimeHour);
                                Debug.WriteLine("El último sleeprecord de la lista sleeprecords22: " + listSleepRecords22.Last().DateTimeHour);

                                //Se define la fecha de creación del diario de sueño como un día antes del día a evaluar
                                sleepWakeDiary.CreatedDate = DateTime.Today.AddDays(contador - 1);
                                Debug.WriteLine("Se registro la fecha de creación del diario de sueño-vigilia");

                                //Calcular a qué hora se fue a su cama el paciente el día anterior al día a evaluar
                                count = 0;
                                if (listSleepRecords1.Count > 0)
                                {
                                    for (int i = 2; i < listSleepRecords1.Count; i++)
                                    {
                                        if (listSleepRecords1.ElementAt(i - 2).Kind == 0 && listSleepRecords1.ElementAt(i - 1).Kind == 0 && listSleepRecords1.ElementAt(i).Kind == 0 && count == 0)
                                        {
                                            sleepWakeDiary.GoToSleepTime = listSleepRecords1.ElementAt(i - 1).DateTimeHour;
                                            count = 1;
                                        }
                                    }
                                }
                                if (sleepWakeDiary.GoToSleepTime == DateTime.MinValue)
                                {
                                    for (int i = 2; i < listSleepRecords2.Count; i++)
                                    {
                                        if (listSleepRecords1.ElementAt(i - 2).Kind == 0 && listSleepRecords2.ElementAt(i - 1).Kind == 0 && listSleepRecords2.ElementAt(i).Kind == 0 && count == 0)
                                        {
                                            sleepWakeDiary.GoToSleepTime = listSleepRecords2.ElementAt(i - 1).DateTimeHour;
                                            count = 1;
                                        }
                                    }
                                }
                                Debug.WriteLine("GoToSleepTime: " + sleepWakeDiary.GoToSleepTime);
                                count = 0;
                                //Calcular a qué hora se durmió el paciente el día anterior al día a evaluar
                                if (listSleepRecords1.Count > 0)
                                {
                                    for (int i = 2; i < listSleepRecords1.Count; i++)
                                    {
                                        if (listSleepRecords1.ElementAt(i - 2).Kind == 0 && listSleepRecords1.ElementAt(i - 1).Kind > 0 && listSleepRecords1.ElementAt(i).Kind > 0 && count == 0)
                                        {
                                            sleepWakeDiary.SleepTime = listSleepRecords1.ElementAt(i - 1).DateTimeHour;
                                            count = 1;
                                        }
                                    }
                                }
                                    if (sleepWakeDiary.SleepTime == DateTime.MinValue)
                                    {
                                        for (int i = 1; i < listSleepRecords2.Count; i++)
                                        {
                                            if (listSleepRecords2.ElementAt(i - 2).Kind == 0 && listSleepRecords2.ElementAt(i - 1).Kind > 0 && listSleepRecords2.ElementAt(i).Kind > 0 && count == 0)
                                            {
                                                sleepWakeDiary.SleepTime = listSleepRecords2.ElementAt(i - 1).DateTimeHour;
                                                count = 1;
                                            }
                                        }
                                    }
                                Debug.WriteLine("SleepTime: " + sleepWakeDiary.SleepTime);
                                Debug.WriteLine("Se calculó la hora a la que se durmió del diario de sueño-vigilia");
                                //Calcular cuántos minutos le tomó dormirse al paciente
                                sleepWakeDiary.TimeToFallSleep = (sleepWakeDiary.SleepTime - sleepWakeDiary.GoToSleepTime).TotalMinutes;
                                Debug.WriteLine("Se calculó cuántos minutos le tomó dormirse al paciente para el diario de sueño-vigilia");
                                Debug.WriteLine("TimeToFallSleep: " + sleepWakeDiary.TimeToFallSleep);
                                //Calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                                //el sleep record registrado antes de éste es 1 o 2
                                for (int i = 1; i < listSleepRecords2.Count; i++)
                                {
                                    if (listSleepRecords2.ElementAt(i - 1).Kind > 0 && listSleepRecords2.ElementAt(i).Kind == 0)
                                    {
                                        sleepWakeDiary.WakeUpTime = listSleepRecords2.ElementAt(i).DateTimeHour;
                                    }
                                }
                                Debug.WriteLine("Se calculó la hora a la que se despertó del diario de sueño-vigilia");
                                Debug.WriteLine("WakeUpTime: " + sleepWakeDiary.WakeUpTime);
                                //Calcular las horas en la cama restando las fechas del primer con el último elemento de la lista
                                sleepWakeDiary.HoursTotal = (sleepWakeDiary.WakeUpTime - sleepWakeDiary.GoToSleepTime).TotalHours;
                                sleepWakeDiary.HoursTotal = Math.Round(sleepWakeDiary.HoursTotal, 2);
                                Debug.WriteLine("HoursTotal: " + sleepWakeDiary.HoursTotal);
                                Debug.WriteLine("Se calcularon las horas totales del diario de sueño-vigilia");

                                foreach (var sleepRecord in listSleepRecords1)
                                {
                                    listSleepRecords3.Add(sleepRecord);
                                }
                                foreach (var sleepRecord in listSleepRecords2)
                                {
                                    listSleepRecords3.Add(sleepRecord);
                                }

                                //Ordenar de la más antigua a la más reciente
                                Debug.WriteLine("Cantidad de sleeprecords3: " + listSleepRecords3.Count);
                                listSleepRecords3 = listSleepRecords3.OrderBy(o => o.DateTimeHour).ToList();
                                Debug.WriteLine("El primer sleeprecord de la lista sleeprecords3: " + listSleepRecords3.First().DateTimeHour);
                                Debug.WriteLine("El último sleeprecord de la lista sleeprecords3: " + listSleepRecords3.Last().DateTimeHour);
                                Debug.WriteLine("Se logró registrar la lista de sleeprecords 3 del paciente desde la hora que durmió hasta la hora que despertó el día siguiente.");

                                count = 0;
                                sum = 0;
                                amountMinutes = 0;
                                //Se calcula los minutos que el paciente estuvo dormido
                                //sleepWakeDiary.HoursSlept = (sleepWakeDiary.WakeUpTime - sleepWakeDiary.SleepTime).TotalMinutes;
                                //sleepWakeDiary.HoursSlept = Math.Round(sleepWakeDiary.HoursSlept / 60, 2);

                                /*for (int i = 0; i < listSleepRecords3.Count; i++)
                                {
                                    count = 0;
                                    amountMinutes = 0;
                                    if (listSleepRecords3.ElementAt(i).Kind > 0)
                                    {
                                        for (int j = i + 1; j < listSleepRecords3.Count; j++)
                                        {
                                            if (listSleepRecords3.ElementAt(j).Kind == 0 && count == 0)
                                            {
                                                amountMinutes = (listSleepRecords3.ElementAt(j).DateTimeHour - listSleepRecords3.ElementAt(i).DateTimeHour).TotalMinutes;
                                                sum = sum + amountMinutes;
                                                count = 1;
                                                i = j + 1;
                                            }
                                        }
                                    }
                                }*/
                                for (int i = 0; i < listSleepRecords3.Count; i++)
                                {
                                    if (listSleepRecords3.ElementAt(i).Kind > 0)
                                    {
                                        amountMinutes++;
                                    }
                                }
                                Debug.WriteLine("Cantida de minutos dormido: " + amountMinutes);
                                //Se calculan las horas dormidas
                                sleepWakeDiary.HoursSlept = amountMinutes / 60;
                                sleepWakeDiary.HoursSlept = Math.Round(sleepWakeDiary.HoursSlept, 2);
                                Debug.WriteLine("Se calcularon las horas horas dormidas del diario de sueño-vigilia");
                                Debug.WriteLine("HoursSlept: " + sleepWakeDiary.HoursSlept);

                                //Y con ello se calcula la eficiencia del sueño del diario de sueño-vigilia
                                sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
                                sleepWakeDiary.SleepEfficiency = Math.Round(sleepWakeDiary.SleepEfficiency, 2);
                                Debug.WriteLine("Se calculó la eficiencia del sueño del diario de sueño-vigilia");
                                Debug.WriteLine("SleepEfficiency: " + sleepWakeDiary.SleepEfficiency);

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
                                              GoToSleepTime = sleepWakeDiary.GoToSleepTime.AddHours(-5),
                                              TimeToFallSleep = sleepWakeDiary.TimeToFallSleep,
                                              HoursSlept = sleepWakeDiary.HoursSlept,
                                              HoursTotal = sleepWakeDiary.HoursTotal,
                                              SleepEfficiency = sleepWakeDiary.SleepEfficiency
                                          });
                                Debug.WriteLine("Se registró el diario de sueño-vigilia del día a evaluar: Día: " + (contador - 1));
                                verificacion2++;
                            }
                            else
                            {
                                Debug.WriteLine("No existe data de sleep records de sueño 1 o 2 del Día: " + (contador - 1) + "y Día " + contador);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No existe data de sleep records de los días en cuestión => Día: " + (contador - 1) + "y Día " + contador);
                        }
                    }
                    if (contador == -6 && verificacion2 > 0)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.", new TimeSpan(8));
                        Debug.WriteLine("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.");
                    }
                    else if (contador == -6 && verificacion2 == 0)
                    {
                        Debug.WriteLine("No existen datos de sueño para crear diarios de sueño-vigilia");
                        Acr.UserDialogs.UserDialogs.Instance.Toast("No existen datos de sueño para crear diarios de sueño-vigilia.", new TimeSpan(8));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("Error - Sincronización fallida con el wearable.");
                Acr.UserDialogs.UserDialogs.Instance.Toast("Error - Sincronización fallida con el wearable.", new TimeSpan(8));
                return;
            }
        }
    }
}
