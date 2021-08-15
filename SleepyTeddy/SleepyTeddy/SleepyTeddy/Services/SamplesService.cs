using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugin.CloudFirestore;
using SleepyTeddy.Data.Interfaces;
using SleepyTeddy.Models;
using SleepyTeddy.Resources;
using SleepyTeddy.ViewModel;
using WindesHeartSDK;
using WindesHeartSDK.Models;
using Xamarin.Forms;

namespace SleepyTeddy.Services
{
    public class SamplesService
    {
        private readonly IHeartrateRepository _heartrateRepository;
        private readonly IStepsRepository _stepsRepository;
        private readonly ISleepRepository _sleepRepository;

        private DateTime _fetchingStartDate;
        private int _totalSamples = 0;

        SleepWakeDiary sleepWakeDiary;
        string documentId;
        GetDataFromLoginUser objData { get; set; }
        List<SleepRecordsView> listSleepRecords;
        TimeSpan sum;

        public SamplesService(IHeartrateRepository heartrateRepository, IStepsRepository stepsRepository, ISleepRepository sleepRepository)
        {
            _heartrateRepository = heartrateRepository;
            _stepsRepository = stepsRepository;
            _sleepRepository = sleepRepository;
            sleepWakeDiary = new SleepWakeDiary();
            objData = new GetDataFromLoginUser();
        }

        public void StartFetching()
        {

            Device.BeginInvokeOnMainThread(delegate
            {
                Globals.MiCuentaPacienteViewModel.FetchProgressVisible = true;
                Globals.MiCuentaPacienteViewModel.EnableDisableButtons(false);
                Globals.MiCuentaPacienteViewModel.IsLoading = true;
            });
            _fetchingStartDate = GetLastAddedDateTime();
            Windesheart.PairedDevice.GetSamples(_fetchingStartDate, FillDatabase, ProgressCalculator);
        }

        private void ProgressCalculator(int remainingSamples)
        {
            if (_totalSamples == 0)
            {
                _totalSamples = remainingSamples;
            }
            //Calculates percentage of progression. -10f to leave some space for DB insertion progress indication.
            float calculatedProgress = ((float)_totalSamples - (float)remainingSamples) / (float)_totalSamples;


            //Leave some space on progressbar for DB insertion
            if (calculatedProgress > 0.9f)
            {
                calculatedProgress = 0.9f;
            }

            Device.BeginInvokeOnMainThread(delegate
            {
                Globals.MiCuentaPacienteViewModel.ShowFetchProgress(calculatedProgress);
            });
        }
        private void FillDatabase(List<ActivitySample> samples)
        {
            Debug.WriteLine("Filling DB with samples: " + samples.Count);
            Globals.Database.Instance.BeginTransaction();
            foreach (var sample in samples)
            {
                var datetime = sample.Timestamp;

                AddHeartrate(datetime, sample);
                AddStep(datetime, sample);
                AddSleep(datetime, sample);
            }
            Globals.Database.Instance.Commit();
            Debug.WriteLine("DB filled with samples");
            Device.BeginInvokeOnMainThread(delegate
            {
                Globals.MiCuentaPacienteViewModel.IsLoading = false;
                Globals.MiCuentaPacienteViewModel.EnableDisableButtons(true);
                Globals.MiCuentaPacienteViewModel.ShowFetchProgress(1f);
            });
        }

        private DateTime GetLastAddedDateTime()
        {
            return _stepsRepository.LastAddedDatetime();
        }

        private void AddHeartrate(DateTime datetime, ActivitySample sample)
        {
            var heartRate = new SleepyTeddy.Models.Heartrate(datetime, sample.HeartRate != 255 ? sample.HeartRate : 0);
            _heartrateRepository.Add(heartRate);
        }

        private void AddStep(DateTime datetime, ActivitySample sample)
        {
            var step = new Step(datetime, sample.Steps);
            _stepsRepository.Add(step);
        }

