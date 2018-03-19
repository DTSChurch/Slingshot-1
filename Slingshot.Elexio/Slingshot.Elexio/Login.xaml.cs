using System;
using System.Collections.Generic;
using System.Linq;
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

using Slingshot.Elexio.Utilities;

namespace Slingshot.Elexio
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        #region Window Events

        private void btnLogin_Click( object sender, RoutedEventArgs e )
        {
            lblMessage.Text = string.Empty;

            if ( ( txtHostname.Text != string.Empty && txtDatabase.Text != string.Empty && cbWindowsAuth.IsChecked == true ) ||
                 ( txtHostname.Text != string.Empty && txtDatabase.Text != string.Empty && cbSQLAuth.IsChecked == true && txtApiUsername.Text != string.Empty && txtApiPassword.Text != string.Empty ))
            {
                ElexioApi.Connect( txtHostname.Text, txtDatabase.Text, txtApiUsername.Text, txtApiPassword.Text, cbSQLAuth.IsChecked.Value );

                if ( ElexioApi.IsConnected )
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    lblMessage.Text = $"Could not login with the information provided. {ElexioApi.ErrorMessage}";
                }
            }
            else
            {
                lblMessage.Text = "Please provide the information needed to connect.";
            }
        }

        private void cbSQLAuth_Click( object sender, RoutedEventArgs e )
        {
            cbSQLAuth.IsChecked = true;
            cbWindowsAuth.IsChecked = false;
            gridMain.RowDefinitions[4].Height = new GridLength( 1, GridUnitType.Auto );
            gridMain.RowDefinitions[5].Height = new GridLength( 1, GridUnitType.Auto );
        }

        private void cbWindowsAuth_Click( object sender, RoutedEventArgs e )
        {
            cbWindowsAuth.IsChecked = true;
            cbSQLAuth.IsChecked = false;
            gridMain.RowDefinitions[4].Height = new GridLength( 0 );
            gridMain.RowDefinitions[5].Height = new GridLength( 0 );
        }

        #endregion
    }
}
