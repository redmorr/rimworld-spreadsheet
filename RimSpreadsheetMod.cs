using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Verse;

namespace RimSpreadsheet
{
    [StaticConstructorOnStartup]
    public static class RimSpreadsheetMod
    {
        private static readonly List<String> armor = new List<String> { "StuffEffectMultiplierArmor", "ArmorRating_Sharp", "ArmorRating_Blunt", "ArmorRating_Heat" };
        private static readonly List<String> insulation = new List<String> { "StuffEffectMultiplierInsulation_Cold", "StuffEffectMultiplierInsulation_Heat", "Insulation_Cold", "Insulation_Heat" };
        private static readonly List<String> layers = new List<String> { "OnSkin", "Middle", "Shell", "Belt", "Overhead", "EyeCover" };
        private static readonly List<String> bodyGroups = new List<String> { "FullHead", "UpperHead", "Neck", "Shoulders", "Arms", "Torso", "Waist", "Legs" };

        private const string trueValue = "X";
        private const string noValue = " ";
        private const string comma = ",";
        private const string multipleValuesSeparator = "; ";
        private const string keyValueSeparator = ": ";

        static RimSpreadsheetMod()
        {
            Log.Message("RimSpreadsheetMod init");
            WriteToApparelsCsv(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\layers3.csv");
            Log.Message("RimSpreadsheetMod fin");
        }

        public static void WriteToApparelsCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                WriteHeaders(sw);
                foreach (ThingDef apparel in GetApparels())
                {
                    sw.Write(apparel.label + comma);
                    WriteApparelStats(sw, apparel, armor, GetApparelStat);
                    WriteApparelStats(sw, apparel, insulation, GetApparelStat);
                    WriteApparelStats(sw, apparel, layers, HasApparelLayerDef);
                    WriteApparelStats(sw, apparel, bodyGroups, HasBodyPartGroupDef);
                    WriteStuff(sw, apparel);
                    WriteEquipedStatOffsets(sw, apparel);
                    sw.Write("\n");
                }
                sw.WriteLine(GetAllBodyGroups());
                sw.WriteLine(GetAllLayers());
            }
        }

        private static void WriteHeaders(StreamWriter sw)
        {
            sw.Write("Name" + comma);
            armor.ForEach(s => sw.Write(s + comma));
            insulation.ForEach(s => sw.Write(s + comma));
            bodyGroups.ForEach(s => sw.Write(s + comma));
            layers.ForEach(s => sw.Write(s + comma));
            sw.Write("Stuff" + comma);
            sw.Write("EquippedStatOffsets" + comma);
        }

        private static void WriteApparelStats(StreamWriter sw, ThingDef apparel, List<String> statNames, Func<ThingDef, string, string> statGetter)
        {
            statNames.ForEach(s => sw.Write(statGetter(apparel, s) + comma));
        }

        public static void WriteStuff(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetApparelStuffCategories(apparel) + comma);
        }

        private static void WriteEquipedStatOffsets(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetEquipedStatOffsets(apparel));
        }

        public static IEnumerable<ThingDef> GetApparels()
        {
            return from s in DefDatabase<ThingDef>.AllDefs
                   where s.IsApparel == true
                   select s;
        }

        public static String GetApparelStat(ThingDef a, string statName)
        {
            StatModifier stat = (from StatModifier s in a.statBases
                                 where s.stat.defName == statName
                                 select s).SingleOrDefault();

            return stat == null ? noValue : stat.value.ToString();
        }

        public static string GetAllLayers()
        {
            return String.Join(multipleValuesSeparator, from ApparelLayerDef c in DefDatabase<ApparelLayerDef>.AllDefs select c.defName);
        }

        public static string GetAllBodyGroups()
        {
            return String.Join(multipleValuesSeparator, from BodyDef c in DefDatabase<BodyDef>.AllDefs select c.defName);
        }

        public static string GetApparelStuffCategories(ThingDef apparelDef)
        {
            if (apparelDef.stuffCategories != null)
            {
                return String.Join(multipleValuesSeparator, from StuffCategoryDef c in apparelDef.stuffCategories select c.defName);
            }
            return noValue;
        }

        private static string GetEquipedStatOffsets(ThingDef apparel)
        {
            if (apparel.equippedStatOffsets != null)
            {
                List<string> statNames = (from StatModifier sm in apparel.equippedStatOffsets select sm.stat.label).ToList();
                List<string> statvalues = (from StatModifier sm in apparel.equippedStatOffsets select sm.value.ToString()).ToList();

                var combined = statNames.Zip(statvalues, (n, v) => new { Name = n, Value = v });

                return String.Join(multipleValuesSeparator, combined.Select(x => x.Name + keyValueSeparator + x.Value).ToList());
            }
            return noValue;
        }

        public static string HasApparelLayerDef(ThingDef apparelDef, string layer)
        {
            IEnumerable<string> layers = from x in apparelDef.apparel.layers
                                         select x.defName;

            return layers.Contains(layer) ? trueValue : noValue;
        }

        public static string HasBodyPartGroupDef(ThingDef apparelDef, string bodyPartGroup)
        {
            IEnumerable<string> bodyPartGroups = from x in apparelDef.apparel.bodyPartGroups
                                                 select x.defName;

            return bodyPartGroups.Contains(bodyPartGroup) ? trueValue : noValue;
        }
    }
}
