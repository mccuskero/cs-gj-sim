# HEUSS CLI - Command-Line Interface

**Human Energy Usage Simulation System - CLI Tool**

The HEUSS CLI provides a powerful command-line interface for running energy simulations and generating comprehensive reports at global, national, or regional scopes.

---

## 🚀 Quick Start

### Installation

```bash
# Build the CLI tool
dotnet build src/HEUSS.CLI

# Or build in release mode
dotnet build src/HEUSS.CLI -c Release
```

### Basic Usage

```bash
# Show help
dotnet run --project src/HEUSS.CLI -- --help

# Run global simulation
dotnet run --project src/HEUSS.CLI -- global

# Run nation simulation
dotnet run --project src/HEUSS.CLI -- nation USA

# Run region simulation
dotnet run --project src/HEUSS.CLI -- region USA California
```

---

## 📖 Commands

### 1. Global Simulation

Run simulation for all nations worldwide.

```bash
dotnet run --project src/HEUSS.CLI -- global [OPTIONS]
```

**Example:**
```bash
# Table output (default)
dotnet run --project src/HEUSS.CLI -- global

# JSON output
dotnet run --project src/HEUSS.CLI -- global --format json

# CSV output to file
dotnet run --project src/HEUSS.CLI -- global --format csv --output global-report.csv
```

**Output:**
```
╭──────────────────────────────────────────────────╮
│           Global Energy Report                   │
├──────────┬──────┬────────────┬─────────────┬──────┤
│ Nation   │ Code │ Population │ Total/Day   │ Per Cap│
├──────────┼──────┼────────────┼─────────────┼──────┤
│ USA      │ USA  │ 331,000,000│ 99.30 PJ    │300.00 MJ│
│ India    │ IND  │1,400,000,000│ 77.00 PJ   │ 55.00 MJ│
│ Brazil   │ BRA  │ 215,000,000│ 30.10 PJ    │140.00 MJ│
│ Japan    │ JPN  │ 125,000,000│ 37.50 PJ    │300.00 MJ│
│ Kenya    │ KEN  │  54,000,000│  1.13 PJ    │ 21.00 MJ│
├──────────┼──────┼────────────┼─────────────┼──────┤
│ TOTAL    │      │2,125,000,000│245.03 PJ   │115.31 MJ│
╰──────────┴──────┴────────────┴─────────────┴──────╯
```

---

### 2. Nation Simulation

Run simulation for a specific nation.

```bash
dotnet run --project src/HEUSS.CLI -- nation <COUNTRY-CODE> [OPTIONS]
```

**Arguments:**
- `country-code` - ISO 3166-1 alpha-3 country code (USA, KEN, IND, JPN, BRA, etc.)

**Examples:**
```bash
# United States
dotnet run --project src/HEUSS.CLI -- nation USA

# Kenya
dotnet run --project src/HEUSS.CLI -- nation KEN

# With JSON output
dotnet run --project src/HEUSS.CLI -- nation IND --format json

# Save to file
dotnet run --project src/HEUSS.CLI -- nation USA --output usa-report.json --format json
```

**Output:**
```
╭────────────────────────────────────────╮
│      United States Energy Report       │
├──────────────────┬─────────────────────┤
│ Metric           │ Value               │
├──────────────────┼─────────────────────┤
│ Scope            │ Nation              │
│ Country Code     │ USA                 │
│ Population       │ 331,000,000         │
│ Total Daily Energy│ 99.30 PJ           │
│ Per Capita Daily │ 300.00 MJ           │
│ Generated At     │ 2025-10-25 21:00:00 │
╰──────────────────┴─────────────────────╯

╭─────────────────────────────────────────────────╮
│         Entity Energy Breakdown                 │
├──────────────┬──────┬──────────────┬────────────┤
│ Entity Type  │ Count│ Total Energy │ % of Total │
├──────────────┼──────┼──────────────┼────────────┤
│ 👤 Persons   │331M  │ 3.31 PJ      │ 3.3%       │
│ 🏠 Houses    │298M  │ 39.72 PJ     │ 40.0%      │
│ 🚗 Vehicles  │497M  │ 29.79 PJ     │ 30.0%      │
│ 🏢 Businesses│ 6.6M │ 19.86 PJ     │ 20.0%      │
│ 🖥️  Data Cntrs│3,310│  4.97 PJ     │  5.0%      │
│ 🚜 Farms     │331K  │  2.98 PJ     │  3.0%      │
├──────────────┼──────┼──────────────┼────────────┤
│ Biological   │      │ 3.31 PJ      │ 10.0%      │
│ Technological│      │ 95.99 PJ     │ 90.0%      │
╰──────────────┴──────┴──────────────┴────────────╯

╭─────────────────────────────────────────────────╮
│            Regional Breakdown                   │
├───────────┬────────────┬──────────────┬─────────┤
│ Region    │ Population │ Total Energy │ Per Cap │
├───────────┼────────────┼──────────────┼─────────┤
│ Region B  │110,333,333 │ 39.72 PJ     │360.00 MJ│
│ Region A  │110,333,333 │ 33.10 PJ     │300.00 MJ│
│ Region C  │110,333,333 │ 26.48 PJ     │240.00 MJ│
╰───────────┴────────────┴──────────────┴─────────╯
```

---

### 3. Region Simulation

Run simulation for a specific region within a nation.

