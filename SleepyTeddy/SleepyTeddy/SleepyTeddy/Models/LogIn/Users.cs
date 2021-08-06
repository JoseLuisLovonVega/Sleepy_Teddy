using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SleepyTeddy.Models.LogIn
{
    public class Users
    {
        [JsonProperty("User_ID")]
        public string User_ID { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("Role_ID")]
        public string Role_ID { get; set; }
        public string Administrator_ID { get; set; }
        [JsonProperty("Names")]
        public string Names { get; set; }
        [JsonProperty("Last_Names")]
        public string Last_Names { get; set; }
        [JsonProperty("Therapist_ID")]
        public string Therapist_ID { get; set; }
        [JsonProperty("Patient_ID")]
        public string Patient_ID { get; set; }

    }
}
