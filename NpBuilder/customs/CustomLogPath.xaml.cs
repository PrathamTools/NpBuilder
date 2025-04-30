using Microsoft.Win32;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NpBuilder.customs
{
    /// <summary>
    /// Interaction logic for CustomLogPath.xaml
    /// </summary>
    public partial class CustomLogPath 
    {
        


        public CustomLogPath(string path)
        {
            InitializeComponent();
            pathfor.Text = path;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.IsFolderPicker = true; // Ensures only folders can be selected
                folderDialog.Title = "Select a folder";

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    selected_path.Text = folderDialog.FileName;
                }
            }

           
        
       }
        public string getLogName()
        {
            return pathfor.Text as string;
        }

        public string getPath() { return selected_path.Text as string; }

        public void clear()
        {
            selected_path.Text = null;
        }
    }
}
