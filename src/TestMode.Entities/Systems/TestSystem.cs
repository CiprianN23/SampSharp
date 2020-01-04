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

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.Entities.SAMP.Components;
using SampSharp.Entities.SAMP.Definitions;
using TestMode.Entities.Components;
using TestMode.Entities.Services;

namespace TestMode.Entities.Systems
{
    public class TestSystem : ISystem
    {
        private GangZone _zone;

        [Event]
        public void OnGameModeInit(IVehicleRepository vehiclesRepository,  IWorldService worldService) 
        {
            // Event methods have dependency injection alongside the arguments

            Console.WriteLine("Do game mode loading goodies...");

            vehiclesRepository.Foo();

            worldService.CreateActor(101, new Vector3(0, 0, 20), 0);

            var blue = Color.Blue;
            blue.A = 128;
            _zone = worldService.CreateGangZone(0, 0, 100, 100);
            _zone.Color = blue;
            _zone.Show();

            var obj = worldService.CreateObject(16638, new Vector3(10, 10, 40), Vector3.Zero, 1000);
            obj.DisableCameraCollisions();

            worldService.CreateVehicle(VehicleModelType.Alpha, new Vector3(40, 40, 10), 0, 0, 0);

            var green = Color.Green;
            green.A = 128;
            worldService.CreateTextLabel("text", green, new Vector3(10, 10, 10), 1000);

            var ctx = SynchronizationContext.Current;
            Task.Run(() =>
            {
                // ... Run things on a worker thread.

                ctx.Send(_ =>
                {
                    // ... Run things on the main thead.
                }, null);
            });

        }

        [Event]
        public void OnPlayerWeaponShot(Player player, int weaponId, int hitType, Entity hit, float x, float y, float z)
        {
            var pos = new Vector3(x, y, z);
            player.SendClientMessage($"You shot {hit} at {pos}");
        }

        [Event]
        public bool OnPlayerCommandText(Player player, string text, IWorldService worldService, IEntityManager entityManager)
        {
            if (text == "/entities")
            {
                void Print(int indent, Entity entity)
                {
                    var ind = string.Concat(Enumerable.Repeat(' ', indent));

                    player.SendClientMessage($"{ind}{entity}");
                    foreach (var com in entity.GetComponents<Component>())
                        player.SendClientMessage($"{ind}::>{com.GetType().Name}");

                    foreach (var child in entity.Children)
                        Print(indent + 2, child);
                }

                Print(0, entityManager.Get(WorldService.WorldId));
                return true;
            }
            if (text == "/weapon")
            {
                player.GiveWeapon(Weapon.AK47, 100);
                player.SetArmedWeapon(Weapon.AK47);
                player.PlaySound(1083);
                return true;
            }
            if (text == "/actor")
            {
                worldService.CreateActor(0, player.Position + Vector3.Up, 0);
                player.SendClientMessage("Actor created!");
                player.PlaySound(1083);
                return true;
            }

            if (text == "/pos")
            {
                player.SendClientMessage(-1, $"You are at {player.Position}");
                return true;
            }

            return false;
        }

        [Event]
        public void OnPlayerConnect(Entity player, IVehicleRepository vehiclesRepository)
        {
            Console.WriteLine("I connected! " + player.Id);

            player.AddComponent<TestComponent>();

            _zone.Show(player);
            vehiclesRepository.FooForPlayer(player);
        }

        [Event]
        public void OnPlayerConnect(Player player)
        {
            player.SendClientMessage($"Hey there, {player.Name}");
        }

        [Event]
        public void OnPlayerText(TestComponent test, string text)
        {
            Console.WriteLine(test.WelcomingMessage + ":::" + text);
        }
    }
}