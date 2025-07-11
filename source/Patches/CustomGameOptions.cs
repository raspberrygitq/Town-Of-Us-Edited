using TownOfUsEdited.CustomOption;
using TownOfUsEdited.NeutralRoles.ExecutionerMod;
using TownOfUsEdited.CrewmateRoles.HaunterMod;
using TownOfUsEdited.CrewmateRoles.MediumMod;
using TownOfUsEdited.CrewmateRoles.VampireHunterMod;
using TownOfUsEdited.NeutralRoles.GuardianAngelMod;
using static TownOfUsEdited.Roles.Modifiers.Madmate;
using TownOfUsEdited.Patches;
using static TownOfUsEdited.CrewmateRoles.MedicMod.StopKill;

namespace TownOfUsEdited
{
    public enum NumAssassins
    {
        None,
        One,
        All
    }
    public enum DisableSkipButtonMeetings
    {
        No,
        Emergency,
        Always
    }
    public enum GameMode
    {
        Classic,
        RoleList,
        BattleRoyale,
        Cultist,
        Werewolf,
        Chaos
    }
    public enum AdminDeadPlayers
    {
        Nobody,
        Spy,
        EveryoneButSpy,
        Everyone
    }
    public enum RoleOptions
    {
        CrewInvest,
        CrewKilling,
        CrewPower,
        CrewProtective,
        CrewSupport,
        CrewCommon,
        CrewSpecial,
        CrewRandom,
        NeutBenign,
        NeutEvil,
        NeutKilling,
        NeutCommon,
        NeutRandom,
        ImpConceal,
        ImpKilling,
        ImpSupport,
        ImpCommon,
        ImpRandom,
        CovenKilling,
        CovenSupport,
        CovenCommon,
        CovenRandom,
        NonImpCoven,
        Any
    }
    public static class CustomGameOptions
    {
        public static int PoliticianOn => (int)Generate.PoliticianOn.Get();
        public static int PlumberOn => (int)Generate.PlumberOn.Get();
        public static int MercenaryOn => (int)Generate.MercenaryOn.Get();
        public static int ClericOn => (int)Generate.ClericOn.Get();
        public static int SerialKillerOn => (int)Generate.SerialKillerOn.Get();
        public static int DoppelgangerOn => (int)Generate.DoppelgangerOn.Get();
        public static int MutantOn => (int)Generate.MutantOn.Get();
        public static int InfectiousOn => (int)Generate.InfectiousOn.Get();
        public static int JesterOn => (int)Generate.JesterOn.Get();
        public static int VultureOn => (int)Generate.VultureOn.Get();
        public static int TrollOn => (int)Generate.TrollOn.Get();
        public static int SheriffOn => (int)Generate.SheriffOn.Get();
        public static int KnightOn => (int)Generate.KnightOn.Get();
        public static int FighterOn => (int)Generate.FighterOn.Get();
        public static int JailorOn => (int)Generate.JailorOn.Get();
        public static int DeputyOn => (int)Generate.DeputyOn.Get();
        public static int JanitorOn => (int)Generate.JanitorOn.Get();
        public static int WitchOn => (int)Generate.WitchOn.Get();
        public static int EngineerOn => (int)Generate.EngineerOn.Get();
        public static int InformantOn => (int)Generate.InformantOn.Get();
        public static int SwapperOn => (int)Generate.SwapperOn.Get();
        public static int SuperstarOn => (int)Generate.SuperstarOn.Get();
        public static int AvengerOn => (int)Generate.AvengerOn.Get();
        public static int AmnesiacOn => (int)Generate.AmnesiacOn.Get();
        public static int ShifterOn => (int)Generate.ShifterOn.Get();
        public static int SoulCollectorOn => (int)Generate.SoulCollectorOn.Get();
        public static int InvestigatorOn => (int)Generate.InvestigatorOn.Get();
        public static int MedicOn => (int)Generate.MedicOn.Get();
        public static int AstralOn => (int)Generate.AstralOn.Get();
        public static int LookoutOn => (int)Generate.LookoutOn.Get();
        public static int SeerOn => (int)Generate.SeerOn.Get();
        public static int GlitchOn => (int)Generate.GlitchOn.Get();
        public static int JuggernautOn => (int)Generate.JuggernautOn.Get();
        public static int MorphlingOn => (int)Generate.MorphlingOn.Get();
        public static int AssassinOn => (int)Generate.AssassinOn.Get();
        public static int ExecutionerOn => (int)Generate.ExecutionerOn.Get();
        public static int SpyOn => (int)Generate.SpyOn.Get();
        public static int SnitchOn => (int)Generate.SnitchOn.Get();
        public static int MinerOn => (int)Generate.MinerOn.Get();
        public static int SwooperOn => (int)Generate.SwooperOn.Get();
        public static int ArsonistOn => (int)Generate.ArsonistOn.Get();
        public static int AltruistOn => (int)Generate.AltruistOn.Get();
        public static int UndertakerOn => (int)Generate.UndertakerOn.Get();
        public static int PhantomOn => (int)Generate.PhantomOn.Get();
        public static int HunterOn => (int)Generate.HunterOn.Get();
        public static int VigilanteOn => (int)Generate.VigilanteOn.Get();
        public static int HaunterOn => (int)Generate.HaunterOn.Get();
        public static int SpiritOn => (int)Generate.SpiritOn.Get();
        public static int BlinderOn => (int)Generate.BlinderOn.Get();
        public static int FreezerOn => (int)Generate.FreezerOn.Get();
        public static int HelperOn => (int)Generate.HelperOn.Get();
        public static int GuardianOn => (int)Generate.GuardianOn.Get();
        public static int GrenadierOn => (int)Generate.GrenadierOn.Get();
        public static int VeteranOn => (int)Generate.VeteranOn.Get();
        public static int TrackerOn => (int)Generate.TrackerOn.Get();
        public static int TrapperOn => (int)Generate.TrapperOn.Get();
        public static int TraitorOn => (int)Generate.TraitorOn.Get();
        public static int PoisonerOn => (int)Generate.PoisonerOn.Get();
        public static int ShooterOn => (int)Generate.ShooterOn.Get();
        public static int TransporterOn => (int)Generate.TransporterOn.Get();
        public static int TimeLordOn => (int)Generate.TimeLordOn.Get();
        public static int MediumOn => (int)Generate.MediumOn.Get();
        public static int SurvivorOn => (int)Generate.SurvivorOn.Get();
        public static int GuardianAngelOn => (int)Generate.GuardianAngelOn.Get();
        public static int MysticOn => (int)Generate.MysticOn.Get();
        public static int BlackmailerOn => (int)Generate.BlackmailerOn.Get();
        public static int ConverterOn => (int)Generate.ConverterOn.Get();
        public static int PlaguebearerOn => (int)Generate.PlaguebearerOn.Get();
        public static int AttackerOn => (int)Generate.AttackerOn.Get();
        public static int WerewolfOn => (int)Generate.WerewolfOn.Get();
        public static int CaptainOn => (int)Generate.CaptainOn.Get();
        public static int ParanoïacOn => (int)Generate.ParanoïacOn.Get();
        public static int ChameleonOn => (int)Generate.ChameleonOn.Get();
        public static int DetectiveOn => (int)Generate.DetectiveOn.Get();
        public static int DoctorOn => (int)Generate.DoctorOn.Get();
        public static int BodyguardOn => (int)Generate.BodyguardOn.Get();
        public static int CrusaderOn => (int)Generate.CrusaderOn.Get();
        public static int EscapistOn => (int)Generate.EscapistOn.Get();
        public static int ImpostorOn => (int)Generate.ImpostorOn.Get();
        public static int CrewmateOn => (int)Generate.CrewmateOn.Get();
        public static int ImitatorOn => (int)Generate.ImitatorOn.Get();
        public static int BomberOn => (int)Generate.BomberOn.Get();
        public static int ConjurerOn => (int)Generate.ConjurerOn.Get();
        public static int BountyHunterOn => (int)Generate.BountyHunterOn.Get();
        public static int DoomsayerOn => (int)Generate.DoomsayerOn.Get();
        public static int VampireOn => (int)Generate.VampireOn.Get();
        public static int VampireHunterOn => (int)Generate.VampireHunterOn.Get();
        public static int ProsecutorOn => (int)Generate.ProsecutorOn.Get();
        public static int WarlockOn => (int)Generate.WarlockOn.Get();
        public static int MafiosoOn => (int)Generate.MafiosoOn.Get();
        public static int ReviverOn => (int)Generate.ReviverOn.Get();
        public static int HypnotistOn => (int)Generate.HypnotistOn.Get();
        public static int ManipulatorOn => (int)Generate.ManipulatorOn.Get();
        public static int OracleOn => (int)Generate.OracleOn.Get();
        public static int WardenOn => (int)Generate.WardenOn.Get();
        public static int VenererOn => (int)Generate.VenererOn.Get();
        public static int AurialOn => (int)Generate.AurialOn.Get();
        public static int CovenOn => (int)Generate.CovenOn.Get();
        public static int RitualistOn => (int)Generate.RitualistOn.Get();
        public static int HexMasterOn => (int)Generate.HexMasterOn.Get();
        public static int CovenLeaderOn => (int)Generate.CovenLeaderOn.Get();
        public static int SpiritualistOn => (int)Generate.SpiritualistOn.Get();
        public static int VoodooMasterOn => (int)Generate.VoodooMasterOn.Get();
        public static int PotionMasterOn => (int)Generate.PotionMasterOn.Get();
        public static int TorchOn => (int)Generate.TorchOn.Get();
        public static int TaskmasterOn => (int)Generate.TaskmasterOn.Get();
        public static int SatelliteOn => (int)Generate.SatelliteOn.Get();
        public static int VengefulOn => (int)Generate.VengefulOn.Get();
        public static int MadmateOn => (int)Generate.MadmateOn.Get();
        public static int DiseasedOn => (int)Generate.DiseasedOn.Get();
        public static int FlashOn => (int)Generate.FlashOn.Get();
        public static int TiebreakerOn => (int)Generate.TiebreakerOn.Get();
        public static int GiantOn => (int)Generate.GiantOn.Get();
        public static int MiniOn => (int)Generate.MiniOn.Get();
        public static int ButtonBarryOn => (int)Generate.ButtonBarryOn.Get();
        public static int BaitOn => (int)Generate.BaitOn.Get();
        public static int LoversOn => (int)Generate.LoversOn.Get();
        public static int SleuthOn => (int)Generate.SleuthOn.Get();
        public static int ScientistOn => (int)Generate.ScientistOn.Get();
        public static int ShyOn => (int)Generate.ShyOn.Get();
        public static int SixthSenseOn => (int)Generate.SixthSenseOn.Get();
        public static int SpotterOn => (int)Generate.SpotterOn.Get();
        public static int MotionlessOn => (int)Generate.MotionlessOn.Get();
        public static int AftermathOn => (int)Generate.AftermathOn.Get();
        public static int RadarOn => (int)Generate.RadarOn.Get();
        public static int DisperserOn => (int)Generate.DisperserOn.Get();
        public static int BloodlustOn => (int)Generate.BloodlustOn.Get();
        public static int SaboteurOn => (int)Generate.SaboteurOn.Get();
        public static int MultitaskerOn => (int)Generate.MultitaskerOn.Get();
        public static int DrunkOn => (int)Generate.DrunkOn.Get();
        public static int DoubleShotOn => (int)Generate.DoubleShotOn.Get();
        public static int UnderdogOn => (int)Generate.UnderdogOn.Get();
        public static int LuckyOn => (int)Generate.LuckyOn.Get();
        public static int TaskerOn => (int)Generate.TaskerOn.Get();
        public static int FrostyOn => (int)Generate.FrostyOn.Get();
        public static RoleOptions Slot1 => (RoleOptions)Generate.Slot1.Get();
        public static RoleOptions Slot2 => (RoleOptions)Generate.Slot2.Get();
        public static RoleOptions Slot3 => (RoleOptions)Generate.Slot3.Get();
        public static RoleOptions Slot4 => (RoleOptions)Generate.Slot4.Get();
        public static RoleOptions Slot5 => (RoleOptions)Generate.Slot5.Get();
        public static RoleOptions Slot6 => (RoleOptions)Generate.Slot6.Get();
        public static RoleOptions Slot7 => (RoleOptions)Generate.Slot7.Get();
        public static RoleOptions Slot8 => (RoleOptions)Generate.Slot8.Get();
        public static RoleOptions Slot9 => (RoleOptions)Generate.Slot9.Get();
        public static RoleOptions Slot10 => (RoleOptions)Generate.Slot10.Get();
        public static RoleOptions Slot11 => (RoleOptions)Generate.Slot11.Get();
        public static RoleOptions Slot12 => (RoleOptions)Generate.Slot12.Get();
        public static RoleOptions Slot13 => (RoleOptions)Generate.Slot13.Get();
        public static RoleOptions Slot14 => (RoleOptions)Generate.Slot14.Get();
        public static RoleOptions Slot15 => (RoleOptions)Generate.Slot15.Get();
        public static bool UniqueRoles => Generate.UniqueRoles.Get();
        public static float SerialKillerKCD => Generate.SerialKillerKCD.Get();
        public static bool SerialKillerCanConvert => Generate.SerialKillerCanConvert.Get();
        public static bool SKConvertImp => Generate.SKConvertImp.Get();
        public static bool SKConvertNK => Generate.SKConvertNK.Get();
        public static bool SKConvertCoven => Generate.SKConvertCoven.Get();
        public static bool SkImpVision => Generate.SkImpVision.Get();
        public static bool SerialKillerVent => Generate.SerialKillerVent.Get();
        public static float MutantKCD => Generate.MutantKCD.Get();
        public static float TransformCD => Generate.TransformCD.Get();
        public static float TransformKCD => Generate.TransformKCD.Get();
        public static bool MutantVent => Generate.MutantVent.Get();
        public static float InfectiousCD => Generate.InfectiousCD.Get();
        public static float InfectiousInfectedCD => Generate.InfectiousInfectedCD.Get();
        public static float InfectedSpeed => Generate.InfectedSpeed.Get();
        public static bool NewSKCanGuess => Generate.NewSKCanGuess.Get();
        public static float InitialCooldowns => Generate.InitialCooldowns.Get();
        public static bool BothLoversDie => Generate.BothLoversDie.Get();
        public static bool ImpLoverKillTeammate => Generate.ImpLoverKillTeammate.Get();
        public static bool NeutralEvilWinsLover => Generate.NeutralEvilWinsLover.Get();
        public static bool MadmateKillEachOther => Generate.MadmateKillEachOther.Get();
        public static bool MadmateHasImpoVision => Generate.MadmateHasImpoVision.Get();
        public static bool MadmateCanChat => Generate.MadmateCanChat.Get();
        public static bool NeutralLovers => Generate.NeutralLovers.Get();
        public static bool CovenLovers => Generate.CovenLovers.Get();
        public static float JailCD => Generate.JailCD.Get();
        public static bool CanJailNB => Generate.CanJailNB.Get();
        public static bool CanJailNE => Generate.CanJailNE.Get();
        public static bool CanJailNK => Generate.CanJailNK.Get();
        public static bool CanJailCoven => Generate.CanJailCoven.Get();
        public static bool CanJailMad => Generate.CanJailMad.Get();
        public static bool JailorDies => Generate.JailorDies.Get();
        public static float KnightKCD => Generate.KnightKCD.Get();
        public static float FighterKCD => Generate.FighterKCD.Get();
        public static bool FighterKillsNB => Generate.FighterKillsNB.Get();
        public static bool FighterKillsNE => Generate.FighterKillsNE.Get();
        public static bool FighterKillsNK => Generate.FighterKillsNK.Get();
        public static bool FighterKillsCoven => Generate.FighterKillsCoven.Get();
        public static bool FighterKillsMadmate => Generate.FighterKillsMadmate.Get();
        public static bool SheriffKillOther => Generate.SheriffKillOther.Get();
        public static bool SheriffKillsNB => Generate.SheriffKillsNB.Get();
        public static bool SheriffKillsNE => Generate.SheriffKillsNE.Get();
        public static bool SheriffKillsNK => Generate.SheriffKillsNK.Get();
        public static bool SheriffKillsCoven => Generate.SheriffKillsCoven.Get();
        public static bool SheriffKillsMad => Generate.SheriffKillsMad.Get();
        public static float SheriffKillCd => Generate.SheriffKillCd.Get();
        public static bool SwapperButton => Generate.SwapperButton.Get();
        public static float FootprintSize => Generate.FootprintSize.Get();
        public static float FootprintInterval => Generate.FootprintInterval.Get();
        public static float FootprintDuration => Generate.FootprintDuration.Get();
        public static bool AnonymousFootPrint => Generate.AnonymousFootPrint.Get();
        public static bool VentFootprintVisible => Generate.VentFootprintVisible.Get();
        public static bool JesterButton => Generate.JesterButton.Get();
        public static bool JesterVent => Generate.JesterVent.Get();
        public static bool JesterImpVision => Generate.JesterImpVision.Get();
        public static bool JesterHaunt => Generate.JesterHaunt.Get();
        public static FortifyOptions ShowFortified => (FortifyOptions)Generate.ShowFortified.Get();
        public static NotificationOptions NotificationShield => (NotificationOptions)Generate.WhoGetsNotification.Get();
        public static bool VultureVent => Generate.VultureVent.Get();
        public static bool VultureImpVision => Generate.VultureImpVision.Get();
        public static bool VultureArrow => Generate.VultureArrow.Get();
        public static int VultureBodies => (int)Generate.VultureBodies.Get();

