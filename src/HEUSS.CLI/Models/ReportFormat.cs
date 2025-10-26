namespace HEUSS.CLI.Models;

/// <summary>
/// Output format for energy reports
/// </summary>
public enum ReportFormat
{
    /// <summary>
    /// Console table output (default)
    /// </summary>
    Table,

    /// <summary>
    /// JSON format
    /// </summary>
    Json,

    /// <summary>
    /// CSV format for spreadsheet import
    /// </summary>
    Csv,

    /// <summary>
    /// Markdown table format
    /// </summary>
    Markdown
}
