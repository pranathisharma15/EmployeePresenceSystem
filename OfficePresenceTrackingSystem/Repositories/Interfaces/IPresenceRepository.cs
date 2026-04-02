using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Repositories.Interfaces
{
    public interface IPresenceRepository        // This interface defines the contract for a presence repository, which is responsible for managing the storage and retrieval of WiFi logs, employee data, and presence records in the application. It includes methods for saving and retrieving these data types, allowing for asynchronous operations to ensure efficient database interactions. Implementing this interface allows for a consistent way to access and manipulate presence-related data across the application.
    {
        Task SaveWifiLogsAsync(List<WifiLog> logs);     // This method is responsible for saving a list of WiFi logs to the database. It takes a list of WifiLog objects as a parameter and performs an asynchronous operation to store them in the database. This allows for efficient handling of WiFi log data, which can be used to track employee presence based on their device connections to the office WiFi network.

        Task SaveEmployeesAsync(List<Employee> employees);  // This method is responsible for saving a list of employee data to the database. It takes a list of Employee objects as a parameter and performs an asynchronous operation to store them in the database. This allows for efficient management of employee information, which can be used to associate WiFi logs with specific employees and track their presence in the office.

        Task<List<WifiLog>> GetWifiLogsAsync(); // This method is responsible for retrieving a list of WiFi logs from the database. It performs an asynchronous operation to fetch the data and returns a list of WifiLog objects. This allows for efficient access to WiFi log data, which can be used to analyze employee presence based on their device connections to the office WiFi network.

        Task<List<Employee>> GetEmployeesAsync();   //  This method is responsible for retrieving a list of employee data from the database. It performs an asynchronous operation to fetch the data and returns a list of Employee objects. This allows for efficient access to employee information, which can be used to associate WiFi logs with specific employees and track their presence in the office.

        Task SavePresenceRecordsAsync(List<PresenceRecord> records);    // This method is responsible for saving a list of presence records to the database. It takes a list of PresenceRecord objects as a parameter and performs an asynchronous operation to store them in the database. This allows for efficient management of presence data, which can be used to track employee attendance and analyze office occupancy patterns.

        Task<List<PresenceRecord>> GetPresenceRecordsAsync();

        // This method is responsible for retrieving a list of presence records from the database. It performs an asynchronous operation to fetch the data and returns a list of PresenceRecord objects. This allows for efficient access to presence data, which can be used to analyze employee attendance and office occupancy patterns.
        Task<List<PresenceRecord>> GetPresenceAsync(DateTime? date);
    }

}