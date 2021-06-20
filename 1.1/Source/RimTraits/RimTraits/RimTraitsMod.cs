using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimTraits
{
    public class RimTraitsMod : Mod
    {
        public RimTraitsMod(ModContentPack pack) : base(pack)
        {
            new Harmony("RimTraits.Mod").PatchAll();
        }
    }
}
