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

        //Cuestionario ISI
        public string ISI1a { get; set; }
        public string ISI1b { get; set; }
        public string ISI1c { get; set; }
        public string ISI2 { get; set; }
        public string ISI3 { get; set; }
        public string ISI4 { get; set; }
        public string ISI5 { get; set; }

        //Cuestionario PHQ9
        public string PHQ91 { get; set; }
        public string PHQ92 { get; set; }
        public string PHQ93 { get; set; }
        public string PHQ94 { get; set; }
        public string PHQ95 { get; set; }
        public string PHQ96 { get; set; }
        public string PHQ97 { get; set; }
        public string PHQ98 { get; set; }
        public string PHQ99 { get; set; }

        //Cuestionario PSQI
        public string PSQI1 { get; set; }
        public string PSQI2 { get; set; }
        public string PSQI3 { get; set; }
        public string PSQI4 { get; set; }
        public string PSQI5a { get; set; }
        public string PSQI5b { get; set; }
        public string PSQI5c { get; set; }
        public string PSQI5d { get; set; }
        public string PSQI5e { get; set; }
        public string PSQI5f { get; set; }
        public string PSQI5g { get; set; }
        public string PSQI5h { get; set; }
        public string PSQI5i { get; set; }
        public string PSQI5j { get; set; }
        public string PSQI6 { get; set; }
        public string PSQI7 { get; set; }
        public string PSQI8 { get; set; }
        public string PSQI9 { get; set; }
    }
}
