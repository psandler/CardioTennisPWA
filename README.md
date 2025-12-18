# Cardio Tennis Score Tracker

A Progressive Web App (PWA) for tracking scores during Cardio Tennis sessions. Built with Blazor WebAssembly, designed for coaches to manage multi-court sessions on mobile devices.

## ?? What is Cardio Tennis?

Cardio Tennis is a fun, high-energy group fitness activity that combines tennis with cardiovascular exercise. Players rotate through matches on multiple courts, with teams changing periodically to ensure everyone plays together.

## ?? Live Demo

**Deployed App:** https://psandler.github.io/CardioTennisPWA/

**Status:** ?? In Development (MVP ~80% complete)

## ? Features

### ? Implemented
- **Multi-Court Management** - Support for 1-9 courts
- **Flexible Player Counts** - 4-8 players per court
- **Smart Team Assignment**
  - Initial random assignment with balancing
  - Remix option for first match
  - Skill-based reassignment (snake draft by score)
- **Match Scoring** - Simple tap to record winners
- **Court Rotation** - "Up and down the ladder" system
- **Set Management** - Unlimited matches per set
- **Offline First** - Works without internet (PWA)
- **Mobile Optimized** - Phone-first design

### ?? In Progress
- Player scores display
- End session functionality
- Session resume on app reload

### ?? Planned (Post-MVP)
- Player names (not just numbers)
- Session history
- Add players mid-session
- Export results

## ??? Architecture

**Framework:** .NET 10, Blazor WebAssembly  
**Storage:** Browser localStorage only (no backend)  
**Deployment:** Static files on GitHub Pages  
**Pattern:** CQRS (Command/Query Separation)

### Project Structure
```
CardioTennisPWA/
??? Components/         # Blazor components (SetView, etc.)
??? Models/            # Data models (GameSession, Match, Player, etc.)
??? Pages/             # Routable pages (Home)
??? Services/          # Business logic
?   ??? IGameSessionQueryService    # Read operations
?   ??? IGameSessionCommandService  # Write operations
?   ??? ILocalStorageService        # Storage abstraction
??? wwwroot/           # Static assets, PWA manifest
```

## ?? How It Works

### Session Flow

1. **Create Session**
   - Coach selects number of courts and players
   - Players count off and remember their numbers

2. **Play Matches**
   - App generates balanced team assignments
   - Coach records match results (tap winning team)
   - Points awarded to winning players

3. **Court Rotation**
   - Winners move "up" toward Court 1
   - Losers move "down" toward last court
   - Winners play winners, losers play losers

4. **Team Reassignment**
   - After each match, coach can continue OR reassign
   - Reassignment uses skill-based balancing (snake draft)
   - New set created with fresh teams

5. **End Session**
   - View final scores
   - Session saved to localStorage

## ??? Development

### Prerequisites
- .NET 10 SDK (preview)
- Visual Studio 2022 (or VS Code)

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
3. Use browser DevTools ? Application ? Service Workers

## ?? Deployment

Automatic deployment via GitHub Actions on push to `main`:
- Builds Blazor WebAssembly app
- Publishes to GitHub Pages
- Configures service worker for offline support

See [.github/workflows/deploy.yml](.github/workflows/deploy.yml)

## ?? PWA Features

- ? Offline support (service worker)
- ? Installable on mobile devices
- ? Responsive design
- ? localStorage persistence
- ?? App icons and splash screens (in progress)

## ?? MVP Status

See [MVP-TODO.md](MVP-TODO.md) for detailed progress and remaining work.

**Completed:** ~80%  
**Target:** 2-3 days to MVP complete

### Critical Remaining Features
1. Player scores display
2. End session functionality
3. Session resume on reload

## ?? Documentation

- [agents.md](agents.md) - AI assistant instructions
- [REQUIREMENTS.md](REQUIREMENTS.md) - Detailed requirements
- [MVP-TODO.md](MVP-TODO.md) - Development roadmap
- [CQRS-IMPLEMENTATION.md](CQRS-IMPLEMENTATION.md) - Architecture details
- [LOCALSTORAGE-REFACTOR.md](LOCALSTORAGE-REFACTOR.md) - Data persistence approach

## ?? Contributing

This is a personal project in active development. Contributions welcome after MVP is complete.

**Rules:**
- Mobile-first design
- No backend/database
- Works offline
- Simple and fast

## ?? License

MIT License - See [LICENSE](LICENSE) file

## ?? Acknowledgments

Built for tennis coaches who need a simple, reliable scoring tool during active play.

---

**Version:** 0.8.0-alpha  
**Last Updated:** 2024-12-14  
**Status:** In Development
