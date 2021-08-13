using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models
{
    public enum Kind
    {
        Awake,
        Light,
        Deep
    }

    public class SleepRecord
    {
        public SleepRecord() { }
        public SleepRecord(DateTime datetime, Kind kind)
        {
            this.DateTimeHour = datetime;
            this.Kind = kind;
        }

        public string SleepRecord_ID { get; set; }
        public string SleepWakeDiary_ID { get; set; }
        public DateTime DateTimeHour { get; set; }
        public Kind Kind { get; set; }
    }
}
