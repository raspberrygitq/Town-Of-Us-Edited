using HarmonyLib;

// Code from MalumMenu, source: https://github.com/scp222thj/MalumMenu/blob/6e1a7b72fc017184063da4f538f6bca476b21290/src/Cheats/CosmeticsUnlocker.cs
[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public class UnlockCosmetics
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
            starBundle.price = 0; // StarBundles don't have a Free property, so price is changed instead
        }

        foreach (var visor in __instance.allVisors)
        {
            visor.Free = true;
        }
    }
}