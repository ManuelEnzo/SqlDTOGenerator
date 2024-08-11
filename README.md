# SqlDTOGenerator

**SqlDTOGenerator** is a .NET application designed to generate Data Transfer Objects (DTOs) for each table in a SQL Server database. This tool reads the schema information from the database and creates corresponding C# class files for each table, which can then be used in your application to facilitate data operations.

## Features

- **Database Integration**: Connects to a SQL Server database using a connection string.
- **DTO Generation**: Automatically generates C# classes for each table in the database.
- **Output Management**: Saves generated DTO classes as `.cs` files in a specified output directory.
- **Console Feedback**: Provides colored console output indicating the success of DTO generation.

## Prerequisites

- .NET Core or .NET Framework
- SQL Server
- [Visual Studio](https://visualstudio.microsoft.com/) or any other C# IDE

## Getting Started

### Installation

1. **Clone the Repository**

    ```bash
    git clone https://github.com/ManuelEnzo/SqlDTOGenerator.git
    cd SqlDTOGenerator
    ```

2. **Install Dependencies**

    Ensure you have the required dependencies by restoring the NuGet packages.

    ```bash
    dotnet restore
    ```

### Configuration

1. **Setup Configuration File**

    Create or update the `appsettings.json` file in the root directory of the project with the following structure:

    ```json
    {
      "ConnectionString": "Data Source=localhost;Initial Catalog=NameOfDB;Integrated Security=True;",
      "OutputDirectory": "path/to/output/directory"
    }
    ```

    - `ConnectionString`: The connection string to your SQL Server database.
    - `OutputDirectory`: The directory where the generated DTO files will be saved.

### Usage

1. **Run the Application**

    Execute the application to generate DTO classes from the database schema.

    ```bash
    dotnet run
    ```

2. **Check Output**

    After execution, the generated DTO files will be located in the directory specified by `OutputDirectory`. Each table in the database will have a corresponding `.cs` file representing its schema.

### Code Overview

    - **`DatabaseIntegration` Class**: Handles the connection to the database, retrieves schema information, and generates DTO files.
    - **`GetAllDatabaseInformation()`**: Retrieves table and column details, generates DTOs, and saves them to files.
    - **`GetCSharpDataType(string sqlDataType)`**: Maps SQL data types to C# data types.

### Contributing

1. **Fork the Repository**

    Click on the "Fork" button at the top-right corner of the repository page on GitHub.

2. **Create a Branch**

    ```bash
    git checkout -b your-branch-name
    ```

3. **Make Changes**

    Edit the code or documentation as needed.

4. **Commit Your Changes**

    ```bash
    git add .
    git commit -m "Description of changes"
    ```

5. **Push to Your Fork**

    ```bash
    git push origin your-branch-name
    ```

6. **Create a Pull Request**

    Go to the original repository and click on "New Pull Request". Select your branch and submit the pull request.

### License

This project is licensed under the [MIT License](LICENSE).

### Contact

For any questions or issues, please open an issue on GitHub or contact [manuelenzo2000@gmail.com](mailto:manuelenzo2000@gmail.com).

---

Thank you for using **SqlDTOGenerator**!
