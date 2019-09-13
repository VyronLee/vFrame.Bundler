//------------------------------------------------------------
//        File:  IScene.cs
//       Brief:  Scene asset interface.
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:07
//   Copyright:  Copyright (c) 2018, VyronLee
//============================================================

namespace vFrame.Bundler.Interface
{
    public interface IScene
    {
        void Unload();
        void Activate();
    }
}