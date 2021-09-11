using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models
{

    public class SleepWakeDiary
    {
        public string SleepWakeDiary_ID { get; set; }
        public string Patient_ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SleepTime { get; set; }
        public DateTime WakeUpTime { get; set; }
        public DateTime GoToSleepTime { get; set; }
        public double TimeToFallSleep { get; set; }
        public double HoursTotal { get; set; }
        public double HoursSlept { get; set; }
        public double SleepEfficiency { get; set; }
    }
}
