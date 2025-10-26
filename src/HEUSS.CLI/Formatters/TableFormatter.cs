using HEUSS.CLI.Models;
using HEUSS.Core.Utilities;
using Spectre.Console;
using System.Text;

namespace HEUSS.CLI.Formatters;

/// <summary>
/// Formats energy reports as console tables using Spectre.Console
/// </summary>
public class TableFormatter : IReportFormatter
{
    public string Format(EnergyReport report)
    {
        var output = new StringBuilder();

        // Create main summary table
        var summaryTable = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[bold yellow]{report.Name} Energy Report[/]")
            .AddColumn("[bold]Metric[/]")
            .AddColumn("[bold]Value[/]");

        summaryTable.AddRow("Scope", report.Scope.ToString());
        summaryTable.AddRow("Country Code", report.CountryCode);
        summaryTable.AddRow("Population", report.Population.ToString("N0"));
        summaryTable.AddRow("Total Daily Energy", EnergyConversion.FormatEnergy(report.TotalDailyEnergyJoules));
        summaryTable.AddRow("Per Capita Daily Energy", EnergyConversion.FormatEnergy(report.PerCapitaDailyEnergyJoules));
        summaryTable.AddRow("Generated At", report.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"));

        AnsiConsole.Write(summaryTable);
        output.AppendLine(summaryTable.ToString());

        // Entity breakdown table
        var entityTable = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Entity Energy Breakdown[/]")
            .AddColumn("[bold]Entity Type[/]")
            .AddColumn("[bold]Count[/]", c => c.RightAligned())
            .AddColumn("[bold]Total Energy[/]", c => c.RightAligned())
            .AddColumn("[bold]% of Total[/]", c => c.RightAligned());

        var entities = report.Entities;
        var total = report.TotalDailyEnergyJoules;

        AddEntityRow(entityTable, "👤 Persons", entities.PersonCount, entities.PersonEnergyJoules, total);
        AddEntityRow(entityTable, "🏠 Houses", entities.HouseCount, entities.HouseEnergyJoules, total);
        AddEntityRow(entityTable, "🚗 Vehicles", entities.VehicleCount, entities.VehicleEnergyJoules, total);
        AddEntityRow(entityTable, "🏢 Businesses", entities.BusinessCount, entities.BusinessEnergyJoules, total);
        AddEntityRow(entityTable, "🖥️  Data Centers", entities.DataCenterCount, entities.DataCenterEnergyJoules, total);
        AddEntityRow(entityTable, "🚜 Farms", entities.FarmCount, entities.FarmEnergyJoules, total);

        entityTable.AddEmptyRow();
        entityTable.AddRow(
            "[bold]Biological Energy[/]",
            "",
            $"[green]{EnergyConversion.FormatEnergy(entities.TotalBiologicalEnergyJoules)}[/]",
            $"[green]{(entities.TotalBiologicalEnergyJoules / total * 100):F1}%[/]"
        );
        entityTable.AddRow(
            "[bold]Technological Energy[/]",
            "",
            $"[blue]{EnergyConversion.FormatEnergy(entities.TotalTechnologicalEnergyJoules)}[/]",
            $"[blue]{(entities.TotalTechnologicalEnergyJoules / total * 100):F1}%[/]"
        );

        AnsiConsole.Write(entityTable);
        output.AppendLine(entityTable.ToString());

        // Regional breakdown if available
        if (report.Regions.Any())
        {
            var regionTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold magenta]Regional Breakdown[/]")
                .AddColumn("[bold]Region[/]")
                .AddColumn("[bold]Population[/]", c => c.RightAligned())
                .AddColumn("[bold]Total Energy[/]", c => c.RightAligned())
                .AddColumn("[bold]Per Capita Energy[/]", c => c.RightAligned());

            foreach (var region in report.Regions.OrderByDescending(r => r.TotalDailyEnergyJoules))
            {
                regionTable.AddRow(
                    region.RegionName,
                    region.Population.ToString("N0"),
                    EnergyConversion.FormatEnergy(region.TotalDailyEnergyJoules),
                    EnergyConversion.FormatEnergy(region.PerCapitaDailyEnergyJoules)
                );
            }

            AnsiConsole.Write(regionTable);
            output.AppendLine(regionTable.ToString());
        }

        return output.ToString();
    }

    public string FormatMultiple(List<EnergyReport> reports)
    {
        var output = new StringBuilder();

        // Global summary table
        var globalTable = new Table()
            .Border(TableBorder.Double)
            .Title("[bold yellow]Global Energy Report[/]")
            .AddColumn("[bold]Nation[/]")
            .AddColumn("[bold]Code[/]")
            .AddColumn("[bold]Population[/]", c => c.RightAligned())
            .AddColumn("[bold]Total Energy/Day[/]", c => c.RightAligned())
            .AddColumn("[bold]Per Capita Energy/Day[/]", c => c.RightAligned());

        foreach (var report in reports.OrderByDescending(r => r.TotalDailyEnergyJoules))
        {
            globalTable.AddRow(
                report.Name,
                report.CountryCode,
                report.Population.ToString("N0"),
                EnergyConversion.FormatEnergy(report.TotalDailyEnergyJoules),
                EnergyConversion.FormatEnergy(report.PerCapitaDailyEnergyJoules)
            );
        }

        // Add totals row
        var totalPop = reports.Sum(r => r.Population);
        var totalEnergy = reports.Sum(r => r.TotalDailyEnergyJoules);
        var avgPerCapita = totalEnergy / totalPop;

        globalTable.AddEmptyRow();
        globalTable.AddRow(
            "[bold]GLOBAL TOTAL[/]",
            "",
            $"[bold]{totalPop:N0}[/]",
            $"[bold]{EnergyConversion.FormatEnergy(totalEnergy)}[/]",
            $"[bold]{EnergyConversion.FormatEnergy(avgPerCapita)}[/]"
        );

        AnsiConsole.Write(globalTable);
        output.AppendLine(globalTable.ToString());

        return output.ToString();
    }

    private static void AddEntityRow(Table table, string name, long count, double energy, double total)
    {
        if (count == 0 && energy == 0) return;

        var percentage = total > 0 ? (energy / total * 100) : 0;
        table.AddRow(
            name,
            count.ToString("N0"),
            EnergyConversion.FormatEnergy(energy),
            $"{percentage:F1}%"
        );
    }
}
