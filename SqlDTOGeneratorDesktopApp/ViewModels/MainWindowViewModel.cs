using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
using SqlDTOGeneratorDesktopApp.Services;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using SqlDTOGeneratorDesktopApp.Models;
using System.ComponentModel;
using SqlDTOGeneratorDesktopApp.DataStructure;
using System.Windows.Input;
using System.Reactive.Concurrency;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization.Metadata;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Newtonsoft.Json.Linq;
using Tmds.DBus.Protocol;
using System.Data;
using System.Linq;
using Avalonia.Threading;

namespace SqlDTOGeneratorDesktopApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageBox _messageBox;
        private readonly IDatabaseIntegration _dbIntegration;
        private readonly IFileService _fileService;
        private bool _isBusy;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="messageBox">Service for displaying messages to the user.</param>
        /// <param name="dbIntegration">Service for handling database operations.</param>
        /// <param name="fileService">Service for handling file operations.</param>
        public MainWindowViewModel(IMessageBox messageBox, IDatabaseIntegration dbIntegration, IFileService fileService)
        {
            _messageBox = messageBox;
            _dbIntegration = dbIntegration;
            _fileService = fileService;

            GenerateDtoCommand = new AsyncRelayCommand(ExecuteGenerateDtoAsync, CanExecuteGenerateDto);
            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);
            RetrieveDataCommand = new AsyncRelayCommand(RetrieveDataAsync);
            ExitCommand = new RelayCommand(OnExit);
            OnLoadedCommand = new RelayCommand(OnLoaded);
            OpenFileDialogCommand = new AsyncRelayCommand(ExecuteOpenFileDialogAsync);

            ListOfTable = new ObservableCollectionRange<TablesDatabase>();
            OptionsDatabase = new OptionDatabase();

            // Event handlers
            OptionsDatabase.PropertyChanged += OnOptionsDatabasePropertyChanged;
            ListOfTable.CollectionChanged += ListOfTable_CollectionChanged;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to generate DTOs asynchronously.
        /// </summary>
        public IAsyncRelayCommand GenerateDtoCommand { get; }

        /// <summary>
        /// Command to test the database connection asynchronously.
        /// </summary>
        public IAsyncRelayCommand TestConnectionCommand { get; }

        /// <summary>
        /// Command to retrieve data from the database asynchronously.
        /// </summary>
        public IAsyncRelayCommand RetrieveDataCommand { get; }

        /// <summary>
        /// Command to exit the application.
        /// </summary>
        public IRelayCommand ExitCommand { get; }

        /// <summary>
        /// Command executed when the application is loaded.
        /// </summary>
        public IRelayCommand OnLoadedCommand { get; }

        /// <summary>
        /// Command to open a file dialog asynchronously.
        /// </summary>
        public IAsyncRelayCommand OpenFileDialogCommand { get; }

        #endregion

        #region Command Methods

        /// <summary>
        /// Determines whether the <see cref="GenerateDtoCommand"/> can execute.
        /// </summary>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        private bool CanExecuteGenerateDto()
        {
            return !string.IsNullOrWhiteSpace(OptionsDatabase.Instance) &&
                   !string.IsNullOrWhiteSpace(OptionsDatabase.Database) &&
                   ListOfTable?.Count > 0;
        }

        /// <summary>
        /// Executes the logic for generating DTOs asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ExecuteGenerateDtoAsync()
        {
            IsBusy = true;
            try
            {
                using var connection = new SqlConnection(GenerateConnectionString());
                await connection.OpenAsync();

                // Clear existing DTO files
                Directory.GetFiles(OptionsDatabase.FolderDto)
                         .ToList()
                         .ForEach(File.Delete);

                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Run(async () =>
                    {
                        int tableCount = 0;
                        int totalCount = ListOfTable.Where(x => x.IsSelected).ToList().Count;
                        foreach (var table in ListOfTable.Where(x => x.IsSelected).ToList())
                        {
                            try
                            {
                                // Retrieve table columns
                                var columns = (await connection.GetSchemaAsync("Columns", new[] { null, null, table.Name }))
                                    .AsEnumerable()
                                    .Select(row => new
                                    {
                                        ColumnName = row["COLUMN_NAME"].ToString(),
                                        DataType = GetCSharpDataType(row["DATA_TYPE"].ToString())
                                    })
                                    .ToList();

                                string className = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(table.Name);
                                string filePath = Path.Combine(OptionsDatabase.FolderDto, $"{className}.cs");

                                // Create or overwrite the DTO file
                                using var sw = new StreamWriter(filePath);
                                await sw.WriteLineAsync("using System;");
                                await sw.WriteLineAsync();
                                await sw.WriteLineAsync($"public class {className}");
                                await sw.WriteLineAsync("{");

                                foreach (var column in columns)
                                {
                                    string propertyName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(column.ColumnName.ToLower());
                                    await sw.WriteLineAsync($"    public {column.DataType} {propertyName} {{ get; set; }}");
                                }

                                await sw.WriteLineAsync("}");

                                tableCount += 1;
                                table.IsElaborated = true;
                                table.Message = "Elaborated";
                                
                            }
                            catch (Exception ex)
                            {
                                table.Message = ex.Message;
                            }
                            finally
                            {
                                IsLoadingMessage = $"Elaborated {tableCount} of {totalCount} total DTO"; 
                            }
                        }
                    });
                });
            }
            finally
            {
                IsBusy = false;
                IsLoadingMessage = string.Empty;
            }
        }

        /// <summary>
        /// Tests the database connection asynchronously and shows the result in a message box.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task TestConnectionAsync()
        {
            IsBusy = true;
            try
            {
                NormalizedConnectionString();
                await TryEstablishedConnectionAsync();
                await _messageBox.ShowStandard("Connection Established", "Connection Established", ButtonEnum.Ok, Icon.Database);
            }
            catch (Exception ex)
            {
                await _messageBox.ShowStandard("Connection Error", ex.Message, ButtonEnum.Ok, Icon.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Retrieves data from the database asynchronously and updates the <see cref="ListOfTable"/>.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task RetrieveDataAsync()
        {
            IsBusy = true;
            try
            {
                var connectionString = GenerateConnectionString();
                if (string.IsNullOrEmpty(connectionString)) return;

                ListOfTable.Clear();

                // Get all tables from the database
                var tables = await _dbIntegration.GetAllDataFromDatabaseAsync(connectionString);
                ListOfTable.AddRange(tables);
            }
            catch (Exception ex)
            {
                await _messageBox.ShowStandard("Retrieve Data Error", ex.Message, ButtonEnum.Ok, Icon.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Opens a folder dialog to select a folder for saving DTO files.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ExecuteOpenFileDialogAsync()
        {
            var selectedFolders = await _fileService.OpenFolderAsync();

            if (selectedFolders.Count > 0)
            {
                OptionsDatabase.FolderDto = selectedFolders[0].Path.LocalPath;
            }
            else
            {
                OptionsDatabase.FolderDto = string.Empty;
            }
        }

        /// <summary>
        /// Selects or deselects all tables in the list.
        /// </summary>
        /// <param name="parameters">Boolean indicating whether to select or deselect all tables.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task SelectAllAsync(object parameters)
        {
            if (ListOfTable == null || ListOfTable.Count == 0) return;

            var selectAll = bool.Parse(parameters.ToString());
            await Task.Run(() => ListOfTable.ToList().ForEach(x => x.IsSelected = selectAll));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generates a connection string for SQL Server using the provided instance, database, and other configuration settings.
        /// </summary>
        /// <returns>A connection string formatted for use with <see cref="SqlConnection"/>.</returns>
        private string GenerateConnectionString()
        {
            NormalizedConnectionString();
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = OptionsDatabase.Instance,
                InitialCatalog = OptionsDatabase.Database,
                IntegratedSecurity = OptionsDatabase.IntegratedSecurity,
                ConnectTimeout = OptionsDatabase.ConnectionTimeout
            };

            builder.Add("Trusted_Connection", OptionsDatabase.TrustedConnection);

            if (!OptionsDatabase.IntegratedSecurity)
            {
                if (!string.IsNullOrEmpty(OptionsDatabase.User))
                    builder.UserID = OptionsDatabase.User;
                if (!string.IsNullOrEmpty(OptionsDatabase.Password))
                    builder.Password = OptionsDatabase.Password;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Normalizes the connection string settings.
        /// </summary>
        private void NormalizedConnectionString()
        {
            string pattern = @"(\\)+";
            OptionsDatabase.Instance = Regex.Replace(OptionsDatabase.Instance, pattern, @"\", RegexOptions.None, TimeSpan.FromSeconds(1));
            OptionsDatabase.Instance = OptionsDatabase.Instance?.Trim();
            OptionsDatabase.Database = OptionsDatabase.Database?.Trim();
            OptionsDatabase.User = OptionsDatabase.User?.Trim();
            OptionsDatabase.Password = OptionsDatabase.Password?.Trim();
        }

        /// <summary>
        /// Attempts to establish a connection to the database and throws an exception if unsuccessful.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task TryEstablishedConnectionAsync()
        {
            await using var connection = new SqlConnection(GenerateConnectionString());
            await connection.OpenAsync();
        }

        /// <summary>
        /// Converts a SQL data type to its corresponding C# data type.
        /// </summary>
        /// <param name="sqlDataType">The SQL data type to convert.</param>
        /// <returns>A string representing the corresponding C# data type.</returns>
        private string GetCSharpDataType(string sqlDataType)
        {
            return sqlDataType.ToLower() switch
            {
                "bigint" => "long",
                "binary" => "byte[]",
                "bit" => "bool",
                "char" => "string",
                "date" => "DateTime",
                "datetime" => "DateTime",
                "decimal" => "decimal",
                "float" => "double",
                "int" => "int",
                "money" => "decimal",
                "nchar" => "string",
                "ntext" => "string",
                "numeric" => "decimal",
                "nvarchar" => "string",
                "real" => "float",
                "smalldatetime" => "DateTime",
                "smallint" => "short",
                "smallmoney" => "decimal",
                "text" => "string",
                "time" => "TimeSpan",
                "timestamp" => "byte[]",
                "tinyint" => "byte",
                "uniqueidentifier" => "Guid",
                "varbinary" => "byte[]",
                "varchar" => "string",
                _ => "string",
            };
        }

        /// <summary>
        /// Handles the PropertyChanged event for the OptionsDatabase object.
        /// </summary>
        private void OnOptionsDatabasePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GenerateDtoCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Handles the CollectionChanged event for the ListOfTable collection.
        /// </summary>
        private void ListOfTable_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            GenerateDtoCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Called when the application is loaded. Retrieves data if a configuration file exists.
        /// </summary>
        private void OnLoaded()
        {
            var temp = Path.Combine(Path.GetTempPath(), "optionSerializedDatabase.json");
            if (File.Exists(temp))
            {
                string jsonString = File.ReadAllText(temp);
                var deserializedOptions = JsonSerializer.Deserialize<OptionDatabase>(jsonString);
                if (deserializedOptions != null)
                    OptionsDatabase = deserializedOptions;
            }
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        private void OnExit()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(OptionsDatabase, options);
            string myTempFile = Path.Combine(Path.GetTempPath(), "optionSerializedDatabase.json");
            File.WriteAllText(myTempFile, jsonString);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of tables retrieved from the database.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GenerateDtoCommand))]
        [NotifyCanExecuteChangedFor(nameof(RetrieveDataCommand))]
        private ObservableCollectionRange<TablesDatabase> _listOfTable;

        /// <summary>
        /// Gets or sets the database options.
        /// </summary>
        [ObservableProperty]
        private OptionDatabase _optionsDatabase;

        [ObservableProperty]
        private string _isLoadingMessage;

        #endregion
    }


}


