using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using WpfProject.Classes;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for AddChauffeurDriverJournal.xaml
    /// </summary>
    public partial class AddChauffeurDriverJournal : Window
    {
        private readonly UserModel _user;

        public AddChauffeurDriverJournal(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillVehicleCombobox().Wait();
        }

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

        #region FillFuelTypeCombobox
        private void cmbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillFuelTypeCombobox().Wait();
        }

        public async Task FillFuelTypeCombobox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt VehicleModel-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;

            HttpResponseMessage response = client.PostAsJsonAsync("api/reportdriverjournal/fillfueltypebyvehicleid", selectedVehicle).Result;

            if (response.IsSuccessStatusCode)
            {
                var fuelTypes = await response.Content.ReadAsStringAsync();
                List<FuelTypeModel> returnedFuelTypeList = JsonConvert.DeserializeObject<List<FuelTypeModel>>(fuelTypes);

                cmbFuelType.Items.Clear();
                
                foreach (var fuelType in returnedFuelTypeList)
                {
                    cmbFuelType.Items.Add(new FuelTypeModel() { ID = fuelType.ID, Type = fuelType.Type });
                }
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }

        private void cmbFuelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FuelTypeModel fuelType = (sender as ComboBox).SelectedItem as FuelTypeModel;

            if (fuelType == null || fuelType.Type != "El")
            {
                tbFuelAmount.IsReadOnly = false;
                tbFuelAmount.Background = Brushes.White;
                tbUnitPrice.IsReadOnly = false;
                tbUnitPrice.Background = Brushes.White;
                tbTotalPrice.IsReadOnly = false;
                tbTotalPrice.Background = Brushes.White;
            }
            else if (fuelType.Type == "El")
            {
                tbFuelAmount.Clear();
                tbUnitPrice.Clear();
                tbTotalPrice.Clear();

                tbFuelAmount.Text = "0";
                tbUnitPrice.Text = "0";
                tbTotalPrice.Text = "0";

                tbFuelAmount.IsReadOnly = true;
                tbFuelAmount.Background = Brushes.LightGray;
                tbUnitPrice.IsReadOnly = true;
                tbUnitPrice.Background = Brushes.LightGray;
                tbTotalPrice.IsReadOnly = true;
                tbTotalPrice.Background = Brushes.LightGray;
            }
        }
        #endregion FillFuelTypeCombobox

        #region AddDriverJournal
        private void btnAddDriverJournal_Click(object sender, RoutedEventArgs e)
        {
            if (dtpDate.Text != string.Empty && string.IsNullOrWhiteSpace(tbMilage.Text) && string.IsNullOrWhiteSpace(tbFuelAmount.Text) && string.IsNullOrWhiteSpace(tbUnitPrice.Text) && string.IsNullOrWhiteSpace(tbTotalPrice.Text) && cmbVehicle.SelectedItem != null &&
                cmbFuelType.SelectedItem != null && _user != null)
            {
                var response = MessageBox.Show("Är du säker på att du vill lägga till denna körjournal?", "Är du säker?",
                    MessageBoxButton.YesNo);
                if (response == MessageBoxResult.Yes)
                {
                    AddNewDriverJournal().Wait();
                }
            }
            else
            {
                MessageBox.Show("Inga fält får vara tomma.", "Tomma fält");
            }
        }

        public async Task AddNewDriverJournal()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt User-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            FuelTypeModel selectedFueltype = cmbFuelType.SelectedItem as FuelTypeModel;
            UserModel selectedUser = _user;
            
            TryParse tryParse = new TryParse();

            bool isDateDateTime = tryParse.IsValidDateTime(dtpDate.Text);
            bool isMilageInt = tryParse.IsValidInteger(tbMilage.Text);
            bool isFuelAmountDecimal = tryParse.IsValidDecimal(tbFuelAmount.Text);
            bool isPricePerUnitDecimal = tryParse.IsValidDecimal(tbUnitPrice.Text);
            bool isTotalPriceDecimal = tryParse.IsValidDecimal(tbTotalPrice.Text);

            if (isDateDateTime && isMilageInt && isFuelAmountDecimal && isPricePerUnitDecimal && isTotalPriceDecimal)
            {
                ReportDriverJournalModel journalToAdd = new ReportDriverJournalModel
                {
                    Date = Convert.ToDateTime(dtpDate.Text),
                    Milage = Convert.ToInt32(tbMilage.Text),
                    FuelAmount = Convert.ToDecimal(tbFuelAmount.Text),
                    PricePerUnit = Convert.ToDecimal(tbUnitPrice.Text),
                    TotalPrice = Convert.ToDecimal(tbTotalPrice.Text),
                    VehicleID = selectedVehicle.ID,
                    FuelTypeID = selectedFueltype.ID,
                    ChauffeurID = selectedUser.ID
                };

                HttpResponseMessage response =
                    client.PostAsJsonAsync("api/reportdriverjournal/adddriverjournal", journalToAdd).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Ny körjournal tillagt!", "Lyckades");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
                }
            }
            else
            {
                MessageBox.Show("Fel format i fälten, vänligen ange rätt format.", "Fel format", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        #endregion AddDriverJournal

        #region CalculateTotalPrice
        private void CountTotalPrice(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbUnitPrice.Text) || !string.IsNullOrWhiteSpace(tbFuelAmount.Text))
            {
                decimal unitPrice = 0;
                decimal fuelAmount = 0;
                decimal totalPrice = 0;

                if (decimal.TryParse(tbUnitPrice.Text, out unitPrice) && decimal.TryParse(tbFuelAmount.Text, out fuelAmount))
                {
                    totalPrice += Convert.ToDecimal((unitPrice * fuelAmount));

                    tbTotalPrice.Text = totalPrice.ToString("n2");
                }
            }
        }
        #endregion
        
    }
}
