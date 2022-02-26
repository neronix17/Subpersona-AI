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

        public static TraitDef SubAI_ShellStandard;

        public static JobDef O21_GatherProgramItem;
        public static JobDef SubAI_EnterReconstructor;
        public static JobDef SubAI_CarryToReconstructor;

        public static ThingDef SubAI_SubpersonaShell;

        public static SoundDef SubAI_OvenDing;
    }
}
