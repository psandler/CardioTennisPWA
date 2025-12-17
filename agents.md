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
- **User Role**: Coach/facilitator (not a player), no authentication/login required

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

### Overview
Cardio Tennis is a fun, group tennis activity where players rotate through matches on multiple courts. The app tracks scores across matches and helps the facilitator manage team assignments and court rotations.

### User Persona
- **Coach/Facilitator**: Manages the session, not a player
- **No Authentication**: Everything runs locally in browser
- **Simple UI**: Quick interactions during active play

### Session Workflow

#### 1. New Session
- "New Session" button (disabled/hidden if session in progress)
- Input: Number of courts
- Input: Number of players (4-8 players per court)
- Players physically count off and remember their numbers

#### 2. Set Structure (Flexible Matches)
- **Core Rule**: Teams stay together for **1 or more consecutive matches** (a "set")
- **Flexible**: Coach decides after each match whether to continue or re-assign teams
- After set ends, new teams are formed
- Courts are ranked: **Court 1** (winner/top court) ? **Court N** (loser/bottom court)

#### 3. First Match Assignment (Round 1, Match 1)
- System randomly assigns players to:
  - Courts (numbered 1 to N)
  - Teams (within each court)
- **Balancing Rule**: Team sizes must differ by no more than 1 player
  - Example: 7 players on a court ? Team A (4 players), Team B (3 players)
- **Re-draw Option**: Coach can tap "Re-draw Teams" to regenerate assignments
  - Only available before first match starts
  - Can re-draw multiple times until satisfied

#### 4. Match Play & Scoring
- Display each court with its teams
- Show round and match number: "Round X, Match Y"
- Facilitator selects winning team for each court
- **Scoring**: Each player on winning team receives 1 point

#### 5. Post-Match Decision
**After each match**, coach chooses:
- **"Continue with Current Teams"**: Play another match in current set
  - Triggers court rotation if multiple courts
  - Can continue indefinitely until coach decides to re-assign
- **"Re-assign Teams"**: End current set, form new teams for next set

#### 6. Court Rotation ("Up and Down the Ladder")
**Applies when**: Coach chooses "Continue with Current Teams" (after any match in the set)

**Single Court:**
- Teams stay on same court (no rotation needed)
- Same teams play multiple matches in a row

**Multiple Courts:**
- **Winners move UP** (toward Court 1)
- **Losers move DOWN** (toward Court N)
- This creates: **Winners vs. Winners**, **Losers vs. Losers**

**Example with 2 Courts:**
```
Set 1, Match 1:
  Court 1: Team A vs Team B ? Team A wins
  Court 2: Team C vs Team D ? Team C wins

Coach chooses "Continue" ?

Set 1, Match 2:
  Court 1: Team A vs Team C (both won Match 1 - moved up)
  Court 2: Team B vs Team D (both lost Match 1 - moved down)

Coach chooses "Continue" again ?

Set 1, Match 3:
  Court 1: [Winner of Court 1 Match 2] vs [Winner of Court 2 Match 2]
  Court 2: [Loser of Court 1 Match 2] vs [Loser of Court 2 Match 2]

...and so on for as many matches as coach chooses
```

**Edge Cases:**
- Court 1 winners: Already at top, stay at Court 1
- Court N losers: Already at bottom, stay at Court N
- System handles these boundary conditions automatically

#### 7. New Set (After Set Ends)
- Coach chooses "Re-assign Teams"
- Display all player scores
- System forms new teams (randomized for MVP)
  - **Goal**: Mix teams so everyone plays with everyone equally
  - **OR**: Balance by win/loss for competitive matches
- Start next set with new team assignments

#### 8. End Session
- **"End Session" button** available at all times during session
- When tapped:
  - Display final scores for all players
  - Clear active session state
  - Return to home screen

### Scoring Rules (MVP)
- **Win = 1 point per player** on winning team
- **Loss = 0 points**
- Track cumulative score across all matches in session

### Future Features (Not MVP)
- Add players mid-session (late arrivals)
- Associate player names with numbers (before or during count-off)
- Advanced team-balancing algorithms
- Session history/statistics
- Session duration tracking (for matches-per-hour planning)

### Key Data Models
- **GameSession**: Courts count, total players, session status, current set number
- **MatchSet**: Set number, list of matches (1 to n), team assignments, status (active/completed)
- **Player**: Number, name (optional), total score
- **Match**: Match number within set, court assignments, team assignments, results
- **Court**: Court number, Team A players, Team B players, winning team
- **Team**: Players list

### UI/UX Principles
- **Target Platform**: Phone-sized screen (mobile-first design)
- **Primary Device**: Coach's smartphone used courtside
- Simple, touch-friendly interface with large tap targets
- Minimal taps to record match results
- Clear visual separation of courts and teams
- Quick access to current scores
- Show round/match progress clearly (e.g., "Round 2, Match 2")
- "End Session" always accessible
- Post-match decision flow is obvious and quick
- Responsive design: works on phones (primary), tablets, desktop (secondary)

### Critical UI Flows

**Initial Setup:**
1. New Session ? Configure courts/players ? Display initial teams
2. [Optional] Re-draw teams ? Display new teams
3. Start Match ? Record results ? Post-match decision

**Mid-Session (After Match):**
1. Record match results ? Post-match screen with scores
2. Choose: Continue (with current teams) OR Re-assign (new set)
3. If Continue: Rotate courts ? Next match in same set
4. If Re-assign: Form new teams ? Next set

**Anytime:**
- View Scores ? Return to current match
- End Session ? Final scores ? Home

---

**Last Updated**: 2025-12-06  
**Applies To**: GitHub Copilot, automated code generation, CI/CD agents
