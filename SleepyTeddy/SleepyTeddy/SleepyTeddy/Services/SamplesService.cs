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

        public SamplesService(IHeartrateRepository heartrateRepository, IStepsRepository stepsRepository, ISleepRepository sleepRepository)
        {
            _heartrateRepository = heartrateRepository;
            _stepsRepository = stepsRepository;
            _sleepRepository = sleepRepository;
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
        private async void FillDatabase(List<ActivitySample> samples)
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
            Globals.SleepPageViewModel.CreateSleepRecords();
            await Globals.SleepPageViewModel.CreateSleepWakeDiaries();
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

        private void AddSleep(DateTime datetime, ActivitySample sample)
        {
            Sleep sleep;
            switch (sample.Category)
            {
                //Categories cuando se despierta el paciente
                //7: El usuario está en su cama; 28, 105: El usuario está despierto; 80, 96, 99: El usuario está sentado; 96: El usuario está parado
                //; 89: El usuario se despierta 
                case 7:
                case 28:
                case 80:
                case 96:
                case 89:
                case 99:
                case 105:
                    if (sample.HeartRate != 255)
                    {
                        sleep = new Sleep(datetime, SleepType.Awake);
                    }
                    else
                    {
                        sleep = new Sleep(datetime, SleepType.Empty);
                    }
                    if (sample.Timestamp.Hour >= 2 && sample.Timestamp.Hour <= 4)
                    {
                        sleep = new Sleep(datetime, SleepType.Sleep);
                    } 
                    break;

                //3,6, 83, 115: no lleva puesto el wearable
                case 3:
                case 6:
                case 83:
                case 115:
                    if (sample.Timestamp.Hour >= 2 && sample.Timestamp.Hour <= 4)
                    {
                        sleep = new Sleep(datetime, SleepType.Sleep);
                    }
                    else
                    {
                        sleep = new Sleep(datetime, SleepType.Empty);
                    }
                    break;

                //4, 12: Sueño profundo
                case 4:
                case 112:
                    sleep = new Sleep(datetime, SleepType.Sleep);
                    break;

                //5, 9, 106, 121, 122, 123: Sueño ligero
                case 5:
                case 9:
                case 106:
                case 121:
                case 122:
                case 123:
                    sleep = new Sleep(datetime, SleepType.Sleep);
                    break;

                default:
                    if (sample.HeartRate != 255)
                    {
                        sleep = new Sleep(datetime, SleepType.Awake);
                    }
                    else
                    {
                        sleep = new Sleep(datetime, SleepType.Empty);
                    }
                    break;
            }
            _sleepRepository.Add(sleep);
        }
    }
}
