# Mocbuilder's Game of Life
## Table of contents
- [Overview](#overview)
- [The Twist](#the-twist)
- [The Algorithm](#the-algorithm)
    - [Intended Effect](#intended-effect)
    - [Complexity and behavior](#complexity-and-behavior)]
- [Modes and CLI usage](#modes-and-cli-usage)
- [Interactive controls](#interactive-controls-when-not-running-in-autonomous-generation-mode)
- [Default values](#default-values)
- [Map dumps](#map-dumps)
- [Installation](#installation)
  - [Option A: Download latest release](#option-a-download-latest-release)
  - [Option B: Build from source](#option-b-build-from-source)

---

## Overview

This is a console implementation of Conway's Game of Life (CGOL) with an additional "twist": an autonomous survival algorithm that attempts to extend the lifetime of the simulation by preserving existing cells through strategic cell additions automatically.

Conway's Game of Life is a zero-player cellular automaton where cells on a grid live, die, or are born based on the number of alive neighbors. This project provides an interactive sandbox and autonomous simulation modes, plus the ability to save and load map dumps.

To simulate an infinite grid, the program uses a finite grid with wrap-around edges (toroidal topology). The grid size is configurable.

## The Twist

Two autonomous features augment the classic rules:

- `--enable-autonomous-survival`: When enabled, the simulation can add one cell each generation in an autonomously chosen "best" location to try to keep the existing population alive as long as possible.
- Autonomous generation mode (`autonomous` command): The simulation advances automatically each generation (no user input required) and can run for a fixed number of generations or indefinitely.

These features are experimental and are intended to explore emergent behavior under guided modification of the grid.

## The Algorithm

- Input: a flat list of Cell objects (`cells`) and the Map context.
- For each live cell (`c.isLive == true`), the algorithm collects its neighbors via `CellHandler.GetNeighbourCells`.
- It counts live neighbors. If a live cell has exactly one live neighbor, the cell is flagged as "about to die" (classic CGOL underpopulation).
- The algorithm takes that lone neighbor and searches the overall `cells` list for a `dead` cell whose coordinates are within ±1 in both X and Y of both the dying cell and the lone neighbor (i.e., a cell that is adjacent to both).
- The first such shared dead cell is returned as the chosen placement for the autonomous cell. If none is found for any candidate pair, the method returns `null`.
- This creates a local triangle/cluster (three adjacent cells) designed to change the neighborhood counts so the originally dying cell and its neighbor will have more neighbors in the next generation and therefore are more likely to survive or to produce new cells.

### Intended Effect
- By inserting a cell in a spot adjacent to both cells, you increase neighbor counts for the vulnerable cell(s), converting an underpopulation death into either survival or emergence of a stable/oscillating micro-pattern.
- The heuristic is intentionally local and cheap: it targets the simplest rescue scenario (single-neighbor death) without globally re-evaluating complex futures.

### Complexity and behavior
- Time complexity: O(N * k) where N is number of cells and k is neighborhood lookup cost (small constant for a fixed neighborhood). The outer loop visits every live cell; neighbor checks and the search for a shared dead neighbor are linear scans over all cells in the current implementation.
- Determinism: The algorithm returns the first matching dead cell it finds. Placement is therefore deterministic relative to iteration order; different ordering will yield different rescue spots.
- Limitations: Only addresses underpopulation (1 neighbor) cases and only uses a single cell placement per call. It does not consider longer-term dynamics (multi-step prediction) nor global optimization (multiple placements per generation).

## Modes and CLI Usage

The program exposes three top-level commands: `sandbox`, `autonomous`, and `read`.

- `sandbox`
  - Runs the simulation interactively. You can step generations manually, save the current map state, or restart.
  - Options:
    - `--dumpfolder <path>` : Root folder for map save dumps. Defaults to `"%USERPROFILE%\\Desktop\\ConwaysGameOfLifeDumps"` (desktop path is used programmatically).
    - `--enable-autonomous-survival` : Enable autonomous survival behavior (default: `false`).
    - `--sizex <n>` : Width of the map (default: `10`).
    - `--sizey <n>` : Height of the map (default: `10`).

- `autonomous <dumpfolder> <generations-count>`
  - Runs the simulation automatically. Must provide the `dumpfolder` argument where dumps will be saved, and the number of generations to run.
  - Arguments:
    - `dumpfolder` : Path to the folder that will contain map save dumps.
    - `generations-count` : Number of generations to run. Use `-1` for unlimited.
  - Options (same as sandbox): `--enable-autonomous-survival`, `--sizex`, `--sizey`.

- `read <file-to-read>`
  - Loads a saved map dump file and allows to continue the generation from that state.
  - Arguments:
    - `file-to-read` : Path to the saved map file to load.

If you run the program without a valid command or with invalid arguments, the program will show help and exit with an error.

## Interactive Controls (when not running in autonomous generation mode)

- Press `Enter` : Advance one generation.
- Press `R` : Restart the simulation (new random map using current settings).
- Press `S` : Save (dump) the current map state to the configured dump folder.
- Press `Ctrl+C` : Exit the program immediately.

When `enableAutonomousGeneration` is active (autonomous mode), the program advances generations automatically without requiring key presses.

## Default Values

- Map size: `10 x 10` (`--sizex 10 --sizey 10`)
- Default dump folder: Desktop → `ConwaysGameOfLifeDumps` (constructed programmatically)
- Default generations count: `10` (use `-1` for unlimited in `autonomous` mode)
- `--enable-autonomous-survival` default: `false`

## Map Dumps

Map dumps are written to the configured dump folder. By default the program constructs a folder on the current user's Desktop named `ConwaysGameOfLifeDumps`. Use `--dumpfolder` or the `dumpfolder` argument for `autonomous` to change the destination.

Each dump file is named with a timestamp and generation number, e.g., `dump_2024-06-01_15-30-00_gen_42.json`. The files are in JSON format and contain the current state of the grid, including live/dead cell positions.

## Installation

Prerequisites
- .NET 9 SDK or Runtime installed: https://dotnet.microsoft.com  
- Terminal: Command Prompt, PowerShell, or POSIX shell.

### Option A: Download latest release
1. Visit: https://github.com/Mocbuilder/mocbuilders_game_of_life/releases/latest.  
2. Download the latest release  
3. Extract to a folder.  
4. Run:
   - If a platform executable is provided (Windows example):
     - `.\conways_game_of_life.exe sandbox`
   - If a framework-dependent DLL is provided:
     - `dotnet conways_game_of_life.dll -- sandbox`

### Option B: Build from source
1. Clone and enter the repo:
   - `git clone https://github.com/Mocbuilder/mocbuilders_game_of_life.git`
   - `cd mocbuilders_game_of_life`

2. Get Required System.CommandLine package (if not already restored):
   - `dotnet add package System.CommandLine`
   - `dotnet restore`

2. Build and run:
   - `dotnet build -c Release`
   - `dotnet run --project . -- sandbox`

3. (Optional) Publish for distribution:
   - Framework-dependent single-file (requires runtime installed):
     - `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false -o ./publish`
   - Self-contained single-file (no runtime required):
     - `dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./publish`  
   - Replace `win-x64` with your target RID (`linux-x64`, `osx-x64`, etc.). After publish, run `./publish/conways_game_of_life.exe sandbox` (or the equivalent executable).

## License
Mocbuilder's Game of Life  Copyright (C) 2026  Mocbuilder

This program is licensed under the GNU General Public License v3.0 (GPLv3). You may redistribute and/or modify it under the terms of the GPLv3 as published by the Free Software Foundation.
For details, see the LICENSE file included in this repository or visit https://www.gnu.org/licenses/gpl-3.0.en.html.
