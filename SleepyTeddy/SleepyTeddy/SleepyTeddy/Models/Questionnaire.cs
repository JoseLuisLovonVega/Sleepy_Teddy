using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models
{
    public class Questionnaire
    {
        public string Questionnaire_ID { get; set; }
        public string Therapist_ID{ get; set; }
        public string Patient_ID{ get; set; }
        public string Type{ get; set; }
        public DateTime D_Assigned_Date{ get; set; }
        public DateTime D_Completed_Date { get; set; }
        public float N_Result{ get; set; }
    }
}
