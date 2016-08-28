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
    /// Interaction logic for CalculateAllVehicleWindow.xaml
    /// </summary>
    public partial class CalculateAllVehicleWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public CalculateAllVehicleWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            // Körs när sidan laddas.
            CalulateFuelConsumptionSinceTheBeginningByAllVehicles().Wait();
        }
        #endregion

        #region Methods
        private void dtpYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpYearMonthDay.SelectedDate.Value.Year;
            int month = dtpYearMonthDay.SelectedDate.Value.Month;
            int day = dtpYearMonthDay.SelectedDate.Value.Day;

            CalculateMontlyFuelConsumptionByAllVehicles(year, month).Wait();
            CalculateYearlyFuelConsumptionByAllVehicles(year).Wait();
            CalculateTotalVehicleCostByAllVehicles(year, month).Wait();
        }

        public async Task CalculateMontlyFuelConsumptionByAllVehicles(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            CalculateValuesModel values = new CalculateValuesModel()
            {
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatemonthlyfuelconsumptionbyallvehicles", values).Result;
            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbMonthlyFuelConsumption.Text = string.Empty;
                tbMonthlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Måste finnas fler än 1 körjournal för att räkna ut månadsförbrukning",
                    "För få körjournaler");
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

        public async Task CalculateYearlyFuelConsumptionByAllVehicles(int year)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            CalculateValuesModel values = new CalculateValuesModel()
            {
                Year = year
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulateyearlyfuelconsumptionbyallvehicles", values).Result;

            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                tbYearhlyFuelConsumption.Text = string.Empty;
                tbYearlyTotalFuelAmount.Text = string.Empty;

                MessageBox.Show("Gick inte räkna ut då fordonet du försöker räkna på är ett El-fordon.", "Fel vid uträkning");
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show("Måste finnas fler än 1 körjournal för att räkna ut årsförbrukning",
                    "För få körjournaler");
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

        public async Task CalulateFuelConsumptionSinceTheBeginningByAllVehicles()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/calulate/calulatefuelconsumptionsincethebeginningbyallvehicles").Result;

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

        public async Task CalculateTotalVehicleCostByAllVehicles(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            CalculateValuesModel values = new CalculateValuesModel()
            {
                Year = year,
                Month = month
            };

            HttpResponseMessage response = client.PostAsJsonAsync("api/calulate/calulatetotalvehiclecostbyallvehicles", values).Result;

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

        #endregion
    }
}
