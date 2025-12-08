# Cardio Tennis Score Tracker - Requirements

## Product Vision
A simple, offline-capable Progressive Web App for tennis coaches to track scores during Cardio Tennis sessions with multiple courts and rotating players.

## User Story
As a **tennis coach/facilitator**, I want to quickly track player scores across multiple rotating matches on multiple courts so that I can focus on coaching while keeping the session organized and fun.

---

## Functional Requirements

### FR-1: Session Management

#### FR-1.1: Create New Session
- **Given** no session is in progress
- **When** coach taps "New Session"
- **Then** coach is prompted to configure:
  - Number of courts (1+)
  - Total number of players
- **Validation**: 4-8 players per court
- **System Action**: Initialize session in localStorage

#### FR-1.2: Active Session State
- **Given** a session is in progress
- **When** coach opens the app
- **Then** "New Session" button is disabled/hidden
- **And** current match state is displayed

#### FR-1.3: End Session
- **Given** a session is in progress
- **When** coach ends the session
- **Then** final scores are displayed
- **And** session data is cleared from active state
- **Optional**: Archive completed session to localStorage history

---

### FR-2: Player Management

#### FR-2.1: Player Count-Off (MVP)
- **Given** session is created
- **When** coach starts first match
- **Then** app displays instruction: "Have players count off and remember their numbers"
- **System Action**: Create player records numbered 1 to N

#### FR-2.2: Player Scoring
- **Given** players are assigned to matches
- **When** a team wins a match
- **Then** each player on that team receives +1 point
- **And** cumulative scores are updated

#### FR-2.3: View Player Scores
- **Given** any time during session
- **When** coach views score screen
- **Then** all players are listed with their current scores
- **Sorting**: Default by score (descending), with player number as tiebreaker

---

### FR-3: Match Management

#### FR-3.1: Round Structure (3-Match Rounds)
- **Core Rule**: Teams stay together for **3 consecutive matches** (a "round")
- After 3 matches, new teams are formed
- Courts are ranked: Court 1 (highest/winner court) → Court N (lowest/loser court)

#### FR-3.2: First Match Assignment (Round 1, Match 1)
- **Given** session is configured
- **When** first match starts
- **Then** system randomly assigns players to:
  - Courts (numbered 1 to N)
  - Teams (Team A and Team B per court)
- **Balancing Rule**: 
  - Players distributed evenly across courts
  - Teams on each court differ by at most 1 player
  - Example: 7 players on a court → Team A (4), Team B (3)

#### FR-3.3: Record Match Results
- **Given** match is in progress
- **When** coach views court list
- **Then** each court shows:
  - Court number
  - Match number within round (1/3, 2/3, or 3/3)
  - Team A players (by number)
  - Team B players (by number)
  - Selectable buttons: "Team A Wins" | "Team B Wins"
- **When** coach selects winning team
- **Then** match result is recorded
- **And** points awarded to winning players

#### FR-3.4: Court Rotation ("Up and Down the Ladder")
- **Applies to**: Matches 2 and 3 within each round
- **Single Court**: Teams stay on same court (no rotation)
- **Multiple Courts**:
  - **Match 1 → Match 2**:
    - Winning teams move UP (toward Court 1)
    - Losing teams move DOWN (toward Court N)
    - Teams switch courts to play winners vs. winners, losers vs. losers
  - **Match 2 → Match 3**:
    - Same rotation logic applies
    - Winners continue moving up, losers continue moving down
  
**Example (2 Courts):**
```
Match 1:
  Court 1: Team A vs Team B → Team A wins
  Court 2: Team C vs Team D → Team C wins

Match 2:
  Court 1: Team A vs Team C (both won Match 1)
  Court 2: Team B vs Team D (both lost Match 1)

Match 3:
  Court 1: Winner of Match 2 Court 1 vs Winner of Match 2 Court 2
  Court 2: Loser of Match 2 Court 1 vs Loser of Match 2 Court 2
```

**Edge Cases:**
- **Court 1 Winners**: Already at top, stay at Court 1
- **Court N Losers**: Already at bottom, stay at Court N
- **Odd number of teams**: System handles unbalanced rotations

