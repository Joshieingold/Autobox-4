using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Toolbox_Class_Library.CtrUpdate;
using Toolbox_Class_Library.Properties;

namespace Rogers_Toolbox_UI
{
    public partial class SettingsWindow : Window
    {
        private MainWindow mainWindow;
        private List<ContractorCategory> contractorCategories = new();
        private List<string> techDevices = new();
        private List<string> techIds = new();

        public SettingsWindow(MainWindow main)
        {
            InitializeComponent();
            this.DataContext = Settings.Default;
            mainWindow = main;
            LoadContractorData();
            InitializeThemeComboBox();
            InitializeWmsFailSettingComboBox();
            LoadTechIds();
        }
        private void LoadTechIds()
        {
            string raw = Settings.Default.TechIds;
            techIds = string.IsNullOrWhiteSpace(raw)
                ? new List<string>()
                : raw.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();

        }
        private void SaveTechIds()
        {
            Settings.Default.TechIds = string.Join(", ", techIds);
            Settings.Default.Save();
        }

        private void SaveTechDevices()
        {
            Settings.Default.TechDevices = string.Join(", ", techDevices);
            Settings.Default.Save();
        }

        private void InitializeThemeComboBox()
        {
            // Set the ComboBox to the current theme
            string currentTheme = Settings.Default.Theme;
            ThemeComboBox.SelectedItem = ThemeComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == currentTheme);
        }

        private void InitializeWmsFailSettingComboBox()
        {
            // Set the ComboBox to the current theme
            string currentAutomation = Settings.Default.WmsFailAutomation;
            WmsFailSettingComboBox.SelectedItem = WmsFailSettingComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == currentAutomation);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save all the settings to persistent storage
            Settings.Default.Save();
            mainWindow.ApplyTheme($"Themes/{Settings.Default.Theme}Theme.xaml");
            mainWindow.UpdateMessage($"Your settings have been successfully updated {Settings.Default.Username}!");
            this.Close();
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedTheme = selectedItem.Content.ToString();
                Settings.Default.Theme = selectedTheme; // Update the setting
                Settings.Default.Save(); // Save the setting

                // Apply the new theme immediately
                mainWindow.ApplyTheme($"Themes/{selectedTheme}Theme.xaml");
            }
        }
        private void WmsFailAutomationSetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WmsFailSettingComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedFailAutomation = selectedItem.Content.ToString();
                Settings.Default.WmsFailAutomation = selectedFailAutomation;
                Settings.Default.Save();
            }
        }

        public List<ContractorCategory> LoadContractorData()
        {
            string jsonData = Settings.Default.ContractorData;

            if (string.IsNullOrWhiteSpace(jsonData))
                return new List<ContractorCategory>();

            contractorCategories = JsonSerializer.Deserialize<List<ContractorCategory>>(jsonData) ?? new List<ContractorCategory>();

            return contractorCategories;
        }

        public void SaveContractorData(List<ContractorCategory> categories)
        {
            string jsonData = JsonSerializer.Serialize(categories);
            Settings.Default.ContractorData = jsonData;
            Settings.Default.Save();
        }





    }

    public class ContractorCategory
    {
        public string Name { get; set; }
        public List<string> Devices { get; set; } = new();
        public List<string> CtrIDs { get; set; } = new();
    }
}