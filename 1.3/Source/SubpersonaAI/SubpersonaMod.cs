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
        public SubpersonaSettings settings;

        public SubpersonaMod(ModContentPack content) : base(content)
        {
            this.mod = this;
            this.settings = GetSettings<SubpersonaSettings>();

            new Harmony("neronix17.subpersonashells.rimworld").PatchAll();
        }

        public override string SettingsCategory() => "SubpersonaAI";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }
    }


}