```bash
dotnet run --project src/HEUSS.CLI -- region <COUNTRY-CODE> <REGION-NAME> [OPTIONS]
```

**Arguments:**
- `country-code` - ISO 3166-1 alpha-3 country code
- `region-name` - Region name (e.g., California, Nairobi, Maharashtra)

**Examples:**
```bash
# California, USA
dotnet run --project src/HEUSS.CLI -- region USA California

# Nairobi, Kenya
dotnet run --project src/HEUSS.CLI -- region KEN Nairobi

# With CSV output
dotnet run --project src/HEUSS.CLI -- region USA California --format csv --output california.csv
```

---

## ⚙️ Options

### Format Options (`--format` or `-f`)

Specify the output format for reports.

| Format | Description | Use Case |
|--------|-------------|----------|
| `table` | Console tables (default) | Interactive viewing |
| `json` | JSON format | API integration, data processing |
| `csv` | Comma-separated values | Spreadsheet import, data analysis |
| `markdown` | Markdown tables | Documentation, reports |

**Examples:**
```bash
# Table format (default)
dotnet run --project src/HEUSS.CLI -- nation USA

# JSON format
dotnet run --project src/HEUSS.CLI -- nation USA --format json

# CSV format
dotnet run --project src/HEUSS.CLI -- global -f csv
```

### Output Options (`--output` or `-o`)

Save report to a file instead of printing to console.

**Examples:**
```bash
# Save JSON to file
dotnet run --project src/HEUSS.CLI -- nation USA --format json --output usa-report.json

# Save CSV to file
dotnet run --project src/HEUSS.CLI -- global -f csv -o global-energy.csv

# Save table to text file
dotnet run --project src/HEUSS.CLI -- region USA California -o california.txt
```

---

## 📊 Report Contents

### Nation/Region Reports Include:

1. **Summary Statistics**
   - Name and country code
   - Population
   - Total daily energy consumption (joules)
   - Per capita daily energy consumption (joules)
   - Generation timestamp

2. **Entity Breakdown**
   - Person count and biological energy
   - House count and residential energy
   - Vehicle count and transportation energy
   - Business count and commercial energy
   - Data center count and IT infrastructure energy
   - Farm count, operational energy, and food production

3. **Energy Classification**
   - Biological energy total and percentage
   - Technological energy total and percentage

4. **Regional Breakdown** (Nation reports only)
   - Regional populations
   - Regional energy totals
   - Regional per capita energy

---

## 📝 Logging

All CLI operations are logged to:

- **Console:** Real-time output with color-coding
- **File:** `logs/heuss-cli-YYYY-MM-DD.log` (rolling daily logs)

**Log Levels:**
- `Information` - Normal operations
- `Error` - Failures and exceptions
- `Fatal` - Critical errors causing termination

---

## 🛠️ Development

### Building

```bash
# Debug build
dotnet build src/HEUSS.CLI

# Release build
dotnet build src/HEUSS.CLI -c Release

# Publish as single executable
dotnet publish src/HEUSS.CLI -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true
```

### Testing

```bash
# Run tests
dotnet test

# Test CLI commands
dotnet run --project src/HEUSS.CLI -- --help
dotnet run --project src/HEUSS.CLI -- global --format json
```

---

## 🎯 Use Cases

### 1. Quick Energy Assessment
```bash
# See global energy distribution
dotnet run --project src/HEUSS.CLI -- global
```

### 2. Country Comparison
```bash
# Compare multiple nations
dotnet run --project src/HEUSS.CLI -- nation USA --format json > usa.json
dotnet run --project src/HEUSS.CLI -- nation KEN --format json > kenya.json
# Then analyze the JSON files
```

### 3. Data Export for Analysis
```bash
# Export to CSV for Excel/Python analysis
dotnet run --project src/HEUSS.CLI -- global --format csv --output global-energy.csv
```

### 4. Regional Deep Dive
```bash
# Analyze specific regions
dotnet run --project src/HEUSS.CLI -- region USA California
dotnet run --project src/HEUSS.CLI -- region USA Texas
dotnet run --project src/HEUSS.CLI -- region USA "New York"
```

---

## 🔧 Troubleshooting

### Command Not Found
```bash
# Make sure you're in the project root directory
cd /path/to/gj-sim
dotnet run --project src/HEUSS.CLI -- global
```

### Nation Not Found
```bash
# Use ISO 3166-1 alpha-3 codes
# Correct: USA, KEN, IND, JPN, BRA
# Incorrect: US, Kenya, India

dotnet run --project src/HEUSS.CLI -- nation USA  # ✓
dotnet run --project src/HEUSS.CLI -- nation US   # ✗
```

### Permission Denied (Logs)
```bash
# Ensure logs directory exists
mkdir -p logs
```

---

## 📚 References

- **System.CommandLine:** Modern command-line parsing
- **Spectre.Console:** Beautiful console tables and formatting
- **Serilog:** Structured logging framework

---

## 🚀 Future Enhancements (Phase 2)

Once Orleans grains are implemented, the CLI will:

- ✅ Connect to live Orleans cluster
- ✅ Run actual distributed simulations
- ✅ Support real-time simulation monitoring
- ✅ Add historical data queries
- ✅ Support custom date ranges
- ✅ Add comparison commands (before/after, nation vs nation)
- ✅ Support watch mode for continuous monitoring

---

**Status:** Phase 1 - Mock Data Implementation
**Next Phase:** Integration with Orleans grains and real simulation data
