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
    /// Interaction logic for CalculateVehicleTypeWindow.xaml
    /// </summary>
    public partial class CalculateVehicleTypeWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public CalculateVehicleTypeWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillVehicleTypeCombobox().Wait();
        }
        #endregion

        #region Methods

        #region FillVehicleTypeComboBox
        private async Task FillVehicleTypeCombobox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/vehicle/getallvehicletypes").Result;

            if (response.IsSuccessStatusCode)
            {
                var vehicles = await response.Content.ReadAsStringAsync();
                List<VehicleTypeModel> returnedUserList = JsonConvert.DeserializeObject<List<VehicleTypeModel>>(vehicles);

                cmbVehicleType.Items.Clear();

                List<VehicleTypeModel> vehicleListToShow = new List<VehicleTypeModel>();

                foreach (var vehicle in returnedUserList)
                {
                    vehicleListToShow.Add(new VehicleTypeModel() { ID = vehicle.ID, Type = vehicle.Type });
                }

                cmbVehicleType.ItemsSource = vehicleListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillVehicleTypeCombobox

        private void cmbVehicleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Rensar alla textboxar.
            tbMonthlyFuelConsumption.Text = string.Empty;
            tbMonthlyTotalFuelAmount.Text = string.Empty;
            tbYearhlyFuelConsumption.Text = string.Empty;
            tbYearlyTotalFuelAmount.Text = string.Empty;
            tbBeginningFuelConsumption.Text = string.Empty;
            tbBeginningTotalFuelAmount.Text = string.Empty;

            // Räknar ut senaste körjournalen på valt fordon.
            //CalulateFuelConsumptionByVehicleType().Wait();
            CalulateFuelConsumptionSinceTheBeginningByVehicleType().Wait();
        }

        private void dtpYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpYearMonthDay.SelectedDate.Value.Year;
            int month = dtpYearMonthDay.SelectedDate.Value.Month;
            int day = dtpYearMonthDay.SelectedDate.Value.Day;

            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;

            if (selectedVehicleType != null)
            {
                CalculateMontlyFuelConsumptionByVehicleType(year, month).Wait();
                CalculateYearlyFuelConsumptionByVehicleType(year).Wait();
                CalculateTotalVehicleCostByVehicleType(year, month).Wait();
            }
            else
            {
                MessageBox.Show("Du måste välja en fordonstyp som du vill visa informationen om.", "Glömt fordonstyp");
            }
        }

        public async Task CalculateMontlyFuelConsumptionByVehicleType(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;
            int vehicleTypeID = selectedVehicleType.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                VehicleTypeID = vehicleTypeID,
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatemonthlyfuelconsumptionbyvehicletypeid", values).Result;
            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbMonthlyFuelConsumption.Text = string.Empty;
                tbMonthlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inga körjournaler på vald fordonstyp.", "Inga körjournaler");
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

        public async Task CalculateYearlyFuelConsumptionByVehicleType(int year)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;
            int vehicleTypeID = selectedVehicleType.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                VehicleTypeID = vehicleTypeID,
                Year = year
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulateyearlyfuelconsumptionbyvehicletypeid", values).Result;

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbYearhlyFuelConsumption.Text = string.Empty;
                tbYearlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inga körjournaler på vald fordonstyp.", "Inga körjournaler");
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

        public async Task CalulateFuelConsumptionSinceTheBeginningByVehicleType()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;
            int vehicleID = selectedVehicleType.ID;

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionsincethebeginningbyvehicletypeid", vehicleID).Result;

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbBeginningFuelConsumption.Text = string.Empty;
                tbBeginningTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Hittade inga körjournaler på vald fordonstyp.", "Inga körjournaler");
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

        public async Task CalculateTotalVehicleCostByVehicleType(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;
            int vehicleTypeID = selectedVehicleType.ID;

            CalculateValuesModel values = new CalculateValuesModel()
            {
                VehicleTypeID = vehicleTypeID,
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatetotalvehiclecostbyvehicletypeid", values).Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Det fanns inga kostnader att räkna på.", "Inga kostnader/korjournaler");
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

        #region TABORT
        //public async Task CalulateFuelConsumptionByVehicleType()
        //{
        //    HttpClient client = new HttpClient();
        //    //client.BaseAddress = new Uri("http://htaxiapi.azurewebsites.net/");
        //    client.BaseAddress = new Uri("http://localhost:15731/");

        //    client.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));

        //    // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
        //    VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;
        //    int vehicleTypeID = selectedVehicleType.ID;

        //    HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatefuelconsumptionbyvehicletypeid", vehicleTypeID).Result;

        //    if (response.StatusCode == HttpStatusCode.NoContent)
        //    {
        //        MessageBox.Show("Hittade inga körjournaler på vald fordonstyp.", "Inga körjournaler");
        //    }
        //    else if (response.IsSuccessStatusCode)
        //    {
        //        var totalFuelConsumption = await response.Content.ReadAsStringAsync();
        //        CalculateValuesModel returnedValue = JsonConvert.DeserializeObject<CalculateValuesModel>(totalFuelConsumption);

        //        tbFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2");
        //        tbTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("n2");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
        //    }
        //}
        #endregion

        #endregion
    }
}
