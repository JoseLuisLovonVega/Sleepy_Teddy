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
        public Double HoursTotal { get; set; }
        public Double HoursSlept { get; set; }
        public Double SleepEfficiency { get; set; }
    }
}
