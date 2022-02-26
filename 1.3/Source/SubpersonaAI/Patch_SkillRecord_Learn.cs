using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace SubpersonaAI
{
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public class Patch_SkillRecord_Learn
    {
        [HarmonyPrefix]
        public static bool Prefix(float xp, bool direct, SkillRecord __instance)
        {
            Pawn pawn = __instance?.pawn;
            if (pawn?.def == SubpersonaDefOf.SubAI_SubpersonaShell)
            {
                return false;
            }

            return true;
        }
    }
}
