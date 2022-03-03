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
    public class SubpersonaMod : Mod
    {
        public SubpersonaMod mod;

        public SubpersonaMod(ModContentPack content) : base(content)
        {
            this.mod = this;

            Log.Message(":: Subpersona Shells :: 1.0.0 ::");

            new Harmony("neronix17.subpersonashells.rimworld").PatchAll();
        }
    }


}
