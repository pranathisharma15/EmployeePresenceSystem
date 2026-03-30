using System.Globalization;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Helpers
{
    public static class CsvParser
    {
        public static List<WifiLog> ParseWifiLogs(Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("WiFi file stream is empty");

            var logs = new List<WifiLog>();

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

                        if (values.Length < 2)
                            throw new FormatException(
                                $"Invalid mapping CSV format at row {rowNumber}");

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
                                    $"SerialNumber missing at row {rowNumber}")
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