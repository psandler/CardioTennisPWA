# Agent Instructions

This file provides guidance for GitHub Copilot and other AI assistants working on the Cardio Tennis Score Tracker.

## Critical Rules

### Git Operations
**NO GIT ACTIONS BY AGENTS.** Only humans perform git operations (commit, push, branch, merge, tag, PR creation).
- Agents may propose changes but must provide exact git commands for humans to execute
- Never attempt automated commits or repository modifications

### Project Context

- **Framework**: .NET 10, Blazor WebAssembly
- **App Type**: Progressive Web App (PWA)
- **Architecture**: Client-side only, static deployment
- **Data Storage**: Browser localStorage only (no backend, no database)
- **Scope**: Simple scoring tracker for Cardio Tennis game

## Code Standards

### Formatting
- Follow `.editorconfig` rules exactly (will be created as preferences are detected)
- Match existing code style in the project
- Use consistent naming conventions for Blazor components

### Project Structure
```
/CardioTennisPWA
  /Components       - Blazor components (.razor files)
  /Services        - Business logic (scoring, localStorage adapter)
  /Models          - Data models and view models
  /wwwroot         - Static assets, PWA manifest, service worker
```

### Blazor Conventions
- One component per `.razor` file
- Use code-behind (`.razor.cs`) for complex component logic
- Prefer `@code` blocks for simple component state
- Use dependency injection for services
- Follow Blazor naming: `ComponentName.razor`, `IServiceName`, `ServiceName`

### localStorage Usage
- All data persists to browser localStorage
- Use a consistent key naming scheme: `cardiotennis:*`
- Encapsulate localStorage access in a dedicated service
- Handle serialization/deserialization (JSON)
- Gracefully handle quota exceeded errors

## PWA Requirements

- Maintain `manifest.json` with proper icons and metadata
- Keep service worker (`service-worker.js`) functional for offline support
- Test offline functionality before proposing changes
- Ensure static deployment compatibility (no server-side rendering)

## Testing Expectations

- Unit tests for scoring logic and services
- Provide manual test steps for UI changes
- Test localStorage edge cases (quota, clear, corruption)

## Accessibility

- Use semantic HTML elements
- Provide ARIA labels where needed
- Ensure keyboard navigation works
- Maintain color contrast ratios

## When Proposing Changes

1. Show full file contents or complete diffs
2. Explain what changed and why
3. List any required Visual Studio settings changes
4. Provide exact git commands for human to execute (but don't run them)
5. Include test scenarios if applicable

## Game Domain (Cardio Tennis)

Details about scoring rules will be added as the human explains the game mechanics. Agents should reference this section when implementing scoring logic.

---

**Last Updated**: 2025-12-06  
**Applies To**: GitHub Copilot, automated code generation, CI/CD agents
