using HarmonyLib;
using TownOfUsEdited.CrewmateRoles.AltruistMod;
using TownOfUsEdited.Roles.Modifiers;
using TownOfUsEdited.Roles;
using System;
using TownOfUsEdited.CrewmateRoles.MedicMod;

namespace TownOfUsEdited.Modifiers.LoversMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;

            var flag3 = __instance.IsLover() && CustomGameOptions.BothLoversDie;
            if (!flag3) return true;
            foreach (var role in Role.GetRoles(RoleEnum.Prosecutor))
            {
                var prosecutor = (Prosecutor)role;
                if (prosecutor.ProsecuteThisMeeting) return true;
            }
            var otherLover = Modifier.GetModifier<Lover>(__instance).OtherLover.Player;
            if (otherLover.Data.IsDead && (!otherLover.Is(RoleEnum.Astral) || !Role.GetRole<Astral>(otherLover).Enabled)) return true;
            bool isAstralDeath = __instance.Is(RoleEnum.Astral) && Role.GetRole<Astral>(__instance).Enabled;

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                if (!otherLover.Is(RoleEnum.Pestilence)) otherLover.Exiled();
            }
            else if (!otherLover.Is(RoleEnum.Pestilence) && !isAstralDeath)
            {
                if (otherLover.Is(RoleEnum.Astral) && Role.GetRole<Astral>(otherLover).Enabled)
                {
                    var astralRole = Role.GetRole<Astral>(otherLover);
                    Utils.MurderPlayer(otherLover, astralRole.AstralBody, false);
                    System.Console.WriteLine("Astral death (lovers)");
                }
                else Utils.MurderPlayer(otherLover, otherLover, false);
                var deadBody = new DeadPlayer
                {
                    PlayerId = otherLover.PlayerId,
                    KillerId = otherLover.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);
            }
            if (otherLover.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(otherLover);
                sheriff.IncorrectKills -= 1;
            }
            var otherLoverRole = Role.GetRole(otherLover);
            otherLoverRole.DeathReason = DeathReasons.Suicided;
            Utils.Rpc(CustomRPC.SetDeathReason, otherLover.PlayerId, (byte)DeathReasons.Suicided);

            return true;
        }
    }
}