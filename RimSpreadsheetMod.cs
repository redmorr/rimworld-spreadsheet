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
        static RimSpreadsheetMod()
        {
            Log.Message("RimSpreadsheetMod init");
            ApparelManager.WriteToApparelsCsv(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\apparels.csv");
            WeaponManager.WriteToWeaponsCsv(Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\weapons.csv");
            Log.Message("RimSpreadsheetMod fin");
        }
    }
}
