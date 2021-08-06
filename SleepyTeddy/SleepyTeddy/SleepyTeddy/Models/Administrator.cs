using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SleepyTeddy.Models
{
    public class Administrator
    {
        public string Administrator_ID { get; set; }
        public string Names { get; set; }
        public string Last_Names { get; set; }
        public string Email { get; set; }
        public string Role_ID { get; set; }
        public string Institution { get; set; }
        public string Password { get; set; }
    }
}