// ------------------------------------------------------------
//         File: AssemblyInfo.cs
//        Brief: Grants the EditMode test assembly visibility into
//               internal types so ref-counting and Destroy-cascade
//               guards can be tested directly.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:29:44
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("vFrame.Bundler.Tests.EditMode")]