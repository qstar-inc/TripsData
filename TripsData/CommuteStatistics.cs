using Game.Citizens;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Game.Common;
using Game.Companies;
using Unity.Collections;
using Game.Simulation;
using Colossal.Entities;
using Game.Vehicles;
using System.IO;
using Game.SceneFlow;

namespace TripsData
{
    public partial class CommuteStatistics : GameSystemBase
    {
        private Dictionary<Entity, Worker> _WorkerToData = new Dictionary<Entity, Worker>();

        private EntityQuery _query;
        private int previous_index = -1;
        private int ticksPerDay = TimeSystem.kTicksPerDay;

        protected override void OnCreate()
        {
            base.OnCreate();

            _query = GetEntityQuery(new EntityQueryDesc()
            {
                All = new[] {
                    ComponentType.ReadWrite<Worker>()
                }
            });

            RequireForUpdate(_query);

            //foreach (var modInfo in GameManager.instance.modManager)
            //{
            //    if (modInfo.asset.name.Equals("Time2Work"))
            //    {
            //        ticksPerDay = Time2WorkTimeSystem.kTicksPerDay;
            //    }
            //}
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnUpdate()
        {
            if (Mod.m_Setting.avg_commute)
            {
                DateTime currentDateTime = this.World.GetExistingSystemManaged<TimeSystem>().GetCurrentDateTime();
                int half_hour = 0;
                if (currentDateTime.Minute >= 30)
                {
                    half_hour = 1;
                }
                int index = 2 * currentDateTime.Hour + half_hour;

                if (previous_index != index && currentDateTime.Hour == 23 && half_hour == 1)
                {
                    previous_index = index;
                    var results = _query.ToEntityArray(Allocator.Temp);

                    int bin_size = 30;
                    int[] commute_15min_bins = new int[bin_size];

                    foreach (var veh in results)
                    {
                        Worker data1;

                        if (EntityManager.TryGetComponent<Worker>(veh, out data1))
                        {
                            float commute = 24f * 60f * 60f * (data1.m_LastCommuteTime / ticksPerDay);

                            int b = (int)Math.Floor(commute / 15f);
                            if (b > (bin_size - 1))
                            {
                                b = bin_size - 1;
                            }
                            commute_15min_bins[b]++;
                        }
                    }

                    string path = Path.Combine(Mod.outputPath, "commute_data");
                    string fileName = path +
                        "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";


                    string header = "bin_15_min,frequency";

                    Utils.createAndDeleteFiles(fileName, header, "commute_data", path);

                    for (int i = 0; i < bin_size; i++)
                    {                       
                        using (StreamWriter sw = File.AppendText(fileName))
                        {
                            string line = $"{i*15},{commute_15min_bins[i]}";
                            sw.WriteLine(line);
                        }
                    }

                    
                }
            }
        }
    }
}
