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

                await objData.GetSleepRecordsViewAsync();
                listSleepRecords = objData.ListSleepRecords.OrderBy(o => o.DateTimeHour).ToList();

                for (int contador = 0; contador > -7; contador--)
                {
                    unicaVerficiacion = 0;
                    await objData.GetSleepRecordsDateFilterViewAsync(contador);
                    if (objData.ListSleepRecordsDateFilter.Count > 0)
                    {
                        listSleepRecordsDateFilter = objData.ListSleepRecordsDateFilter.OrderBy(o => o.DateTimeHour).ToList();
                        if (unicaVerficiacion == 0)
                        {
                            foreach (var sleepRecord in listSleepRecordsDateFilter)
                            {
                                if (sleepRecord.DateTimeHour.Hour < 16 && sleepRecord.Kind > 0)
                                {
                                    unicaVerficiacion = 1;
                                    sleepWakeDiary.CreatedDate = sleepRecord.DateTimeHour.Date;

                                    foreach (var sleepRecord2 in listSleepRecords)
                                    {
                                        if (sleepRecord2.DateTimeHour.AddDays(contador - 1).Hour > 16
                                            && sleepRecord2.DateTimeHour.AddDays(contador).Hour < 16 && sleepRecord2.Kind > 0)
                                        {
                                            listSleepRecords2.Add(sleepRecord2);
                                        }
                                    }
                                    if (listSleepRecords2.Count != 0)
                                    {
                                        sleepWakeDiary.HoursTotal = (listSleepRecords2.First().DateTimeHour - listSleepRecords2.Last().DateTimeHour).TotalHours;
                                        foreach (var SleepRecord in listSleepRecords2)
                                        {
                                            if (SleepRecord.Kind ==2 && count == 0)
                                            {
                                                sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                                                count = 1;
                                            }
                                        }
                                        count = 0;
                                        listSleepRecords2 = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();

                                        for (int i = 0; i < listSleepRecords2.Count; i++)
                                        {
                                            if (listSleepRecords2.ElementAt(i).Kind == 0 && listSleepRecords2.ElementAt(i - 1).Kind != 0 && count == 0)
                                            {
                                                sleepWakeDiary.WakeUpTime = listSleepRecords2.ElementAt(i).DateTimeHour;
                                                count = 1;
                                            }
                                        }
                                        listSleepRecords2 = objData.ListSleepRecords.OrderBy(o => o.DateTimeHour).ToList();
                                        for (int i = 0; i < listSleepRecords2.Count; i++)
                                        {
                                            amountMinutes = listSleepRecords2.ElementAt(i + 1).DateTimeHour.Subtract(listSleepRecords2.ElementAt(i).DateTimeHour).TotalMinutes;
                                            sum += amountMinutes;
                                        }
                                        sum = sum / 60;
                                        sleepWakeDiary.HoursSlept = sum;
                                        sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
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
                                        Acr.UserDialogs.UserDialogs.Instance.Toast(".", new TimeSpan(3));
                                    }
                                }
                            }
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
