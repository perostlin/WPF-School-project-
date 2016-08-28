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
using MessageBox = System.Windows.MessageBox;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for CalculateWindow.xaml
    /// </summary>
    public partial class CalculateWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public CalculateWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillVehicleCombobox().Wait();
        }
        #endregion

        #region Methods

        #region FillVehicleComboBox
        private async Task FillVehicleCombobox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/vehicle/getallvehicles").Result;

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

        private void cmbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Rensar alla textboxar.
            tbFuelConsumption.Text = string.Empty;
            tbTotalFuelAmount.Text = string.Empty;
            tbMonthlyFuelConsumption.Text = string.Empty;
            tbMonthlyTotalFuelAmount.Text = string.Empty;
            tbYearhlyFuelConsumption.Text = string.Empty;
            tbYearlyTotalFuelAmount.Text = string.Empty;

            // Räknar ut senaste körjournalen på valt fordon.
            CalulateFuelConsumptionByVehicleID().Wait();
            CalulateFuelConsumptionSinceTheBeginning().Wait();
        }

        private void dtpYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpYearMonthDay.SelectedDate.Value.Year;
            int month = dtpYearMonthDay.SelectedDate.Value.Month;
            int day = dtpYearMonthDay.SelectedDate.Value.Day;

            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;

            if (selectedVehicle != null)
            {
                CalculateMontlyFuelConsumption(year, month).Wait();
                CalculateYearlyFuelConsumption(year).Wait();
                CalculateTotalVehicleCost(year, month).Wait();
            }
            else
            {
                MessageBox.Show("Du måste välja ett fordon som du vill visa informationen om.", "Glömt fordon");
            }
        }

        public async Task CalulateFuelConsumptionByVehicleID()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            Guid vehicleID = selectedVehicle.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionbyvehicleid", vehicleID).Result;

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbFuelConsumption.Text = string.Empty;
                tbTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbFuelConsumption.Text = string.Empty;
                tbTotalFuelAmount.Text = string.Empty;
                
                MessageBox.Show("Hittade inga körjournaler på valt fordon.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2");
                tbTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
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

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbMonthlyFuelConsumption.Text = string.Empty;
                tbMonthlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbMonthlyFuelConsumption.Text = string.Empty;
                tbMonthlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Fanns inga körjournaler på vald månad.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbMonthlyFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbMonthlyTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
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

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbYearhlyFuelConsumption.Text = string.Empty;
                tbYearlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbYearhlyFuelConsumption.Text = string.Empty;
                tbYearlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Fanns inga körjournaler på valt år.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbYearhlyFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbYearlyTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
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

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbBeginningFuelConsumption.Text = string.Empty;
                tbBeginningTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbBeginningFuelConsumption.Text = string.Empty;
                tbBeginningTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Hittade inga körjournaler på valt fordon.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalFuelConsumption = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

                tbBeginningFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbBeginningTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateTotalVehicleCost(int year, int month)
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

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatetotalvehiclecost", values).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                tbMonthlyTotalVehicleCost.Text = string.Empty;
                tbYearlyTotalVehicleCost.Text = string.Empty;

                MessageBox.Show("Det fanns inga körjournaler att räkna på.", "Inga körjournaler");
            }
            else if (response.IsSuccessStatusCode)
            {
                var totalVehicleCost = await response.Content.ReadAsStringAsync();
                CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalVehicleCost);

                tbMonthlyTotalVehicleCost.Text = returnedValue.TotalMonthlyVehicleCost.ToString("c");
                tbYearlyTotalVehicleCost.Text = returnedValue.TotalYearlyVehicleCost.ToString("c");
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

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
        #endregion

        //public async Task FillDriverJournalCombobox()
        //{
        //    HttpClient client = new HttpClient();
        //    //client.BaseAddress = new Uri("http://htaxiapi.azurewebsites.net/");
        //    client.BaseAddress = new Uri("http://localhost:15731/");

        //    client.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));

        //    // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
        //    VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;

        //    HttpResponseMessage response = client.PostAsJsonAsync("api/reportdriverjournal/filldriverjournalbyvehicleid", selectedVehicle).Result;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var driverJournals = await response.Content.ReadAsStringAsync();
        //        List<ReportDriverJournalModel> returnedDriverJournalList = JsonConvert.DeserializeObject<List<ReportDriverJournalModel>>(driverJournals);

        //        lboxDriverJournal.Items.Clear();

        //        foreach (var driverJournal in returnedDriverJournalList)
        //        {
        //            lboxDriverJournal.Items.Add(new ReportDriverJournalModel()
        //            {
        //                ID = driverJournal.ID,
        //                Date = driverJournal.Date,
        //                Milage = driverJournal.Milage,
        //                PricePerUnit = driverJournal.PricePerUnit,
        //                FuelAmount = driverJournal.FuelAmount,
        //                TotalPrice = driverJournal.TotalPrice,
        //                ChauffeurID = driverJournal.ChauffeurID,
        //                FuelTypeID = driverJournal.FuelTypeID,
        //                VehicleID = driverJournal.VehicleID
        //            });
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
        //    }
        //}

        #endregion
    }
}
