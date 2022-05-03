using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using Verse.AI;

namespace SubpersonaAI
{
    [HarmonyPatch(typeof(HealthAIUtility), "ShouldEverReceiveMedicalCareFromPlayer")]
    public class Patch_HealthAIUtility_ShouldEverReceiveMedicalCareFromPlayer
    {
        [HarmonyPostfix]
        public static bool Prefix(Pawn pawn, bool __result)
        {
            if (pawn != null && pawn.def == SubpersonaDefOf.SubAI_SubpersonaShell)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
