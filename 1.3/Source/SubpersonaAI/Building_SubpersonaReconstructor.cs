using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace SubpersonaAI
{
    public class Building_SubpersonaReconstructor : Building_CorpseCasket
	{
		public const int repairTime = 60000;

		public bool isRepairing = false;

		public int ticksRemaining = 0;

		public override void ExposeData()
        {
            base.ExposeData();

			Scribe_Values.Look(ref isRepairing, "isRepairing", false);
			Scribe_Values.Look(ref ticksRemaining, "ticksRemaining", 0);
		}

        public override void Tick()
        {
            base.Tick();
            if (isRepairing)
            {
				ticksRemaining--;
				if(ticksRemaining <= 0)
                {
					CompleteRepairs();
                }
            }
            else if (HasAnyContents && (Find.TickManager.TicksGame % 60 == 0))
			{
				StartRepairs();
			}

        }

        public override bool Accepts(Thing thing)
		{
			if (!base.Accepts(thing))
			{
				return false;
			}
			if (HasAnyContents)
			{
				return false;
			}
			if(!(thing.def == SubpersonaDefOf.O21_AI_SubpersonaShell || (thing as Corpse != null && (thing as Corpse).InnerPawn.def == SubpersonaDefOf.O21_AI_SubpersonaShell)))
            {
				return false;
            }
			return true;
		}

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if(base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(SoundInfo.InMap(this));
                }
                return true;
            }
            return false;
		}

        public override string GetInspectString()
        {
			StringBuilder builder = new StringBuilder(base.GetInspectString());
            if (isRepairing)
            {
				builder.Append("\nRepairing Shell: " + ticksRemaining.ToStringTicksToPeriod());

			}
			return builder.ToString();

        }

        public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (Faction == Faction.OfPlayer && innerContainer.Count > 0 && def.building.isPlayerEjectable && !isRepairing)
			{
				// Eject
				{
					Command_Action command_eject = new Command_Action();
					command_eject.action = new Action(EjectContents);
					command_eject.defaultLabel = "CommandPodEject".Translate();
					command_eject.defaultDesc = "CommandPodEjectDesc".Translate();
					if (innerContainer.Count == 0)
					{
						command_eject.Disable("CommandPodEjectFailEmpty".Translate());
					}
					command_eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
					yield return command_eject;
				}
			}
			yield break;
		}

		public void StartRepairs()
        {
			isRepairing = true;
            if (HasCorpse)
            {
				ticksRemaining = (int)(repairTime * 1f);
            }
            else
            {
				ticksRemaining = (int)(repairTime * (1f - ((Pawn)innerContainer.First(t => t as Pawn != null))?.health?.summaryHealth?.SummaryHealthPercent ?? 0.5f));
            }
		}

		public void CompleteRepairs()
        {
            if (HasCorpse)
            {
				Pawn pawn = Corpse.InnerPawn;
				EjectContents();
				ResurrectionUtility.Resurrect(pawn);
            }
			foreach(Thing thing in innerContainer)
            {
				Pawn pawn = thing as Pawn;
				if(pawn != null)
                {
					pawn.health.RemoveAllHediffs();
                }
            }

			isRepairing = false;

			SubpersonaDefOf.O21_AI_OvenDing.PlayOneShot(SoundInfo.InMap(this));

			EjectContents();
        }

		public override void EjectContents()
		{
			isRepairing = false;
			foreach (Thing thing in innerContainer)
			{
				Pawn pawn = thing as Pawn;
				if (pawn != null)
				{
					PawnComponentsUtility.AddComponentsForSpawn(pawn);
					pawn.filth.GainFilth(ThingDefOf.Filth_MachineBits);
				}
			}
			if (!base.Destroyed)
			{
				SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(this));
			}
			base.EjectContents();
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			if(myPawn.def != SubpersonaDefOf.O21_AI_SubpersonaShell)
			{
				FloatMenuOption floatMenuOption = new FloatMenuOption("CannotUseReason".Translate("SubAI_ReconstructorShellsOnly".Translate()), null);
				yield return floatMenuOption;
				yield break;
			}
			if (myPawn.IsQuestLodger())
			{
				FloatMenuOption floatMenuOption = new FloatMenuOption("CannotUseReason".Translate("SubAI_SubpersonaReconstructorGuestsNotAllowed".Translate()), null);
				yield return floatMenuOption;
				yield break;
			}
			foreach (FloatMenuOption floatMenuOption2 in base.GetFloatMenuOptions(myPawn))
			{
				yield return floatMenuOption2;
			}
			if (this.innerContainer.Count == 0)
			{
				if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
				{
					FloatMenuOption floatMenuOption3 = new FloatMenuOption("CannotUseNoPath".Translate(), null);
					yield return floatMenuOption3;
				}
				else
				{
					JobDef jobDef = SubpersonaDefOf.O21_AI_EnterReconstructor;
					string label = "SubAI_EnterSubpersonaReconstructor".Translate();
					Action action = delegate ()
					{
						Job job = JobMaker.MakeJob(jobDef, this);
						myPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
					};
					yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), myPawn, this);
				}
			}
			yield break;
		}

		public static bool IsReconstructor(ThingDef def)
        {
			return typeof(Building_SubpersonaReconstructor).IsAssignableFrom(def.thingClass);
        }

		public static Building_SubpersonaReconstructor FindReconstructorFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
		{
			foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => IsReconstructor(def)))
			{
				Building_SubpersonaReconstructor building = (Building_SubpersonaReconstructor)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(item), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f, (Thing x) => !((Building_SubpersonaReconstructor)x).HasAnyContents && traveler.CanReserve(x, 1, -1, null, ignoreOtherReservations));
				if (building != null)
				{
					return building;
				}
			}
			return null;
		}
	}
}
