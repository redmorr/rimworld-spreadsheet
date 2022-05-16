using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimSpreadsheet
{
    internal class Common
    {
        public const string trueValue = "X";
        public const string noValue = " ";
        public const string comma = ",";
        public const string multipleValuesSeparator = "; ";
        public const string keyValueSeparator = ": ";

        public static void WriteStatBases(StreamWriter sw, ThingDef apparel, List<String> statNames, Func<ThingDef, string, string> statGetter)
        {
            statNames.ForEach(s => sw.Write(statGetter(apparel, s) + comma));
        }

        public static String GetStatBase(ThingDef thingDef, string statBaseDefName)
        {
            StatModifier stat = (from StatModifier s in thingDef.statBases
                                 where s.stat.defName == statBaseDefName
                                 select s).SingleOrDefault();

            return stat == null ? Common.noValue : stat.value.ToString();
        }
    }
}
