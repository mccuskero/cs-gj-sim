# HEUSS - Human Energy Usage Simulation System

**Part of the Global Joules Platform**

HEUSS simulates continuous energy usage and production patterns of human populations using a hierarchical model (Individual → Region → Nation). The system tracks both **biological energy** (caloric intake) and **technological energy** (electricity, fuel, thermal) in unified joules to enable comprehensive energy consumption analysis.

## 🎯 Key Capabilities

- **Real-time distributed simulation** using Microsoft Orleans actor model
- **Scalable** from thousands to millions of concurrent entities
- **Microservice architecture** with gRPC/REST APIs
- **ML-based predictive analytics** for energy trends
- **Cross-platform deployment** via Docker containers
- **Energy inequality analysis** across socioeconomic levels, regions, and nations

## 🏗️ Architecture

**Language:** C# (.NET 9)
**Paradigm:** Object-Oriented + Microservice (Actor Model)
**Runtime:** Microsoft Orleans 9.2
**Simulation Type:** Continuous temporal, time-stepped energy flow modeling

### Domain Model

```
Nation
  └─ Region (State, Province, County)
      ├─ Person (biological + optional technological energy)
      ├─ House (residential energy)
      ├─ Vehicle (transportation energy)
      ├─ Business (commercial energy)
      ├─ DataCenter (high-density energy)
      └─ Farm (energy consumer + food energy producer)
```

### Socioeconomic Archetypes

HEUSS models **6 socioeconomic archetypes** to capture energy inequality:

| Archetype | Energy/Day | Biological % | Technological % | Characteristics |
|-----------|------------|--------------|-----------------|-----------------|
| **Subsistence** | 6-12 MJ | 100% | 0% | No house, no vehicle, no electricity |
| **Low Income** | 12-30 MJ | 80% | 20% | Basic shelter, limited electricity |
| **Lower-Middle** | 30-80 MJ | 40% | 60% | Small home, may own motorcycle |
| **Middle** | 80-200 MJ | 20% | 80% | House + car, 24h electricity |
| **Upper-Middle** | 200-400 MJ | 10% | 90% | Large house, 1-2 cars, full HVAC |
| **Affluent** | 400-1000+ MJ | 5% | 95% | Multiple properties, 2-3+ vehicles |

**Energy Inequality Ratio:** Developed regions consume **37.5x** more energy per capita than subsistence regions.

## 🚀 Quick Start

### Prerequisites

- **.NET 9 SDK** (or .NET 8)
- **Docker** and **Docker Compose** (optional, for PostgreSQL/Redis)
- **Git**

### Setup

```bash
# Clone repository
git clone https://github.com/your-org/gj-sim.git
cd gj-sim

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test
```

### Running the Simulation System (Phase 2)

**Step 1: Start the Orleans Silo (in a separate terminal)**

```bash
# Terminal 1: Start the Orleans distributed actor system
dotnet run --project src/HEUSS.Services/SimulationEngine
```

You should see:
```
✓ Orleans Silo started successfully
Dashboard listening on 8080
Press Enter to terminate...
```

**Access the Orleans Dashboard:** http://localhost:8080

**Step 2: Use the CLI Tool to Query Energy Data**

```bash
# Terminal 2: Run CLI queries (uses mock data in Phase 2)
dotnet run --project src/HEUSS.CLI -- --help
dotnet run --project src/HEUSS.CLI -- nation USA
dotnet run --project src/HEUSS.CLI -- nation KEN
dotnet run --project src/HEUSS.CLI -- global

# Export to JSON
dotnet run --project src/HEUSS.CLI -- nation USA --format json --output usa-report.json

# Export to CSV
dotnet run --project src/HEUSS.CLI -- global --format csv --output global-report.csv
```

**CLI Options:**
- `--format, -f` - Output format (Table, Json, Csv) - default: Table
- `--output, -o` - Save to file instead of console
- `--version, -v` - Show version information
- `--help, -h` - Show help

### Example Output

```bash
$ dotnet run --project src/HEUSS.CLI -- nation USA
```

Generates a beautiful table with:
- National energy totals (99.3 PJ/day)
- Per-capita consumption (300 MJ/person/day)
- Entity breakdown (houses, vehicles, businesses, etc.)
- Regional variations
- Biological vs. technological energy split

