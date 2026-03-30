using System.Globalization;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Helpers
{
    public static class CsvParser
    {
        public static List<WifiLog> ParseWifiLogs(Stream fileStream)
        {
            var logs = new List<WifiLog>();

            using (var reader = new StreamReader(fileStream))
            {
                bool isHeader = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var values = line.Split(',');

                    if (values.Length < 5)
                        continue;

                    DateTime startTime = DateTime.TryParse(
                        values[1]?.Trim(),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var start
                    ) ? start : DateTime.MinValue;

                    DateTime endTime = DateTime.TryParse(
                        values[2]?.Trim(),
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var end
                    ) ? end : DateTime.MinValue;

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
            }

            return logs;
        }

        public static List<Employee> ParseMappings(Stream fileStream)
        {
            var employees = new List<Employee>();

            using (var reader = new StreamReader(fileStream))
            {
                bool isHeader = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var values = line.Split(',');

                    if (values.Length < 2)
                        continue;

                    employees.Add(new Employee
                    {
                        EmployeeName = values[0]?.Trim() ?? string.Empty,

                        // ✅ FIXED: same normalization as WiFi logs
                        SerialNumber = values[1]?
                            .Replace("NEX-", "", StringComparison.OrdinalIgnoreCase)
                            .Trim()
                            .ToUpper() ?? string.Empty
                    });
                }
            }

            return employees;
        }
    }
}