using Game.Citizens;
using Game.Creatures;
using Game.Simulation;
using Game.Vehicles;
using System.IO;
using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Game;
using System.Linq;
using Game.Buildings;
using Colossal.Entities;
using Game.Net;
using Game.Common;
using Game.Prefabs;
using Unity.Mathematics;
using UnityEngine;

namespace TripsData
{
    public partial class GarbageTruckStatistics : GameSystemBase
    {
        private EntityQuery m_QueryGarbageTrucks;
        private int previous_index = -1;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_QueryGarbageTrucks = this.GetEntityQuery(new EntityQueryDesc()
            {
                Any = new ComponentType[1]
              {
                ComponentType.ReadOnly<Game.Vehicles.GarbageTruck>()
              }
            });
            this.RequireForUpdate(this.m_QueryGarbageTrucks);
        }

        protected override void OnUpdate()
        {
            if (Mod.m_Setting.truck)
            {
                DateTime currentDateTime = this.World.GetExistingSystemManaged<TimeSystem>().GetCurrentDateTime();
                int half_hour = 0;
                if (currentDateTime.Minute >= 30)
                {
                    half_hour = 1;
                }
                int index = 2 * currentDateTime.Hour + half_hour;

                if (previous_index != index)
                {
                    previous_index = index;
                    var results = m_QueryGarbageTrucks.ToEntityArray(Allocator.Temp);

                    int trucks = 0;
                    foreach (var veh in results)
                    {
                        Game.Vehicles.GarbageTruck data1;

                        if (EntityManager.TryGetComponent<Game.Vehicles.GarbageTruck>(veh, out data1))
                        {
                            trucks++;
                        }
                    }

                    string path = Path.Combine(Mod.outputPath, Mod.garbage_trucks);
                    string fileName = path +
                        "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";

                    if (index == 0)
                    {
                        string header = "hour,total_garbage_trucks";

                        Utils.createAndDeleteFiles(fileName, header, Mod.garbage_trucks, path);

                    }

                    string line = $"{(float)index / 2f},{trucks}";

                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        public GarbageTruckStatistics()
        {
        }
    }
}
