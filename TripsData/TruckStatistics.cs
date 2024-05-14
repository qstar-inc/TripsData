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
    public partial class TruckStatistics : GameSystemBase
    {
        private EntityQuery m_QueryTrucks;
        private int previous_index = -1;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.m_QueryTrucks = this.GetEntityQuery(new EntityQueryDesc()
            {
                Any = new ComponentType[1]
              {
                ComponentType.ReadOnly<Game.Vehicles.DeliveryTruck>()
              }
            });
            this.RequireForUpdate(this.m_QueryTrucks);
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
                    var results = m_QueryTrucks.ToEntityArray(Allocator.Temp);

                    //Truck Flags
                    int[] truck_flags = new int[5];

                    int trucks = 0;
                    foreach (var veh in results)
                    {
                        Game.Vehicles.DeliveryTruck data1;

                        if (EntityManager.TryGetComponent<Game.Vehicles.DeliveryTruck>(veh, out data1))
                        {
                            trucks++;
                            if(data1.m_State.Equals(DeliveryTruckFlags.DummyTraffic))
                            {
                                truck_flags[0]++;
                            }
                            if (data1.m_State.Equals(DeliveryTruckFlags.Returning))
                            {
                                truck_flags[1]++;
                            }
                            if (data1.m_State.Equals(DeliveryTruckFlags.Buying))
                            {
                                truck_flags[2]++;
                            }
                            if (data1.m_State.Equals(DeliveryTruckFlags.Delivering))
                            {
                                truck_flags[3]++;
                            }
                            if (data1.m_State.Equals(DeliveryTruckFlags.TransactionCancelled))
                            {
                                truck_flags[4]++;
                            }
                        }
                    }

                    string path = Path.Combine(Mod.outputPath, Mod.trucks);
                    string fileName = path +
                        "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";

                    if (index == 0)
                    {
                        string header = "hour,total_trucks,dummy_traffic,returning,buying,delivering,transaction_cancelled";

                        Utils.createAndDeleteFiles(fileName, header, Mod.cars, path);

                    }

                    string line = $"{(float)index / 2f},{trucks}";
                    for (int i = 0; i < 5; i++)
                    {
                        line += $",{truck_flags[i]}";
                    }

                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        public TruckStatistics()
        {
        }
    }
}
