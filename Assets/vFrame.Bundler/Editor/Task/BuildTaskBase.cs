// ------------------------------------------------------------
//         File: BuildTaskBase.cs
//        Brief: 构建任务基类
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-24 21:27
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler.Editor.Task
{
    internal abstract class BuildTaskBase
    {
        public abstract void Run(BuildContext context);
    }
}