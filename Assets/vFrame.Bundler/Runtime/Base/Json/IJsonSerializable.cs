// ------------------------------------------------------------
//         File: IJsonSerializable.cs
//        Brief: Marks a type as JSON-serializable via the JsonSerializableProperty attribute.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:33:49
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Marks a type as JSON-serializable; members to include are selected by the
    ///     <see cref="JsonSerializableProperty"/> attribute and processed by <c>JsonExtension</c>.
    /// </summary>
    public interface IJsonSerializable
    {

    }
}