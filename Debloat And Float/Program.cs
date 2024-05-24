using IWshRuntimeLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace DebloatAndFloat
{
    [Serializable]
    public class AppSettings
    {
        public bool ShowRemoveRABloatButton { get; set; }
        public bool ShowRemoveCustomButtons { get; set; }
        public bool ShowRemoveOptionalButtons { get; set; }
        public bool ShowRemoveProgramButtons { get; set; }
        public bool ShowRemoveDebloatButtons { get; set; }
        public bool ShowRemoveRepairButtons { get; set; }
        public bool ShrinkTextBox { get; set; }

        public decimal monitorsettingAC { get; set; }
        public decimal sleepsettingAC { get; set; }
        public decimal monitorsettingDC { get; set; }
        public decimal sleepsettingDC { get; set; }

        // Add more settings as needed
    }

    public class SoftwareSettings
    {
        public string bloatWare { get; set; }
        public string whiteListedApps { get; set; }
        public string nonRemovable { get; set; }

        // Add more settings as needed
    }

    internal class MainForm : Form
    {
        private TextBox textBox1;
        private AppSettings appSettings;
        private SoftwareSettings softwareSettings;
        private int frameIndex;
        private Bitmap buffer;
        private string logpath = "C:/Temp/DebloatAndFloat/Logs/";

        public MainForm()
        {
            //GIU Setup
            this.ClientSize = new System.Drawing.Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            this.Text = "Debloat And Float";
            this.TopMost = false;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#252525");

            // Debloat Options

            Label pleaseWaitLabel = new Label();
            pleaseWaitLabel.Text = "Please Wait...";
            pleaseWaitLabel.Location = new System.Drawing.Point(20, 30);
            pleaseWaitLabel.Size = new System.Drawing.Size(660, 470);
            pleaseWaitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            pleaseWaitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 100);
            pleaseWaitLabel.ForeColor = System.Drawing.Color.Red;
            pleaseWaitLabel.Visible = false;

            Button Settingsbutton = new Button();
            Settingsbutton.Text = "Settings";
            Settingsbutton.Width = 80;
            Settingsbutton.Height = 25;
            Settingsbutton.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            Settingsbutton.Location = new System.Drawing.Point(12, 570);
            Settingsbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            Settingsbutton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            Settingsbutton.Visible = true;

            Label debloat = new Label();
            debloat.Text = "Debloat Options";
            debloat.AutoSize = true;
            debloat.Width = 25;
            debloat.Height = 10;
            debloat.Location = new System.Drawing.Point(10, 9);
            debloat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold | FontStyle.Underline);
            debloat.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            debloat.Visible = true;

            //Button customizeBlacklists = new Button();
            //customizeBlacklists.Text = "Customize Blacklist";
            //customizeBlacklists.Width = 150;
            //customizeBlacklists.Height = 45;
            //customizeBlacklists.Location = new System.Drawing.Point(10, 32);
            //customizeBlacklists.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            //customizeBlacklists.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            //customizeBlacklists.Visible = true;

            Button removeBloatware = new Button();
            removeBloatware.Text = "Remove Bloatware";
            removeBloatware.Width = 150;
            removeBloatware.Height = 45;
            removeBloatware.Location = new System.Drawing.Point(10, 32);
            removeBloatware.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            removeBloatware.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            removeBloatware.Visible = true;

            Button removeAdvancedBloatware = new Button();
            removeAdvancedBloatware.Text = "Remove Advanced Bloatware";
            removeAdvancedBloatware.Width = 150;
            removeAdvancedBloatware.Height = 45;
            removeAdvancedBloatware.Location = new System.Drawing.Point(10, 80);
            removeAdvancedBloatware.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            removeAdvancedBloatware.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            removeAdvancedBloatware.Visible = true;

            Button removeAllBloatware = new Button();
            removeAllBloatware.Text = "Debloat And Optional Fixes";
            removeAllBloatware.Width = 150;
            removeAllBloatware.Height = 45;
            removeAllBloatware.Location = new System.Drawing.Point(10, 130);
            removeAllBloatware.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            removeAllBloatware.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            removeAllBloatware.Visible = true;

            // Optional Changes/Fixes
            Label label3 = new Label();
            label3.Text = "Optional Changes/Fixes";
            label3.AutoSize = true;
            label3.Width = 25;
            label3.Height = 10;
            label3.Location = new System.Drawing.Point(445, 9);
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold | FontStyle.Underline);
            label3.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            label3.Visible = true;

            Button disableTaskbarBloat = new Button();
            disableTaskbarBloat.Text = "Remove Taskbar Bloat";
            disableTaskbarBloat.Width = 140;
            disableTaskbarBloat.Height = 45;
            disableTaskbarBloat.Location = new System.Drawing.Point(410, 32);
            disableTaskbarBloat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            disableTaskbarBloat.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            disableTaskbarBloat.Visible = true;

            Button stopEdgePDFTakeover = new Button();
            stopEdgePDFTakeover.Text = "Stop Edge PDF Takeover";
            stopEdgePDFTakeover.Width = 140;
            stopEdgePDFTakeover.Height = 45;
            stopEdgePDFTakeover.Location = new System.Drawing.Point(555, 32);
            stopEdgePDFTakeover.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            stopEdgePDFTakeover.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            stopEdgePDFTakeover.Visible = true;

            Button disableTelemetry = new Button();
            disableTelemetry.Text = "Disable Telemetry/Tasks";
            disableTelemetry.Width = 140;
            disableTelemetry.Height = 45;
            disableTelemetry.Location = new System.Drawing.Point(410, 80);
            disableTelemetry.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            disableTelemetry.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            disableTelemetry.Visible = true;

            //Button removeRegkeys = new Button();
            //removeRegkeys.Text = "Remove Bloatware Regkeys";
            //removeRegkeys.Width = 140;
            //removeRegkeys.Height = 45;
            //removeRegkeys.Location = new System.Drawing.Point(555, 80);
            //removeRegkeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            //removeRegkeys.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            //removeRegkeys.Visible = false;

            Button longSleep = new Button();
            longSleep.Text = "Longer Sleep";
            longSleep.Width = 140;
            longSleep.Height = 45;
            longSleep.Location = new System.Drawing.Point(410, 130);
            longSleep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            longSleep.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            longSleep.Visible = true;

            Button removeStartBloat = new Button();
            removeStartBloat.Text = "Remove Start Bloat";
            removeStartBloat.Width = 140;
            removeStartBloat.Height = 45;
            removeStartBloat.Location = new System.Drawing.Point(555, 130);
            removeStartBloat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            removeStartBloat.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            removeStartBloat.Visible = true;

            // Install Programs
            Label label4 = new Label();
            label4.Text = "Install Programs";
            label4.AutoSize = true;
            label4.Width = 25;
            label4.Height = 10;
            label4.Location = new System.Drawing.Point(80, 290);
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold | FontStyle.Underline);
            label4.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            label4.Visible = true;

            Button basicSS = new Button();
            basicSS.Text = "Install Basic System Setup";
            basicSS.Width = 140;
            basicSS.Height = 45;
            basicSS.Location = new System.Drawing.Point(10, 315);
            basicSS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            basicSS.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee"); ;
            basicSS.Visible = true;

            Button isFile = new Button();
            isFile.Text = "Install From Installed Software File";
            isFile.Width = 140;
            isFile.Height = 45;
            isFile.Location = new System.Drawing.Point(10, 365);
            isFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            isFile.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            isFile.Visible = true;

            Button itdb = new Button();
            itdb.Text = "Install Todo Backup";
            itdb.Width = 140;
            itdb.Height = 45;
            itdb.Location = new System.Drawing.Point(160, 365);
            itdb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            itdb.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            itdb.Visible = true;

            Button installGames = new Button();
            installGames.Text = "Install Game Launchers";
            installGames.Width = 140;
            installGames.Height = 45;
            installGames.Location = new System.Drawing.Point(160, 315);
            installGames.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            installGames.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            installGames.Visible = true;

            //Windows Fixes
            Label label2 = new Label();
            label2.Text = "Windows Repair";
            label2.AutoSize = true;
            label2.Width = 25;
            label2.Height = 10;
            label2.Location = new System.Drawing.Point(308, 290);
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold | FontStyle.Underline);
            label2.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            label2.Visible = true;

            Button WindowsRepair = new Button();
            WindowsRepair.Text = "Windows Corruption Repair";
            WindowsRepair.Width = 140;
            WindowsRepair.Height = 45;
            WindowsRepair.Location = new System.Drawing.Point(308, 315);
            WindowsRepair.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            WindowsRepair.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            WindowsRepair.Visible = true;

            Button CheckRemote = new Button();
            CheckRemote.Text = "Check for Remote Desktop";
            CheckRemote.Width = 140;
            CheckRemote.Height = 45;
            CheckRemote.Location = new System.Drawing.Point(308, 365);
            CheckRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            CheckRemote.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            CheckRemote.Visible = true;

            // Windows Customization
            Label Customization = new Label();
            Customization.Text = "Windows Customization";
            Customization.AutoSize = true;
            Customization.Width = 457;
            Customization.Height = 142;
            Customization.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            Customization.Location = new System.Drawing.Point(490, 290);
            Customization.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold | FontStyle.Underline);
            Customization.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            Customization.Visible = true;

            Button enableDarkMode = new Button();
            enableDarkMode.Text = "Dark Mode";
            enableDarkMode.Width = 115;
            enableDarkMode.Height = 45;
            enableDarkMode.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            enableDarkMode.Location = new System.Drawing.Point(580, 315);
            enableDarkMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            enableDarkMode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            enableDarkMode.Visible = true;

            Button disableDarkMode = new Button();
            disableDarkMode.Text = "Light Mode";
            disableDarkMode.Width = 115;
            disableDarkMode.Height = 45;
            disableDarkMode.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            disableDarkMode.Location = new System.Drawing.Point(580, 365);
            disableDarkMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            disableDarkMode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            disableDarkMode.Visible = true;

            Button Taskbartoleft = new Button();
            Taskbartoleft.Text = "Taskbar Left";
            Taskbartoleft.Width = 118;
            Taskbartoleft.Height = 45;
            Taskbartoleft.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            Taskbartoleft.Location = new System.Drawing.Point(455, 365);
            Taskbartoleft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            Taskbartoleft.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            Taskbartoleft.Visible = true;

            Button Taskbartocenter = new Button();
            Taskbartocenter.Text = "Taskbar Center";
            Taskbartocenter.Width = 118;
            Taskbartocenter.Height = 45;
            Taskbartocenter.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            Taskbartocenter.Location = new System.Drawing.Point(455, 315);
            Taskbartocenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            Taskbartocenter.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            Taskbartocenter.Visible = true;

            // Create and configure the TextBox
            textBox1 = new TextBox();
            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.ReadOnly = true;
            textBox1.Width = 680;
            textBox1.Height = 150;
            textBox1.Location = new System.Drawing.Point(10, 420);
            textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            textBox1.BackColor = System.Drawing.Color.FromArgb(37, 37, 37);
            textBox1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

            this.Controls.AddRange(new Control[] {
            textBox1,
            debloat,
            pleaseWaitLabel,
            Settingsbutton,
            //customizeBlacklists,
            removeBloatware,
            removeAdvancedBloatware,
            removeAllBloatware,
            label2,
            WindowsRepair,
            CheckRemote,
            label3,
            disableTaskbarBloat,
            stopEdgePDFTakeover,
            disableTelemetry,
            //removeRegkeys,
            longSleep,
            removeStartBloat,
            label4,
            basicSS,
            isFile,
            itdb,
            installGames,
            Customization,
            enableDarkMode,
            disableDarkMode,
            Taskbartoleft,
            Taskbartocenter,
            // Add other controls here
            });

            //settings code
            // Load settings on form initialization
            appSettings = LoadSettings();

            void ApplySettings()
            {
                // Apply settings to UI controls
                debloat.Visible = !appSettings.ShowRemoveDebloatButtons;
                removeBloatware.Visible = !appSettings.ShowRemoveDebloatButtons;
                if (!appSettings.ShowRemoveDebloatButtons == false)
                {
                    removeAdvancedBloatware.Visible = false;
                }
                else
                {
                    removeAdvancedBloatware.Visible = !appSettings.ShowRemoveRABloatButton;
                }
                removeAllBloatware.Visible = !appSettings.ShowRemoveDebloatButtons;

                label3.Visible = !appSettings.ShowRemoveOptionalButtons;
                disableTaskbarBloat.Visible = !appSettings.ShowRemoveOptionalButtons;
                stopEdgePDFTakeover.Visible = !appSettings.ShowRemoveOptionalButtons;
                disableTelemetry.Visible = !appSettings.ShowRemoveOptionalButtons;
                //removeRegkeys.Visible = !appSettings.ShowRemoveOptionalButtons;
                longSleep.Visible = !appSettings.ShowRemoveOptionalButtons;
                removeStartBloat.Visible = !appSettings.ShowRemoveOptionalButtons;

                enableDarkMode.Visible = !appSettings.ShowRemoveCustomButtons;
                disableDarkMode.Visible = !appSettings.ShowRemoveCustomButtons;
                Taskbartocenter.Visible = !appSettings.ShowRemoveCustomButtons;
                Taskbartoleft.Visible = !appSettings.ShowRemoveCustomButtons;
                Customization.Visible = !appSettings.ShowRemoveCustomButtons;

                label4.Visible = !appSettings.ShowRemoveProgramButtons;
                basicSS.Visible = !appSettings.ShowRemoveProgramButtons;
                isFile.Visible = !appSettings.ShowRemoveProgramButtons;
                itdb.Visible = !appSettings.ShowRemoveProgramButtons;
                installGames.Visible = !appSettings.ShowRemoveProgramButtons;

                label2.Visible = !appSettings.ShowRemoveRepairButtons;
                WindowsRepair.Visible = !appSettings.ShowRemoveRepairButtons;
                CheckRemote.Visible = !appSettings.ShowRemoveRepairButtons;

                if (appSettings.ShrinkTextBox)
                {
                    // Adjust the height of the textbox when ChangeSize is true
                    textBox1.Height = 50;
                    textBox1.Location = new System.Drawing.Point(10, 520);
                }
                else
                {
                    textBox1.Height = 150;
                    textBox1.Location = new System.Drawing.Point(10, 420);
                }
            }

            void SaveSettings()
            {
                // Save settings to file
                SaveSettingsToFile(appSettings);
            }

            AppSettings LoadSettings()
            {
                // Load settings from file
                return LoadSettingsFromFile();
            }

            void SaveSettingsToFile(AppSettings settings)
            {
                try
                {
                    // Combine the app folder with the settings file name
                    string filePath = "settings.json";

                    // Check if the file exists, and create it if it doesn't
                    if (!System.IO.File.Exists(filePath))
                    {
                        using (FileStream createFile = System.IO.File.Create(filePath))
                        {
                            // No need to write anything; the file is created
                        }
                    }

                    // Serialize and save the settings to the file
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                            jsonSerializer.Serialize(sw, settings);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving settings: " + ex.Message);
                }
            }

            AppSettings LoadSettingsFromFile()
            {
                try
                {
                    string settingsFilePath = "settings.json";
                    string CustomSoftwareFilePath = "CustomSoftware.json";
                    // Check if the file exists, and create it if it doesn't
                    if (!System.IO.File.Exists(settingsFilePath))
                    {
                        CopyEmbeddedResource("Debloat_And_Float.Resources.settings.json", "settings.json");
                    }

                    if (!System.IO.File.Exists(CustomSoftwareFilePath))
                    {
                        CopyEmbeddedResource("Debloat_And_Float.Resources.CustomSoftware.json", "CustomSoftware.json");
                    }

                    // Read the settings from the file
                    using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                            return (AppSettings)jsonSerializer.Deserialize(sr, typeof(AppSettings));
                        }
                    }
                }
                catch (JsonReaderException)
                {
                    // Handle JsonReaderException, for example, by showing an error message
                    MessageBox.Show("Error loading settings. The settings file is corrupted or empty.");

                    // Return default settings or take appropriate action
                    return new AppSettings();
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    MessageBox.Show("Error loading settings: " + ex.Message);

                    // Return default settings or take appropriate action
                    return new AppSettings();
                }
            }

            string BuildNumber = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "").ToString();

            int DetermineResult(string buildNumberString)
            {
                int buildNumber;
                if (int.TryParse(buildNumberString, out buildNumber))
                {
                    if (buildNumber > 22000)
                    {
                        return 11;
                    }
                    else
                    {
                        return 10;
                    }
                }
                else
                {
                    // Handle invalid buildNumberString
                    return -1; // Or any other appropriate error code
                }
            }
            int OSVersion = DetermineResult(BuildNumber);

            //Button Logic
            {
                // Settings Button Click Event
                Settingsbutton.Click += (sender, e) =>
                {
                    string settingsFilePath = "settings.json";

                    // Check if the settings file exists
                    if (System.IO.File.Exists(settingsFilePath))
                    {
                        // Load existing settings from the file
                        //AppSettings appSettings = LoadSettings(settingsFilePath);

                        // If the file exists, open the SettingsForm with loaded settings
                        Form SettingsForm = new Form();
                        {
                            // Form Settings
                            SettingsForm.ClientSize = new Size(600, 400);
                            SettingsForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                            SettingsForm.MinimizeBox = false;
                            SettingsForm.MaximizeBox = false;
                            SettingsForm.ShowIcon = false;
                            SettingsForm.Text = "Settings";
                            SettingsForm.TopMost = false;
                            SettingsForm.AutoScroll = true;
                            SettingsForm.BackColor = System.Drawing.ColorTranslator.FromHtml("#252525");

                            Button CustomizeBloatware = new Button();
                            CustomizeBloatware.Text = "Customize Bloatware";
                            CustomizeBloatware.Width = 150;
                            CustomizeBloatware.Height = 45;
                            CustomizeBloatware.Location = new System.Drawing.Point(290, 10);
                            CustomizeBloatware.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            CustomizeBloatware.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            CustomizeBloatware.Visible = true;
                            SettingsForm.Controls.Add(CustomizeBloatware);

                            Button CustomizeAdvancedBloatware = new Button();
                            CustomizeAdvancedBloatware.Text = "Customize Advanced Bloatware";
                            CustomizeAdvancedBloatware.Width = 150;
                            CustomizeAdvancedBloatware.Height = 45;
                            CustomizeAdvancedBloatware.Location = new System.Drawing.Point(445, 10);
                            CustomizeAdvancedBloatware.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            CustomizeAdvancedBloatware.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            SettingsForm.Controls.Add(CustomizeAdvancedBloatware);

                            Button CustomizeBSS = new Button();
                            CustomizeBSS.Text = "Customize Basic System Setup";
                            CustomizeBSS.Width = 150;
                            CustomizeBSS.Height = 45;
                            CustomizeBSS.Location = new System.Drawing.Point(290, 60);
                            CustomizeBSS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            CustomizeBSS.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            SettingsForm.Controls.Add(CustomizeBSS);

                            // Create a checkbox for settings
                            CheckBox RABloatCheckBox = new CheckBox();
                            RABloatCheckBox.Text = "Toggle Advanced Bloatware Button";
                            RABloatCheckBox.Width = 300;
                            RABloatCheckBox.Location = new System.Drawing.Point(10, 10);
                            RABloatCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RABloatCheckBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RABloatCheckBox.Checked = appSettings.ShowRemoveRABloatButton; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RABloatCheckBox);

                            // Create a checkbox for settings
                            CheckBox RemoveCustomWin = new CheckBox();
                            RemoveCustomWin.Text = "Toggle Customize Windows Buttons";
                            RemoveCustomWin.Width = 300;
                            RemoveCustomWin.Location = new System.Drawing.Point(10, 40);
                            RemoveCustomWin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RemoveCustomWin.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RemoveCustomWin.Checked = appSettings.ShowRemoveCustomButtons; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RemoveCustomWin);

                            // Create a checkbox for settings
                            CheckBox ShrinkTextBox = new CheckBox();
                            ShrinkTextBox.Text = "Shrink TextBox";
                            ShrinkTextBox.Width = 300;
                            ShrinkTextBox.Location = new System.Drawing.Point(10, 70);
                            ShrinkTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            ShrinkTextBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            ShrinkTextBox.Checked = appSettings.ShrinkTextBox; // Set initial state based on the settings
                            SettingsForm.Controls.Add(ShrinkTextBox);

                            // Create a checkbox for settings
                            CheckBox RemovePrograms = new CheckBox();
                            RemovePrograms.Text = "Remove Programs Buttons";
                            RemovePrograms.Width = 300;
                            RemovePrograms.Location = new System.Drawing.Point(10, 100);
                            RemovePrograms.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RemovePrograms.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RemovePrograms.Checked = appSettings.ShowRemoveProgramButtons; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RemovePrograms);

                            // Create a checkbox for settings
                            CheckBox RemoveDebloat = new CheckBox();
                            RemoveDebloat.Text = "Remove Debloat Buttons";
                            RemoveDebloat.Width = 300;
                            RemoveDebloat.Location = new System.Drawing.Point(10, 130);
                            RemoveDebloat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RemoveDebloat.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RemoveDebloat.Checked = appSettings.ShowRemoveDebloatButtons; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RemoveDebloat);

                            // Create a checkbox for settings
                            CheckBox RemoveRepair = new CheckBox();
                            RemoveRepair.Text = "Remove Repair Buttons";
                            RemoveRepair.Width = 300;
                            RemoveRepair.Location = new System.Drawing.Point(10, 160);
                            RemoveRepair.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RemoveRepair.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RemoveRepair.Checked = appSettings.ShowRemoveRepairButtons; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RemoveRepair);

                            // Create a checkbox for settings
                            CheckBox RemoveOptional = new CheckBox();
                            RemoveOptional.Text = "Remove Optional Buttons";
                            RemoveOptional.Width = 300;
                            RemoveOptional.Location = new System.Drawing.Point(10, 190);
                            RemoveOptional.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            RemoveOptional.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            RemoveOptional.Checked = appSettings.ShowRemoveOptionalButtons; // Set initial state based on the settings
                            SettingsForm.Controls.Add(RemoveOptional);

                            // Create a Label for the NumericUpDown control
                            Label SleepHelp = new Label();
                            SleepHelp.Text = "Sleep settings are in minutes.\r\nDefault Windows Values: 5, 10, 15, 20, 25, 30, 45\r\n60 = 1 Hour, 120 = 2 Hours, 180 = 3 Hours,\r\n240 = 4 hours, 300 = 5 Hours, 0 = Never"; // Add the desired text
                            SleepHelp.AutoSize = true; // Adjust size automatically
                            SleepHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            SleepHelp.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            SleepHelp.Location = new System.Drawing.Point(255, 245);
                            SettingsForm.Controls.Add(SleepHelp);

                            // Create a Label for the NumericUpDown control
                            Label sleepsettingACLabel = new Label();
                            sleepsettingACLabel.Text = "Sleep Setting (AC):"; // Add the desired text
                            sleepsettingACLabel.AutoSize = true; // Adjust size automatically
                            sleepsettingACLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            sleepsettingACLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            sleepsettingACLabel.Location = new System.Drawing.Point(10, 220);
                            SettingsForm.Controls.Add(sleepsettingACLabel);

                            // Create a NumericUpDown control for settings
                            NumericUpDown sleepsettingAC = new NumericUpDown();
                            sleepsettingAC.Minimum = 0;
                            sleepsettingAC.Maximum = 300;
                            sleepsettingAC.Value = appSettings.sleepsettingAC; // Set initial value based on the settings
                            sleepsettingAC.Location = new System.Drawing.Point(150, 220);
                            sleepsettingAC.Width = 100;
                            SettingsForm.Controls.Add(sleepsettingAC);

                            // Create a Label for the NumericUpDown control
                            Label monitorsettingACLabel = new Label();
                            monitorsettingACLabel.Text = "Monitor Setting (AC):"; // Add the desired text
                            monitorsettingACLabel.AutoSize = true; // Adjust size automatically
                            monitorsettingACLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            monitorsettingACLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            monitorsettingACLabel.Location = new System.Drawing.Point(10, 250);
                            SettingsForm.Controls.Add(monitorsettingACLabel);

                            // Create a NumericUpDown control for settings
                            NumericUpDown monitorsettingAC = new NumericUpDown();
                            monitorsettingAC.Minimum = 0;
                            monitorsettingAC.Maximum = 300;
                            monitorsettingAC.Value = appSettings.monitorsettingAC; // Set initial value based on the settings
                            monitorsettingAC.Location = new System.Drawing.Point(150, 250);
                            monitorsettingAC.Width = 100;
                            SettingsForm.Controls.Add(monitorsettingAC);

                            // Create a Label for the NumericUpDown control
                            Label sleepsettingDCLabel = new Label();
                            sleepsettingDCLabel.Text = "Sleep Setting (DC):"; // Add the desired text
                            sleepsettingDCLabel.AutoSize = true; // Adjust size automatically
                            sleepsettingDCLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            sleepsettingDCLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            sleepsettingDCLabel.Location = new System.Drawing.Point(10, 280);
                            SettingsForm.Controls.Add(sleepsettingDCLabel);

                            // Create a NumericUpDown control for settings
                            NumericUpDown sleepsettingDC = new NumericUpDown();
                            sleepsettingDC.Minimum = 0;
                            sleepsettingDC.Maximum = 300;
                            sleepsettingDC.Value = appSettings.sleepsettingDC; // Set initial value based on the settings
                            sleepsettingDC.Location = new System.Drawing.Point(150, 280);
                            sleepsettingDC.Width = 100;
                            SettingsForm.Controls.Add(sleepsettingDC);

                            Label monitorsettingDCLabel = new Label();
                            monitorsettingDCLabel.Text = "Monitor Setting (DC):"; // Add the desired text
                            monitorsettingDCLabel.AutoSize = true; // Adjust size automatically
                            monitorsettingDCLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                            monitorsettingDCLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                            monitorsettingDCLabel.Location = new System.Drawing.Point(10, 310);
                            SettingsForm.Controls.Add(monitorsettingDCLabel);

                            // Create a NumericUpDown control for settings
                            NumericUpDown monitorsettingDC = new NumericUpDown();
                            monitorsettingDC.Minimum = 0;
                            monitorsettingDC.Maximum = 300;
                            monitorsettingDC.Value = appSettings.monitorsettingDC; // Set initial value based on the settings
                            monitorsettingDC.Location = new System.Drawing.Point(150, 310);
                            monitorsettingDC.Width = 100;
                            SettingsForm.Controls.Add(monitorsettingDC);

                            RABloatCheckBox.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            RemoveCustomWin.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            ShrinkTextBox.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            RemovePrograms.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            RemoveDebloat.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            RemoveRepair.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            RemoveOptional.CheckedChanged += ToggleCheckBox_CheckedChanged;
                            sleepsettingAC.ValueChanged += SaveValue_ValueChanged;
                            sleepsettingDC.ValueChanged += SaveValue_ValueChanged;
                            monitorsettingDC.ValueChanged += SaveValue_ValueChanged;
                            monitorsettingAC.ValueChanged += SaveValue_ValueChanged;

                            // Handle value changed event for the NumericUpDown control
                            void SaveValue_ValueChanged(object sender, EventArgs e)
                            {
                                NumericUpDown numericUpDown = (NumericUpDown)sender;

                                // Update the setting based on the numericUpDown value
                                if (numericUpDown == sleepsettingAC)
                                    appSettings.sleepsettingAC = (int)numericUpDown.Value;
                                else if (numericUpDown == sleepsettingDC)
                                    appSettings.sleepsettingDC = (int)numericUpDown.Value;
                                else if (numericUpDown == monitorsettingAC)
                                    appSettings.monitorsettingAC = (int)numericUpDown.Value;
                                else if (numericUpDown == monitorsettingDC)
                                    appSettings.monitorsettingDC = (int)numericUpDown.Value;

                                // Apply the updated settings to the UI controls
                                ApplySettings();

                                // Save the updated settings
                                SaveSettings();
                            }

                            void ToggleCheckBox_CheckedChanged(object sender, EventArgs e)
                            {
                                CheckBox checkBox = (CheckBox)sender;

                                // Update the setting based on the checkbox state
                                if (checkBox == RABloatCheckBox)
                                    appSettings.ShowRemoveRABloatButton = checkBox.Checked;
                                else if (checkBox == RemoveCustomWin)
                                    appSettings.ShowRemoveCustomButtons = checkBox.Checked;
                                else if (checkBox == ShrinkTextBox)
                                    appSettings.ShrinkTextBox = checkBox.Checked;
                                else if (checkBox == RemovePrograms)
                                    appSettings.ShowRemoveProgramButtons = checkBox.Checked;
                                else if (checkBox == RemoveDebloat)
                                    appSettings.ShowRemoveDebloatButtons = checkBox.Checked;
                                else if (checkBox == RemoveOptional)
                                    appSettings.ShowRemoveOptionalButtons = checkBox.Checked;
                                else if (checkBox == RemoveRepair)
                                    appSettings.ShowRemoveRepairButtons = checkBox.Checked;
                                // Add more conditions for other checkboxes

                                // Apply the updated settings to the UI controls
                                ApplySettings();

                                // Save the updated settings
                                SaveSettings();
                            }

                            // Show the SettingsForm
                            SettingsForm.Show();

                            CustomizeBloatware.Click += (sender, e) =>
                            {
                                try
                                {
                                    string jsonFilePath = "CustomSoftware.json"; // Adjust the path as needed

                                    // Read the JSON file
                                    string jsonData = System.IO.File.ReadAllText(jsonFilePath);

                                    // Deserialize JSON data
                                    dynamic customSoftware = JsonConvert.DeserializeObject(jsonData);

                                    // Extract BloatWare list
                                    List<string> bloatWare = customSoftware["BloatWare"].ToObject<List<string>>();
                                    List<string> whiteListedApps = customSoftware["WhiteListedApps"].ToObject<List<string>>();
                                    List<string> nonRemovable = customSoftware["nonRemovable"].ToObject<List<string>>();
                                    List<string> disabledNonRemovable = customSoftware["disabledNonRemovable"].ToObject<List<string>>();
                                    List<string> disabledWhiteListed = customSoftware["disabledWhiteListed"].ToObject<List<string>>();

                                    // Extract CheckboxStates or initialize it if it doesn't exist
                                    Dictionary<string, bool> checkboxStates = customSoftware.ContainsKey("BloatwareCheckboxStates") ?
                                    customSoftware["BloatwareCheckboxStates"].ToObject<Dictionary<string, bool>>() :
                                    new Dictionary<string, bool>();

                                    // Open a form for editing BloatWare list
                                    using (var bloatwareForm = new Form())
                                    {
                                        // Set form properties
                                        bloatwareForm.ClientSize = new Size(700, 400);
                                        bloatwareForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                                        bloatwareForm.MinimizeBox = false;
                                        bloatwareForm.MaximizeBox = false;
                                        bloatwareForm.ShowIcon = false;
                                        bloatwareForm.Text = "Customize Bloatware";
                                        bloatwareForm.TopMost = false;
                                        bloatwareForm.AutoScroll = true;
                                        bloatwareForm.BackColor = System.Drawing.ColorTranslator.FromHtml("#252525");

                                        // Add Save button
                                        Button saveButton = new Button();
                                        saveButton.Text = "Save";
                                        saveButton.Width = 70;
                                        saveButton.Height = 30;
                                        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                                        saveButton.Location = new Point(350, 350);
                                        saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        saveButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(saveButton);

                                        // Add Add button
                                        Button addButton = new Button();
                                        addButton.Text = "Add";
                                        addButton.Width = 70;
                                        addButton.Height = 30;
                                        addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        addButton.Location = new Point((bloatwareForm.ClientSize.Width - saveButton.Width - addButton.Width - 20) / 2, bloatwareForm.ClientSize.Height - addButton.Height - 20);
                                        addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        addButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(addButton);

                                        Button WhitelistButton = new Button();
                                        WhitelistButton.Text = "Edit Whitelist";
                                        WhitelistButton.Width = 150;
                                        WhitelistButton.Height = 30;
                                        WhitelistButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        WhitelistButton.Location = new Point(100, 350);
                                        WhitelistButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        WhitelistButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(WhitelistButton);

                                        Button NonRemovableButton = new Button();
                                        NonRemovableButton.Text = "Edit Non-Removable";
                                        NonRemovableButton.Width = 150;
                                        NonRemovableButton.Height = 30;
                                        NonRemovableButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        NonRemovableButton.Location = new Point(450, 350);
                                        NonRemovableButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        NonRemovableButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(NonRemovableButton);

                                        // Add text box
                                        TextBox textBox = new TextBox();
                                        textBox.Width = 200;
                                        textBox.Height = 30;
                                        textBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        textBox.Location = new System.Drawing.Point(200, 200);
                                        textBox.Visible = false; // Initially hidden
                                        textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        textBox.ForeColor = System.Drawing.Color.Black;
                                        bloatwareForm.Controls.Add(textBox);

                                        // Add submit button
                                        Button submitButton = new Button();
                                        submitButton.Text = "Submit";
                                        submitButton.Width = 80;
                                        submitButton.Height = 30;
                                        submitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        submitButton.Location = new System.Drawing.Point(400, 195);
                                        submitButton.Visible = false; // Initially hidden
                                        submitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        submitButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(submitButton);

                                        // Create a Panel for scrolling
                                        Panel scrollPanel = new Panel();
                                        scrollPanel.Dock = DockStyle.Fill;
                                        scrollPanel.AutoScroll = true;

                                        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                                        tableLayoutPanel.AutoSize = true; // Consider setting AutoSizeMode.GrowAndShrink for proper resizing
                                        tableLayoutPanel.ColumnCount = 4;
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Adjust this line to ensure the last column fills the remaining space

                                        int row = 0;
                                        foreach (string software in bloatWare)
                                        {
                                            // Create label for software
                                            Label label = new Label();
                                            label.Text = software;
                                            label.AutoSize = true;
                                            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                            // Create label for whitelisted status
                                            Label whiteListedLabel = new Label();
                                            whiteListedLabel.Text = whiteListedApps.Contains(software) ? "Whitelisted" : "";
                                            whiteListedLabel.AutoSize = true;
                                            whiteListedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            whiteListedLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                            // Create label for non-removable status
                                            Label nonRemovableLabel = new Label();
                                            nonRemovableLabel.Text = nonRemovable.Contains(software) ? "Non-Removable" : "";
                                            nonRemovableLabel.AutoSize = true;
                                            nonRemovableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            nonRemovableLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                            // Create checkbox
                                            CheckBox checkBox = new CheckBox();
                                            checkBox.Text = "Include";
                                            checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                            checkBox.Checked = checkboxStates.ContainsKey(software) ? checkboxStates[software] : true; // Set checkbox state based on saved state or default to true if not found
                                            checkBox.Anchor = AnchorStyles.Right;

                                            // Assign software name to Tag property of checkbox
                                            checkBox.Tag = software;

                                            // Check if the software is non-removable and disable the checkbox if so
                                            if (nonRemovable.Contains(software))
                                            {
                                                checkBox.Enabled = false;
                                            }

                                            // Add controls to the table
                                            tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column
                                            tableLayoutPanel.Controls.Add(whiteListedLabel, 1, row); // Add whitelisted label to the second column
                                            tableLayoutPanel.Controls.Add(nonRemovableLabel, 2, row); // Add non-removable label to the third column
                                            tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column

                                            row++;

                                        }

                                        Label emptySpace = new Label();
                                        emptySpace.Height = 50; // Adjust the height as needed for your layout

                                        // Add empty space to the last row to create padding at the bottom
                                        tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                        tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                        saveButton.Click += (s, ev) =>
                                        {
                                            try
                                            {
                                                // Iterate over each control in the tableLayoutPanel
                                                foreach (Control control in tableLayoutPanel.Controls)
                                                {
                                                    if (control is CheckBox chkBox)
                                                    {
                                                        // Retrieve the software name from the Tag property of the checkbox
                                                        string softwareName = chkBox.Tag.ToString();

                                                        // Check if the software is non-removable or whitelisted
                                                        bool isNonRemovable = nonRemovable.Contains(softwareName);
                                                        bool isWhiteListed = whiteListedApps.Contains(softwareName);
                                                        bool isDisabledWhiteListed = disabledWhiteListed.Contains(softwareName);
                                                        bool isDisabledNonRemovable = disabledNonRemovable.Contains(softwareName);

                                                        // Update the enabled/disabled lists based on checkbox state
                                                        if (isWhiteListed)
                                                        {
                                                            if (!chkBox.Checked)
                                                            {
                                                                disabledWhiteListed.Add(softwareName); // Add to disabled list
                                                                whiteListedApps.Remove(softwareName); // Remove from enabled list
                                                            }
                                                        }
                                                        else if (isNonRemovable)
                                                        {
                                                            if (!chkBox.Checked)
                                                            {
                                                                disabledNonRemovable.Add(softwareName); // Add to disabled list
                                                                nonRemovable.Remove(softwareName); // Remove from enabled list
                                                            }
                                                        }
                                                        else if (isDisabledWhiteListed)
                                                        {
                                                            if (chkBox.Checked)
                                                            {
                                                                disabledWhiteListed.Remove(softwareName); // Remove from disabled list
                                                                whiteListedApps.Add(softwareName); // Add back to original list if not already enabled
                                                            }
                                                        }
                                                        else if (isDisabledNonRemovable)
                                                        {
                                                            if (chkBox.Checked)
                                                            {
                                                                disabledNonRemovable.Remove(softwareName); // Remove from disabled list
                                                                nonRemovable.Add(softwareName); // Add back to original list if not already enabled
                                                            }
                                                        }
                                                        else // Bloatware
                                                        {
                                                            // Update the checkbox state in the checkboxStates dictionary
                                                            checkboxStates[softwareName] = chkBox.Checked;
                                                        }
                                                    }
                                                }

                                                // Update JSON data
                                                customSoftware["WhiteListedApps"] = JArray.FromObject(whiteListedApps); // Update whitelisted apps
                                                customSoftware["nonRemovable"] = JArray.FromObject(nonRemovable); // Update non-removable apps
                                                customSoftware["disabledWhiteListed"] = JArray.FromObject(disabledWhiteListed); // Update disabled whitelisted apps
                                                customSoftware["disabledNonRemovable"] = JArray.FromObject(disabledNonRemovable); // Update disabled non-removable apps
                                                customSoftware["BloatwareCheckboxStates"] = JObject.FromObject(checkboxStates); // Update bloatware checkbox states

                                                // Serialize the updated JSON data
                                                string updatedJsonData = JsonConvert.SerializeObject(customSoftware, Formatting.Indented);

                                                // Write updated JSON data to file
                                                System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);

                                                // Confirmation message
                                                MessageBox.Show("BloatWare list and checkbox states updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            catch (Exception ex)
                                            {
                                                // Handle exceptions
                                                MessageBox.Show($"An error occurred while saving checkbox states: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        // Handle add button click event
                                        addButton.Click += (s, ev) =>
                                        {
                                            // Show text box and submit button
                                            textBox.Visible = true;
                                            submitButton.Visible = true;
                                        };

                                        // Handle submit button click event
                                        submitButton.Click += (s, ev) =>
                                        {
                                            // Get the text from the text box
                                            string newBloatware = textBox.Text.Trim();

                                            // Check if the entered bloatware is already whitelisted or non-removable
                                            if (whiteListedApps.Contains(newBloatware))
                                            {
                                                MessageBox.Show("The entered software is already whitelisted and cannot be added as bloatware.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                return;
                                            }
                                            else if (nonRemovable.Contains(newBloatware))
                                            {
                                                MessageBox.Show("The entered software is marked as non-removable and cannot be added as bloatware.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                return;
                                            }

                                            // Add the new bloatware to the list
                                            if (!string.IsNullOrEmpty(newBloatware))
                                            {
                                                // Remove the empty space control from the table
                                                tableLayoutPanel.Controls.Remove(emptySpace);

                                                // Add the new software to the table
                                                bloatWare.Add(newBloatware); // Add to the list of bloatware

                                                // Update the customSoftware object with the new bloatware
                                                customSoftware["BloatWare"] = JArray.FromObject(bloatWare);

                                                // Create label for software
                                                Label label = new Label();
                                                label.Text = newBloatware;
                                                label.AutoSize = true;
                                                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create checkbox
                                                CheckBox checkBox = new CheckBox();
                                                checkBox.Name = "chk" + Guid.NewGuid().ToString("N"); // Generate a unique name for the checkbox
                                                checkBox.Text = "Include";
                                                checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                                checkBox.Checked = true; // Default to checked for new checkboxes
                                                checkBox.Anchor = AnchorStyles.Right; // Anchor checkbox to the right side of the cell

                                                // Assign software name to Tag property of checkbox
                                                checkBox.Tag = newBloatware;

                                                // Update the checkboxStates dictionary
                                                checkboxStates[newBloatware] = true;

                                                // Add controls to the table
                                                tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column of the current row
                                                tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column of the current row

                                                // Increment row count
                                                tableLayoutPanel.RowCount++;

                                                // Increment row index
                                                row++;

                                                // Clear the text box
                                                textBox.Text = "";

                                                // Re-add the empty space control to the table
                                                tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                                tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                                // Show or hide the text box and submit button as needed
                                                textBox.Visible = false;
                                                submitButton.Visible = false;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Please enter the name of the new bloatware.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        WhitelistButton.Click += (s, ev) =>
                                        {
                                            // Remove the empty space control from the table
                                            tableLayoutPanel.Controls.Remove(emptySpace);

                                            // Combine whitelisted and disabled whitelisted apps
                                            List<string> combinedWhiteListedApps = whiteListedApps.Concat(disabledWhiteListed).ToList();

                                            foreach (string software in combinedWhiteListedApps)
                                            {
                                                // Create label for software
                                                Label label = new Label();
                                                label.Text = software;
                                                label.AutoSize = true;
                                                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create label for whitelisted status
                                                Label whiteListedLabel = new Label();
                                                whiteListedLabel.Text = whiteListedApps.Contains(software) ? "Whitelisted" : "";
                                                whiteListedLabel.AutoSize = true;
                                                whiteListedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                whiteListedLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create label for non-removable status
                                                Label nonRemovableLabel = new Label();
                                                nonRemovableLabel.Text = nonRemovable.Contains(software) ? "Non-Removable" : "";
                                                nonRemovableLabel.AutoSize = true;
                                                nonRemovableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                nonRemovableLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create checkbox
                                                CheckBox checkBox = new CheckBox();
                                                checkBox.Text = "Include";
                                                checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                                checkBox.Checked = !disabledWhiteListed.Contains(software); // Set checkbox state based on presence in disabled list
                                                checkBox.Anchor = AnchorStyles.Right;

                                                // Assign software name to Tag property of checkbox
                                                checkBox.Tag = software;

                                                // Check if the software is non-removable and disable the checkbox if so
                                                if (nonRemovable.Contains(software))
                                                {
                                                    checkBox.Enabled = false;
                                                }

                                                // Add event handler for checkbox state change
                                                checkBox.CheckedChanged += (sender, e) =>
                                                {
                                                    // Update the enabled/disabled lists based on checkbox state
                                                    string softwareName = (sender as CheckBox).Tag.ToString();
                                                    if (checkBox.Checked && disabledWhiteListed.Contains(softwareName))
                                                    {
                                                        disabledWhiteListed.Remove(softwareName);
                                                        whiteListedApps.Add(softwareName); // Add back to original list if moved to enabled
                                                    }
                                                    else if (!checkBox.Checked && !disabledWhiteListed.Contains(softwareName))
                                                    {
                                                        disabledWhiteListed.Add(softwareName);
                                                        whiteListedApps.Remove(softwareName); // Remove from enabled list if moved to disabled
                                                    }
                                                };

                                                // Add controls to the table
                                                tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column
                                                tableLayoutPanel.Controls.Add(whiteListedLabel, 1, row); // Add whitelisted label to the second column
                                                tableLayoutPanel.Controls.Add(nonRemovableLabel, 2, row); // Add non-removable label to the third column
                                                tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column

                                                row++;
                                            }

                                            // Re-add the empty space control to the table
                                            tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                            tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns
                                        };

                                        NonRemovableButton.Click += (s, ev) =>
                                        {
                                            // Remove the empty space control from the table
                                            tableLayoutPanel.Controls.Remove(emptySpace);

                                            // Combine non-removable and disabled non-removable apps
                                            List<string> combinedNonRemovableApps = nonRemovable.Concat(disabledNonRemovable).ToList();

                                            foreach (string software in combinedNonRemovableApps)
                                            {
                                                // Create label for software
                                                Label label = new Label();
                                                label.Text = software;
                                                label.AutoSize = true;
                                                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create label for whitelisted status
                                                Label whiteListedLabel = new Label();
                                                whiteListedLabel.Text = whiteListedApps.Contains(software) ? "Whitelisted" : "";
                                                whiteListedLabel.AutoSize = true;
                                                whiteListedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                whiteListedLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create label for non-removable status
                                                Label nonRemovableLabel = new Label();
                                                nonRemovableLabel.Text = nonRemovable.Contains(software) ? "Non-Removable" : "";
                                                nonRemovableLabel.AutoSize = true;
                                                nonRemovableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                nonRemovableLabel.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create checkbox
                                                CheckBox checkBox = new CheckBox();
                                                checkBox.Text = "Include";
                                                checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                                checkBox.Checked = !disabledNonRemovable.Contains(software); // Set checkbox state based on presence in disabled list
                                                checkBox.Anchor = AnchorStyles.Right;

                                                // Assign software name to Tag property of checkbox
                                                checkBox.Tag = software;

                                                // Check if the software is whitelisted and disable the checkbox if so
                                                if (whiteListedApps.Contains(software))
                                                {
                                                    checkBox.Enabled = false;
                                                }

                                                // Add event handler for checkbox state change
                                                checkBox.CheckedChanged += (sender, e) =>
                                                {
                                                    // Update the enabled/disabled lists based on checkbox state
                                                    string softwareName = (sender as CheckBox).Tag.ToString();
                                                    if (checkBox.Checked && disabledNonRemovable.Contains(softwareName))
                                                    {
                                                        disabledNonRemovable.Remove(softwareName);
                                                        nonRemovable.Add(softwareName); // Add back to original list if moved to enabled
                                                    }
                                                    else if (!checkBox.Checked && !disabledNonRemovable.Contains(softwareName))
                                                    {
                                                        disabledNonRemovable.Add(softwareName);
                                                        nonRemovable.Remove(softwareName); // Remove from enabled list if moved to disabled
                                                    }
                                                };

                                                // Add controls to the table
                                                tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column
                                                tableLayoutPanel.Controls.Add(whiteListedLabel, 1, row); // Add whitelisted label to the second column
                                                tableLayoutPanel.Controls.Add(nonRemovableLabel, 2, row); // Add non-removable label to the third column
                                                tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column

                                                row++;
                                            }

                                            // Re-add the empty space control to the table
                                            tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                            tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns
                                        };

                                        // Add tableLayoutPanel to the scrollPanel
                                        scrollPanel.Controls.Add(tableLayoutPanel);

                                        // Add scrollPanel to the form
                                        bloatwareForm.Controls.Add(scrollPanel);

                                        // Show the form
                                        bloatwareForm.ShowDialog();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Handle exceptions
                                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            };

                            CustomizeAdvancedBloatware.Click += (sender, e) =>
                            {
                                try
                                {
                                    string jsonFilePath = "CustomSoftware.json"; // Adjust the path as needed

                                    // Read the JSON file
                                    string jsonData = System.IO.File.ReadAllText(jsonFilePath);

                                    // Deserialize JSON data
                                    dynamic customSoftware = JsonConvert.DeserializeObject(jsonData);

                                    // Extract BloatWare list
                                    List<string> bloatWare = customSoftware["WinGetBloat"].ToObject<List<string>>();
                                    
                                    // Extract CheckboxStates or initialize it if it doesn't exist
                                    Dictionary<string, bool> checkboxStates = customSoftware.ContainsKey("BloatwareCheckboxStates") ?
                                    customSoftware["BloatwareCheckboxStates"].ToObject<Dictionary<string, bool>>() :
                                    new Dictionary<string, bool>();

                                    // Open a form for editing BloatWare list
                                    using (var bloatwareForm = new Form())
                                    {
                                        // Set form properties
                                        bloatwareForm.ClientSize = new Size(700, 400);
                                        bloatwareForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                                        bloatwareForm.MinimizeBox = false;
                                        bloatwareForm.MaximizeBox = false;
                                        bloatwareForm.ShowIcon = false;
                                        bloatwareForm.Text = "Customize Bloatware";
                                        bloatwareForm.TopMost = false;
                                        bloatwareForm.AutoScroll = true;
                                        bloatwareForm.BackColor = System.Drawing.ColorTranslator.FromHtml("#252525");

                                        // Add Save button
                                        Button saveButton = new Button();
                                        saveButton.Text = "Save";
                                        saveButton.Width = 70;
                                        saveButton.Height = 30;
                                        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                                        saveButton.Location = new Point(350, 350);
                                        saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        saveButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(saveButton);

                                        // Add Add button
                                        Button addButton = new Button();
                                        addButton.Text = "Add";
                                        addButton.Width = 70;
                                        addButton.Height = 30;
                                        addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        addButton.Location = new Point((bloatwareForm.ClientSize.Width - saveButton.Width - addButton.Width - 20) / 2, bloatwareForm.ClientSize.Height - addButton.Height - 20);
                                        addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        addButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(addButton);

                                        // Add text box
                                        TextBox textBox = new TextBox();
                                        textBox.Width = 200;
                                        textBox.Height = 30;
                                        textBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        textBox.Location = new System.Drawing.Point(200, 200);
                                        textBox.Visible = false; // Initially hidden
                                        textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        textBox.ForeColor = System.Drawing.Color.Black;
                                        bloatwareForm.Controls.Add(textBox);

                                        // Add submit button
                                        Button submitButton = new Button();
                                        submitButton.Text = "Submit";
                                        submitButton.Width = 80;
                                        submitButton.Height = 30;
                                        submitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        submitButton.Location = new System.Drawing.Point(400, 195);
                                        submitButton.Visible = false; // Initially hidden
                                        submitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        submitButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(submitButton);

                                        // Add ? button
                                        Button ShowHelp = new Button();
                                        ShowHelp.Text = "?";
                                        ShowHelp.Width = 40;
                                        ShowHelp.Height = 30;
                                        ShowHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        ShowHelp.Location = new System.Drawing.Point(480, 195);
                                        ShowHelp.Visible = false; // Initially hidden
                                        ShowHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        ShowHelp.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(ShowHelp);

                                        Label NewBSSHelp = new Label();
                                        NewBSSHelp.Text = "Here you edit what software is removed from advanced bloatware.\nTo add new software you need to get the package ID or name from winget.\nTo get this open cmd and type 'winget list' if the software is installed.";
                                        NewBSSHelp.Location = new System.Drawing.Point(160, 10);
                                        NewBSSHelp.Size = new System.Drawing.Size(360, 150);
                                        NewBSSHelp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                                        NewBSSHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15);
                                        NewBSSHelp.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        NewBSSHelp.Visible = false;
                                        bloatwareForm.Controls.Add(NewBSSHelp);

                                        Button WingetList = new Button();
                                        WingetList.Text = "Run Command";
                                        WingetList.Width = 160;
                                        WingetList.Height = 30;
                                        WingetList.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        WingetList.Location = new System.Drawing.Point(250, 165);
                                        WingetList.Visible = false; // Initially hidden
                                        WingetList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        WingetList.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(WingetList);

                                        // Create a Panel for scrolling
                                        Panel scrollPanel = new Panel();
                                        scrollPanel.Dock = DockStyle.Fill;
                                        scrollPanel.AutoScroll = true;

                                        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                                        tableLayoutPanel.AutoSize = true; // Consider setting AutoSizeMode.GrowAndShrink for proper resizing
                                        tableLayoutPanel.ColumnCount = 4;
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Adjust this line to ensure the last column fills the remaining space

                                        int row = 0;
                                        foreach (string software in bloatWare)
                                        {
                                            // Create label for software
                                            Label label = new Label();
                                            label.Text = software;
                                            label.AutoSize = true;
                                            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                            // Create checkbox
                                            CheckBox checkBox = new CheckBox();
                                            checkBox.Text = "Include";
                                            checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                            checkBox.Checked = checkboxStates.ContainsKey(software) ? checkboxStates[software] : true; // Set checkbox state based on saved state or default to true if not found
                                            checkBox.Anchor = AnchorStyles.Right;

                                            // Assign software name to Tag property of checkbox
                                            checkBox.Tag = software;

                                            // Add controls to the table
                                            tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column
                                            tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column

                                            row++;

                                        }

                                        Label emptySpace = new Label();
                                        emptySpace.Height = 50; // Adjust the height as needed for your layout

                                        // Add empty space to the last row to create padding at the bottom
                                        tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                        tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                        saveButton.Click += (s, ev) =>
                                        {
                                            try
                                            {
                                                // Iterate over each control in the tableLayoutPanel
                                                foreach (Control control in tableLayoutPanel.Controls)
                                                {
                                                    if (control is CheckBox chkBox)
                                                    {
                                                        // Retrieve the software name from the Tag property of the checkbox
                                                        string softwareName = chkBox.Tag.ToString();

                                                        // Update the checkbox state in the checkboxStates dictionary
                                                        checkboxStates[softwareName] = chkBox.Checked;
                                                    }
                                                }

                                                // Update JSON data
                                                customSoftware["BloatwareCheckboxStates"] = JObject.FromObject(checkboxStates); // Update bloatware checkbox states

                                                // Serialize the updated JSON data
                                                string updatedJsonData = JsonConvert.SerializeObject(customSoftware, Formatting.Indented);

                                                // Write updated JSON data to file
                                                System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);

                                                // Confirmation message
                                                MessageBox.Show("BloatWare list and checkbox states updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            catch (Exception ex)
                                            {
                                                // Handle exceptions
                                                MessageBox.Show($"An error occurred while saving checkbox states: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        // Handle add button click event
                                        addButton.Click += (s, ev) =>
                                        {
                                            // Show text box and submit button
                                            textBox.Visible = true;
                                            submitButton.Visible = true;
                                            ShowHelp.Visible = true;
                                        };

                                        ShowHelp.Click += (s, ev) =>
                                        {
                                            NewBSSHelp.Visible = true;
                                            WingetList.Visible = true;
                                        };

                                        WingetList.Click += (s, ev) =>
                                        {
                                            Process process = new Process
                                            {
                                                StartInfo = new ProcessStartInfo
                                                {
                                                    FileName = "powershell.exe",
                                                    Arguments = "winget list",
                                                    UseShellExecute = false,
                                                    
                                                    //RedirectStandardOutput = true,
                                                    //RedirectStandardError = true,
                                                }
                                            };
                                            process.Start();
                                        };

                                        // Handle submit button click event
                                        submitButton.Click += (s, ev) =>
                                        {
                                            // Get the text from the text box
                                            string newBloatware = textBox.Text.Trim();

                                            // Add the new bloatware to the list
                                            if (!string.IsNullOrEmpty(newBloatware))
                                            {
                                                // Remove the empty space control from the table
                                                tableLayoutPanel.Controls.Remove(emptySpace);

                                                // Add the new software to the table
                                                bloatWare.Add(newBloatware); // Add to the list of bloatware

                                                // Update the customSoftware object with the new bloatware
                                                customSoftware["WinGetBloat"] = JArray.FromObject(bloatWare);

                                                // Create label for software
                                                Label label = new Label();
                                                label.Text = newBloatware;
                                                label.AutoSize = true;
                                                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create checkbox
                                                CheckBox checkBox = new CheckBox();
                                                checkBox.Name = "chk" + Guid.NewGuid().ToString("N"); // Generate a unique name for the checkbox
                                                checkBox.Text = "Include";
                                                checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                                checkBox.Checked = true; // Default to checked for new checkboxes
                                                checkBox.Anchor = AnchorStyles.Right; // Anchor checkbox to the right side of the cell

                                                // Assign software name to Tag property of checkbox
                                                checkBox.Tag = newBloatware;

                                                // Update the checkboxStates dictionary
                                                checkboxStates[newBloatware] = true;

                                                // Add controls to the table
                                                tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column of the current row
                                                tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column of the current row

                                                // Increment row count
                                                tableLayoutPanel.RowCount++;

                                                // Increment row index
                                                row++;

                                                // Clear the text box
                                                textBox.Text = "";

                                                // Re-add the empty space control to the table
                                                tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                                tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                                // Show or hide the text box and submit button as needed
                                                textBox.Visible = false;
                                                submitButton.Visible = false;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Please enter the name of the new bloatware.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        // Add tableLayoutPanel to the scrollPanel
                                        scrollPanel.Controls.Add(tableLayoutPanel);

                                        // Add scrollPanel to the form
                                        bloatwareForm.Controls.Add(scrollPanel);

                                        // Show the form
                                        bloatwareForm.ShowDialog();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Handle exceptions
                                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            };

                            CustomizeBSS.Click += (sender, e) =>
                            {
                                try
                                {
                                    string jsonFilePath = "CustomSoftware.json"; // Adjust the path as needed

                                    // Read the JSON file
                                    string jsonData = System.IO.File.ReadAllText(jsonFilePath);

                                    // Deserialize JSON data
                                    dynamic customSoftware = JsonConvert.DeserializeObject(jsonData);

                                    // Extract BloatWare list
                                    List<string> WingetPackages = customSoftware["WingetPackages"].ToObject<List<string>>();

                                    // Extract CheckboxStates or initialize it if it doesn't exist
                                    Dictionary<string, bool> checkboxStates = customSoftware.ContainsKey("WingetCheckboxStates") ?
                                    customSoftware["WingetCheckboxStates"].ToObject<Dictionary<string, bool>>() :
                                    new Dictionary<string, bool>();

                                    // Open a form for editing BloatWare list
                                    using (var bloatwareForm = new Form())
                                    {
                                        // Set form properties
                                        bloatwareForm.ClientSize = new Size(700, 400);
                                        bloatwareForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                                        bloatwareForm.MinimizeBox = false;
                                        bloatwareForm.MaximizeBox = false;
                                        bloatwareForm.ShowIcon = false;
                                        bloatwareForm.Text = "Customize Basic System Setup";
                                        bloatwareForm.TopMost = false;
                                        bloatwareForm.AutoScroll = true;
                                        bloatwareForm.BackColor = System.Drawing.ColorTranslator.FromHtml("#252525");

                                        // Add Save button
                                        Button saveButton = new Button();
                                        saveButton.Text = "Save";
                                        saveButton.Width = 70;
                                        saveButton.Height = 30;
                                        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                                        saveButton.Location = new Point(350, 350);
                                        saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        saveButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(saveButton);

                                        // Add Add button
                                        Button addButton = new Button();
                                        addButton.Text = "Add";
                                        addButton.Width = 70;
                                        addButton.Height = 30;
                                        addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        addButton.Location = new Point((bloatwareForm.ClientSize.Width - saveButton.Width - addButton.Width - 20) / 2, bloatwareForm.ClientSize.Height - addButton.Height - 20);
                                        addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        addButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(addButton);

                                        // Add text box
                                        TextBox textBox = new TextBox();
                                        textBox.Width = 200;
                                        textBox.Height = 30;
                                        textBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        textBox.Location = new System.Drawing.Point(200, 200);
                                        textBox.Visible = false; // Initially hidden
                                        textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        textBox.ForeColor = System.Drawing.Color.Black;
                                        bloatwareForm.Controls.Add(textBox);

                                        // Add submit button
                                        Button submitButton = new Button();
                                        submitButton.Text = "Submit";
                                        submitButton.Width = 80;
                                        submitButton.Height = 30;
                                        submitButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        submitButton.Location = new System.Drawing.Point(400, 195);
                                        submitButton.Visible = false; // Initially hidden
                                        submitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        submitButton.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(submitButton);

                                        // Add ? button
                                        Button ShowHelp = new Button();
                                        ShowHelp.Text = "?";
                                        ShowHelp.Width = 40;
                                        ShowHelp.Height = 30;
                                        ShowHelp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // Anchor to bottom left
                                        ShowHelp.Location = new System.Drawing.Point(480, 195);
                                        ShowHelp.Visible = false; // Initially hidden
                                        ShowHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                        ShowHelp.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        bloatwareForm.Controls.Add(ShowHelp);

                                        Label NewBSSHelp = new Label();
                                        NewBSSHelp.Text = "Here you can add new software to the basic system setup.\nTo add new software you need to get the package ID or name from winget.\nTo get this open cmd and type 'winget search' then the software name.";
                                        NewBSSHelp.Location = new System.Drawing.Point(160, 10);
                                        NewBSSHelp.Size = new System.Drawing.Size(360, 270);
                                        NewBSSHelp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                                        NewBSSHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15);
                                        NewBSSHelp.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                        NewBSSHelp.Visible = false;
                                        bloatwareForm.Controls.Add(NewBSSHelp);

                                        // Create a Panel for scrolling
                                        Panel scrollPanel = new Panel();
                                        scrollPanel.Dock = DockStyle.Fill;
                                        scrollPanel.AutoScroll = true;

                                        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                                        tableLayoutPanel.AutoSize = true; // Consider setting AutoSizeMode.GrowAndShrink for proper resizing
                                        tableLayoutPanel.ColumnCount = 4;
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200)); // Adjust this line to ensure the last column fills the remaining space

                                        int row = 0;
                                        foreach (string software in WingetPackages)
                                        {
                                            // Create label for software
                                            Label label = new Label();
                                            label.Text = software;
                                            label.AutoSize = true;
                                            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                            // Create checkbox
                                            CheckBox checkBox = new CheckBox();
                                            checkBox.Text = "Include";
                                            checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                            checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                            checkBox.Checked = checkboxStates.ContainsKey(software) ? checkboxStates[software] : true; // Set checkbox state based on saved state or default to true if not found
                                            checkBox.Anchor = AnchorStyles.Right;

                                            // Assign software name to Tag property of checkbox
                                            checkBox.Tag = software;

                                            // Add controls to the table
                                            tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column
                                            tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column

                                            row++;

                                        }

                                        Label emptySpace = new Label();
                                        emptySpace.Height = 50; // Adjust the height as needed for your layout

                                        // Add empty space to the last row to create padding at the bottom
                                        tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                        tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                        saveButton.Click += (s, ev) =>
                                        {
                                            try
                                            {
                                                // Iterate over each control in the tableLayoutPanel
                                                foreach (Control control in tableLayoutPanel.Controls)
                                                {
                                                    if (control is CheckBox chkBox)
                                                    {
                                                        // Retrieve the software name from the Tag property of the checkbox
                                                        string softwareName = chkBox.Tag.ToString();

                                                        // Update the checkbox state in the checkboxStates dictionary
                                                        checkboxStates[softwareName] = chkBox.Checked;
                                                    }
                                                }

                                                // Update JSON data
                                                customSoftware["WingetCheckboxStates"] = JObject.FromObject(checkboxStates); // Update bloatware checkbox states

                                                // Serialize the updated JSON data
                                                string updatedJsonData = JsonConvert.SerializeObject(customSoftware, Formatting.Indented);

                                                // Write updated JSON data to file
                                                System.IO.File.WriteAllText(jsonFilePath, updatedJsonData);

                                                // Confirmation message
                                                MessageBox.Show("BloatWare list and checkbox states updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            catch (Exception ex)
                                            {
                                                // Handle exceptions
                                                MessageBox.Show($"An error occurred while saving checkbox states: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        // Handle add button click event
                                        addButton.Click += (s, ev) =>
                                        {
                                            // Show text box and submit button
                                            textBox.Visible = true;
                                            submitButton.Visible = true;
                                            ShowHelp.Visible = true;
                                        };

                                        ShowHelp.Click += (s, ev) => 
                                        {
                                            NewBSSHelp.Visible = true;
                                        };

                                        // Handle submit button click event
                                        submitButton.Click += (s, ev) =>
                                        {
                                            // Get the text from the text box
                                            string newBloatware = textBox.Text.Trim();

                                            // Add the new bloatware to the list
                                            if (!string.IsNullOrEmpty(newBloatware))
                                            {
                                                // Remove the empty space control from the table
                                                tableLayoutPanel.Controls.Remove(emptySpace);

                                                // Add the new software to the table
                                                WingetPackages.Add(newBloatware); // Add to the list of bloatware

                                                // Update the customSoftware object with the new bloatware
                                                customSoftware["WingetPackages"] = JArray.FromObject(WingetPackages);

                                                // Create label for software
                                                Label label = new Label();
                                                label.Text = newBloatware;
                                                label.AutoSize = true;
                                                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");

                                                // Create checkbox
                                                CheckBox checkBox = new CheckBox();
                                                checkBox.Name = "chk" + Guid.NewGuid().ToString("N"); // Generate a unique name for the checkbox
                                                checkBox.Text = "Include";
                                                checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
                                                checkBox.ForeColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                                                checkBox.Checked = true; // Default to checked for new checkboxes
                                                checkBox.Anchor = AnchorStyles.Right; // Anchor checkbox to the right side of the cell

                                                // Assign software name to Tag property of checkbox
                                                checkBox.Tag = newBloatware;

                                                // Update the checkboxStates dictionary
                                                checkboxStates[newBloatware] = true;

                                                // Add controls to the table
                                                tableLayoutPanel.Controls.Add(label, 0, row); // Add label to the first column of the current row
                                                tableLayoutPanel.Controls.Add(checkBox, 3, row); // Add checkbox to the fourth (rightmost) column of the current row

                                                // Increment row count
                                                tableLayoutPanel.RowCount++;

                                                // Increment row index
                                                row++;

                                                // Clear the text box
                                                textBox.Text = "";

                                                // Re-add the empty space control to the table
                                                tableLayoutPanel.Controls.Add(emptySpace, 0, row);
                                                tableLayoutPanel.SetColumnSpan(emptySpace, 4); // Span it across all columns

                                                // Show or hide the text box and submit button as needed
                                                textBox.Visible = false;
                                                submitButton.Visible = false;
                                                ShowHelp.Visible = false;
                                                NewBSSHelp.Visible = false;
                                            }
                                            else
                                            {
                                                MessageBox.Show("Please enter the name of the new Winget Packages.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        };

                                        // Add tableLayoutPanel to the scrollPanel
                                        scrollPanel.Controls.Add(tableLayoutPanel);

                                        // Add scrollPanel to the form
                                        bloatwareForm.Controls.Add(scrollPanel);

                                        // Show the form
                                        bloatwareForm.ShowDialog();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Handle exceptions
                                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            };
                        }
                    }
                };

                //removeBloatware
                {
                    removeBloatware.Click += (sender, e) =>
                    {
                        pleaseWaitLabel.Visible = true;

                        // Read the JSON file
                        string jsonFilePath = "CustomSoftware.json"; // Adjust the path as needed
                        string jsonData = System.IO.File.ReadAllText(jsonFilePath);

                        // Deserialize JSON data
                        dynamic customSoftware = JsonConvert.DeserializeObject(jsonData);

                        // Extract BloatWare, WhiteListedApps, and nonRemovable lists
                        List<string> bloatWare = customSoftware["BloatWare"].ToObject<List<string>>();
                        List<string> whiteListedApps = customSoftware["WhiteListedApps"].ToObject<List<string>>();
                        List<string> nonRemovable = customSoftware["nonRemovable"].ToObject<List<string>>();
                        Dictionary<string, bool> checkboxStates = customSoftware["BloatwareCheckboxStates"].ToObject<Dictionary<string, bool>>();

                        foreach (string packageName in bloatWare)
                        {
                            // Check if the package is whitelisted or non-removable
                            if (whiteListedApps.Contains(packageName))
                            {
                                Console.WriteLine($"Package '{packageName}' is whitelisted and will not be uninstalled.");
                            }
                            else if (nonRemovable.Contains(packageName))
                            {
                                Console.WriteLine($"Package '{packageName}' is in the non-removable list and will not be uninstalled.");
                            }
                            else
                            {
                                // Check checkbox state
                                if (checkboxStates.ContainsKey(packageName) && checkboxStates[packageName])
                                {
                                    UninstallAppxPackage(packageName);
                                }
                            }
                        }
                        Console.WriteLine("Customized Bloatware Has Been Removed");
                        pleaseWaitLabel.Visible = false;
                    };

                    static void UninstallAppxPackage(string packageName)
                        {
                            try
                            {
                                using (Process process = new Process())
                                {
                                    ProcessStartInfo startInfo = new ProcessStartInfo
                                    {
                                        FileName = "powershell.exe",
                                        RedirectStandardInput = true,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = true,
                                        UseShellExecute = false
                                    };

                                    process.StartInfo = startInfo;
                                    process.Start();

                                    using (var sw = process.StandardInput)
                                    {
                                        if (sw.BaseStream.CanWrite)
                                        {
                                            // Uninstall the appx package using PowerShell command
                                            sw.WriteLine($"Get-AppxPackage -Name {packageName} | Remove-AppxPackage");
                                        }
                                    }

                                    process.WaitForExit();
                                    int exitCode = process.ExitCode;

                                    if (exitCode == 0)
                                    {
                                        Console.WriteLine($"Package '{packageName}' has been uninstalled successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to uninstall package '{packageName}'. Exit code: {exitCode}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }

                        Console.WriteLine("Bloatware removed!");
                        //pleaseWaitLabel.Visible = false;
                    };

                //removeAdvancedBloatware
                {
                    removeAdvancedBloatware.Click += removeAdvancedBloatware_Click;
                    async void removeAdvancedBloatware_Click(object sender, EventArgs e)
                    {
                        pleaseWaitLabel.Visible = true;
                        try
                        {
                            if (!IsWinGetInstalled())
                            {
                                InstallWinget();
                                if (!IsWinGetInstalled())
                                {
                                    throw new Exception("Failed to install WinGet.");
                                }
                            }
                            // Read the JSON file
                            string jsonFilePath = "CustomSoftware.json"; // Adjust the path as needed
                            string jsonData = System.IO.File.ReadAllText(jsonFilePath);

                            // Deserialize JSON data
                            dynamic customSoftware = JsonConvert.DeserializeObject(jsonData);

                            // Extract BloatWare, WhiteListedApps, and nonRemovable lists
                            List<string> WinGetBloat = customSoftware["WinGetBloat"].ToObject<List<string>>();

                            Console.WriteLine("Removing Customized Bloatware");

                            foreach (string packageName in WinGetBloat)
                            {
                                await UnInstallWinGetPackage(packageName);
                            }

                            Console.WriteLine("Final Cleanup");
                            string outputFilePath = "C:/Temp/DebloatAndFloat/Logs/winget-list.txt";

                            using (var outputStream = new StreamWriter(outputFilePath))
                            {
                                using (Process process = new Process())
                                {
                                    process.StartInfo.FileName = "winget.exe";
                                    process.StartInfo.Arguments = "list";
                                    process.StartInfo.CreateNoWindow = true;
                                    process.StartInfo.UseShellExecute = false;
                                    process.StartInfo.RedirectStandardOutput = true;

                                    // Set up an event handler to capture the output data
                                    process.OutputDataReceived += (sender, e) =>
                                    {
                                        if (!string.IsNullOrEmpty(e.Data))
                                        {
                                            string cleanedLine = CleanOutput(e.Data);

                                            outputStream.WriteLine(cleanedLine);
                                        }
                                    };

                                    process.Start();

                                    // Begin asynchronous reading of the standard output stream
                                    process.BeginOutputReadLine();

                                    await process.WaitForExitAsync();
                                }
                            }

                            Console.WriteLine("Customized Bloatware Has Been Removed");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        pleaseWaitLabel.Visible = false;
                    };

                    async Task UnInstallWinGetPackage(string packageName)
                    {
                        Console.WriteLine($"Uninstalling {packageName}.");
                        string output = await RunCommandAsync($"uninstall \"{packageName}\" -h --disable-interactivity --accept-source-agreements");
                        Console.WriteLine($"{packageName} Uninstalled.");
                    }
                }

                //removeAllBloatware
                {
                    removeAllBloatware.Click += (sender, e) =>
                    {
                        Console.WriteLine("Removing bloatware and appling optional changes.");
                        removeBloatware.PerformClick();
                        removeAdvancedBloatware.PerformClick();
                        disableTaskbarBloat.PerformClick();
                        disableTelemetry.PerformClick();
                        longSleep.PerformClick();
                        stopEdgePDFTakeover.PerformClick();
                        //removeRegkeys.PerformClick();
                        removeStartBloat.PerformClick();
                        Console.WriteLine("Removed bloatware and appled optional changes.");
                    };
                }

                //Windows Corruption
                {
                    WindowsRepair.Click += WindowsRepair_Click;
                    async void WindowsRepair_Click(object sender, EventArgs e)
                    {
                        pleaseWaitLabel.Visible = true;
                        Console.WriteLine("Starting SFC");
                        await RunPowerShellAsync("sfc /scannow");
                        Console.WriteLine("Starting DISM");
                        // await RunPowerShellAsync("DISM /Online /Cleanup-Image /ScanHealth");
                        await RunPowerShellAsync("DISM /Online /Cleanup-Image /RestoreHealth");
                        pleaseWaitLabel.Visible = false;
                    };
                }

                //CheckRemoteDesktop
                {
                    CheckRemote.Click += (sender, e) =>
                    {
                        pleaseWaitLabel.Visible = true;
                        CheckInstalledSoftware("RemotePC");
                        CheckInstalledSoftware("TeamViewer");
                        CheckInstalledSoftware("AnyDesk");

                        {
                            string userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                            string screenConnectPath = Path.Combine(userProfile, @"AppData\Local\Apps\2.0");

                            if (Directory.Exists(screenConnectPath))
                            {
                                Console.WriteLine("ScreenConnect folder exists");
                                Console.WriteLine("Stopping ScreenConnect");
                                if (!KillProcess("ScreenConnect.WindowsClient.exe"))
                                {
                                    Console.WriteLine("Failed to stop ScreenConnect.WindowsClient.exe");
                                }
                                if (!KillProcess("ScreenConnect.ClientService.exe"))
                                {
                                    Console.WriteLine("Failed to stop ScreenConnect.ClientService.exe");
                                }
                                Console.WriteLine("Deleting ScreenConnect folder");
                                Directory.Delete(screenConnectPath, true);
                            }
                            else
                            {
                                Console.WriteLine("ScreenConnect is NOT installed");
                            }
                        }

                        static bool KillProcess(string processName)
                        {
                            Process[] processes = Process.GetProcessesByName(processName);
                            foreach (Process process in processes)
                            {
                                try
                                {
                                    process.Kill();
                                    process.WaitForExit(); // Wait for the process to exit
                                    return true; // Return true if the process was successfully killed
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error killing process {processName}: {ex.Message}");
                                }
                            }
                            return false; // Return false if the process couldn't be killed
                        }
                        pleaseWaitLabel.Visible = false;
                    };
                    static void CheckInstalledSoftware(string softwareName)
                    {
                        var processStartInfo = new ProcessStartInfo
                        {
                            FileName = "wmic",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = $@"product where ""Name like '{softwareName}%"""" get Name"
                        };

                        using (var process = Process.Start(processStartInfo))
                        {
                            if (process != null)
                            {
                                string output = process.StandardOutput.ReadToEnd();
                                process.WaitForExit();

                                if (process.ExitCode == 0)
                                {
                                    Console.WriteLine($"{softwareName} is installed");
                                }
                                else
                                {
                                    Console.WriteLine($"{softwareName} is NOT installed");
                                }
                            }
                        }
                    };
                }

                //TaskbarBloat
                {
                    disableTaskbarBloat.Click += (sender, e) =>
                    {
                        try
                        {
                            pleaseWaitLabel.Visible = true;
                            Console.WriteLine("Removing Taskbar Bloat");

                            // Registry keys
                            string cortana1 = @"SOFTWARE\Microsoft\Personalization\Settings";
                            string cortana2 = @"SOFTWARE\Microsoft\InputPersonalization";
                            string cortana3 = @"SOFTWARE\Microsoft\InputPersonalization\TrainedDataStore";
                            string taskbarButtons = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                            string searchHighlights = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";
                            string meetNow1 = @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";

                            // Disable Cortana
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + cortana1, "AcceptedPrivacyPolicy", 0, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + cortana2, "RestrictImplicitTextCollection", 1, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + cortana2, "RestrictImplicitInkCollection", 1, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + cortana3, "HarvestContacts", 0, RegistryValueKind.DWord);

                            // Disable Taskbar Bloat
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "ShowCortanaButton", 0, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "ShowTaskViewButton", 0, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "TaskbarMn", 0, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "HideSCAMeetNow", 0, RegistryValueKind.DWord);
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "ShowCopilotButton", 0, RegistryValueKind.DWord);

                            // Disable Search Highlights
                            RegistryKey searchKey = Registry.LocalMachine.CreateSubKey(searchHighlights, true);
                            searchKey.SetValue("EnableDynamicContentInWSB", 0, RegistryValueKind.DWord);

                            // Hide SCAMeetNow
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + meetNow1, "HideSCAMeetNow", 1, RegistryValueKind.DWord);

                            // Restart Explorer
                            int OSVersion = DetermineResult(BuildNumber);
                            if (OSVersion == 10)
                            {
                                Process.Start("taskkill", "/f /im explorer.exe").WaitForExit();
                                Process.Start("explorer.exe");
                            }
                            Console.WriteLine("Taskbar Bloat Has Been Removed.");
                            pleaseWaitLabel.Visible = false;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    };
                }

                //Long Sleep
                {
                    longSleep.Click += (sender, e) =>
                    {
                        Console.WriteLine("Changing Sleep Settings");

                        int SleepSettingAC = (int)appSettings.sleepsettingAC;
                        int SleepSettingDC = (int)appSettings.sleepsettingDC;
                        int MonitorSettingAC = (int)appSettings.monitorsettingAC;
                        int MonitorSettingDC = (int)appSettings.monitorsettingDC;

                        Process cmd = new Process();

                        cmd.StartInfo.FileName = "cmd.exe";
                        cmd.StartInfo.RedirectStandardInput = true;
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.StartInfo.CreateNoWindow = true;

                        cmd.Start();

                        using (StreamWriter sw = cmd.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                // Change sleep settings based on the longerSleepSetting
                                sw.WriteLine($"Powercfg /Change monitor-timeout-ac {MonitorSettingAC}");
                                sw.WriteLine($"Powercfg /Change monitor-timeout-dc {MonitorSettingDC}");
                                sw.WriteLine($"Powercfg /Change standby-timeout-ac {SleepSettingAC}");
                                sw.WriteLine($"Powercfg /Change standby-timeout-dc {SleepSettingDC}");
                            }
                        }
                        Console.WriteLine("Sleep Settings Changed.");
                    };
                }

                //RemoveStartBloat
                {
                    removeStartBloat.Click += (sender, e) =>
                    {
                        try
                        {
                            Console.WriteLine("Removing Online Services");
                            OnlineServices();
                            Console.WriteLine("Removed Online Services.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }

                        string scriptResourceName = "Debloat_And_Float.Resources.start2.bin";

                        // Destination directory
                        string destinationDirectory = "appdata/local/Packages/Microsoft.Windows.StartMenuExperienceHost_cw5n1h2txyewy/LocalState";

                        string homeFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                        // Combine the home folder path with the additional path
                        string combinedPath = Path.Combine(homeFolderPath, destinationDirectory);

                        // Ensure the destination directory exists
                        Directory.CreateDirectory(combinedPath);

                        // Copy the embedded resources to the destination directory
                        CopyEmbeddedResource(scriptResourceName, Path.Combine(combinedPath, "start2.bin"));
                    };

                    void OnlineServices()
                    {
                        string partialTargetPathToCheck = "C:\\Program Files (x86)\\Online Services";
                        string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

                        foreach (string shortcutFile in Directory.GetFiles(startMenuPath, "*.lnk", SearchOption.AllDirectories))
                        {
                            try
                            {
                                string shortcutTarget = GetShortcutTarget(shortcutFile);

                                if (shortcutTarget.Contains(partialTargetPathToCheck))
                                {
                                    Console.WriteLine("Deleting .lnk file: " + Path.GetFileName(shortcutFile));
                                    System.IO.File.Delete(shortcutFile);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error processing shortcut: " + ex.Message);
                            }
                        }

                        foreach (string urlFile in Directory.GetFiles(startMenuPath, "*.url", SearchOption.AllDirectories))
                        {
                            Console.WriteLine("Deleting .url file: " + Path.GetFileName(urlFile));
                            System.IO.File.Delete(urlFile);
                        }

                        Console.WriteLine("Deleting Online Services Folder");
                        RunPowerShellAsync($"rd -Force -Recurse '\"{partialTargetPathToCheck}\"'");
                    }

                    string GetShortcutTarget(string shortcutPath)
                    {
                        IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                        IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
                        return shortcut.TargetPath;
                    }
                }

                //stopEdgePDFTakeover
                {
                    stopEdgePDFTakeover.Click += (sender, e) =>
                    {
                        Microsoft.Win32.RegistryKey noOpenWith = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".pdf", true);
                        Microsoft.Win32.RegistryKey noProgids = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".pdf\\OpenWithProgids", true);
                        Microsoft.Win32.RegistryKey noWithList = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".pdf\\OpenWithList", true);

                        if (noOpenWith != null && noProgids != null && noWithList != null)
                        {
                            noOpenWith.SetValue("NoOpenWith", 1, Microsoft.Win32.RegistryValueKind.DWord);
                            noOpenWith.SetValue("NoStaticDefaultVerb", 1, Microsoft.Win32.RegistryValueKind.DWord);

                            noProgids.SetValue("NoOpenWith", 1, Microsoft.Win32.RegistryValueKind.DWord);
                            noProgids.SetValue("NoStaticDefaultVerb", 1, Microsoft.Win32.RegistryValueKind.DWord);

                            noWithList.SetValue("NoOpenWith", 1, Microsoft.Win32.RegistryValueKind.DWord);
                            noWithList.SetValue("NoStaticDefaultVerb", 1, Microsoft.Win32.RegistryValueKind.DWord);
                        }

                        string edgeKey = "AppXd4nrz8ff68srnhf9t5a8sbjyar1cr723_";
                        Microsoft.Win32.RegistryKey edge = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(edgeKey, true);

                        if (edge != null)
                        {
                            edge.SetValue("", edgeKey);
                        }

                        Console.WriteLine("Edge should no longer take over as the default .PDF.");
                    };
                }

                //DisableTelemitry
                {
                    disableTelemetry.Click += (sender, e) =>
                    {
                        pleaseWaitLabel.Visible = true;
                        Console.WriteLine("Disabling Telemetry");
                        // Registry keys
                        string advertisingInfoKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo";
                        string windowsSearchKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";
                        string contentDeliveryManagerKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
                        string holographicKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Holographic";
                        string wifiSense1KeyPath = @"SOFTWARE\Microsoft\PolicyManager\default\WiFi\AllowWiFiHotSpotReporting";
                        string wifiSense2KeyPath = @"SOFTWARE\Microsoft\PolicyManager\default\WiFi\AllowAutoConnectToWiFiSenseHotspots";
                        string wifiSense3KeyPath = @"SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config";
                        string pushNotificationsKeyPath = @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications";
                        string dataCollection1KeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";
                        string dataCollection2KeyPath = @"SOFTWARE\Policies\Microsoft\Windows\DataCollection";
                        string dataCollection3KeyPath = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection";
                        string sensorStateKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
                        string locationConfigKeyPath = @"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration";
                        string peopleIconKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";

                        // Modify registry keys
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + advertisingInfoKeyPath, "Enabled", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + windowsSearchKeyPath, "AllowCortana", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_CURRENT_USER\" + contentDeliveryManagerKeyPath, "BingSearchEnabled", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_CURRENT_USER\" + contentDeliveryManagerKeyPath, "DisableWebSearch", 1, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_CURRENT_USER\" + holographicKeyPath, "FirstRunSucceeded", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + wifiSense1KeyPath, "Value", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + wifiSense2KeyPath, "Value", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + wifiSense3KeyPath, "AutoConnectAllowedOEM", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_CURRENT_USER\" + pushNotificationsKeyPath, "NoTileApplicationNotification", 1, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + dataCollection1KeyPath, "AllowTelemetry", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + dataCollection2KeyPath, "AllowTelemetry", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + dataCollection3KeyPath, "AllowTelemetry", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + sensorStateKeyPath, "SensorPermissionState", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\" + locationConfigKeyPath, "Status", 0, RegistryValueKind.DWord);
                        Registry.SetValue(@"HKEY_CURRENT_USER\" + peopleIconKeyPath, "PeopleBand", 0, RegistryValueKind.DWord);

                        Console.WriteLine("Telemetry has been disabled!");
                        pleaseWaitLabel.Visible = false;
                    };
                }

                //removeRegkeys (Disabled)
                {
                    //removeRegkeys.Click += (sender, e) =>
                    //{
                    //    pleaseWaitLabel.Visible = true;
                    //    RegistryKey contractIdKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Extensions\ContractId", true);
                    //    RegistryKey windowsBackgroundTasksKey = contractIdKey?.OpenSubKey("Windows.BackgroundTasks", true);
                    //    RegistryKey windowsFileKey = contractIdKey?.OpenSubKey("Windows.File", true);
                    //    RegistryKey windowsLaunchKey = contractIdKey?.OpenSubKey("Windows.Launch", true);
                    //    RegistryKey windowsPreInstalledConfigTaskKey = contractIdKey?.OpenSubKey("Windows.PreInstalledConfigTask", true);
                    //    RegistryKey windowsProtocolKey = contractIdKey?.OpenSubKey("Windows.Protocol", true);
                    //    RegistryKey windowsShareTargetKey = contractIdKey?.OpenSubKey("Windows.ShareTarget", true);

                    //    string[] keys = new string[]
                    //    {
                    //    // Remove Background Tasks
                    //    "46928bounde.EclipseManager_2.2.4.51_neutral__a5h4egax66k6y",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0",
                    //    "Microsoft.MicrosoftOfficeHub_17.7909.7600.0_x64__8wekyb3d8bbwe",
                    //    "Microsoft.PPIProjection_10.0.15063.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.15063.0.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.16299.15.0_neutral_neutral_cw5n1h2txyewy",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0",
                    //    "46928bounde.EclipseManager_2.2.4.51_neutral__a5h4egax66k6y",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0",
                    //    "Microsoft.PPIProjection_10.0.15063.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.15063.0.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.16299.15.0_neutral_neutral_cw5n1h2txyewy",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0",
                    //    "Microsoft.MicrosoftOfficeHub_17.7909.7600.0_x64__8wekyb3d8bbwe",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0",
                    //    "Microsoft.PPIProjection_10.0.15063.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.15063.0.0_neutral_neutral_cw5n1h2txyewy",
                    //    "Microsoft.XboxGameCallableUI_1000.16299.15.0_neutral_neutral_cw5n1h2txyewy",
                    //    "ActiproSoftwareLLC.562882FEEB491_2.6.18.18_neutral__24pqs290vpjk0"
                    //    };

                    //    foreach (var key in keys)
                    //    {
                    //        string registryKeyPath = $@"SOFTWARE\Classes\Extensions\ContractId\Windows.BackgroundTasks\PackageId\{key}";
                    //        Console.WriteLine($"Removing {registryKeyPath} from registry");
                    //        windowsBackgroundTasksKey?.DeleteSubKeyTree($"PackageId\\{key}");
                    //    }

                    //    pleaseWaitLabel.Visible = false;
                    //    Console.WriteLine("Additional bloatware keys have been removed!");
                    //};
                }

                //Enable Dark Mode
                {
                    enableDarkMode.Click += (sender, e) =>
                    {
                        try
                        {
                            //WriteLog("Enabling Dark Mode");
                            RegistryKey themeKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", true);
                            themeKey.SetValue("AppsUseLightTheme", 0);
                            themeKey.SetValue("SystemUsesLightTheme", 0);
                            themeKey.Close();
                            System.Threading.Thread.Sleep(1000); // Sleep for 1 second
                            // Restart Explorer
                            Process.Start("taskkill", "/f /im explorer.exe").WaitForExit();
                            Process.Start("explorer.exe");
                            Console.WriteLine("Dark Mode Enabled");
                        }
                        catch (Exception ex)
                        {
                            //WriteLog("Error: " + ex.Message);
                        }
                    };
                }

                //Enable Light Mode
                {
                    disableDarkMode.Click += (sender, e) =>
                    {
                        try
                        {
                            //WriteLog("Disabling Dark Mode");
                            RegistryKey themeKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", true);
                            themeKey.SetValue("AppsUseLightTheme", 1);
                            themeKey.SetValue("SystemUsesLightTheme", 1);
                            themeKey.Close();
                            System.Threading.Thread.Sleep(1000); // Sleep for 1 second
                            // Restart Explorer
                            Process.Start("taskkill", "/f /im explorer.exe").WaitForExit();
                            Process.Start("explorer.exe");
                            Console.WriteLine("Dark Mode Disabled");
                        }
                        catch (Exception ex)
                        {
                            //WriteLog("Error: " + ex.Message);
                        }
                    };
                }

                //Taskbar to center
                {
                    Taskbartocenter.Click += (sender, e) =>
                    {
                        if (OSVersion == 11)
                        {
                            string taskbarButtons = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "TaskbarAl", 1, RegistryValueKind.DWord);
                            Console.WriteLine("Set Taskbar to Center");
                        }
                        else
                        {
                            Console.WriteLine("You are NOT on Windows 11");
                        }
                    };
                }

                //Taskbar to left
                {
                    Taskbartoleft.Click += (sender, e) =>
                    {
                        if (OSVersion == 11)
                        {
                            string taskbarButtons = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                            Registry.SetValue(@"HKEY_CURRENT_USER\" + taskbarButtons, "TaskbarAl", 0, RegistryValueKind.DWord);
                            Console.WriteLine("Set Taskbar to Left");
                        }
                        else
                        {
                            Console.WriteLine("You are NOT on Windows 11");
                        }
                    };
                }

                //Install Basic System Setup
                {
                    basicSS.Click += basicSS_Click;
                    async void basicSS_Click(object sender, EventArgs e)
                    {
                        pleaseWaitLabel.Visible = true;
                        try
                        {
                            if (!IsWinGetInstalled())
                            {
                                InstallWinget();
                                if (!IsWinGetInstalled())
                                {
                                    throw new Exception("Failed to install WinGet.");
                                }
                            }

                            // Read the JSON file
                            string jsonFilePath = "CustomSoftware.json"; // Change the path if necessary
                            if (!System.IO.File.Exists(jsonFilePath))
                            {
                                throw new FileNotFoundException("CustomSoftware.json not found.");
                            }

                            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);
                            dynamic customSoftware = JsonConvert.DeserializeObject(jsonContent);
                            List<string> WingetPackages = customSoftware["WingetPackages"].ToObject<List<string>>();
                            Dictionary<string, bool> checkboxStates = customSoftware["WingetCheckboxStates"].ToObject<Dictionary<string, bool>>();

                            // Install each package listed in the JSON file
                            foreach (var package in WingetPackages)
                            {
                                if (checkboxStates.ContainsKey(package) && checkboxStates[package])
                                {
                                    string packageName = package.ToString();
                                    await InstallWinGetPackage(packageName);
                                }
                            }

                            Console.WriteLine("Basic System Setup Installed");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        pleaseWaitLabel.Visible = false;

                        async Task InstallWinGetPackage(string packageName)
                        {
                            Console.WriteLine($"Installing {packageName}.");
                            string output = await RunCommandAsync($"install \"{packageName}\" -h --accept-package-agreements --accept-source-agreements");
                        }
                    }
                }

                //ISFile
                {
                    isFile.Click += (sender, e) =>
                    {
                        pleaseWaitLabel.Visible = true;
                        Console.WriteLine("Launching Auto Installer");
                        Console.WriteLine("This will take some time...");

                        string scriptResourceName = "Debloat_And_Float.Resources.Autoinstaller.ps1";

                        // Destination directory
                        string destinationDirectory = "C:/Temp/DebloatAndFloat/bin";

                        // Ensure the destination directory exists
                        Directory.CreateDirectory(destinationDirectory);

                        // Copy the embedded resources to the destination directory
                        CopyEmbeddedResource("Debloat_And_Float.Resources.Autoinstaller.ps1", Path.Combine(destinationDirectory, "Autoinstaller.ps1"));
                        CopyEmbeddedResource("Debloat_And_Float.Resources.HtmlAgilityPack.dll", Path.Combine(destinationDirectory, "HtmlAgilityPack.dll"));

                        // Run the first resource (Resource1.ps1)
                        string scriptPath = Path.Combine(destinationDirectory, "Autoinstaller.ps1");
                        RunPowerShellScript(scriptPath);

                        Console.WriteLine("Auto Installer Finished.");
                        pleaseWaitLabel.Visible = false;
                    };

                    static void RunPowerShellScript(string scriptPath)
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "powershell.exe";
                            process.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
                            process.StartInfo.UseShellExecute = false;
                            //process.StartInfo.CreateNoWindow = true;
                            //process.StartInfo.RedirectStandardOutput = true;
                            //process.StartInfo.RedirectStandardInput = true;
                            //process.StartInfo.RedirectStandardError = true;

                            process.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
                            {
                                Console.WriteLine(e.Data);
                            });
                            process.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
                            {
                                Console.WriteLine(e.Data);
                            });

                            process.Start();
                            //process.BeginOutputReadLine();
                            //process.BeginErrorReadLine();

                            // Send input to PowerShell
                            //StreamWriter sw = process.StandardInput;
                            //sw.WriteLine("Write-Host 'Enter your input: '");
                            //sw.WriteLine("$input = Read-Host");
                            //sw.WriteLine("Write-Host 'You entered: ' $input");

                            process.WaitForExit();

                            // Close the StreamWriter
                            //sw.Close();
                        }
                    }
                }

                //Install ToDo Backup
                {
                    itdb.Click += itdb_Click;
                    async void itdb_Click(object sender, EventArgs e)
                    {
                        pleaseWaitLabel.Visible = true;
                        try
                        {
                            if (!IsWinGetInstalled())
                            {
                                InstallWinget();
                                if (!IsWinGetInstalled())
                                {
                                    throw new Exception("Failed to install WinGet.");
                                }
                            }

                            Console.WriteLine("Installing Todo Backup");
                            await InstallWinGetPackage("EaseUS.TodoBackup");
                            Console.WriteLine("Todo Backup Installed");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        pleaseWaitLabel.Visible = false;
                    };

                    async Task InstallWinGetPackage(string packageName)
                    {
                        Console.WriteLine($"Installing {packageName}.");
                        string output = await RunCommandAsync($"install \"{packageName}\" -h --accept-package-agreements --accept-source-agreements");
                    }
                }

                //Install Game Launchers
                {
                    installGames.Click += InstallGames_Click;
                    async void InstallGames_Click(object sender, EventArgs e)
                    {
                        pleaseWaitLabel.Visible = true;
                        try
                        {
                            if (!IsWinGetInstalled())
                            {
                                InstallWinget();
                                if (!IsWinGetInstalled())
                                {
                                    throw new Exception("Failed to install WinGet.");
                                }
                            }
                            Console.WriteLine("Installing Game Launchers");
                            await InstallWinGetPackage("Valve.Steam");
                            await InstallWinGetPackage("EpicGames.EpicGamesLauncher");
                            Console.WriteLine("Insalled Launchers");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        pleaseWaitLabel.Visible = false;
                    };

                    async Task InstallWinGetPackage(string packageName)
                    {
                        Console.WriteLine($"Installing {packageName}.");
                        string output = await RunCommandAsync($"install \"{packageName}\" -h --accept-package-agreements --accept-source-agreements");
                    }
                }
            }

            //Winget Code

            static bool IsWinGetInstalled()
            {
                try
                {
                    Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "winget.exe",
                            Arguments = "-v",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    Console.WriteLine($"Winget Version: {output}");

                    // Check for successful execution (exit code 0) and non-empty output
                    if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                    {
                        // Use a regular expression to extract version information
                        // Assuming the version string is in the format "vX.X.X"
                        Regex regex = new Regex(@"v(\d+\.\d+\.\d+)");
                        Match match = regex.Match(output);

                        if (match.Success)
                        {
                            string versionString = match.Groups[1].Value;
                            Version wingetVersion = Version.Parse(versionString);

                            // Compare with minimum required version (1.5)
                            Version minimumRequiredVersion = new Version(1, 5);

                            return wingetVersion >= minimumRequiredVersion;
                        }
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            static string InstallWinget()
            {
                const int maxAttempts = 2;

                string xamlZipPath = @"C:/Temp/DebloatAndFloat/bin/xaml.zip";
                string xamlDirectory = @"C:/Temp/DebloatAndFloat/bin/xaml";
                string wingetMsixBundlePath = Path.Combine(@"C:/Temp/DebloatAndFloat/bin", "winget.msixbundle");

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        Console.WriteLine($"Installation Attempt {attempt}:");

                        Console.WriteLine("Installing WinGet.");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        string nugetUri = "https://www.nuget.org/api/v2/package/Microsoft.UI.Xaml/2.8";
                        string wingetDownloadUrl = "https://aka.ms/getwinget";
                        string appxPath = Path.Combine(xamlDirectory, "tools", "AppX", "x64", "Release", "Microsoft.UI.Xaml.2.8.appx");

                        if (!Directory.Exists(xamlDirectory))
                        {
                            Directory.CreateDirectory(xamlDirectory);
                            Console.WriteLine($"Directory created: {xamlDirectory}");
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(nugetUri, xamlZipPath);
                        }

                        Directory.CreateDirectory(xamlDirectory);
                        System.IO.Compression.ZipFile.ExtractToDirectory(xamlZipPath, xamlDirectory);

                        // Run the Add-AppxPackage command through PowerShell
                        Console.WriteLine("Installing Xaml");
                        ProcessStartInfo psi = new ProcessStartInfo("powershell.exe");
                        psi.Arguments = $"-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -Command \"Add-AppxPackage -Path '{appxPath}'\"";
                        psi.CreateNoWindow = true;
                        Process.Start(psi).WaitForExit();

                        using (WebClient webClient = new WebClient())
                        {
                            Console.WriteLine("Downloading Winget");
                            webClient.DownloadFile(wingetDownloadUrl, wingetMsixBundlePath);

                            // Install Winget
                            Console.WriteLine("Installing Winget");
                            psi = new ProcessStartInfo("powershell.exe");
                            psi.Arguments = $"-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -Command \"Add-AppxPackage -Path '{wingetMsixBundlePath}'\"";
                            psi.CreateNoWindow = true;
                            Process.Start(psi).WaitForExit();
                        }
                        if (IsWinGetInstalled())
                        {
                            System.IO.File.Delete(xamlZipPath);
                            Directory.Delete(xamlDirectory, true);
                            System.IO.File.Delete(wingetMsixBundlePath);
                            return "Install Verified";
                        }

                        if (attempt < maxAttempts)
                        {
                            // If there are more attempts remaining, wait for some time before the next attempt
                            Console.WriteLine($"Waiting for 10 seconds before the next attempt (Attempt {attempt + 1})...");
                            Thread.Sleep(10000); // 10 seconds
                        }
                        if (attempt == 1)
                        {
                            Console.WriteLine("Stoping all Winget services");
                            psi = new ProcessStartInfo("powershell.exe");
                            psi.Arguments = "taskkill /f /im winget.exe /im WindowsPackageManagerServer.exe";
                            psi.CreateNoWindow = true;
                            Process.Start(psi).WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
                Console.WriteLine("Installation failed after multiple attempts");
                Console.WriteLine("The Microsoft Store will now open plese update App Installer");
                Console.WriteLine("Try agian after App Installer is updated");
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.FileName = @"ms-windows-store://pdp/?ProductId=9NBLGGH4NNS1";
                p.StartInfo = startInfo;
                p.Start();
                System.IO.File.Delete(xamlZipPath);
                Directory.Delete(xamlDirectory, true);
                System.IO.File.Delete(wingetMsixBundlePath);
                return "Installation failed";
            }

            async Task<string> RunCommandAsync(string command)
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "winget.exe",
                        Arguments = command,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    }
                };

                process.Start();

                // Asynchronously read the standard output stream
                using (StreamReader reader = process.StandardOutput)
                {
                    StringBuilder output = new StringBuilder();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();

                        // Clean the line
                        string cleanedLine = CleanOutput(line);

                        // Output the current line in real-time only if it's not empty
                        if (!string.IsNullOrWhiteSpace(cleanedLine))
                        {
                            Console.WriteLine("WinGet Output: " + cleanedLine);
                        }

                        output.AppendLine(cleanedLine);
                    }

                    process.WaitForExit();

                    return output.ToString();
                }
            }

            string CleanOutput(string rawOutput)
            {
                // Define regular expressions to remove unwanted symbols and whitespace
                string[] patternsToRemove = { @"[\\|/-]", @"â–" };

                // Replace unwanted symbols
                foreach (var pattern in patternsToRemove)
                {
                    rawOutput = Regex.Replace(rawOutput, pattern, "");
                }

                // Remove leading and trailing whitespaces
                rawOutput = rawOutput.Trim();

                // Remove consecutive newline characters, keeping at most two consecutive newlines
                rawOutput = Regex.Replace(rawOutput, @"(\r\n){3,}", "\r\n\r\n");

                return rawOutput;
            }

            async Task<string> RunPowerShellAsync(string command)
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = command,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    }
                };

                process.Start();

                // Asynchronously read the standard output stream
                using (StreamReader reader = process.StandardOutput)
                {
                    StringBuilder output = new StringBuilder();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();

                        // Clean the line
                        string cleanedLine = CleanOutput(line);

                        // Output the current line in real-time only if it's not empty
                        if (!string.IsNullOrWhiteSpace(cleanedLine))
                        {
                            Console.WriteLine("Powershell Output: " + line);
                        }

                        output.AppendLine(cleanedLine);
                    }

                    process.WaitForExit();

                    return output.ToString();
                }
            }

            //End Of Winget Code
            static void CopyEmbeddedResource(string resourceName, string destinationPath)
            {
                using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        using (FileStream fileStream = System.IO.File.Create(destinationPath))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Resource not found: " + resourceName);
                    }
                }
            }

            //Logs logic
            string pcname = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName", "ComputerName", "").ToString();
            string releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "").ToString();

            // Determine whether the directory exists.
            if (Directory.Exists(logpath))
            {
                Console.WriteLine("That path exists already.");
            }
            else
            {
                Directory.CreateDirectory(logpath);
            }
            // Redirect standard output to the TextBox control and test.txt
            StringWriter stringWriter = new StringWriter();
            TextWriter originalConsoleOut = Console.Out; // Save the original console output

            // Redirect output to TextBox and file simultaneously
            Console.SetOut(new TextBoxWriter(textBox1, stringWriter, $"C:/Temp/DebloatAndFloat/Logs/{pcname} {DateTime.Now.ToString("yyyy-MM-dd hh.mm tt")} Output.txt"));

            //Load PC Data to Logs
            Console.WriteLine($"Windows {OSVersion} {releaseId} ");
            Console.WriteLine($"PC Name: {pcname} ");
            Console.WriteLine("Hello, Thanks for using Debloat And Float!");

            // Determine whether the file exists.
            if (System.IO.File.Exists("Background.gif"))
            {
                Image myimage = new Bitmap(@"Background.gif");
                this.BackgroundImage = new Bitmap(myimage);
                this.BackgroundImageLayout = ImageLayout.Stretch;
                buffer = new Bitmap(this.Width, this.Height);
                ImageAnimator.Animate(myimage, OnFrameChanged);

                void OnFrameChanged(object sender, EventArgs e)
                {
                    // Force the animation to advance to the next frame
                    ImageAnimator.UpdateFrames(myimage);

                    // Set the current frame as the background in the buffer
                    using (Graphics g = Graphics.FromImage(buffer))
                    {
                        g.DrawImage(myimage, new Rectangle(0, 0, buffer.Width, buffer.Height));
                    }

                    // Copy the buffer to the screen
                    using (Graphics g = this.CreateGraphics())
                    {
                        g.DrawImage(buffer, new Rectangle(0, 0, this.Width, this.Height));
                    }

                    // Increment the frame index
                    frameIndex++;

                    // If you want to loop the animation, reset the frame index when it reaches the end
                    if (frameIndex >= myimage.GetFrameCount(FrameDimension.Time))
                    {
                        frameIndex = 0;
                    }
                }
            }

            // Determine whether the file exists.
            else if (System.IO.File.Exists("Background.jpg"))
            {
                Image myimage = new Bitmap(@"Background.jpg");
                this.BackgroundImage = myimage;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Determine whether the file exists.
            else if (System.IO.File.Exists("Background.png"))
            {
                Image myimage = new Bitmap(@"Background.png");
                this.BackgroundImage = myimage;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            LoadSettings();
            ApplySettings();
        }
    }

    public class TextBoxWriter : TextWriter
    {
        private TextBox _textBox;
        private StringWriter _stringWriter;
        private StreamWriter _fileWriter;

        public TextBoxWriter(TextBox textBox, StringWriter stringWriter, string filePath)
        {
            _textBox = textBox;
            _stringWriter = stringWriter;
            _fileWriter = new StreamWriter(filePath);
        }

        public override void Write(char value)
        {
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() => _textBox.AppendText(value.ToString())));
            }
            else
            {
                _textBox.AppendText(value.ToString());
            }

            _stringWriter.Write(value);
            _fileWriter.Write(value);
            _fileWriter.Flush(); // Ensure content is flushed to the file
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileWriter.Close(); // Close the file writer
                _fileWriter.Dispose();
            }
            base.Dispose(disposing);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}