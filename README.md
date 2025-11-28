# <img width="36" align="center" src="https://user-images.githubusercontent.com/32073152/126047438-2f1b7e59-ca31-43f2-bcf6-00b2f00e408c.png"/> Tarkov Price Viewer v1.35
This Overlay software is meant to help new players.
You can easily distinguish all the strengths and weaknesses of all the ammo in the game.
It is especially good to find out the market price in advance when the flea market is not open yet. 
Currently, only English is available.

Most of the information for this program (names, prices, wiki links, etc.) was provided by the Tarkov Market, and some information (Needs, etc.) was provided by the Tarkov Wiki.

## How to use
<img src="https://i.imgur.com/bluk1dQ.gif"/>

Hover the item and press the shortcut key (default: F9).
By default, the overlay disappears when you move the mouse, and this can be disabled in the settings.
It also disappears if you press the shortcut key again (default: F10).
</br>
</br>
<img src="https://user-images.githubusercontent.com/32073152/130593787-f0979114-46cf-47cb-93f3-2f364195e7e9.gif"/>

This program can also be used for comparing the price of parts in **Edit Preset**.
Since version 1.06 you can compare some options of items with the shortcut key (default: F8).

## Changelog

### v1.08 – Sorting & Ballistics

- Added sorting function for item lists.
- Added ballistics view for ammo.

<img src="https://i.imgur.com/iLxEsIc.png"/>

### v1.21 – Loot Tiers

- Added **Loot Tiers** for containers and locations.

<img src="https://i.imgur.com/9POHPgA.png"/>
<img src="https://i.imgur.com/Xru5QA5.png"/>
<img src="https://i.imgur.com/GKyoYLl.png"/>
<img src="https://i.imgur.com/Sn7DPlO.png"/>

### v1.23 – Item Class & Key Locations

- Added item Class (Armours, Helmets, etc.).
- Added key use locations.

<img src="https://i.imgur.com/Z58iBos.png"/>
<img src="https://i.imgur.com/Y4hGpaj.png"/>
<img src="https://i.imgur.com/2ZvATWX.png"/>

### v1.24 – Hideout Upgrade Info

- Added info on items that are needed for **Hideout Upgrades**, based on TarkovTracker progress.

<img src="https://i.imgur.com/QemMxGG.png"/>

### v1.25 – Barters & Crafts

- Re-added **Barters & Crafts**.

<img src="https://i.imgur.com/EsfSOkv.png"/>

### v1.28 – Language & Game Modes

- Added language settings: `en`, `ko`, `jp`, `cn`.
- Added support for game modes: regular (PvP) and PvE.

### v1.29 – OCR Performance

- Speed up OCR to improve scan times.

### v1.30 – Online Paddle OCR

- Use online language Paddle OCR to reduce release file size.

### v1.34 – Loot Tiers & Worth Indicator

- **Loot Tiers**: aligned with tarkov.dev. Now shows `[★] Loot Tier X (tarkov.dev)` for curated tiers and `(per slot)` for calculated ones.

<img src="https://imgur.com/Rbc2jR7.png"/>
<img src="https://imgur.com/tgvJd0t.png"/>

- **Worth Indicator**: added configurable "WORTH" tag for high-value-per-slot items.

<img src="https://imgur.com/jgEy72D.png"/>

- **Performance**: optimized OCR engine for faster scanning.

<img src="https://imgur.com/5p5fiAy.gif"/>

### v1.35 – TarkovTracker & Hideout Integration

- **Tracker-aware overlay for tasks**
  - The overlay shows how many items you still need for each quest that uses the hovered item.
  - It respects TarkovTracker progress: turned-in or completed objectives are subtracted and hidden.
  - Only objectives that require **Found in Raid** items are shown.

- **Local-only hideout tracking**
  - The overlay shows a separate **Needed for Hideout** section with remaining counts per station level.
  - Hideout progress is tracked locally (no writes to TarkovTracker) and persisted as a human-readable JSON file (`tarkovtracker-hideout.json`).

- **Interactive hotkeys for tracker and hideout**
  - While the overlay is open on an item:
    - `↑` (Up Arrow): increase progress for the first incomplete task objective requiring that item.
    - `↓` (Down Arrow): decrease progress from the last objective that still has progress.
  - If there is no matching task objective, the same keys will adjust local hideout requirements for that item instead.
  - Hotkeys can be changed in the **Settings** window.

- **Local persistence**
  - Task changes are buffered locally and periodically flushed to the TarkovTracker API.
  - Two JSON files are written next to the executable:
    - `tarkovtracker-tasks.json` – pending task objectives that need to be synced.
    - `tarkovtracker-hideout.json` – local-only hideout item requirements and counts.

- **Startup performance**
  - On startup the app now eagerly preloads:
    - `tarkov.dev` item and hideout data.
    - TarkovTracker progress (if enabled in settings).
    - PaddleOCR language model.
  - This makes the **first scan much faster and more consistent**.

