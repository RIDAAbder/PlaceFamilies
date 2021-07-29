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

namespace PlaceFamilies.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            this.stepTxtBox.Text = Properties.Settings.Default.step.ToString();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            int factor = 0;
            bool canBeParsed = Int32.TryParse(this.stepTxtBox.Text, out factor);
            if (!canBeParsed)
            {
                MessageBox.Show("Insert an integer value for the step");
                return;
            }
            Properties.Settings.Default.step = factor;
            this.DialogResult = true;
        }
    }
}
