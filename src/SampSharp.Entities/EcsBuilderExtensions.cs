﻿// SampSharp
// Copyright 2019 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace SampSharp.Entities
{
    /// <summary>
    /// Provides extended functionality for configuring a <see cref="IEcsBuilder" /> instance.
    /// </summary>
    public static class EcsBuilderExtensions
    {
        /// <summary>
        /// Adds the system with the specified type <typeparamref name="T" />. The specified type must be available in the service provider.
        /// </summary>
        /// <typeparam name="T">Type of the system.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>The builder.</returns>
        public static IEcsBuilder UseSystem<T>(this IEcsBuilder builder) where T : ISystem
        {
            return builder.UseSystem(typeof(T));
        }
    }
}