#### FR-3.5: New Round Assignment (After 3 Matches)
- **Given** 3 matches completed in current round
- **When** coach starts next match
- **Then** system forms new teams
- **Algorithm Goal (MVP)**: Random with aim to mix teams
- **Future**: Smart algorithm based on:
  - Who has played together
  - Win/loss balancing
  - Session length (matches per hour)

#### FR-3.6: Match History
- **Given** multiple matches played
- **When** coach views history
- **Then** list shows:
  - Round number
  - Match number within round
  - Court assignments
  - Results
  - Scores after that match

---

### FR-4: Data Persistence

#### FR-4.1: Auto-Save
- **Given** any state change (match result, session config)
- **When** change occurs
- **Then** state is immediately saved to localStorage

#### FR-4.2: Resume Session
- **Given** session in progress
- **When** coach closes/reopens browser
- **Then** session state is restored
- **And** coach can continue from last match

#### FR-4.3: Storage Keys
- Use consistent naming: `cardiotennis:session`, `cardiotennis:players`, `cardiotennis:matches`

---

## Non-Functional Requirements

### NFR-1: Performance
- Match result recording: < 100ms response time
- Session load from localStorage: < 200ms

### NFR-2: Offline Support
- App works completely offline after initial install
- Service worker caches all assets
- No network requests during normal operation

### NFR-3: Usability
- Touch-friendly UI (minimum 44x44px touch targets)
- Large, clear text (coaches use phones/tablets courtside)
- High contrast colors for outdoor visibility
- Minimal taps to accomplish tasks

### NFR-4: Accessibility
- Semantic HTML
- ARIA labels for interactive elements
- Keyboard navigable
- Color contrast ratios meet WCAG AA

### NFR-5: Browser Support
- Modern browsers: Chrome, Edge, Safari, Firefox
- Mobile: iOS Safari, Chrome Android
- PWA installable on iOS and Android

### NFR-6: Data Limits
- Handle localStorage quota errors gracefully
- Session data should be minimal (< 1MB typical)

---

## Out of Scope (Future Features)

### Phase 2
- Add players mid-session (late arrivals)
- Player names (not just numbers)
- Smart team balancing algorithm
- Session duration tracking
- Session history/archives

### Phase 3
- Multiple session profiles
- Export results (CSV, PDF)
- Statistics (who played with whom, win rates)
- Preset player lists
- Custom scoring rules

---

## Technical Constraints

- .NET 10, Blazor WebAssembly
- No backend services
- No user authentication
- Static deployment (Azure Static Web Apps, GitHub Pages, etc.)
- Browser localStorage only

---

## User Interface Screens (MVP)

1. **Home/Start Screen**
   - "New Session" button
   - (Future: "Resume Session", "View History")

2. **Session Setup Screen**
   - Number of courts (numeric input)
   - Number of players (numeric input)
   - Validation message
   - "Start Session" button

3. **Match Screen (Active)**
   - Round number and match number (e.g., "Round 2, Match 2/3")
   - For each court:
     - Court number
     - Team A players (by number)
     - Team B players (by number)
     - Selectable buttons: "Team A Wins" | "Team B Wins"
   - "View Scores" button
   - "End Session" button

4. **Scores Screen**
   - Player list with scores
   - Sort options
   - "Back to Match" button
   - "End Session" button

5. **End Session Screen**
   - Final scores
   - "Start New Session" button

---

## Questions / Decisions Needed

1. **Team Assignment Algorithm**: MVP is random, but future needs smart balancing. Should we plan the interface now?
2. **Session Length Input**: Needed for future algorithm, include in MVP setup?
3. **Court Count Max**: Any practical upper limit?
4. **Match Count**: Fixed number or continue until coach ends session?
5. **Tie-breaking**: If final scores tied, display order or special indicator?
6. **Court Rotation Edge Cases**: With odd number of courts, how do middle courts rotate? (e.g., 3 courts, Court 2 winner/loser movement)

---

## Success Criteria

- Coach can run a 1-hour session with 2 courts and 12 players
- All scores are accurately tracked
- Court rotation ("up and down the ladder") works correctly for 3-match rounds
- App works offline throughout session
- Coach can complete match result entry in < 5 seconds per court
- No data loss if browser is accidentally closed

---

**Document Version**: 1.1  
**Last Updated**: 2025-12-06  
**Status**: Draft for Review
