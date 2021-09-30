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
using Microcharts;
using SleepyTeddy.Views.TherapistViews;
using Entry = Microcharts.ChartEntry;
using System.Globalization;

namespace SleepyTeddy.ViewModel
{
    public class SleepPageViewModel : INotifyPropertyChanged
    {
        public static IEnumerable<SleepRecordsView> SleepInfo = new List<SleepRecordsView>();
        public static IEnumerable<Sleep> SleepInfo2 = new List<Sleep>();
        public DateTime StartDate { get; }
        public DateTime SelectedDate;
        public DateTime StartDate2 { get; }
        public DateTime SelectedDate2;
        public event PropertyChangedEventHandler PropertyChanged;

        public string AwakeColor = "#ffffff";
        public string LightColor = "#1281ff";
        public string DeepColor = "#002bba";

        CultureInfo ci = new CultureInfo("Es-Es");

        private ISleepRepository _sleepRepository;
        private ButtonRow _buttonRow;
        private Chart _chart;
        private bool _isLoading;

        SleepWakeDiary sleepWakeDiary;
        GetDataFromLoginUser objData { get; set; }

        List<SleepRecordsView> listSleepRecordsLocalDB;
        List<SleepRecordsView> listSleepRecords1;
        List<SleepRecordsView> listSleepRecords2;
        List<SleepRecordsView> listSleepRecords3;

        List<SleepRecordsView> listSleepRecords12;
        List<SleepRecordsView> listSleepRecords22;

        int count = 0;
        double amountMinutes = 0;
        int verificacion = 0;
        int verificacion2 = 0;

