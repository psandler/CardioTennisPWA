# MVP TODO List

## Current Status: ~80% Complete

### ? Completed Features

**Core Infrastructure:**
- ? Blazor WebAssembly PWA setup
- ? localStorage service implementation
- ? CQRS pattern (Query/Command services)
- ? Data models (GameSession, MatchSet, Match, Court, Team, Player)
- ? GitHub Pages deployment configured

**Session Management:**
- ? Create new game session
- ? Configure courts (1-9) and players (4-8 per court)
- ? Player numbering instructions modal
- ? Session state persisted to localStorage

**Team Assignment:**
- ? Initial random team generation
- ? Remix teams (first match only)
- ? Skill-based team reassignment (snake draft by score)
- ? Balanced team sizing (max 1 player difference)

**Match Management:**
- ? Display courts with team assignments
- ? Record match results (Team A/B Wins buttons)
- ? Award points to winning players
- ? Auto-mark match complete when all courts done
- ? Continue with current teams (creates next match)
- ? "Up and down the ladder" court rotation
- ? Re-assign teams (creates new set with skill balancing)

**UI/UX:**
- ? Mobile-first design
- ? Set tabs (left sidebar)
- ? Match accordion (expandable)
- ? Court cards with team display
- ? Winner highlighting (green border/background)
- ? Post-match action buttons
- ? Large touch targets for mobile

---

## ?? Remaining MVP Features (20%)

### High Priority (Must Have for MVP)

**1. Player Scores Display** ?? ~2 hours
- [ ] Create scores view/modal
- [ ] Show all players with current scores
- [ ] Sort by score (descending), then player number
- [ ] Accessible from any screen during session
- [ ] **Files to modify:**
  - Create `Components/PlayerScores.razor` + `.razor.cs`
  - Add "View Scores" button to SetView
  - Implement `GetPlayerStandingsAsync()` query

**2. End Session Functionality** ?? ~2 hours
- [ ] "End Session" button accessible at all times
- [ ] Confirmation dialog before ending
- [ ] Mark session as inactive
- [ ] Show final scores screen
- [ ] Clear active session (or keep for history)
- [ ] Return to home screen
- [ ] **Files to modify:**
  - Update `SetView.razor` with End Session button
  - Create `Components/FinalScores.razor`
  - Implement `EndGameSessionAsync()` logic
  - Update `Home.razor.cs` to check for active session

**3. Session Resume on App Load** ?? ~1 hour
- [ ] Check localStorage for active session on app start
- [ ] If found, load and display SetView automatically
- [ ] If not found, show "New Session" button
- [ ] **Files to modify:**
  - Update `Home.razor.cs` `OnInitializedAsync()`
  - Call `QueryService.GetCurrentGameSessionAsync()`
  - Conditionally show SetView vs New Session button

**4. Error Handling & Validation** ?? ~3 hours
- [ ] localStorage quota exceeded handling
- [ ] Invalid state recovery (corrupted data)
- [ ] Network error handling (PWA offline)
- [ ] Validate match state before actions
- [ ] User-friendly error messages
- [ ] **Files to modify:**
  - Wrap localStorage calls in try/catch
  - Add error state to components
  - Create error toast/alert component

### Medium Priority (Nice to Have for MVP)

**5. Set Completion Logic** ?? ~1 hour
- [ ] Mark set as complete when re-assigning teams
- [ ] Show completion badge on set tabs
- [ ] Disable "Continue" after reasonable match count (optional)
- [ ] **Files to modify:**
  - Update `ReassignTeamsAsync()` to mark current set complete
  - Update SetView to show completion status

**6. UI Polish** ?? ~2 hours
- [ ] Loading spinners for async operations
- [ ] Smooth transitions between states
- [ ] Improved mobile navigation
- [ ] Better visual feedback for button clicks
- [ ] Consistent spacing and typography
- [ ] **Files to modify:**
  - Add loading states to components
  - Create CSS animations
  - Update `MainLayout.razor.css`

**7. Court Rotation Edge Cases** ?? ~2 hours
- [ ] Test and fix rotation with odd number of courts
- [ ] Handle courts with unequal players
- [ ] Verify winner/loser pairing logic
- [ ] Add unit tests for rotation algorithm
- [ ] **Files to modify:**
  - Review `ContinueWithCurrentTeamsAsync()` logic
  - Add edge case handling
  - Create test scenarios

### Low Priority (Post-MVP)

**8. Session History**
- [ ] Save completed sessions
- [ ] View past session results
- [ ] Export/share functionality

**9. Advanced Features**
- [ ] Player names (not just numbers)
- [ ] Add players mid-session
- [ ] Custom scoring rules
- [ ] Session duration tracking
- [ ] Match timer

**10. PWA Enhancements**
- [ ] App icons (multiple sizes)
- [ ] Splash screens
- [ ] Offline page
- [ ] Install prompts
- [ ] Push notifications (optional)

---

## ?? Immediate Next Steps (This Week)

### Priority 1: Core Gameplay Loop
1. ? ~~Implement skill-based team reassignment~~
2. ? ~~Fix accordion to allow viewing completed matches~~
3. **Implement Player Scores Display**
4. **Implement End Session Functionality**
5. **Implement Session Resume**

### Priority 2: Stability
6. **Add error handling throughout**
7. **Test edge cases (odd courts, unequal teams)**
8. **Mark sets as complete**

### Priority 3: Polish
9. **Add loading states**
10. **UI improvements**
11. **Documentation updates**

---

## ?? MVP Definition of Done

**A coach can:**
1. ? Create a new session with multiple courts
2. ? Have players count off and remember numbers
3. ? See initial team assignments (and remix if desired)
4. ? Record match results easily
5. ? Continue with same teams OR reassign (skill-balanced)
6. ? View team rotations ("up and down ladder")
7. ? **View current player scores at any time**
8. ? **End session and see final scores**
9. ? **Resume session if app is closed/refreshed**
10. ? All functionality works offline (PWA)
11. ? Works well on mobile phone screen

**Technical Requirements:**
- ? No backend/database needed
- ? All data in localStorage
- ? Static deployment (GitHub Pages)
- ? Mobile-first responsive design
- ? Error handling for edge cases
- ? Clean CQRS architecture

---

## ?? Estimated Time to MVP Complete

| Category | Estimated Hours |
|----------|----------------|
| Player Scores Display | 2 |
| End Session | 2 |
| Session Resume | 1 |
| Error Handling | 3 |
| Set Completion | 1 |
| UI Polish | 2 |
| Testing & Bug Fixes | 3 |
| **Total** | **14 hours** |

**Target MVP Date:** ~2-3 days of focused development

---

## ?? Known Issues

1. ? ~~Team Wins buttons not working~~ - **FIXED** (localStorage object identity issue)
2. ? ~~Can't expand completed matches~~ - **FIXED** (removed accordion parent restriction)
3. ?? **No player scores visible during game** - HIGH PRIORITY
4. ?? **No way to end session** - HIGH PRIORITY
5. ?? **Session doesn't resume on refresh** - HIGH PRIORITY
6. ?? **No error handling for localStorage quota** - MEDIUM PRIORITY
7. ?? **Court rotation untested with odd courts** - MEDIUM PRIORITY

---

## ?? Notes

- Focus on **player scores** and **end session** first - these are critical for usability
- **Session resume** is important for PWA experience
- Error handling can be basic for MVP but should be present
- UI polish can wait until after core features work
- Test on actual mobile device before declaring MVP done

---

**Last Updated:** 2024-12-14  
**Status:** In Active Development  
**Version:** 0.8.0-alpha
