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
using System.Windows.Shapes;

namespace PDFMaker
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
            TextBlock1.FontSize = 16;
            TextBlock1.Text = "Bold Text: \t*Text* \nItalic Text: \t+Text+ \nUnderline: \t_Text_ \nHeadline: \t#Text \nHeadline2: \t##Text \nHeadline3: \t###Text\nImage: \t[image](image.png)";
        }
    }
}
