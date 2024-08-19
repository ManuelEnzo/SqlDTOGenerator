SqlDTOGenerator
===============

**SqlDTOGenerator** is a .NET application designed to generate Data Transfer Objects (DTOs) for each table in a SQL Server database. This tool reads the schema information from the database and creates corresponding C# class files for each table, which can then be used in your application to facilitate data operations.

Features
--------

*   **Database Integration**: Connects to a SQL Server database using a connection string.
    
*   **DTO Generation**: Automatically generates C# classes for each table in the database.
    
*   **Output Management**: Saves generated DTO classes as .cs files in a specified output directory.
    
*   **Console Feedback**: Provides colored console output indicating the success of DTO generation.
    

Prerequisites
-------------

*   .NET Core or .NET Framework
    
*   SQL Server
    
*   [Visual Studio](https://visualstudio.microsoft.com/) or any other C# IDE
    

Getting Started
---------------

### Installation

1.   ```bash
     git clone https://github.com/ManuelEnzo/SqlDTOGenerator.git
     cd SqlDTOGenerator
     ```
    
2.  Ensure you have the required dependencies by restoring the NuGet packages.
       ```bash
       dotnet restore
       ```
    

### Configuration

1.  Create or update the appsettings.json file in the root directory of the project with the following structure:
       ```json
       {
           "ConnectionString": "Data Source=localhost;Initial Catalog=NameOfDB;Integrated Security=True;",
           "OutputDirectory": "path/to/output/directory"
       }
    ```
       
    *   ConnectionString: The connection string to your SQL Server database.
        
    *   OutputDirectory: The directory where the generated DTO files will be saved.
        

### Usage

1.  Execute the application to generate DTO classes from the database schema.
    ```bash
    dotnet run
    ```
    
3.  After execution, the generated DTO files will be located in the directory specified by OutputDirectory. Each table in the database will have a corresponding .cs file representing its schema.
    

### Code Overview

*   **DatabaseIntegration Class**: Handles the connection to the database, retrieves schema information, and generates DTO files.
    
*   **GetAllDatabaseInformation()**: Retrieves table and column details, generates DTOs, and saves them to files.
    
*   **GetCSharpDataType(string sqlDataType)**: Maps SQL data types to C# data types.
    

SqlDTOGenerator Desktop Application (WPF)
=========================================

**SqlDTOGenerator Desktop Application** is a WPF-based graphical user interface (GUI) that enhances the functionality of the core SqlDTOGenerator by providing a user-friendly interface for database interactions, DTO generation, and file management.

Features
--------

*   **Intuitive UI**: Provides a WPF-based interface for interacting with the SQL Server database, selecting tables, and generating DTOs.
    
*   **Real-Time Feedback**: Displays the status of database operations and DTO generation in real time.
    
*   **File Management**: Allows users to specify and select the output directory for generated DTO files directly from the GUI.
    
*   **Command Execution**: Supports asynchronous command execution to maintain UI responsiveness during long-running tasks.
    
*   **MVVM Architecture**: Utilizes the MVVM (Model-View-ViewModel) pattern for clean separation of concerns and easier testing.
    

Getting Started
---------------

### Installation

1.   ```bash
     git clone https://github.com/ManuelEnzo/SqlDTOGeneratorDesktopApp.git
     cd SqlDTOGeneratorDesktopApp
     ```
    
2.  Ensure you have the required dependencies by restoring the NuGet packages.
    ```bash
    dotnet restore
    ```
   
### Configuration

1.  The application allows users to configure database connection settings and output directory through the UI. These settings can be adjusted in the MainWindowViewModel and saved for future use.
    

### Usage

1.  Run the application using Visual Studio or your preferred C# IDE. Upon launch, you can connect to your SQL Server database, retrieve table data, and generate DTOs directly from the UI.
    
2.  **Perform Database Operations**
    
    *   **Test Connection**: Verify the connection to your SQL Server database.
        
    *   **Retrieve Data**: Fetch all tables from the database.
        
    *   **Generate DTOs**: Generate C# DTO classes for selected tables and save them to the specified output directory.
        
3.  The application automatically saves the last used configuration (like database instance, output directory) to a temporary file and loads them when the application starts.
    

### Code Overview

*   **MainWindowViewModel Class**: Core class that manages the application's state, handles user interactions, and coordinates data operations between the UI and backend services.
    
*   **GenerateDtoCommand**: Command responsible for generating DTOs asynchronously based on user input.
    
*   **TestConnectionCommand**: Command to test and validate the database connection.
    
*   **RetrieveDataCommand**: Command to retrieve tables and their schema from the connected SQL Server database.
    

### Contributing

1.  Click on the "Fork" button at the top-right corner of the repository page on GitHub.
    
2.  ```bash
    git checkout -b your-branch-name
    ```
3.  Edit the code or documentation as needed.
    
4.  ```bash
    git add .git commit -m "Description of changes"
    ```
    
5.  ```bash
    git push origin your-branch-name
    ```
    
6.  Go to the original repository and click on "New Pull Request". Select your branch and submit the pull request.
    

### License

This project is licensed under the [MIT License](LICENSE).

### Contact

For any questions or issues, please open an issue on GitHub or contact [manuelenzo2000@gmail.com](mailto:manuelenzo2000@gmail.com).

Thank you for using **SqlDTOGenerator** and the **SqlDTOGenerator Desktop Application**!
