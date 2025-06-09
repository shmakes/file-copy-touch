using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FileCopyTouch
{
    public partial class MainWindow : Window
    {
        private Configuration _config;
        private DispatcherTimer _debounceTimer;
        private ObservableCollection<string> _fileResults;
        private string _currentSearchText = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Create default config if it doesn't exist
            Configuration.CreateDefaultConfigIfNotExists();
            
            // Load configuration
            _config = Configuration.Load();
            
            // Initialize collections
            _fileResults = new ObservableCollection<string>();
            FileListBox.ItemsSource = _fileResults;
            
            // Initialize debounce timer
            _debounceTimer = new DispatcherTimer();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(_config.DebounceMilliseconds);
            _debounceTimer.Tick += DebounceTimer_Tick;
            
            // Set up placeholder text functionality
            SetupPlaceholderText();
            
            // Update configuration info display
            UpdateConfigInfo();
            
            // Set the application title
            UpdateApplicationTitle();
            
            // Validate directories
            ValidateDirectories();
        }

        private void UpdateConfigInfo()
        {
            ConfigInfoTextBlock.Text = $"Min Length: {_config.MinimumInputLength} | " +
                                      $"Debounce: {_config.DebounceMilliseconds}ms | " +
                                      $"Source: {_config.SourceDirectory} | " +
                                      $"Target: {_config.TargetDirectory}";
        }

        private void UpdateApplicationTitle()
        {
            TitleTextBlock.Text = _config.ApplicationTitle;
            this.Title = $"{_config.ApplicationTitle} - Touch Interface";
        }

        private void SetupPlaceholderText()
        {
            const string placeholderText = "Enter search text or scan barcode...";
            
            // Set initial placeholder
            InputTextBox.Text = placeholderText;
            InputTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            
            InputTextBox.GotFocus += (sender, e) =>
            {
                if (InputTextBox.Text == placeholderText)
                {
                    InputTextBox.Text = "";
                    InputTextBox.Foreground = System.Windows.Media.Brushes.Black;
                }
            };
            
            InputTextBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(InputTextBox.Text))
                {
                    InputTextBox.Text = placeholderText;
                    InputTextBox.Foreground = System.Windows.Media.Brushes.Gray;
                }
            };
        }

        private void ValidateDirectories()
        {
            bool sourceExists = Directory.Exists(_config.SourceDirectory);
            bool targetExists = Directory.Exists(_config.TargetDirectory);
            
            if (!sourceExists || !targetExists)
            {
                string message = "Directory validation:\n";
                if (!sourceExists) message += $"• Source directory does not exist: {_config.SourceDirectory}\n";
                if (!targetExists) message += $"• Target directory does not exist: {_config.TargetDirectory}\n";
                message += "\nPlease check your config.json file.";
                
                MessageBox.Show(message, "Directory Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            const string placeholderText = "Enter search text or scan barcode...";
            
            // Don't process if placeholder text is showing
            if (InputTextBox.Text == placeholderText)
            {
                _currentSearchText = "";
                _fileResults.Clear();
                StatusTextBlock.Text = "Ready to search files...";
                return;
            }
            
            _currentSearchText = InputTextBox.Text;
            
            // Stop the current timer
            _debounceTimer.Stop();
            
            // Clear results if text is too short
            if (_currentSearchText.Length < _config.MinimumInputLength)
            {
                _fileResults.Clear();
                StatusTextBlock.Text = $"Enter at least {_config.MinimumInputLength} characters to search...";
                return;
            }
            
            // Start the debounce timer
            _debounceTimer.Start();
            StatusTextBlock.Text = "Typing...";
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            SearchFiles(_currentSearchText);
        }

        private void SearchFiles(string searchText)
        {
            try
            {
                StatusTextBlock.Text = "Searching files...";
                _fileResults.Clear();
                
                if (!Directory.Exists(_config.SourceDirectory))
                {
                    StatusTextBlock.Text = "Source directory not found!";
                    return;
                }
                
                var files = Directory.GetFiles(_config.SourceDirectory, "*", SearchOption.TopDirectoryOnly);
                var matchingFiles = files
                    .Select(Path.GetFileName)
                    .Where(fileName => !string.IsNullOrEmpty(fileName) && fileName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(fileName => fileName)
                    .ToList();
                
                foreach (var file in matchingFiles)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        _fileResults.Add(file);
                    }
                }
                
                StatusTextBlock.Text = $"Found {matchingFiles.Count} matching file(s)";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error searching files: {ex.Message}";
                MessageBox.Show($"Error searching files: {ex.Message}", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListBox.SelectedItem is string selectedFileName)
            {
                CopySelectedFile(selectedFileName);
            }
        }

        private async void CopySelectedFile(string fileName)
        {
            try
            {
                StatusTextBlock.Text = "Preparing to copy file...";
                
                string sourceFilePath = Path.Combine(_config.SourceDirectory, fileName);
                string targetFilePath = Path.Combine(_config.TargetDirectory, fileName);
                
                if (!File.Exists(sourceFilePath))
                {
                    MessageBox.Show($"Source file not found: {sourceFilePath}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Confirm the operation
                var result = MessageBox.Show(
                    $"This will:\n\n" +
                    $"1. Clear all files in the target directory:\n   {_config.TargetDirectory}\n\n" +
                    $"2. Copy the selected file:\n   {fileName}\n\n" +
                    $"Do you want to continue?",
                    "Confirm File Copy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                
                if (result != MessageBoxResult.Yes)
                {
                    StatusTextBlock.Text = "Copy operation cancelled";
                    FileListBox.SelectedItem = null;
                    return;
                }
                
                StatusTextBlock.Text = "Clearing target directory...";
                
                // Clear target directory
                if (Directory.Exists(_config.TargetDirectory))
                {
                    var targetFiles = Directory.GetFiles(_config.TargetDirectory);
                    foreach (var file in targetFiles)
                    {
                        File.Delete(file);
                    }
                }
                else
                {
                    Directory.CreateDirectory(_config.TargetDirectory);
                }
                
                StatusTextBlock.Text = "Copying file...";
                
                // Copy the file
                await Task.Run(() => File.Copy(sourceFilePath, targetFilePath, true));
                
                StatusTextBlock.Text = "Copy completed successfully!";
                
                // Show completion confirmation
                var confirmResult = MessageBox.Show(
                    $"File copied successfully!\n\n" +
                    $"From: {sourceFilePath}\n" +
                    $"To: {targetFilePath}\n\n" +
                    $"Click OK to return to the main screen.",
                    "Copy Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                // Reset the interface
                ClearResults();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error copying file: {ex.Message}";
                MessageBox.Show($"Error copying file: {ex.Message}", "Copy Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                FileListBox.SelectedItem = null;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
        }

        private void ClearResults()
        {
            const string placeholderText = "Enter search text or scan barcode...";
            InputTextBox.Text = placeholderText;
            InputTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            _currentSearchText = "";
            _fileResults.Clear();
            _debounceTimer.Stop();
            StatusTextBlock.Text = "Ready to search files...";
            FileListBox.SelectedItem = null;
        }
    }
} 