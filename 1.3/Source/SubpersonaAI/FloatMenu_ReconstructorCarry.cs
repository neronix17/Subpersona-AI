using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using O21Toolbox.Utility;
using Verse.AI;

namespace SubpersonaAI
{
    public class FloatMenu_ReconstructorCarry : HumanlikeOrdersUtility.FloatMenuPatch
	{
        public override IEnumerable<KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>> GetFloatMenus()
		{
			var FloatMenus = new List<KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>();

			var curCondition = new HumanlikeOrdersUtility._Condition(HumanlikeOrdersUtility._ConditionType.ThingHasComp, typeof(Comp_ProgramCartridgeSlot));

			List<FloatMenuOption> CurFunc(Vector3 clickPos, Pawn pawn, Thing curThing)
            {
                var opts = new List<FloatMenuOption>();

                var c = clickPos.ToIntVec3();

				foreach (LocalTargetInfo item in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), thingsOnly: true))
				{
					LocalTargetInfo localTargetInfo = item;
					Pawn victim = (Pawn)localTargetInfo.Thing;
					if (!victim.Downed || !pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true) || Building_SubpersonaReconstructor.FindReconstructorFor(victim, pawn, ignoreOtherReservations: true) == null)
					{
						continue;
					}
					string text2 = "SubAI_CarryToSubpersonaReconstructor".Translate(localTargetInfo.Thing.LabelCap, localTargetInfo.Thing);
					JobDef jDef = SubpersonaDefOf.O21_AI_CarryToReconstructor;
					Action action2 = delegate
					{
						Building_SubpersonaReconstructor building_CryptosleepCasket = Building_SubpersonaReconstructor.FindReconstructorFor(victim, pawn);
						if (building_CryptosleepCasket == null)
						{
							building_CryptosleepCasket = Building_SubpersonaReconstructor.FindReconstructorFor(victim, pawn, ignoreOtherReservations: true);
						}
						if (building_CryptosleepCasket == null)
						{
							Messages.Message("SubAI_CannotCarryToSubpersonaReconstructor".Translate() + ": " + "SubAI_NoSubpersonaReconstructor".Translate(), victim, MessageTypeDefOf.RejectInput, historical: false);
						}
						else
						{
							Job job19 = JobMaker.MakeJob(jDef, victim, building_CryptosleepCasket);
							job19.count = 1;
							pawn.jobs.TryTakeOrderedJob(job19, JobTag.Misc);
						}
					};
					if (victim.IsQuestLodger())
					{
						text2 += " (" + "SubAI_SubpersonaReconstructorGuestsNotAllowed".Translate() + ")";
						opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text2, null, MenuOptionPriority.Default, null, victim), pawn, victim));
					}
					else if (victim.GetExtraHostFaction() != null)
					{
						text2 += " (" + "SubAI_SubpersonaReconstructorGuestPrisonersNotAllowed".Translate() + ")";
						opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text2, null, MenuOptionPriority.Default, null, victim), pawn, victim));
					}
					else
					{
						opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text2, action2, MenuOptionPriority.Default, null, victim), pawn, victim));
					}
				}
				return opts;
            }

            KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>> curSec =
                new KeyValuePair<HumanlikeOrdersUtility._Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>(curCondition, CurFunc);
            FloatMenus.Add(curSec);
            return FloatMenus;
        }
    }
}
