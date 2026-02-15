# Cardio Tennis Score Tracker

A Progressive Web App (PWA) for tracking scores during Cardio Tennis sessions. Built with Blazor WebAssembly, designed for coaches to manage multi-court sessions on mobile devices.

## What is Cardio Tennis?

Cardio Tennis is a fun, high-energy group fitness activity that combines tennis with cardiovascular exercise. Players rotate through matches on multiple courts, with teams changing periodically to ensure everyone plays together.

## Live Demo

**Deployed App:** https://psandler.github.io/CardioTennisPWA/

## Features

### Core Features
- **Multi-Court Management** - Support for 1-9 courts
- **Flexible Player Counts** - 4-8 players per court
- **Smart Team Assignment**
  - Initial random assignment with balancing
  - Remix option for first match
  - Skill-based reassignment (snake draft by score)
- **Match Scoring** - Simple tap to record winners
- **Court Rotation** - "Up and down the ladder" system
- **Set Management** - Unlimited matches per set
- **Player Scores** - View current standings anytime
- **Session Management** - End session and view final scores
- **Session Resume** - Automatically resume interrupted sessions
- **Offline First** - Works without internet (PWA)
- **Mobile Optimized** - Phone-first design
- **Help System** - In-app instructions and PWA installation guide

### Planned Future Features
- **Player Names** - Assign names to player numbers
- **Export Results** - Export to CSV/Excel
- **Add/Remove Players** - Modify player count during active session
- **Score Corrections** - Fix mistakes after recording
- **Session History** - View and review past sessions
- **Custom Court Names** - Label courts (e.g., "Red Court", "Blue Court")
- **Match Duration Tracking** - Record how long each match takes

## Architecture

**Framework:** .NET 10, Blazor WebAssembly  
**Storage:** Browser localStorage only (no backend)  
**Deployment:** Static files on GitHub Pages  
**Pattern:** CQRS (Command/Query Separation)

### Project Structure
```
CardioTennisPWA/
??? Components/         # Blazor components (SetView, HelpModal, etc.)
??? Models/            # Data models (GameSession, Match, Player, etc.)
??? Pages/             # Routable pages (Home)
??? Services/          # Business logic
?   ??? IGameSessionQueryService    # Read operations
?   ??? IGameSessionCommandService  # Write operations
?   ??? ILocalStorageService        # Storage abstraction
??? wwwroot/           # Static assets, PWA manifest
??? docs/              # Documentation
```

## How It Works

### Session Flow

1. **Create Session**
   - Coach selects number of courts and players
   - Players count off and remember their numbers

2. **First Match Setup**
   - App generates balanced team assignments
   - Coach can remix teams if desired (first match only)

3. **Play Matches**
   - Coach records match results (tap winning team)
   - Points awarded to winning players

4. **Court Rotation**
   - Winners move "up" toward Court 1
   - Losers move "down" toward last court
   - Winners play winners, losers play losers

5. **Team Reassignment**
   - After each match, coach can continue OR reassign
   - Reassignment uses skill-based balancing (snake draft by current score)
   - New set created with fresh teams

6. **End Session**
   - View final scores ranked by points
   - Session marked as complete in localStorage

## Development

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026, Visual Studio 2022, VS Code, or Rider

### Local Development

```bash
# Clone repository
git clone https://github.com/psandler/CardioTennisPWA.git
cd CardioTennisPWA

# Run locally
dotnet run --project CardioTennisPWA

# Build for production
dotnet publish CardioTennisPWA/CardioTennisPWA.csproj -c Release
```

**Local URL:** http://localhost:5263

### Testing PWA Locally

1. Build in Release mode
2. Serve from `bin/Release/net10.0/publish/wwwroot/`
3. Use browser DevTools > Application > Service Workers

## Deployment

Automatic deployment via GitHub Actions on push to `main`:
- Builds Blazor WebAssembly app
- Publishes to GitHub Pages
- Configures service worker for offline support

See [.github/workflows/deploy.yml](.github/workflows/deploy.yml)

## PWA Features

- **Offline Support** - Service worker caches app for offline use
- **Installable** - Add to home screen on mobile devices
- **Responsive Design** - Optimized for phone, tablet, and desktop
- **localStorage Persistence** - Sessions saved automatically
- **App Icons** - Custom icons for all platforms

## Documentation

- [agents.md](agents.md) - AI assistant instructions
- [HumanNotes.md](HumanNotes.md) - Development notes
- [docs/REQUIREMENTS.md](docs/REQUIREMENTS.md) - Detailed requirements
- [docs/CQRS-IMPLEMENTATION.md](docs/CQRS-IMPLEMENTATION.md) - Architecture details
- [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) - Deployment configuration

## Contributing

This is a personal project. Contributions are welcome!

**Design Principles:**
- Mobile-first design
- No backend/database required
- Works offline
- Simple and fast
- Touch-friendly interface

## License

MIT License - see [LICENSE](LICENSE) for details.

## Acknowledgments

Built for tennis coaches who need a simple, reliable scoring tool during active play.

---

**Version:** 1.0.0  
**Status:** Complete (MVP)

