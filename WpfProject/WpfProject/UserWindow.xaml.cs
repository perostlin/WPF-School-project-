using System.Windows;
using WpfProject.Classes;
using WpfProject.Model;

namespace WpfProject
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        #region Properties
        private readonly UserModel _user;
        #endregion

        #region Page_Load
        public UserWindow(UserModel user)
        {
            InitializeComponent();

            _user = user;

            lbLoggedInUser.Content = _user.Username;
        }
        #endregion

        #region Methods

        private void btnAddNewJournal_Click(object sender, RoutedEventArgs e)
        {
            AddChauffeurDriverJournal chauffeurDriverJournalWindow = new AddChauffeurDriverJournal(_user);
            chauffeurDriverJournalWindow.Show();
        }

        private void btnMyConsumption_Click(object sender, RoutedEventArgs e)
        {
            CalculateChauffeurConsumptionWindow chauffeurConsumptionWindow = new CalculateChauffeurConsumptionWindow(_user);
            chauffeurConsumptionWindow.Show();
        }
       

        #endregion
    }
}
