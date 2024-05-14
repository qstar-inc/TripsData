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
    public partial class CarStatistics : GameSystemBase
    {
        private EntityQuery m_QueryParkedCar;
        private EntityQuery m_QueryGarageLane;
        private int previous_index = -1;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_QueryParkedCar = this.GetEntityQuery(new EntityQueryDesc()
            {
                Any = new ComponentType[1]
              {
                ComponentType.ReadOnly<ParkedCar>()
              }
            });
            this.RequireForUpdate(this.m_QueryParkedCar);
        }

        protected override void OnUpdate()
        {
            if (Mod.m_Setting.cars)
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
                    var results = m_QueryParkedCar.ToEntityArray(Allocator.Temp);

                    int parkedCar = 0;
                    foreach (var veh in results)
                    {
                        ParkedCar data1;

                        if (EntityManager.TryGetComponent<ParkedCar>(veh, out data1))
                        {
                            parkedCar++;
                        }
                    }

                    string path = Path.Combine(Mod.outputPath, Mod.cars);
                    string fileName = path +
                        "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";


                    if (index == 0)
                    {
                        string header = "hour,parked_cars";

                        Utils.createAndDeleteFiles(fileName, header, Mod.cars, path);

                    }

                    string line = $"{(float)index / 2f},{parkedCar}";

                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
        
        public CarStatistics()
        {
        }
    }
}
