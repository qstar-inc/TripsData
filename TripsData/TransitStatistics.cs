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
using Game.Routes;
using System.Collections.Generic;
using Unity.Mathematics;
using Game.Objects;
using Game.Common;
using Colossal.IO.AssetDatabase;

namespace TripsData
{
    public partial class TransitStatistics : GameSystemBase
    {
        private EntityQuery m_Query;
        private int previous_index = -1;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_Query = this.GetEntityQuery(new EntityQueryDesc()
            {
                Any = new ComponentType[1]
              {
                ComponentType.ReadOnly<WaitingPassengers>()
              }
            });
            this.RequireForUpdate(this.m_Query);
        }

        protected override void OnUpdate()
        {
            if (Mod.m_Setting.transit)
            {
                var results = m_Query.ToEntityArray(Allocator.Temp);

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
                    int waitingPassengers = 0;
                    float averageWaitingTime = 0;

                    foreach (var veh in results)
                    {
                        WaitingPassengers data1;

                        if (EntityManager.TryGetComponent<WaitingPassengers>(veh, out data1))
                        {
                            waitingPassengers += data1.m_Count;
                            averageWaitingTime += data1.m_AverageWaitingTime * data1.m_Count;
                        }
                    }

                    averageWaitingTime /= waitingPassengers;
                    averageWaitingTime /= 60f;

                    string path = Path.Combine(Mod.outputPath, Mod.transit);
                    string fileName = path +
                        "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";

                    if (index == 0)
                    {
                        string header = "hour,waitingPassengers,averageWaitingTime_minutes";

                        Utils.createAndDeleteFiles(fileName, header, Mod.transit, path);

                    }

                    string line = $"{(float)index / 2f},{waitingPassengers},{averageWaitingTime}";

                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(line);
                    }
                }

            }
        }

        private void __AssignQueries(ref SystemState state)
        {
        }

        protected override void OnCreateForCompiler()
        {
            base.OnCreateForCompiler();
            this.__AssignQueries(ref this.CheckedStateRef);
        }

        public TransitStatistics()
        {
        }
    }
}
