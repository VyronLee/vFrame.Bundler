[English](README.md) | [简体中文](README.zh-CN.md)

# SBP 选项验证工具（#110）

这是可复现的**证据工具**，不是 Bundler 的 SBP 实现，也不证明产品已完全兼容。

## 环境与安装

需要 Windows、PowerShell 7+、已有的 Unity **2022.3.62f3** 编辑器及 Windows 构建支持、解压后的官方 **SBP 1.20.1** 包。克隆本仓库即可；不增加库、不下载编辑器，也不把 SBP 安装到 Bundler 原工程。只有临时工程引用传入的本地包。

## 运行

在仓库根目录执行：

```powershell
pwsh -NoProfile -File tools/sbp-option-oracle/test-report.ps1
pwsh -NoProfile -File tools/sbp-option-oracle/test-preflight.ps1
pwsh -NoProfile -File tools/sbp-option-oracle/run.ps1 -SbpPackagePath '<解压后的-sbp-1.20.1>'
```

可用 `-UnityPath '<Unity.exe>'` 指定已有编辑器。`-RunDirectory '<新的短路径>'` 必须不存在，且位于已有 Unity 工程和 SBP 源码包之外；不会覆盖旧实验。超时只终止本工具创建的子进程，不会关闭其他 Editor。

## 验证方法

- 两个独立工程分别完成预热、布局变化、内容变化，共六次 Editor 进程。原生 flag 对照不共享构建缓存。C# 只在进程完全退出后修改，并检查已加载 Editor 与已编译 Player 的字段一致。
- 保持输出/缓存配置固定，用原生 BuildReport 的真实构建步骤和 SBP 公开日志的序列化/归档事件判断复用、失效和强制重建。不把文件数量或哈希变化当成执行证据。
- 只在构建调用窗口内开启序列化错误刺激，原样记录严格/宽松结果，并在前后验证真正干净的构建。缺失输入另测失败行为，不用检查固定坏资源的伪 guard 强制拒绝。
- 报告自测使用合成数据**仅验证判定器**，不能作为 Unity 行为证据。

## 结果与限制

工具输出唯一临时工作目录，以及可长期本地保存的 `TestResults/OptionOracle/<id>/`。后者保留源码标识、准确命令/退出码、各阶段 JSON、构建日志和 verdict，也保留失败阶段。原始日志可能含机器/许可证信息，因此目录不进入 git；清理测试结果前应保存或导出。

仓库中的 `baseline-2026-10-18.json` 保存已记录的源码标识和结果，不含机器绝对路径；它不能替代实际重跑。

`verdict.json` 将控制条件是否有效与 StrictMode 的原始结果分开，不会认证产品等价。非零退出、缺少阶段、布局不匹配、错误刺激未发生或控制失败都不能算通过。旧无效探针的文件不读取、不修改、不删除；新协议替代其方法，不抹去旧失败记录。

已记录的 2022.3 样例中：SBP 布局变化写入两个包，未变输入为零，强制重建为三个包。原生 IgnoreTypeTreeChanges 可复用旧布局，而普通原生构建会重建。序列化回调中的 `Debug.LogError` 即使在 StrictMode 下也未使任一原始后端拒绝；该刺激在原生 BuildReport 中的错误数为零。这**不证明**全部错误类型或产品 guard 的行为。

**2019.4.40f1**、真实 IBundler/独立 Player、场景/Shader 和其他选项仍未验证。缺失编辑器不能算通过；CI 许可证激活问题另行记录。

## 许可

[Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)。
