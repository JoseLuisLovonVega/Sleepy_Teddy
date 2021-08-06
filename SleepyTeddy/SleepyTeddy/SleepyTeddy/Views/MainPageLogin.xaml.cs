using Acr.UserDialogs;
using Firebase.Auth;
using Newtonsoft.Json;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Views.AdministratorViews;
using SleepyTeddy.Views.TherapistViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SleepyTeddy.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPageLogin : ContentPage
    {
        LoginViewModel loginViewModel;
        public string WebAPIkey = "AIzaSyCn23mEfbZVvw9Y7RTuCqmhpI5fcwQRL5o";
        public MainPageLogin()
        {
            loginViewModel = new LoginViewModel(); 
            InitializeComponent();
            BindingContext = loginViewModel;

        }
        protected override bool OnBackButtonPressed()
        {
            // Begin an asyncronous task on the UI thread because we intend to ask the users permission.
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("¿Cerrar aplicación?", "Está seguro que desea cerrar la aplicación", "SÍ", "NO"))
                {
                    base.OnBackButtonPressed();

                    System.Environment.Exit(0);
                }
            });

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
        /*async void LoginUser(System.Object sender, System.EventArgs e)
        {
            //null or empty field validation, check weather email and password is null or empty    

            if (string.IsNullOrEmpty(UserLoginEmail.Text) || string.IsNullOrEmpty(UserLoginPassword.Text))
                await App.Current.MainPage.DisplayAlert("Empty Values", "Please enter Email and Password", "OK");
            else
            {
                //call GetUser function which we define in Firebase helper class    
                var user = await FirebaseHelperLogIn.GetUser(UserLoginEmail.Text);
                //firebase return null valuse if user data not found in database    
                if (user != null)
                    if (UserLoginEmail.Text == user.Email && UserLoginPassword.Text == user.Password)
                    {
                        await App.Current.MainPage.DisplayAlert("Login Success", "", "Ok");
                        //Navigate to Wellcom page after successfuly login    
                        //pass user email to welcom page    
                        await App.Current.MainPage.Navigation.PushAsync(new AsignarCuestionarioPaciente());
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Login Fail", "Please enter correct Email and Password", "OK");
                else
                    await App.Current.MainPage.DisplayAlert("Login Fail", "User not found", "OK");
            }
        }


        async void loginbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserLoginEmail.Text, UserLoginPassword.Text);
                var content = await auth.GetFreshAuthAsync();
                var serializedcontnet = JsonConvert.SerializeObject(content);
                Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);
                using (var dialog = UserDialogs.Instance.Progress("Procesando"))
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        await Task.Delay(100);
                        dialog.PercentComplete = i * 10;
                    }
                }
                await Navigation.PushAsync(new AsignarPacienteTerapeuta());
                NavigationPage.SetHasBackButton(this, false);
                //Acr.UserDialogs.UserDialogs.Instance.Toast("Inicio de sesión correcto", new TimeSpan(3));
                //CrossToastPopUp.Current.ShowToastMessage("Inicio de sesión correcto");
                //UserDialogs.Instance.ShowLoading("Correcto");

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", "Usuario o contraseña incorrecto", "OK");
                await Navigation.PushAsync(new AsignarPacienteTerapeuta());
            }
        }
        async void singupbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegAdministrador());
        }
        */
    }
}