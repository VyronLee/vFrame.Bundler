//------------------------------------------------------------
//        File:  IAsync.cs
//       Brief:  IAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:05
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections;

namespace vFrame.Bundler.Interface
{
    public interface IAsync : IEnumerator
    {
        bool IsDone { get; }
        float Progress { get; }
    }
}