using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using WpfProject.Classes;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for VehicleWindow.xaml
    /// </summary>
    public partial class VehicleWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public VehicleWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            FillAllComboBoxes().Wait();
        }
        #endregion

        #region Methods

        #region FillAllComboBoxes
        private async Task FillAllComboBoxes()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/vehicle/fillallcomboboxes").Result;

            if (response.IsSuccessStatusCode)
            {
                var comboBox = await response.Content.ReadAsStringAsync();
                VehicleComboboxModel returnedComboBoxInformationLists = JsonConvert.DeserializeObject<VehicleComboboxModel>(comboBox);

                // Rensar alla comboboxar.
                cmbColor.Items.Clear();
                cmbFuelType.Items.Clear();
                cmbModelYear.Items.Clear();
                cmbVehicleType.Items.Clear();

                // Skapar ett nytt objekt av Combobox.
                VehicleComboboxModel comboBoxModel = new VehicleComboboxModel();

                // Fyller listorna i Combobox-objektet.
                foreach (var color in returnedComboBoxInformationLists.ColorList)
                {
                    comboBoxModel.ColorList.Add(new ColorModel()
                    {
                        ID = color.ID,
                        ColorName = color.ColorName
                    });
                }
                foreach (var fuelType in returnedComboBoxInformationLists.FuelTypeList)
                {
                    comboBoxModel.FuelTypeList.Add(new FuelTypeModel()
                    {
                        ID = fuelType.ID,
                        Type = fuelType.Type
                    });
                }
                foreach (var modelYear in returnedComboBoxInformationLists.ModelYearList)
                {
                    comboBoxModel.ModelYearList.Add(new ModelYearModel()
                    {
                        ID = modelYear.ID,
                        Year = modelYear.Year.ToString()
                    });
                }
                foreach (var vehicleType in returnedComboBoxInformationLists.VehicleTypeList)
                {
                    comboBoxModel.VehicleTypeList.Add(new VehicleTypeModel()
                    {
                        ID = vehicleType.ID,
                        Type = vehicleType.Type
                    });
                }

                cmbColor.ItemsSource = comboBoxModel.ColorList;
                cmbFuelType.ItemsSource = comboBoxModel.FuelTypeList;
                cmbModelYear.ItemsSource = comboBoxModel.ModelYearList;
                cmbVehicleType.ItemsSource = comboBoxModel.VehicleTypeList;
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }
        }
        #endregion FillAllComboBoxes

        #region AddVehicle
        private void btnAddVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbRegNo.Text) && !string.IsNullOrWhiteSpace(tbOriginalMilage.Text) &&
                cmbVehicleType != null && cmbFuelType != null)
            {
                var response = MessageBox.Show("Är du säker på att du vill lägga til detta fordon?", "Är du säker?",
                    MessageBoxButton.YesNo);
                if (response == MessageBoxResult.Yes)
                {
                    AddVehicle().Wait();
                }
            }
            else
            {
                MessageBox.Show("Obligatoriska fält får inte vara tomma.", "Tomma fält", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        public async Task AddVehicle()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            // Skapar ett nytt User-objekt samt lägger till värdena.
            ColorModel selectedColor = cmbColor.SelectedItem as ColorModel;
            FuelTypeModel selectedFueltype = cmbFuelType.SelectedItem as FuelTypeModel;
            ModelYearModel selectedModelYear = cmbModelYear.SelectedItem as ModelYearModel;
            VehicleTypeModel selectedVehicleType = cmbVehicleType.SelectedItem as VehicleTypeModel;

            // Check's so the RegNo textbox value is of 3 letters and 3 integers.
            Regex regexRegNo = new Regex(@"^[A-Z]{3}\d{3}$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            bool isRegNo = regexRegNo.IsMatch(tbRegNo.Text);

            // Check's so the OriginalMilage textbox value is only integers.
            TryParse tryParse = new TryParse();
            bool isOriginalMilageInteger = tryParse.IsValidInteger(tbOriginalMilage.Text);

            if (isRegNo && isOriginalMilageInteger)
            {
                VehicleModel vehicleToAdd = new VehicleModel
                {
                    RegNo = tbRegNo.Text,
                    Description = !string.IsNullOrWhiteSpace(tbDescription.Text) ? tbDescription.Text : string.Empty,
                    OriginalMilage = Convert.ToInt32(tbOriginalMilage.Text),
                    FuelTypeID = selectedFueltype.ID,
                    FuelType = selectedFueltype.Type,
                    VehicleTypeID = selectedVehicleType.ID,
                    VehicleType = selectedVehicleType.Type
                };

                if (selectedColor != null)
                {
                    vehicleToAdd.ColorID = selectedColor.ID;
                    vehicleToAdd.Color = selectedColor.ColorName;
                };

                if (selectedModelYear != null)
                {
                    vehicleToAdd.ModelYearID = selectedModelYear.ID;
                    vehicleToAdd.ModelYear = Convert.ToInt32(selectedModelYear.Year);
                };

                HttpResponseMessage response = client.PostAsJsonAsync("api/vehicle/addvehicle", vehicleToAdd).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Nytt fordon tillagt!", "Lyckades");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
                }
            }
            else
            {
                MessageBox.Show("Fel format inskrivet, vänligen ange rätt format.", "Fel format", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }
        #endregion

        #endregion
    }
}

