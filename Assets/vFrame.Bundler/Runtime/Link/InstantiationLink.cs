// ------------------------------------------------------------
//         File: InstantiationLink.cs
//        Brief: Non-exclusive link type: tracks asset instances created via Instantiate, allowing
//               many instances per loader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-4 21:39
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    public class InstantiationLink : LinkBase
    {
        internal override bool Exclusive => false;
    }
}