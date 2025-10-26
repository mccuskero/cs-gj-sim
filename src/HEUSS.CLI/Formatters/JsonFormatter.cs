using HEUSS.CLI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HEUSS.CLI.Formatters;

/// <summary>
/// Formats energy reports as JSON
/// </summary>
public class JsonFormatter : IReportFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public string Format(EnergyReport report)
    {
        return JsonSerializer.Serialize(report, Options);
    }

    public string FormatMultiple(List<EnergyReport> reports)
    {
        var wrapper = new
        {
            Scope = "Global",
            TotalNations = reports.Count,
            TotalPopulation = reports.Sum(r => r.Population),
            TotalDailyEnergyJoules = reports.Sum(r => r.TotalDailyEnergyJoules),
            AveragePerCapitaDailyEnergyJoules = reports.Sum(r => r.TotalDailyEnergyJoules) / reports.Sum(r => r.Population),
            GeneratedAt = DateTime.UtcNow,
            Nations = reports
        };

        return JsonSerializer.Serialize(wrapper, Options);
    }
}
