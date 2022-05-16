using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSpreadsheet
{
    internal class WeaponManager
    {
        private static readonly List<String> rangedAccuracy = new List<String> { "AccuracyTouch", "AccuracyShort", "AccuracyMedium", "AccuracyLong", "RangedWeapon_Cooldown" };

        public static void WriteToWeaponsCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                WriteHeaders(sw);
                foreach (ThingDef rangedWeapon in GetRangedWeapons())
                {
                    sw.Write(rangedWeapon.label + Common.comma);
                    Common.WriteStatBases(sw, rangedWeapon, rangedAccuracy, Common.GetStatBase);
                    WriteVerbs(sw, rangedWeapon);
                    WriteDefaultProjectile(sw, rangedWeapon);
                    sw.Write("\n");
                }
            }
        }

        private static void WriteHeaders(StreamWriter sw)
        {
            sw.Write("Name" + Common.comma);
            rangedAccuracy.ForEach(s => sw.Write(s + Common.comma));
            sw.Write("\n");
        }

        public static void WriteVerbs(StreamWriter sw, ThingDef weapon)
        {
            if (weapon.Verbs != null)
            {
                sw.Write(String.Join(Common.multipleValuesSeparator, from VerbProperties verb in weapon.Verbs select verb.label) + Common.comma);
                sw.Write(String.Join(Common.multipleValuesSeparator, from VerbProperties verb in weapon.Verbs select verb.range) + Common.comma);
                sw.Write(String.Join(Common.multipleValuesSeparator, from VerbProperties verb in weapon.Verbs select verb.warmupTime) + Common.comma);
                
            } 
            else
            {
                sw.Write(Common.noValue + Common.comma);
            }
        }

        public static void WriteDefaultProjectile(StreamWriter sw, ThingDef weapon)
        {
            ThingDef defaultProjectile = (from VerbProperties verb in weapon.Verbs select verb.defaultProjectile).FirstOrDefault();
            if (defaultProjectile != null)
            {
                sw.Write(defaultProjectile.label);
            }
            else
            {
                sw.Write(Common.noValue + Common.comma);
            }
        }

        public static IEnumerable<ThingDef> GetRangedWeapons()
        {
            return from thingDef in DefDatabase<ThingDef>.AllDefs
                   where thingDef.IsRangedWeapon == true
                   select thingDef;
        }
    }
}
