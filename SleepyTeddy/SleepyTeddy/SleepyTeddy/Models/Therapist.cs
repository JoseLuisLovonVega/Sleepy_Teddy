using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SleepyTeddy.Models
{
    public class Therapist
    {
        public string Therapist_ID { get; set; }
        public string administrator_ID { get; set; }
        public string Role_ID { get; set; }
        public string Names { get; set; }
        public string Last_Names { get; set; }
        public string Especiality { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}