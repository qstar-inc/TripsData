using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.Internal;
using Game.Modding;
using Game.Settings;
using Game.UI.InGame;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using System.IO;

namespace TripsData
{
    [FileLocation(nameof(TripsData))]
    [SettingsUIGroupOrder(SettingsGroup, TripsDataGroup, CarsDataGroup, TransitDataGroup, TruckDataGroup, SimulationDataGroup)]
    [SettingsUIShowGroupName(SettingsGroup, TripsDataGroup, CarsDataGroup, TransitDataGroup, TruckDataGroup, SimulationDataGroup)]
    public class Setting : ModSetting
    {
        public const string SettingsSection = "Settings";

        public const string SettingsGroup = "Settings";
        public const string TripsDataGroup = "Trips Data";
        public const string CarsDataGroup = "Cars Data";
        public const string TransitDataGroup = "Transit Data";
        public const string TruckDataGroup = "Truck Data";
        public const string SimulationDataGroup = "Simulation Data";

        public Setting(IMod mod) : base(mod)
        {

        }

        public override void SetDefaults()
        {


        }

        [SettingsUISection(SettingsSection, SettingsGroup)]
        [SettingsUIMultilineText]
        public string MultilineText => string.Empty;

        [SettingsUIDropdown(typeof(Setting), nameof(GetIntDropdownItems))]
        [SettingsUISection(SettingsSection, SettingsGroup)]
        public int numOutputs { get; set; } = 3;
        [SettingsUISection(SettingsSection, TripsDataGroup)]
        public bool trip_type { get; set; } = false;

        [SettingsUISection(SettingsSection, TripsDataGroup)]
        public bool citizen_purpose { get; set; } = false;

        [SettingsUISection(SettingsSection, CarsDataGroup)]
        public bool cars { get; set; } = false;

        [SettingsUISection(SettingsSection, TransitDataGroup)]
        public bool transit { get; set; } = false;

        [SettingsUISection(SettingsSection, TruckDataGroup)]
        public bool truck { get; set; } = false;

        [SettingsUISection(SettingsSection, SimulationDataGroup)]
        public bool smooth_speed { get; set; } = false;


        public DropdownItem<int>[] GetIntDropdownItems()
        {
            var items = new List<DropdownItem<int>>();
        
            for (var i = 1; i < 11; i += 1)
            {
                items.Add(new DropdownItem<int>()
                {
                    value = i,
                    displayName = i.ToString(),
                });
            }
        
            return items.ToArray();
        }


    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "TripsData" },
                { m_Setting.GetOptionTabLocaleID(Setting.SettingsSection), "Settings" },

                { m_Setting.GetOptionGroupLocaleID(Setting.SettingsGroup), "Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.TripsDataGroup), "Trips Data" },
                { m_Setting.GetOptionGroupLocaleID(Setting.CarsDataGroup), "Cars Data" },
                { m_Setting.GetOptionGroupLocaleID(Setting.TransitDataGroup), "Transit Data" },
                { m_Setting.GetOptionGroupLocaleID(Setting.TruckDataGroup), "Truck Data" },
                { m_Setting.GetOptionGroupLocaleID(Setting.SimulationDataGroup), "Simulation Data" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.MultilineText)), $"Output Folder:\n{Mod.outputPath}" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.numOutputs)), "Number of output files to keep saved" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.numOutputs)), $"Number of output files to keep saved" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.citizen_purpose)), "Citizen Game Purposes by every half hour" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.citizen_purpose)), $"The purposes include: Going Home, Going to School, Going to Work, etc." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.trip_type)), "Number of trips by type and by every half hour" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.trip_type)), $"There are four trip types: Home Based Work (Work to Home and Home to Work), Home Based School (Home to School and School to Home), Home Based Other (All trips that start or end at home but don't have work or school as the origin or destination), Non Home Based (All trips that don't have home as the origin or destination)" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.cars)), "Number of cars parked" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.cars)), $"Number of cars parked by every half hour" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.transit)), "Transit Passenger Waiting Statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.transit)), $"Number of passengers waiting for transit and average waiting time (in minutes) for every half hour" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.truck)), "Number of Trucks and Truck status" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.truck)), $"Number of trucks and truck status for every half hour" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.smooth_speed)), "Smooth Speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.smooth_speed)), $"Smooth Speed is collected around every 20 minutes." },
            };
        }

        public void Unload()
        {

        }
    }
}
