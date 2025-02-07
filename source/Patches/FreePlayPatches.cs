using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Text;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches
{
    // This Partches don't work for now, I might try fixing that later, but for now, it's not my priority
    public static class FreePlayerPatch
    {
        private static Scroller Scroller;
        [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
        public class PatchFolder
        {
            public static bool Prefix(TaskAdderGame __instance, [HarmonyArgument(0)] TaskFolder taskFolder)
            {
                StringBuilder stringBuilder = new StringBuilder(64);
		        __instance.Hierarchy.Add(taskFolder);
		        for (int i = 0; i < __instance.Hierarchy.Count; i++)
		        {
			        stringBuilder.Append(__instance.Hierarchy[i].FolderName);
			        stringBuilder.Append("\\");
		        }
		        __instance.PathText.text = stringBuilder.ToString();
		        for (int j = 0; j < __instance.ActiveItems.Count; j++)
		        {
			        Object.Destroy(__instance.ActiveItems[j].gameObject);
		        }
		        __instance.ActiveItems.Clear();
		        float num = 0f;
		        float num2 = 0f;
		        float num3 = 0f;
		        for (int k = 0; k < taskFolder.SubFolders.Count; k++)
		        {
			        TaskFolder taskFolder2 = Object.Instantiate<TaskFolder>(taskFolder.SubFolders[k], __instance.TaskParent);
			        taskFolder2.gameObject.SetActive(true);
			        taskFolder2.Parent = __instance;
			        taskFolder2.transform.localPosition = new Vector3(num, num2, 0f);
			        taskFolder2.transform.localScale = Vector3.one;
			        num3 = Mathf.Max(num3, taskFolder2.Text.bounds.size.y + 1.1f);
			        num += __instance.folderWidth;
			        if (num > __instance.lineWidth)
			        {
				        num = 0f;
				        num2 -= num3;
				        num3 = 0f;
			        }
			        __instance.ActiveItems.Add(taskFolder2.transform);
			        if (taskFolder2 != null && taskFolder2.Button != null)
			        {
				        ControllerManager.Instance.AddSelectableUiElement(taskFolder2.Button, false);
				        if (!string.IsNullOrEmpty(__instance.restorePreviousSelectionByFolderName) && taskFolder2.FolderName.Equals(__instance.restorePreviousSelectionByFolderName))
				        {
					        __instance.restorePreviousSelectionFound = taskFolder2.Button;
				        }
			        }
		        }
                if (taskFolder.FolderName == "Roles")
                {
                    var taskAddButton3 = Object.Instantiate(__instance.RoleButton);
                    taskAddButton3.SafePositionWorld = __instance.SafePositionWorld;
                    taskAddButton3.Text.text = "Be_Crewmate.exe";
                    taskAddButton3.Role = DestroyableSingleton<CrewmateRole>.Instance;
                    var taskAddButton2 = Object.Instantiate(__instance.RoleButton);
                    taskAddButton2.SafePositionWorld = __instance.SafePositionWorld;
                    taskAddButton2.Text.text = "Be_Sheriff.exe";
                    taskAddButton2.Role = DestroyableSingleton<CrewmateRole>.Instance;

                    if (taskAddButton2.Button != null && taskAddButton3.Button != null)
                    {
                        __instance.AddFileAsChild(taskFolder, taskAddButton2, ref num, ref num2, ref num3);
                        __instance.AddFileAsChild(taskFolder, taskAddButton3, ref num, ref num2, ref num3);
                        ControllerManager.Instance.AddSelectableUiElement(taskAddButton2.Button);
                        ControllerManager.Instance.AddSelectableUiElement(taskAddButton3.Button);
                        ControllerManager.Instance.SetDefaultSelection(taskAddButton3.Button);
                    }
                }
		        bool flag = false;

                var list = taskFolder.Children.ToArray().OrderBy(t => t.TaskType).ToList();
		        for (int l = 0; l < list.Count; l++)
		        {
			        TaskAddButton taskAddButton = Object.Instantiate<TaskAddButton>(__instance.TaskPrefab);
			        taskAddButton.MyTask = list[l];
			        if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower)
			        {
				        SystemTypes targetSystem = ((DivertPowerTask)taskAddButton.MyTask).TargetSystem;
				        taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.DivertPowerTo, (Il2CppSystem.Object[])(new object[]
				        {
					        DestroyableSingleton<TranslationController>.Instance.GetString(targetSystem)
				        }));
			        }
			        else if (taskAddButton.MyTask.TaskType == TaskTypes.FixWeatherNode)
			        {
				        int nodeId = ((WeatherNodeTask)taskAddButton.MyTask).NodeId;
				        taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixWeatherNode, (Il2CppSystem.Object[])Array.Empty<object>()) + " " + DestroyableSingleton<TranslationController>.Instance.GetString(WeatherSwitchGame.ControlNames[nodeId], (Il2CppSystem.Object[])Array.Empty<object>());
			        }
			        else
			        {
				        taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(taskAddButton.MyTask.TaskType);
			        }
			        __instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);
			        if (taskAddButton != null && taskAddButton.Button != null)
			        {
				        ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button, false);
				        if (__instance.Hierarchy.Count != 1 && !flag)
				        {
					        TaskFolder component = ControllerManager.Instance.CurrentUiState.CurrentSelection.GetComponent<TaskFolder>();
					        if (component != null)
					        {
						        __instance.restorePreviousSelectionByFolderName = component.FolderName;
					        }
					        ControllerManager.Instance.SetDefaultSelection(taskAddButton.Button, null);
					        flag = true;
				        }
			        }
		        }

                if (Scroller && Scroller != null)
                {
                    Scroller.CalculateAndSetYBounds(__instance.ActiveItems.Count, 6, 3, 1f);
                    Scroller.SetYBoundsMin(0.0f);
                    Scroller.ScrollToTop();
                }

                if (Scroller && Scroller != null)
                {
                    Scroller.CalculateAndSetYBounds(__instance.ActiveItems.Count, 6, 3, 1f);
                    Scroller.SetYBoundsMin(0.0f);
                    Scroller.ScrollToTop();
                }

		        if (__instance.Hierarchy.Count == 1)
		        {
			        ControllerManager.Instance.SetBackButton(__instance.BackButton);
			        return false;
		        }
		        ControllerManager.Instance.SetBackButton(__instance.FolderBackButton);
                return false;
            }
        }
        [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Role), MethodType.Setter)]
        public class FixColor
        {
            public static bool Prefix(TaskAddButton __instance)
            {
                if (__instance.Text.text != null)
                {
                    if (__instance.Text.text == "Be_Sheriff.exe")
                    {
                        __instance.FileImage.color = Patches.Colors.Sheriff;
                    }
                    else if (__instance.Text.text == "Be_Crewmate.exe")
                    {
                        __instance.FileImage.color = Patches.Colors.Crewmate;
                    }
                }

                __instance.RolloverHandler.OutColor = __instance.FileImage.color;
                return false;
            }
        }
        [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Update))]
        public class ButtonClick
        {
            public static bool Prefix(TaskAddButton __instance)
            {
                var selection = ControllerManager.Instance.CurrentUiState.CurrentSelection;
                if (selection != null && selection == __instance.Button)
                {
                    if (__instance.Text.text == "Be_Sheriff.exe" && !PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
                    {
                        if (Role.RoleDictionary.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                        {
                            Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                        }
                        var newRole = new Sheriff(PlayerControl.LocalPlayer);
                        newRole.RegenTask();
                    }
                    else if (__instance.Text.text == "Be_Crewmate.exe" && !PlayerControl.LocalPlayer.Is(RoleEnum.Crewmate))
                    {
                        if (Role.RoleDictionary.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
                        {
                            Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                        }
                        var newRole = new Crewmate(PlayerControl.LocalPlayer);
                        newRole.RegenTask();
                    }
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.Begin))]
        // Code from mira api, link: https://github.com/All-Of-Us-Mods/MiraAPI/blob/d0161ba85ca6647fe6deed8d4603feae429aad21/MiraAPI/Patches/Roles/TaskAdderPatch.cs
        public class AddRolesFolder
        {
            public static void Postfix(TaskAdderGame __instance)
            {
                GameObject inner = new("Inner");
                inner.transform.SetParent(__instance.TaskParent.transform, false);

                Scroller = __instance.TaskParent.gameObject.AddComponent<Scroller>();
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.Inner = inner.transform;

                GameObject hitbox = new("Hitbox")
                {
                    layer = 5,
                };
                hitbox.transform.SetParent(__instance.TaskParent.transform, false);
                hitbox.transform.localScale = new Vector3(7.5f, 6.5f, 1);
                hitbox.transform.localPosition = new Vector3(2.8f, -2.2f, 0);

                var mask = hitbox.AddComponent<SpriteMask>();
                mask.sprite = TownOfUs.NextButton;
                mask.alphaCutoff = 0.0f;

                var collider = hitbox.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(1f, 1f);
                collider.enabled = true;

                Scroller.ClickMask = collider;

                __instance.TaskPrefab.GetComponent<PassiveButton>().ClickMask = collider;
                __instance.RoleButton.GetComponent<PassiveButton>().ClickMask = collider;
                __instance.RootFolderPrefab.GetComponent<PassiveButton>().ClickMask = collider;
                __instance.RootFolderPrefab.gameObject.SetActive(false);

                __instance.TaskParent = inner.transform;

                var rolesFolder = Object.Instantiate(__instance.RootFolderPrefab, Scroller.Inner);
                rolesFolder.gameObject.SetActive(false);
                rolesFolder.FolderName = "Roles";
                rolesFolder.name = "RolesFolder";

                __instance.Root.SubFolders.Add(rolesFolder);

                __instance.GoToRoot();
            }
        }
    }
}