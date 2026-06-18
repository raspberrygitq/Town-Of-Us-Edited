using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Reactor;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.IO;
using System.Reflection;
using TownOfUsEdited.CrewmateRoles.DetectiveMod;
using TownOfUsEdited.CustomOption;
using TownOfUsEdited.NeutralRoles.SoulCollectorMod;
using TownOfUsEdited.Patches;
using TownOfUsEdited.RainbowMod;
using UnityEngine;

namespace TownOfUsEdited
{
    [BepInPlugin(Id, "Town Of Us: Edited", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    [BepInIncompatibility("MalumMenu")]
    public class TownOfUsEdited : BasePlugin
    {
        public static string DataPath => Path.GetFullPath("edited_presets", Application.persistentDataPath);
        public const string Id = "com.lekillerdesgames.townofusedited";
        public const string VersionString = "3.0.1";
        public static Version Version = Version.Parse(VersionString);
        public const string VersionTag = "<color=#00F0FF></color>";

        public static AssetLoader bundledAssets;

        public static Sprite JanitorClean;
        public static Sprite SwitchRole;
        public static Sprite PlaceHolder;
        public static Sprite SabotageCoven;
        public static Sprite Potion;
        public static Sprite Drink;
        public static Sprite Recruit;
        public static Sprite SpiritualistControl;
        public static Sprite Hex;
        public static Sprite HexBomb;
        public static Sprite Freeze;
        public static Sprite Blind;
        public static Sprite Rewind;
        public static Sprite Dissociate;
        public static Sprite Eat;
        public static Sprite Avenge;
        public static Sprite Spell;
        public static Sprite Bounty;
        public static Sprite Curse;
        public static Sprite StoreSprite;
        public static Sprite ManipulateSprite;
        public static Sprite JailSprite;
        public static Sprite PoisonSprite;
        public static Sprite ShiftButton;
        public static Sprite DocReviveButton;
        public static Sprite CapZoomButton;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Footprint;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite Arrow;
        public static Sprite MineSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite DisperseSprite;
        public static Sprite CycleBackSprite;
        public static Sprite CycleForwardSprite;
        public static Sprite GuessSprite;
        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite TransformSprite;
        public static Sprite UnTransformSprite;
        public static Sprite FlashSprite;
        public static Sprite AlertSprite;
        public static Sprite RememberSprite;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite TransportSprite;
        public static Sprite MediateSprite;
        public static Sprite VestSprite;
        public static Sprite BlackmailSprite;
        public static Sprite BlackmailLetterSprite;
        public static Sprite BlackmailOverlaySprite;
        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
        public static Sprite InfectSprite;
        public static Sprite RampageSprite;
        public static Sprite TrapSprite;
        public static Sprite TrackSprite;
        public static Sprite InspectSprite;
        public static Sprite ExamineSprite;
        public static Sprite EscapeSprite;
        public static Sprite MarkSprite;
        public static Sprite Revive2Sprite;
        public static Sprite ChameleonSwoop;
        public static Sprite ImitateSelectSprite;
        public static Sprite ImitateDeselectSprite;
        public static Sprite ObserveSprite;
        public static Sprite BiteSprite;
        public static Sprite StakeSprite;
        public static Sprite RevealSprite;
        public static Sprite ConfessSprite;
        public static Sprite NoAbilitySprite;
        public static Sprite CamouflageSprite;
        public static Sprite CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
        public static Sprite SKConvertSprite;
        public static Sprite StalkSprite;
        public static Sprite CrimeSceneSprite;
        public static Sprite CampaignSprite;
        public static Sprite FortifySprite;
        public static Sprite HypnotiseSprite;
        public static Sprite HysteriaSprite;
        public static Sprite InJailSprite;
        public static Sprite ReapSprite;
        public static Sprite SoulSprite;
        public static Sprite Voodoo;
        public static Sprite CampSprite;
        public static Sprite ShootSprite;
        public static Sprite WatchSprite;
        public static Sprite BlessSprite;
        public static Sprite FlushSprite;
        public static Sprite BlockSprite;
        public static Sprite BarricadeSprite;
        public static Sprite BlindSprite;
        public static Sprite GuardSprite;
        public static Sprite BribeSprite;
        public static Sprite BarrierSprite;
        public static Sprite CleanseSprite;
        public static Sprite DetectSprite;
        public static Sprite NoclipSprite;
        public static Sprite AdminSprite;
        public static Sprite JailCellSprite;
        public static Sprite CrusadeSprite;

        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite ZoomPlusButton;
        public static Sprite ZoomMinusButton;
        public static Sprite ZoomPlusActiveButton;
        public static Sprite ZoomMinusActiveButton;
        public static Sprite NextButton;
        public static Sprite NextButtonActive;

        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private Harmony _harmony;

        public static string RuntimeLocation;
        public override void Load()
        {
            RuntimeLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfUsEdited)).Location);

            ReactorCredits.Register<TownOfUsEdited>(ReactorCredits.AlwaysShow);

            System.Console.WriteLine("000.000.000.000/000000000000000000");
            System.Console.WriteLine(Constants.GetBroadcastVersion());
            System.Console.WriteLine($"{Constants.Year}.{Constants.Month}.{Constants.Day}");

            _harmony = new Harmony("com.lekillerdesgames.townofusedited");

            Generate.GenerateAll();

            bundledAssets = new();

            var shortPath = "TownOfUsEdited.Resources";

            PlaceHolder = CreateSprite($"{shortPath}.Placeholder.png");
            SwitchRole = CreateSprite($"{shortPath}.SwitchRole.png");
            WatchSprite = CreateSprite($"{shortPath}.Watch.png");
            BlessSprite = CreateSprite($"{shortPath}.Bless.png");
            FlushSprite = CreateSprite($"{shortPath}.Flush.png");
            BlockSprite = CreateSprite($"{shortPath}.Block.png");
            BarricadeSprite = CreateSprite($"{shortPath}.Barricade.png");
            BlindSprite = CreateSprite($"{shortPath}.Blind.png");
            GuardSprite = CreateSprite($"{shortPath}.Guard.png");
            BribeSprite = CreateSprite($"{shortPath}.Bribe.png");
            BarrierSprite = CreateSprite($"{shortPath}.Barrier.png");
            CleanseSprite = CreateSprite($"{shortPath}.Cleanse.png");
            DetectSprite = CreateSprite($"{shortPath}.Detect.png");
            ShootSprite = CreateSprite($"{shortPath}.Shoot.png");
            CampSprite = CreateSprite($"{shortPath}.Camp.png");
            ChameleonSwoop = CreateSprite($"{shortPath}.ChameleonSwoop.png");
            JanitorClean = CreateSprite($"{shortPath}.Clean.png");
            Voodoo = CreateSprite($"{shortPath}.Voodoo.png");
            Potion = CreateSprite($"{shortPath}.Potion.png");
            Drink = CreateSprite($"{shortPath}.Drink.png");
            SabotageCoven = CreateSprite($"{shortPath}.SabotageCoven.png");
            Recruit = CreateSprite($"{shortPath}.Recruit.png");
            SpiritualistControl = CreateSprite($"{shortPath}.Control.png");
            Hex = CreateSprite($"{shortPath}.Hex.png");
            HexBomb = CreateSprite($"{shortPath}.HexBomb.png");
            Freeze = CreateSprite($"{shortPath}.Freeze.png");
            Blind = CreateSprite($"{shortPath}.Blind.png");
            Rewind = CreateSprite($"{shortPath}.Rewind.png");
            Eat = CreateSprite($"{shortPath}.Eat.png");
            Dissociate = CreateSprite($"{shortPath}.Dissociate.png");
            Avenge = CreateSprite($"{shortPath}.Avenge.png");
            TrackSprite = CreateSprite($"{shortPath}.Track.png");
            Spell = CreateSprite($"{shortPath}.Spell.png");
            Curse = CreateSprite($"{shortPath}.Curse.png");
            Bounty = CreateSprite($"{shortPath}.Bounty.png");
            StoreSprite = CreateSprite($"{shortPath}.Store.png");
            ManipulateSprite = CreateSprite($"{shortPath}.ManipulateButton.png");
            JailSprite = CreateSprite($"{shortPath}.JailButton.png");
            PoisonSprite = CreateSprite($"{shortPath}.Poison.png");
            ShiftButton = CreateSprite($"{shortPath}.Shift.png");
            DocReviveButton = CreateSprite($"{shortPath}.DocReviveButton.png");
            CapZoomButton = CreateSprite($"{shortPath}.CapZoom.png");
            EngineerFix = CreateSprite($"{shortPath}.Engineer.png");
            SwapperSwitch = CreateSprite($"{shortPath}.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite($"{shortPath}.SwapperSwitchDisabled.png");
            Footprint = CreateSprite($"{shortPath}.Footprint.png");
            MedicSprite = CreateSprite($"{shortPath}.Medic.png");
            SeerSprite = CreateSprite($"{shortPath}.Seer.png");
            SampleSprite = CreateSprite($"{shortPath}.Sample.png");
            Arrow = CreateSprite($"{shortPath}.Arrow.png");
            MineSprite = CreateSprite($"{shortPath}.Mine.png");
            DouseSprite = CreateSprite($"{shortPath}.Douse.png");
            IgniteSprite = CreateSprite($"{shortPath}.Ignite.png");
            ReviveSprite = CreateSprite($"{shortPath}.Revive.png");
            ButtonSprite = CreateSprite($"{shortPath}.Button.png");
            DisperseSprite = CreateSprite($"{shortPath}.Disperse.png");
            DragSprite = CreateSprite($"{shortPath}.Drag.png");
            DropSprite = CreateSprite($"{shortPath}.Drop.png");
            TransformSprite = CreateSprite($"{shortPath}.Transform.png");
            UnTransformSprite = CreateSprite($"{shortPath}.UnTransform.png");
            CycleBackSprite = CreateSprite($"{shortPath}.CycleBack.png");
            CycleForwardSprite = CreateSprite($"{shortPath}.CycleForward.png");
            GuessSprite = CreateSprite($"{shortPath}.Guess.png");
            FlashSprite = CreateSprite($"{shortPath}.Flash.png");
            AlertSprite = CreateSprite($"{shortPath}.Alert.png");
            RememberSprite = CreateSprite($"{shortPath}.Remember.png");
            PlantSprite = CreateSprite($"{shortPath}.Plant.png");
            DetonateSprite = CreateSprite($"{shortPath}.Detonate.png");
            TransportSprite = CreateSprite($"{shortPath}.Transport.png");
            MediateSprite = CreateSprite($"{shortPath}.Mediate.png");
            VestSprite = CreateSprite($"{shortPath}.Vest.png");
            BlackmailSprite = CreateSprite($"{shortPath}.Blackmail.png");
            BlackmailLetterSprite = CreateSprite($"{shortPath}.BlackmailLetter.png");
            BlackmailOverlaySprite = CreateSprite($"{shortPath}.BlackmailOverlay.png");
            LighterSprite = CreateSprite($"{shortPath}.Lighter.png");
            DarkerSprite = CreateSprite($"{shortPath}.Darker.png");
            InfectSprite = CreateSprite($"{shortPath}.Infect.png");
            RampageSprite = CreateSprite($"{shortPath}.Rampage.png");
            TrapSprite = CreateSprite($"{shortPath}.Trap.png");
            InspectSprite = CreateSprite($"{shortPath}.Inspect.png");
            ExamineSprite = CreateSprite($"{shortPath}.Examine.png");
            EscapeSprite = CreateSprite($"{shortPath}.Mark.png");
            MarkSprite = CreateSprite($"{shortPath}.Mark.png");
            Revive2Sprite = CreateSprite($"{shortPath}.Revive2.png");
            ImitateSelectSprite = CreateSprite($"{shortPath}.ImitateSelect.png");
            ImitateDeselectSprite = CreateSprite($"{shortPath}.ImitateDeselect.png");
            ObserveSprite = CreateSprite($"{shortPath}.Observe.png");
            BiteSprite = CreateSprite($"{shortPath}.Bite.png");
            StakeSprite = CreateSprite($"{shortPath}.Stake.png");
            RevealSprite = CreateSprite($"{shortPath}.Reveal.png");
            ConfessSprite = CreateSprite($"{shortPath}.Confess.png");
            NoAbilitySprite = CreateSprite($"{shortPath}.NoAbility.png");
            CamouflageSprite = CreateSprite($"{shortPath}.Camouflage.png");
            CamoSprintSprite = CreateSprite($"{shortPath}.CamoSprint.png");
            CamoSprintFreezeSprite = CreateSprite($"{shortPath}.CamoSprintFreeze.png");
            HackSprite = CreateSprite($"{shortPath}.Hack.png");
            MimicSprite = CreateSprite($"{shortPath}.Mimic.png");
            LockSprite = CreateSprite($"{shortPath}.Lock.png");
            SKConvertSprite = CreateSprite($"{shortPath}.SKConvert.png");
            StalkSprite = CreateSprite($"{shortPath}.Stalk.png");
            CrimeSceneSprite = CreateSprite($"{shortPath}.CrimeScene.png");
            CampaignSprite = CreateSprite($"{shortPath}.Campaign.png");
            FortifySprite = CreateSprite($"{shortPath}.Fortify.png");
            HypnotiseSprite = CreateSprite($"{shortPath}.Hypnotise.png");
            HysteriaSprite = CreateSprite($"{shortPath}.Hysteria.png");
            InJailSprite = CreateSprite($"{shortPath}.InJail.png");
            ReapSprite = CreateSprite($"{shortPath}.Reap.png");
            SoulSprite = CreateSprite($"{shortPath}.Soul.png");
            NoclipSprite = CreateSprite($"{shortPath}.Noclip.png");
            AdminSprite = CreateSprite($"{shortPath}.Admin.png");
            JailCellSprite = CreateSprite($"{shortPath}.JailCell.png");
            CrusadeSprite = CreateSprite($"{shortPath}.Crusade.png");

            ToUBanner = CreateSprite($"{shortPath}.TownOfUsEditedBanner.png", 125f);
            UpdateTOUButton = CreateSprite($"{shortPath}.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite($"{shortPath}.UpdateSubmergedButton.png");

            ZoomPlusButton = CreateSprite($"{shortPath}.Plus.png");
            ZoomMinusButton = CreateSprite($"{shortPath}.Minus.png");
            ZoomPlusActiveButton = CreateSprite($"{shortPath}.PlusActive.png");
            ZoomMinusActiveButton = CreateSprite($"{shortPath}.MinusActive.png");
            NextButton = CreateSprite($"{shortPath}.NextButton.png");
            NextButtonActive = CreateSprite($"{shortPath}.NextButtonActive.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<CrimeScene>();
            ClassInjector.RegisterTypeInIl2Cpp<Soul>();

            // RegisterInIl2CppAttribute.Register();

            TownOfUsEditedConfig.Bind(Config);

            _harmony.PatchAll();

            SubmergedCompatibility.Initialize();
            IL2CPPChainloader.Instance.Finished += LevelImpostorCompatibility.Initialize; // LI has a circular dependency on TOU, so we need to wait for LI to finish loading before we can initialize it

            ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
        }

        public static Sprite CreateSprite(string name, float pixelsPerUnit = 100f)
        {
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = LoadTextureFromResourcePath(name, assembly);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.name = name;
            sprite.DontDestroy();
            return sprite;
        }
        public static Texture2D LoadTextureFromResourcePath(string resourcePath, Assembly assembly)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
            };
            var myStream = assembly.GetManifestResourceStream(resourcePath);
            if (myStream != null)
            {
                var buttonTexture = myStream.ReadFully();
                tex.LoadImage(buttonTexture, false);
            }
            else
            {
                throw new ArgumentException($"Resource not found: {resourcePath}");
            }

            tex.name = resourcePath;
            return tex;
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}
