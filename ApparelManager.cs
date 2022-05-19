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

        private static readonly List<Def> armor = new List<Def> { StatDefOf.StuffEffectMultiplierArmor, StatDefOf.ArmorRating_Sharp, StatDefOf.ArmorRating_Blunt, StatDefOf.ArmorRating_Heat };
        private static readonly List<Def> insulation = new List<Def> { StatDefOf.StuffEffectMultiplierInsulation_Cold, StatDefOf.StuffEffectMultiplierInsulation_Heat, StatDefOf.Insulation_Cold, StatDefOf.Insulation_Heat };
        private static readonly List<Def> layers = new List<Def> { ApparelLayerDefOf.OnSkin, ApparelLayerDefOf.Middle, ApparelLayerDefOf.Shell, ApparelLayerDefOf.Belt, ApparelLayerDefOf.Overhead, ApparelLayerDefOf.EyeCover };
        private static readonly List<Def> bodyGroups = new List<Def> { BodyPartGroupDefOf.FullHead, BodyPartGroupDefOf.UpperHead, BodyPartGroupDefOf.Eyes, BodyPartGroupDefOf.Torso, BodyPartGroupDefOf.LeftHand, BodyPartGroupDefOf.RightHand, BodyPartGroupDefOf.Legs };


        public static void WriteToApparelsCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                WriteHeaders(sw);
                foreach (ThingDef apparel in GetApparels())
                {
                    var a = BodyDefOf.Human;
                    var b = BodyPartDefOf.Arm;
                    var c = BodyPartGroupDefOf.UpperHead;
                    sw.Write(apparel.label + Common.comma);
                    Common.WriteStats(sw, apparel, armor, Common.GetStatBase);
                    Common.WriteStats(sw, apparel, insulation, Common.GetStatBase);
                    Common.WriteStats(sw, apparel, layers, HasApparelLayerDef);
                    Common.WriteStats(sw, apparel, bodyGroups, HasBodyPartGroupDef);
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

        public static string HasApparelLayerDef(ThingDef apparelDef, Def layer)
        {
            return (from l in apparelDef.apparel.layers select l).Contains(layer) ? Common.trueValue : Common.noValue;
        }

        public static string HasBodyPartGroupDef(ThingDef apparelDef, Def bodyPartGroup)
        {
            return (from bpg in apparelDef.apparel.bodyPartGroups select bpg).Contains(bodyPartGroup) ? Common.trueValue : Common.noValue;
        }

        private static void WriteEquipedStatOffsets(StreamWriter sw, ThingDef apparel)
        {
            sw.Write(GetEquipedStatOffsets(apparel));
        }
    }
}
