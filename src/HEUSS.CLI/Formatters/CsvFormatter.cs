using HEUSS.CLI.Models;
using System.Text;

namespace HEUSS.CLI.Formatters;

/// <summary>
/// Formats energy reports as CSV
/// </summary>
public class CsvFormatter : IReportFormatter
{
    public string Format(EnergyReport report)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Name,CountryCode,Population,TotalDailyEnergyJoules,PerCapitaDailyEnergyJoules," +
                      "PersonCount,PersonEnergy,HouseCount,HouseEnergy,VehicleCount,VehicleEnergy," +
                      "BusinessCount,BusinessEnergy,DataCenterCount,DataCenterEnergy,FarmCount,FarmEnergy");

        // Data row
        var e = report.Entities;
        csv.AppendLine($"{Escape(report.Name)},{report.CountryCode},{report.Population}," +
                      $"{report.TotalDailyEnergyJoules},{report.PerCapitaDailyEnergyJoules}," +
                      $"{e.PersonCount},{e.PersonEnergyJoules}," +
                      $"{e.HouseCount},{e.HouseEnergyJoules}," +
                      $"{e.VehicleCount},{e.VehicleEnergyJoules}," +
                      $"{e.BusinessCount},{e.BusinessEnergyJoules}," +
                      $"{e.DataCenterCount},{e.DataCenterEnergyJoules}," +
                      $"{e.FarmCount},{e.FarmEnergyJoules}");

        return csv.ToString();
    }

    public string FormatMultiple(List<EnergyReport> reports)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Name,CountryCode,Population,TotalDailyEnergyJoules,PerCapitaDailyEnergyJoules," +
                      "PersonCount,PersonEnergy,HouseCount,HouseEnergy,VehicleCount,VehicleEnergy," +
                      "BusinessCount,BusinessEnergy,DataCenterCount,DataCenterEnergy,FarmCount,FarmEnergy");

        // Data rows
        foreach (var report in reports)
        {
            var e = report.Entities;
            csv.AppendLine($"{Escape(report.Name)},{report.CountryCode},{report.Population}," +
                          $"{report.TotalDailyEnergyJoules},{report.PerCapitaDailyEnergyJoules}," +
                          $"{e.PersonCount},{e.PersonEnergyJoules}," +
                          $"{e.HouseCount},{e.HouseEnergyJoules}," +
                          $"{e.VehicleCount},{e.VehicleEnergyJoules}," +
                          $"{e.BusinessCount},{e.BusinessEnergyJoules}," +
                          $"{e.DataCenterCount},{e.DataCenterEnergyJoules}," +
                          $"{e.FarmCount},{e.FarmEnergyJoules}");
        }

        return csv.ToString();
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
