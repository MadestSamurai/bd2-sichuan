# BD2 Sichuan

English · [简体中文](README.md)

A standalone Windows assistant for BrownDust II's Sichuan (tile-matching) minigame. It reads the live board, suggests a legal pair and connecting path, and can play one pair or the current round automatically.

## Download

Get **0.3.0** from [Releases](https://github.com/MadestSamurai/bd2-sichuan/releases/latest). Both editions have the same features and include Chinese and English.

| Edition | Required runtime | Choose this if… |
| --- | --- | --- |
| Portable, Windows x64 | Included | You want to run without installing .NET. |
| Lite, Windows x64 | [.NET Desktop Runtime 8 x64](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) | You already have the runtime and want a smaller download. |

Use the EXE directly, or the ZIP containing the EXE, both READMEs and licenses. No Python, Visual Studio or game SDK is required. Check downloads against `SHA256SUMS.txt`.

## Quick start

1. When upgrading, close the old tool and restart the game to unload the previous component.
2. Open the tile-matching minigame in BrownDust II.
3. Run `BD2Sichuan-0.3.0-Portable-win-x64.exe` (or Lite). Choose **English** in **语言 / Language** if needed.
4. Click **Connect game** and wait for the live board. You do not need to match a pair manually first.
5. Use **Show hints**, **Play next pair**, or **Auto-play round**. The next pair is marked ① and ②.
6. Use **Stop now** to stop execution; closing the window also stops the tool.

## Controls

- **Pair interval:** default **1000 ms**, adjustable from **50 to 60000 ms**. Click **Apply interval** or press Enter. It can be changed during a run and is saved for next time.
- **Hints:** show a legal next pair first, then try to find a full route. Hints update after manual changes, combo rewards and shuffles. A complete static route is not a guarantee that the game will leave the board unchanged.
- **Auto-play round:** confirms each pair using client events and board readback, waits through pauses, shuffles and input locks, and stops when the round ends. It does not start the next level or spend items.
- **Always on top:** keeps the assistant visible over other windows.
- **Language:** live Chinese/English switching, independent of game language. Changing language does not restart a run or change its interval.

## Settings and troubleshooting

Local settings, board evidence and diagnostics are under `%LOCALAPPDATA%\BD2Sichuan`; board-specific files are in its `sichuan` subfolder. Language defaults to Chinese on Chinese systems and English otherwise, then remembers your choice. Game images and raw diagnostics remain unchanged.

If the board is missing, make sure the actual minigame is open, the tool is connected, and the game has finished loading. Stale hints are withdrawn and temporary read failures retried. A component mismatch or game process change requires reconnecting; a previously loaded different version requires a normal game restart. Do not repeatedly connect while loading.

The connector checks and adapts to local game interfaces. Unknown or ambiguous interfaces are rejected with diagnostics. This does not guarantee compatibility with every future update. No game assemblies or account data are included.

## Build and contribute

Requires Windows, the .NET 8 SDK and PowerShell:

```powershell
./build.ps1 -Locked
./package.ps1 -Locked
```

Tests compare the solver with an independent board oracle and check execution, leases, compatibility and localization. Packaging verifies both runtime configurations and runs isolated UI checks in both languages. See [release workflow](docs/RELEASING.md), [compatibility](docs/COMPATIBILITY.md) and [translation maintenance](docs/LOCALIZATION.md).

## License

Project code: [MIT](LICENSE). See [third-party notices](THIRD_PARTY_NOTICES.md) for dependency licenses. Not affiliated with the game developer or publisher.
