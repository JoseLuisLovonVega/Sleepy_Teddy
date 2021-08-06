using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SleepyTeddy.Droid.Services
{
    public class FirestoreService
    {
        private static Firebase.FirebaseApp app;
        public static FirebaseFirestore Instance
        {
            get
            {
                return FirebaseFirestore.GetInstance(app);
            }
        }
        public static string AppName { get; } = "SleepyTeddy.Android";
        public static void Init(Android.Content.Context context)
        {
            var baseOptions = Firebase.FirebaseOptions.FromResource(context);
            var options = new Firebase.FirebaseOptions.Builder(baseOptions).SetProjectId(baseOptions.StorageBucket.Split('.')[0]).Build();
            app = Firebase.FirebaseApp.InitializeApp(context, options, AppName);
            if (app != null)
            {

                var temp = FirebaseFirestore.GetInstance(app);
            }
        }
    }
}