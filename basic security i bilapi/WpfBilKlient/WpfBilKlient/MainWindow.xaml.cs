using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WpfBilKlient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttonGetAllCars_Click(object sender, RoutedEventArgs e)
        {
            //Exekveringen fortsätter förbi den här raden eftersom GetCarsFromWebApi är async
            GetCarsFromWebApi();
        }

        private async Task GetCarsFromWebApi()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(textBoxAddress.Text);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Setting authorization header
            string credentials = "user:password";
            var credentialBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
           client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentialBytes));  
           
            HttpResponseMessage response = client.GetAsync("api/Bil").Result;
            

            if (response.IsSuccessStatusCode)
            {
                var text = response.Content.ReadAsStringAsync();
                //Read async så vi måste vänta på att allt lästs in, därav await.
                var bilar = JsonConvert.DeserializeObject<List<Bil>>(await text);
                dataGridBilar.ItemsSource = bilar;
                dataGridBilar.Visibility = Visibility.Visible;
                NewCarPanel.Visibility = Visibility.Collapsed;
                newCarBorder.Visibility = Visibility.Collapsed;
                dataGridBilar.CanUserAddRows = false;
                dataGridBilar.CanUserDeleteRows = false;
                
            }
            else
            {
                //Error
                MessageBox.Show("Hämtningen gick inte bra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonHamtaBil_Click(object sender, RoutedEventArgs e)
        {
            GetCarsFromWebApi(textBoxSokRegnr.Text);
        }

        private async Task GetCarsFromWebApi(string regnr)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(textBoxAddress.Text);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/Bil?" + regnr).Result;

            if (response.IsSuccessStatusCode)
            {
                var text = response.Content.ReadAsStringAsync();
                //Read async så vi måste vänta på att allt lästs in, därav await.
                var bilar = JsonConvert.DeserializeObject<List<Bil>>(await text);
                dataGridBilar.ItemsSource = bilar;
                dataGridBilar.Visibility = Visibility.Visible;
                NewCarPanel.Visibility = Visibility.Collapsed;
                newCarBorder.Visibility = Visibility.Collapsed;
                
                dataGridBilar.CanUserAddRows = false;
                dataGridBilar.CanUserDeleteRows = false;

            }
            else
            {
                //Error
                MessageBox.Show("Hämtningen gick inte bra.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxSokRegnr.Text = "";
            dataGridBilar.ItemsSource = null;
            dataGridBilar.Visibility = Visibility.Collapsed;
            NewCarPanel.Visibility = Visibility.Visible;
            newCarBorder.Visibility = Visibility.Visible;
        }

        private void ButtonNewCar_OnClick(object sender, RoutedEventArgs e)
        {
            SendNewCar();
        }

        private async Task SendNewCar()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(textBoxAddress.Text);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            int arsmodell;
            Int32.TryParse(textBoxArsmodell.Text, out arsmodell);
            Bil bil = new Bil
            {
                Registreringsnummer = textBoxSokRegnr.Text,
                Arsmodell = arsmodell,
                Modellbeteckning = textBoxModell.Text,
                Marke = textBoxMarke.Text,
                Farg = textBoxFarg.Text
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(bil));
            //bil.Metalliclack = checkBoxMetallic.IsChecked.Value;  Används ändå ej i API:t just nu.
            var response = client.PostAsync("api/bil", content);
        }

        private void buttonLocal_Click(object sender, RoutedEventArgs e)
        {
            textBoxAddress.Text = "http://localhost:58284/";
        }

        private void buttonAzure_Click(object sender, RoutedEventArgs e)
        {
            textBoxAddress.Text = "http://microsoft-apiappdba56dcc1d6e4d2e9c0a2c61707dbd1e.azurewebsites.net/";
        }
    }
}
