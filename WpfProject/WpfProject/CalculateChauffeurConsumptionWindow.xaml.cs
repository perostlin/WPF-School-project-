using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for CalculateChauffeurConsumptionWindow.xaml
    /// </summary>
    public partial class CalculateChauffeurConsumptionWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public CalculateChauffeurConsumptionWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            // Körs när sidan laddas.
            FillVehicleCombobox().Wait();
            CalculateFuelConsumptionByChauffeurID().Wait();
            CalulateFuelConsumptionSinceTheBeginningByChauffeurID().Wait();
        }
        #endregion

        #region Methods

        #region FillVehicleComboBox
        private async Task FillVehicleCombobox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.PostAsJsonAsync("api/vehicle/getallvehiclesbychauffeurid", _user.ID).Result;

            if (response.IsSuccessStatusCode)
            {
                var vehicles = await response.Content.ReadAsStringAsync();
                List<VehicleModel> returnedUserList = JsonConvert.DeserializeObject<List<VehicleModel>>(vehicles);

                cmbVehicle.Items.Clear();

                List<VehicleModel> vehicleListToShow = new List<VehicleModel>();

                foreach (var vehicle in returnedUserList)
                {
                    vehicleListToShow.Add(new VehicleModel() { ID = vehicle.ID, RegNo = vehicle.RegNo });
                }

                cmbVehicle.ItemsSource = vehicleListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillVehicleCombobox
        
        #region Chauffeur
        private void dtpYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpYearMonthDay.SelectedDate.Value.Year;
            int month = dtpYearMonthDay.SelectedDate.Value.Month;
            int day = dtpYearMonthDay.SelectedDate.Value.Day;

            UserModel selectedUser = _user;

            if (selectedUser != null)
            {
                CalculateMontlyFuelConsumptionByChauffeurID(year, month).Wait();
                CalculateYearlyFuelConsumptionByChauffeurID(year).Wait();
            }
            else
            {
                MessageBox.Show("Du måste välja en chaufför som du vill visa informationen om.", "Glömt chauförr");
            }
        }
        public async Task CalculateFuelConsumptionByChauffeurID()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt UserModel-objekt samt lägger till värdena.
            Guid chauffeurID = _user.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionbychauffeurid", chauffeurID).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inga körjournaler på inloggad chaufför.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbLatestFuelConsumption.Text = returnedValue.AverageFuelConsumption.ToString("n2") + " liter";
                tbLatestTotalFuelAmount.Text = returnedValue.AverageFuelCost.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateMontlyFuelConsumptionByChauffeurID(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt UserModel-objekt samt lägger till värdena.
            UserModel selectedUser = _user;
            Guid userID = selectedUser.ID;


            CalculateValuesModel values = new CalculateValuesModel()
            {
                ChauffeurID = userID,
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatemonthlyfuelconsumptionbychauffeurid", values).Result;

            if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbMonthlyFuelConsumption.Text = returnedValue.AverageFuelConsumption.ToString("n2") + " liter";
                tbMonthlyTotalFuelAmount.Text = returnedValue.AverageFuelCost.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateYearlyFuelConsumptionByChauffeurID(int year)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt UserModel-objekt samt lägger till värdena.
            UserModel selectedUser = _user;
            Guid userID = selectedUser.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                ChauffeurID = userID,
                Year = year
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulateyearlyfuelconsumptionbychauffeurid", values).Result;

            if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbYearhlyFuelConsumption.Text = returnedValue.AverageFuelConsumption.ToString("n2") + " liter";
                tbYearlyTotalFuelAmount.Text = returnedValue.AverageFuelCost.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalulateFuelConsumptionSinceTheBeginningByChauffeurID()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            UserModel selectedUser = _user;
            Guid userID = selectedUser.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionsincethebeginningbychauffeurid", userID).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inga körjournaler på valt fordon.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbBeginningFuelConsumption.Text = returnedValue.AverageFuelConsumption.ToString("n2") + " liter";
                tbBeginningTotalFuelAmount.Text = returnedValue.AverageFuelCost.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }
        #endregion
        
        #region Vehicle
        private void dtpVehicleYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpVehicleYearMonthDay.SelectedDate.Value.Year;
            int month = dtpVehicleYearMonthDay.SelectedDate.Value.Month;
            int day = dtpVehicleYearMonthDay.SelectedDate.Value.Day;

            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;

            if (selectedVehicle != null)
            {
                CalculateMontlyFuelConsumption(year, month).Wait();
                CalculateYearlyFuelConsumption(year).Wait();
            }
            else
            {
                MessageBox.Show("Du måste välja ett fordon som du vill visa informationen om.", "Glömt fordon");
            }
        }

        private void cmbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Räknar ut senaste körjournalen på valt fordon.
            CalulateFuelConsumptionByVehicleID().Wait();
            CalulateFuelConsumptionSinceTheBeginning().Wait();
        }
        public async Task CalulateFuelConsumptionByVehicleID()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            Guid vehicleID = selectedVehicle.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionbyvehicleid", vehicleID).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbLatestVehicleFuelConsumption.Text = string.Empty;
                tbLatestVehicleTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Hittade inga körjournaler på valt fordon.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbLatestVehicleFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbLatestVehicleTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateMontlyFuelConsumption(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            Guid vehicleID = selectedVehicle.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                VehicleID = vehicleID,
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatemonthlyfuelconsumption", values).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbMonthlyAverageFuelConsumption.Text = string.Empty;
                tbMonthlyAverageTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Fanns inga körjournaler på vald månad.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbMonthlyAverageFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbMonthlyAverageTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateYearlyFuelConsumption(int year)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            Guid vehicleID = selectedVehicle.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                VehicleID = vehicleID,
                Year = year
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulateyearlyfuelconsumption", values).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbYearlyAverageFuelConsumption.Text = string.Empty;
                tbYearlyAverageTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Fanns inga körjournaler på valt år.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbYearlyAverageFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbYearlyAverageTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalulateFuelConsumptionSinceTheBeginning()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            Guid vehicleID = selectedVehicle.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionsincethebeginning", vehicleID).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbBeginningAverageFuelConsumption.Text = string.Empty;
                tbBeginningAverageTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Hittade inga körjournaler på valt fordon.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbBeginningAverageFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbBeginningAverageTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }
        #endregion

        #region Makes DatePicker start at Decade
        private void dtpYearMonthDay_CalendarOpened(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker != null)
            {
                var popup = datepicker.Template.FindName(
                    "PART_Popup", datepicker) as Popup;
                if (popup != null && popup.Child is Calendar)
                {
                    ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
                }
            }
        }

        private void dtpVehicleYearMonthDay_CalendarOpened(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker != null)
            {
                var popup = datepicker.Template.FindName(
                    "PART_Popup", datepicker) as Popup;
                if (popup != null && popup.Child is Calendar)
                {
                    ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
                }
            }
        }
        #endregion

        #endregion
    }
}
