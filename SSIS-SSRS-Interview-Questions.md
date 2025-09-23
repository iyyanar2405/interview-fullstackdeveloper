# SSIS & SSRS Interview Questions & Answers
## Comprehensive Guide for All Levels

### Table of Contents
1. [SSIS Fundamentals (Beginner)](#ssis-fundamentals-beginner)
2. [SSIS Intermediate Level](#ssis-intermediate-level)
3. [SSIS Expert Level](#ssis-expert-level)
4. [SSIS Architecture & Performance](#ssis-architecture--performance)
5. [SSRS Fundamentals (Beginner)](#ssrs-fundamentals-beginner)
6. [SSRS Intermediate Level](#ssrs-intermediate-level)
7. [SSRS Expert Level](#ssrs-expert-level)
8. [SSRS Architecture & Performance](#ssrs-architecture--performance)
9. [Real-World Scenarios](#real-world-scenarios)
10. [Best Practices & Troubleshooting](#best-practices--troubleshooting)

---

## SSIS Fundamentals (Beginner)

### 1. What is SSIS and what are its main components?

**Answer:**
SSIS (SQL Server Integration Services) is a component of Microsoft SQL Server used for data integration, transformation, and workflow applications. It's part of Microsoft's data platform and provides enterprise-level package management and security features.

**Main Components:**
- **Control Flow**: Defines the workflow of the package
- **Data Flow**: Handles data extraction, transformation, and loading
- **Event Handlers**: Handle events that occur during package execution
- **Package Explorer**: Shows package structure and components
- **Connection Managers**: Manage connections to data sources
- **Variables**: Store values that can be used throughout the package
- **Parameters**: Allow external values to be passed to packages

**Real-world Example:**
```sql
-- Example of a simple SSIS workflow
Control Flow:
1. Execute SQL Task (Truncate staging table)
2. Data Flow Task (Load data from CSV to staging)
3. Execute SQL Task (Merge staging to production)
4. Send Mail Task (Send completion notification)
```

### 2. What are the different types of transformations in SSIS?

**Answer:**
SSIS transformations are categorized into several types:

**Row Transformations (Process row by row):**
- **Derived Column**: Add new columns or modify existing ones
- **Data Conversion**: Convert data types
- **Copy Column**: Create copies of input columns
- **Character Map**: Apply string functions

**Rowset Transformations (Process multiple rows):**
- **Sort**: Sort data based on columns
- **Aggregate**: Perform grouping operations (SUM, COUNT, etc.)
- **Pivot/Unpivot**: Transform rows to columns and vice versa

**Split and Join Transformations:**
- **Conditional Split**: Route rows to different outputs based on conditions
- **Multicast**: Send copies of input to multiple outputs
- **Union All**: Combine multiple inputs into one output
- **Merge**: Combine two sorted datasets
- **Merge Join**: Join two sorted datasets (Inner, Left, Full Outer)
- **Lookup**: Look up values in a reference dataset

**Business Intelligence Transformations:**
- **Slowly Changing Dimension (SCD)**: Handle dimension changes in data warehouses
- **Fuzzy Lookup**: Find approximate matches
- **Fuzzy Grouping**: Group similar records

**Example Implementation:**
```sql
-- Derived Column Transformation Example
-- Input: FirstName, LastName
-- Output: FirstName, LastName, FullName, EmailAddress

FullName = [FirstName] + " " + [LastName]
EmailAddress = LOWER([FirstName]) + "." + LOWER([LastName]) + "@company.com"
```

### 3. What are Connection Managers and name the different types?

**Answer:**
Connection Managers define how SSIS connects to data sources and destinations. They encapsulate connection information and can be reused across multiple tasks.

**Types of Connection Managers:**

**Database Connections:**
- **OLE DB**: Most common for SQL Server and other databases
- **ODBC**: For ODBC-compliant data sources
- **ADO.NET**: For .NET data providers
- **Oracle**: Specifically for Oracle databases
- **MySQL**: For MySQL databases

**File Connections:**
- **File**: For single files
- **Multiple Files**: For multiple files with same structure
- **Flat File**: For delimited or fixed-width text files
- **Multiple Flat Files**: For multiple text files
- **Excel**: For Excel workbooks

**Other Connections:**
- **FTP**: For FTP file transfers
- **HTTP**: For web services and HTTP operations
- **SMTP**: For sending emails
- **WMI**: For Windows Management Instrumentation
- **Azure Storage**: For Azure blob storage

**Best Practice Example:**
```xml
<!-- Connection Manager Configuration -->
<ConnectionManager>
    <Name>SQL_Server_Production</Name>
    <ConnectionString>Data Source=PROD-SQL01;Initial Catalog=DataWarehouse;Integrated Security=True</ConnectionString>
    <RetainSameConnection>True</RetainSameConnection>
    <DelayValidation>True</DelayValidation>
</ConnectionManager>
```

### 4. Explain the difference between Control Flow and Data Flow in SSIS.

**Answer:**

**Control Flow:**
- **Purpose**: Orchestrates the execution order of tasks
- **Level**: Package level workflow
- **Components**: Tasks, containers, precedence constraints
- **Execution**: Sequential or parallel based on constraints
- **Data Handling**: Does not process row-by-row data

**Data Flow:**
- **Purpose**: Extracts, transforms, and loads data
- **Level**: Task level (inside Data Flow Task)
- **Components**: Sources, transformations, destinations
- **Execution**: Row-by-row or batch processing
- **Data Handling**: Processes actual data rows

**Practical Example:**
```
Control Flow Structure:
┌─────────────────────┐
│ 1. Prepare Database │ (Execute SQL Task)
└─────────┬───────────┘
          │ (On Success)
┌─────────▼───────────┐
│ 2. Load Customer    │ (Data Flow Task)
│    Data             │
└─────────┬───────────┘
          │ (On Success)
┌─────────▼───────────┐
│ 3. Send Notification│ (Send Mail Task)
└─────────────────────┘

Data Flow Structure (Inside Task 2):
CSV Source → Data Conversion → Derived Column → OLE DB Destination
```

### 5. What are Variables and Parameters in SSIS?

**Answer:**

**Variables:**
- **Scope**: Package, Container, or Task level
- **Purpose**: Store values during package execution
- **Types**: User-defined or System variables
- **Access**: Read/Write during execution
- **Persistence**: Values lost after execution

**Parameters:**
- **Scope**: Package or Project level
- **Purpose**: Accept external values at runtime
- **Types**: Required or Optional
- **Access**: Read-only during execution
- **Persistence**: Values passed from external source

**Practical Examples:**

**Variable Usage:**
```sql
-- User Variable: BatchDate (DateTime)
-- Used in Execute SQL Task:
SELECT * FROM Orders 
WHERE OrderDate >= ? AND OrderDate < DATEADD(day, 1, ?)

-- System Variable: PackageName
-- Used in logging: "Processing package: " + @[System::PackageName]
```

**Parameter Usage:**
```sql
-- Package Parameter: $Package::SourcePath (String)
-- Used in Flat File Connection Manager
-- File path: $Package::SourcePath + "\\customers.csv"

-- Project Parameter: $Project::Environment (String)
-- Used for environment-specific configurations
```

### 6. What is a Precedence Constraint in SSIS?

**Answer:**
Precedence Constraints control the execution flow between tasks in the Control Flow. They determine when and under what conditions tasks execute.

**Types of Precedence Constraints:**

**By Result:**
- **Success (Green)**: Execute if previous task succeeds
- **Failure (Red)**: Execute if previous task fails
- **Completion (Blue)**: Execute regardless of success/failure

**By Expression:**
- **Expression**: Custom expression evaluates to True/False
- **Expression and Constraint**: Both condition and expression must be met
- **Expression or Constraint**: Either condition or expression must be met

**Logical Operators:**
- **Logical AND**: All incoming constraints must be satisfied
- **Logical OR**: Any incoming constraint must be satisfied

**Real-world Example:**
```
Error Handling Flow:
┌─────────────────┐
│ Import Data     │
└─────┬─────┬─────┘
      │     │ (On Failure)
      │     ▼
      │ ┌─────────────────┐
      │ │ Log Error       │
      │ └─────────────────┘
      │ (On Success)
      ▼
┌─────────────────┐
│ Validate Data   │
└─────────────────┘
```

### 7. What are Containers in SSIS and when would you use them?

**Answer:**
Containers group tasks and provide additional functionality like looping, transactions, and scope management.

**Types of Containers:**

**1. Sequence Container:**
- **Purpose**: Group related tasks for organization
- **Use Case**: Logical grouping, easier maintenance
- **Transaction Support**: Yes

**2. For Loop Container:**
- **Purpose**: Execute tasks for a specified number of iterations
- **Use Case**: Process multiple files, batch processing
- **Configuration**: InitExpression, EvalExpression, AssignExpression

**3. Foreach Loop Container:**
- **Purpose**: Iterate over collections (files, ADO objects, variables)
- **Use Case**: Process all files in a folder, loop through recordset
- **Enumerator Types**: File, ADO, Variable, NodeList, SMO

**4. ForEach Loop Container Enumerators:**
- **Foreach File**: Process files in a directory
- **Foreach ADO**: Loop through recordset rows
- **Foreach Variable**: Iterate through variable arrays
- **Foreach NodeList**: Process XML nodes

**Practical Examples:**

**ForEach File Example:**
```
Scenario: Process all CSV files in a folder
┌─────────────────────────────────┐
│ ForEach Loop Container          │
│ ┌─────────────────────────────┐ │
│ │ File Enumerator:            │ │
│ │ Folder: C:\Data\Input\      │ │
│ │ Files: *.csv                │ │
│ │ Variable: User::FileName    │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │ Data Flow Task              │ │
│ │ Source: @[User::FileName]   │ │
│ └─────────────────────────────┘ │
└─────────────────────────────────┘
```

**For Loop Example:**
```sql
-- Process data in batches
InitExpression: @BatchNumber = 1
EvalExpression: @BatchNumber <= @MaxBatches
AssignExpression: @BatchNumber = @BatchNumber + 1

-- Inside loop: Process batch @BatchNumber
```

### 8. What is the difference between Merge and Union All transformations?

**Answer:**

**Merge Transformation:**
- **Input Requirement**: Exactly 2 sorted inputs
- **Sorting**: Both inputs must be pre-sorted on merge keys
- **Output**: Single sorted output maintaining sort order
- **Performance**: Fast for sorted data
- **Use Case**: Combine two sorted datasets while maintaining order

**Union All Transformation:**
- **Input Requirement**: Multiple inputs (2 or more)
- **Sorting**: No sorting requirement
- **Output**: Combined output, order not guaranteed
- **Performance**: Faster as no sorting required
- **Use Case**: Append data from multiple sources

**Practical Examples:**

**Merge Scenario:**
```
Use Case: Combine current month and previous month sales data (both sorted by date)

Input 1: Current Month Sales (sorted by SaleDate)
Input 2: Previous Month Sales (sorted by SaleDate)
Output: Combined sales data (maintains chronological order)

Configuration:
- Sort Key: SaleDate
- Both inputs must be sorted before Merge
```

**Union All Scenario:**
```
Use Case: Combine customer data from multiple regions

Input 1: US_Customers
Input 2: EU_Customers  
Input 3: ASIA_Customers
Output: All_Customers

Configuration:
- Map columns by position or name
- No sorting required
- Faster execution
```

### 9. What are the different ways to deploy SSIS packages?

**Answer:**

**Deployment Models:**

**1. Package Deployment Model (Legacy):**
- **Storage**: File system or SQL Server (msdb database)
- **Configuration**: Package configurations (.dtsConfig files)
- **Security**: File system or SQL Server security
- **Management**: SQL Server Management Studio

**2. Project Deployment Model (Recommended):**
- **Storage**: SSIS Catalog (SSISDB database)
- **Configuration**: Parameters and environments
- **Security**: Role-based security with SSISDB
- **Management**: SSIS Catalog with extensive monitoring

**Deployment Methods:**

**Manual Deployment:**
```sql
-- Deploy to SSIS Catalog
1. Build project in SQL Server Data Tools (SSDT)
2. Generate .ispac file
3. Deploy using Integration Services Deployment Wizard
4. Configure environments and parameters

-- Deploy using SQL Server Management Studio
EXEC catalog.deploy_project 
    @project_name = 'DataWarehouseETL',
    @project_stream = (SELECT * FROM OPENROWSET(BULK 'C:\Deploy\DataWarehouseETL.ispac', SINGLE_BLOB) AS x)
```

**Automated Deployment:**
```powershell
# PowerShell deployment script
$SSISNamespace = "Microsoft.SqlServer.Management.IntegrationServices"
$SQLServerName = "PROD-SQL01"
$CatalogName = "SSISDB"
$FolderName = "Production"
$ProjectName = "DataWarehouseETL"

# Deploy project
$catalog.Deploy($projectFilePath, $FolderName, $ProjectName)
```

### 10. What is an SSIS Catalog and what are its benefits?

**Answer:**
The SSIS Catalog (SSISDB) is a centralized repository for storing, managing, and monitoring SSIS projects and packages in SQL Server 2012 and later.

**Components:**
- **Database**: SSISDB system database
- **Folders**: Logical containers for projects
- **Projects**: Collections of packages and project-level resources
- **Environments**: Named collections of parameter values
- **Operations**: Execution tracking and monitoring

**Benefits:**

**1. Centralized Management:**
```sql
-- View all projects and packages
SELECT f.name AS folder_name, p.name AS project_name, pk.name AS package_name
FROM catalog.folders f
INNER JOIN catalog.projects p ON f.folder_id = p.folder_id
INNER JOIN catalog.packages pk ON p.project_id = pk.project_id
```

**2. Enhanced Security:**
- Role-based security (ssis_admin, ssis_logreader)
- Granular permissions at folder, project, and environment levels
- Encryption of sensitive parameters

**3. Advanced Monitoring:**
```sql
-- Monitor package executions
SELECT execution_id, package_name, start_time, end_time, execution_result
FROM catalog.executions
WHERE start_time >= DATEADD(day, -7, GETDATE())
ORDER BY start_time DESC
```

**4. Parameter Management:**
```sql
-- Set environment parameters
EXEC catalog.set_environment_variable_value
    @environment_name = 'Production',
    @variable_name = 'DatabaseServer',
    @value = 'PROD-SQL01'
```

---

## SSIS Intermediate Level

### 11. How do you implement error handling in SSIS packages?

**Answer:**
Error handling in SSIS can be implemented at multiple levels using various techniques:

**1. Precedence Constraints for Task-Level Error Handling:**
```
Task Flow with Error Handling:
┌─────────────────┐
│ Data Import     │
└─────┬─────┬─────┘
      │     │ (On Failure)
      │     ▼
      │ ┌─────────────────┐
      │ │ Log Error &     │
      │ │ Send Alert      │
      │ └─────────────────┘
      │ (On Success)
      ▼
┌─────────────────┐
│ Data Validation │
└─────────────────┘
```

**2. Data Flow Error Outputs:**
```sql
-- Configure error output for transformations
Error Output Configuration:
- Truncation: Redirect Row
- Error: Redirect Row
- Destination: Error_Output table

-- Error table structure
CREATE TABLE ETL_Errors (
    ErrorID INT IDENTITY(1,1),
    PackageName NVARCHAR(255),
    TaskName NVARCHAR(255),
    ErrorCode INT,
    ErrorColumn NVARCHAR(255),
    ErrorDescription NVARCHAR(MAX),
    ErrorDateTime DATETIME2,
    SourceRow NVARCHAR(MAX)
)
```

**3. Try-Catch in Script Tasks:**
```csharp
// Script Task error handling
try
{
    // Business logic here
    Dts.TaskResult = (int)ScriptResults.Success;
}
catch (Exception ex)
{
    // Log error to variables or database
    Dts.Variables["ErrorMessage"].Value = ex.Message;
    Dts.Log($"Error in script task: {ex.Message}", 0, null);
    Dts.TaskResult = (int)ScriptResults.Failure;
}
```

**4. Event Handlers:**
```sql
-- OnError Event Handler
- Capture error information
- Log to database or file
- Send email notifications
- Clean up resources

-- Example: Log error details
INSERT INTO ETL_ErrorLog (PackageName, ErrorCode, ErrorDescription, ErrorDateTime)
VALUES (?, ?, ?, GETDATE())
```

**5. Logging Configuration:**
```xml
<!-- Enable logging in package -->
<LoggingOptions>
    <SelectedLogProviders>
        <LogProvider>SSIS log provider for SQL Server</LogProvider>
    </SelectedLogProviders>
    <LoggingDetails>
        <Event>OnError</Event>
        <Event>OnWarning</Event>
        <Event>OnInformation</Event>
    </LoggingDetails>
</LoggingOptions>
```

### 12. Explain different types of Slowly Changing Dimensions (SCD) and how to implement them in SSIS.

**Answer:**
Slowly Changing Dimensions handle changes to dimension data in data warehouses.

**Types of SCD:**

**SCD Type 1 - Overwrite:**
- **Description**: Overwrite old values with new values
- **Use Case**: Corrections, unimportant historical data
- **Implementation**: Simple UPDATE statement

```sql
-- SCD Type 1 Example
-- Customer address change - overwrite old address
UPDATE DimCustomer 
SET Address = 'New Address', City = 'New City'
WHERE CustomerKey = 123
```

**SCD Type 2 - Add New Record:**
- **Description**: Create new record for each change, keep history
- **Use Case**: Track historical changes over time
- **Implementation**: Insert new record, expire old record

```sql
-- SCD Type 2 Example
-- Step 1: Expire current record
UPDATE DimCustomer 
SET IsActive = 0, EndDate = GETDATE()
WHERE CustomerKey = 123 AND IsActive = 1

-- Step 2: Insert new record
INSERT INTO DimCustomer (CustomerID, Name, Address, City, IsActive, StartDate, EndDate)
VALUES (123, 'John Doe', 'New Address', 'New City', 1, GETDATE(), '9999-12-31')
```

**SCD Type 3 - Add New Column:**
- **Description**: Add column to store previous value
- **Use Case**: Track only one previous value
- **Implementation**: Add column for current and previous values

```sql
-- SCD Type 3 Example
ALTER TABLE DimCustomer ADD PreviousAddress NVARCHAR(255)

UPDATE DimCustomer 
SET PreviousAddress = Address, Address = 'New Address'
WHERE CustomerKey = 123
```

**SSIS SCD Wizard Implementation:**
```
SCD Wizard Configuration:
1. Input Source: Staging table with new/changed records
2. Dimension Table: Target dimension table
3. Business Key: Natural key (CustomerID)
4. SCD Type Selection:
   - Fixed Attributes (Type 1): Name corrections
   - Changing Attributes (Type 2): Address, Phone
   - Historical Attributes (Type 3): Manager changes
5. Inferred Member Support: Handle missing dimension members
```

**Advanced SCD Type 2 with SSIS Components:**
```
Data Flow Design:
OLE DB Source → Lookup (Existing Records) → Conditional Split
                     ↓                            ↓
              [No Match] → Derived Column → Union All → OLE DB Destination
                                              ↑
              [Match + Changed] → Derived Column ----┘
                                              ↓
              [Match + Same] → (Ignore)
```

### 13. How do you optimize SSIS package performance?

**Answer:**
SSIS performance optimization involves multiple strategies at different levels:

**1. Data Flow Optimizations:**

**Buffer Configuration:**
```xml
<!-- Optimize buffer sizes -->
<DataFlow DefaultBufferMaxRows="10000" DefaultBufferSize="104857600">
    <!-- 10,000 rows or 100MB buffer -->
</DataFlow>
```

**Parallel Processing:**
```sql
-- Set MaxConcurrentExecutables for parallel task execution
MaxConcurrentExecutables = -1  -- Use all available processors
EngineThreads = 10            -- Number of worker threads
```

**Source Optimizations:**
```sql
-- Use SQL Command instead of Table/View for better control
SELECT CustomerID, CustomerName, City, State
FROM Customers 
WHERE ModifiedDate >= ?  -- Use parameters for incremental loads
ORDER BY CustomerID      -- Pre-sort data for better performance
```

**2. Transformation Optimizations:**

**Lookup Transformation:**
```sql
-- Optimization strategies:
1. Use appropriate cache mode:
   - Full Cache: Small reference tables (<50MB)
   - Partial Cache: Medium reference tables
   - No Cache: Large reference tables (>500MB)

2. Enable cache transformation for reuse:
   Cache Transform → Multiple Lookup Transformations

3. Optimize lookup query:
   SELECT DISTINCT CustomerKey, CustomerID, CustomerName
   FROM DimCustomer
   WHERE IsActive = 1  -- Filter inactive records
```

**Sort Elimination:**
```sql
-- Avoid unnecessary sorting
1. Pre-sort data at source level
2. Use sorted sources with "IsSorted" property = True
3. Avoid Sort transformation when possible
```

**3. Destination Optimizations:**

**Bulk Insert Operations:**
```sql
-- OLE DB Destination settings
Table Lock = True
Check Constraints = False  -- Disable during load, enable after
Keep Identity = True      -- For identity columns
Keep Nulls = False       -- Better for data quality

-- Batch size optimization
Rows per Batch = 10000    -- Optimal batch size
Maximum Insert Commit Size = 0  -- Commit at end of data flow
```

**4. Memory and Resource Management:**

**Memory Configuration:**
```xml
<!-- Package-level memory settings -->
<Package MaxConcurrentExecutables="4">
    <Property Name="DelayValidation">True</Property>
    <Property Name="MaximumErrorCount">10</Property>
</Package>
```

**Connection Management:**
```sql
-- Connection optimization
RetainSameConnection = True     -- Reuse connections
ConnectRetryCount = 3          -- Retry failed connections
ConnectRetryInterval = 10      -- Seconds between retries
```

**5. Monitoring and Profiling:**

**Performance Counters:**
```sql
-- Monitor key metrics
1. Buffer Memory Usage
2. Buffers Spooled
3. Rows Read/Written per second
4. BLOB bytes read/written
5. Private/Virtual Memory usage
```

**Execution Monitoring:**
```sql
-- Use SSIS Catalog reports
SELECT 
    exe.execution_id,
    exe.package_name,
    exe.start_time,
    exe.end_time,
    DATEDIFF(minute, exe.start_time, exe.end_time) AS duration_minutes,
    stat.execution_path,
    stat.execution_result
FROM catalog.executions exe
INNER JOIN catalog.execution_component_phases stat 
    ON exe.execution_id = stat.execution_id
WHERE exe.start_time >= DATEADD(day, -1, GETDATE())
ORDER BY duration_minutes DESC
```

### 14. What are Expressions in SSIS and provide practical examples?

**Answer:**
SSIS Expressions are formulas that evaluate to values at runtime. They're used in various contexts for dynamic behavior.

**Expression Language Components:**

**1. Literals:**
```sql
-- String literals
"Hello World"
"File_" + "Export"

-- Numeric literals
100
3.14159

-- Date literals
(DT_DBTIMESTAMP)"2024-01-15 10:30:00"
```

**2. Variables and Parameters:**
```sql
-- User variables
@[User::FilePath]
@[User::BatchDate]

-- System variables
@[System::PackageName]
@[System::StartTime]

-- Parameters
$Package::SourceDatabase
$Project::Environment
```

**3. Functions:**

**String Functions:**
```sql
-- String manipulation
UPPER(@[User::CustomerName])                    -- Convert to uppercase
SUBSTRING(@[User::ProductCode], 1, 3)          -- Extract first 3 characters
LEN(@[User::Description])                       -- String length
REPLACE(@[User::PhoneNumber], "-", "")         -- Remove dashes
TRIM(@[User::CustomerName])                     -- Remove leading/trailing spaces
```

**Date Functions:**
```sql
-- Date operations
GETDATE()                                       -- Current date/time
DATEADD("day", -1, GETDATE())                  -- Yesterday
DATEPART("year", @[User::OrderDate])           -- Extract year
(DT_WSTR, 8) DATEPART("year", GETDATE()) + 
    RIGHT("0" + (DT_WSTR, 2) DATEPART("month", GETDATE()), 2) -- YYYYMM format
```

**Mathematical Functions:**
```sql
-- Math operations
ROUND(@[User::Price], 2)                        -- Round to 2 decimals
ABS(@[User::Variance])                          -- Absolute value
@[User::Quantity] * @[User::UnitPrice]         -- Multiplication
```

**4. Conditional Logic:**
```sql
-- Conditional expressions
@[User::Status] == "Active" ? "Y" : "N"        -- Simple condition

-- Complex conditions
@[User::Age] >= 18 && @[User::Status] == "Active" ? "Eligible" : "Not Eligible"

-- Null handling
ISNULL(@[User::MiddleName]) ? @[User::FirstName] + " " + @[User::LastName] 
                             : @[User::FirstName] + " " + @[User::MiddleName] + " " + @[User::LastName]
```

**Practical Examples:**

**1. Dynamic File Naming:**
```sql
-- Expression for file path
@[User::OutputPath] + "\\Customer_Export_" + 
(DT_WSTR, 8) DATEPART("year", GETDATE()) + 
RIGHT("0" + (DT_WSTR, 2) DATEPART("month", GETDATE()), 2) + 
RIGHT("0" + (DT_WSTR, 2) DATEPART("day", GETDATE()), 2) + ".csv"

-- Result: C:\Exports\Customer_Export_20240315.csv
```

**2. Conditional Split Logic:**
```sql
-- Route records based on business rules
-- High Value Customers
@[TotalPurchases] > 10000 && @[CustomerType] == "Premium"

-- New Customers  
DATEDIFF("day", @[RegistrationDate], GETDATE()) <= 30

-- Inactive Customers
DATEDIFF("day", @[LastPurchaseDate], GETDATE()) > 365
```

**3. Derived Column Transformations:**
```sql
-- Full Name creation
ISNULL(@[MiddleName]) ? @[FirstName] + " " + @[LastName] 
                       : @[FirstName] + " " + @[MiddleName] + " " + @[LastName]

-- Email validation flag
FINDSTRING(@[EmailAddress], "@", 1) > 0 && 
FINDSTRING(@[EmailAddress], ".", FINDSTRING(@[EmailAddress], "@", 1)) > 0 ? "Valid" : "Invalid"

-- Age calculation
DATEDIFF("year", @[BirthDate], GETDATE()) - 
(DATEPART("dayofyear", @[BirthDate]) > DATEPART("dayofyear", GETDATE()) ? 1 : 0)
```

**4. Property Expressions:**
```sql
-- Dynamic connection string
"Data Source=" + @[User::ServerName] + ";Initial Catalog=" + @[User::DatabaseName] + ";Integrated Security=True;"

-- Dynamic SQL command
"SELECT * FROM " + @[User::TableName] + " WHERE ModifiedDate >= '" + 
(DT_WSTR, 23) @[User::LastExtractDate] + "'"

-- Conditional task execution
@[User::ProcessCustomers] == "Y"  -- Disable property expression
```

### 15. How do you implement incremental data loading in SSIS?

**Answer:**
Incremental loading loads only new or changed data since the last extraction, improving performance and reducing resource usage.

**Common Approaches:**

**1. Timestamp-Based Incremental Loading:**

**Setup:**
```sql
-- Create control table
CREATE TABLE ETL_Control (
    TableName NVARCHAR(100) PRIMARY KEY,
    LastExtractDateTime DATETIME2,
    CreatedDate DATETIME2 DEFAULT GETDATE(),
    ModifiedDate DATETIME2 DEFAULT GETDATE()
)

-- Initialize control table
INSERT INTO ETL_Control (TableName, LastExtractDateTime)
VALUES ('Customers', '1900-01-01 00:00:00')
```

**SSIS Implementation:**
```sql
-- Step 1: Get last extract date
Execute SQL Task:
SELECT LastExtractDateTime FROM ETL_Control WHERE TableName = 'Customers'
Result Set: Single Row → Variable: LastExtractDate

-- Step 2: Extract incremental data
OLE DB Source Query:
SELECT CustomerID, CustomerName, Address, ModifiedDate
FROM Customers 
WHERE ModifiedDate > ? AND ModifiedDate <= GETDATE()
Parameters: 0 = User::LastExtractDate

-- Step 3: Update control table
Execute SQL Task:
UPDATE ETL_Control 
SET LastExtractDateTime = GETDATE(), ModifiedDate = GETDATE()
WHERE TableName = 'Customers'
```

**2. Change Data Capture (CDC) Based Loading:**

**Enable CDC:**
```sql
-- Enable CDC on database
EXEC sys.sp_cdc_enable_db

-- Enable CDC on table
EXEC sys.sp_cdc_enable_table
    @source_schema = 'dbo',
    @source_name = 'Customers',
    @role_name = NULL,
    @supports_net_changes = 1
```

**SSIS CDC Implementation:**
```sql
-- Use CDC Source component
CDC Source Configuration:
- CDC Processing Mode: All with Old Values
- CDC State Variable: User::CDCState
- Capture Instance: dbo_Customers

-- Process different change types
Conditional Split:
- Insert: __$operation == 2
- Update: __$operation == 4  
- Delete: __$operation == 1

-- Route to appropriate destinations
```

**3. Checksum-Based Change Detection:**

**Source Table Modification:**
```sql
-- Add checksum column to source
ALTER TABLE Customers 
ADD DataChecksum AS CHECKSUM(CustomerName, Address, City, State, ZipCode) PERSISTED

-- Add to staging table
ALTER TABLE Staging_Customers 
ADD DataChecksum INT
```

**SSIS Change Detection:**
```sql
-- Compare checksums to detect changes
Lookup Transformation:
Reference Table: Production.Customers
Lookup Columns: CustomerID
Return Columns: DataChecksum AS ExistingChecksum

Conditional Split:
- New Records: ISNULL(ExistingChecksum)
- Changed Records: !ISNULL(ExistingChecksum) && DataChecksum != ExistingChecksum
- Unchanged Records: DataChecksum == ExistingChecksum (redirect to no-op)
```

**4. Delta Detection with Merge:**

**Staging Approach:**
```sql
-- Load all source data to staging
Data Flow: Source → Staging_Customers

-- Identify changes using MERGE
Execute SQL Task:
WITH SourceData AS (
    SELECT CustomerID, CustomerName, Address, City, State, ZipCode, 
           CHECKSUM(CustomerName, Address, City, State, ZipCode) AS DataChecksum
    FROM Staging_Customers
),
TargetData AS (
    SELECT CustomerID, CustomerName, Address, City, State, ZipCode,
           CHECKSUM(CustomerName, Address, City, State, ZipCode) AS DataChecksum
    FROM Production.Customers
)
MERGE Production.Customers AS target
USING SourceData AS source ON target.CustomerID = source.CustomerID
WHEN MATCHED AND target.DataChecksum != source.DataChecksum THEN
    UPDATE SET CustomerName = source.CustomerName,
               Address = source.Address,
               City = source.City,
               State = source.State,
               ZipCode = source.ZipCode,
               ModifiedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (CustomerID, CustomerName, Address, City, State, ZipCode, CreatedDate)
    VALUES (source.CustomerID, source.CustomerName, source.Address, 
            source.City, source.State, source.ZipCode, GETDATE())
WHEN NOT MATCHED BY SOURCE THEN
    UPDATE SET IsActive = 0, ModifiedDate = GETDATE();
```

**5. Watermark-Based Incremental Loading:**

**Implementation:**
```sql
-- Package parameters
$Package::WatermarkValue (DateTime)
$Package::NewWatermarkValue (DateTime)

-- Get current watermark
Execute SQL Task:
SELECT MAX(ModifiedDate) FROM ETL_Control WHERE TableName = 'Orders'
Result: Single Value → Parameter: WatermarkValue

-- Extract data using watermark
OLE DB Source:
SELECT OrderID, CustomerID, OrderDate, TotalAmount, ModifiedDate
FROM Orders 
WHERE ModifiedDate > ? 
ORDER BY ModifiedDate
Parameters: 0 = $Package::WatermarkValue

-- Calculate new watermark
Execute SQL Task:
SELECT MAX(ModifiedDate) FROM Orders WHERE ModifiedDate > ?
Parameters: 0 = $Package::WatermarkValue
Result: Single Value → Parameter: NewWatermarkValue

-- Update watermark
Execute SQL Task:
UPDATE ETL_Control 
SET LastExtractDateTime = ?
WHERE TableName = 'Orders'
Parameters: 0 = $Package::NewWatermarkValue
```

---

## SSIS Expert Level

### 16. How do you implement custom transformations and script components in SSIS?

**Answer:**
Custom transformations and script components extend SSIS functionality when built-in components don't meet specific requirements.

**Script Component Types:**

**1. Script Transformation (Synchronous):**
```csharp
[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{
    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        // Synchronous transformation - one output row per input row
        
        // Example: Custom phone number formatting
        if (!Row.PhoneNumber_IsNull)
        {
            string phone = Row.PhoneNumber;
            // Remove all non-digits
            phone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
            
            if (phone.Length == 10)
            {
                // Format as (123) 456-7890
                Row.FormattedPhone = $"({phone.Substring(0, 3)}) {phone.Substring(3, 3)}-{phone.Substring(6, 4)}";
                Row.IsValidPhone = true;
            }
            else
            {
                Row.FormattedPhone = phone;
                Row.IsValidPhone = false;
            }
        }
        else
        {
            Row.FormattedPhone_IsNull = true;
            Row.IsValidPhone = false;
        }
    }
}
```

**2. Script Source (Asynchronous):**
```csharp
[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{
    public override void CreateNewOutputRows()
    {
        // Generate custom data or read from external sources
        
        // Example: Generate test data
        Random rand = new Random();
        string[] firstNames = {"John", "Jane", "Bob", "Alice", "Charlie"};
        string[] lastNames = {"Smith", "Doe", "Johnson", "Brown", "Davis"};
        
        for (int i = 1; i <= 1000; i++)
        {
            Output0Buffer.AddRow();
            Output0Buffer.CustomerID = i;
            Output0Buffer.FirstName = firstNames[rand.Next(firstNames.Length)];
            Output0Buffer.LastName = lastNames[rand.Next(lastNames.Length)];
            Output0Buffer.Email = $"{Output0Buffer.FirstName.ToLower()}.{Output0Buffer.LastName.ToLower()}@example.com";
            Output0Buffer.CreatedDate = DateTime.Now.AddDays(-rand.Next(365));
        }
    }
}
```

**3. Script Destination:**
```csharp
[Microsoft.SqlServer.Dts.Pipeline.SSISScriptComponentEntryPointAttribute]
public class ScriptMain : UserComponent
{
    private StreamWriter logWriter;
    
    public override void PreExecute()
    {
        // Initialize resources
        string logPath = Variables.LogFilePath.ToString();
        logWriter = new StreamWriter(logPath, true);
        logWriter.WriteLine($"Processing started at: {DateTime.Now}");
    }
    
    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        // Custom destination logic
        try
        {
            // Example: Log high-value transactions
            if (Row.Amount > 10000)
            {
                logWriter.WriteLine($"High-value transaction: ID={Row.TransactionID}, Amount={Row.Amount:C}, Date={Row.TransactionDate}");
            }
            
            // Could also write to web service, message queue, etc.
        }
        catch (Exception ex)
        {
            this.ComponentMetaData.FireError(0, "Script Destination", ex.Message, "", 0, out bool cancel);
        }
    }
    
    public override void PostExecute()
    {
        // Clean up resources
        logWriter?.WriteLine($"Processing completed at: {DateTime.Now}");
        logWriter?.Close();
        logWriter?.Dispose();
    }
}
```

**Advanced Script Component Features:**

**4. Multiple Outputs (Asynchronous):**
```csharp
public class ScriptMain : UserComponent
{
    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        // Route data to different outputs based on business logic
        
        if (Row.CustomerType == "Premium")
        {
            PremiumCustomersBuffer.AddRow();
            PremiumCustomersBuffer.CustomerID = Row.CustomerID;
            PremiumCustomersBuffer.CustomerName = Row.CustomerName;
            PremiumCustomersBuffer.TotalPurchases = Row.TotalPurchases;
        }
        else if (Row.CustomerType == "Standard")
        {
            StandardCustomersBuffer.AddRow();
            StandardCustomersBuffer.CustomerID = Row.CustomerID;
            StandardCustomersBuffer.CustomerName = Row.CustomerName;
        }
        else
        {
            // Send to error output
            ErrorOutputBuffer.AddRow();
            ErrorOutputBuffer.CustomerID = Row.CustomerID;
            ErrorOutputBuffer.ErrorDescription = "Unknown customer type";
        }
    }
}
```

**5. External Assembly Integration:**
```csharp
// Add references to external assemblies
using ExternalLibrary;
using Newtonsoft.Json;

public class ScriptMain : UserComponent
{
    private ExternalService service;
    
    public override void PreExecute()
    {
        // Initialize external service
        service = new ExternalService(Variables.ApiKey.ToString());
    }
    
    public override void Input0_ProcessInputRow(Input0Buffer Row)
    {
        try
        {
            // Call external service
            var result = service.ValidateAddress(new Address
            {
                Street = Row.Street,
                City = Row.City,
                State = Row.State,
                ZipCode = Row.ZipCode
            });
            
            Row.IsValidAddress = result.IsValid;
            Row.StandardizedAddress = result.StandardizedAddress;
            Row.Latitude = result.Latitude;
            Row.Longitude = result.Longitude;
        }
        catch (Exception ex)
        {
            Row.IsValidAddress = false;
            Row.ValidationError = ex.Message;
        }
    }
}
```

**Custom Task Development:**

**6. Custom Task Example:**
```csharp
[DtsTask(DisplayName = "File Archiver Task",
         Description = "Archives files based on age and size criteria")]
public class FileArchiverTask : Task
{
    public string SourceFolder { get; set; }
    public string ArchiveFolder { get; set; }
    public int DaysOld { get; set; }
    public long MinFileSizeBytes { get; set; }
    
    public override DTSExecResult Execute(Connections connections, 
                                        VariableDispenser variableDispenser, 
                                        IDTSComponentEvents componentEvents, 
                                        IDTSLogging log, 
                                        object transaction)
    {
        try
        {
            DirectoryInfo sourceDir = new DirectoryInfo(SourceFolder);
            DirectoryInfo archiveDir = new DirectoryInfo(ArchiveFolder);
            
            if (!archiveDir.Exists)
                archiveDir.Create();
            
            var filesToArchive = sourceDir.GetFiles()
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-DaysOld) && 
                           f.Length > MinFileSizeBytes);
            
            foreach (var file in filesToArchive)
            {
                string archivePath = Path.Combine(ArchiveFolder, file.Name);
                file.MoveTo(archivePath);
                
                componentEvents.FireInformation(0, "FileArchiverTask", 
                    $"Archived file: {file.Name}", "", 0, ref fireAgain);
            }
            
            return DTSExecResult.Success;
        }
        catch (Exception ex)
        {
            componentEvents.FireError(0, "FileArchiverTask", ex.Message, "", 0);
            return DTSExecResult.Failure;
        }
    }
}
```

### 17. How do you implement advanced logging and monitoring in SSIS?

**Answer:**
Advanced logging and monitoring provide comprehensive visibility into package execution, performance, and errors.

**1. SSIS Catalog Logging:**

**Built-in Logging Views:**
```sql
-- Execution overview
SELECT 
    exe.execution_id,
    exe.folder_name,
    exe.project_name,
    exe.package_name,
    exe.start_time,
    exe.end_time,
    DATEDIFF(minute, exe.start_time, exe.end_time) AS duration_minutes,
    exe.execution_result,
    exe.stopped_by_name
FROM catalog.executions exe
WHERE exe.start_time >= DATEADD(day, -7, GETDATE())
ORDER BY exe.start_time DESC

-- Detailed execution statistics
SELECT 
    stats.execution_id,
    stats.execution_path,
    stats.start_time,
    stats.end_time,
    DATEDIFF(millisecond, stats.start_time, stats.end_time) AS duration_ms,
    stats.execution_result,
    stats.execution_value
FROM catalog.execution_component_phases stats
WHERE stats.execution_id = @ExecutionId
ORDER BY stats.start_time

-- Error details
SELECT 
    msg.operation_id,
    msg.message_time,
    msg.message_type,
    msg.message_source_name,
    msg.message,
    msg.extended_info_id
FROM catalog.operation_messages msg
WHERE msg.operation_id = @ExecutionId 
    AND msg.message_type IN (110, 120, 130)  -- Errors, Warnings, Information
ORDER BY msg.message_time DESC
```

**2. Custom Logging Framework:**

**Logging Tables:**
```sql
-- Create comprehensive logging tables
CREATE TABLE ETL_PackageExecution (
    ExecutionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    PackageName NVARCHAR(255) NOT NULL,
    Environment NVARCHAR(50) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL, -- Running, Success, Failed
    ExecutionUser NVARCHAR(255) NOT NULL,
    ServerName NVARCHAR(255) NOT NULL,
    ParametersJSON NVARCHAR(MAX) NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    RowsProcessed BIGINT NULL,
    DurationSeconds AS DATEDIFF(SECOND, StartTime, EndTime),
    CreatedDate DATETIME2 DEFAULT GETDATE()
)

CREATE TABLE ETL_TaskExecution (
    TaskExecutionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ExecutionID BIGINT NOT NULL,
    TaskName NVARCHAR(255) NOT NULL,
    TaskType NVARCHAR(100) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL,
    RowsRead BIGINT NULL,
    RowsWritten BIGINT NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    FOREIGN KEY (ExecutionID) REFERENCES ETL_PackageExecution(ExecutionID)
)

CREATE TABLE ETL_DataQuality (
    QualityCheckID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ExecutionID BIGINT NOT NULL,
    CheckName NVARCHAR(255) NOT NULL,
    CheckType NVARCHAR(100) NOT NULL, -- RowCount, Duplicates, NullValues, DataType
    ExpectedValue NVARCHAR(255) NULL,
    ActualValue NVARCHAR(255) NULL,
    Status NVARCHAR(20) NOT NULL, -- Pass, Fail, Warning
    CheckTime DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ExecutionID) REFERENCES ETL_PackageExecution(ExecutionID)
)
```

**Logging Script Component:**
```csharp
public class CustomLogger : UserComponent
{
    private SqlConnection logConnection;
    private long executionId;
    
    public override void PreExecute()
    {
        // Initialize logging
        string connectionString = Variables.LoggingConnectionString;
        logConnection = new SqlConnection(connectionString);
        logConnection.Open();
        
        // Start package execution logging
        using (var cmd = new SqlCommand(@"
            INSERT INTO ETL_PackageExecution 
            (PackageName, Environment, StartTime, Status, ExecutionUser, ServerName, ParametersJSON)
            VALUES (@PackageName, @Environment, @StartTime, @Status, @User, @Server, @Parameters);
            SELECT SCOPE_IDENTITY();", logConnection))
        {
            cmd.Parameters.AddWithValue("@PackageName", Variables.PackageName);
            cmd.Parameters.AddWithValue("@Environment", Variables.Environment);
            cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@Status", "Running");
            cmd.Parameters.AddWithValue("@User", Environment.UserName);
            cmd.Parameters.AddWithValue("@Server", Environment.MachineName);
            cmd.Parameters.AddWithValue("@Parameters", Variables.ParametersJSON);
            
            executionId = Convert.ToInt64(cmd.ExecuteScalar());
        }
        
        Variables.ExecutionID = executionId;
    }
    
    public void LogTaskStart(string taskName, string taskType)
    {
        using (var cmd = new SqlCommand(@"
            INSERT INTO ETL_TaskExecution 
            (ExecutionID, TaskName, TaskType, StartTime, Status)
            VALUES (@ExecutionID, @TaskName, @TaskType, @StartTime, @Status)", logConnection))
        {
            cmd.Parameters.AddWithValue("@ExecutionID", executionId);
            cmd.Parameters.AddWithValue("@TaskName", taskName);
            cmd.Parameters.AddWithValue("@TaskType", taskType);
            cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@Status", "Running");
            cmd.ExecuteNonQuery();
        }
    }
    
    public void LogDataQualityCheck(string checkName, string checkType, 
                                   string expectedValue, string actualValue, string status)
    {
        using (var cmd = new SqlCommand(@"
            INSERT INTO ETL_DataQuality 
            (ExecutionID, CheckName, CheckType, ExpectedValue, ActualValue, Status)
            VALUES (@ExecutionID, @CheckName, @CheckType, @ExpectedValue, @ActualValue, @Status)", logConnection))
        {
            cmd.Parameters.AddWithValue("@ExecutionID", executionId);
            cmd.Parameters.AddWithValue("@CheckName", checkName);
            cmd.Parameters.AddWithValue("@CheckType", checkType);
            cmd.Parameters.AddWithValue("@ExpectedValue", expectedValue ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ActualValue", actualValue ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.ExecuteNonQuery();
        }
    }
}
```

**3. Performance Monitoring:**

**Buffer Monitoring Script:**
```csharp
public class PerformanceMonitor : UserComponent
{
    private PerformanceCounter bufferMemoryCounter;
    private PerformanceCounter privateMemoryCounter;
    private Timer performanceTimer;
    
    public override void PreExecute()
    {
        // Initialize performance counters
        bufferMemoryCounter = new PerformanceCounter("SSIS Pipeline", "Buffer Memory Usage", "");
        privateMemoryCounter = new PerformanceCounter("Process", "Private Bytes", "DTExec");
        
        // Log performance metrics every 30 seconds
        performanceTimer = new Timer(LogPerformanceMetrics, null, 0, 30000);
    }
    
    private void LogPerformanceMetrics(object state)
    {
        try
        {
            var metrics = new
            {
                Timestamp = DateTime.Now,
                BufferMemoryMB = bufferMemoryCounter.NextValue() / (1024 * 1024),
                PrivateMemoryMB = privateMemoryCounter.NextValue() / (1024 * 1024),
                ExecutionID = Variables.ExecutionID
            };
            
            // Log to database or file
            LogMetrics(metrics);
        }
        catch (Exception ex)
        {
            // Handle logging errors
        }
    }
}
```

**4. Real-time Monitoring Dashboard:**

**Monitoring Queries:**
```sql
-- Real-time execution monitoring
CREATE VIEW vw_CurrentExecutions AS
SELECT 
    exe.execution_id,
    exe.package_name,
    exe.start_time,
    DATEDIFF(minute, exe.start_time, GETDATE()) AS running_minutes,
    exe.execution_result,
    CASE exe.status 
        WHEN 1 THEN 'Created'
        WHEN 2 THEN 'Running' 
        WHEN 3 THEN 'Canceled'
        WHEN 4 THEN 'Failed'
        WHEN 7 THEN 'Success'
        WHEN 8 THEN 'Stopping'
        WHEN 9 THEN 'Completed'
    END AS status_description,
    (SELECT COUNT(*) FROM catalog.operation_messages msg 
     WHERE msg.operation_id = exe.execution_id AND msg.message_type = 120) AS error_count,
    (SELECT COUNT(*) FROM catalog.operation_messages msg 
     WHERE msg.operation_id = exe.execution_id AND msg.message_type = 110) AS warning_count
FROM catalog.executions exe
WHERE exe.status IN (2, 8)  -- Running or Stopping
   OR (exe.end_time >= DATEADD(hour, -1, GETDATE()))  -- Completed in last hour

-- Performance trending
CREATE VIEW vw_PerformanceTrends AS
SELECT 
    exe.package_name,
    CAST(exe.start_time AS DATE) AS execution_date,
    COUNT(*) AS execution_count,
    AVG(DATEDIFF(minute, exe.start_time, exe.end_time)) AS avg_duration_minutes,
    MIN(DATEDIFF(minute, exe.start_time, exe.end_time)) AS min_duration_minutes,
    MAX(DATEDIFF(minute, exe.start_time, exe.end_time)) AS max_duration_minutes,
    SUM(CASE WHEN exe.execution_result = 0 THEN 1 ELSE 0 END) AS success_count,
    SUM(CASE WHEN exe.execution_result = 1 THEN 1 ELSE 0 END) AS failure_count
FROM catalog.executions exe
WHERE exe.start_time >= DATEADD(day, -30, GETDATE())
GROUP BY exe.package_name, CAST(exe.start_time AS DATE)
```

**5. Alerting and Notifications:**

**Email Alert Script:**
```csharp
public void SendAlert(string alertType, string message, string packageName)
{
    try
    {
        using (var smtpClient = new SmtpClient(Variables.SMTPServer))
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(Variables.FromEmail),
                Subject = $"SSIS Alert - {alertType}: {packageName}",
                Body = BuildAlertMessage(alertType, message, packageName),
                IsBodyHtml = true
            };
            
            // Add recipients based on alert type
            if (alertType == "ERROR")
            {
                mailMessage.To.Add(Variables.ErrorNotificationEmails);
                mailMessage.Priority = MailPriority.High;
            }
            else if (alertType == "WARNING")
            {
                mailMessage.To.Add(Variables.WarningNotificationEmails);
            }
            
            smtpClient.Send(mailMessage);
        }
    }
    catch (Exception ex)
    {
        // Log email sending error
        LogError($"Failed to send alert email: {ex.Message}");
    }
}

private string BuildAlertMessage(string alertType, string message, string packageName)
{
    var html = new StringBuilder();
    html.AppendLine("<html><body>");
    html.AppendLine($"<h2>SSIS {alertType} Alert</h2>");
    html.AppendLine($"<p><strong>Package:</strong> {packageName}</p>");
    html.AppendLine($"<p><strong>Time:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
    html.AppendLine($"<p><strong>Server:</strong> {Environment.MachineName}</p>");
    html.AppendLine($"<p><strong>Message:</strong></p>");
    html.AppendLine($"<pre>{message}</pre>");
    
    if (alertType == "ERROR")
    {
        html.AppendLine("<p>Please investigate this issue immediately.</p>");
    }
    
    html.AppendLine("</body></html>");
    return html.ToString();
}
```

---

## SSRS Fundamentals (Beginner)

### 18. What is SSRS and what are its main components?

**Answer:**
SSRS (SQL Server Reporting Services) is a server-based reporting platform that provides comprehensive reporting functionality for various data sources. It's part of SQL Server Services suite.

**Main Components:**

**1. Report Server:**
- **Web Service**: Processes and renders reports
- **Report Server Database**: Stores report metadata, cached reports, and configuration
- **Report Manager**: Web-based administration interface

**2. Report Server Database:**
- **ReportServer**: Primary database storing report definitions, metadata
- **ReportServerTempDB**: Temporary database for cached reports and sessions

**3. Report Builder:**
- **Client Tool**: For creating and editing reports
- **Ad-hoc Reporting**: Business user-friendly report creation
- **Report Parts**: Reusable report components

**4. SQL Server Data Tools (SSDT):**
- **Development Environment**: Professional report development
- **Report Projects**: Visual Studio integration
- **Advanced Features**: Complex reports, subreports, drill-through

**Architecture Overview:**
```
Client Applications
        ↓
Report Manager (Web Portal)
        ↓
Report Server Web Service
        ↓
Report Server Database
        ↓
Data Sources (SQL Server, Oracle, Web Services, etc.)
```

### 19. What are the different types of reports in SSRS?

**Answer:**
SSRS supports various report types to meet different business requirements:

**1. Tabular Reports:**
- **Description**: Traditional row-and-column format
- **Use Case**: Financial statements, customer lists, inventory reports
- **Structure**: Headers, detail rows, footers

```sql
-- Example: Customer Sales Report
SELECT 
    c.CustomerName,
    c.City,
    c.State,
    SUM(o.TotalAmount) AS TotalSales,
    COUNT(o.OrderID) AS OrderCount
FROM Customers c
INNER JOIN Orders o ON c.CustomerID = o.CustomerID
WHERE o.OrderDate >= @StartDate AND o.OrderDate <= @EndDate
GROUP BY c.CustomerName, c.City, c.State
ORDER BY TotalSales DESC
```

**2. Matrix Reports (Crosstab):**
- **Description**: Pivot table format with row and column groups
- **Use Case**: Sales by region and month, performance matrices
- **Structure**: Row groups, column groups, data cells

```sql
-- Example: Sales by Month and Region
SELECT 
    YEAR(OrderDate) AS SalesYear,
    MONTH(OrderDate) AS SalesMonth,
    Region,
    SUM(TotalAmount) AS SalesAmount
FROM Orders o
INNER JOIN Customers c ON o.CustomerID = c.CustomerID
GROUP BY YEAR(OrderDate), MONTH(OrderDate), Region
```

**3. List Reports:**
- **Description**: Free-form layout with repeated data regions
- **Use Case**: Invoices, catalogs, mailing labels
- **Structure**: Flexible positioning of report items

**4. Subreports:**
- **Description**: Reports embedded within other reports
- **Use Case**: Customer details with order history, complex hierarchical data
- **Limitation**: Performance impact if overused

**5. Drill-through Reports:**
- **Description**: Navigation from summary to detailed reports
- **Use Case**: Dashboard with detailed views, hierarchical navigation
- **Implementation**: Action settings on report items

**6. Chart Reports:**
- **Description**: Graphical data representation
- **Types**: Column, Bar, Line, Pie, Area, Scatter, etc.
- **Use Case**: Trends, comparisons, distributions

**7. Dashboard Reports:**
- **Description**: Multiple visualizations on single report
- **Use Case**: Executive dashboards, KPI monitoring
- **Components**: Charts, gauges, sparklines, data bars

### 20. What are Data Sources and Datasets in SSRS?

**Answer:**

**Data Sources:**
Data Sources define connections to external data systems.

**Types of Data Sources:**
- **SQL Server**: Most common, native integration
- **OLE DB**: Generic database connectivity
- **ODBC**: Legacy database systems
- **Oracle**: Native Oracle connectivity
- **XML**: Web services, XML files
- **Web Service**: SOAP/REST web services
- **SharePoint**: SharePoint lists and libraries
- **Azure SQL Database**: Cloud database connectivity

**Data Source Configuration:**
```xml
<!-- Shared Data Source Example -->
<DataSource Name="AdventureWorks">
    <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>Data Source=SERVER01;Initial Catalog=AdventureWorks</ConnectString>
        <IntegratedSecurity>true</IntegratedSecurity>
    </ConnectionProperties>
</DataSource>
```

**Datasets:**
Datasets define the actual data retrieved from data sources.

**Dataset Types:**
- **Query-based**: SQL queries, stored procedures
- **Command Type**: Text, StoredProcedure, TableDirect
- **Parameters**: Dynamic query filtering
- **Calculated Fields**: Computed columns using expressions

**Dataset Examples:**

**Simple Query Dataset:**
```sql
-- Basic dataset query
SELECT 
    ProductID,
    ProductName,
    CategoryName,
    UnitPrice,
    UnitsInStock
FROM Products p
INNER JOIN Categories c ON p.CategoryID = c.CategoryID
WHERE (@CategoryID IS NULL OR p.CategoryID = @CategoryID)
ORDER BY ProductName
```

**Stored Procedure Dataset:**
```sql
-- Stored procedure with parameters
CREATE PROCEDURE sp_GetSalesReport
    @StartDate DATE,
    @EndDate DATE,
    @RegionID INT = NULL
AS
BEGIN
    SELECT 
        r.RegionName,
        e.FirstName + ' ' + e.LastName AS SalesRep,
        COUNT(o.OrderID) AS OrderCount,
        SUM(od.UnitPrice * od.Quantity) AS TotalSales
    FROM Orders o
    INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
    INNER JOIN Territories t ON e.TerritoryID = t.TerritoryID
    INNER JOIN Region r ON t.RegionID = r.RegionID
    INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
    WHERE o.OrderDate >= @StartDate 
      AND o.OrderDate <= @EndDate
      AND (@RegionID IS NULL OR r.RegionID = @RegionID)
    GROUP BY r.RegionName, e.FirstName, e.LastName
    ORDER BY TotalSales DESC
END
```

**Calculated Fields:**
```vb
' VB.NET expression for calculated field
=Fields!UnitPrice.Value * Fields!Quantity.Value * (1 - Fields!Discount.Value)

' Conditional formatting
=IIF(Fields!UnitsInStock.Value <= Fields!ReorderLevel.Value, "Red", "Black")

' Date formatting
=Format(Fields!OrderDate.Value, "MMMM yyyy")
```

### 21. Explain Report Parameters in SSRS and their types.

**Answer:**
Report Parameters allow users to filter and customize report content dynamically.

**Parameter Types:**

**1. Data Types:**
- **Boolean**: True/False values
- **DateTime**: Date and time values
- **Integer**: Whole numbers
- **Float**: Decimal numbers
- **String**: Text values

**2. Parameter Properties:**

**Basic Properties:**
```xml
<!-- Parameter Configuration -->
<ReportParameter Name="StartDate">
    <DataType>DateTime</DataType>
    <DefaultValue>
        <Values>
            <Value>=Today().AddDays(-30)</Value>
        </Values>
    </DefaultValue>
    <Prompt>Start Date:</Prompt>
    <AllowBlank>false</AllowBlank>
    <AllowNull>false</AllowNull>
</ReportParameter>
```

**Available Values (Dropdown Lists):**
```sql
-- Parameter query for dropdown
SELECT DISTINCT 
    CategoryID AS Value,
    CategoryName AS Label
FROM Categories
ORDER BY CategoryName
```

**3. Multi-Value Parameters:**
```xml
<ReportParameter Name="CategoryIDs">
    <DataType>Integer</DataType>
    <MultiValue>true</MultiValue>
    <AvailableValues>
        <DataSetReference>
            <DataSetName>CategoryDataset</DataSetName>
            <ValueField>CategoryID</ValueField>
            <LabelField>CategoryName</LabelField>
        </DataSetReference>
    </AvailableValues>
</ReportParameter>
```

**Using Multi-Value Parameters in Queries:**
```sql
-- Handle multi-value parameters
SELECT *
FROM Products
WHERE CategoryID IN (@CategoryIDs)

-- Alternative approach for complex scenarios
SELECT *
FROM Products
WHERE (@CategoryIDs IS NULL 
       OR CategoryID IN (SELECT CAST(value AS INT) 
                         FROM STRING_SPLIT(@CategoryIDs, ',')))
```

**4. Cascading Parameters:**
```sql
-- First parameter: Regions
SELECT DISTINCT RegionID, RegionName 
FROM Regions 
ORDER BY RegionName

-- Second parameter: Territories (depends on Region)
SELECT TerritoryID, TerritoryDescription
FROM Territories
WHERE RegionID = @RegionID
ORDER BY TerritoryDescription

-- Third parameter: Employees (depends on Territory)
SELECT EmployeeID, FirstName + ' ' + LastName AS EmployeeName
FROM Employees
WHERE TerritoryID IN (@TerritoryIDs)
ORDER BY LastName, FirstName
```

**5. Hidden Parameters:**
```vb
' Hidden parameters for internal calculations
=Parameters!UserId.Value
=Parameters!ReportExecutionTime.Value
```

**6. Parameter Expressions and Default Values:**
```vb
' Dynamic default values
=Today()                          ' Current date
=Today().AddDays(-30)            ' 30 days ago
=DateAdd("m", -1, Today())       ' Last month
=User!UserID                     ' Current user
=Globals!ExecutionTime           ' Report execution time

' Conditional defaults
=IIF(Weekday(Today()) = 2, Today().AddDays(-3), Today().AddDays(-1))  ' Monday logic
```

### 22. What are Report Items and Data Regions in SSRS?

**Answer:**

**Report Items:**
Basic building blocks that can be added to report body or data regions.

**1. Text Items:**
- **Textbox**: Display static text or expressions
- **Label**: Static text labels
- **Rich Text**: Formatted text with HTML markup

```vb
' Textbox expressions
="Report Generated on: " & FormatDateTime(Now(), DateFormat.GeneralDate)
="Page " & Globals!PageNumber & " of " & Globals!TotalPages
=User!UserID & " - " & Parameters!ReportTitle.Value
```

**2. Image Items:**
- **Static Images**: Logos, backgrounds
- **Database Images**: BLOB data from database
- **External Images**: URLs or file paths

**3. Line and Rectangle:**
- **Line**: Visual separators
- **Rectangle**: Container for grouping items

**Data Regions:**
Display repeating rows of data from datasets.

**1. Table:**
- **Structure**: Fixed columns, dynamic rows
- **Use Case**: Detail reports, lists
- **Groups**: Row groups for categorization

```xml
<!-- Table structure example -->
<Table>
    <TableColumns>
        <TableColumn><Width>2in</Width></TableColumn>
        <TableColumn><Width>3in</Width></TableColumn>
        <TableColumn><Width>1.5in</Width></TableColumn>
    </TableColumns>
    <Header>
        <TableRows>
            <TableRow>
                <TableCells>
                    <TableCell><ReportItems><Textbox><Value>Product ID</Value></Textbox></ReportItems></TableCell>
                    <TableCell><ReportItems><Textbox><Value>Product Name</Value></Textbox></ReportItems></TableCell>
                    <TableCell><ReportItems><Textbox><Value>Unit Price</Value></Textbox></ReportItems></TableCell>
                </TableCells>
            </TableRow>
        </TableRows>
    </Header>
    <Details>
        <TableRows>
            <TableRow>
                <TableCells>
                    <TableCell><ReportItems><Textbox><Value>=Fields!ProductID.Value</Value></Textbox></ReportItems></TableCell>
                    <TableCell><ReportItems><Textbox><Value>=Fields!ProductName.Value</Value></Textbox></ReportItems></TableCell>
                    <TableCell><ReportItems><Textbox><Value>=Fields!UnitPrice.Value</Value></Textbox></ReportItems></TableCell>
                </TableCells>
            </TableRow>
        </TableRows>
    </Details>
</Table>
```

**2. Matrix:**
- **Structure**: Dynamic rows and columns
- **Use Case**: Crosstab reports, pivot tables
- **Groups**: Row groups and column groups

**3. List:**
- **Structure**: Free-form repeating layout
- **Use Case**: Invoices, catalogs, labels
- **Flexibility**: Position items anywhere within list

**4. Chart:**
- **Types**: Column, Bar, Line, Pie, Area, Scatter, etc.
- **Series**: Data groupings
- **Categories**: X-axis groupings

**Chart Configuration Example:**
```xml
<!-- Chart data configuration -->
<Chart>
    <CategoryGroups>
        <CategoryGroup>
            <Grouping Name="CategoryGroup">
                <GroupExpressions>
                    <GroupExpression>=Fields!CategoryName.Value</GroupExpression>
                </GroupExpressions>
            </Grouping>
        </CategoryGroup>
    </CategoryGroups>
    <SeriesGroups>
        <SeriesGroup>
            <Grouping Name="SeriesGroup">
                <GroupExpressions>
                    <GroupExpression>=Fields!Year.Value</GroupExpression>
                </GroupExpressions>
            </Grouping>
        </SeriesGroup>
    </SeriesGroups>
    <ChartData>
        <ChartSeries>
            <DataPoints>
                <DataPoint>
                    <DataValues>
                        <DataValue>
                            <Value>=Sum(Fields!SalesAmount.Value)</Value>
                        </DataValue>
                    </DataValues>
                </DataPoint>
            </DataPoints>
        </ChartSeries>
    </ChartData>
</Chart>
```

**5. Gauge:**
- **Types**: Radial, Linear
- **Use Case**: KPIs, performance indicators
- **Components**: Scales, pointers, ranges

**6. Sparkline and Data Bar:**
- **Sparkline**: Inline mini-charts
- **Data Bar**: Inline horizontal bar charts
- **Use Case**: Trends within table cells

### 23. How do you create and configure a basic report in SSRS?

**Answer:**
Creating a basic report involves several steps from data source setup to report deployment.

**Step 1: Create Report Project**
```
1. Open SQL Server Data Tools (SSDT)
2. Create New Project → Reporting Services Project
3. Name: "SalesReports"
4. Add to solution or create new solution
```

**Step 2: Create Shared Data Source**
```xml
<!-- AdventureWorks.rds -->
<?xml version="1.0" encoding="utf-8"?>
<RptDataSource xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Name>AdventureWorks</Name>
  <DataSourceDefinition>
    <Extension>SQL</Extension>
    <ConnectString>Data Source=localhost;Initial Catalog=AdventureWorks2019</ConnectString>
    <UseOriginalConnectString>false</UseOriginalConnectString>
    <OriginalConnectStringExpressionBased>false</OriginalConnectStringExpressionBased>
    <Credentials>
      <Storage>Integrated</Storage>
    </Credentials>
  </DataSourceDefinition>
</RptDataSource>
```

**Step 3: Create Report with Table Wizard**
```
1. Add New Item → Report
2. Choose "Table or Matrix Wizard"
3. Select Data Source: AdventureWorks
4. Design Query:
```

```sql
-- Query for Product Sales Report
SELECT 
    p.ProductID,
    p.Name AS ProductName,
    pc.Name AS Category,
    p.ListPrice,
    SUM(sod.OrderQty) AS TotalQuantitySold,
    SUM(sod.LineTotal) AS TotalSales
FROM Production.Product p
    INNER JOIN Production.ProductCategory pc ON p.ProductCategoryID = pc.ProductCategoryID
    INNER JOIN Sales.SalesOrderDetail sod ON p.ProductID = sod.ProductID
    INNER JOIN Sales.SalesOrderHeader soh ON sod.SalesOrderID = soh.SalesOrderID
WHERE soh.OrderDate >= @StartDate 
    AND soh.OrderDate <= @EndDate
    AND (@CategoryID IS NULL OR pc.ProductCategoryID = @CategoryID)
GROUP BY p.ProductID, p.Name, pc.Name, p.ListPrice
HAVING SUM(sod.LineTotal) > 0
ORDER BY TotalSales DESC
```

**Step 4: Configure Parameters**
```xml
<!-- Report Parameters -->
<ReportParameters>
  <ReportParameter Name="StartDate">
    <DataType>DateTime</DataType>
    <DefaultValue>
      <Values>
        <Value>=DateAdd("m", -3, Today())</Value>
      </Values>
    </DefaultValue>
    <Prompt>Start Date:</Prompt>
  </ReportParameter>
  
  <ReportParameter Name="EndDate">
    <DataType>DateTime</DataType>
    <DefaultValue>
      <Values>
        <Value>=Today()</Value>
      </Values>
    </DefaultValue>
    <Prompt>End Date:</Prompt>
  </ReportParameter>
  
  <ReportParameter Name="CategoryID">
    <DataType>Integer</DataType>
    <AllowBlank>true</AllowBlank>
    <AllowNull>true</AllowNull>
    <Prompt>Product Category:</Prompt>
    <AvailableValues>
      <DataSetReference>
        <DataSetName>CategoryDataset</DataSetName>
        <ValueField>ProductCategoryID</ValueField>
        <LabelField>Name</LabelField>
      </DataSetReference>
    </AvailableValues>
  </ReportParameter>
</ReportParameters>
```

**Step 5: Format Table**
```vb
' Header formatting
Font.Bold = True
BackgroundColor = LightGray
TextAlign = Center

' Currency formatting for sales columns
=Format(Fields!ListPrice.Value, "C")
=Format(Fields!TotalSales.Value, "C")

' Number formatting for quantity
=Format(Fields!TotalQuantitySold.Value, "N0")

' Conditional formatting based on sales
=IIF(Fields!TotalSales.Value > 50000, "Green", 
  IIF(Fields!TotalSales.Value > 10000, "Blue", "Black"))
```

**Step 6: Add Report Header/Footer**
```vb
' Report Header
="Product Sales Report - " & Format(Parameters!StartDate.Value, "MM/dd/yyyy") & 
  " to " & Format(Parameters!EndDate.Value, "MM/dd/yyyy")

' Report Footer
="Generated on " & Format(Globals!ExecutionTime, "MM/dd/yyyy hh:mm tt") & 
  " by " & User!UserID

' Page Footer
="Page " & Globals!PageNumber & " of " & Globals!TotalPages
```

**Step 7: Add Grouping and Totals**
```
1. Right-click table row → Insert Group → Parent Group
2. Group by: =Fields!Category.Value
3. Add group header with category name
4. Add group footer with category totals
```

```vb
' Group expressions
Group Header: =Fields!Category.Value
Group Footer: 
  ="Category Total: " & Sum(Fields!TotalSales.Value)
  
Report Footer:
  ="Grand Total: " & Sum(Fields!TotalSales.Value, "DataSet1")
```

**Step 8: Preview and Deploy**
```
1. Preview tab - Test with different parameters
2. Build solution - Check for errors
3. Deploy to Report Server:
   - Set TargetServerURL in project properties
   - Right-click project → Deploy
   - Or deploy individual reports
```

---

## SSRS Intermediate Level

### 24. How do you implement grouping and sorting in SSRS reports?

**Answer:**
Grouping and sorting organize report data hierarchically and control display order.

**Row Grouping Implementation:**

**1. Basic Row Grouping:**
```xml
<!-- Category grouping in table -->
<TableGroups>
  <TableGroup>
    <Grouping Name="CategoryGroup">
      <GroupExpressions>
        <GroupExpression>=Fields!CategoryName.Value</GroupExpression>
      </GroupExpressions>
      <PageBreakAtStart>false</PageBreakAtStart>
      <PageBreakAtEnd>false</PageBreakAtEnd>
    </Grouping>
    <Header>
      <TableRows>
        <TableRow>
          <TableCells>
            <TableCell>
              <ReportItems>
                <Textbox Name="CategoryHeader">
                  <Value>=Fields!CategoryName.Value</Value>
                  <Style>
                    <FontWeight>Bold</FontWeight>
                    <BackgroundColor>LightBlue</BackgroundColor>
                  </Style>
                </Textbox>
              </ReportItems>
            </TableCell>
          </TableCells>
        </TableRow>
      </TableRows>
    </Header>
    <Footer>
      <TableRows>
        <TableRow>
          <TableCells>
            <TableCell>
              <ReportItems>
                <Textbox Name="CategoryTotal">
                  <Value>="Category Total: " &amp; Format(Sum(Fields!SalesAmount.Value), "C")</Value>
                  <Style>
                    <FontWeight>Bold</FontWeight>
                  </Style>
                </Textbox>
              </ReportItems>
            </TableCell>
          </TableCells>
        </TableRow>
      </TableRows>
    </Footer>
  </TableGroup>
</TableGroups>
```

**2. Multiple Level Grouping:**
```sql
-- Data source with multiple grouping levels
SELECT 
    r.RegionName,
    t.TerritoryDescription,
    e.FirstName + ' ' + e.LastName AS SalesRep,
    c.CategoryName,
    p.ProductName,
    SUM(od.UnitPrice * od.Quantity) AS SalesAmount,
    COUNT(od.OrderID) AS OrderCount
FROM Orders o
    INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
    INNER JOIN Products p ON od.ProductID = p.ProductID
    INNER JOIN Categories c ON p.CategoryID = c.CategoryID
    INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
    INNER JOIN Territories t ON e.TerritoryID = t.TerritoryID
    INNER JOIN Region r ON t.RegionID = r.RegionID
WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
GROUP BY r.RegionName, t.TerritoryDescription, e.FirstName, e.LastName, 
         c.CategoryName, p.ProductName
ORDER BY r.RegionName, t.TerritoryDescription, SalesAmount DESC
```

**Grouping Hierarchy:**
```
Region Group (Parent)
  └── Territory Group (Child)
      └── Sales Rep Group (Child)
          └── Category Group (Child)
              └── Detail Rows (Products)
```

**3. Column Grouping (Matrix):**
```xml
<!-- Sales by Month Matrix -->
<Matrix>
  <ColumnGroupings>
    <ColumnGrouping>
      <DynamicColumns>
        <Grouping Name="MonthGroup">
          <GroupExpressions>
            <GroupExpression>=Format(Fields!OrderDate.Value, "yyyy-MM")</GroupExpression>
          </GroupExpressions>
        </Grouping>
        <ReportItems>
          <Textbox Name="MonthHeader">
            <Value>=Format(CDate(Fields!OrderDate.Value), "MMM yyyy")</Value>
          </Textbox>
        </ReportItems>
      </DynamicColumns>
    </ColumnGrouping>
  </ColumnGroupings>
  <RowGroupings>
    <RowGrouping>
      <DynamicRows>
        <Grouping Name="ProductGroup">
          <GroupExpressions>
            <GroupExpression>=Fields!ProductName.Value</GroupExpression>
          </GroupExpressions>
        </Grouping>
        <ReportItems>
          <Textbox Name="ProductName">
            <Value>=Fields!ProductName.Value</Value>
          </Textbox>
        </ReportItems>
      </DynamicRows>
    </RowGrouping>
  </RowGroupings>
</Matrix>
```

**Sorting Implementation:**

**1. Dataset-Level Sorting:**
```sql
-- SQL Server sorting (most efficient)
SELECT ProductName, CategoryName, UnitPrice, UnitsInStock
FROM Products p
INNER JOIN Categories c ON p.CategoryID = c.CategoryID
ORDER BY c.CategoryName, p.UnitPrice DESC, p.ProductName
```

**2. Group-Level Sorting:**
```xml
<!-- Sort groups by total sales descending -->
<Grouping Name="CategoryGroup">
  <GroupExpressions>
    <GroupExpression>=Fields!CategoryName.Value</GroupExpression>
  </GroupExpressions>
  <SortBy>
    <SortExpression>=Sum(Fields!SalesAmount.Value)</SortExpression>
    <Direction>Descending</Direction>
  </SortBy>
</Grouping>
```

**3. Interactive Sorting:**
```xml
<!-- Allow users to sort columns -->
<Textbox Name="ProductNameHeader">
  <Value>Product Name</Value>
  <UserSort>
    <SortExpression>=Fields!ProductName.Value</SortExpression>
    <SortExpressionScope>TableGroup1</SortExpressionScope>
  </UserSort>
  <Style>
    <TextDecoration>Underline</TextDecoration>
    <Color>Blue</Color>
  </Style>
</Textbox>
```

**4. Complex Sorting with Expressions:**
```vb
' Custom sort order using expressions
=IIF(Fields!Priority.Value = "High", 1,
  IIF(Fields!Priority.Value = "Medium", 2, 3))

' Sort by multiple criteria
=Fields!CategoryName.Value & "|" & Format(Fields!UnitPrice.Value, "00000.00")

' Sort dates with null handling
=IIF(IsNothing(Fields!DueDate.Value), CDate("12/31/2099"), Fields!DueDate.Value)
```

**Advanced Grouping Features:**

**5. Conditional Grouping:**
```vb
' Group by different criteria based on parameter
=IIF(Parameters!GroupBy.Value = "Category", Fields!CategoryName.Value,
  IIF(Parameters!GroupBy.Value = "Supplier", Fields!SupplierName.Value,
      Fields!ProductName.Value))
```

**6. Page Breaks:**
```xml
<!-- Page break between groups -->
<Grouping Name="RegionGroup">
  <GroupExpressions>
    <GroupExpression>=Fields!RegionName.Value</GroupExpression>
  </GroupExpressions>
  <PageBreakAtStart>false</PageBreakAtStart>
  <PageBreakAtEnd>true</PageBreakAtEnd>
</Grouping>
```

**7. Keep Together:**
```xml
<!-- Keep group content together -->
<Group Name="CustomerGroup">
  <GroupExpressions>
    <GroupExpression>=Fields!CustomerID.Value</GroupExpression>
  </GroupExpressions>
  <PageBreak>
    <BreakLocation>Between</BreakLocation>
  </PageBreak>
  <KeepTogether>true</KeepTogether>
</Group>
```

### 25. How do you create subreports and drill-through reports in SSRS?

**Answer:**
Subreports and drill-through reports provide hierarchical navigation and detailed views within SSRS.

**Subreports Implementation:**

**1. Creating a Subreport:**

**Main Report (Customer List):**
```sql
-- Main report dataset
SELECT 
    CustomerID,
    CustomerName,
    ContactName,
    City,
    Country,
    Phone
FROM Customers
ORDER BY CustomerName
```

**Subreport (Customer Orders):**
```sql
-- Subreport dataset with parameter
SELECT 
    o.OrderID,
    o.OrderDate,
    o.RequiredDate,
    o.ShippedDate,
    SUM(od.UnitPrice * od.Quantity) AS OrderTotal
FROM Orders o
    INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
WHERE o.CustomerID = @CustomerID
GROUP BY o.OrderID, o.OrderDate, o.RequiredDate, o.ShippedDate
ORDER BY o.OrderDate DESC
```

**Subreport Configuration:**
```xml
<!-- Subreport control in main report -->
<Subreport Name="CustomerOrders">
  <ReportName>CustomerOrdersSubreport</ReportName>
  <Parameters>
    <Parameter Name="CustomerID">
      <Value>=Fields!CustomerID.Value</Value>
    </Parameter>
  </Parameters>
  <Top>0.25in</Top>
  <Left>0.1in</Left>
  <Height>2in</Height>
  <Width>6in</Width>
</Subreport>
```

**2. Subreport with Multiple Parameters:**
```csharp
// Subreport parameter passing
public void ConfigureSubreport()
{
    // In main report code
    private void subreport1_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
    {
        string customerID = e.Parameters["CustomerID"].Values[0].ToString();
        DateTime startDate = DateTime.Parse(e.Parameters["StartDate"].Values[0].ToString());
        DateTime endDate = DateTime.Parse(e.Parameters["EndDate"].Values[0].ToString());
        
        // Get data for subreport
        DataTable orderData = GetCustomerOrders(customerID, startDate, endDate);
        e.DataSources.Add(new ReportDataSource("OrderDataset", orderData));
    }
}
```

**3. Conditional Subreport Display:**
```vb
' Show subreport only if customer has orders
Hidden = (CountRows("OrderDataset") = 0)

' Subreport visibility expression
=IIF(Fields!OrderCount.Value > 0, False, True)
```

**Drill-Through Reports Implementation:**

**4. Simple Drill-Through:**

**Summary Report (Sales by Category):**
```sql
-- Summary level data
SELECT 
    c.CategoryID,
    c.CategoryName,
    COUNT(DISTINCT p.ProductID) AS ProductCount,
    SUM(od.UnitPrice * od.Quantity) AS TotalSales,
    AVG(od.UnitPrice * od.Quantity) AS AverageSale
FROM Categories c
    INNER JOIN Products p ON c.CategoryID = p.CategoryID
    INNER JOIN OrderDetails od ON p.ProductID = od.ProductID
    INNER JOIN Orders o ON od.OrderID = o.OrderID
WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
GROUP BY c.CategoryID, c.CategoryName
ORDER BY TotalSales DESC
```

**Detail Report (Products in Category):**
```sql
-- Detail level data
SELECT 
    p.ProductID,
    p.ProductName,
    s.SupplierName,
    p.UnitPrice,
    SUM(od.Quantity) AS TotalQuantitySold,
    SUM(od.UnitPrice * od.Quantity) AS TotalSales,
    AVG(od.UnitPrice * od.Quantity) AS AverageSale
FROM Products p
    INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
    INNER JOIN OrderDetails od ON p.ProductID = od.ProductID
    INNER JOIN Orders o ON od.OrderID = o.OrderID
WHERE p.CategoryID = @CategoryID
    AND o.OrderDate BETWEEN @StartDate AND @EndDate
GROUP BY p.ProductID, p.ProductName, s.SupplierName, p.UnitPrice
ORDER BY TotalSales DESC
```

**Drill-Through Action Configuration:**
```xml
<!-- Action on category name textbox -->
<Action>
  <Drillthrough>
    <ReportName>ProductDetailReport</ReportName>
    <Parameters>
      <Parameter Name="CategoryID">
        <Value>=Fields!CategoryID.Value</Value>
      </Parameter>
      <Parameter Name="CategoryName">
        <Value>=Fields!CategoryName.Value</Value>
      </Parameter>
      <Parameter Name="StartDate">
        <Value>=Parameters!StartDate.Value</Value>
      </Parameter>
      <Parameter Name="EndDate">
        <Value>=Parameters!EndDate.Value</Value>
      </Parameter>
    </Parameters>
  </Drillthrough>
</Action>
```

**5. Multi-Level Drill-Through:**

**Level 1: Regional Sales Summary**
```sql
SELECT RegionName, COUNT(DISTINCT CustomerID) AS CustomerCount, 
       SUM(TotalSales) AS RegionSales
FROM vw_SalesSummary
GROUP BY RegionName
```

**Level 2: Territory Sales Detail**
```sql
SELECT TerritoryName, SalesRep, CustomerCount, TotalSales
FROM vw_SalesSummary
WHERE RegionName = @RegionName
```

**Level 3: Customer Sales Detail**
```sql
SELECT CustomerName, OrderCount, TotalSales, LastOrderDate
FROM vw_SalesSummary
WHERE TerritoryName = @TerritoryName
```

**6. Drill-Through with Dynamic Report Selection:**
```vb
' Dynamic report name based on data
=IIF(Fields!ReportType.Value = "Summary", "SalesSymmaryReport",
  IIF(Fields!ReportType.Value = "Detail", "SalesDetailReport",
      "DefaultReport"))

' Dynamic parameter passing
=IIF(Fields!DataLevel.Value = "Product", Fields!ProductID.Value,
     Fields!CategoryID.Value)
```

**Best Practices:**

**7. Performance Optimization:**
```sql
-- Optimize subreport queries
-- Use indexed columns in WHERE clauses
-- Limit data returned
-- Consider using shared datasets

-- Subreport with optimized query
SELECT TOP 10 
    OrderID, OrderDate, OrderTotal
FROM vw_CustomerOrders
WHERE CustomerID = @CustomerID
ORDER BY OrderDate DESC
```

**8. Error Handling:**
```vb
' Subreport error handling
=IIF(IsError(ReportItems!Subreport1.Report.ReportItems!Total.Value),
     "Data not available",
     ReportItems!Subreport1.Report.ReportItems!Total.Value)

' Drill-through availability
=IIF(Fields!HasDetailData.Value = True, False, True)  ' Hidden property
```

**9. Navigation Aids:**
```vb
' Breadcrumb navigation in drill-through report
="Home > " & Parameters!RegionName.Value & " > " & Parameters!TerritoryName.Value

' Back button simulation
Action: Go to URL
URL Expression: ="javascript:history.back()"
```

---

*Continuing with SSRS Expert Level and remaining sections...*