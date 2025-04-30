
using Microsoft.Win32;
using NpBuilder.customs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Linq;
using static ScintillaNET.Style;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace NpBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CustomLogPath posData_path;
        string startNp6File = "";
        string kiosknp6file = "";
        string waynp6file = "";
        private readonly string specialFile = "_8000_pos-db.xml";
        private readonly string wayspecialFile = "_8000_pos-db.xml";
        string posDatafolderPath = "";
        string version ="";
        string promotionId = "";
        string foesimulatorpath = "";
        string offersimulatorpath = "";
        CustomLogPath filepath;
        CustomLogPath decoupleKioskpath;
        CustomLogPath decoupleWaypath;

        OpenFileDialog openFileDialog = new OpenFileDialog();


        public MainWindow()
        {
            InitializeComponent();
            
            posData_path = new CustomLogPath("PosData Path :");
            posdataboxpath.Children.Clear();
            posdataboxpath.Children.Add(posData_path);

            decoupleKioskpath = new CustomLogPath("Kiosk PosData Path :");
            kioskposdataboxpath.Children.Clear();
            kioskposdataboxpath.Children.Add(decoupleKioskpath);

            decoupleWaypath = new CustomLogPath("Waystation PosData Path :");
            wayposdataboxpath.Children.Clear();
            wayposdataboxpath.Children.Add(decoupleWaypath);
          

        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximize_Click(object sender, RoutedEventArgs e) => WindowState =
            WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void Close_Click(object sender, RoutedEventArgs e) => Close();


        bool checkposdata(string path)
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, "*.xml").Length > 0;
            return false;
        }
        private void startCreateBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                buildNeedRun.Children.Clear();

                hometabmenu.Visibility = Visibility.Visible;
                homeTabContent.Visibility = Visibility.Visible;
                decoupledTabmenu.Visibility = Visibility.Collapsed;
                decoupleTabContent.Visibility = Visibility.Collapsed;

                fileTabContent.Visibility = Visibility.Collapsed;
                fileTabmenu.Visibility = Visibility.Collapsed;


                if (checkposdata(posData_path.getPath()))
                {

                    posDatafolderPath = posData_path.getPath();

                    if (File.Exists(posDatafolderPath + "\\product.specification"))
                    {
                        XDocument document = XDocument.Load(posDatafolderPath + "\\product.specification");
                        var getversion = document.Descendants("ProductSpecification").FirstOrDefault();

                        if (getversion != null)
                        {
                            var vsion = getversion.Attribute("version")?.Value;
                            version = vsion.Split('.')[0];

                            var setvsion = getversion.Attribute("artifact")?.Value;
                            versionupdate.Text = "Build Version :- " + setvsion;


                        }
                        else
                            MessageBox.Show("Version not found");
                    }
                    else
                        MessageBox.Show("Product specification not found");



                    if (version.Equals("36"))
                        version36();
                    else if (version.Equals("30"))
                        version30();
                    else
                        MessageBox.Show("Not Found Version");


                }
                else
                    MessageBox.Show("Please provide valid PosData path");

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void version30()
        {
            try
            {

          
            bool proceed = true;
            string parentFolderPath = System.IO.Path.GetDirectoryName(posDatafolderPath);


            // Check if specific subfolders exist in the parent folder
            string[] folderNames = { "NpSharpBin", "Bat", "Bin" };

            // Get all directories in the parent folder
            string[] directories = Directory.GetDirectories(parentFolderPath);

            // Check if the required folders exist (case-insensitive)
            foreach (var folderName in folderNames)
            {
                bool folderExists = Array.Exists(directories, dir =>
                    string.Equals(System.IO.Path.GetFileName(dir), folderName, StringComparison.OrdinalIgnoreCase));

                if (!folderExists)
                {
                    MessageBox.Show(folderName + " Not Available");
                    proceed = false;
                }

            }

                if (proceed)
                {

                    bool filenotavailable = false;
                    string[] requiredFiles = { "clean.bat", "_NP61_StartNPsharpApp.bat", "_np6x_Close_All_NewPOS6x_ Apps.bat" };
                    string extractPath = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(posDatafolderPath))).FullName;

                    foreach (string file in requiredFiles)
                    {
                        string filePath = System.IO.Path.Combine(parentFolderPath, file);
                        //   MessageBox.Show(filePath);
                        if (!File.Exists(filePath))
                        {
                            filenotavailable = true;
                        }
                        else
                        {
                            // MessageBox.Show($"{file} is missing.");
                        }
                    }
                    if (filenotavailable)
                    {
                        string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
                        string zipFilePath = System.IO.Path.Combine(assetsFolder, "v30buildbats.zip");


                        try
                        {
                            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    string relativePath = entry.FullName;

                                    // If the zip contains a folder (like 'v36CashlessSimulator/'), remove it
                                    if (relativePath.Contains("/"))
                                    {
                                        relativePath = relativePath.Substring(relativePath.IndexOf("/") + 1);
                                    }

                                    // Ensure the destination path does not include the original root folder
                                    string destinationPath = System.IO.Path.Combine(extractPath, relativePath);

                                    // Ensure directories exist
                                    string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                                    if (!Directory.Exists(directoryPath))
                                    {
                                        Directory.CreateDirectory(directoryPath);
                                    }

                                    // Extract files (skip directories)
                                    if (!string.IsNullOrEmpty(entry.Name))
                                    {
                                        entry.ExtractToFile(destinationPath, overwrite: true);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Exception :" + ex);
                        }
                    }

                    string batFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bat", StringComparison.OrdinalIgnoreCase));
                    string binFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bin", StringComparison.OrdinalIgnoreCase));

                    startNp6File = System.IO.Path.Combine(batFolderPath, "start.np6");

                    System.IO.File.WriteAllText(startNp6File, string.Empty);

                    if (System.IO.File.Exists(startNp6File))
                    {

                        var lines = System.IO.File.ReadAllLines(startNp6File).ToList();

                        // Get all valid XML file names
                        var xmlFiles = Directory.GetFiles(posDatafolderPath, "*_pos-db.xml")
                            .Select(System.IO.Path.GetFileName)
                            .Where(file => !file.Contains("Np6PosCore") && !file.Contains("npsharp") && !file.Contains("np6WayCore"))
                            .ToList();

                        bool updated = false;
                        bool specialFileExists = lines.Any(line => line.Contains("_8000_pos-db.xml"));

                        // Normal command format for all XML files except _8000_pos-db.xml
                        foreach (var file in xmlFiles)
                        {
                            string command = $@"..\bin\ | npapp.exe ""..\PosData\{file}"" ""..\OUT"" ""..\TEMP"" pos-log61.properties";

                            // Add only if missing
                            if (!lines.Any(line => line.Contains(file)))
                            {
                                if (!file.Contains(specialFile))
                                    lines.Add(";" + command); // Disabled by default
                                updated = true;
                            }
                        }

                        // Special handling for _8000_pos-db.xml
                        if (!specialFileExists)
                        {
                            // If _8000_pos-db.xml is completely missing, add the Java command
                            string specialCommand = @"..\bin\JavaBin | java -Xms64m -Xmx256m -XX:NewRatio=3 -XX:NewSize=16m -XX:MaxNewSize=32m -jar np6-app.jar -storedbpath=../../PosData/ -localfile=""_8000_pos-db.xml""";
                            lines.Add(specialCommand);
                            updated = true;
                        }

                        if (updated)
                            System.IO.File.WriteAllLines(startNp6File, lines);
                        var liness = System.IO.File.ReadAllLines(startNp6File);
                        // Get XML files that exist in start.np6
                        var xmlFiless = Directory.GetFiles(posDatafolderPath, "*_pos-db.xml")
                            .Select(System.IO.Path.GetFileName)
                            .Where(file => liness.Any(line => line.Contains(file))) // Only add buttons for files in start.np6
                            .ToList();

                        foreach (var file in xmlFiless)
                        {
                            string displayName = file.Replace("_", "").Replace("pos-db.xml", ""); // Remove undesired parts
                            Button btn = new Button
                            {

                                Content = displayName,
                                Background = IsFileEnabledInStartNp6(file) ? Brushes.Green : Brushes.Transparent,
                                Foreground = Brushes.White,
                                Margin = new Thickness(5),
                                Tag = file,
                                Padding = new Thickness(8),
                                FontSize = 12,
                                FontWeight = FontWeights.DemiBold,
                                Width = 200,
                                HorizontalAlignment = HorizontalAlignment.Left
                            };

                            btn.Click += ToggleButtonState;
                            buildNeedRun.Children.Add(btn);
                        }

                        CommentNecessaryParamters(posDatafolderPath);
                        ChangeStoredbParameters(posDatafolderPath + "\\store-db.xml");
                        ChangeSecurityData(posDatafolderPath + "\\Security.data");
                        ChangeRegData(binFolderPath);
                        homeBuildContent.Visibility = Visibility.Visible;
                        startRunBuild.Visibility = Visibility.Visible;
                    }
                    else
                        MessageBox.Show("Unable to find Start.np6 in bat");
                }

            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void version36()
        {
            try
            {

           
            bool proceed = true;
            string parentFolderPath = System.IO.Path.GetDirectoryName(posDatafolderPath);


            // Check if specific subfolders exist in the parent folder
            string[] folderNames = { "NpSharpBin", "Bat", "Drivers" };

            // Get all directories in the parent folder
            string[] directories = Directory.GetDirectories(parentFolderPath);

            // Check if the required folders exist (case-insensitive)
            foreach (var folderName in folderNames)
            {
                bool folderExists = Array.Exists(directories, dir =>
                    string.Equals(System.IO.Path.GetFileName(dir), folderName, StringComparison.OrdinalIgnoreCase));

                if (!folderExists)
                {
                    MessageBox.Show(folderName + " Not Available");
                    proceed = false;
                }

            }

            if (proceed)
            {
                bool filenotavailable = false;
                string[] requiredFiles = { "_clean.bat", "_runLoad.bat", "_stop_ALL.bat" };
                string extractPath = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(posDatafolderPath))).FullName;

                foreach (string file in requiredFiles)
                {
                    string filePath = System.IO.Path.Combine(parentFolderPath, file);

                    if (!File.Exists(filePath))
                    {
                        filenotavailable = true;
                    }
                    else
                    {
                        Console.WriteLine($"{file} is missing.");
                    }
                }
                if (filenotavailable)
                {
                    string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
                    string zipFilePath = System.IO.Path.Combine(assetsFolder, "v36buildbats.zip");
                   

                    try
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                string relativePath = entry.FullName;

                                // If the zip contains a folder (like 'v36CashlessSimulator/'), remove it
                                if (relativePath.Contains("/"))
                                {
                                    relativePath = relativePath.Substring(relativePath.IndexOf("/") + 1);
                                }

                                // Ensure the destination path does not include the original root folder
                                string destinationPath = System.IO.Path.Combine(extractPath, relativePath);

                                // Ensure directories exist
                                string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                // Extract files (skip directories)
                                if (!string.IsNullOrEmpty(entry.Name))
                                {
                                    entry.ExtractToFile(destinationPath, overwrite: true);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception :" + ex);
                    }
                }


                string batFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bat", StringComparison.OrdinalIgnoreCase));
                string binFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bin", StringComparison.OrdinalIgnoreCase));

                if (binFolderPath == null)
                {
                   
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(posDatafolderPath) + "\\Bin");
                    binFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bin", StringComparison.OrdinalIgnoreCase));

                }

                startNp6File = System.IO.Path.Combine(batFolderPath, "start.np6");

                System.IO.File.WriteAllText(startNp6File, string.Empty);

                if (System.IO.File.Exists(startNp6File))
                {

                    var lines = System.IO.File.ReadAllLines(startNp6File).ToList();

                    // Get all valid XML file names
                    var xmlFiles = Directory.GetFiles(posDatafolderPath, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => !file.Contains("Np6PosCore") && !file.Contains("npsharp") && !file.Contains("np6WayCore"))
                        .ToList();

                    bool updated = false;
                    bool specialFileExists = lines.Any(line => line.Contains("_8000_pos-db.xml"));

                    // Normal command format for all XML files except _8000_pos-db.xml
                    foreach (var file in xmlFiles)
                    {
                        string command = $@"..\bin\ | npapp.exe ""..\PosData\{file}"" ""..\OUT"" ""..\TEMP"" pos-log61.properties";

                        // Add only if missing
                        if (!lines.Any(line => line.Contains(file)))
                        {
                            if (!file.Contains(specialFile))
                                lines.Add(";" + command); // Disabled by default
                            updated = true;
                        }
                    }

                    // Special handling for _8000_pos-db.xml
                    if (!specialFileExists)
                    {
                        // If _8000_pos-db.xml is completely missing, add the Java command
                        string specialCommand = @"..\bin\JavaBin | java -Xms64m -Xmx256m -XX:NewRatio=3 -XX:NewSize=16m -XX:MaxNewSize=32m -jar np6-app.jar -storedbpath=../../PosData/ -localfile=""_8000_pos-db.xml""";
                        lines.Add(specialCommand);
                        updated = true;
                    }

                    if (updated)
                        System.IO.File.WriteAllLines(startNp6File, lines);
                    var liness = System.IO.File.ReadAllLines(startNp6File);
                    // Get XML files that exist in start.np6
                    var xmlFiless = Directory.GetFiles(posDatafolderPath, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => liness.Any(line => line.Contains(file))) // Only add buttons for files in start.np6
                        .ToList();

                    foreach (var file in xmlFiless)
                    {
                        string displayName = file.Replace("_", "").Replace("pos-db.xml", ""); // Remove undesired parts
                        Button btn = new Button
                        {

                            Content = displayName,
                            Background = IsFileEnabledInStartNp6(file) ? Brushes.Green : Brushes.Transparent,
                            Foreground = Brushes.White,
                            Margin = new Thickness(5),
                            Tag = file,
                            Padding = new Thickness(8),
                            FontSize = 12,
                            FontWeight = FontWeights.DemiBold,
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        btn.Click += ToggleButtonState;
                        buildNeedRun.Children.Add(btn);
                    }

                   CommentNecessaryParamters(posDatafolderPath);
                   ChangeStoredbParameters(posDatafolderPath + "\\store-db.xml");
                    ChangeSecurityData(posDatafolderPath + "\\Security.data");
                    ChangeRegData(binFolderPath);
                    homeBuildContent.Visibility = Visibility.Visible;
                    startRunBuild.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Unable to find Start.np6 in bat");
            }
        }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
}


        private void ChangeRegData(string binFolderPath)
        {
           
            string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
            string sourcePath = System.IO.Path.Combine(assetsFolder, "regdata.gz");
           
          
            
            // Get the "javaBin" folder inside "Bin"
            string javaBin = Directory
                .EnumerateDirectories(binFolderPath)
                .FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "JavaBin", StringComparison.OrdinalIgnoreCase));

            if (javaBin == null)
            {
                Directory.CreateDirectory(binFolderPath+"\\JavaBin");
                javaBin = Directory
                .EnumerateDirectories(binFolderPath)
                .FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "JavaBin", StringComparison.OrdinalIgnoreCase));
                // return;
            }

            if (!System.IO.File.Exists(sourcePath))
            {
                MessageBox.Show("Source file not found: " + sourcePath);
                return;
            }

            // Destination file paths
            string destBinPath = System.IO.Path.Combine(binFolderPath, "regdata.gz");
            string destJavaBinPath = System.IO.Path.Combine(javaBin, "regdata.gz");

            // Copy the file to both locations
            System.IO.File.Copy(sourcePath, destBinPath, true);
            System.IO.File.Copy(sourcePath, destJavaBinPath, true);
           

        }

        private void ChangeSecurityData(string data)
        {
            string ndata = "1,Ricardo Caram,86,99991231,b7cdd06a367c3478a905d3bb2ed771a,74d932ff74642f2ebe3e8ea389d5a066\r\n2,Allan Shepard,86,99991231,4e4df1e0d9a172e240c5661f6b4e8d1,dfc48ef6f26494b154c750ced82d984\r\n3,Carl Sagan,90,99991231,1fb29bb392c5556ef6e45c4339865,5a957d5d21980edef43626cc053bb79\r\n4,Roger Ingold,0,99991231,43257944f62b4deb6fd40e14ba86f5,e9e382b66bfced803910d8cfec2ecbc\r\n5,Johnanthan Hart,0,99991231,38d377ccebd07cfbdb26793598d75242,3b53978ace9753e32ff5497c89e146\r\n6,Neil Armstrong,0,99991231,57ec7c7a8a62a9b5dd74984386b63ba3,fa5e555d78065399aa32e57138769f0\r\n";
            System.IO.File.WriteAllText(data, ndata);

        }

        private void ChangeStoredbParameters(string folderPath)
        {
            var posdataxml = System.IO.File.ReadAllText(folderPath);
            string updatedContent = posdataxml;
            List<string> targetNames = new List<string>
        {

            "ATFiscal",
            "Fiscal",
            "FiscalPluginConfig",
            "Fiscal",
            "FiscalRebootScreenNbr",
            "actionDuringOpenDay",
            "restartAfterOpen",
            "opos.cashdrawer",
            "opos.linedisplay",
            "opos.cardreader",
            "opos.scanner"



        };


            var lines = System.IO.File.ReadAllLines(folderPath).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                foreach (var target in targetNames)
                {
                    if (lines[i].Contains("Adaptor") && lines[i].Contains("standard.account"))
                    {
                        lines[i] = "<Adaptor type=\"standard.account\" startonload=\"false\">";
                    }

                    if (lines[i].Contains("Parameter") && lines[i].Contains("rebootAfterOpen"))
                    {
                        lines[i] = "<Parameter name=\"rebootAfterOpen\" value=\"false\"/>";
                    }
                    if (lines[i].Contains("Parameter") && lines[i].Contains("enableTSE"))
                    {
                        lines[i] = "<Parameter name=\"enableTSE\" value=\"false\"/>";
                    }
                 
  //if (lines[i].Contains("Parameter") && lines[i].Contains("restartAfterOpen"))
  //                  {
  //                      lines[i] = "<Parameter name=\"restartAfterOpen\" value=\"false\"/>";
  //                  }

                    if (lines[i].Contains("Parameter") && lines[i].Contains("showCursor"))
                    {
                        lines[i] = "<Parameter name=\"showCursor\" value=\"true\"/>";
                    }



                    if (lines[i].Contains("Parameter") && lines[i].Contains("managerAuthorization"))
                    {
                        lines[i] = "<Parameter name=\"managerAuthorization\" value=\"login\"/>";
                    }
                    if (lines[i].Contains("Parameter") && lines[i].Contains("networkAdaptorBaseIp"))
                    {
                        lines[i] = "<Parameter name=\"networkAdaptorBaseIp\" value=\"127.0.0.1\"/>";
                    }
                 


                    if (lines[i].Contains("Adaptor") && lines[i].Contains("nps.xmlrpc"))
                    {
                        for (; !(lines[i].Contains("</Adaptor>")); i++)
                        {
                            if (lines[i].Contains("Parameter") && lines[i].Contains("url"))
                                lines[i] = "<Parameter name=\"url\" value=\"http://127.0.0.1:8080/goform/RPC2\"/>";

                        }

                    }

                    if (lines[i].Contains("Adaptor") && lines[i].Contains("xmlrpccli"))
                    {
                        for (; !(lines[i].Contains("</Adaptor>")); i++)
                        {
                            if (lines[i].Contains("Parameter") && lines[i].Contains("url"))
                                lines[i] = "<Parameter name=\"url\" value=\"http://127.0.0.1:8080/goform/RPC2\"/>";

                        }

                    }

                    if (lines[i].Contains("Section") && lines[i].Contains("Multichannel.MOP"))
                    {
                        for (; !(lines[i].Contains("</Section>")); i++)
                        {
                            if (lines[i].Contains("Parameter") && lines[i].Contains("ServerEndpoint"))
                                lines[i] = "<Parameter name=\"ServerEndpoint\" value=\"http://127.0.0.1:8197\" />";

                        }

                    }




                    if (lines[i].Contains("Section") && lines[i].Contains("Multichannel.Plexure"))
                    {
                        for (; !(lines[i].Contains("</Section>")); i++)
                        {
                           if (lines[i].Contains("Parameter") && lines[i].Contains("ServerEndpoint"))
                               lines[i] = "<Parameter name=\"ServerEndpoint\" value=\"http://127.0.0.1:8197\" />";

                       }

                    }
                    if (lines[i].Contains("Adaptor") && lines[i].Contains("foe.xmlrpc"))
                    {
                        for (; !(lines[i].Contains("</Adaptor>")); i++)
                        {
                            if (lines[i].Contains("Parameter") && lines[i].Contains("port"))
                                lines[i] = "<Parameter name=\"port\" value=\"8089\"/>";

                        }

                    }

                    if (lines[i].Contains("Parameter") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                    {
                        lines[i] = $"<!-- {lines[i]} -->"; // Comment the line
                    }

                    if (lines[i].Contains("Section") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                    {
                        do
                        {
                            lines[i] = $"<!-- {lines[i]} -->";
                            i++;
                        } while (!lines[i].Contains("</Section>"));

                        lines[i] = $"<!-- {lines[i]} -->";
                    }
                    if (lines[i].Contains("Adaptor") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                    {
                        do
                        {
                            lines[i] = $"<!-- {lines[i]} -->";
                            i++;
                        } while (!lines[i].Contains("</Adaptor>"));
                        lines[i] = $"<!-- {lines[i]} -->";
                    }
                }
            }

            // Write updated content back to file
            System.IO.File.WriteAllLines(folderPath, lines);
        }



        private void CommentNecessaryParamters(string folderPath)
        {
            var xmlFiles = Directory.GetFiles(folderPath, "*_pos-db.xml");

            foreach (var file in xmlFiles)
            {


                List<string> targetNames = new List<string>
        {
            "networkAdaptorBaseIp",
            "serverPort",
            "Component.DriverHost.Printer",
            "FiscalMarket",
            "DSLDevDrv.cardreader",
            "DSLDevDrv.linedisplay",
            "nps.BarcodeReader",
            "opos.linedisplay",
            "nps.extension.npsExtATGiftCard",
            "Component.DriverHost.LineDisplay",
         //   "Component.DriverHost.Cashless",
            "Component.DriverHost.CashDrawer",
            "Component.DriverHost.Scanner",
            "opos.cashdrawer",
            "opos.scanner",
            "panasonic.card.reader",
            "panasonic.cash.drawer",
            "panasonic.line.display",
            "multicastPort",
            "multicastTTL",
            "np6fiscal.linedisplay",
            "DSLDevDrv.cash",
            "Fiscal",
            "FiscalRebootScreenNbr",
            "managerAuthorization",
            "Component.DriverHost.GrillPrinter01",
            "Component.DriverHost.ReceiptPrinter101"


        };



                var lines = System.IO.File.ReadAllLines(file).ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    foreach (var target in targetNames)
                    {

                        //if (lines[i].Contains("Section") && lines[i].Contains("Component.Browser.Kiosk"))
                        //{
                        //    while (!lines[i].Contains("</Section>"))
                        //    {
                        //        if (lines[i].Contains("Size"))
                        //            lines[i] = "<Parameter name=\"Size\" value=\"1080x1920\" />";
                        //    }
                        //}

                        if (lines[i].Contains("Parameter") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                        {
                            lines[i] = $"<!-- {lines[i]} -->"; // Comment the line
                        }

                        if (lines[i].Contains("Section") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                        {
                            do
                            {
                                if (lines[i].Contains("<!--"))
                                    lines[i] = lines[i];
                                else
                                    lines[i] = $"<!-- {lines[i]} -->";
                                i++;
                            } while (!lines[i].Contains("</Section>"));

                            lines[i] = $"<!-- {lines[i]} -->";
                        }
                        if (lines[i].Contains("Adaptor") && lines[i].Contains(target) && !lines[i].Contains("<!--"))
                        {
                            do
                            {
                                lines[i] = $"<!-- {lines[i]} -->";
                                i++;
                            } while (!lines[i].Contains("</Adaptor>"));
                            lines[i] = $"<!-- {lines[i]} -->";
                        }

                        if (lines[i].Contains("Parameter") && lines[i].Contains("ShowCursor"))
                        {
                            lines[i] = "<Parameter name=\"ShowCursor\" value=\"true\"/>";
                        }

                        if (lines[i].Contains("Parameter") && lines[i].Contains("checkPrinterStatus"))
                        {
                            lines[i] = "<Parameter name=\"checkPrinterStatus\" value=\"false\"/>";
                        }

                        if (lines[i].Contains("Service") && lines[i].Contains("NPW"))
                        {

                            lines[i] = lines[i].Replace("startonload=\"true\"", "startonload=\"false\"");

                        }
                        if (lines[i].Contains("Parameter") && lines[i].Contains("PrinterStatusCheckOff"))
                        {
                            lines[i] = "<Parameter name=\"PrinterStatusCheckOff\" value=\"1\"/>";
                        }

                    
                    }
                }
                // Only update the file if changes were made


                System.IO.File.WriteAllLines(file, lines);

            }
            // Comment out single-line elements containing name="targetName"





        }

        private void ToggleButtonState(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string fileName = (string)btn.Tag;  // Use full filename for logic

            // Read start.np6 file
            var lines = System.IO.File.ReadAllLines(startNp6File).ToList();

            // Toggle `;` before command
            int index = lines.FindIndex(line => line.Contains(fileName));
            if (index != -1)
            {
                if (lines[index].StartsWith(";"))
                    lines[index] = lines[index].Substring(1); // Remove `;` to enable
                else
                    lines[index] = ";" + lines[index]; // Add `;` to disable
            }

            // Write back changes
            System.IO.File.WriteAllLines(startNp6File, lines);

            // Toggle button color
            btn.Background = btn.Background == Brushes.Green ? Brushes.Transparent : Brushes.Green;

            // Keep the shortened name
            btn.Content = fileName.Replace("_", "").Replace("pos-db.xml", "");
        }

        private void KioskToggleButtonState(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string fileName = (string)btn.Tag;  // Use full filename for logic

            // Read start.np6 file
            var lines = System.IO.File.ReadAllLines(kiosknp6file).ToList();

            // Toggle `;` before command
            int index = lines.FindIndex(line => line.Contains(fileName));
            if (index != -1)
            {
                if (lines[index].StartsWith(";"))
                    lines[index] = lines[index].Substring(1); // Remove `;` to enable
                else
                    lines[index] = ";" + lines[index]; // Add `;` to disable
            }

            // Write back changes
            System.IO.File.WriteAllLines(kiosknp6file, lines);

            // Toggle button color
            btn.Background = btn.Background == Brushes.Green ? Brushes.Transparent : Brushes.Green;

            // Keep the shortened name
            btn.Content = fileName.Replace("_", "").Replace("pos-db.xml", "");
        }

        private void WayToggleButtonState(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string fileName = (string)btn.Tag;  // Use full filename for logic

            // Read start.np6 file
            var lines = System.IO.File.ReadAllLines(waynp6file).ToList();

            // Toggle `;` before command
            int index = lines.FindIndex(line => line.Contains(fileName));
            if (index != -1)
            {
                if (lines[index].StartsWith(";"))
                    lines[index] = lines[index].Substring(1); // Remove `;` to enable
                else
                    lines[index] = ";" + lines[index]; // Add `;` to disable
            }

            // Write back changes
            System.IO.File.WriteAllLines(waynp6file, lines);

            // Toggle button color
            btn.Background = btn.Background == Brushes.Green ? Brushes.Transparent : Brushes.Green;

            // Keep the shortened name
            btn.Content = fileName.Replace("_", "").Replace("pos-db.xml", "");
        }






        private bool IsFileEnabledInStartNp6(string fileName)
        {
            if (!System.IO.File.Exists(startNp6File)) return false;
            var lines = System.IO.File.ReadAllLines(startNp6File);
            return lines.Any(line => line.Contains(fileName) && !line.StartsWith(";"));
        }

        private bool wayIsFileEnabledInStartNp6(string fileName)
        {
            if (!System.IO.File.Exists(waynp6file)) return false;
            var lines = System.IO.File.ReadAllLines(waynp6file);
            return lines.Any(line => line.Contains(fileName) && !line.StartsWith(";"));
        }
        private bool kioskIsFileEnabledInStartNp6(string fileName)
        {
            if (!System.IO.File.Exists(kiosknp6file)) return false;
            var lines = System.IO.File.ReadAllLines(kiosknp6file);
            return lines.Any(line => line.Contains(fileName) && !line.StartsWith(";"));
        }

        private void startRunBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(version.Equals("30"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        // FileName = "_runLoad.bat", // Your batch file
                        FileName = "_NP61_StartNPsharpApp.bat",
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();

                    startCleanBuild.Visibility = Visibility.Visible;
                    startStopBuild.Visibility = Visibility.Visible;
                }
                if(version.Equals("36"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        // FileName = "_runLoad.bat", // Your batch file
                        FileName = "_runLoad.bat",
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();

                    startCleanBuild.Visibility = Visibility.Visible;
                    startStopBuild.Visibility = Visibility.Visible;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void startStopBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (version.Equals("30"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "_np6x_Close_All_NewPOS6x_ Apps.bat", // Your batch file
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();
                }
                if (version.Equals("36"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "_stop_ALL.bat", // Your batch file
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void startCleanBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (version.Equals("36"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "_clean.bat", // Your batch file
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();
                }
                if (version.Equals("30"))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "clean.bat", // Your batch file
                        WorkingDirectory = System.IO.Path.GetDirectoryName(posDatafolderPath), // Change this to the desired folder
                        UseShellExecute = true,
                        Verb = "runas" // This requests admin privileges
                    };

                    Process process = new Process { StartInfo = psi };
                    process.Start();
                }
                }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void foeDoStore_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                foeDoStore.Background = Brushes.Green;
                foeDoStore.BorderBrush = Brushes.Green;
                foeTransferFromStaging.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeTransferFromStaging.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
                foeDoStoreStaging.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeDoStoreStaging.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));

                string workingDir = foesimulatorpath;
                string exePath = System.IO.Path.Combine(workingDir, "bin", "xmlrpc_client.exe");
                string xmlFile = System.IO.Path.Combine(workingDir, "test_prod.xml");

                if (!File.Exists(exePath))
                {
                   MessageBox.Show("Error: xmlrpc_client.exe not found at " + exePath);
                    return;
                }

                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Error: test_prod.xml not found at " + xmlFile);
                    return;
                }
                else
                {
                    File.WriteAllText(xmlFile, prodinfoBox.Text);
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"DoFoeStoreFromFile {xmlFile}",
                    WorkingDirectory = System.IO.Path.Combine(workingDir, "bin"), // Set to bin directory
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd(); // Get standard output
                    string error = process.StandardError.ReadToEnd(); // Get error output

                    process.WaitForExit(); // Wait for the process to finish

                   MessageBox.Show("Response :\n" + output);
                    MessageBox.Show("Error :\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void foeDoStoreStaging_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foeDoStoreStaging.Background = Brushes.Green;
                foeDoStoreStaging.BorderBrush = Brushes.Green;
                foeTransferFromStaging.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeTransferFromStaging.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
                foeDoStore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeDoStore.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));

                string workingDir = foesimulatorpath;
                string exePath = System.IO.Path.Combine(workingDir, "bin", "xmlrpc_client.exe");
                string xmlFile = System.IO.Path.Combine(workingDir, "test_prod.xml");

                if (!File.Exists(exePath))
                {
                    MessageBox.Show("Error: xmlrpc_client.exe not found at " + exePath);
                    return;
                }

                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Error: test_prod.xml not found at " + xmlFile);
                    return;
                }
                else
                {
                    File.WriteAllText(xmlFile, prodinfoBox.Text);
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"DoFoeStoreStagingFromFile {xmlFile}",
                    WorkingDirectory = System.IO.Path.Combine(workingDir, "bin"), // Set to bin directory
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd(); // Get standard output
                    string error = process.StandardError.ReadToEnd(); // Get error output

                    process.WaitForExit(); // Wait for the process to finish
                    MessageBox.Show("Response :\n" + output);
                    MessageBox.Show("Error :\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void foeTransferFromStaging_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foeTransferFromStaging.Background = Brushes.Green;
                foeTransferFromStaging.BorderBrush = Brushes.Green;
                foeDoStoreStaging.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeDoStoreStaging.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
                foeDoStore.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                foeDoStore.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));

                string workingDir = foesimulatorpath;
                string exePath = System.IO.Path.Combine(workingDir, "bin", "xmlrpc_client.exe");
                string xmlFile = System.IO.Path.Combine(workingDir, "test_prod.xml");

                if (!File.Exists(exePath))
                {
                    MessageBox.Show("Error: xmlrpc_client.exe not found at " + exePath);
                    return;
                }

                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Error: test_prod.xml not found at " + xmlFile);
                    return;
                }
                else
                {
                    File.WriteAllText(xmlFile, prodinfoBox.Text);
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = $"TransferFromStagingFromFile {xmlFile}",
                    WorkingDirectory = System.IO.Path.Combine(workingDir, "bin"), // Set to bin directory
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd(); // Get standard output
                    string error = process.StandardError.ReadToEnd(); // Get error output

                    process.WaitForExit(); // Wait for the process to finish

                    MessageBox.Show("Response :\n" + output);
                    MessageBox.Show("Error :\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void foeSimulatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkposdata(posData_path.getPath()))
            {
                offerSimulatorbox.Visibility = Visibility.Collapsed;
                foeSimulatorbox.Visibility = Visibility.Visible;
                foeSimulatorButton.Background = Brushes.Green;
                foeSimulatorButton.BorderBrush = Brushes.Green;
                offerSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                offerSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
                cashlessSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
                cashlessSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));



                string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
                string zipFilePath = System.IO.Path.Combine(assetsFolder, "FOESimulator.zip");
                string extractPath = System.IO.Path.GetDirectoryName(posDatafolderPath); // Change this path

                try
                {
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(posDatafolderPath) + "\\FOESimulator"))
                    {
                      //  MessageBox.Show("FOESimulator folder already exists.");
                     
                    }
                    else
                    {
                        // Extract the zip file
                        ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                        MessageBox.Show("FOE Simulator added to Build");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }


                foesimulatorpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath), "FOESimulator");
                string xmlFilePath = System.IO.Path.Combine(foesimulatorpath, "test_prod.xml");
                if (File.Exists(xmlFilePath))
                {
                    string prodInfo = File.ReadAllText(xmlFilePath); // Read the entire file content as a string
                    prodinfoBox.Text = prodInfo; // Assign content to TextBox
                }
            }
            else
                MessageBox.Show("Please provide Build Path");
        }

        private void offerSimulatorButton_Click(object sender, RoutedEventArgs e)
        {

     
           
            if (checkposdata(posData_path.getPath()))
            {
                offerSimulatorbox.Visibility = Visibility.Visible;
                foeSimulatorbox.Visibility = Visibility.Collapsed;
                offerSimulatorButton.Background = Brushes.Green;
            offerSimulatorButton.BorderBrush = Brushes.Green;
            foeSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
            foeSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
            cashlessSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
            cashlessSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));


            string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
            string zipFilePath = System.IO.Path.Combine(assetsFolder, "OfferSimulator.zip");
            string extractPath = System.IO.Path.GetDirectoryName(posDatafolderPath); // Change this path

            try
            {
                if (Directory.Exists(System.IO.Path.GetDirectoryName(posDatafolderPath) + "\\OfferSimulator"))
                {
                    //  MessageBox.Show("FOESimulator folder already exists.");

                }
                else
                {
                    // Extract the zip file
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                    MessageBox.Show("Offer Simulator added to Build");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }


            offersimulatorpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath), "OfferSimulator\\db\\promotions");
            string xmlFilePath = System.IO.Path.Combine(offersimulatorpath, "promotion-db.xml");
                if (File.Exists(xmlFilePath))
                {
                    string promotionInfo = File.ReadAllText(xmlFilePath); // Read the entire file content as a string
                    promotioninfoBox.Text = promotionInfo; // Assign content to TextBox
                }
                else
                    MessageBox.Show(xmlFilePath +" Not found" );
        }
            else
                MessageBox.Show("Please provide Build Path");

        }


        private void runOfferSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            string workingDir = offersimulatorpath;
            string exePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath),"OfferSimulator", "OfferProviderSimulator.exe");
            string xmlFile = System.IO.Path.Combine(workingDir, "promotion-db.xml");

                if (!File.Exists(exePath))
                {
                    MessageBox.Show("Offer simulator not found" + exePath);
                    return;
                }

                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Promotion.xml not found at " + xmlFile);
                    return;
                }
                else
                {
                    File.WriteAllText(xmlFile, promotioninfoBox.Text);

                    if (File.Exists(xmlFile))
                    {
                        XDocument document = XDocument.Load(xmlFile);
                        var getversion = document.Descendants("Promotion").FirstOrDefault();
                        if (getversion != null)
                        {
                            var vsion = getversion.Attribute("id")?.Value;
                            promotionId = vsion.Split('.')[0];

                        }
                        else
                            MessageBox.Show("Promotion ID not found");
                    }
                    else
                        MessageBox.Show("Promotion xml not found");

                    byte chk = 3;
                    string couponname = "M123";
                    

                    for (int i = 1;i<=4; i++)
                    {
                        couponname = couponname + (++chk).ToString();
                        string coupon = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath), "OfferSimulator", "db", "coupons", couponname);
                        File.WriteAllText(coupon, "{\r\n  \"CustomerInfo\": {\r\n    \"CustomerId\": 123456,\r\n\t\"DisplayName\": \"Pratham\"\r\n  },\r\n  \"OfferInfo\": {\r\n    \"OfferId\": 1235,\r\n    \"PromotionId\": "+ promotionId+"\r\n  }\r\n}");
                    }
                    

                }
                Task.Run(() =>
                {

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = exePath,
                        WorkingDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath), "OfferSimulator"), // Set to bin directory
                                                                                                                                         // RedirectStandardOutput = true,
                                                                                                                                         //  RedirectStandardError = true,
                        UseShellExecute = true

                    };

                    using (Process process = new Process { StartInfo = psi })
                    {
                        process.Start();

                        //string output = process.StandardOutput.ReadToEnd(); // Get standard output
                        //string error = process.StandardError.ReadToEnd(); // Get error output

                        process.WaitForExit(); // Wait for the process to finish

                        //MessageBox.Show("Response :\n" + output);
                        //MessageBox.Show("Error :\n" + error);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }

        }


        private void cashlessSimulatorButton_Click(object sender, RoutedEventArgs e)
        {
            cashlessSimulatorButton.Background = Brushes.Green;
            cashlessSimulatorButton.BorderBrush = Brushes.Green;
            foeSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
            foeSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));
            offerSimulatorButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4D4D4D"));
            offerSimulatorButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838"));


            string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");

            if (version.Equals("30"))
            {

            }
            else if (version.Equals("36"))
            {

                string zipFilePath = System.IO.Path.Combine(assetsFolder, "v36CashlessSimulator.zip");
                string extractPath = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(posDatafolderPath), "drivers"))).FullName;

                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string relativePath = entry.FullName;

                            // If the zip contains a folder (like 'v36CashlessSimulator/'), remove it
                            if (relativePath.Contains("/"))
                            {
                                relativePath = relativePath.Substring(relativePath.IndexOf("/") + 1);
                            }

                            // Ensure the destination path does not include the original root folder
                            string destinationPath = System.IO.Path.Combine(extractPath, relativePath);

                            // Ensure directories exist
                            string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            // Extract files (skip directories)
                            if (!string.IsNullOrEmpty(entry.Name))
                            {
                                entry.ExtractToFile(destinationPath, overwrite: true);
                            }
                        }
                    }

                    MessageBox.Show("Running Cashless Simulator...");

                    try
                    {
                        MessageBox.Show(extractPath);
                        ProcessStartInfo cashless = new ProcessStartInfo
                        {
                            FileName = "CashlessSimulator.exe",
                            WorkingDirectory = extractPath,
                            Verb = "runas",
                            UseShellExecute = true

                        };
                        Process runsimulator = new Process { StartInfo = cashless };
                        runsimulator.Start();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

            }
            else
                MessageBox.Show("Please provide build path");
        }

        private void home_button_Click(object sender, RoutedEventArgs e)
        {
            home_button.Foreground = Brushes.White;
            decouple_button.Foreground = Brushes.Gray;
            file_button.Foreground = Brushes.Gray;
            
          
            startCreateBuild.Visibility = Visibility.Visible;

            hometabmenu.Visibility = Visibility.Visible;
            homeTabContent.Visibility = Visibility.Visible;
            decoupledTabmenu.Visibility = Visibility.Collapsed;
            decoupleTabContent.Visibility = Visibility.Collapsed;
            
            fileTabContent.Visibility = Visibility.Collapsed;
            fileTabmenu.Visibility = Visibility.Collapsed;
            

          
        }

        private void file_button_Click(object sender, RoutedEventArgs e)
        {
            home_button.Foreground = Brushes.Gray;
            decouple_button.Foreground = Brushes.Gray;
            file_button.Foreground = Brushes.White;

            filepath = new CustomLogPath("Path");
          
            fileTabmenu.Visibility= Visibility.Visible;
            fileTabContent.Visibility = Visibility.Visible;
            hometabmenu.Visibility= Visibility.Collapsed;
            decoupledTabmenu.Visibility = Visibility.Collapsed;
            homeTabContent.Visibility = Visibility.Collapsed;
            decoupleTabContent.Visibility = Visibility.Collapsed;
           


        }

        private void openStld_Click(object sender, RoutedEventArgs e)
        {
         string path = filepath.getPath() + "\\STLD.xml";
       //   string path = 
            try
            {
                // Read the XML content as a string
                string xmlContent = File.ReadAllText(path);

                // Replace #END# with a space
         

                // Load the modified XML content into XDocument
                XDocument doc = XDocument.Parse(xmlContent);

                // Save the formatted XML
             doc.Save(path);

              //  filecontent.Text = xmlContent;
           


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

         
        }



        private void decouple_button_Click(object sender, RoutedEventArgs e)
        {

            home_button.Foreground = Brushes.Gray;
            decouple_button.Foreground = Brushes.White;
            file_button.Foreground = Brushes.Gray;

            hometabmenu.Visibility = Visibility.Collapsed;
            homeTabContent.Visibility = Visibility.Collapsed;
            decoupledTabmenu.Visibility = Visibility.Visible;
            decoupleTabContent.Visibility = Visibility.Visible;
           fileTabContent.Visibility = Visibility.Collapsed;
            fileTabmenu.Visibility = Visibility.Collapsed;





    }

        private void DecoupleVersion36()
        {

            kioskbuildNeedRun.Children.Clear();
            bool proceed = true;
            string parentFolderPath = System.IO.Path.GetDirectoryName(decoupleKisokPosData);


            // Check if specific subfolders exist in the parent folder
            string[] folderNames = { "NpSharpBin", "Bat", "Drivers" };

            // Get all directories in the parent folder
            string[] directories = Directory.GetDirectories(parentFolderPath);

            // Check if the required folders exist (case-insensitive)
            foreach (var folderName in folderNames)
            {
                bool folderExists = Array.Exists(directories, dir =>
                    string.Equals(System.IO.Path.GetFileName(dir), folderName, StringComparison.OrdinalIgnoreCase));

                if (!folderExists)
                {
                    MessageBox.Show(folderName + " Not Available");
                    proceed = false;
                }

            }

            if (proceed)
            {
                bool filenotavailable = false;
                string[] requiredFiles = { "_clean.bat", "_run.bat", "_np6x_Close_All_NewPOS6x_ Apps.bat" };
                string extractPath = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(decoupleKisokPosData))).FullName;

                foreach (string file in requiredFiles)
                {
                    string filePath = System.IO.Path.Combine(parentFolderPath, file);

                    if (!File.Exists(filePath))
                    {
                        filenotavailable = true;
                        break;
                    }
                   
                }
                if (filenotavailable)
                {
                    string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
                    string zipFilePath = System.IO.Path.Combine(assetsFolder, "KioskBats.zip");


                    try
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                string relativePath = entry.FullName;

                                // If the zip contains a folder (like 'v36CashlessSimulator/'), remove it
                                if (relativePath.Contains("/"))
                                {
                                    relativePath = relativePath.Substring(relativePath.IndexOf("/") + 1);
                                }

                                // Ensure the destination path does not include the original root folder
                                string destinationPath = System.IO.Path.Combine(extractPath, relativePath);

                                // Ensure directories exist
                                string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                // Extract files (skip directories)
                                if (!string.IsNullOrEmpty(entry.Name))
                                {
                                    entry.ExtractToFile(destinationPath, overwrite: true);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception :" + ex);
                    }
                }


                string batFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bat", StringComparison.OrdinalIgnoreCase));
                string binFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bin", StringComparison.OrdinalIgnoreCase));

                kiosknp6file = System.IO.Path.Combine(batFolderPath, "start.np6");

                System.IO.File.WriteAllText(kiosknp6file, string.Empty);

                if (System.IO.File.Exists(kiosknp6file))
                {

                    var lines = System.IO.File.ReadAllLines(kiosknp6file).ToList();

                    // Get all valid XML file names
                    var xmlFiles = Directory.GetFiles(decoupleKisokPosData, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => !file.Contains("Np6PosCore") && !file.Contains("npsharp") && !file.Contains("np6WayCore") && file.Contains("CSO"))
                        .ToList();

                    bool updated = false;
                  

                    // Normal command format for all XML files except _8000_pos-db.xml
                    foreach (var file in xmlFiles)
                    {
                        string command = $@"..\bin\ | npapp.exe ""..\PosData\{file}"" ""..\OUT"" ""..\TEMP"" pos-log61.properties";

                        // Add only if missing
                        if (!lines.Any(line => line.Contains(file)))
                        {
                            if (!file.Contains(specialFile))
                                lines.Add(";" + command); // Disabled by default
                            updated = true;
                        }
                    }

                    // Special handling for _8000_pos-db.xml
                  

                    if (updated)
                        System.IO.File.WriteAllLines(kiosknp6file, lines);
                    var liness = System.IO.File.ReadAllLines(kiosknp6file);
                    // Get XML files that exist in start.np6
                    var xmlFiless = Directory.GetFiles(decoupleKisokPosData, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => liness.Any(line => line.Contains(file))) // Only add buttons for files in start.np6
                        .ToList();

                    foreach (var file in xmlFiless)
                    {
                        string displayName = file.Replace("_", "").Replace("pos-db.xml", ""); // Remove undesired parts
                        Button btn = new Button
                        {

                            Content = displayName,
                            Background = kioskIsFileEnabledInStartNp6(file) ? Brushes.Green : Brushes.Transparent,
                            Foreground = Brushes.White,
                            Margin = new Thickness(5),
                            Tag = file,
                            Padding = new Thickness(8),
                            FontSize = 12,
                            FontWeight = FontWeights.DemiBold,
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        btn.Click += KioskToggleButtonState;
                        kioskbuildNeedRun.Children.Add(btn);
                    }

                  
                //    V36CommentNecessaryParamters(decoupleKisokPosData);
                 //   V36ChangeStoredbParameters(decoupleKisokPosData + "\\store-db.xml");
                    ChangeSecurityData(decoupleKisokPosData + "\\Security.data");
                    ChangeRegData(binFolderPath);
                   // homeBuildContent.Visibility = Visibility.Visible;
                  //  startRunBuild.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Unable to find Start.np6 in bat");
            }

        }
        private void DecoupleVversion30()
        {
            waybuildNeedRun.Children.Clear();
            bool proceed = true;
            string parentFolderPath = System.IO.Path.GetDirectoryName(decoupleWayPosdata);


            // Check if specific subfolders exist in the parent folder
            string[] folderNames = { "NpSharpBin", "Bat", "Bin" };

            // Get all directories in the parent folder
            string[] directories = Directory.GetDirectories(parentFolderPath);

            // Check if the required folders exist (case-insensitive)
            foreach (var folderName in folderNames)
            {
                bool folderExists = Array.Exists(directories, dir =>
                    string.Equals(System.IO.Path.GetFileName(dir), folderName, StringComparison.OrdinalIgnoreCase));

                if (!folderExists)
                {
                    MessageBox.Show(folderName + " Not Available");
                    proceed = false;
                }

            }

            if (proceed)
            {

                bool filenotavailable = false;
                string[] requiredFiles = { "_clean.bat", "_run.bat", "_np6x_Close_All_NewPOS6x_ Apps.bat" };
                string extractPath = new DirectoryInfo(System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(decoupleWayPosdata))).FullName;

                foreach (string file in requiredFiles)
                {
                    string filePath = System.IO.Path.Combine(parentFolderPath, file);
                    //   MessageBox.Show(filePath);
                    if (File.Exists(filePath))
                    {
                        filenotavailable = true;
                        break;
                    }
                    else
                    {
                        // MessageBox.Show($"{file} is missing.");
                    }
                }
                if (filenotavailable)
                {
                    string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string assetsFolder = System.IO.Path.Combine(projectFolder, "Assets");
                    string zipFilePath = System.IO.Path.Combine(assetsFolder, "WaystationBats.zip");

                  
                    try
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                string relativePath = entry.FullName;

                                // If the zip contains a folder (like 'v36CashlessSimulator/'), remove it
                                if (relativePath.Contains("/"))
                                {
                                    relativePath = relativePath.Substring(relativePath.IndexOf("/") + 1);
                                }

                                // Ensure the destination path does not include the original root folder
                                string destinationPath = System.IO.Path.Combine(extractPath, relativePath);

                                // Ensure directories exist
                                string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                // Extract files (skip directories)
                                if (!string.IsNullOrEmpty(entry.Name))
                                {
                                    entry.ExtractToFile(destinationPath, overwrite: true);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception :" + ex);
                    }
                }

                string batFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bat", StringComparison.OrdinalIgnoreCase));
                string binFolderPath = Directory.EnumerateDirectories(extractPath).FirstOrDefault(dir => string.Equals(System.IO.Path.GetFileName(dir), "Bin", StringComparison.OrdinalIgnoreCase));

                waynp6file = System.IO.Path.Combine(batFolderPath, "start.np6");

                System.IO.File.WriteAllText(waynp6file, string.Empty);

                if (System.IO.File.Exists(waynp6file))
                {

                    var lines = System.IO.File.ReadAllLines(waynp6file).ToList();

                    // Get all valid XML file names
                    var xmlFiles = Directory.GetFiles(decoupleWayPosdata, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => !file.Contains("Np6PosCore") && !file.Contains("npsharp") && !file.Contains("np6WayCore")&& !file.Contains("CSO"))
                        .ToList();

                    bool updated = false;
                    bool specialFileExists = lines.Any(line => line.Contains("_8000_pos-db.xml"));

                    // Normal command format for all XML files except _8000_pos-db.xml
                    foreach (var file in xmlFiles)
                    {
                        string command = $@"..\bin\ | npapp.exe ""..\PosData\{file}"" ""..\OUT"" ""..\TEMP"" pos-log61.properties";

                        // Add only if missing
                        if (!lines.Any(line => line.Contains(file)))
                        {
                            if (!file.Contains(wayspecialFile))
                                lines.Add(";" + command); // Disabled by default
                            updated = true;
                        }
                    }

                    // Special handling for _8000_pos-db.xml
                    if (!specialFileExists)
                    {
                        // If _8000_pos-db.xml is completely missing, add the Java command
                        string specialCommand = @"..\bin\JavaBin | java -Xms64m -Xmx256m -XX:NewRatio=3 -XX:NewSize=16m -XX:MaxNewSize=32m -jar np6-app.jar -storedbpath=../../PosData/ -localfile=""_8000_pos-db.xml""";
                        lines.Add(specialCommand);
                        updated = true;
                    }

                    if (updated)
                        System.IO.File.WriteAllLines(waynp6file, lines);
                    var liness = System.IO.File.ReadAllLines(waynp6file);
                    // Get XML files that exist in start.np6
                    var xmlFiless = Directory.GetFiles(decoupleWayPosdata, "*_pos-db.xml")
                        .Select(System.IO.Path.GetFileName)
                        .Where(file => liness.Any(line => line.Contains(file))) // Only add buttons for files in start.np6
                        .ToList();

                    foreach (var file in xmlFiless)
                    {
                        string displayName = file.Replace("_", "").Replace("pos-db.xml", ""); // Remove undesired parts
                        Button btn = new Button
                        {

                            Content = displayName,
                            Background = wayIsFileEnabledInStartNp6(file) ? Brushes.Green : Brushes.Transparent,
                            Foreground = Brushes.White,
                            Margin = new Thickness(5),
                            Tag = file,
                            Padding = new Thickness(8),
                            FontSize = 12,
                            FontWeight = FontWeights.DemiBold,
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        btn.Click += WayToggleButtonState;
                        
                        waybuildNeedRun.Children.Add(btn);
                    }

                  //  V36CommentNecessaryParamters(posDatafolderPath);
                 //   V36ChangeStoredbParameters(posDatafolderPath + "\\store-db.xml");
                    ChangeSecurityData(decoupleWayPosdata + "\\Security.data");
                    ChangeRegData(binFolderPath);
                    //   homeBuildContent.Visibility = Visibility.Visible;
                    //     startRunBuild.Visibility = Visibility.Visible;
                    decoupledRunCreateBuild.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Unable to find Start.np6 in bat");
            }
           
        }

        string decoupleKisokPosData = "";
        string decoupleWayPosdata = "";

        private void decoupledstartCreateBuild_Click(object sender, RoutedEventArgs e)
        {

            if (checkposdata(decoupleKioskpath.getPath()))
            {

                decoupleKisokPosData = decoupleKioskpath.getPath();

                if (File.Exists(decoupleKisokPosData + "\\product.specification"))
                {
                    XDocument document = XDocument.Load(decoupleKisokPosData + "\\product.specification");
                    var getversion = document.Descendants("NpSharp").FirstOrDefault();
                    if (getversion != null)
                    {
                        var vsion = getversion.Attribute("version")?.Value;
                        version = vsion.Split('.')[0];


                    }
                    else
                        MessageBox.Show("Version not found");

                    ChangeSecurityData(decoupleKisokPosData + "\\Security.data");

                }
                else
                    MessageBox.Show("Product specification not found");

  

                if (version.Equals("36"))
                    DecoupleVersion36();
                else
                    MessageBox.Show("Not Found Version");

            }
            else
                MessageBox.Show("Please provide valid Kisok PosData path");



            if (checkposdata(decoupleWaypath.getPath()))
            {

                decoupleWayPosdata = decoupleWaypath.getPath();

                if (File.Exists(decoupleWayPosdata + "\\product.specification"))
                {
                    XDocument document = XDocument.Load(decoupleWayPosdata + "\\product.specification");
                    var getversion = document.Descendants("NpSharp").FirstOrDefault();
                    if (getversion != null)
                    {
                        var vsion = getversion.Attribute("version")?.Value;
                        version = vsion.Split('.')[0];
                    

                    }
                    else
                        MessageBox.Show("Version not found");


                    ChangeSecurityData(decoupleWayPosdata+"\\Security.data");
                }
                else
                    MessageBox.Show("Product specification not found");



                if (version.Equals("30"))
                    DecoupleVversion30();
                else
                    MessageBox.Show("Not Found Version");


            }
            else
                MessageBox.Show("Please provide valid Waystation PosData path");
    

        }

        private void decoupledRunCreateBuild_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_run.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleKisokPosData), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process = new Process { StartInfo = psi };
                process.Start();

                decoupledStopCreateBuild.Visibility = Visibility.Visible;
                decoupledCleanCreateBuild.Visibility = Visibility.Visible;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            try
            {


                ProcessStartInfo psi = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_run.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleWayPosdata), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process = new Process { StartInfo = psi };
                process.Start();

                decoupledStopCreateBuild.Visibility = Visibility.Visible;
                decoupledCleanCreateBuild.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void decoupledStopCreateBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {

       
                ProcessStartInfo psi1 = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_np6x_Close_All_NewPOS6x_ Apps.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleKisokPosData), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process1 = new Process { StartInfo = psi1 };
                process1.Start();

                ProcessStartInfo psi2 = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_np6x_Close_All_NewPOS6x_ Apps.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleWayPosdata), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process2 = new Process { StartInfo = psi2 };
                process2.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }

        private void decoupledCleanCreateBuild_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ProcessStartInfo psi1 = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_clean.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleKisokPosData), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process1 = new Process { StartInfo = psi1 };
                process1.Start();

                ProcessStartInfo psi2 = new ProcessStartInfo
                {
                    // FileName = "_runLoad.bat", // Your batch file
                    FileName = "_clean.bat",
                    WorkingDirectory = System.IO.Path.GetDirectoryName(decoupleWayPosdata), // Change this to the desired folder
                    UseShellExecute = true,
                    Verb = "runas" // This requests admin privileges
                };

                Process process2 = new Process { StartInfo = psi2 };
                process2.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void openanyfile_Click(object sender, RoutedEventArgs e)
        {
            string parsexml = "";
            string xml = "";

            try
            {


                openFileDialog.Filter = "XML Files (*.xml)|*.xml";
                if (openFileDialog.ShowDialog() == true)
                {
                    if (openFileDialog.FileName.ToString().Contains("STLD.xml"))
                    {
                        parsexml = File.ReadAllText(openFileDialog.FileName);
                        parsexml = parsexml.Replace("#END#", " ");
                        XDocument xmldoc = XDocument.Parse(parsexml);
                        xmldoc.Save(openFileDialog.FileName);
                        MessageBox.Show("STLD is Created please open in NotePad++");
                        return;
                    }

                    parsexml = File.ReadAllText(openFileDialog.FileName);
                    // xmlContent = xmlContent.Replace("#END#", " ");


                    XDocument doc = XDocument.Parse(parsexml);
                    doc.Save(openFileDialog.FileName);
                    xml = File.ReadAllText(openFileDialog.FileName);

                }

                Paragraph para = new Paragraph();
                Regex regex = new Regex(@"(</?[^<>]+?>)|([^<>]+)", RegexOptions.Compiled);

                foreach (Match match in regex.Matches(xml))
                {
                    string value = match.Value;

                    Run run = new Run(value);

                    if (value.StartsWith("<"))
                    {
                        // Highlight tags
                        run.Foreground = Brushes.Blue;

                        if (value.Contains("="))
                            run.Foreground = Brushes.DarkCyan;
                    }
                    else
                    {
                        // Text inside tags
                        run.Foreground = Brushes.Black;
                    }

                    para.Inlines.Add(run);
                }

                richTextBox.Document = new FlowDocument(para);
                savefile.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void savefile_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            File.WriteAllText(openFileDialog.FileName, textRange.Text.Trim());
            MessageBox.Show("Saved");

        }

     
    }
   
}
