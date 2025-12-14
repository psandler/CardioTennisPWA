# ?? Cardio Tennis Score Tracker

A Progressive Web App (PWA) built with Blazor WebAssembly for tracking scores during Cardio Tennis sessions.

## Current Status: Hello World + localStorage Demo

This is a working Blazor PWA with:
- ? Blazor WebAssembly (.NET 10)
- ? PWA support (installable, offline-capable)
- ? localStorage service implementation
- ? GitHub Pages deployment configured

## Quick Start

### Run Locally
```bash
dotnet run --project CardioTennisPWA
```

Visit: http://localhost:5000

### Deploy to GitHub Pages
See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed instructions.

**TL;DR:**
1. Push changes to `main` branch
2. Enable GitHub Pages in repository settings (Source: GitHub Actions)
3. Visit: https://psandler.github.io/CardioTennisPWA/

## Project Structure

```
CardioTennisPWA/
??? Pages/
?   ??? Home.razor              # localStorage demo page
??? Services/
?   ??? ILocalStorageService.cs # localStorage abstraction
?   ??? LocalStorageService.cs  # localStorage implementation
??? wwwroot/
?   ??? index.html              # PWA shell
?   ??? manifest.webmanifest    # PWA manifest
?   ??? service-worker.js       # Development service worker
?   ??? service-worker.published.js # Production service worker
??? Program.cs                  # App entry point + DI setup
```

## Features

### Current (Hello World)
- localStorage CRUD operations
- PWA installation support
- Offline capability
- Bootstrap UI

### Planned (See [REQUIREMENTS.md](REQUIREMENTS.md))
- Session management
- Player tracking
- Match scoring
- Court rotation ("up and down the ladder")
- Team assignment algorithm

## Documentation

- **[agents.md](agents.md)** - Instructions for Copilot and AI assistants
- **[REQUIREMENTS.md](REQUIREMENTS.md)** - Detailed feature requirements
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Deployment guide

## Technology Stack

- **Framework**: .NET 10, Blazor WebAssembly
- **UI**: Bootstrap 5
- **Storage**: Browser localStorage
- **Deployment**: GitHub Pages (static)
- **CI/CD**: GitHub Actions

## Development Guidelines

See [agents.md](agents.md) for:
- Code standards
- Project structure
- localStorage key naming (`cardiotennis:*`)
- PWA best practices

## License

This project is for personal use.

---

**Version**: 0.1.0 (Hello World)  
**Last Updated**: 2025-12-06