### Environment Configuration

Copy `.env.example` to `.env` and configure:

```bash
cp docker/.env.example docker/.env
```

## 📂 Project Structure

```
/src
  /HEUSS.Core               # Domain models, interfaces, enums, utilities
    /Models
      - Nation.cs
      - Region.cs
      /Entities             # Person, Vehicle, House, Business, DataCenter, Farm
    /Interfaces
      /Grains               # Orleans grain interfaces
    /Enums                  # SocioeconomicArchetype, DevelopmentLevel, ClimateType
    /Utilities              # EnergyConversion utilities

  /HEUSS.CLI                # ✨ Command-line interface (NEW!)
    /Commands               # Command handlers
    /Services               # Simulation runner
    /Formatters             # Table, JSON, CSV formatters
    /Models                 # Report models
    Program.cs              # CLI entry point
    README.md               # CLI documentation

  /HEUSS.Simulation         # Orleans grains and simulation logic
    /Grains
      /EntityGrains
    /OrleansConfig
    /Streams

  /HEUSS.Services           # Microservices
    /SimulationEngine       # Orleans silo host
    /EnergyAnalytics        # Analytics and ML
    /FoodEnergy             # Food/caloric calculations
    /DataGateway            # External API

  /HEUSS.Tests              # Unit and integration tests
    /UnitTests

/config
  /archetypes               # YAML archetype configurations (6 levels)

/docker                     # Docker Compose and configs
/docs                       # Documentation
```

## 🔬 Energy Conversion

All energy is measured in **joules** (SI base unit):

```csharp
using HEUSS.Core.Utilities;

// Food energy
double joules = EnergyConversion.KilocaloriesToJoules(2000); // 2000 kcal → 8,368,000 J

// Electrical energy
double joules = EnergyConversion.KWhToJoules(10); // 10 kWh → 36,000,000 J

// Vehicle fuel
double joules = EnergyConversion.GallonsGasolineToJoules(1); // 1 gallon → 130,000,000 J

// Format for display
string formatted = EnergyConversion.FormatEnergy(150_000_000); // "150.00 MJ"
```

## 🧪 Running Tests

**Test Framework:** xUnit 2.8.2
**Coverage:** ✅ **88.88%** line coverage (49 tests, 100% passing)

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter FullyQualifiedName~PersonEntityTests

# Run with coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Exclude="[*.Tests]*"
```

**Coverage Breakdown:**
- Line Coverage: 88.88%
- Branch Coverage: 70.21%
- Method Coverage: 87.5%

See [TEST_COVERAGE_REPORT.md](TEST_COVERAGE_REPORT.md) for detailed coverage analysis.

## 📊 Archetype Configuration

Archetypes are defined in YAML files in `/config/archetypes/`:

```yaml
# subsistence.yaml
name: Subsistence
level: 1
energy_profile:
  total_daily_joules_min: 6000000
  total_daily_joules_max: 12000000
  biological_percentage: 100
  technological_percentage: 0

infrastructure:
  has_electricity: false
  has_running_water: false

assets:
  owns_house: false
  owns_vehicle: false
```

## 🖥️ CLI Tool

**NEW:** Comprehensive command-line interface for running simulations!

```bash
# Run global simulation
dotnet run --project src/HEUSS.CLI -- global

# Run nation simulation with JSON output
dotnet run --project src/HEUSS.CLI -- nation USA --format json

