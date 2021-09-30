using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models
{
    public enum SleepType
    {
        Awake,
        Sleep,
        Empty
    }

    [Table("Sleep")]
    public class Sleep
    {
        public Sleep() { }
        public Sleep(DateTime datetime, SleepType sleepType, string patient_ID)
        {
            this.DateTime = datetime;
            this.SleepType = sleepType;
            this.Patient_ID = patient_ID;
        }

        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }

        [Column("DateTime")]
        public DateTime DateTime { get; set; }

        [Column("SleepType")]
        public SleepType SleepType { get; set; }

        [Column("Patient_ID")]
        public string Patient_ID { get; set; }
    }
}
