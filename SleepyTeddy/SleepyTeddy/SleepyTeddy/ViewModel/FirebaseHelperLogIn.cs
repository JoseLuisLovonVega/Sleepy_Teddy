using Firebase.Database;
using SleepyTeddy.Models.LogIn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Plugin.CloudFirestore;

namespace SleepyTeddy.ViewModel
{
    class FirebaseHelperLogIn
    {
        public static FirebaseClient firebase = new FirebaseClient("https://sleepy-teddy-tp-default-rtdb.firebaseio.com/");

        //Read All    
        public static async Task<List<Users>> GetAllUser()
        {
            /*try
            {
                var userlist = (await firebase
                    .Child("User")
                    .OnceAsync<Users>()).Select(item =>
                new Users
                {
                    Email = item.Object.Email,
                    Password = item.Object.Password
                }).ToList();
                return userlist;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return null;
            }*/
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .GetAsync();

            var resModel = document.ToObjects<Users>().ToList();
            return resModel;

        }
        //Read     
        public static async Task<Users> GetUser(string email)
        {
            try
            {
                var allUsers = await GetAllUser();

                return allUsers.Where(a => a.Email == email).FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error:{e}");
                return null;
            }
        }
    }
}
