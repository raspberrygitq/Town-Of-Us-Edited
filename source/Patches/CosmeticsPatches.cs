using HarmonyLib;

namespace TownOfUsEdited.Patches;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class UnlockCosmetics_HatManager_Initialize_Postfix
{
    public static void Postfix(HatManager __instance)
    {
        foreach (var bundle in __instance.allBundles)
        {
            bundle.Free = true; 
        }

        foreach (var featuredBundle in __instance.allFeaturedBundles)
        {
            featuredBundle.Free = true;
        }

        foreach (var featuredCube in __instance.allFeaturedCubes)
        { 
            featuredCube.Free = true;
        }

        foreach (var featuredItem in __instance.allFeaturedItems)
        {
            featuredItem.Free = true;
        }

        foreach (var hat in __instance.allHats)
        { 
            hat.Free = true; 
        }

        foreach (var nameplate in __instance.allNamePlates)
        {
            nameplate.Free = true;
        }

        foreach (var pet in __instance.allPets)
        {
            pet.Free = true;
        }

        foreach (var skin in __instance.allSkins)
        {
            skin.Free = true;
        }

        foreach (var starBundle in __instance.allStarBundles)
        {
            starBundle.price = 0;
        }

        foreach (var visor in __instance.allVisors)
        {
            visor.Free = true;
        }
    }
}