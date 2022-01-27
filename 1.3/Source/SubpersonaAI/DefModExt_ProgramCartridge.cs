using O21Toolbox.Drones;
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
    public class DefModExt_ProgramCartridge : DefModExtension
    {
        public List<SkillDef> skills = new List<SkillDef>();

        public TraitDef trait = null;
    }
}