        public static bool ShieldBreaks => Generate.ShieldBreaks.Get();
        public static float MedicReportColorDuration => Generate.MedicReportColorDuration.Get();
        public static bool ShowReports => Generate.MedicReportSwitch.Get();
        public static ShieldOptions ShowShielded => (ShieldOptions)Generate.ShowShielded.Get();
        public static float SeerCd => Generate.SeerCooldown.Get();
        public static bool CrewKillingRed => Generate.CrewKillingRed.Get();
        public static bool NeutBenignRed => Generate.NeutBenignRed.Get();
        public static bool NeutEvilRed => Generate.NeutEvilRed.Get();
        public static bool NeutKillingRed => Generate.NeutKillingRed.Get();
        public static bool TraitorColourSwap => Generate.TraitorColourSwap.Get();
        public static float MimicCooldown => Generate.MimicCooldownOption.Get();
        public static float MimicDuration => Generate.MimicDurationOption.Get();
        public static float HackCooldown => Generate.HackCooldownOption.Get();
        public static float HackDuration => Generate.HackDurationOption.Get();
        public static float GlitchKillCooldown => Generate.GlitchKillCooldownOption.Get();
        public static bool GlitchVent => Generate.GlitchVent.Get();
        public static float JuggKCd => Generate.JuggKillCooldown.Get();
        public static float ReducedKCdPerKill => Generate.ReducedKCdPerKill.Get();
        public static bool JuggVent => Generate.JuggVent.Get();
        public static float MorphlingCd => Generate.MorphlingCooldown.Get();
        public static float MorphlingDuration => Generate.MorphlingDuration.Get();
        public static bool MorphlingVent => Generate.MorphlingVent.Get();
        public static bool ColourblindComms => Generate.ColourblindComms.Get();
        public static bool ReactorScreenShake => Generate.ReactorScreenShake.Get();
        public static bool OxygenBlackout => Generate.OxygenBlackout.Get();
        public static bool FlashlightMode => Generate.FlashlightMode.Get();
        public static bool AllowUp => Generate.AllowUp.Get();
        public static bool SpectateHost => Generate.SpectateHost.Get();
        public static float CrewmateFlashlightVision => Generate.CrewmateFlashlightVision.Get();
        public static float ImpostorFlashlightVision => Generate.ImpostorFlashlightVision.Get();
        public static bool AutoRejoin => Generate.AutoRejoin.Get();
        public static int RejoinSeconds => (int)Generate.RejoinSeconds.Get();
        public static bool AutoStart => Generate.AutoStart.Get();
        public static int StartMinutes => (int)Generate.StartMinutes.Get();
        public static OnTargetDead OnTargetDead => (OnTargetDead)Generate.OnTargetDead.Get();
        public static bool ExecutionerButton => Generate.ExecutionerButton.Get();
        public static bool ExecutionerTorment => Generate.ExecutionerTorment.Get();
        public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals.Get();
        public static int SnitchTasksRemaining => (int)Generate.SnitchTasksRemaining.Get();
        public static bool SnitchSeesImpInMeeting => Generate.SnitchSeesImpInMeeting.Get();
        public static bool SnitchSeesTraitor => Generate.SnitchSeesTraitor.Get();
        public static float MineCd => Generate.MineCooldown.Get();
        public static float MafiosoKillTimer => Generate.MafiosoKillTimer.Get();
        public static float SwoopCd => Generate.SwoopCooldown.Get();
        public static float SwoopDuration => Generate.SwoopDuration.Get();
        public static bool SwooperVent => Generate.SwooperVent.Get();
        public static bool ImpostorSeeRoles => Generate.ImpostorSeeRoles.Get();
        public static bool CovenSeeRoles => Generate.CovenSeeRoles.Get();
        public static bool DeadSeeRoles => Generate.DeadSeeRoles.Get();
        public static bool FirstDeathShield => Generate.FirstDeathShield.Get();
        public static bool NeutralEvilWinEndsGame => Generate.NeutralEvilWinEndsGame.Get();
        public static bool SeeTasksDuringRound => Generate.SeeTasksDuringRound.Get();
        public static bool SeeTasksDuringMeeting => Generate.SeeTasksDuringMeeting.Get();
        public static bool SeeTasksWhenDead => Generate.SeeTasksWhenDead.Get();
        public static bool ImpostorChat => Generate.ImpostorChat.Get();
        public static bool SKChat => Generate.SKChat.Get();
        public static bool VampireChat => Generate.VampireChat.Get();
        public static bool CovenChat => Generate.CovenChat.Get();
        public static bool LoversChat => Generate.LoversChat.Get();
        public static float DouseCd => Generate.DouseCooldown.Get();
        public static float VultureCD => Generate.VultureCooldown.Get();
        public static int MaxDoused => (int)Generate.MaxDoused.Get();
        public static bool ArsoVent => Generate.ArsoVent.Get();
        public static bool IgniteCdRemoved => Generate.IgniteCdRemoved.Get();
        public static int MinNeutralBenignRoles => (int)Generate.MinNeutralBenignRoles.Get();
        public static int MaxNeutralBenignRoles => (int)Generate.MaxNeutralBenignRoles.Get();
        public static int MinNeutralEvilRoles => (int)Generate.MinNeutralEvilRoles.Get();
        public static int MaxNeutralEvilRoles => (int)Generate.MaxNeutralEvilRoles.Get();
        public static int MinNeutralKillingRoles => (int)Generate.MinNeutralKillingRoles.Get();
        public static int MaxNeutralKillingRoles => (int)Generate.MaxNeutralKillingRoles.Get();
        public static int MinCoven => (int)Generate.MinCoven.Get();
        public static int MaxCoven => (int)Generate.MaxCoven.Get();
        public static bool CovenReplaceImps => Generate.CovenReplaceImps.Get();
        public static float CovenKCD => Generate.CovenKCD.Get();
        public static bool ParallelMedScans => Generate.ParallelMedScans.Get();
        public static int MaxFixes => (int)Generate.MaxFixes.Get();
        public static int MaxMeetings => (int)Generate.MaxMeetings.Get();
        public static float ReviveDuration => Generate.ReviveDuration.Get();
        public static bool AltruistTargetBody => Generate.AltruistTargetBody.Get();
        public static bool SheriffBodyReport => Generate.SheriffBodyReport.Get();
        public static float DragCd => Generate.DragCooldown.Get();
        public static float UndertakerDragSpeed => Generate.UndertakerDragSpeed.Get();
        public static bool UndertakerVent => Generate.UndertakerVent.Get();
        public static bool UndertakerVentWithBody => Generate.UndertakerVentWithBody.Get();
        public static bool AssassinGuessNeutralBenign => Generate.AssassinGuessNeutralBenign.Get();
        public static bool AssassinGuessNeutralEvil => Generate.AssassinGuessNeutralEvil.Get();
        public static bool AssassinGuessNeutralKilling => Generate.AssassinGuessNeutralKilling.Get();
        public static bool AssassinGuessImpostors => Generate.AssassinGuessImpostors.Get();
        public static bool AssassinGuessCoven => Generate.AssassinGuessCoven.Get();
        public static bool AssassinGuessModifiers => Generate.AssassinGuessModifiers.Get();
        public static bool AssassinGuessLovers => Generate.AssassinGuessLovers.Get();
        public static bool AssassinCrewmateGuess => Generate.AssassinCrewmateGuess.Get();
        public static int AssassinKills => (int)Generate.AssassinKills.Get();
        public static NumAssassins NumberOfImpostorAssassins => (NumAssassins)Generate.NumberOfImpostorAssassins.Get();
        public static NumAssassins NumberOfNeutralAssassins => (NumAssassins)Generate.NumberOfNeutralAssassins.Get();
        public static bool AmneTurnImpAssassin => Generate.AmneTurnImpAssassin.Get();
        public static bool AssassinImpostorRole => Generate.AssassinImpostorRole.Get();
        public static bool AmneTurnNeutAssassin => Generate.AmneTurnNeutAssassin.Get();
        public static bool ShiftTurnImpAssassin => Generate.ShiftTurnImpAssassin.Get();
        public static bool ShiftTurnNeutAssassin => Generate.ShiftTurnNeutAssassin.Get();
        public static bool TraitorCanAssassin => Generate.TraitorCanAssassin.Get();
        public static bool AssassinMultiKill => Generate.AssassinMultiKill.Get();
        public static float UnderdogKillBonus => Generate.UnderdogKillBonus.Get();
        public static bool UnderdogIncreasedKC => Generate.UnderdogIncreasedKC.Get();
        public static float ReducedSaboCD => Generate.ReducedSaboCooldown.Get();
        public static float DetectDuration => Generate.DetectDuration.Get();
        public static int PhantomTasksRemaining => (int)Generate.PhantomTasksRemaining.Get();
        public static bool PhantomSpook => Generate.PhantomSpook.Get();
        public static bool TrollHaunts => Generate.TrollHaunts.Get();
        public static bool VigilanteGuessNeutralBenign => Generate.VigilanteGuessNeutralBenign.Get();
        public static bool VigilanteGuessNeutralEvil => Generate.VigilanteGuessNeutralEvil.Get();
        public static bool VigilanteGuessNeutralKilling => Generate.VigilanteGuessNeutralKilling.Get();
        public static bool VigilanteGuessCoven => Generate.VigilanteGuessCoven.Get();
        public static bool VigilanteGuessImpModifiers => Generate.VigilanteGuessImpModifiers.Get();
        public static bool VigilanteGuessLovers => Generate.VigilanteGuessLovers.Get();
        public static int VigilanteKills => (int)Generate.VigilanteKills.Get();
        public static bool VigilanteMultiKill => Generate.VigilanteMultiKill.Get();
        public static int HaunterTasksRemainingClicked => (int)Generate.HaunterTasksRemainingClicked.Get();
        public static int HaunterTasksRemainingAlert => (int)Generate.HaunterTasksRemainingAlert.Get();
        public static bool HaunterRevealsNeutrals => Generate.HaunterRevealsNeutrals.Get();
        public static HaunterCanBeClickedBy HaunterCanBeClickedBy => (HaunterCanBeClickedBy)Generate.HaunterCanBeClickedBy.Get();
        public static int SpiritTasksRemainingClicked => (int)Generate.SpiritTasksRemainingClicked.Get();
        public static int SpiritTasksRemainingAlert => (int)Generate.SpiritTasksRemainingAlert.Get();
        public static float GrenadeCd => Generate.GrenadeCooldown.Get();
        public static float GrenadeDuration => Generate.GrenadeDuration.Get();
        public static bool GrenadierVent => Generate.GrenadierVent.Get();
        public static float FlashRadius => Generate.FlashRadius.Get();
        public static int LovingImpPercent => (int)Generate.LovingImpPercent.Get();
        public static float CampaignCd => Generate.CampaignCooldown.Get();
        public static bool KilledOnAlert => Generate.KilledOnAlert.Get();
        public static float AlertCd => Generate.AlertCooldown.Get();
        public static float HelperCD => Generate.HelperCooldown.Get();
        public static float HelperDuration => Generate.HelperDuration.Get();
        public static float GuardCD => Generate.GuardCooldown.Get();
        public static float GuardDuration => Generate.GuardDuration.Get();
        public static float BlindCD => Generate.BlindCooldown.Get();
        public static float BlindDuration => Generate.BlindDuration.Get();
        public static float FreezeCD => Generate.FreezeCooldown.Get();
        public static float FreezeDuration => Generate.FreezeDuration.Get();
        public static float GhostCD => Generate.GhostCooldown.Get();
        public static float AlertDuration => Generate.AlertDuration.Get();
        public static float GhostDuration => Generate.GhostDuration.Get();
        public static int MaxAlerts => (int)Generate.MaxAlerts.Get();
        public static float WatchCD => Generate.WatchCooldown.Get();
        public static float WatchDuration => Generate.WatchDuration.Get();
        public static bool WatchedKnows => Generate.WatchedKnows.Get(); 
        public static bool LookoutUseVitals => Generate.LookoutUseVitals.Get();
        public static float UpdateInterval => Generate.UpdateInterval.Get();
        public static float TrackCd => Generate.TrackCooldown.Get();
        public static bool ResetOnNewRound => Generate.ResetOnNewRound.Get();
        public static int MaxTracks => (int)Generate.MaxTracks.Get();
        public static int LatestSpawn => (int)Generate.LatestSpawn.Get();
        public static bool NeutralKillingStopsTraitor => Generate.NeutralKillingStopsTraitor.Get();
        public static float TransportCooldown => Generate.TransportCooldown.Get();
        public static int TransportMaxUses => (int)Generate.TransportMaxUses.Get();
        public static bool TransporterVitals => Generate.TransporterVitals.Get();
        public static float RewindCooldown => Generate.RewindCooldown.Get();
        public static float RewindDuration => Generate.RewindDuration.Get();
        public static bool TimeLordVitals => Generate.TimeLordVitals.Get();
        public static bool RememberArrows => Generate.RememberArrows.Get();
        public static float RememberArrowDelay => Generate.RememberArrowDelay.Get();
        public static float VultureArrowDelay => Generate.VultureArrowDelay.Get();
        public static float ShiftCD => Generate.ShiftCD.Get();
        public static float MediateCooldown => Generate.MediateCooldown.Get();
        public static bool ShowMediatePlayer => Generate.ShowMediatePlayer.Get();
        public static bool ShowMediumToDead => Generate.ShowMediumToDead.Get();
        public static DeadRevealed DeadRevealed => (DeadRevealed)Generate.DeadRevealed.Get();
        public static float VestCd => Generate.VestCd.Get();
        public static float VestDuration => Generate.VestDuration.Get();
        public static int MaxVests => (int)Generate.MaxVests.Get();
        public static float ProtectCd => Generate.ProtectCd.Get();
        public static float ProtectDuration => Generate.ProtectDuration.Get();
        public static float TempSaveCdReset => Generate.TempSaveCdReset.Get();
        public static int MaxProtects => (int)Generate.MaxProtects.Get();
        public static ProtectOptions ShowProtect => (ProtectOptions)Generate.ShowProtect.Get();
        public static BecomeOptions GaOnTargetDeath => (BecomeOptions)Generate.GaOnTargetDeath.Get();
        public static BecomeMadmateOptions MadmateOnImpoDeath => (BecomeMadmateOptions)Generate.MadmateOnImpoDeath.Get();
        public static bool GATargetKnows => Generate.GATargetKnows.Get();
        public static bool GAKnowsTargetRole => Generate.GAKnowsTargetRole.Get();
        public static int EvilTargetPercent => (int)Generate.EvilTargetPercent.Get();
        public static float MysticArrowDuration => Generate.MysticArrowDuration.Get();
        public static float BlackmailCd => Generate.BlackmailCooldown.Get();
        public static bool BlackmailInvisible => Generate.BlackmailInvisible.Get();
        public static int LatestNonVote => (int)Generate.LatestNonVote.Get();
        public static float ConvertCD => Generate.ConvertCooldown.Get();
        public static float GiantSlow => Generate.GiantSlow.Get();
        public static float FlashSpeed => Generate.FlashSpeed.Get();
        public static float HelperSpeed => Generate.HelperSpeed.Get();
        public static float DiseasedMultiplier => Generate.DiseasedKillMultiplier.Get();
        public static float BaitMinDelay => Generate.BaitMinDelay.Get();
        public static float BaitMaxDelay => Generate.BaitMaxDelay.Get();
        public static float InfectCd => Generate.InfectCooldown.Get();
        public static float PestKillCd => Generate.PestKillCooldown.Get();
        public static bool PestVent => Generate.PestVent.Get();
        public static float TerroristKillCD => Generate.TerroristKillCooldown.Get();
        public static bool TerroristVent => Generate.TerroristVent.Get();
        public static float DoppelKCD => Generate.DoppelKCD.Get();
        public static bool DoppelVent => Generate.DoppelVent.Get();
        public static float RampageCd => Generate.RampageCooldown.Get();
        public static float RampageDuration => Generate.RampageDuration.Get();
        public static float RampageKillCd => Generate.RampageKillCooldown.Get();
        public static bool WerewolfVent => Generate.WerewolfVent.Get();
        public static bool InfectiousVent => Generate.InfectiousVent.Get();
        public static float TrapCooldown => Generate.TrapCooldown.Get();
        public static bool TrapsRemoveOnNewRound => Generate.TrapsRemoveOnNewRound.Get();
        public static int MaxTraps => (int)Generate.MaxTraps.Get();
        public static float MinAmountOfTimeInTrap => Generate.MinAmountOfTimeInTrap.Get();
        public static float TrapSize => Generate.TrapSize.Get();
        public static int MinAmountOfPlayersInTrap => (int) Generate.MinAmountOfPlayersInTrap.Get();
        public static float ExamineCd => Generate.ExamineCooldown.Get();
        public static bool DetectiveReportOn => Generate.DetectiveReportOn.Get();
        public static float DetectiveRoleDuration => Generate.DetectiveRoleDuration.Get();
        public static float DetectiveFactionDuration => Generate.DetectiveFactionDuration.Get();
        public static float DocReviveCooldown => Generate.DocReviveCooldown.Get();
        public static float CrusadeCD => Generate.CrusadeCD.Get();
        public static bool OnlyMedRevive => Generate.OnlyMedRevive.Get();
        public static float DoctorDragSpeed => Generate.DoctorDragSpeed.Get();
        public static int MaxRevives => (int)Generate.MaxRevives.Get();
        public static float ZoomCooldown => Generate.ZoomCooldown.Get();
        public static float ZoomDuration => Generate.ZoomDuration.Get();
        public static float ZoomRange => Generate.ZoomRange.Get();
        public static float ChamSwoopCooldown => Generate.ChamSwoopCooldown.Get();
        public static float ChamSwoopDuration => Generate.ChamSwoopDuration.Get();
        public static float EscapeCd => Generate.EscapeCooldown.Get();
        public static bool EscapistVent => Generate.EscapistVent.Get();
        public static float DetonateDelay => Generate.DetonateDelay.Get();
        public static int MaxKillsInDetonation => (int) Generate.MaxKillsInDetonation.Get();
        public static float DetonateRadius => Generate.DetonateRadius.Get();
        public static float BodyguardRadius => Generate.BodyguardRadius.Get();
        public static bool BomberVent => Generate.BomberVent.Get();
        public static float ManipulateCD => Generate.ManipulateCooldown.Get();
        public static float ManipulationDuration => Generate.ManipulationDuration.Get();
        public static float PoisonCD => Generate.PoisonCooldown.Get();
        public static float PoisonDuration => Generate.PoisonDuration.Get();
        public static bool PoisonerVent => Generate.PoisonerVent.Get();
        public static float TargetLongCooldown => Generate.TargetLongCooldown.Get();
        public static float TargetShortCooldown => Generate.TargetShortCooldown.Get();
        public static float TargetDuration => Generate.TargetDuration.Get();
        public static int MaxStore => (int)Generate.MaxStore.Get();
        public static float ObserveCooldown => Generate.ObserveCooldown.Get();
        public static int DoomsayerGuessesToWin => (int)Generate.DoomsayerGuessesToWin.Get();
        public static bool HexMasterVent => Generate.HexMasterVent.Get();
        public static int RitualistKills => (int)Generate.RitualistKills.Get();
        public static bool RitualistMultiKill => Generate.RitualistMultiKill.Get();
        public static bool RitualistGuessNeutralBenign => Generate.RitualistGuessNeutralBenign.Get();
        public static bool RitualistGuessNeutralEvil => Generate.RitualistGuessNeutralEvil.Get();
        public static bool RitualistGuessNeutralKilling => Generate.RitualistGuessNeutralKilling.Get();
        public static bool RitualistGuessImpostors => Generate.RitualistGuessImpostors.Get();
        public static bool RitualistGuessLovers => Generate.RitualistGuessLovers.Get();
        public static bool RitualistGuessModifiers => Generate.RitualistGuessModifiers.Get();
        public static bool RitualistVent => Generate.RitualistVent.Get();
        public static float PotionCD => Generate.PotionCD.Get();
        public static float PotionDuration => Generate.PotionDuration.Get();
        public static float PotionSpeed => Generate.PotionSpeed.Get();
        public static float StrengthKCD => Generate.StrengthKCD.Get();
        public static bool SpiritualistVent => Generate.SpiritualistVent.Get();
        public static bool VoodooMasterVent => Generate.VoodooMasterVent.Get();
        public static float BiteCd => Generate.BiteCooldown.Get();
        public static bool VampImpVision => Generate.VampImpVision.Get();
        public static bool VampVent => Generate.VampVent.Get();
        public static bool NewVampCanAssassin => Generate.NewVampCanAssassin.Get();
        public static int MaxVampiresPerGame => (int)Generate.MaxVampiresPerGame.Get();
        public static bool CanBiteNeutralBenign => Generate.CanBiteNeutralBenign.Get();
        public static bool CanBiteNeutralEvil => Generate.CanBiteNeutralEvil.Get();
        public static float StakeCd => Generate.StakeCooldown.Get();
        public static int MaxFailedStakesPerGame => (int)Generate.MaxFailedStakesPerGame.Get();
        public static bool CanStakeRoundOne => Generate.CanStakeRoundOne.Get();
        public static bool SelfKillAfterFinalStake => Generate.SelfKillAfterFinalStake.Get();
        public static BecomeEnum BecomeOnVampDeaths => (BecomeEnum)Generate.BecomeOnVampDeaths.Get();
        public static bool ProsDiesOnIncorrectPros => Generate.ProsDiesOnIncorrectPros.Get();
        public static float ChargeUpDuration => Generate.ChargeUpDuration.Get();
        public static float ChargeUseDuration => Generate.ChargeUseDuration.Get();
        public static float ConfessCd => Generate.ConfessCooldown.Get();
        public static float BlessCD => Generate.BlessCooldown.Get();
        public static float RevealAccuracy => Generate.RevealAccuracy.Get();
        public static bool NeutralBenignShowsEvil => Generate.NeutralBenignShowsEvil.Get();
        public static bool NeutralEvilShowsEvil => Generate.NeutralEvilShowsEvil.Get();
        public static bool NeutralKillingShowsEvil => Generate.NeutralKillingShowsEvil.Get();
        public static float AbilityCd => Generate.AbilityCooldown.Get();
        public static float AbilityDuration => Generate.AbilityDuration.Get();
        public static float SprintSpeed => Generate.SprintSpeed.Get();
        public static float MinFreezeSpeed => Generate.MinFreezeSpeed.Get();
        public static float FreezeRadius => Generate.FreezeRadius.Get();
        public static float ChillDuration => Generate.ChillDuration.Get();
        public static float ChillStartSpeed => Generate.ChillStartSpeed.Get();
        public static float AuraInnerRadius => (float)Generate.AuraInnerRadius.Get();
        public static float AuraOuterRadius => (float)Generate.AuraOuterRadius.Get();
        public static float SenseDuration => (float)Generate.SenseDuration.Get();
        public static AdminDeadPlayers WhoSeesDead => (AdminDeadPlayers)Generate.WhoSeesDead.Get();
        public static bool SpyHasPortableAdmin => Generate.SpyHasPortableAdmin.Get();
        public static bool VentImprovements => Generate.VentImprovements.Get();
        public static bool VitalsLab => Generate.VitalsLab.Get();
        public static bool ColdTempDeathValley => Generate.ColdTempDeathValley.Get();
        public static bool WifiChartCourseSwap => Generate.WifiChartCourseSwap.Get();
        public static bool AirshipPolusDoors => Generate.AirshipPolusDoors.Get();
        public static bool RandomMapEnabled => Generate.RandomMapEnabled.Get();
        public static float RandomMapSkeld => Generate.RandomMapSkeld.Get();
        public static float RandomMapMira => Generate.RandomMapMira.Get();
        public static float RandomMapPolus => Generate.RandomMapPolus.Get();
        public static float RandomMapAirship => Generate.RandomMapAirship.Get();
        public static float RandomMapFungle => Generate.RandomMapFungle.Get();
        public static float RandomMapSubmerged => Patches.SubmergedCompatibility.Loaded ? Generate.RandomMapSubmerged.Get() : 0f;
        public static float RandomMapLevelImpostor => RandomMap.LevelImpLoaded ? Generate.RandomMapLevelImpostor.Get() : 0f;
        public static bool SmallMapHalfVision => Generate.SmallMapHalfVision.Get();
        public static bool AutoAdjustCooldowns => Generate.AutoAdjustCooldowns.Get();
        public static float SmallMapDecreasedCooldown => Generate.SmallMapDecreasedCooldown.Get();
        public static float LargeMapIncreasedCooldown => Generate.LargeMapIncreasedCooldown.Get();
        public static int SmallMapIncreasedShortTasks => (int)Generate.SmallMapIncreasedShortTasks.Get();
        public static int SmallMapIncreasedLongTasks => (int)Generate.SmallMapIncreasedLongTasks.Get();
        public static int LargeMapDecreasedShortTasks => (int)Generate.LargeMapDecreasedShortTasks.Get();
        public static int LargeMapDecreasedLongTasks => (int)Generate.LargeMapDecreasedLongTasks.Get();
        public static DisableSkipButtonMeetings SkipButtonDisable =>
            (DisableSkipButtonMeetings)Generate.SkipButtonDisable.Get();
        public static GameMode GameMode =>
            (GameMode)Generate.GameMode.Get();
        public static bool CamoCommsKillAnyone => Generate.CamoCommsKillAnyone.Get();
        public static bool CrewKillersContinue => Generate.CrewKillersContinue.Get();
        public static float HypnotiseCd => Generate.HypnotiseCooldown.Get();
        public static float ReapCd => Generate.ReapCooldown.Get();
        public static float HunterKillCd => Generate.HunterKillCd.Get();
        public static float HunterStalkCd => Generate.HunterStalkCd.Get();
        public static float HunterStalkDuration => Generate.HunterStalkDuration.Get();
        public static int HunterStalkUses => (int)Generate.HunterStalkUses.Get();
        public static bool HunterBodyReport => Generate.HunterBodyReport.Get();
        public static bool RetributionOnVote => Generate.RetributionOnVote.Get();
        public static bool DoomsayerCantObserve => Generate.DoomsayerCantObserve.Get();
        public static bool SCVent => Generate.SCVent.Get();
        public static float InvisDelay => Generate.InvisDelay.Get();
        public static float TransformInvisDuration => Generate.TransformInvisDuration.Get();
        public static float FinalTransparency => Generate.FinalTransparency.Get();
        public static float RampageCD => Generate.RampageCD.Get();
        public static float WerewolfKillCD => Generate.WerewolfKillCD.Get();
        public static int SorcererOn => (int)Generate.SorcererOn.Get();
        public static int BasicWerewolfOn => (int)Generate.BasicWerewolfOn.Get();
        public static int VillagerOn => (int)Generate.VillagerOn.Get();
        public static int WerewolfSeerOn => (int)Generate.WerewolfSeerOn.Get();
        public static int WerewolfProsecutorOn => (int)Generate.WerewolfProsecutorOn.Get();
        public static int WerewolfParanoïacOn => (int)Generate.WerewolfParanoïacOn.Get();
        public static int WerewolfMayorOn => (int)Generate.WerewolfMayorOn.Get();
        public static int SoulCatcherOn => (int)Generate.SoulCatcherOn.Get();
        public static int GuardOn => (int)Generate.GuardOn.Get();
        public static int WerewolfChameleonOn => (int)Generate.WerewolfChameleonOn.Get();
        public static int BlackWolfOn => (int)Generate.BlackWolfOn.Get();
        public static int WhiteWolfOn => (int)Generate.WhiteWolfOn.Get();
        public static int TalkativeWolfOn => (int)Generate.TalkativeWolfOn.Get();
        public static int WerewolfSheriffOn => (int)Generate.WerewolfSheriffOn.Get();
        public static float BattleRoyaleKillCD => Generate.BattleRoyaleKillCD.Get();
        public static float BattleRoyaleStartingCD => Generate.BattleRoyaleStartingCD.Get();
        public static bool BattleDisableVent => Generate.BattleDisableVent.Get();
        public static float FlushCd => Generate.FlushCooldown.Get();
        public static int MaxBarricades => (int)Generate.MaxBarricades.Get();
        public static float MercenaryCD => Generate.MercenaryCD.Get();
        public static int MaxGuards => (int)Generate.MaxGuards.Get();
        public static int GoldToBribe => (int)Generate.GoldToBribe.Get();
        public static float BarrierCD => Generate.BarrierCd.Get();
        public static BarrierOptions ShowBarriered => (BarrierOptions)Generate.ShowBarriered.Get();
        public static bool ClericAttackNotification => Generate.ClericGetsAttackNotification.Get();
        public static int DrunkDuration => (int)Generate.DrunkDuration.Get();
    }
}