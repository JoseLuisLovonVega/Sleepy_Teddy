using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.CloudFirestore;
using SleepyTeddy.ViewModel;
using SleepyTeddy.Models;
using System.Text.RegularExpressions;
using FormsControls.Base;
//using WindesHeartApp.Pages;
using SleepyTeddy.Services;
using SleepyTeddy.Resources;

namespace SleepyTeddy.Views.PatientViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Wearable : ContentPage, IAnimationPage
    {
        //DevicePageViewModel devicePageViewModel;
        public static ListView Devicelist;
        public static Button ScanButton;
        Patient patient;
        string documentId;
        string Patient_ID=LoginViewModel.Patient_ID;
        
        public Wearable()
        {
            InitializeComponent(); 
            //getPatient();
            //devicePageViewModel = new DevicePageViewModel();
            //BindingContext = devicePageViewModel;
            BuildPage();
        }

        protected override void OnAppearing()
        {
            App.RequestLocationPermission();
        }
        private async void getPatient()
        {
            String role_id = "2";
            var document = await CrossCloudFirestore.Current
                                       .Instance
                                       .Collection("Users")
                                       .WhereEqualsTo("Role_ID", role_id)
                                       .WhereEqualsTo("Patient_ID", Patient_ID)
                                       .GetAsync();
            patient = document.Documents.ElementAt(0).ToObject<Patient>();
            documentId = document.Documents.ElementAt(0).Id;
        }

        private void BuildPage()
        {
            absoluteLayout = new AbsoluteLayout();
            PageBuilder.BuildPageBasics(absoluteLayout, this);
            PageBuilder.AddLabel(absoluteLayout, "Wearables Encontrados", 0.50, 0.05, Color.Black, "", 30);

            ScanButton = PageBuilder.AddButton(absoluteLayout, "", Globals.DevicePageViewModel.ScanButtonClicked, 0.15, 0.25, 120, 50, 14, 12, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#0AB8AA"));
            ScanButton.SetBinding(Button.TextProperty, "ScanButtonText");
            PageBuilder.AddActivityIndicator(absoluteLayout, "IsLoading", 0.50, 0.25, 50, 50, AbsoluteLayoutFlags.PositionProportional, Color.Black);
            PageBuilder.AddActivityIndicator(absoluteLayout, "IsLoading", 0.50, 0.25, 50, 50, AbsoluteLayoutFlags.PositionProportional, Color.Black);
            PageBuilder.AddLabel(absoluteLayout, "", 0.80, 0.25, Color.Black, "StatusText", 14);

            #region device ListView
            var deviceTemplate = new DataTemplate(() =>
            {
                Grid grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition {Width = (int) Application .Current.MainPage.Width / 100 * 33},
                        new ColumnDefinition {Width = (int) Application .Current.MainPage.Width / 100 * 33},
                        new ColumnDefinition {Width = (int) Application .Current.MainPage.Width / 100 * 33},
                    }
                };

                Label label = new Label { FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                label.SetBinding(Label.TextProperty, "Device.Name");
                grid.Children.Add(label, 0, 0);


                Label label2 = new Label { VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Italic, FontSize = 12 };
                label2.SetBinding(Label.TextProperty, "Rssi");

                grid.Children.Add(label2, 2, 0);

                Label label3 = new Label { Text = "Signal strength:", VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                grid.Children.Add(label3, 1, 0);

                return new ViewCell { View = grid };
            });
            Devicelist = new ListView { BackgroundColor = Color.FromHex("#EDF5F7"), ItemTemplate = deviceTemplate };
            Devicelist.SetBinding(ListView.SelectedItemProperty, new Binding("SelectedDevice", BindingMode.TwoWay));
            Devicelist.SetBinding(ListView.ItemsSourceProperty, new Binding("DeviceList"));
            AbsoluteLayout.SetLayoutBounds(Devicelist, new Rectangle(0.5, 0.55, 0.90, 0.4));
            AbsoluteLayout.SetLayoutFlags(Devicelist, AbsoluteLayoutFlags.All);
            absoluteLayout.Children.Add(Devicelist);
            #endregion

            #region disconnectButton
            Button disconnectButton = PageBuilder.AddButton(absoluteLayout, "Desconectar", Globals.DevicePageViewModel.DisconnectButtonClicked, 0.50, 0.85, 120, 50, 14, 12, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#0AB8AA"));
            #endregion

            #region cancelButton
            Button cancelButton = PageBuilder.AddButton(absoluteLayout, "Cancelar", btnCancelar_clicked, 0.50, 0.95, 120, 50, 14, 12, AbsoluteLayoutFlags.PositionProportional, Color.FromHex("#0AB8AA"));
            #endregion
        }

        protected override void OnDisappearing()
        {
            Globals.DevicePageViewModel.OnDisappearing();
        }

        public IPageAnimation PageAnimation { get; } = new SlidePageAnimation { Duration = AnimationDuration.Short, Subtype = AnimationSubtype.FromLeft };

        public void OnAnimationStarted(bool isPopAnimation)
        {
            // Put your code here but leaving empty works just fine
        }

        public void OnAnimationFinished(bool isPopAnimation)
        {
            // Put your code here but leaving empty works just fine
        }

        /*private void btnScan_Clicked(object sender, EventArgs e)
        {

           devicePageViewModel.ScanButtonClicked(sender, EventArgs.Empty);

        }
        private void list_wearables_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
        private void btnDisconnect_Clicked(object sender, EventArgs e)
        {

        }*/
        private async void btnCancelar_clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}