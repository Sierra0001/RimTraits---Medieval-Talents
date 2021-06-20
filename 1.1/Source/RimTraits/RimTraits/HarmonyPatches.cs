using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimTraits
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("RimTraits.Mod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class GenerateTraits_Patch
    {
        public static void Prefix(out List<TraitDef> __state)
        {
            __state = DefDatabase<TraitDef>.AllDefs.Where(x => x.GetModExtension<TraitExtension>()?.mustSpawn ?? false).ToList();
            foreach (var def in __state)
            {
                try
                {
                    MethodInfo methodInfo = AccessTools.Method(typeof(DefDatabase<TraitDef>), "Remove", null, null);
                    methodInfo.Invoke(null, new object[]
                    {
                        def
                    });
                }
                catch (Exception ex)
                {
                    Log.Error("Error removing def: " + def + " - " + ex);
                };
            }
        }

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Pawn pawn, PawnGenerationRequest request, List<TraitDef> __state)
        {
            if (__state.Any())
            {
                foreach (var def in __state)
                {
                    DefDatabase<TraitDef>.Add(def);
                }
                var count = 0;
                while (count < 999)
                {
                    count++;
                    TraitDef newTraitDef = __state.RandomElementByWeight((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn.gender));
                    if (pawn.story.traits.HasTrait(newTraitDef) || (request.KindDef.disallowedTraits != null && request.KindDef.disallowedTraits.Contains(newTraitDef)) ||
                        (request.KindDef.requiredWorkTags != 0 && (newTraitDef.disabledWorkTags & request.KindDef.requiredWorkTags) != 0) || (newTraitDef == TraitDefOf.Gay && (!request.AllowGay || LovePartnerRelationUtility.HasAnyLovePartnerOfTheOppositeGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheOppositeGender(pawn))) || (request.ProhibitedTraits != null && request.ProhibitedTraits.Contains(newTraitDef)) || (request.Faction != null && Faction.OfPlayerSilentFail != null && request.Faction.HostileTo(Faction.OfPlayer) && !newTraitDef.allowOnHostileSpawn) || pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr)) || (newTraitDef.requiredWorkTypes != null && pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes)) || pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags) || (newTraitDef.forcedPassions != null && pawn.workSettings != null && newTraitDef.forcedPassions.Any((SkillDef p) => p.IsDisabled(pawn.story.DisabledWorkTagsBackstoryAndTraits, pawn.GetDisabledWorkTypes(permanentOnly: true)))))
                    {
                        continue;
                    }
                    int degree = PawnGenerator.RandomTraitDegree(newTraitDef);
                    if (!pawn.story.childhood.DisallowsTrait(newTraitDef, degree) && (pawn.story.adulthood == null || !pawn.story.adulthood.DisallowsTrait(newTraitDef, degree)))
                    {
                        Trait trait2 = new Trait(newTraitDef, degree);
                        if (pawn.mindState == null || pawn.mindState.mentalBreaker == null || !((pawn.mindState.mentalBreaker.BreakThresholdMinor + trait2.OffsetOfStat(StatDefOf.MentalBreakThreshold)) * trait2.MultiplierOfStat(StatDefOf.MentalBreakThreshold) > 0.5f))
                        {
                            pawn.story.traits.GainTrait(trait2);
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Need_Joy), "FallPerInterval", MethodType.Getter)]
    public static class FallPerInterval_Patch
    {
        private static void Postfix(Pawn ___pawn, ref float __result)
        {
            if (___pawn?.story?.traits != null)
            {
                foreach (var trait in ___pawn.story.traits.allTraits)
                {
                    __result += trait.OffsetOfStat(RT_DefOf.RTMT_RecreationNeed_Decay);
                    __result += trait.MultiplierOfStat(RT_DefOf.RTMT_RecreationNeed_Decay);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Need_Food), "FoodFallPerTick", MethodType.Getter)]
    internal static class Patch_FoodFallPerTick
    {
        private static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn?.story?.traits != null)
            {
                foreach (var trait in ___pawn.story.traits.allTraits)
                {
                    __result += trait.OffsetOfStat(RT_DefOf.RTMT_FoodNeedDecay);
                    __result += trait.MultiplierOfStat(RT_DefOf.RTMT_FoodNeedDecay);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Need_Rest), "RestFallPerTick", MethodType.Getter)]
    internal static class Patch_RestFallPerTick
    {
        private static void Postfix(ref float __result, Pawn ___pawn)
        {
            if (___pawn?.story?.traits != null)
            {
                foreach (var trait in ___pawn.story.traits.allTraits)
                {
                    __result += trait.OffsetOfStat(RT_DefOf.RTMT_RestNeed_Decay);
                    __result += trait.MultiplierOfStat(RT_DefOf.RTMT_RestNeed_Decay);
                }
            }
        }
    }
}