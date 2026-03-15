# Fun Clicker — AGENTS.md

## 1. Project Overview

**Fun Clicker** is a Unity 2D / UI-based clicker-idle game focused on responsive tapping, combo-based rewards, visual feedback, skins, and fixed-card upgrades.

This file is the working handoff / project context document for future chats and collaborators.

---

## 2. Current Design Direction

The project is currently built around these MVP pillars:

1. **Tap to Score**

   * Player taps a UI click area.
   * Each tap grants score.
   * Tap feedback includes DOTween scale punch and floating score popup.

2. **Combo System**

   * A vertical combo bar fills as the player clicks repeatedly.
   * The bar drains over time.
   * Combo thresholds:

     * below 50%: `x1`
     * 50% to below 75%: `x2`
     * 75% and above: `x4`
   * Combo multiplier affects actual tap reward, but does **not** change the base SPC stat.

3. **Score Display & Stats**

   * Main score is displayed in real time.
   * SPC (Score Per Click) is the **base click value**, not the combo-multiplied reward.
   * SPS (Score Per Second) exists as a stat and UI field already, but real-time SPS income is not implemented yet.

4. **Skin System by Combo State**

   * Each skin contains:

     * 3 character sprites: x1 / x2 / x4
     * 3 background sprites: x1 / x2 / x4
   * Character and background swap automatically based on the active combo multiplier.
   * Odin Inspector is used for better data authoring in ScriptableObjects.

5. **Upgrade System Direction**

   * Upgrade cards are fixed-definition items.
   * Each upgrade has:

     * type: PPC or SPS
     * icon
     * name
     * fixed cost
     * fixed bonus amount
     * purchase count
   * Buying an upgrade increases player stats, but the card’s cost and bonus remain unchanged.
   * Example:

     * cost `50`, bonus `+1 score/click`
     * bought multiple times -> still `50`, still `+1`, only purchase count increases.

---

## 3. Systems Already Implemented / In Progress

### Implemented or mostly implemented

* Click area via UI panel raycast.
* Tap adds score.
* DOTween click feedback on main visual.
* Floating `+score` popup on tap.
* Score text updates in real time.
* Combo logic manager with thresholds and decay.
* Combo UI bar and multiplier label.
* Base SPC separated from combo-multiplied tap reward.
* SkinSO direction defined for x1/x2/x4 character + background states.

### Implemented conceptually but not finished end-to-end

* SPS stat exists in data/UI.
* Upgrade model and shop direction are defined.
* Skin switching runtime controller direction is defined.

---

## 4. Important Gameplay Rules

### Score / Tap Rules

* `BaseScorePerClick` is the player’s permanent click stat.
* Final tap reward = `BaseScorePerClick * ComboMultiplier`.
* SPC UI must always show the **base** value only.
* Combo must **not** overwrite base SPC.

### Combo Rules

* Combo increases on click.
* Combo decreases continuously over time.
* Multiplier thresholds:

  * x1 below half bar
  * x2 at half bar
  * x4 at three-quarter bar

### SPS Rules

* `ScorePerSecond` is a persistent stat.
* It will later be used by an idle income loop.
* Current work stores and displays SPS, but does not yet grant live score over time.

### Upgrade Rules

* No infinite scaling button with growing cost.
* Instead: repeated purchases of fixed upgrade cards.
* Each card keeps:

  * fixed cost
  * fixed bonus
  * incrementing purchase count

---

## 5. Current Technical Architecture

### Core Runtime Managers

* **PointManager**

  * Holds current score.
  * Holds base SPC.
  * Holds SPS.
  * Raises UI update events.
  * Should eventually support spending score cleanly.

* **ComboManager**

  * Stores combo fill value.
  * Handles combo decay.
  * Resolves active multiplier (x1/x2/x4).
  * Raises combo and multiplier change events.

* **UpgradeManager**

  * Will hold upgrade definitions and runtime purchase counts.
  * Applies PPC / SPS bonuses to PointManager.

### UI / Input

* **ClickAreaController**

  * Receives pointer input from UI panel.
  * Adds score using combo multiplier.
  * Plays tap animation.
  * Spawns floating popup.

* **PointTextView**

  * Displays main score.

* **PointStatsView**

  * Displays base SPC and SPS.

* **ComboBarView**

  * Shows combo fill and multiplier state.

* **FloatingScorePopup / Spawner**

  * Visual tap reward feedback.
  * Designed for pooling / repeated spawning.

* **SkinVisualController**

  * Will apply character/background sprites based on current skin + combo multiplier.

### Data Assets

* **SkinSO**

  * Stores combo-state sprites for character and background.

* **UpgradeSO**

  * Stores fixed upgrade data:

    * type
    * cost
    * bonus
    * icon
    * display name

---

## 6. Unity / Tools Conventions

* Engine: **Unity**
* Language: **C#**
* Main layer: **2D UI-driven gameplay**
* Tweening: **DOTween**
* Inspector tooling: **Odin Inspector**
* Pooling: intended to use **PoolBoss** where useful for repeated popup visuals

---

## 7. Current UI Direction

The current game screen includes:

* Main clickable character area
* Score text
* SPC / SPS text
* Left-side combo bar
* Upgrade button / popup panel
* Skin button / settings button

Upgrade popup style direction:

* Scrollable card list
* Each item shows:

  * icon
  * name
  * bonus description
  * cost
  * purchase count

---

## 8. Things To Build Next

Priority order:

### Next Priority

1. **Complete Upgrade Data + Runtime Purchase Flow**

   * Create UpgradeSO assets for PPC and SPS cards.
   * Create UpgradeManager runtime count handling.
   * Allow repeated purchase with fixed cost/fixed bonus.

2. **Upgrade UI**

   * Card prefab
   * Scroll list binding
   * Cost / bonus / count refresh
   * Buy button state

3. **Real SPS Income Loop**

   * Apply `ScorePerSecond` to current score over time.
   * Ensure UI updates correctly.

### After That

4. **Skin Runtime Integration**

   * Plug SkinSO into runtime visuals.
   * Update background + character on combo state change.

5. **Save / Load**

   * Current score
   * base SPC
   * SPS
   * upgrade purchase counts
   * selected skin

6. **Number Formatting**

   * Better display for large values.

7. **Offline Progress**

   * Real-time delta based reward.

---

## 9. Important Design Constraints

* Keep systems modular and event-driven where possible.
* Do not mix base stat logic with combo multiplier logic.
* Prefer fixed upgrade-card definitions over dynamic cost-scaling for current design.
* Prioritize gameplay logic and code architecture over art polish.
* UI feedback should feel immediate and readable.

---

## 10. Known Open Questions / Future Decisions

* Final number formatting strategy for large values.
* Save format and persistence layer.
* Whether SPS should tick continuously or in fixed intervals.
* Whether combo should later affect SPS, skills, or only tap reward.
* Whether upgrades should later unlock based on milestones.

---

## 11. Short Summary

Fun Clicker currently revolves around:

* tap score
* combo multiplier bar
* responsive popup feedback
* base SPC/SPS stat tracking
* combo-driven skin changes
* fixed repeated upgrade cards for PPC and SPS

The immediate next milestone is to finish the **upgrade system and upgrade popup flow**, then connect **real SPS generation**, then move to **save/load** and broader idle features.
