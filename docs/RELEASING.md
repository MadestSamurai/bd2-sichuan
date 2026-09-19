# 发布检查

## 应用发布

- [ ] 修改 Directory.Build.props 的版本、README 的下载文件名和 RELEASE_NOTES。
- [ ] 修改 Hook 时递增 Runtime 名称，同步 CLI、成品身份断言和 ABI 探针。
- [ ] 更新 .NET 时同步 RuntimeFrameworkVersion、固定 ILLink 版本和全部依赖锁文件。
- [ ] build.ps1 -Locked：独立构建、求解/状态机与接口适配回归。
- [ ] package.ps1 -Locked：Portable/Lite 独立输出目录、单 EXE、实际 runtimeconfig 和 WPF 离线验证。
- [ ] 同一契约对可用新旧客户端运行 check 和 ABI，不注入游戏。
- [ ] 检查 staged 文件：只有源码、契约、依赖锁、文档及许可证；没有游戏 DLL/资源、账号、捕获记录、个人路径或私钥。
- [ ] Release 正文只写功能、双版本区别表和升级步骤；内部 Runtime 编号、指纹及测试状态写维护文档。
- [ ] 推送源码，再推送 vX.Y.Z 标签；GitHub Actions 无游戏、无私有父仓库构建双版本，并生成 EXE/ZIP、SHA256SUMS、release.json。
- [ ] 下载正式资产、核对全部 SHA256、ZIP 文件清单、版本和两版 WPF；正式 EXE 再运行离线客户端检查。
- [ ] 更新维护记录，明确离线通过和实机待验收的区别。

Portable 内置固定 .NET Desktop Runtime；Lite 使用 Microsoft.WindowsDesktop.App 8.0.0，可滚动到更新的 8.0 补丁。两版不能共用 SelfContained 切换后的中间产物。ZIP 保留 README、MIT 和完整第三方许可证，不包含客户端 DLL。

## 游戏更新

- [ ] 优先用已发布 EXE 检查，不因客户端 SHA/MVID 变化立即重新生成契约。
- [ ] 核对主线程帧、盘面初始化、格子/选中字段、输入锁、动画数、暂停/洗牌与剩余时间。
- [ ] 核对方块枚举/数值、特殊方块效果及求解规则。
- [ ] 核对原生点击、配对委托签名、正确 add/remove 与回读。
- [ ] 核对 Start/End 原始回执唯一、图标字段及依赖闭包。
- [ ] 验证执行租约、停止、关闭、重连和其他 Hook 冲突处理。
- [ ] 仅在确认接口不兼容后更新引导定义、生成契约，并复核新旧客户端。

构建、测试、签名/发布产物均与原私有项目隔离。不得复制旧父仓库 Git 历史或私密资料。

## Languages

Every Portable/Lite EXE includes Chinese and English. ZIPs include both README files. Release notes contain both language sections and a runtime comparison table. See [Localization](LOCALIZATION.md).

## 文档格式 / Documentation format

README、仓库简介和 Release 统一遵循 [Publication style](PUBLICATION_STYLE.md)。新版本从 [Release template](RELEASE_TEMPLATE.md) 开始，更新 [当前版本说明](RELEASE_NOTES.md) 后再打包。

Use the shared format for READMEs, repository descriptions and releases. Update both languages and release notes before packaging.
