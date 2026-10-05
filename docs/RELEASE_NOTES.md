# BD2 Sichuan v0.3.3

## 简体中文

### 更新内容

- 改进网络等待的恢复：先确认游戏请求已结束，再回收过期观察记录，避免长期等待或过早切换组件。
- 持续更新结算诊断状态，并完善统一宿主连接与语言切换。

### 下载

适用于 Windows x64。两版功能相同，均为单 EXE，内置简体中文／English。

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 内置 .NET 运行时 | 首次使用推荐，下载即用 |
| **Lite** | 需安装 .NET Desktop Runtime 8 x64 | 已安装运行时，下载更小 |

下载一种版本即可。ZIP 附中英文说明和许可证；SHA256SUMS.txt 可用于核对下载文件。

### 升级

暂停并关闭旧工具，再打开新版连接；本机设置保留。当前组件支持在游戏保持运行时更新和交接；如游戏本身仍显示断线或登录提示，请先恢复游戏连接。

[使用说明与风险声明](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.md)

## English

### Changes

- Improved network recovery by checking that game requests have finished before retiring stale observations, avoiding indefinite waits or premature component switches.
- Settlement diagnostics now update continuously; shared-host connections and language switching are improved.

### Downloads

For Windows x64. Both editions have the same features, run as a single EXE and include Simplified Chinese / English.

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| **Portable** | .NET included | Most users; download and run |
| **Lite** | .NET Desktop Runtime 8 x64 required | Smaller download if the runtime is installed |

Download one edition. ZIPs include both READMEs and licenses. Use SHA256SUMS.txt to verify downloaded files.

### Upgrade

Pause and close the old assistant, then connect with the new version. Local settings are retained. Current components support updates and handoff while the game stays open. If the game itself is disconnected or asking you to log in, restore its connection first.

[Usage and risk disclaimer](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.en.md)

---

# BD2 Sichuan v0.3.1

## 简体中文

### 更新内容

- 主窗口增加免费开源署名：GitHub MadestSamurai／B站 MadSamurai。
- 新增「来源与说明」，可查看并复制官方仓库与下载链接；随界面切换中英文。
- 统一双语 README、来源与风险说明，ZIP 附带完整说明；MIT 许可证保持不变。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 自带 .NET，无需另装运行库 | 大多数用户 |
| **Lite** | 需要 .NET Desktop Runtime 8 x64 | 已安装桌面运行库、希望减小下载体积 |

两版功能相同，内置简体中文／English。EXE 可独立使用；ZIP 附带双语说明与许可证。用 `SHA256SUMS.txt` 核对下载。

### 升级

停止自动操作并关闭旧工具，再打开新版。已有设置保留；本次主要更新来源与说明界面。

作者发布版免费。第三方收费不代表作者参与、背书或提供服务。[使用说明与风险提示](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.md)。

## English

### Changes

- Adds free-release attribution to the main window: GitHub MadestSamurai / Bilibili MadSamurai.
- Adds About & source with selectable official repository and download links, following the selected UI language.
- Standardizes bilingual READMEs and source/risk notices, also included in ZIPs. The MIT License is unchanged.

### Downloads

| Build | Runtime | Recommended for |
| --- | --- | --- |
| **Portable** | Includes .NET; no separate runtime needed | Most users |
| **Lite** | Requires .NET Desktop Runtime 8 x64 | Smaller download when the desktop runtime is installed |

Both builds have identical features and include Simplified Chinese / English. EXEs run independently; ZIPs include bilingual documentation and licenses. Verify downloads with `SHA256SUMS.txt`.

### Upgrade

Stop automation and close the old tool, then open the new version. Existing settings are retained; this update primarily changes attribution and source information.

Official releases are free. Third-party fees do not imply the author's involvement, endorsement or support. [Usage and risk notice](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.en.md).

---

# BD2 Sichuan v0.3.0

## 简体中文

### 更新内容

- 内置英语界面，覆盖按钮、提示、运行状态及工具错误。顶部随时切换简体中文／English，记住选择，不重启正在运行的任务。
- 附独立英文 README；两种语言共用 Portable／Lite 下载，无需单独语言包。
- 调整顶部布局和小窗口尺寸，让英文提示和盘面都可读。
- 保留默认 1000 ms 可调配对间隔、单步消除、自动本局、暂停／洗牌续接及停止控制。

### 下载

| 版本 | 运行环境 | 建议 |
| --- | --- | --- |
| **Portable** | 内置 .NET 运行时 | 首次使用推荐，下载即用 |
| **Lite** | 需安装 [.NET Desktop Runtime 8 x64](https://dotnet.microsoft.com/download/dotnet/8.0) | 已安装运行时，下载更小 |

适用于 Windows x64。EXE 可独立运行，ZIP 附说明与许可证；使用 `SHA256SUMS.txt` 校验下载。两版功能相同，均内置简体中文／English。

### 升级

暂停并关闭旧工具，正常重启游戏，再打开新版连接。本机设置保留；具体功能与设置迁移见上面的更新内容。

[使用说明与风险声明](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.md)

## English

### Changes

- Built-in English UI covering controls, hints, live status and tool errors. Switch between Chinese and English at any time; the choice is saved without restarting an active run.
- Separate English README. Both languages are included in each Portable/Lite download.
- A compact header and revised minimum window size keep the board and longer English text readable.
- Keeps adjustable pair timing (1000 ms by default), single-pair execution, auto-play for the current round, pause/shuffle recovery and immediate stop.

### Downloads

| Edition | Runtime requirement | Recommended for |
| --- | --- | --- |
| **Portable** | .NET included | Most users; download and run |
| **Lite** | [.NET Desktop Runtime 8 x64](https://dotnet.microsoft.com/download/dotnet/8.0) | Smaller download if the runtime is installed |

For Windows x64. EXEs run on their own; ZIPs include documentation and licenses. Verify downloads against `SHA256SUMS.txt`. Both editions have the same features and include Simplified Chinese / English.

### Upgrade

Pause and close the old assistant, restart the game normally, then connect with the new version. Local preferences are retained; see Changes above for feature and setting migrations.

[Usage and risk disclaimer](https://github.com/MadestSamurai/bd2-sichuan/blob/main/README.en.md)
