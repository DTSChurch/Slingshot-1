using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using Slingshot.City.Utilities;

namespace Slingshot.City
{
    /// <summary>
    /// Interaction logic for Open.xaml
    /// </summary>
    public partial class Open : Window
    {
        public Open()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            lblMessage.Text = string.Empty;
            if (txtFilename.Text != string.Empty && System.IO.Directory.Exists(txtFilename.Text))
            {
                string filepath = txtFilename.Text + @"\";
                string persons = filepath + "Persons";
                string groups = filepath + "Groups";
                string financial = filepath + "Financial";
                bool ifExists = false;

                if(System.IO.Directory.Exists(persons))
                {
                    ifExists = true;
                }
                if (System.IO.Directory.Exists(groups))
                {
                    ifExists = true;
                }
                if (System.IO.Directory.Exists(financial))
                {
                    ifExists = true;
                }

                if (ifExists == true)
                {
                    TheCityExport.FilePath = filepath;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show(); 
                    this.Close();
                }
                else
                {
                    lblMessage.Text = 
                        "Please select a folder containing folders named either Persons, Financial, or Groups ";
                }                
            }
            else
            {
                lblMessage.Text = "Please choose a valid directory.";
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = fileDialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.RootFolder;
                    string sFilename = fileDialog.SelectedPath + @"\";
                    txtFilename.Text = fileDialog.SelectedPath + @"\";

                    txtFilename.ToolTip = file;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    txtFilename.Text = null;
                    txtFilename.ToolTip = null;
                    break;
            }
        }
    }
}
