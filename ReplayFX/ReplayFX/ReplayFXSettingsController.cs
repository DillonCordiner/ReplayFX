using Malee;
using SkaterXL.Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using ReplayFX.UI;
using GameManagement;
using UnityEngine.ResourceManagement;
using UnityEngine.AddressableAssets;
using ReplayFX;
using Rewired;

namespace ReplayFX
{
    public class ReplayFXSettingsController : MonoBehaviour
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

        public ProceduralMenuPage cameraSettings;
        public ProceduralMenuPage keyframeSettings;
        //public ProceduralMenuPage colorSettings;

        //public ReorderableArray<SettingsMenuController.SettingsCategory> SettingsCategories;
        [Reorderable]
        public SettingsCategoryArray SettingsCategories;

        public CategoryButton SettingsCategoryButton;
        public Transform settingsPageParent;

        public ReplayFXSettingsState rfxSettingsState;
        public GameObject clonedMenu;

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
                Main.Logger.LogException($"Failed to initialize replay settings menu:", ex);
            }
            
            //SetupClonedMenu(SettingsMenuController.Instance.gameObject);
            //RemoveDefaultPages();
            //SetStartPage("Camera Settings");
            //SetCurrentCategory("Camera Settings");
        }
        
        public async Task InitializeMenuAsync()
        {
            if (pagesCreated) return;

            cameraSettings = await PageBuilder.BuildCameraPageAsync();
            keyframeSettings = await PageBuilder.BuildKeyframePageAsync();

            pagesCreated = true;
        }
        private void OnDestroy()
        {
            if (SettingsCategoryButton != null)
            {
                SettingsCategoryButton.OnNextCategory -= NextCategory;
                SettingsCategoryButton.OnPreviousCategory -= PreviousCategory;
            }
        }
        public void SetupClonedMenu(GameObject originalMenuPrefab)
        {
            if (clonedMenu == null)
            {
                clonedMenu = Instantiate(originalMenuPrefab);
                clonedMenu.transform.SetParent(Main.ScriptManager.transform);
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
        }

        private void UpdateUI()
        {
            currentCategoryIndex = Mathf.Clamp(currentCategoryIndex, 0, SettingsCategories.Count - 1);
            for (int i = 0; i < SettingsCategories.Count; i++)
            {
                bool isActive = (i == currentCategoryIndex);

                SettingsCategories[i].Panel.SetActive(isActive);

                if (isActive && SettingsCategoryButton != null)
                {
                    SettingsCategoryButton.SetText(SettingsCategories[i].Name, false);
                }
            }    
        }
        public void SetStartPage(string settingsPage)
        {
            for (int i = 0; i < SettingsCategories.Length; i++)
            {
                if (SettingsCategories[i].Name == settingsPage)
                {
                    currentCategoryIndex = i;
                    return;
                }
            }
        }
        public void SetCurrentCategory(string categoryName)
        {
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
            /*
            foreach (SettingsMenuController.SettingsCategory catagory in SettingsCategories)
            {
                if (catagory.Name == name)
                {
                    SettingsCategories.Remove(catagory);
                    Destroy(catagory.Panel);
                }
            }
            */
        }
        private void AddSettingsState(GameObject obj)
        {
            if (rfxSettingsState == null)
            {
                rfxSettingsState = obj.AddComponent<ReplayFXSettingsState>();
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
        /* old SetupClonedMenu
        public void SetupClonedMenu(GameObject originalMenuPrefab)
        {
            GameObject clonedMenu = Instantiate(originalMenuPrefab);
            SettingsMenuController originalController = clonedMenu.GetComponentInChildren<SettingsMenuController>();
            if (originalController != null)
            {
                Destroy(originalController);
            }
            clonedMenu.transform.SetParent(Main.ScriptManager.transform);
            AddSettingsState(clonedMenu);
            //SettingsCategories = SettingsMenuController.Instance.SettingsCategories; // change later to match clone settings panels
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
        }
        */
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
                //platforms = RuntimePlatformFlag.All,
                //debugOnly = false
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
            //GameObject pageObj = await Addressables.InstantiateAsync(ProceduralMenuPage.prefabKey, settingsPageParent, false, true).Task;
            //proceduralMenuPage = pageObj.GetComponent<ProceduralMenuPage>();
            //proceduralMenuPage = CreateProceduralPage();

            proceduralMenuPage.name = name + " Page";
            proceduralMenuPage.layoutGroup.spacing = 16f;
            AddSettingsPage(name, proceduralMenuPage, index);
            return proceduralMenuPage;
        }
        /*
        private ProceduralMenuPage CreateProceduralPage()
        {
            // 1. Find ANY existing vanilla ProceduralMenuPage in the hierarchy
            ProceduralMenuPage vanillaPage = SettingsMenuController.Instance.GetComponentInChildren<ProceduralMenuPage>(true);

            if (vanillaPage != null)
            {
                // 2. Clone it synchronously (No Addressables required)
                GameObject clone = Instantiate(vanillaPage.gameObject, settingsPageParent);

                // 3. Get the component and wipe all the cloned vanilla items
                ProceduralMenuPage newPage;
                newPage = clone.GetComponent<ProceduralMenuPage>();
                newPage.RemoveAll(); // Clears the list and destroys the cloned items

                return newPage;
            }
            else
            {
                Main.Logger.Log("Could not find a vanilla page to clone!");
                return null;
            }
        }
        */
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