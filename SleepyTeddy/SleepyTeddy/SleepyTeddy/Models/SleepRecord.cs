using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models
{

    public class SleepRecord
    {
        public string SleepRecord_ID { get; set; }
        public string SleepWakeDiary_ID { get; set; }
        public DateTime DateTimeHour { get; set; }
        public int Kind { get; set; }
    }
}
