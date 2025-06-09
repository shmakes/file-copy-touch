using System;
using System.IO;
using System.Text.Json;

namespace FileCopyTouch
{
    public class Configuration
    {
        public int MinimumInputLength { get; set; } = 5;
        public int DebounceMilliseconds { get; set; } = 500;
        public string SourceDirectory { get; set; } = "";
        public string TargetDirectory { get; set; } = "";

        private static readonly string ConfigFileName = "config.json";

        public static Configuration Load()
        {
            try
            {
                if (File.Exists(ConfigFileName))
                {
                    string json = File.ReadAllText(ConfigFileName);
                    var config = JsonSerializer.Deserialize<Configuration>(json) ?? new Configuration();
                    
                    // Validate and constrain values
                    config.MinimumInputLength = Math.Max(5, Math.Min(20, config.MinimumInputLength));
                    config.DebounceMilliseconds = Math.Max(100, Math.Min(3000, config.DebounceMilliseconds));
                    
                    return config;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading configuration: {ex.Message}\n\nUsing default values.", "Configuration Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }

            return new Configuration();
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFileName, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving configuration: {ex.Message}", "Configuration Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public static void CreateDefaultConfigIfNotExists()
        {
            if (!File.Exists(ConfigFileName))
            {
                var defaultConfig = new Configuration
                {
                    MinimumInputLength = 5,
                    DebounceMilliseconds = 500,
                    SourceDirectory = @"C:\SourceFiles",
                    TargetDirectory = @"C:\TargetFiles"
                };
                defaultConfig.Save();
            }
        }
    }
} 