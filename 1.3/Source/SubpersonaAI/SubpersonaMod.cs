using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using System.Reflection;
using System.IO;

namespace SubpersonaAI
{
    public class SubpersonaMod : Mod
    {
        public SubpersonaMod mod;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Neronix17.SubpersonaShells").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public SubpersonaMod(ModContentPack content) : base(content)
        {
            this.mod = this;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            Log.Message($":: Subpersona Shells :: {CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);

            new Harmony("neronix17.subpersonashells.rimworld").PatchAll();
        }
    }


}
