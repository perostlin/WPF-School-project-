using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfProject.Classes;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants
        private const string TestApi = "http://htaxiapi.azurewebsites.net/";
        #endregion

        #region Page_Load
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods

        #region Login
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login().Wait();
        }

        public async Task Login()
        {
            string apiAddress = ConfigurationManager.AppSettings["ApiAddress"];

            if (apiAddress == null)
            {
                MessageBox.Show("Fanns ingen api adress att hämta, vänligen kontakta personal.", "Fel uppstod",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RegexValidator regex = new RegexValidator();

            bool isLocalHost = regex.IsValidLocalHost(apiAddress);
            bool isAzure = regex.IsValidAzure(apiAddress);

            if (isLocalHost || isAzure && apiAddress != null)
            {
                UserModel userToLogin = new UserModel
                {
                    Username = tbUsername.Text.ToLower(),
                    Password = tbPassword.Password.ToLower()
                };

                HttpClient client = HelperMethods.GetClient(userToLogin);
                
                HttpResponseMessage response = new HttpResponseMessage();

                try
                {
                    response = client.PostAsJsonAsync("api/user/login", userToLogin).Result;
                }
                catch (AggregateException we)
                {
                    MessageBox.Show(
                        we.InnerException.Message +
                        " - kontrollera att api-adressen stämmer eller kontakta administratör om problemet kvarstår.",
                        "Fel uppstod", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Avbryter alla metoder som körs.
                    return;
                }

                if (response.IsSuccessStatusCode)
                {
                    var user = response.Content.ReadAsAsync<UserModel>().Result;

                    if (user.IsAdmin == true)
                    {
                        AdminWindow adminWindow = new AdminWindow(user);
                        adminWindow.Show();
                    }
                    else
                    {
                        UserWindow userWindow = new UserWindow(user);
                        userWindow.Show();
                    }

                    this.Close();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Du angav fel inloggningsuppgifter, försök igen.", "Inloggningen misslyckades");
                }
                else
                {
                    MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
                }
            }
            else
            {
                MessageBox.Show("Din api-adress verkar vara korrupt, vänligen kontakta personal.", "Korrupt api-adress",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion Login

        #endregion
    }
}
