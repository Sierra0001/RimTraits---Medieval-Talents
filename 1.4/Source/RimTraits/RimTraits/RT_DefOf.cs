﻿using HarmonyLib;
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
    [DefOf]
    public static class RT_DefOf
    {
        public static StatDef RTMT_FoodNeedDecay;
        public static StatDef RTMT_RestNeed_Decay;
        public static StatDef RTMT_RecreationNeed_Decay;
        public static StatDef SmithingSpeed;
        public static StatDef TailoringSpeed;
        public static StatDef SculptingSpeed;
        public static EffecterDef Sculpt;
        public static EffecterDef Smith;
        public static EffecterDef Smelt;
        public static EffecterDef Cook;
        public static EffecterDef Tailor;
    }
}