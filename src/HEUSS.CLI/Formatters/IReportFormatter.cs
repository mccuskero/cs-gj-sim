using HEUSS.CLI.Models;

namespace HEUSS.CLI.Formatters;

/// <summary>
/// Interface for formatting energy reports
/// </summary>
public interface IReportFormatter
{
    /// <summary>
    /// Format an energy report for output
    /// </summary>
    string Format(EnergyReport report);

    /// <summary>
    /// Format multiple reports (for global scope)
    /// </summary>
    string FormatMultiple(List<EnergyReport> reports);
}