        private async void AddSleep(DateTime datetime, ActivitySample sample)
        {
            Sleep sleep;
            switch (sample.Category)
            {
                case 112:
                    sleep = new Sleep(datetime, SleepType.Light);
                    break;
                case 121:
                case 122:
                case 123:
                    sleep = new Sleep(datetime, SleepType.Deep);
                    break;

                default:
                    sleep = new Sleep(datetime, SleepType.Awake);
                    break;
            }
            _sleepRepository.Add(sleep);
            await CrossCloudFirestore.Current
                         .Instance
                         .Collection("SleepRecords")
                         .AddAsync(new SleepRecord
                         {
                             SleepWakeDiary_ID = Globals.MiCuentaPacienteViewModel.SleepWakeDiary,
                             SleepRecord_ID = Guid.NewGuid().ToString().Replace("-", ""),
                             DateTimeHour = sleep.DateTime,
                             Kind = (int)sleep.SleepType
                         });
        }
        public async void AddSleepWakeDiary()
        {
            await CrossCloudFirestore.Current
                         .Instance
                         .Collection("SleepWakeDiaries")
                         .AddAsync(new SleepWakeDiary
                         {
                             SleepWakeDiary_ID = Globals.MiCuentaPacienteViewModel.SleepWakeDiary,
                             Patient_ID = LoginViewModel.Patient_ID,
                             CreatedDate = DateTime.Now.AddHours(-5),
                             SleepTime = DateTime.MinValue,
                             WakeUpTime = DateTime.MinValue,
                             HoursTotal = 0,
                             HoursSlept = 0,
                             SleepEfficiency = 0
                         });
        }
        private async void getSleepWakeDiary()
        {
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("SleepWakeDiaries")
                                       .WhereEqualsTo("SleepWakeDiary_ID", Globals.MiCuentaPacienteViewModel.SleepWakeDiary)
                                       .GetAsync();
            sleepWakeDiary = document.Documents.ElementAt(0).ToObject<SleepWakeDiary>();
            documentId = document.Documents.ElementAt(0).Id;
        }
        public async void CompleteSleepWakeDiary()
        {
            try
            {
                int count = 0;
                listSleepRecords = new List<SleepRecordsView>();
                getSleepWakeDiary();
                await objData.GetSleepRecordsViewAsync();
                listSleepRecords = objData.ListSleepRecords.OrderByDescending(o => o.DateTimeHour).ToList();
                sleepWakeDiary.HoursTotal = (listSleepRecords.First().DateTimeHour - listSleepRecords.Last().DateTimeHour).TotalHours;
                if (listSleepRecords.Count != 0)
                {
                    foreach (var SleepRecord in listSleepRecords)
                    {
                        if (SleepRecord.Kind != 0 && count == 0)
                        {
                            sleepWakeDiary.SleepTime = SleepRecord.DateTimeHour;
                            count = 1;
                        }
                    }
                    count = 0;
                    listSleepRecords = objData.ListSleepRecords.OrderBy(o => o.DateTimeHour).ToList();
                    for (int i = 0; i < listSleepRecords.Count; i++)
                    {
                        if (listSleepRecords.ElementAt(i).Kind == 0 && listSleepRecords.ElementAt(i + 1).Kind != 0 && count == 0)
                        {
                            sleepWakeDiary.WakeUpTime = listSleepRecords.ElementAt(i).DateTimeHour;
                            count = 1;
                        }
                    }
                    for (int i = 0; i < listSleepRecords.Count; i++)
                    {
                        if (listSleepRecords.ElementAt(i).Kind != 0)
                        {
                            //sum += listSleepRecords.ElementAt(i).DateTimeHour.ToShortTimeString();
                        }
                    }
                    sleepWakeDiary.SleepEfficiency = sleepWakeDiary.HoursSlept / sleepWakeDiary.HoursTotal * 100;
                    await CrossCloudFirestore.Current
                            .Instance
                            .Collection("SleepWakeDiaries")
                            .Document(documentId)
                            .UpdateAsync(sleepWakeDiary);
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Registro de diario de sueño-vigilia finalizado.", new TimeSpan(3));
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("El diario de sueño-vigilia no tiene registros guardados.", new TimeSpan(3));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("ERROR al completar el diario de sueño-vigilia");
                Acr.UserDialogs.UserDialogs.Instance.Toast("El diario de sueño-vigilia no tiene registros guardados.", new TimeSpan(3));
                return;
            }
        }
    }
}
