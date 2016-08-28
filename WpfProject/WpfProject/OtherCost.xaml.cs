using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using WpfProject.Model;
using WpfProject.Classes;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for OtherCost.xaml
    /// </summary>
    public partial class OtherCost : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public OtherCost(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillVehicleCombobox().Wait();

            FillTypeOfCostCombobox().Wait();
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

        #region FillTypeOfCostComboBox
        private async Task FillTypeOfCostCombobox()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/othercost/filltypeofcostcombobox").Result;

            if (response.IsSuccessStatusCode)
            {
                var typeOfCosts = await response.Content.ReadAsStringAsync();
                List<TypeOfCostModel> returnedTypeOfCostList = JsonConvert.DeserializeObject<List<TypeOfCostModel>>(typeOfCosts);

                cmbTypeOfCost.Items.Clear();

                List<TypeOfCostModel> typeOfCostListToShow = new List<TypeOfCostModel>();

                foreach (var typeOfCost in returnedTypeOfCostList)
                {
                    typeOfCostListToShow.Add(new TypeOfCostModel() { ID = typeOfCost.ID, Type = typeOfCost.Type });
                }

                cmbTypeOfCost.ItemsSource = typeOfCostListToShow;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }
        #endregion FillTypeOfCostCombobox

        #region AddNewCost
        private void btnAddCost_Click(object sender, RoutedEventArgs e)
        {
            if (dtpDate.Text != string.Empty && tbCost.Text != string.Empty && cmbVehicle.SelectedItem != null &&
                cmbTypeOfCost.SelectedItem != null)
            {
                var response = MessageBox.Show("Är du säker på att du vill lägga till denna kostnad?", "Är du säker?",
                    MessageBoxButton.YesNo);
                if (response == MessageBoxResult.Yes)
                {
                    AddNewCost().Wait();
                }
            }
            else
            {
                MessageBox.Show("Inga fält får vara tomma.", "Tomma fält");
            }
        }

        public async Task AddNewCost()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt User-objekt samt lägger till värdena.
            VehicleModel selectedVehicle = cmbVehicle.SelectedItem as VehicleModel;
            TypeOfCostModel selectedTypeOfCost = cmbTypeOfCost.SelectedItem as TypeOfCostModel;

            TryParse tryParse = new TryParse();

            bool isDateDateTime = tryParse.IsValidDateTime(dtpDate.Text);
            bool isCostInteger = tryParse.IsValidInteger(tbCost.Text);

            if (isDateDateTime && isCostInteger)
            {
                OtherCostModel costToAdd = new OtherCostModel
                {
                    Date = Convert.ToDateTime(dtpDate.Text),
                    Cost = tbCost.Text,
                    Comment = tbComment.Text,
                    VehicleID = selectedVehicle.ID,
                    TypeOfCostID = selectedTypeOfCost.ID
                };

                HttpResponseMessage response = client.PostAsJsonAsync("api/othercost/addnewcost", costToAdd).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Ny kostnad tillagt!", "Lyckades");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
                }
            }
            else
            {
                MessageBox.Show("Fel format i fälten, vänligen ange rätt format.", "Fel format", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion AddNewCost

        #endregion
    }
}