        int dia = 0;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public Chart Chart
        {
            get => _chart;
            set
            {
                _chart = value;
                OnPropertyChanged();
            }
        }
        public async void OnAppearing()
        {
            await objData.GetSleepRecordsViewAsync(Globals.patientID);
            //Get all sleep data of the patient from DB
            Debug.WriteLine("Cantidad de sleep records: " + objData.ListSleepRecords.Count);
            SleepInfo = objData.ListSleepRecords;

            if (SleepInfo.Count() == 0)
            {
                Device.BeginInvokeOnMainThread(async delegate
                {
                    await Application.Current.MainPage.DisplayAlert("No existen datos", "Desafortunadamente, el paciente no tiene datos de sueño registrados.", "OK");
                });
            }

            //Init buttons on bottom
            List<Button> dayButtons = new List<Button>
            {
                VisualizarInformacionSueñoPaciente.Day1Button,
                VisualizarInformacionSueñoPaciente.Day2Button,
                VisualizarInformacionSueñoPaciente.Day3Button,
                VisualizarInformacionSueñoPaciente.Day4Button,
                VisualizarInformacionSueñoPaciente.Day5Button,
                VisualizarInformacionSueñoPaciente.Day6Button,
                VisualizarInformacionSueñoPaciente.TodayButton
            };
            _buttonRow = new ButtonRow(dayButtons);

            //Switch to today
            TodayBtnClick(VisualizarInformacionSueñoPaciente.TodayButton, new EventArgs());

            //Update chart in other thread
            IsLoading = true;
            await Task.Run(() =>
            {
                var data = GetData();
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateChart(data);
                });
            });
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public SleepPageViewModel(ISleepRepository sleepRepository)
        {
            _sleepRepository = sleepRepository;
            StartDate = DateTime.Today;
            SelectedDate = StartDate;

            StartDate2 = DateTime.Today;

            objData = new GetDataFromLoginUser();
            sleepWakeDiary = new SleepWakeDiary();

            listSleepRecordsLocalDB = new List<SleepRecordsView>();
            listSleepRecords1 = new List<SleepRecordsView>();
            listSleepRecords2 = new List<SleepRecordsView>();
            listSleepRecords3 = new List<SleepRecordsView>();

            listSleepRecords12 = new List<SleepRecordsView>();
            listSleepRecords22 = new List<SleepRecordsView>();
        }
        private async Task UpdateInfo()
        {
            IsLoading = true;

            if (SelectedDate == StartDate)
            {
                VisualizarInformacionSueñoPaciente.CurrentDayLabel.Text = "Hoy";
            }
            else if (SelectedDate >= StartDate.AddDays(-6))
            {
                VisualizarInformacionSueñoPaciente.CurrentDayLabel.Text = ci.TextInfo.ToTitleCase(ci.DateTimeFormat.GetDayName(SelectedDate.DayOfWeek).ToString());
            }
            else
            {
                VisualizarInformacionSueñoPaciente.CurrentDayLabel.Text = SelectedDate.ToString("dd/MM/yyyy");
            }

            //Update chart in other thread
            await Task.Run(() =>
            {
                var data = GetData();
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateChart(data);
                });
            });
        }

        private List<SleepRecordsView> GetCurrentSleep()
        {
            return SleepInfo.Where(s => s.DateTimeHour.Year == SelectedDate.Year &&
            s.DateTimeHour.Month == SelectedDate.Month &&
            s.DateTimeHour > SelectedDate.AddHours(-4) &&
            s.DateTimeHour < SelectedDate.AddHours(12)).
            OrderBy(x => x.DateTimeHour).ToList();
        }

        private List<Sleep> GetCurrentSleep2()
        {
            return SleepInfo2.Where(s => s.DateTime.Year == SelectedDate2.Year &&
            s.DateTime.Month == SelectedDate2.Month &&
            s.DateTime > SelectedDate2.AddHours(-4) &&
            s.DateTime < SelectedDate2.AddHours(12)).
            OrderBy(x => x.DateTime).ToList();
        }

        public async void CreateSleepRecords()
        {
            Debug.WriteLine("Se inicia el proceso para agregar los sleeprecords a la lista designada");
            listSleepRecordsLocalDB = new List<SleepRecordsView>();
            SleepInfo2 = _sleepRepository.GetAll();
            Debug.WriteLine("Cantidad de sleep records en la BD local: " + SleepInfo2.Count());
            await objData.GetSleepRecordsViewAsync(Globals.patientID);
            //listSleepRecordsLocalDB = new List<SleepRecordsView>();
            for (int k = 0; k > -7; k--)
            {
                SelectedDate2 = StartDate2.AddDays(k);
                List<Sleep> sleepData = GetCurrentSleep2();
                if (sleepData.Count() > 0)
                {
                    Debug.WriteLine("Se analiza el Día: " + StartDate2.AddDays(k));
                    //For each hour
                    for (int i = 20; i < 36; i++)
                    {
                        int hour = i;
                        if (i >= 24) hour -= 24;

                        //Get sleep data for that hour
                        List<Sleep> data = sleepData.Where(x => x.DateTime.Hour == hour).ToList();
                        for (int j = 0; j < 60; j++)
                        {
                            if (data.ElementAt(j) != null) //ElementAtOrDefault
                            {
                                if ((int)data[j].SleepType < 2)
                                {
                                    listSleepRecordsLocalDB.Add(new SleepRecordsView
                                    {
                                        Key = data[j].Id,
                                        Patient_ID = LoginViewModel.Patient_ID,
                                        DateTimeHour = data[j].DateTime,
                                        Kind = (int)data[j].SleepType
                                    });
                                }
                                if (objData.ListSleepRecords.Exists(x => x.DateTimeHour == data[j].DateTime) == false)
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
            CreateSleepWakeDiaries();
        }

        public async void CreateSleepWakeDiaries()
        {
            try
            {
                if (listSleepRecordsLocalDB.Count > 0)
                {
                    listSleepRecordsLocalDB = listSleepRecordsLocalDB.OrderBy(o => o.DateTimeHour).ToList();
                    Debug.WriteLine("Se logró ordenar ascendentemente todos los sleep records del paciente.");
                    Debug.WriteLine("CREANDO DIARIOS DE SUEÑO-VIGILIA...");
                    Debug.WriteLine("Cantidad de sleep records agregados a la lista para crear diarios de sueño-vigilia: " + listSleepRecordsLocalDB.Count);
                    verificacion2 = 0;
                    await objData.GetSleepWakeDiariesViewAsync(LoginViewModel.Patient_ID);
                    await objData.GetSleepRecordsViewAsync(Globals.patientID);
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
                            count = 0;

                            listSleepRecords1 = new List<SleepRecordsView>();

                            listSleepRecords2 = new List<SleepRecordsView>();

                            listSleepRecords3 = new List<SleepRecordsView>();

                            listSleepRecords12 = new List<SleepRecordsView>();

                            listSleepRecords22 = new List<SleepRecordsView>();

                            dia = contador - 1;

                            Debug.WriteLine("Se logró obtener todos los sleep records del paciente del día: " + DateTime.Now.AddDays(contador - 1).ToString("dd/MM/yy"));
                            //Ordenar de la más antigua a la más reciente

                            foreach (var sleepRecord in listSleepRecordsLocalDB)
                            {
                                if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).AddHours(-4).ToString("dd/MM/yy") && sleepRecord.DateTimeHour > DateTime.Today.AddDays(contador).AddHours(-4))
                                {
                                    listSleepRecords1.Add(sleepRecord);
                                }
                                else if (sleepRecord.DateTimeHour.ToString("dd/MM/yy") == DateTime.Today.AddDays(contador).AddHours(12).ToString("dd/MM/yy") && sleepRecord.DateTimeHour < DateTime.Today.AddDays(contador).AddHours(12))
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
                                    if (sleepRecord.Kind == 1)
                                    {
                                        listSleepRecords12.Add(sleepRecord);
                                    }
                                }
                                foreach (var sleepRecord in listSleepRecords2)
                                {
                                    if (sleepRecord.Kind == 1)
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
                                        for (int i = 1; i < listSleepRecords1.Count; i++)
                                        {
                                            if (listSleepRecords1.ElementAt(i - 1).Kind == 0 && listSleepRecords1.ElementAt(i).Kind == 0 && count == 0)
                                            {
                                                sleepWakeDiary.GoToSleepTime = listSleepRecords1.ElementAt(i - 1).DateTimeHour;
                                                count = 1;
                                            }
                                        }
                                    }
                                    if (sleepWakeDiary.GoToSleepTime == DateTime.MinValue)
                                    {
                                        for (int i = 1; i < listSleepRecords2.Count; i++)
                                        {
                                            if (listSleepRecords2.ElementAt(i - 1).Kind == 0 && listSleepRecords2.ElementAt(i).Kind == 0 && count == 0)
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
                                            if (listSleepRecords1.ElementAt(i - 2).Kind == 0 && listSleepRecords1.ElementAt(i - 1).Kind == 1 && listSleepRecords1.ElementAt(i).Kind == 1 && count == 0)
                                            {
                                                sleepWakeDiary.SleepTime = listSleepRecords1.ElementAt(i - 1).DateTimeHour;
                                                count = 1;
                                            }
                                        }
                                    }
                                    if (sleepWakeDiary.SleepTime == DateTime.MinValue)
                                    {
                                        for (int i = 2; i < listSleepRecords2.Count; i++)
                                        {
                                            if (listSleepRecords2.ElementAt(i - 2).Kind == 0 && listSleepRecords2.ElementAt(i - 1).Kind == 1 && listSleepRecords2.ElementAt(i).Kind == 1 && count == 0)
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
                                    for (int i = 2; i < listSleepRecords2.Count; i++)
                                    {
                                        if (listSleepRecords2.ElementAt(i - 2).Kind == 1 && listSleepRecords2.ElementAt(i - 1).Kind == 1 && listSleepRecords2.ElementAt(i).Kind == 0)
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
                                    amountMinutes = 0;
                                    //Se calcula los minutos que el paciente estuvo dormido
                                    for (int i = 0; i < listSleepRecords3.Count; i++)
                                    {
                                        if (listSleepRecords3.ElementAt(i).Kind == 1)
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
                                    Debug.WriteLine("No existe data de sleep records de sueño 1 o 2 del Día: " + (contador - 1) + " y Día " + contador);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("No existe data de sleep records de los días en cuestión => Día: " + (contador - 1) + " y Día " + contador);
                            }
                        }
                        if (contador == -6 && verificacion2 > 0)
                        {
                            Acr.UserDialogs.UserDialogs.Instance.Toast("Sincronización Exitosa. Registro de datos de sueño finalizado.", new TimeSpan(8));
                            Debug.WriteLine("Sincronización Exitosa. Registro de datos de sueño finalizado.");
                        }
                        else if (contador == -6 && verificacion2 == 0)
                        {
                            Debug.WriteLine("No existen datos de sueño en el wearable");
                            Acr.UserDialogs.UserDialogs.Instance.Toast("No existen datos de sueño en el wearable.", new TimeSpan(8));
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No existen datos de sueño en el wearable");
                    Acr.UserDialogs.UserDialogs.Instance.Toast("No existen datos de sueño en el wearable.", new TimeSpan(8));
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

        private List<Entry> GetData()
        {
            List<SleepRecordsView> sleepData = GetCurrentSleep();
            List<Entry> entries = new List<Entry>();

            //For each hour
            for (int i = 20; i < 36; i++)
            {
                int hour = i;
                if (i >= 24) hour -= 24;

                //Get sleep data for that hour
                List<SleepRecordsView> data = sleepData.Where(x => x.DateTimeHour.Hour == hour).ToList();

                for (int j = 0; j < 60; j++)
                {
                    if (data.ElementAtOrDefault(j) != null)
                    {
                        switch (data[j].Kind)
                        {
                            case 0:
                            case 2:
                                Entry awakeEntry = new Entry(1);
                                awakeEntry.Color = SKColor.Parse(AwakeColor);
                                entries.Add(awakeEntry);
                                break;
                            case 1:
                                Entry sleepEntry = new Entry(1);
                                sleepEntry.Color = SKColor.Parse(DeepColor);
                                entries.Add(sleepEntry);
                                break;
                                /*case 2:
                                    Entry emptyEntry = new Entry(1);
                                    emptyEntry.Color = SKColor.Parse(AwakeColor);
                                    entries.Add(emptyEntry);
                                    break;*/
                        }
                    }
                    else
                    {
                        Entry entry = new Entry(1);
                        entry.Color = SKColor.Parse(AwakeColor);
                        entries.Add(entry);
                    }
                }
            }
            return entries;
        }
        public void UpdateChart(List<Entry> entries)
        {
            Chart = new BarChart
            {
                Entries = entries,
                BackgroundColor = SKColors.Transparent,
                Margin = 0
            };
            IsLoading = false;
        }

        public async void PreviousDayBtnClick(object sender, EventArgs args)
        {
            Trace.WriteLine("Día anterior seleccionado!");

            //You can always go back
            _buttonRow.ToPrevious();
            SelectedDate = SelectedDate.AddDays(-1);
            await UpdateInfo();
        }

        public async void NextDayBtnClick(object sender, EventArgs args)
        {
            //If already today, you cant go next
            if (_buttonRow.ToNext())
            {
                SelectedDate = SelectedDate.AddDays(1);
                await UpdateInfo();
            }
        }

        public async void TodayBtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate;
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day6BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-1);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day5BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-2);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day4BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-3);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day3BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-4);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day2BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-5);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }

        public async void Day1BtnClick(object sender, EventArgs args)
        {
            SelectedDate = StartDate.AddDays(-6);
            if (_buttonRow.SwitchTo(sender as Button))
            {
                await UpdateInfo();
            }
        }
    }
}