- **Worth thresholds & trader vs Flea tolerance**
  - New **Item Worth Threshold (₽/slot)** setting to control when high value-per-slot items are highlighted as "WORTH".
  - Separate **Ammo Worth Threshold** so typical bullet prices can still trigger worth highlighting without affecting other items.
  - A **Profit vs. Flea tolerance** slider controls when the overlay prefers a trader over the Flea Market, based on per-slot value.
  - The overlay shows a helper suffix like `(68% of Flea, min 40%)` next to the trader line and highlights it in green when the actual percentage meets or exceeds the configured minimum.

### TarkovTracker & Hideout settings (v1.35)

- To use tracker-aware features you need a **TarkovTracker API key**:
  - Generate one on https://tarkovtracker.org/ and paste it in the **TarkovTracker API Key** field.
  - Enable the option to **Use TarkovTracker API** in the settings.
- Once enabled:
  - The overlay will display **Used in Task** with `You have` / `Needed` counts per item.
  - The overlay will display **Needed for Hideout** with remaining counts per station level.
  - You can use the **Up/Down arrow keys** while the overlay is visible to increment/decrement progress for the current item.
  - Local progress will be persisted between sessions in the `tarkovtracker-tasks.json` and `tarkovtracker-hideout.json` files.

- **Item & ammo worth thresholds**
  - `Item Worth Threshold (₽/slot)`: base per-slot value above which items are highlighted as "WORTH".
  - `Ammo Worth Threshold`: per-round threshold used only for ammo types.
- **Profit vs. Flea tolerance**
  - Slider from 0100% that controls how close a trader's per-slot price must be to Flea to be considered competitive.
  - The overlay displays `(XX% of Flea, min YY%)`; it turns green when `XX >= YY`.
</br>

## Settings

<img src="https://imgur.com/x86dvF0.png"/>

You can change the settings how you like.
</br>
</br>
If you get errors, please check you have installed:
</br>
Latest .NET Desktop Runtime 10.0 (or newer) (https://dotnet.microsoft.com/download)
</br>
Latest Visual C++ Redistributable (https://aka.ms/vs/17/release/vc_redist.x64.exe)

# Notice - User
1. It cannot be used in full screen. Borderless windowed mode is recommended.
2. I do not take any responsibility for any disadvantages or damages including the above. However, there is no danger of ban, so don't worry. Overwolf's minimap overlay or RatScanner are also not a reason for ban.

# Notice - Developer
1. I'm a newbie developer who just graduated from college who used to program mostly in Java programs So please understand that most of the code is not optimized for C#. If you tell us about any strange parts in the code or where it can be improved, we will try to fix it as much as possible.
2. Originally, I was going to make a program that recognizes the image of the product itself, but there were too many variables in the image, so I stopped development and changed the direction to recognizing the name. If anyone can implement it, I'll try to fix it.
3. Capture the game screen, find the item name with OpenCV and recognize the text with PaddleOCR. If there are other free OCR modules, please recommend them.

# Program (or site) used and license
1. OpenCV (https://opencv.org/license)
2. PaddleSharp / Sdcb.PaddleOCR (https://github.com/sdcb/PaddleSharp/blob/master/LICENSE)
3. Tarkov Market (https://tarkov-market.com/)
4. Tarkov Wiki (https://escapefromtarkov.fandom.com/wiki/Escape_from_Tarkov_Wiki)
5. Escape from Tarkov (https://www.escapefromtarkov.com/)
6. Tarkov.dev API (https://tarkov.dev/)
7. TarkovTracker API (https://tarkovtracker.org)

# Funny description written by ChatGPT
Hey fellow Tarkov players, listen up! If you're struggling to keep up with all the ammo types in the game, or if you're having a hard time finding out the market price of certain items before the flea market opens, then boy do I have the perfect tool for you!

Introducing the Escape from Tarkov Overlay Helper - your new best friend when it comes to all things Tarkov. This app is jam-packed with all the information you need to survive in the game, and it's especially helpful for newer players who are still getting the hang of things.

With this tool, you can easily distinguish all the strengths and weaknesses of every single ammo type in the game thanks to the ammo chart and ammo tier list. Plus, you can use it to compare prices of parts in Edit Preset, so you can get the best deal possible.

But that's not all - this overlay also has a sorting function, ballistics information, and even loot tiers! So you can make sure you're looting the right places and getting the best possible gear.

And if you're struggling with certain tasks or quests, this app also has maps and a guide to help you out. And for those who prefer a scanner alternative, you're in luck - this overlay tool has got you covered.

And if you're looking for info on items that are needed for hideout upgrades, you're in luck too! This tool has all the details you need, based on TarkovTracker progress.

So what are you waiting for? Download the Escape from Tarkov Overlay Helper now and stay safe out there, fellow Tarkovians!
