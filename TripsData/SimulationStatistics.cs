using Game;
using Game.Debug;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripsData
{
    public partial class SimulationStatistics : GameSystemBase
    {
        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // One day (or month) in-game is '262144' ticks
            return TimeSystem.kTicksPerDay / 64;
        }

        protected override void OnUpdate()
        {
            if(Mod.m_Setting.smooth_speed)
            {
                DateTime currentDateTime = this.World.GetExistingSystemManaged<TimeSystem>().GetCurrentDateTime();
                int half_hour = 0;
                if (currentDateTime.Minute >= 30)
                {
                    half_hour = 1;
                }
                int index = 2 * currentDateTime.Hour + half_hour;
                SimulationSystem sim = World.GetExistingSystemManaged<SimulationSystem>();
                float speed = sim.smoothSpeed;

                string path = Path.Combine(Mod.outputPath, Mod.smoothspeed);
                string fileName = path +
                    "_" + currentDateTime.DayOfYear + "_" + currentDateTime.Year + ".csv";

                if (index == 0)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    Mod.log.Info($"Creating file: {fileName}");
                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine($"hour,smooth_speed");
                    }

                    // Get the files
                    DirectoryInfo info = new DirectoryInfo(Mod.outputPath);
                    FileInfo[] files = info.GetFiles(Mod.cimpurposeoutput + "*");

                    // Sort by creation-time descending 
                    Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
                    {
                        return f2.CreationTime.CompareTo(f1.CreationTime);
                    });

                    while (files.Length > Mod.m_Setting.numOutputs)
                    {
                        Mod.log.Info($"Deleting: {files[0].FullName}");
                        File.Delete(files[0].FullName);

                        // Get the files
                        info = new DirectoryInfo(path);
                        files = info.GetFiles(Mod.tripsoutput + "*");

                        // Sort by creation-time descending 
                        Array.Sort(files, delegate (FileInfo f1, FileInfo f2)
                        {
                            return f2.CreationTime.CompareTo(f1.CreationTime);
                        });
                    }
                }

                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine($"{(float)(currentDateTime.Hour + currentDateTime.Minute / 60f)},{speed}");
                }
            }           
        }
    }
}
