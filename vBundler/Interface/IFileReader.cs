//------------------------------------------------------------
//        File:  IFileReader.cs
//       Brief:  IFileReader
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//     Modified:  2019-05-14 10:25
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

namespace vBundler.Interface
{
    public interface IFileReader : ICloneable<IFileReader>
    {
        byte[] ReadAllBytes(string path);
    }
}