using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using BattleTech;

namespace JK_Components
{
    class UncritableComponents
    {
        public static bool CheckingForCrit = false;

        // If we are checking for crit set a flag so we know to ignore components
        [HarmonyPatch(typeof(Mech), "CheckForCrit")]
        public static class Mech_CheckForCrit
        {
            public static void Prefix()
            {
                CheckingForCrit = true;
            }

            public static void Postfix()
            {
                CheckingForCrit = false;
            }
        }

        // If we are checking for crit set a flag so we know to ignore components
        [HarmonyPatch(typeof(Mech), "GetComponentInSlot")]
        public static class Mech_GetComponentInSlot
        {
            public static void Postfix(ref MechComponent __result)
            {
                if(CheckingForCrit)
                {
                    if (__result != null &&
                        __result.componentDef != null &&
                        __result.componentDef.Description != null &&
                        __result.componentDef.Description.Id != null)
                    {
                        if (__result.componentDef.Description.Id.StartsWith("Gear_Ferro") ||
                            __result.componentDef.Description.Id.StartsWith("Gear_Endo") ||
                            __result.componentDef.Description.Id.StartsWith("Gear_HeatSink_Engine") ||
                            __result.componentDef.Description.Id.StartsWith("Gear_XL_Engine"))
                        {
                            __result = null;
                        }
                    }
                }
            }
        }
    }
}
