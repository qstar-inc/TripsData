using Game;
using Game.Debug;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
                    string header = "hour,smooth_speed";

                    Utils.createAndDeleteFiles(fileName, header, Mod.smoothspeed, path);

                }

                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine($"{(float)(currentDateTime.Hour + currentDateTime.Minute / 60f)},{speed}");
                }
            }           
        }
    }
}
