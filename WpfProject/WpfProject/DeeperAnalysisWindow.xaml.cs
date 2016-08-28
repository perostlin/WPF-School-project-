using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using WpfProject.Model;
using System.Net;
using System.Linq;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for DeeperAnalysisWindow.xaml
    /// </summary>
    public partial class DeeperAnalysisWindow : INotifyPropertyChanged
    {
        #region Properties
        private readonly UserModel _user;        

        public List<DeeperAnalysisModel> DeeperAnalysisToBind { get; set; }

        private readonly ObservableCollection<DeeperAnalysisModel> _contributeList;
        public ObservableCollection<DeeperAnalysisModel> ContributeList
        {
            get { return _contributeList; }
        }

        private string _totalAverageFuelConsumption;
        public string TotalAverageFuelConsumption
        {
            get { return _totalAverageFuelConsumption; }
            set
            {
                _totalAverageFuelConsumption = value;
                OnPropertyChanged();
            }
        }

        private string _totalMilage;
        public string TotalMilage
        {
            get { return _totalMilage; }
            set
            {
                _totalMilage = value;
                OnPropertyChanged();
            }
        }

        private string _totalFuelCost;
        public string TotalFuelCost
        {
            get { return _totalFuelCost; }
            set
            {
                _totalFuelCost = value;
                OnPropertyChanged();
            }
        }

        private string _totalAverageFuelCost;
        public string TotalAverageFuelCost
        {
            get { return _totalAverageFuelCost; }
            set
            {
                _totalAverageFuelCost = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Page_Load
        public DeeperAnalysisWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            DataContext = this;

            DeeperAnalysisToBind = new List<DeeperAnalysisModel>();

            _contributeList = new ObservableCollection<DeeperAnalysisModel>();
            _contributeList.CollectionChanged += ContributeList_CollectionChanged;

            // Fills the listview with vehicles and their data.
            FillVehicleListViewWithData().Wait();
        }
        #endregion

        #region Methods
        private async Task FillVehicleListViewWithData()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            HttpResponseMessage response = client.GetAsync("api/deeperanalysis/getallvehicleswithdata").Result;

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                MessageBox.Show(
                    "Det fanns inga fordon med körjournaler denna månad, vänligen lägg till några och försök igen.",
                    "Gick inte att hämta");
            }
            else if (response.IsSuccessStatusCode)
            {
                var vehicles = await response.Content.ReadAsStringAsync();
                List<DeeperAnalysisModel> returnedUserList = JsonConvert.DeserializeObject<List<DeeperAnalysisModel>>(vehicles);

                DeeperAnalysisList.Items.Clear();

                foreach (var vehicle in returnedUserList)
                {
                    DeeperAnalysisToBind.Add(new DeeperAnalysisModel()
                    {
                        VehicleID = vehicle.VehicleID,
                        RegNo = vehicle.RegNo,
                        VehicleType = vehicle.VehicleType,
                        MilageLatestMonth = vehicle.MilageLatestMonth,
                        FuelConsumptionLatestMonth = vehicle.FuelConsumptionLatestMonth,
                        FuelCostLatestMonth = vehicle.FuelCostLatestMonth,
                        FuelType = vehicle.FuelType
                    });
                }
            }
            else
            {
                MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
            }

        }

        private void DeeperAnalysisList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeeperAnalysisList.SelectedItems.Count >= 1)
            {
                decimal totalAverageFuelConsumption = 0;
                int totalMilage = 0;
                decimal totalFuelCost = 0;
                decimal totalAverageFuelCost = 0;

                string previousFuelType = string.Empty;
                int errorCounter = 0;
                int firstLoop = 0;
            
                foreach (DeeperAnalysisModel item in DeeperAnalysisList.SelectedItems)
                {
                    firstLoop++;
                    
                    if (errorCounter >= 1)
                    {
                        totalAverageFuelConsumption = 0;
                    }
                    else if (previousFuelType == item.FuelType || firstLoop == 1)
                    {
                        totalAverageFuelConsumption += item.FuelConsumptionLatestMonth;
                    }
                    else
                    {
                        MessageBox.Show("Fordonen du försöker räkna på har inte samma drivmedel.",
                            "Det gick inte få ut snittförbrukningen");

                        totalAverageFuelConsumption = 0;
                        errorCounter++;
                    }

                    totalMilage += item.MilageLatestMonth;
                    totalFuelCost += item.FuelCostLatestMonth;

                    if (ContributeList.Count == 0)
                    {
                        ContributeList.Add(new DeeperAnalysisModel
                        {
                            RegNo = item.RegNo,
                            MilageLatestMonth = item.MilageLatestMonth,
                            FuelCostLatestMonth = item.FuelCostLatestMonth
                        });
                    }
                    else
                    {
                        if (!ContributeList.Any(x => x.RegNo == item.RegNo))
                        {
                            ContributeList.Add(new DeeperAnalysisModel
                            {
                                RegNo = item.RegNo,
                                MilageLatestMonth = item.MilageLatestMonth,
                                FuelCostLatestMonth = item.FuelCostLatestMonth
                            });
                        }
                    }

                    // Sets the previous fueltype.
                    previousFuelType = item.FuelType;
                }

                totalAverageFuelCost = totalFuelCost / totalMilage;

                TotalAverageFuelConsumption = totalAverageFuelConsumption.ToString("n2") + " liter";
                TotalMilage = totalMilage.ToString();
                TotalFuelCost = totalFuelCost.ToString("c");
                TotalAverageFuelCost = totalAverageFuelCost.ToString("c");
            }
            else
            {
                TotalAverageFuelConsumption = string.Empty;
                TotalMilage = string.Empty;
                TotalFuelCost = string.Empty;
                TotalAverageFuelCost = string.Empty;
            }
        }

        private void ContributeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
        }
        #endregion

        #region PropertyOnChange
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
