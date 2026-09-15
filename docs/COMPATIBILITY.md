# 接口适配与验证

公开版不保存发行客户端 SHA/MVID 锁。连接前以 Mono.Cecil 只读本机 Managed 元数据，使用接口形状、规范化方法指纹和引用位置解析本功能契约，再通过内置 Roslyn 编译本地组件。歧义/缺失/规则变化拒绝连接，不猜测匹配。

Hook 的 CompiledMvid 只检查本次编译针对的文件和运行中进程是否一致，防止游戏更新后旧进程未退出；不是发行时的版本限制。缓存类型/成员查找；每帧只读取所需盘面和状态，不重复全程序集解析或编译。

## 连连看契约

覆盖盘面发现、宽高/格子、游戏状态、暂停/洗牌/输入锁、动画计数、剩余时间/连击/加时、图标、选择状态、原生自动点击、原生配对事件、主线程帧与网络回执。

配对事件使用当前客户端的精确委托类型，订阅与取消订阅成对执行，通过回调与盘面回读确认操作；不替换游戏已有委托。回执观察器解析 IL 指令中的方法操作数，拒绝把任意四字节当作 Parser 调用。Start/End 消息仅观察，不构造或重发结算。

方块枚举名称/数值与契约逐项对比，防止岩石、钥匙、锁和加时的游戏定义改变后继续套用旧求解规则。无法确认时停止连接，需要维护者更新逻辑。

## 0.2.0 离线验证

同一份契约对两份真实客户端均解析、编译成功：

| 客户端 MVID | 解析类型/成员 | 自动适配类型/成员改名 | 接口与回调检查 |
| --- | --- | --- | --- |
| 3865bd9f-476e-40f8-a69b-aebc7727bda9 | 8 / 30 | 0 / 0 | 40 项通过 |
| 048fef50-fcf3-4121-8d6d-d0668959c1b2 | 8 / 30 | 4 / 26 | 40 项通过 |

本轮没有向游戏进程注入或发送游戏请求。公开版新组件的实际整局、结算与动态间隔仍待使用时验收；离线检查不代表任意未来版本或实机均已通过。

合成程序集回归覆盖重命名、元数据重排、增加无关方法、缺失和歧义拒绝；不包含游戏 DLL。求解/状态机使用本项目生成盘面及独立判定器。发行源只保留必要接口描述和单向指纹，不包含客户端方法正文。

## 离线检查

~~~powershell
BD2Sichuan-0.2.0-Portable-win-x64.exe --check-client "C:\YourGame\BrownDust II_Data\Managed" "compatibility-result.json"
dotnet run --project compatibility-cli -c Release -- check "C:\YourGame\BrownDust II_Data\Managed" .build/client-check
dotnet run --project abi-probe -c Release -- "C:\YourGame\BrownDust II_Data\Managed" .build/client-check/BD2Sichuan.Runtime2.dll
~~~

这些入口不注入。接口发生破坏性变化时先研究失败项，再修改 ContractGenerator/Hook 并 generate 新契约；不得仅因版本标识变化就重建契约。必须保留旧契约在新客户端的检查结果，并用新契约复核仍要支持的版本。
