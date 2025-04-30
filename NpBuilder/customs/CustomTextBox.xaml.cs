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

namespace NpBuilder.customs
{
    /// <summary>
    /// Interaction logic for CustomTextBox.xaml
    /// </summary>
    public partial class CustomTextBox : UserControl
    {

     

        public CustomTextBox()
        {
            InitializeComponent();
        }

        public CustomTextBox(string name)
        {
            InitializeComponent();

            tbname.Text = name;
        }

        public string getTbInput() { return tbinput.Text as string; }

    }
}
