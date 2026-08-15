// ------------------------------------------------------------
//         File: AssemblyInfo.cs
//        Brief: Exposes internal reference-counting types to the
//               EditMode test assembly so the Destroy-cascade
//               underflow guard (R5) can be exercised directly.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2026-08-09 00:00:00
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("vFrame.Bundler.Tests.EditMode")]