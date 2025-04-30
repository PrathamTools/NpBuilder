using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net;
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
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace NpBuilder
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {

        static string empid;
        static string firebaseUri = "https://asc-project-f71d1-default-rtdb.firebaseio.com/Teams/NewPOS.json";
        static bool found = false;
        public SignUp()
        {
            InitializeComponent();
        }

        private async void signIn_Click(object sender, RoutedEventArgs e)
        {
            empid = empId.Text.ToString();

            if (empid.Length > 7)
            {
                await CheckEmpId(empid);

                if (found)
                {
                  MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                  
                }
            }
            else
                MessageBox.Show("Enter valid Employee Id");





        }

        static async Task CheckEmpId(string empid)
        {
            HttpClient httpClient = new HttpClient();
            // Httpclient is used to interact with rest api(firebase)

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(firebaseUri);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(responseBody);

                if (data.ContainsKey(empid))
                {
                    string lid = data[empid]?["lid"]?.ToString();

                    if (lid.Equals(Dns.GetHostName()))
                    {
                        found = true;
                    }
                    else
                        MessageBox.Show("Please use company laptop");

                }
                else
                    MessageBox.Show("Unable to find " + empid + " on database");


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();

    }
    }

