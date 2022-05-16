using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace RimSpreadsheet
{
    internal class ApparelManager
    {

        private static readonly List<String> armor = new List<String> { "StuffEffectMultiplierArmor", "ArmorRating_Sharp", "ArmorRating_Blunt", "ArmorRating_Heat" };
        private static readonly List<String> insulation = new List<String> { "StuffEffectMultiplierInsulation_Cold", "StuffEffectMultiplierInsulation_Heat", "Insulation_Cold", "Insulation_Heat" };
        private static readonly List<String> layers = new List<String> { "OnSkin", "Middle", "Shell", "Belt", "Overhead", "EyeCover" };
        private static readonly List<String> bodyGroups = new List<String> { "FullHead", "UpperHead", "Neck", "Shoulders", "Arms", "Torso", "Waist", "Legs" };


        public static void WriteToApparelsCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                WriteHeaders(sw);
                foreach (ThingDef apparel in GetApparels())
                {
                    sw.Write(apparel.label + Common.comma);
                    Common.WriteStatBases(sw, apparel, armor, Common.GetStatBase);
                    Common.WriteStatBases(sw, apparel, insulation, Common.GetStatBase);
                    Common.WriteStatBases(sw, apparel, layers, HasApparelLayerDef);
                    Common.WriteStatBases(sw, apparel, bodyGroups, HasBodyPartGroupDef);
                    WriteStuff(sw, apparel);
                    WriteEquipedStatOffsets(sw, apparel);
                    sw.Write("\n");
                }
            }
        }

        private static void WriteHeaders(StreamWriter sw)
        {
            sw.Write("Name" + Common.comma);
            armor.ForEach(s => sw.Write(s + Common.comma));
            insulation.ForEach(s => sw.Write(s + Common.comma));
            bodyGroups.ForEach(s => sw.Write(s + Common.comma));
            layers.ForEach(s => sw.Write(s + Common.comma));
            sw.Write("Stuff" + Common.comma);
            sw.Write("EquippedStatOffsets" + Common.comma);
            sw.Write("\n");
        }

        public static IEnumerable<ThingDef> GetApparels()
        {
            return from s in DefDatabase<ThingDef>.AllDefs
                   where s.IsApparel == true
                   select s;
        }

        public static void WriteStuff(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetApparelStuffCategories(apparel) + Common.comma);
        }

        public static string GetApparelStuffCategories(ThingDef apparelDef)
        {
            if (apparelDef.stuffCategories != null)
            {
                return String.Join(Common.multipleValuesSeparator, from StuffCategoryDef c in apparelDef.stuffCategories select c.defName);
            }
            return Common.noValue;
        }

        private static string GetEquipedStatOffsets(ThingDef apparel)
        {
            if (apparel.equippedStatOffsets != null)
            {
                List<string> statNames = (from StatModifier sm in apparel.equippedStatOffsets select sm.stat.label).ToList();
                List<string> statvalues = (from StatModifier sm in apparel.equippedStatOffsets select sm.value.ToString()).ToList();

                var combined = statNames.Zip(statvalues, (n, v) => new { Name = n, Value = v });

                return String.Join(Common.multipleValuesSeparator, combined.Select(x => x.Name + Common.keyValueSeparator + x.Value).ToList());
            }
            return Common.noValue;
        }

        public static string HasApparelLayerDef(ThingDef apparelDef, string layer)
        {
            IEnumerable<string> layers = from x in apparelDef.apparel.layers
                                         select x.defName;

            return layers.Contains(layer) ? Common.trueValue : Common.noValue;
        }

        public static string HasBodyPartGroupDef(ThingDef apparelDef, string bodyPartGroup)
        {
            IEnumerable<string> bodyPartGroups = from x in apparelDef.apparel.bodyPartGroups
                                                 select x.defName;

            return bodyPartGroups.Contains(bodyPartGroup) ? Common.trueValue : Common.noValue;
        }

        private static void WriteEquipedStatOffsets(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetEquipedStatOffsets(apparel));
        }
    }
}
