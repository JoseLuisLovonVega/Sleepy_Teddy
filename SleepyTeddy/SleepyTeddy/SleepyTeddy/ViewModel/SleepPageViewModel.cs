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
        public List<Sleep> SleepInfo2;
        public DateTime StartDate { get; }
        public DateTime SelectedDate;
        public DateTime GoToSleepTime;

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

        int dia;

        private ISleepRepository _sleepRepository;

        public event PropertyChangedEventHandler PropertyChanged;

        public SleepPageViewModel(ISleepRepository sleepRepository)
        {
            _sleepRepository = sleepRepository;
            //Si la hora actual es menor a 6 el DateTime.Today no se bugeará y no mandará como si ya estuviera mañana
            if (DateTime.Now.AddHours(-5).Hour < 18)
            {
                StartDate = DateTime.Today;
            }
            else
            {
                StartDate = DateTime.Today.AddDays(-1);
            }
            Debug.WriteLine("La fecha de hoy es: " + StartDate);
            //SelectedDate = StartDate;

            SleepInfo2 = new List<Sleep>();

            objData = new GetDataFromLoginUser();
            sleepWakeDiary = new SleepWakeDiary();

            listSleepRecordsLocalDB = new List<SleepRecordsView>();
            listSleepRecords1 = new List<SleepRecordsView>();
            listSleepRecords2 = new List<SleepRecordsView>();

            sleepRecord = new SleepRecordsView();

            listSleepRecords12 = new List<SleepRecordsView>();
            listSleepRecords22 = new List<SleepRecordsView>();
        }

        private List<Sleep> GetCurrentSleep()
        {
            return SleepInfo.Where(s => s.DateTime.Year == SelectedDate.Year &&
            s.DateTime.Month == SelectedDate.Month &&
            s.DateTime > SelectedDate.AddHours(-4) &&
            s.DateTime < SelectedDate.AddHours(13)).
            OrderBy(x => x.DateTime).ToList();
        }

        public void CreateSleepRecords()
        {
            Debug.WriteLine("Se inicia el proceso para agregar los sleeprecords a la lista designada");
            SleepInfo = _sleepRepository.GetAll();
            for (int k = 0; k > -7; k--)
            {
                SelectedDate = StartDate.AddDays(k);
                List<Sleep> sleepData = GetCurrentSleep();

                if (sleepData.Count() > 0)
                {
                    Debug.WriteLine("Se analiza el Día: " + StartDate.AddDays(k));
                    //For each hour
                    for (int i = 20; i < 37; i++)
                    {
                        int hour = i;
                        if (i >= 24) hour -= 24;

                        //Get sleep data for that hour
                        List<Sleep> data = sleepData.Where(x => x.DateTime.Hour == hour).ToList();
                        for (int j = 0; j < 60; j++)
                        {
                            if (data.ElementAtOrDefault(j) != null)
                            {
                                if (data[j].SleepType != SleepType.Empty)
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
                        Debug.WriteLine("Día "+ k + ": Se registraron sleep records de la hora: " + i);
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

        public async Task CreateCompletedSleepWakeDiaries()
        {
            try
            {
                await objData.GetSleepWakeDiariesViewAsync(LoginViewModel.Patient_ID);
                //CreateSleepRecords();
                for (int contador = 0; contador > -6; contador--)
                {
                    verificacion = 0;

                    //Verificar que no se registren diarios de sueño-vigilia con la misma fecha
                    if (objData.ListSleepWakeDiaries.Count > 0)
                    {
                        foreach (var SWDiary in objData.ListSleepWakeDiaries)
                        {
                            if (DateTime.Now.AddHours(-5).AddDays(contador - 1).Date.ToString("dd/MM/yy") == SWDiary.CreatedDate_S)
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
                        sleepWakeDiary.SleepTime=DateTime.MinValue;
                        sleepWakeDiary.WakeUpTime = DateTime.MinValue;
                        sleepWakeDiary.GoToSleepTime=0;
                        sleepWakeDiary.HoursSlept=0;
                        sleepWakeDiary.HoursTotal=0;
                        sleepWakeDiary.SleepEfficiency=0;

                        amountMinutes = 0;
                        sum = 0;
                        count = 0;

                        listSleepRecords1 = new List<SleepRecordsView>();
                        listSleepRecords2 = new List<SleepRecordsView>();
                        listSleepRecords3 = new List<SleepRecordsView>();

                        listSleepRecords12 = new List<SleepRecordsView>();
                        listSleepRecords22 = new List<SleepRecordsView>();

                        GoToSleepTime = new DateTime();

                        dia = contador-1;

                        //await objData.GetSleepRecordsViewAsync();
                        Debug.WriteLine("Se logró obtener todos los sleep records del paciente del día: " + DateTime.Now.AddHours(-5).AddDays(contador-1));
                        //Ordenar de la más antigua a la más reciente
                        listSleepRecordsLocalDB = listSleepRecordsLocalDB.OrderBy(o => o.DateTimeHour).ToList();
                        Debug.WriteLine("Se logró ordenar ascendentemente todos los sleep records del paciente.");

                        foreach (var sleepRecord in listSleepRecordsLocalDB)
                        {
                            if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Now.AddHours(-5).AddDays(contador - 1).ToString("dd/MM/yy") && sleepRecord.DateTimeHour.Hour >= 20)
                            {
                                listSleepRecords1.Add(sleepRecord);
                            }
                        }

                        foreach (var sleepRecord in listSleepRecordsLocalDB)
                        {
                            if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Now.AddHours(-5).AddDays(contador).ToString("dd/MM/yy") && sleepRecord.DateTimeHour.Hour < 14)
                            {
                                listSleepRecords2.Add(sleepRecord);
                            }
                        }

                        if (listSleepRecords1.Count > 0 && listSleepRecords2.Count > 0)
                        {
                            //Ordenar de la más antigua a la más reciente
                            Debug.WriteLine("Cantidad de sleeprecords1: " + listSleepRecords1.Count);
                            listSleepRecords1 = listSleepRecords1.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("El primer sleeprecord de la lista sleeprecords1: " + listSleepRecords1.First().DateTimeHour);
                            Debug.WriteLine("El último sleeprecord de la lista sleeprecords1: " + listSleepRecords1.Last().DateTimeHour);
                            //Ordenar de la más antigua a la más reciente
                            Debug.WriteLine("Cantidad de sleeprecords2: " + listSleepRecords2.Count);
                            listSleepRecords2 = listSleepRecords2.OrderBy(o => o.DateTimeHour).ToList();
                            Debug.WriteLine("El primer sleeprecord de la lista sleeprecords2: " + listSleepRecords2.First().DateTimeHour);
                            Debug.WriteLine("El último sleeprecord de la lista sleeprecords2: " + listSleepRecords2.Last().DateTimeHour);

                            foreach (var sleepRecord in listSleepRecords1)
                            {
                                if (sleepRecord.Kind > 0)
                                {
                                    listSleepRecords12.Add(sleepRecord);
                                }
                            }

                            foreach (var sleepRecord in listSleepRecords2)
                            {
                                if (sleepRecord.Kind > 0)
                                {
                                    listSleepRecords22.Add(sleepRecord);
                                }
                            }

                            if (listSleepRecords12.Count > 0 && listSleepRecords22.Count > 0)
                            {
                                //Ordenar de la más antigua a la más reciente
                                listSleepRecords12 = listSleepRecords12.OrderBy(o => o.DateTimeHour).ToList();
                                Debug.WriteLine("Cantidad de sleeprecords12: " + listSleepRecords12.Count);
                                Debug.WriteLine("El primer sleeprecord de la lista sleeprecords12: " + listSleepRecords12.First().DateTimeHour);
                                Debug.WriteLine("El último sleeprecord de la lista sleeprecords12: " + listSleepRecords12.Last().DateTimeHour);

                                //Ordenar de la más antigua a la más reciente
                                listSleepRecords22 = listSleepRecords22.OrderBy(o => o.DateTimeHour).ToList();
                                Debug.WriteLine("Cantidad de sleeprecords22: " + listSleepRecords22.Count);
                                Debug.WriteLine("El primer sleeprecord de la lista sleeprecords22: " + listSleepRecords22.First().DateTimeHour);
                                Debug.WriteLine("El último sleeprecord de la lista sleeprecords22: " + listSleepRecords22.Last().DateTimeHour);

                                //Se define la fecha de creación del diario de sueño como un día antes del día a evaluar
                                if (DateTime.Now.AddHours(-5).Hour < 18)
                                {
                                    sleepWakeDiary.CreatedDate = DateTime.Today.AddDays(contador-1);
                                }
                                else
                                {
                                    sleepWakeDiary.CreatedDate = DateTime.Today.AddDays(contador-2);
                                }
                                Debug.WriteLine("Se registro la fecha de creación del diario de sueño-vigilia");

                                //Calcular a qué hora se fue a su cama el paciente el día anterior al día a evaluar
                                count = 0;
                                for (int i = 1; i < listSleepRecords1.Count; i++)
                                {
                                    if (listSleepRecords1.ElementAt(i).Kind == 0 && listSleepRecords1.ElementAt(i-1).Kind == 0 && count == 0)
                                    {
                                        GoToSleepTime = listSleepRecords1.ElementAt(i-1).DateTimeHour;
                                        count = 1;
                                    }
                                }
                                count = 0;
                                //Calcular a qué hora se durmió el paciente el día anterior al día a evaluar
                                for (int i = 1; i < listSleepRecords1.Count; i++)
                                {
                                    if (listSleepRecords1.ElementAt(i).Kind > 0 && listSleepRecords1.ElementAt(i-1).Kind > 0 && count == 0)
                                    {
                                        sleepWakeDiary.SleepTime = listSleepRecords1.ElementAt(i-1).DateTimeHour;
                                        count = 1;
                                    }
                                }
                                Debug.WriteLine("Se calculó la hora a la que se durmió del diario de sueño-vigilia");
                                //Calcular cuántos minutos le tomó dormirse al paciente
                                sleepWakeDiary.GoToSleepTime = (sleepWakeDiary.SleepTime - GoToSleepTime).TotalMinutes;
                                Debug.WriteLine("Se calculó cuántos minutos le tomó dormirse al paciente para el diario de sueño-vigilia");

                                //Calcular a que horá se despertó el paciente, el criterio es si el sleep record es de tipo 0 y si
                                //el sleep record registrado antes de éste es 1 o 2
                                for (int i = 2; i < listSleepRecords2.Count; i++)
                                {
                                    if (listSleepRecords2.ElementAt(i-2).Kind>0 && listSleepRecords2.ElementAt(i-1).Kind == 0 && listSleepRecords2.ElementAt(i).Kind == 0)
                                    {
                                        sleepWakeDiary.WakeUpTime = listSleepRecords2.ElementAt(i-1).DateTimeHour;
                                    }
                                }
                                Debug.WriteLine("Se calculó la hora a la que se despertó del diario de sueño-vigilia");

                                //Calcular las horas en la cama restando las fechas del primer con el último elemento de la lista
                                sleepWakeDiary.HoursTotal = (sleepWakeDiary.WakeUpTime - GoToSleepTime).TotalMinutes;
                                sleepWakeDiary.HoursTotal = Math.Round(sleepWakeDiary.HoursTotal / 60, 2);
                                Debug.WriteLine("Se calcularon las horas totales del diario de sueño-vigilia");

                                foreach (var sleepRecord in listSleepRecordsLocalDB)
                                {
                                    if (sleepRecord.DateTimeHour >= sleepWakeDiary.SleepTime && sleepRecord.DateTimeHour <= sleepWakeDiary.WakeUpTime)
                                    {
                                        listSleepRecords3.Add(sleepRecord);
                                    }
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
                                for (int i = 0; i < listSleepRecords3.Count; i++)
                                {
                                    count = 0;
                                    if (listSleepRecords3.ElementAt(i).Kind > 0)
                                    {
                                        for (int j = i + 1; j < listSleepRecords3.Count; j++)
                                        {
                                            if (listSleepRecords3.ElementAt(j).Kind == 0 && count == 0)
                                            {
                                                amountMinutes = (listSleepRecords3.ElementAt(j).DateTimeHour - listSleepRecords3.ElementAt(i).DateTimeHour).TotalMinutes;
                                                sum = sum + amountMinutes;
                                                count = 1;
                                                i = j;
                                            }
                                        }
                                    }
                                }
                                //Se calculan las horas dormidas
                                sum = sum / 60;
                                sleepWakeDiary.HoursSlept = Math.Round(sum, 2);
                                Debug.WriteLine("Se calcularon las horas horas dormidas del diario de sueño-vigilia");

                                //Y con ello se calcula la eficiencia del sueño del diario de sueño-vigilia
                                sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
                                Debug.WriteLine("Se calculó la eficiencia del sueño del diario de sueño-vigilia");
                                sleepWakeDiary.SleepEfficiency = Math.Round(sleepWakeDiary.SleepEfficiency, 2);

                                //Se crea el diario de sueño-vigilia
                                await CrossCloudFirestore.Current
                                          .Instance
                                          .Collection("SleepWakeDiaries")
                                          .AddAsync(new SleepWakeDiary
                                          {
                                              SleepWakeDiary_ID = Guid.NewGuid().ToString().Replace("-", ""),
                                              Patient_ID = LoginViewModel.Patient_ID,
                                              CreatedDate = sleepWakeDiary.CreatedDate.AddHours(-5),
                                              SleepTime = sleepWakeDiary.SleepTime.AddHours(-5),
                                              WakeUpTime = sleepWakeDiary.WakeUpTime.AddHours(-5),
                                              GoToSleepTime = sleepWakeDiary.GoToSleepTime,
                                              HoursSlept = sleepWakeDiary.HoursSlept,
                                              HoursTotal = sleepWakeDiary.HoursTotal,
                                              SleepEfficiency = sleepWakeDiary.SleepEfficiency
                                          });
                                Debug.WriteLine("Se registró el diario de sueño-vigilia del día a evaluar: Día: " + (contador-1));
                            }
                            else
                            {
                                Debug.WriteLine("No existe data de sleep records de sueño 1 o 2 del Día: " + (contador-1) + "y Día " + contador);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("No existe data de sleep records de los días en cuestión.: Día: " + (contador - 1) + "y Día " + contador);
                        }
                    }
                    if (contador == -5)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización Exitosa. Registros de diarios de sueño-vigilia completados.", new TimeSpan(4));
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
