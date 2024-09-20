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

namespace PDFMaker
{
    /// <summary>
    /// Interaction logic for TextSettings.xaml
    /// </summary>
    public partial class TextSettings : Page
    {
        public TextSettings()
        {
            InitializeComponent();
            FontSize.Text = Properties.Settings.Default.FontSize.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FontSize = Convert.ToInt32(FontSize.Text);
            Properties.Settings.Default.Save();
        }
    }
}
