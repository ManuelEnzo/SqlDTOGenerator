using Avalonia.Threading;
using SqlDTOGeneratorDesktopApp.DataStructure;
using SqlDTOGeneratorDesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace SqlDTOGeneratorDesktopApp.Services
{
    public interface IDatabaseIntegration
    {
        public Task<ObservableCollectionRange<TablesDatabase>> GetAllDataFromDatabaseAsync(string connectionString);
        public event Action<float> ProgressChanged;
    }

    public class DatabaseIntegration : IDatabaseIntegration
    {
        public async Task<ObservableCollectionRange<TablesDatabase>> GetAllDataFromDatabaseAsync(string connectionString)
        {
            // Create a builder for ImmutableList
            var listBuilder = new ObservableCollectionRange<TablesDatabase>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var tables = (await connection.GetSchemaAsync("Tables"))
                           .AsEnumerable()
                           .Where(row => row["TABLE_TYPE"].ToString() == "BASE TABLE")
                           .Select(row => row["TABLE_NAME"].ToString())
                           .ToList();

            int totalTables = tables.Count;
            int processedTables = 0;

            foreach (var table in tables)
            {
                var t = new TablesDatabase
                {
                    Name = table,
                    IsSelected = true,
                    IsElaborated = false,
                    Message = string.Empty
                };
                listBuilder.Add(t);

                // Incrementa il numero di tabelle elaborate
                processedTables++;

                float progress = totalTables > 0 ? (processedTables * 100f) / totalTables : 0;
                ProgressChanged?.Invoke(progress);
            }

            // Convert the builder to an immutable list
            return listBuilder;
        }

        public event Action<float> ProgressChanged;
    }
}
