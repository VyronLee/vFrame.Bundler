// ------------------------------------------------------------
//         File: PropertyLink.cs
//        Brief: Base for exclusive links that bind a loaded asset to a Component property via Set().
//               Only one property link may target a given component at a time.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:10:17
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Exclusive link base that binds a loaded asset to a property of a target <see cref="Component"/> via <see cref="Set"/>.
    /// </summary>
    /// <remarks>Links of this kind are mutually exclusive: a component can be bound by at most one property link.</remarks>
    /// <typeparam name="T1">Type of the component whose property receives the asset.</typeparam>
    /// <typeparam name="T2">Type of the loaded asset object to bind.</typeparam>
    public abstract class PropertyLink<T1, T2> : LinkBase where T1 : Component where T2 : Object
    {
        /// <summary>
        /// Assigns the loaded asset to the target component's property.
        /// </summary>
        /// <param name="target">Component instance whose property receives the asset.</param>
        /// <param name="asset">Loaded asset object to assign.</param>
        public abstract void Set(T1 target, T2 asset);

        /// <summary>Always true: property links are exclusive to a single binding per component.</summary>
        internal override bool Exclusive => true;
    }
}