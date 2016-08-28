using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public RegisterWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;
        }
        #endregion

        #region Methods

        #region Register
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbUsername.Text) && !string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                var response = MessageBox.Show("Är du säker på att du vill lägga till denna användare?", "Är du säker?",
                    MessageBoxButton.YesNo);

                if (response == MessageBoxResult.Yes)
                {
                    RegisterUser().Wait();
                }
            }
            else
            {
                MessageBox.Show("Obligatoriska fält får inte vara tomma.", "Tomma fält", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
        }

        public async Task RegisterUser()
        {
            HttpClient client = Classes.HelperMethods.GetClient(_user);

            if (tbUsername.Text.Length >= 3 && tbPassword.Password.Length >= 6)
            {
                // Skapar ett nytt User-objekt samt lägger till värdena.
                UserModel userToRegister = new UserModel
                {
                    Username = tbUsername.Text.ToLower(),
                    Password = tbPassword.Password.ToLower(),
                    IsAdmin = (bool) cboxIsAdmin.IsChecked
                };

                HttpResponseMessage response = client.PostAsJsonAsync("api/user/register", userToRegister).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Ny användare tillagd!", "Lyckades");
                    this.Close();
                }
                else if (response.StatusCode == HttpStatusCode.Found)
                {
                    MessageBox.Show("Det finns redan en användare med detta användarnamn, försök med ett annat.",
                        "Registreringen misslyckades");
                }
                else
                {
                    MessageBox.Show("Uppkopplingen till server:n misslyckades, kontakta personal.", "Server nere");
                }
            }
            else
            {
                MessageBox.Show("Måste uppfylla kriterierna för att kunna lägga till en ny användare.",
                    "Kriterier ej uppfyllda", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        #endregion Register

        #endregion
    }
}
