using Malee;
using SkaterXL.Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using GameManagement;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using ReplayFX;
using ReplayEditor;
using ReplayFX.State;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace ReplayFX.UI
{
    public class RFXMenuManager : MonoBehaviour
    {
        [Serializable]
        public class SettingsCategoryArray : ReorderableArray<SettingsCategory>
        {
        }
        [Serializable]
        public struct SettingsCategory
        {
            public string Name;
            public GameObject Panel;
            //[EnumFlags]
            //public RuntimePlatformFlag platforms;
            //public bool debugOnly;
        }
        public int currentCategoryIndex = 0;
        public bool pagesCreated = false;

        public ProceduralMenuPage cameraMenuPage;
        public ProceduralMenuPage keyframeMenuPage;
        public ProceduralMenuPage colorMenuPage;
        public ProceduralMenuPage otherMenuPage;

        //public ReorderableArray<SettingsMenuController.SettingsCategory> SettingsCategories;
        [Reorderable]
        public SettingsCategoryArray SettingsCategories = new SettingsCategoryArray();

        public CategoryButton SettingsCategoryButton;
        public Transform settingsPageParent;

        public RFXMenuState rfxMenuState;
        public GameObject clonedMenu;
        private MenuButton replayMenuButton;
        //private MenuButton testImpulseButton;
        public GameObject clonedInfoPanel;
        private List<Transform> clonedInfoPanelItems = new List<Transform>();

        private async void Start()
        {
            SetupClonedMenu(SettingsMenuController.Instance.gameObject);
            AddSettingsState(clonedMenu);

            try
            {
                await InitializeMenuAsync();
            }
            catch (Exception ex)
            {
                Main.Logger.Log("Failed to initialize replay settings menu:");
                Main.Logger.LogException(ex);
            }

            SetCurrentCategory(PageBuilder.cameraSettings);
            CreateCustomButtons();
            SetupClonedInfoPanel();
        }
        
        public async Task InitializeMenuAsync()
        {
            if (pagesCreated) return;

            cameraMenuPage = await PageBuilder.BuildCameraPageAsync();
            keyframeMenuPage = await PageBuilder.BuildKeyframePageAsync();
            colorMenuPage = await PageBuilder.BuildColorPageAsync();
            otherMenuPage = await PageBuilder.BuildOtherPageAsync();
            if (!Main.settings.enableNoise)
            {
                cameraMenuPage.SetVisible("camera_profile", false);
            }
            UpdateUI();
            cameraMenuPage.UpdatePage();
            keyframeMenuPage.UpdatePage();
            colorMenuPage.UpdatePage();
            otherMenuPage.UpdatePage();
            pagesCreated = true;
        }
        private void OnDestroy()
        {
            if (SettingsCategoryButton != null)
            {
                SettingsCategoryButton.OnNextCategory -= NextCategory;
                SettingsCategoryButton.OnPreviousCategory -= PreviousCategory;
            }
            Destroy(clonedMenu);
            clonedMenu = null;
            Destroy(replayMenuButton.gameObject);
            replayMenuButton = null;
            Destroy(clonedInfoPanel);
            clonedInfoPanel = null;
        }
        public void SetupClonedInfoPanel()
        {
            if (clonedInfoPanel == null)
            {
                clonedInfoPanel = Instantiate(ReplayEditorController.Instance.ReplayControlsPanel, ReplayEditorController.Instance.ReplayUI.transform);
                clonedInfoPanel.gameObject.GetComponent<RectTransform>();
                RectTransform rect = clonedInfoPanel.gameObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-1880, -38);
                rect.pivot = new Vector2 (0, 1);
                rect.sizeDelta = new Vector2(480, rect.sizeDelta.y);
                //clonedInfoPanel.gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                HashSet<int> indexesToKeep = new HashSet<int> { 1, 2, 9, 10 };
                GetClonedPanelItems(clonedInfoPanel.transform, indexesToKeep);
                //RemoveUnusedButtonItems(clonedInfoPanel.transform, 2);
                RemoveUnusedPanelItems(clonedInfoPanel.transform, indexesToKeep);

                SetClonePanelItemNames(clonedInfoPanelItems);
                AddAdaptiveLabels(clonedInfoPanelItems, 0, "ReplayFX", "", "");
                AddAdaptiveLabels(clonedInfoPanelItems, 1, "Playback Key Speed:", "<sprite name=XB1_RB> + <sprite name=d-pad-up>/<sprite name=d-pad-down>", "<sprite name=PS4_R2> + <sprite name=d-pad-up>/<sprite name=d-pad-down>");
                AddAdaptiveLabels(clonedInfoPanelItems, 2, "Add Playback Key", "<sprite name=XB1_RB> + <sprite name=XB1_A>", "<sprite name=PS4_R2> + <sprite name=PS4_Cross_Button>");
                AddAdaptiveLabels(clonedInfoPanelItems, 3, "Add Impulse Key", "<sprite name=XB1_RB> + <sprite name=XB1_Y>", "<sprite name=PS4_R2> + <sprite name=PS4_Triangle_Button>");
            }
        }
        private void AddAdaptiveLabels(List<Transform> clonedPanelItems, int index, string mainLabel, string xboxLabel, string psLabel)
        {
            if (clonedPanelItems == null) return;

            Transform item = clonedInfoPanelItems[index];
            item.gameObject.AddComponent<AdaptiveLabels>().Setup(mainLabel, xboxLabel, psLabel);

        }
        private void GetClonedPanelItems(Transform cloneInfoPanel, HashSet<int> savedItemIndexes)
        {
            if (cloneInfoPanel == null)
                return;

            clonedInfoPanelItems.Clear();
            for (int i = 0; i < cloneInfoPanel.childCount; i++)
            {
                if (savedItemIndexes.Contains(i))
                {
                    Transform child = cloneInfoPanel.GetChild(i);
                    clonedInfoPanelItems.Add(child);
                }
            }
        }
        private void RemoveUnusedPanelItems(Transform parentTransform, HashSet<int> savedItemIndexes)
        {
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                if (!savedItemIndexes.Contains(i))
                {
                    Transform child = parentTransform.GetChild(i);
                    child.gameObject.SetActive(false);
                    Destroy(child.gameObject);
                }
            }
        }
        private void RemoveUnusedButtonItems(Transform parentTransform, int index)
        {
            Transform child = parentTransform.GetChild(index);
            //Transform buttonitem = child.Find("Button Label");
            TMP_SubMeshUI buttonitem = child.gameObject.GetComponentInChildren<TMP_SubMeshUI>();
            Destroy(buttonitem.transform.parent.gameObject);
        }
        private void RemoveUnusedButtonItems2(Transform parentTransform, HashSet<int> targetIndexs)
        {
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                if (targetIndexs.Contains(i))
                {
                    Transform child = parentTransform.GetChild(i);
                    Transform buttonitem = child.Find("Button Label");
                    //Transform buttonitem = child.FindChildRecursively(buttonItemName);
                    if (buttonitem != null)
                    {
                        buttonitem.gameObject.SetActive(false);
                        Destroy(buttonitem.gameObject);
                    }
                }
            }
        }
        private void SetClonePanelItemNames(List<Transform> clonedPanelItems)
        {
            if (clonedPanelItems == null)
                return;

            string[] newNames = new string[]
            {
                "Replay FX Label",
                "Playback Speed Item",
                "Add Playback Key Item",
                "Add Impulse Key Item",
            };

            if (clonedPanelItems.Count != newNames.Length)
                return;

            for (int i = 0; i < clonedPanelItems.Count; i++)
            {
                if (i < newNames.Length && clonedPanelItems[i] != null)
                {
                    clonedPanelItems[i].gameObject.name = newNames[i];
                }
            }
        }
        private void CreateCustomButtons()
        {
            MenuButton originalButton = ReplayEditorController.Instance.Menu.MainMenuPanel.GetComponentInChildren<MenuButton>();
            if (originalButton == null)
            {
                Main.Logger.Log("[SetUpMenuButton] Failed to get parent button");
                return;
            }
            replayMenuButton = PageBuilder.CreateButton(originalButton, "Replay FX", MenuButtonAction);
            if (replayMenuButton != null)
            {
                ReplayEditorController.Instance.Menu.MainMenuPanel.GetComponent<FixFirstSelected>().selected = replayMenuButton.gameObject;
            }
            /*
            testImpulseButton = PageBuilder.CreateButton(originalButton, "Test Impulse", Main.camController.GenerateImpluse);
            if (testImpulseButton)
            {
                testImpulseButton.gameObject.transform.SetParent(keyframeMenuPage.itemParent.transform, false);
                testImpulseButton.gameObject.transform.SetAsLastSibling();
            }
            */
        }
        private void MenuButtonAction()
        {
            clonedMenu.SetActive(true);
            ReplayEditorController.Instance.Menu.SettingsMenu.gameObject.SetActive(false);
            ReplayEditorController.Instance.Menu.SaveMenu.gameObject.SetActive(false);
            ReplayEditorController.Instance.Menu.MainMenuPanel.SetActive(false);
            UpdateUI();
            ReplayEditorController.Instance.Menu.MainMenuPanel.GetComponent<FixFirstSelected>().selected = replayMenuButton.gameObject;
        }
        public void SetupClonedMenu(GameObject originalMenuPrefab)
        {
            if (clonedMenu == null)
            {
                clonedMenu = Instantiate(originalMenuPrefab);
                //clonedMenu.transform.SetParent(Main.ScriptManager.transform);
                clonedMenu.transform.SetParent(ReplayEditorController.Instance.Menu.transform);
                clonedMenu.gameObject.name = "Replay FX Settings";
                SettingsMenuController originalController = clonedMenu.GetComponentInChildren<SettingsMenuController>();
                if (originalController != null)
                {
                    Destroy(originalController);
                }
            }
            SetUpReferences();
        }
        private void SetUpReferences()
        {
            SettingsCategoryButton = clonedMenu.GetComponentInChildren<CategoryButton>(true);
            if (SettingsCategoryButton != null)
            {
                SettingsCategoryButton.OnNextCategory += NextCategory;
                SettingsCategoryButton.OnPreviousCategory += PreviousCategory;
            }
            //Transform pageParent = clonedMenu.transform.Find("Options Area");
            Transform pageParent = clonedMenu.transform.FindChildRecursively("Options Area");
            if (pageParent != null)
            {
                settingsPageParent = pageParent;
            }
            BuildInfosUI buildinfo = clonedMenu.GetComponentInChildren<BuildInfosUI>();
            if (buildinfo != null)
            {
                buildinfo.gameObject.SetActive(false);
            }
        }
        public void UpdateUI()
        {
            if (SettingsCategories.Count <= 0)
                return;

            currentCategoryIndex = Mathf.Clamp(currentCategoryIndex, 0, SettingsCategories.Count - 1);
            for (int i = 0; i < SettingsCategories.Count; i++)
            {
                if (i == currentCategoryIndex)
                {
                    SettingsCategories[i].Panel.SetActive(true);
                    SettingsCategoryButton.SetText(SettingsCategories[i].Name, false);
                    ProceduralMenuPage page = GetSettingsPage(SettingsCategories[i].Name);
                    page.UpdatePage();
                }
                else
                {
                    SettingsCategories[i].Panel.SetActive(false);
                }
            }
        }
        public void SetCurrentCategory(string categoryName)
        {
            if (SettingsCategories.Count <= 0)
                return;

            for (int i = 0; i < SettingsCategories.Count; i++)
            {
                if (SettingsCategories[i].Name == categoryName)
                {
                    currentCategoryIndex = i;
                    if (SettingsCategoryButton.isActiveAndEnabled)
                    {
                        UpdateUI();
                        return;
                    }
                }
            }
        }
        public void NextCategory()
        {
            currentCategoryIndex++;
            if (currentCategoryIndex >= SettingsCategories.Count)
            {
                currentCategoryIndex = 0;
            }
            UpdateUI();
            //NextCategory();
        }
        public void PreviousCategory()
        {
            currentCategoryIndex--;
            if (currentCategoryIndex < 0)
            {
                currentCategoryIndex = SettingsCategories.Count - 1;
            }
            UpdateUI();
            //PreviousCategory();
        }
        public ProceduralMenuPage GetSettingsPage(string name)
        {
            int num = SettingsCategories.FindIndex((SettingsCategory c) => c.Name == name);
            if (num < 0)
            {
                return null;
            }
            SettingsCategory settingsCategory = SettingsCategories[num];
            if (settingsCategory.Panel == null)
            {
                return null;
            }
            return settingsCategory.Panel.GetComponent<ProceduralMenuPage>();
        }
        private void RemovePage(string name)
        {
            ProceduralMenuPage page = GetSettingsPage(name);
            int num = SettingsCategories.FindIndex((SettingsCategory c) => c.Name == name);
            SettingsCategories.RemoveAt(num);
            Destroy(page.gameObject);
        }
        private void AddSettingsState(GameObject obj)
        {
            
            if (rfxMenuState == null)
            {
                rfxMenuState = obj.AddComponent<RFXMenuState>();
                RemoveOldStates(obj);
            }
            else
            {
                Main.Logger.Log("[ReplayFXSettingsState] ReplayFXSettingsState Already Exists");
            }
            
        }
        private void RemoveOldStates(GameObject obj)
        {
            SettingsState[] settingsstates = obj.GetComponentsInChildren<SettingsState>();
            if (settingsstates.Length > 0)
            {
                foreach (SettingsState state in settingsstates)
                {
                    Destroy(state);
                }
            }
        }
        private void AddSettingsPage(string name, ProceduralMenuPage menuPage, int index = -1)
        {
            int num = SettingsCategories.FindIndex((SettingsCategory c) => c.Panel == menuPage.gameObject);
            if (num >= 0)
            {
                SettingsCategory settingsCategory = SettingsCategories[num];
                settingsCategory.Name = name;
                SettingsCategories[num] = settingsCategory;
                return;
            }
            SettingsCategory settingsCategory2 = new SettingsCategory
            {
                Name = name,
                Panel = menuPage.gameObject,
            };
            menuPage.transform.SetParent(settingsPageParent);
            if (index < 0)
            {
                SettingsCategories.Add(settingsCategory2);
                return;
            }
            SettingsCategories.Insert(index, settingsCategory2);
        }
        public async Task<ProceduralMenuPage> CreateSettingsPage(string name, int index = -1)
        {
            int num = SettingsCategories.FindIndex((SettingsCategory c) => c.Name == name);
            ProceduralMenuPage proceduralMenuPage;
            if (num >= 0)
            {
                //SettingsMenuController.SettingsCategory settingsCategory = SettingsCategories[num];
                SettingsCategory settingsCategory = SettingsCategories[num];
                proceduralMenuPage = ((settingsCategory.Panel != null) ? settingsCategory.Panel.GetComponent<ProceduralMenuPage>() : null);
                if (proceduralMenuPage != null)
                {
                    if (index >= 0 && num != index)
                    {
                        SettingsCategories.RemoveAt(num);
                        SettingsCategories.Insert(index, settingsCategory);
                    }
                    return proceduralMenuPage;
                }
                Main.Logger.Error("Adding custom Setting page with same name as non-custom one");
                name += "(Custom)";
            }

            proceduralMenuPage = (await Addressables.InstantiateAsync(ProceduralMenuPage.prefabKey, settingsPageParent, false, true)).GetComponent<ProceduralMenuPage>();

            //var handle = Addressables.InstantiateAsync(ProceduralMenuPage.prefabKey, settingsPageParent, false, true);
            //GameObject pageObj = await handle.Task;
            //proceduralMenuPage = pageObj.GetComponent<ProceduralMenuPage>();

            //GameObject pageObj = await Addressables.InstantiateAsync(ProceduralMenuPage.prefabKey, settingsPageParent, false, true).Task;
            //proceduralMenuPage = pageObj.GetComponent<ProceduralMenuPage>();

            proceduralMenuPage.name = name + " Page";
            proceduralMenuPage.layoutGroup.spacing = 16f;
            AddSettingsPage(name, proceduralMenuPage, index);
            return proceduralMenuPage;
        }
        public async void CreateSettingsPage(string name, Action<ProceduralMenuPage> action, int index = -1)
        {
            ProceduralMenuPage proceduralMenuPage = await CreateSettingsPage(name, index);
            if (action != null)
            {
                action(proceduralMenuPage);
            }
        }
    }
}