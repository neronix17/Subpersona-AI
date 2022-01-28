using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SubpersonaAI
{
    [DefOf]
    public static class SubpersonaDefOf
    {
        static SubpersonaDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SubpersonaDefOf));
        }

        public static TraitDef O21_AI_ShellStandard;

        public static JobDef O21_GatherProgramItem;
        public static JobDef O21_AI_EnterReconstructor;
        public static JobDef O21_AI_CarryToReconstructor;

        public static ThingDef O21_AI_SubpersonaShell;

        public static SoundDef O21_AI_OvenDing;
    }
}
