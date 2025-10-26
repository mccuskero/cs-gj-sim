using HEUSS.CLI.Formatters;
using HEUSS.CLI.Models;
using HEUSS.CLI.Services;
using Serilog;
using Spectre.Console;

namespace HEUSS.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/heuss-cli-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            return await ProcessCommandAsync(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    static async Task<int> ProcessCommandAsync(string[] args)
    {
        if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
        {
            ShowHelp();
            return 0;
        }

        if (args[0] == "--version" || args[0] == "-v")
        {
            ShowVersion();
            return 0;
        }

        var command = args[0].ToLower();
        var format = GetFormat(args);
        var outputFile = GetOutputFile(args);

        // For Phase 2, use mock data
        // TODO: Implement Orleans client connection in Phase 3
        var runner = new SimulationRunner(Log.Logger);

        switch (command)
        {
            case "global":
                return await HandleGlobalAsync(runner, format, outputFile);

            case "nation" when args.Length >= 2:
                return await HandleNationAsync(runner, args[1], format, outputFile);

            case "region" when args.Length >= 3:
                return await HandleRegionAsync(runner, args[1], args[2], format, outputFile);

            default:
                AnsiConsole.MarkupLine("[red]Invalid command. Use --help for usage information.[/]");
                return 1;
        }
    }

    static async Task<int> HandleGlobalAsync(SimulationRunner runner, ReportFormat format, string? outputFile)
    {
        AnsiConsole.MarkupLine("[bold green]Running Global Simulation...[/]");
        AnsiConsole.WriteLine();

        var reports = await runner.RunGlobalAsync();
        var formatter = GetFormatter(format);
        var output = formatter.FormatMultiple(reports);

        await WriteOutputAsync(output, outputFile);

        AnsiConsole.MarkupLine($"[bold green]✓[/] Simulation complete. Processed {reports.Count} nations.");
        return 0;
    }

    static async Task<int> HandleNationAsync(SimulationRunner runner, string countryCode, ReportFormat format, string? outputFile)
    {
        AnsiConsole.MarkupLine($"[bold green]Running Simulation for Nation: {countryCode.ToUpper()}[/]");
        AnsiConsole.WriteLine();

        var report = await runner.RunNationAsync(countryCode);
        var formatter = GetFormatter(format);
        var output = formatter.Format(report);

        await WriteOutputAsync(output, outputFile);

        AnsiConsole.MarkupLine($"[bold green]✓[/] Simulation complete for {report.Name}");
        return 0;
    }

    static async Task<int> HandleRegionAsync(SimulationRunner runner, string countryCode, string regionName, ReportFormat format, string? outputFile)
    {
        AnsiConsole.MarkupLine($"[bold green]Running Simulation for Region: {regionName}, {countryCode.ToUpper()}[/]");
        AnsiConsole.WriteLine();

        var report = await runner.RunRegionAsync(countryCode, regionName);
        var formatter = GetFormatter(format);
        var output = formatter.Format(report);

        await WriteOutputAsync(output, outputFile);

        AnsiConsole.MarkupLine($"[bold green]✓[/] Simulation complete for {report.Name}");
        return 0;
    }

    static ReportFormat GetFormat(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--format" || args[i] == "-f")
            {
                return Enum.TryParse<ReportFormat>(args[i + 1], true, out var format)
                    ? format
                    : ReportFormat.Table;
            }
        }
        return ReportFormat.Table;
    }

    static string? GetOutputFile(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--output" || args[i] == "-o")
            {
                return args[i + 1];
            }
        }
        return null;
    }

    static bool GetUseOrleans(string[] args)
    {
        return args.Contains("--orleans") || args.Contains("--live");
    }

    static IReportFormatter GetFormatter(ReportFormat format)
    {
        return format switch
        {
            ReportFormat.Json => new JsonFormatter(),
            ReportFormat.Csv => new CsvFormatter(),
            ReportFormat.Table => new TableFormatter(),
            _ => new TableFormatter()
        };
    }

    static async Task WriteOutputAsync(string content, string? outputFile)
    {
        if (outputFile != null)
        {
            var directory = Path.GetDirectoryName(outputFile);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await File.WriteAllTextAsync(outputFile, content);
            AnsiConsole.MarkupLine($"[dim]Output written to: {outputFile}[/]");
        }
        else if (!content.Contains("[bold"))
        {
            Console.WriteLine(content);
        }
    }

    static void ShowVersion()
    {
        var version = typeof(Program).Assembly.GetName().Version;
        var infoVersion = typeof(Program).Assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .FirstOrDefault() as System.Reflection.AssemblyInformationalVersionAttribute;

        AnsiConsole.Write(
            new FigletText("HEUSS CLI")
                .LeftJustified()
                .Color(Color.Green));

        AnsiConsole.MarkupLine("[bold]Human Energy Usage Simulation System - CLI Tool[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]Version:[/] {infoVersion?.InformationalVersion ?? version?.ToString() ?? "Unknown"}");
        AnsiConsole.MarkupLine($"[green].NET Runtime:[/] {Environment.Version}");
        AnsiConsole.MarkupLine($"[green]Platform:[/] {Environment.OSVersion}");
        AnsiConsole.WriteLine();
    }

    static void ShowHelp()
    {
        AnsiConsole.Write(
            new FigletText("HEUSS CLI")
                .LeftJustified()
                .Color(Color.Green));

        AnsiConsole.MarkupLine("[bold]Human Energy Usage Simulation System - CLI Tool[/]");
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Command[/]")
            .AddColumn("[bold]Description[/]")
            .AddColumn("[bold]Example[/]");

        table.AddRow(
            "[green]global[/]",
            "Run simulation for all nations",
            "[dim]heuss global[/]");

        table.AddRow(
            "[green]nation <CODE>[/]",
            "Run simulation for a specific nation",
            "[dim]heuss nation USA[/]");

        table.AddRow(
            "[green]region <CODE> <NAME>[/]",
            "Run simulation for a specific region",
            "[dim]heuss region USA California[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold]Options:[/]");
        AnsiConsole.MarkupLine("  [green]--format, -f[/]     Output format (Table, Json, Csv) [dim](default: Table)[/]");
        AnsiConsole.MarkupLine("  [green]--output, -o[/]     Output file path [dim](default: console)[/]");
        AnsiConsole.MarkupLine("  [green]--orleans, --live[/] Use live Orleans grains [dim](requires running silo)[/]");
        AnsiConsole.MarkupLine("  [green]--version, -v[/]    Show version information");
        AnsiConsole.MarkupLine("  [green]--help, -h[/]       Show this help message");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]$ dotnet run --project src/HEUSS.CLI -- global[/]");
        AnsiConsole.MarkupLine("  [dim]$ dotnet run --project src/HEUSS.CLI -- nation USA --format json[/]");
        AnsiConsole.MarkupLine("  [dim]$ dotnet run --project src/HEUSS.CLI -- region KEN Nairobi -o report.csv -f csv[/]");
    }
}
