using SqlDTOGenerator.ClassBase;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SqlDTOGenerator.Database
{
    public sealed class DatabaseIntegration(JsonStructure json)
    {
        private readonly JsonStructure json = json;
        
        /// <summary>
        /// Retrieves all the tables and their columns from the database, generating DTO classes for each table.
        /// The DTO classes are saved as .cs files in the output directory specified in the JSON configuration.
        /// </summary>
        public async Task GetAllDatabaseInformation()
        {
            Debug.Assert(json != null);
            Debug.Assert(json.ConnectionString != string.Empty, "Connection String Empty");
            Debug.Assert(json.OutputDirectory != string.Empty, "Output Directory Empty");

            if (!Directory.Exists(json.OutputDirectory))
            {
                Directory.CreateDirectory(json.OutputDirectory);
            }
            else
            {
                Directory.GetFiles(json.OutputDirectory).ToList().ForEach(file => File.Delete(file));
            }

            using var connection = new SqlConnection(json.ConnectionString);
            await connection.OpenAsync();

            var tables = (await connection.GetSchemaAsync("Tables"))
                           .AsEnumerable()
                           .Where(row => row["TABLE_TYPE"].ToString() == "BASE TABLE")
                           .Select(row => row["TABLE_NAME"].ToString())
                           .ToList();

            foreach (var table in tables)
            {
                
                var columns = (await connection.GetSchemaAsync("Columns", new[] { null, null, table }))
                                .AsEnumerable()
                                .Select(row => new
                                {
                                    ColumnName = row["COLUMN_NAME"].ToString(),
                                    DataType = GetCSharpDataType(row["DATA_TYPE"].ToString())
                                })
                                .ToList();

                string className = table;
                string filePath = Path.Combine(json.OutputDirectory, $"{className}.cs");

                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    await sw.WriteLineAsync("using System;");
                    await sw.WriteLineAsync();
                    await sw.WriteLineAsync($"public class {className}");
                    await sw.WriteLineAsync("{");

                    foreach (var column in columns)
                    {
                        await sw.WriteLineAsync($"    public {column.DataType} {column.ColumnName} {{ get; set; }}");
                    }

                    await sw.WriteLineAsync("}");
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Generated DTO for table {table} at {filePath}");
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Maps SQL data types to their corresponding C# data types.
        /// </summary>
        /// <param name="sqlDataType">The SQL data type as a string.</param>
        /// <returns>The corresponding C# data type as a string.</returns>
        static string GetCSharpDataType(string sqlDataType)
        {
            return sqlDataType switch
            {
                "int" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "decimal" => "decimal",
                "numeric" => "decimal",
                "money" => "decimal",
                "smallmoney" => "decimal",
                "float" => "double",
                "real" => "float",
                "datetime" => "DateTime",
                "smalldatetime" => "DateTime",
                "char" => "string",
                "varchar" => "string",
                "text" => "string",
                "nchar" => "string",
                "nvarchar" => "string",
                "ntext" => "string",
                "binary" => "byte[]",
                "varbinary" => "byte[]",
                "image" => "byte[]",
                _ => "string", // Default case
            };
        }

    }
}