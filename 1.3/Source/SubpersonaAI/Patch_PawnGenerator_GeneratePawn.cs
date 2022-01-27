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
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(PawnGenerationRequest request, Pawn __result)
        {
            Pawn pawn = __result;
            if(pawn.def == SubpersonaDefOf.O21_AI_SubpersonaShell)
            {
                for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
                {
                    pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
                }
                pawn.story.traits.GainTrait(new Trait(SubpersonaDefOf.O21_AI_ShellStandard));
            }
        }
    }
}
