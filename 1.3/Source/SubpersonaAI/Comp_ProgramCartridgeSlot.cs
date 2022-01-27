using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using O21Toolbox.SlotLoadable;

namespace SubpersonaAI
{
    public class Comp_ProgramCartridgeSlot : Comp_SlotLoadable
    {
        public Pawn pawn => parent as Pawn;

        public override void PostPostMake()
        {
            base.PostPostMake();
        }

        public void TryCancel(string reason = "")
        {
            if (pawn != null)
            {
                if (pawn.CurJob.def == O21Toolbox.Utility.JobDefOf.O21_GatherSlotItem)
                {
                    pawn.jobs.StopAll();
                }
                isGathering = false;
            }
        }

        private void TryGiveLoadSlotJob(Thing itemToLoad)
        {
            if (pawn != null)
            {
                if (!pawn.Drafted)
                {
                    isGathering = true;

                    var job = JobMaker.MakeJob(SubpersonaDefOf.O21_GatherProgramItem, itemToLoad);
                    job.count = 1;
                    pawn.jobs.TryTakeOrderedJob(job);
                    //GetPawn.jobs.jobQueue.EnqueueFirst(job);
                    //GetPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
                else
                {
                    Messages.Message(string.Format("{0} is drafted.", new object[]
                    {
                        pawn.Name.ToString()
                    }), MessageTypeDefOf.RejectInput);
                }
            }
        }

        public new void ProcessInput(SlotLoadable slot)
        {
            var floatList = new List<FloatMenuOption>();
            if (!isGathering)
            {
                var map = pawn.Map;
                if (slot.SlotOccupant == null && slot.SlottableTypes is List<ThingDef> loadTypes)
                {
                    if (loadTypes.Count > 0)
                    {
                        foreach (var current in loadTypes)
                        {
                            var thingToLoad = map.listerThings.ThingsOfDef(current).FirstOrDefault(x => map.reservationManager.CanReserve(pawn, x));
                            if (thingToLoad != null)
                            {
                                var text = "Load".Translate() + " " + thingToLoad.def.label;
                                floatList.Add(new FloatMenuOption(text, delegate { TryGiveLoadSlotJob(thingToLoad); }, MenuOptionPriority.Default, null, null, 29f, null, null));
                            }
                            else
                            {
                                FloatMenuOption option = new FloatMenuOption(string.Format("{0} unavailable", new object[] { current.label }), delegate { }, MenuOptionPriority.Default);
                                option.Disabled = true;
                                floatList.Add(option);
                            }
                        }
                    }
                    else
                    {
                        FloatMenuOption option = new FloatMenuOption("No load options available.", delegate { }, MenuOptionPriority.Default);
                        option.Disabled = true;
                        floatList.Add(option);
                    }
                }
            }
            if (!slot.IsEmpty())
            {
                var text = string.Format("Unload {0}", new object[] { slot.SlotOccupant.Label });
                floatList.Add(new FloatMenuOption(text, delegate { TryEmptySlot(slot); }, MenuOptionPriority.Default, null, null, 29f, null, null));
            }
            if(isGathering)
            {
                isGathering = false;
            }
            Find.WindowStack.Add(new FloatMenu(floatList));
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            for (int i = 0; i < base.CompGetGizmosExtra().Count(); i++)
            {
                yield return base.CompGetGizmosExtra().ElementAt(i);
            }
            if (!Slots.NullOrEmpty() && pawn.Faction.IsPlayer)
            {
                if (isGathering)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Designator_Cancel".Translate(),
                        defaultDesc = "Designator_CancelDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                        action = delegate { TryCancel(); }
                    };
                }
                foreach (var slot in Slots)
                {
                    if (slot.IsEmpty())
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = slot.LabelNoCount,
                            icon = Command.BGTex,
                            defaultDesc = SlotDesc(slot),
                            action = delegate { ProcessInput(slot); }
                        };
                    }
                    else
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = slot.LabelNoCount,
                            icon = slot.SlotIcon(),
                            defaultDesc = SlotDesc(slot),
                            defaultIconColor = slot.SlotColor(),
                            action = delegate { ProcessInput(slot); }
                        };
                    }
                }
            }
        }

        public override bool TryLoadSlot(Thing thing)
        {
            if (base.TryLoadSlot(thing))
            {
                DefModExt_ProgramCartridge modExt = thing.def.GetModExtension<DefModExt_ProgramCartridge>();
                if(modExt != null && !modExt.skills.NullOrEmpty())
                {
                    ResetSkills();
                    SetSkillsFromQuality(thing);
                    if (modExt.trait != null)
                    {
                        for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
                        {
                            pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
                        }
                        pawn.story.traits.GainTrait(new Trait(modExt.trait));
                    }
                }
                return true;
            }

            return false;
        }

        public override void TryEmptySlot(SlotLoadable slot)
        {
            base.TryEmptySlot(slot);
            ResetSkills();
            for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
            {
                pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
            }
            pawn.story.traits.GainTrait(new Trait(SubpersonaDefOf.O21_AI_ShellStandard));
        }

        public void ResetSkills()
        {
            for (int i = 0; i < pawn.skills.skills.Count(); i++)
            {
                pawn.skills.skills[i].levelInt = 0;
                pawn.skills.skills[i].xpSinceLastLevel = 0;
            }
        }

        public void SetSkillsFromQuality(Thing thing)
        {
            DefModExt_ProgramCartridge modExt = thing.def.GetModExtension<DefModExt_ProgramCartridge>();
            if (modExt != null && !modExt.skills.NullOrEmpty())
            {
                for (int i = 0; i < modExt.skills.Count(); i++)
                {
                    QualityCategory quality = QualityCategory.Awful;
                    thing.TryGetQuality(out quality);
                    pawn.skills.skills.Find(s => s.def == modExt.skills[i]).levelInt = GetLevelFromQuality(quality);
                }
            }
        }

        public int GetLevelFromQuality(QualityCategory quality)
        {
            int result = 0;
            switch (quality)
            {
                case QualityCategory.Awful:
                    result = 3;
                    break;
                case QualityCategory.Poor:
                    result = 6;
                    break;
                case QualityCategory.Normal:
                    result = 9;
                    break;
                case QualityCategory.Good:
                    result = 12;
                    break;
                case QualityCategory.Excellent:
                    result = 15;
                    break;
                case QualityCategory.Masterwork:
                    result = 18;
                    break;
                case QualityCategory.Legendary:
                    result = 20;
                    break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }
    }
}
