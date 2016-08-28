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
    /// Interaction logic for CalculateChauffeurWindow.xaml
    /// </summary>
    public partial class CalculateChauffeurWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public CalculateChauffeurWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillUserComboBox().Wait();
        }
        #endregion

        #region Methods

        #region FillUserComboBox
        private async Task FillUserComboBox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/user/getallusers").Result;

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadAsStringAsync();
                List<UserModel> returnedUserList = JsonConvert.DeserializeObject<List<UserModel>>(users);

                cmbUsers.Items.Clear();

                List<UserModel> userListToShow = new List<UserModel>();

                foreach (var user in returnedUserList)
                {
                    userListToShow.Add(new UserModel() { ID = user.ID, Username = user.Username });
                }

                cmbUsers.ItemsSource = userListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillUserComboBox
        
        private void cmbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Rensar alla textboxar.
            //tbFuelConsumption.Text = string.Empty;
            //tbTotalFuelAmount.Text = string.Empty;
            tbMonthlyFuelConsumption.Text = string.Empty;
            tbMonthlyTotalFuelAmount.Text = string.Empty;
            tbYearhlyFuelConsumption.Text = string.Empty;
            tbYearlyTotalFuelAmount.Text = string.Empty;
            tbBeginningFuelConsumption.Text = string.Empty;
            tbBeginningTotalFuelAmount.Text = string.Empty;

            // Räknar ut senaste körjournalen på valt fordon.
            //CalculateFuelConsumptionByChauffeurID().Wait();
            CalulateFuelConsumptionSinceTheBeginningByChauffeurID().Wait();

        }

        private void dtpYearMonthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int year = dtpYearMonthDay.SelectedDate.Value.Year;
            int month = dtpYearMonthDay.SelectedDate.Value.Month;
            int day = dtpYearMonthDay.SelectedDate.Value.Day;

            UserModel selectedUser = cmbUsers.SelectedItem as UserModel;

            if (selectedUser != null)
            {
                CalculateMontlyFuelConsumptionByChauffeurID(year, month).Wait();
                CalculateYearlyFuelConsumptionBuChauffeurID(year).Wait();
            }
            else
            {
                MessageBox.Show("Du måste välja en chaufför som du vill visa informationen om.", "Glömt chauförr");
            }
        }

        public async Task CalculateMontlyFuelConsumptionByChauffeurID(int year, int month)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt UserModel-objekt samt lägger till värdena.
            UserModel selectedUser = cmbUsers.SelectedItem as UserModel;
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

                tbMonthlyFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbMonthlyTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
                
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        public async Task CalculateYearlyFuelConsumptionBuChauffeurID(int year)
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt UserModel-objekt samt lägger till värdena.
            UserModel selectedUser = cmbUsers.SelectedItem as UserModel;
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

                tbYearhlyFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbYearlyTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
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
            UserModel selectedUser = cmbUsers.SelectedItem as UserModel;
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

                tbBeginningFuelConsumption.Text = returnedValue.AverageFuelValue.ToString("n2") + " liter";
                tbBeginningTotalFuelAmount.Text = returnedValue.TotalFuelValue.ToString("c");
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
