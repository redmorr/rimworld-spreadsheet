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
        static private List<String> armor = new List<String> { "StuffEffectMultiplierArmor", "ArmorRating_Sharp", "ArmorRating_Blunt", "ArmorRating_Heat" };
        static private List<String> insulation = new List<String> { "StuffEffectMultiplierInsulation_Cold", "StuffEffectMultiplierInsulation_Heat", "Insulation_Cold", "Insulation_Heat" };
        static private List<String> bodyGroups = new List<String> { "FullHead", "UpperHead", "Neck", "Shoulders", "Arms", "Torso", "Waist", "Legs" };
        static private List<String> layers = new List<String> { "OnSkin", "Middle", "Shell", "Belt", "Overhead", "EyeCover" };

        static RimSpreadsheetMod()
        {
            Log.Message("Hello World!");
            writeToApparelsCsv(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\layers3.csv");
            Log.Message("Goodbye World!");
        }

        public static void writeToApparelsCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                WriteHeaders(sw);
                foreach (ThingDef apparel in GetApparels())
                {
                    sw.Write(apparel.label + ",");
                    WriteArmors(sw, apparel, armor);
                    WriteInsulation(sw, apparel, insulation);
                    WriteLayers(sw, apparel, layers);
                    WriteBodyGroups(sw, apparel, bodyGroups);
                    WriteStuff(sw, apparel);
                    WriteEquipedStatOffsets(sw, apparel);
                    sw.Write("\n");
                }
                sw.WriteLine(GetAllBodyGroups());
                sw.WriteLine(GetAllLayers());
            }
        }

        private static string GetEquipedStatOffsets(ThingDef apparel)
        {
            if (apparel.equippedStatOffsets != null)
            {
                List<string> statNames = (from StatModifier sm in apparel.equippedStatOffsets select sm.stat.label).ToList();
                List<string> statvalues = (from StatModifier sm in apparel.equippedStatOffsets select sm.value.ToString()).ToList();

                var combined = statNames.Zip(statvalues, (n, v) => new { Name = n, Value = v });

                return String.Join(" ; ", combined.Select(x => x.Name + ": " + x.Value).ToList());
            }
            return " ";
        }

        private static void WriteEquipedStatOffsets(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetEquipedStatOffsets(apparel));
        }

        private static void WriteHeaders(StreamWriter sw)
        {
            sw.Write("Name" + ",");
            armor.ForEach(s => sw.Write(s + ","));
            insulation.ForEach(s => sw.Write(s + ","));
            bodyGroups.ForEach(s => sw.Write(s + ","));
            layers.ForEach(s => sw.Write(s + ","));
            sw.Write("Stuff" + ",");
            sw.Write("EquippedStatOffsets" + ",");
        }

        private static void WriteArmors(StreamWriter sw, ThingDef apparel, List<String> statNames)
        {
            statNames.ForEach(s => sw.Write(GetApparelStat(apparel, s) + ","));
        }

        private static void WriteInsulation(StreamWriter sw, ThingDef apparel, List<String> statNames)
        {
            statNames.ForEach(s => sw.Write(GetApparelStat(apparel, s) + ","));
        }

        private static void WriteBodyGroups(StreamWriter sw, ThingDef apparel, List<String> statNames)
        {
            statNames.ForEach(s => sw.Write(HasBodyPartGroupDef(apparel, s) + ","));
        }

        public static void WriteLayers(StreamWriter sw, ThingDef apparel, List<String> statNames)
        {
            statNames.ForEach(s => sw.Write(HasApparelLayerDef(apparel, s) + ","));

        }

        public static void WriteStuff(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetApparelStuffCategories(apparel) + ",");
        }


        public static IEnumerable<BodyPartGroupDef> GetBodyPartGroupDefs()
        {
            return from s in DefDatabase<BodyPartGroupDef>.AllDefs
                   select s;
        }

        public static string HasApparelLayerDef(ThingDef apparelDef, string layer)
        {
            IEnumerable<string> layers = from x in apparelDef.apparel.layers
                                         select x.defName;

            if (layers.Contains(layer))
            {
                return "X";
            }
            else
            {
                return " ";
            }
        }

        public static string HasBodyPartGroupDef(ThingDef apparelDef, string bodyPartGroup)
        {
            IEnumerable<string> bodyPartGroups = from x in apparelDef.apparel.bodyPartGroups
                                                 select x.defName;

            if (bodyPartGroups.Contains(bodyPartGroup))
            {
                return "X";
            }
            else
            {
                return " ";
            }
        }


        public static string GetApparelStuffCategories(ThingDef apparelDef)
        {
            if (apparelDef.stuffCategories != null)
            {
                return String.Join(" / ", from StuffCategoryDef c in apparelDef.stuffCategories select c.defName);

            }

            return " ";
        }

        public static string GetAllBodyGroups()
        {
            return String.Join(" / ", from BodyDef c in DefDatabase<BodyDef>.AllDefs select c.defName);
        }

        public static string GetAllLayers()
        {
            return String.Join(" / ", from ApparelLayerDef c in DefDatabase<ApparelLayerDef>.AllDefs select c.defName);
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
            if (stat == null)
            {
                return " ";
            }
            else
            {
                return stat.value.ToString();
            }
        }

        public static IEnumerable<StatModifier> GetApparelHitPoints(ThingDef a)
        {
            return from StatModifier s in a.statBases
                   where s.stat.defName == "MaxHitPoints"
                   select s;
        }

        public static IEnumerable<ThingDef> GetTextiles()
        {
            return from r in DefDatabase<ThingDef>.AllDefs
                   where (r.category == ThingCategory.Item && r.FirstThingCategory != null && r.FirstThingCategory.defName == "Textiles")
                   select r;
        }

    }
}
