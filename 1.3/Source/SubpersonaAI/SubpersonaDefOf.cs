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

        public static ThingDef O21_AI_SubpersonaShell;
    }
}
