//------------------------------------------------------------
//        File:  IFileReaderAsync.cs
//       Brief:  IFileReaderAsync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//     Modified:  2019-05-14 10:26
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System;
using System.IO;

namespace vFrame.Bundler.Interface
{
    public interface IFileReaderRequest : IAsync, IDisposable
    {
        byte[] GetBytes();

        Stream GetStream();
    }

    public interface IFileReaderAsync : ICloneable<IFileReaderAsync>
    {
        IFileReaderRequest ReadAllBytesAsync(string path);
    }
}