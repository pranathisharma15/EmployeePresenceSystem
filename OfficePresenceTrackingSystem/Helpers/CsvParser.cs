using System.Globalization;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Helpers
{
    public static class CsvParser   // Static helper class for parsing CSV files related to WiFi logs and employee mappings
    {
        public static List<WifiLog> ParseWifiLogs(Stream fileStream)    // Parses a CSV file stream to extract WiFi log entries and returns a list of WifiLog objects
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("WiFi file stream is empty");

            var logs = new List<WifiLog>();

            try
            {
                using var reader = new StreamReader(fileStream);    // Using StreamReader to read the CSV file stream
                bool isHeader = true;   // Flag to skip the header row
                int rowNumber = 0;  // Counter to track the current row number for error reporting

                while (!reader.EndOfStream) // Loop through each line of the CSV file until the end is reached
                {
                    var line = reader.ReadLine();
                    rowNumber++;

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try     // Try to parse the current line into a WifiLog object, handling potential format issues
                    {
                        var values = line.Split(',');

                        if (values.Length < 5)
                            throw new FormatException(
                                $"Invalid WiFi CSV format at row {rowNumber}");

                        DateTime startTime = DateTime.TryParse(
                            values[1]?.Trim(),
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var start)
                            ? start
                            : throw new FormatException(
                                $"Invalid StartTime at row {rowNumber}");

                        DateTime endTime = DateTime.TryParse(
                            values[2]?.Trim(),
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var end)
                            ? end
                            : throw new FormatException(
                                $"Invalid EndTime at row {rowNumber}");

                        var cleanedHost = values[4]?
                            .Replace("NEX-", "", StringComparison.OrdinalIgnoreCase)
                            .Trim()
                            .ToUpper();

                        logs.Add(new WifiLog
                        {
                            IPAddress = values[0]?.Trim() ?? string.Empty,
                            StartTime = startTime,
                            EndTime = endTime,
                            MACAddress = values[3]?.Trim() ?? string.Empty,
                            HostName = cleanedHost ?? string.Empty
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(
                            $"Error parsing WiFi CSV row {rowNumber}: {ex.Message}");
                    }
                }

                return logs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse WiFi CSV file: {ex.Message}");
            }
        }

        public static List<Employee> ParseMappings(Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Mapping file stream is empty");

            var employees = new List<Employee>();

            try
            {
                using var reader = new StreamReader(fileStream);
                bool isHeader = true;
                int rowNumber = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    rowNumber++;

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var values = line.Split(',');

                        if (values.Length < 5)
                            throw new FormatException(
                                $"Invalid mapping CSV format at row {rowNumber}. Expected 5 columns.");

                        employees.Add(new Employee
                        {
                            EmployeeName = values[0]?.Trim()
                                ?? throw new FormatException(
                                    $"EmployeeName missing at row {rowNumber}"),

                            SerialNumber = values[1]?
                                .Replace("NEX-", "", StringComparison.OrdinalIgnoreCase)
                                .Trim()
                                .ToUpper()
                                ?? throw new FormatException(
                                    $"SerialNumber missing at row {rowNumber}"),

                            Department = values[2]?.Trim() ?? string.Empty,

                            Team = values[3]?.Trim() ?? string.Empty,

                            Designation = values[4]?.Trim() ?? string.Empty
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(
                            $"Error parsing mapping CSV row {rowNumber}: {ex.Message}");
                    }
                }

                return employees;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse mapping CSV file: {ex.Message}");
            }
        }
    }
}