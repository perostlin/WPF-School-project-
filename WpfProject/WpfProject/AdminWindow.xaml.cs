using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using WpfProject.Classes;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public AdminWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillUserListBox().Wait();

            FillVehicleListBox().Wait();

            GetBestChauffeur().Wait();

            GetBestVehicle().Wait();
        }
        #endregion

        #region UserTab

        #region FillUserListBox
        private async Task FillUserListBox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/user/getallusers").Result;

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadAsStringAsync();
                List<UserModel> returnedUserList = JsonConvert.DeserializeObject<List<UserModel>>(users);

                lboxUsers.Items.Clear();

                List<UserModel> userListToShow = new List<UserModel>();

                foreach (var user in returnedUserList)
                {
                    userListToShow.Add(new UserModel() { ID = user.ID, Username = user.Username });
                }

                lboxUsers.ItemsSource = userListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillUserListBox

        #region GetSelectedUser
        private void lboxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectedUser().Wait();
        }

        private async Task GetSelectedUser()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            UserModel selectedUser = lboxUsers.SelectedItem as UserModel;

            HttpResponseMessage response = client.PostAsJsonAsync("api/user/getselecteduser", selectedUser).Result;

            if (response.IsSuccessStatusCode)
            {
                var user = response.Content.ReadAsAsync<UserModel>().Result;

                tbUser_Username.Text = user.Username;

                if (user.IsAdmin)
                {
                    cboxUser_User.IsChecked = false;
                    cboxUser_Admin.IsChecked = true;
                }
                else
                {
                    cboxUser_Admin.IsChecked = false;
                    cboxUser_User.IsChecked = true;
                }
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade ingen användare på vald användare i listan.", "Fel uppstod");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion GetSelectedUser

        #region RegisterButton
        private void btnRegisterWindow_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(_user);
            registerWindow.Show();
        }
        #endregion RegisterButton    

        #region UpdateUser ONGOING
        //private void btnUser_Update_Click(object sender, RoutedEventArgs e)
        //{
        //    // Sätter om synligheten på de olika kontrollerna.
        //    btnUser_Update.Visibility = Visibility.Hidden;
        //    btnUser_Save.Visibility = Visibility.Visible;
        //    lbUser_CurrentPassword.Visibility = Visibility.Visible;
        //    lbUser_NewPassword.Visibility = Visibility.Visible;
        //    tbUser_CurrentPassword.Visibility = Visibility.Visible;
        //    tbUser_NewPassword.Visibility = Visibility.Visible;
        //}
        #endregion

        #endregion UserTab

        #region VehicleTab

        #region FillVehicleListBox
        private async Task FillVehicleListBox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/vehicle/getallvehicles").Result;

            if (response.IsSuccessStatusCode)
            {
                var vehicles = await response.Content.ReadAsStringAsync();
                List<VehicleModel> returnedUserList = JsonConvert.DeserializeObject<List<VehicleModel>>(vehicles);

                lboxVehicles.Items.Clear();

                List<VehicleModel> vehicleListToShow = new List<VehicleModel>();

                foreach (var vehicle in returnedUserList)
                {
                    vehicleListToShow.Add(new VehicleModel() { ID = vehicle.ID, RegNo = vehicle.RegNo });
                }

                lboxVehicles.ItemsSource = vehicleListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillVehicleListBox

        #region GetSelectedVehicle
        private void lboxVehicles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectedVehicle().Wait();
        }

        private async Task GetSelectedVehicle()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            VehicleModel selectedVehicle = lboxVehicles.SelectedItem as VehicleModel;

            HttpResponseMessage response = client.PostAsJsonAsync("api/vehicle/getselectedvehicle", selectedVehicle).Result;

            if (response.IsSuccessStatusCode)
            {
                var vehicle = response.Content.ReadAsAsync<VehicleModel>().Result;

                tbVehicle_RegNo.Text = vehicle.RegNo;
                tbVehicle_Description.Text = vehicle.Description;
                tbVehicle_OriginalMilage.Text = vehicle.OriginalMilage.ToString();
                tbVehicle_Color.Text = vehicle.Color;
                tbVehicle_FuelType.Text = vehicle.FuelType;
                tbVehicle_ModelYear.Text = vehicle.ModelYear.ToString();
                tbVehicle_VehicleType.Text = vehicle.VehicleType;
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inget fordon på valt fordon i listan.", "Fel uppstod");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion GetSelectedVehicle

        #region AddVehicle
        private void btnAddVehicle_Click(object sender, RoutedEventArgs e)
        {
            VehicleWindow vehicleWindow = new VehicleWindow(_user);
            vehicleWindow.Show();
        }
        #endregion AddVehicle

        #endregion VehicleTab
        
        #region DriverJournalTab

        #region AddDriverJournal
        private void btnAddDriverJournal_Click(object sender, RoutedEventArgs e)
        {
            AddDriverJournal addDriverJournalWindow = new AddDriverJournal(_user);
            addDriverJournalWindow.Show();
        }
        #endregion AddDriverJournal

        #endregion DriverJournalTab

        #region OtherCostsTab
        private void btnAddOtherCost_Click(object sender, RoutedEventArgs e)
        {
            OtherCost otherCostWindow = new OtherCost(_user);
            otherCostWindow.Show();
        }
        #endregion OtherCostTab

        #region Calculate
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            CalculateWindow calculateWindow = new CalculateWindow(_user);
            calculateWindow.Show();
        }

        private void btnCalculateOnChauffeur_Click(object sender, RoutedEventArgs e)
        {
            CalculateChauffeurWindow chauffeurWindow = new CalculateChauffeurWindow(_user);
            chauffeurWindow.Show();
        }

        private void btnCalculateOnVehicleType_Click(object sender, RoutedEventArgs e)
        {
            CalculateVehicleTypeWindow vehicleTypeWindow = new CalculateVehicleTypeWindow(_user);
            vehicleTypeWindow.Show();
        }

        private void btnCalculateOnAllVehicles_Click(object sender, RoutedEventArgs e)
        {
            CalculateAllVehicleWindow allVehicleWindow = new CalculateAllVehicleWindow(_user);
            allVehicleWindow.Show();
        }

        private void btnDeeperAnalysis_Click(object sender, RoutedEventArgs e)
        {
            DeeperAnalysisWindow deeperAnalysis = new DeeperAnalysisWindow(_user);
            deeperAnalysis.Show();
        }
        #endregion

        #region Best
        #region Chauffeur
        private async Task GetBestChauffeur()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/calulate/calulatebestchauffeurfuelconsumption").Result;


            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                // Do nothing...
            }
            else if (response.IsSuccessStatusCode)
            {
                var chauffeur = response.Content.ReadAsAsync<BestValueModel>().Result;

                lbBestChauffeurName.Content = "Chaufförens namn: " + chauffeur.Name;
                lbBestChauffeurFuelConsumption.Content = "Bränsleförbrukningen: " + chauffeur.FuelConsumption.ToString("n2");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }
        #endregion

        #region Vehicle
        private async Task GetBestVehicle()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/calulate/calulatebestvehiclefuelconsumption").Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                // Do nothing...
            }
            else if (response.IsSuccessStatusCode)
            {
                var vehicle = response.Content.ReadAsAsync<BestValueModel>().Result;

                lbBestVehicleName.Content = "Fordonets namn: " + vehicle.Name;
                lbBestVehicleFuelConsumption.Content = "Bränsleförbrukningen: " + vehicle.FuelConsumption.ToString("n2");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }
        #endregion

        #endregion

        #region Update Api-address in App.config file.
        private void btnChangeAppConfig_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbApiAddress.Text))
            {
                var response = MessageBox.Show("Är du säker på att du vill ändra den nuvarande api-adressen?", "Är du säker?",
                    MessageBoxButton.YesNo);

                if (response == MessageBoxResult.Yes)
                {

                    AppConfigChanger.UpdateConfigKey("ApiAddress", tbApiAddress.Text);
                    tbApiAddress.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Vänligen skriv in något i text-rutan.", "Får ej vara tomt");
            }
        }
        #endregion

        #region UpdateList methods (don't work...)
        private void btnVehicle_UpdateList_Click(object sender, RoutedEventArgs e)
        {
            lboxVehicles.Items.Refresh();
        }

        private void btnUser_UpdateList_Click(object sender, RoutedEventArgs e)
        {
            lboxUsers.Items.Refresh();
        }
        #endregion UpdateList methods

    }
}
