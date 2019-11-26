using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using Harmony;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;

namespace JK_Components
{
    class Ferro
    {
        [HarmonyPatch(typeof(Mech), "get_ArmorMultiplier")]
        public static class Mech_get_ArmorMultiplier
        {
            public static void Postfix(Mech __instance, ref float __result)
            {
                try
                {
                    if (__instance.MechDef.Chassis.ChassisTags.Contains("chassis_ferro"))
                    {
                        __result = __result / 1.12f;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
        public static class MechLabMechInfoWidget_CalculateTonnage_Patch
        {
            public static void Postfix(MechLabMechInfoWidget __instance,
                                       MechLabPanel ___mechLab,
                                       LocalizableText ___totalTonnage,
                                       UIColorRefTracker ___totalTonnageColor,
                                       LocalizableText ___remainingTonnage,
                                       UIColorRefTracker ___remainingTonnageColor)
            {
                try
                {
                    if (___mechLab.activeMechDef != null)
                    {
                        if (___mechLab.activeMechDef.Chassis.ChassisTags.Contains("chassis_ferro"))
                        {
                            float maxTonnage = ___mechLab.activeMechDef.Chassis.Tonnage;

                            float armorTotal = ___mechLab.headWidget.currentArmor;
                            armorTotal += ___mechLab.centerTorsoWidget.currentArmor;
                            armorTotal += ___mechLab.centerTorsoWidget.currentRearArmor;
                            armorTotal += ___mechLab.leftTorsoWidget.currentArmor;
                            armorTotal += ___mechLab.leftTorsoWidget.currentRearArmor;
                            armorTotal += ___mechLab.rightTorsoWidget.currentArmor;
                            armorTotal += ___mechLab.rightTorsoWidget.currentRearArmor;
                            armorTotal += ___mechLab.leftArmWidget.currentArmor;
                            armorTotal += ___mechLab.rightArmWidget.currentArmor;
                            armorTotal += ___mechLab.leftLegWidget.currentArmor;
                            armorTotal += ___mechLab.rightLegWidget.currentArmor;

                            float nonFerroWeight = armorTotal / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);
                            float ferroWeight = armorTotal / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 11.2f);

                            __instance.currentTonnage += ferroWeight - nonFerroWeight;

                            float tonnageFree = maxTonnage - __instance.currentTonnage;
                            UIColor uicolor = (tonnageFree < 0f) ? UIColor.Red : UIColor.WhiteHalf;
                            UIColor uicolor2 = (tonnageFree < 0f) ? UIColor.Red : ((tonnageFree <= 5f) ? UIColor.Gold : UIColor.White);
                            ___totalTonnage.SetText("{0:0.##} / {1}", new object[]
                            {
                                __instance.currentTonnage,
                                maxTonnage
                            });
                            ___totalTonnageColor.SetUIColor(uicolor);
                            if (tonnageFree < 0f)
                            {
                                tonnageFree = Mathf.Abs(tonnageFree);
                                ___remainingTonnage.SetText("{0:0.##} ton{1} overweight", new object[]
                                {
                                    tonnageFree,
                                    (tonnageFree == 1f) ? "" : "s"
                                });
                            }
                            else
                            {
                                ___remainingTonnage.SetText("{0:0.##} ton{1} remaining", new object[]
                                {
                                    tonnageFree,
                                    (tonnageFree == 1f) ? "" : "s"
                                });
                            }
                            ___remainingTonnageColor.SetUIColor(uicolor2);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(MechStatisticsRules), nameof(MechStatisticsRules.CalculateTonnage))]
        public static class MechStatisticsRules_CalculateTonnage_Patch
        {
            public static void Postfix(MechDef mechDef, ref float currentValue)
            {
                try
                {
                    if (mechDef.Chassis.ChassisTags.Contains("chassis_ferro"))
                    {
                        float armorTotal = mechDef.Head.AssignedArmor;
                        armorTotal += mechDef.CenterTorso.AssignedArmor;
                        armorTotal += mechDef.CenterTorso.AssignedRearArmor;
                        armorTotal += mechDef.LeftTorso.AssignedArmor;
                        armorTotal += mechDef.LeftTorso.AssignedRearArmor;
                        armorTotal += mechDef.RightTorso.AssignedArmor;
                        armorTotal += mechDef.RightTorso.AssignedRearArmor;
                        armorTotal += mechDef.LeftArm.AssignedArmor;
                        armorTotal += mechDef.RightArm.AssignedArmor;
                        armorTotal += mechDef.LeftLeg.AssignedArmor;
                        armorTotal += mechDef.RightLeg.AssignedArmor;

                        float nonFerroWeight = armorTotal / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);
                        float ferroWeight = armorTotal / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 11.2f);

                        currentValue += ferroWeight - nonFerroWeight;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