# Run region simulation and save to file
dotnet run --project src/HEUSS.CLI -- region KEN Nairobi -o report.csv -f csv
```

**Features:**
- ✅ Three scopes: Global, Nation, Region
- ✅ Three formats: Table (default), JSON, CSV
- ✅ File output support
- ✅ Beautiful console tables (Spectre.Console)
- ✅ Structured logging (Serilog)
- ✅ Comprehensive help system

See [CLI_COMPLETE.md](CLI_COMPLETE.md) and [src/HEUSS.CLI/README.md](src/HEUSS.CLI/README.md) for full documentation.

---

## 🗺️ Roadmap

### ✅ Phase 1: Foundation (Complete!)
- [x] Core domain models
- [x] Energy conversion utilities
- [x] Socioeconomic archetypes (6 levels)
- [x] Orleans grain interfaces
- [x] Unit tests (88.88% coverage, 49 tests)
- [x] Docker Compose infrastructure
- [x] **CLI tool with mock data** ✨

### ✅ Phase 2: Orleans + Basic Simulation (Complete!)
- [x] Implement Orleans grains (PersonGrain, HouseGrain, VehicleGrain, BusinessGrain, DataCenterGrain, FarmGrain)
- [x] Implement RegionGrain and NationGrain with aggregation
- [x] Implement TemporalCoordinatorGrain with Reminders
- [x] Orleans Persistent Streams setup (8 queues, EnergyStreamProvider)
- [x] Grain state persistence (in-memory for Phase 2)
- [x] Orleans Silo host configured and tested
- [x] Orleans Dashboard integration (port 8080)
- [x] **CLI --version flag** ✨
- [x] **Working end-to-end demo** ✨

**See [PHASE2_PROGRESS.md](PHASE2_PROGRESS.md) for detailed Phase 2 completion report.**

### 🚧 Phase 3: Orleans Client & Integration
- [ ] Create Orleans client library for external connections
- [ ] Update CLI to use Orleans client (connect to running silo)
- [ ] Create grain population service (seed initial data)
- [ ] Implement real-time tick execution via TemporalCoordinatorGrain
- [ ] Create integration tests (entity → region → nation aggregation)
- [ ] Build test scenarios (SmallTown, MetroArea)
- [ ] Validate energy flow through streaming hierarchy

### 📅 Phase 4: Real-World Data & Advanced Modeling
- [ ] Climate/weather integration
- [ ] Entity ownership relationships
- [ ] Real-world data integration (World Bank, Census, EIA)
- [ ] Archetype calibration from GDP data
- [ ] Populate 1 US state from real data

### 📅 Phase 5: Analytics & Services
- [ ] EnergyAnalyticsService with CQRS projections
- [ ] Comparative analysis APIs (region/nation energy inequality)
- [ ] FoodEnergyService for caloric calculations
- [ ] DataGatewayService with REST/gRPC APIs
- [ ] Blazor dashboard with inequality visualizations
- [ ] ML.NET predictive modeling

### 📅 Phase 6: Production Readiness
- [ ] Kafka/Azure Queue Storage for streams
- [ ] PostgreSQL grain persistence
- [ ] Kubernetes deployment
- [ ] Logging and monitoring
- [ ] Load testing (100K+ entities)
- [ ] CI/CD pipelines

## 📖 Documentation

- **[CLAUDE.md](./CLAUDE.md)** - Complete system specification
- **[PHASE1_FINAL_STATUS.md](./PHASE1_FINAL_STATUS.md)** - Phase 1 completion report
- **[PHASE2_PROGRESS.md](./PHASE2_PROGRESS.md)** - Phase 2 completion report ✨ NEW!
- **[USA_ENERGY_DEMO.md](./USA_ENERGY_DEMO.md)** - CLI demo results for United States
- **[TEST_COVERAGE_REPORT.md](./TEST_COVERAGE_REPORT.md)** - Unit test coverage (88.88%)
- **[CLI_COMPLETE.md](./CLI_COMPLETE.md)** - CLI tool documentation
- **[EnergyCalculations.md](./docs/EnergyCalculations.md)** - Energy formulas and examples (coming soon)
- **[GrainLifecycle.md](./docs/GrainLifecycle.md)** - Orleans grain management (coming soon)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

[Add your license here]

## 🙏 Acknowledgments

- **Microsoft Orleans** - Virtual actor framework
- **World Bank Open Data** - Global development indicators
- **US Energy Information Administration (EIA)** - State energy data
- **Global Joules** - Energy inequality research platform

---

**Current Status:** Phase 2 Complete - Orleans Grains Implemented ✅
**Orleans Silo:** Running with 9 grain types, streaming, and dashboard
**CLI Tool:** Operational with mock data (Phase 2) - Orleans client integration coming in Phase 3
**Next Milestone:** Create Orleans client and connect CLI to live grains

**What's Working Right Now:**
- ✅ Start Orleans silo: `dotnet run --project src/HEUSS.Services/SimulationEngine`
- ✅ Access dashboard: http://localhost:8080
- ✅ Query energy data: `dotnet run --project src/HEUSS.CLI -- nation USA`
- ✅ Export reports: `--format json --output report.json`
