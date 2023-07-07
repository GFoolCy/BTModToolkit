#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace CrossLink
{
    public class TemplateWizardAsset : ScriptableObject
    {
        public enum ModType
        {
            Weapon,
            Song,
            Scene,
            Role,
            Skin,
            Avatar
        }

        static TemplateWizardAsset wizard;

        public string newModFolderName = "";
        string previousNewModFolderName = "";

        public ModType selectedModType = ModType.Weapon;
        ModType previousselectedModType = ModType.Weapon;
        [ConditionalField("selectedModType", false, ModType.Weapon, ModType.Song)]public StoreItemInfo storeItemInfo;
        [ConditionalField("selectedModType", false, ModType.Scene)]public SceneModInfo sceneModInfo;
        [ConditionalField("selectedModType", false, ModType.Role)]public RoleModInfo roleModInfo;
        [ConditionalField("selectedModType", false, ModType.Skin)]public SkinInfo skinInfo;
        [ConditionalField("selectedModType", false, ModType.Avatar)]public AvatarInfo avatarInfo;

        [EasyButtons.Button]
        void GenerateTemplate()
        {
            if (newModFolderName == "") {
                SetStatusMessage("Please enter a mod folder name.", MessageType.Warning);
                return;
            }
            if (newModFolderName.Contains(" ")) {
                SetStatusMessage("Mod folder name should not contain spaces.", MessageType.Warning);
                return;
            }
            if (!char.IsUpper(newModFolderName[0])) {
                SetStatusMessage("Mod folder name should start with a capital.", MessageType.Warning);
                return;
            }

            if (AssetDatabase.IsValidFolder($"Assets/Build/{newModFolderName}")){
                SetStatusMessage("The mod already exists", MessageType.Warning);
                return;
            }

            string guid = AssetDatabase.CreateFolder("Assets/Build", newModFolderName);
            string newModFolderPath = AssetDatabase.GUIDToAssetPath(guid);

            if(selectedModType == ModType.Weapon) {
                GenerateWeaponTemplate(newModFolderPath);
            }

            if(selectedModType == ModType.Song) {
                GenerateSongTemplate(newModFolderPath);
            }

            if(selectedModType == ModType.Scene) {
                GenerateSceneTemplate(newModFolderPath);
            }

            if(selectedModType == ModType.Role) {
                GenerateRoleTemplate(newModFolderPath);
            }

            if(selectedModType == ModType.Skin) {
                GenerateSkinTemplate(newModFolderPath);
            }

            if(selectedModType == ModType.Avatar) {
                GenerateAvatarTemplate(newModFolderPath);
            }

            if(AddressableConfig.GetConfig().addressablePaths.Contains(newModFolderPath) == false) {
                AddressableConfig.GetConfig().addressablePaths.Add(newModFolderPath);
            }

            AddressableHelper.CreateAndRefreshAddressables();
            
            SetStatusMessage("Success!", MessageType.Info);
        }

        private void CreateWeaponPrefab(string newModFolderName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Toolkit/Prefabs/RootWeaponNode.prefab");
            var instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.SaveAsPrefabAssetAndConnect(instantiatedPrefab, $"Assets/Build/{newModFolderName}/Weapon/{newModFolderName}.prefab", InteractionMode.AutomatedAction);
            DestroyImmediate(instantiatedPrefab);
        }

        private void CreateRolePrefab(string newModFolderName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Toolkit/Prefabs/RootRoleNode.prefab");
            var instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.SaveAsPrefabAssetAndConnect(instantiatedPrefab, $"Assets/Build/{newModFolderName}/Role/{newModFolderName}.prefab", InteractionMode.AutomatedAction);
            DestroyImmediate(instantiatedPrefab);
        }

        private void CreateSkinPrefab(string newModFolderName)
        {
            GameObject gameObject = new GameObject();
            PrefabUtility.SaveAsPrefabAsset(gameObject, $"Assets/Build/{newModFolderName}/Skin/{newModFolderName}.prefab");
            DestroyImmediate(gameObject);
        }

        private void CreateAvatarPrefab(string newModFolderName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Toolkit/Prefabs/RootAvatarNode.prefab");
            var instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            PrefabUtility.SaveAsPrefabAssetAndConnect(instantiatedPrefab, $"Assets/Build/{newModFolderName}/Avatar/{newModFolderName}.prefab", InteractionMode.AutomatedAction);
            DestroyImmediate(instantiatedPrefab);
        }

        private void CreateIcon(string newModFolderName)
        {
            AssetDatabase.CopyAsset(
                $"Assets/Toolkit/TemplateWizard/Dummy/icon.png", 
                $"Assets/Build/{newModFolderName}/ICon/{newModFolderName}.png"
            );
        }

        private void CreateScene(string newModFolderName)
        {
            AssetDatabase.CopyAsset(
                $"Assets/Toolkit/TemplateWizard/Dummy/scene.unity", 
                $"Assets/Build/{newModFolderName}/Scene/{newModFolderName}.unity"
            );
        }

        private void CreateSceneInitIfNotAvailable()
        {
            var commonSceneFolder = "Assets/Build/CommonScene";

            if (!Directory.Exists($"{commonSceneFolder}/Script")) {
                Directory.CreateDirectory($"{commonSceneFolder}/Script");
            }
            
            if (!File.Exists($"{commonSceneFolder}/Script/SceneInitScript.txt")) {
                AssetDatabase.CopyAsset(
                    "Assets/Toolkit/TemplateWizard/Dummy/SceneInitScript.txt", 
                    $"{commonSceneFolder}/Script/SceneInitScript.txt"
                );
            }
            
            if(AddressableConfig.GetConfig().addressablePaths.Contains(commonSceneFolder) == false) {
                AddressableConfig.GetConfig().addressablePaths.Add(commonSceneFolder);
            }

            AddressableHelper.CreateAndRefreshAddressables();
        }

        static public TemplateWizardAsset GetWizard()
        {
            wizard = Resources.Load("TemplateWizard") as TemplateWizardAsset;
            return wizard;
        }

        [MenuItem("Tools/Template Wizard", false, 10)]
        static void SelectAddressablesConfig()
        {
            Selection.activeObject = GetWizard();
        }

        void OnValidate() {
            if(previousNewModFolderName != newModFolderName) {
                previousNewModFolderName = newModFolderName;
                OnImportantChange();
            }

            if(previousselectedModType != selectedModType) {
                previousselectedModType = selectedModType;
                OnImportantChange();
            }
        }
 
        void OnImportantChange() {
            if(selectedModType == ModType.Weapon){
                storeItemInfo.addStoreItemName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                storeItemInfo.itemType = ItemInfoConfig.ItemType.Weapon;
                return;
            }
            if(selectedModType == ModType.Song){
                storeItemInfo.addStoreItemName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                storeItemInfo.itemType = ItemInfoConfig.ItemType.Music;
                return;
            }
            if(selectedModType == ModType.Scene){
                sceneModInfo.sceneName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                return;
            }
            if(selectedModType == ModType.Role){
                roleModInfo.roleName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                return;
            }
            if(selectedModType == ModType.Skin){
                skinInfo.skinName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                return;
            }
            if(selectedModType == ModType.Avatar){
                avatarInfo.avatarName = AddressableConfig.GetConfig().GetPrefix() + newModFolderName;
                return;
            }
        }

        void SetStatusMessage(string statusMessage, MessageType statusMessageType) {
            if(statusMessageType == MessageType.Info){
                Debug.Log(statusMessage);
            } else {
                Debug.LogError(statusMessage);
            }
        }

        void GenerateWeaponTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Weapon");

            CreateIcon(newModFolderName);
            CreateWeaponPrefab(newModFolderName);
            
            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>();
            itemInfoConfig.storeItemInfo = new StoreItemInfo[1];
            itemInfoConfig.storeItemInfo[0] = storeItemInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }

        void GenerateSongTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Audio");

            CreateIcon(newModFolderName);

            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>();
            itemInfoConfig.storeItemInfo = new StoreItemInfo[1];
            itemInfoConfig.storeItemInfo[0] = storeItemInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }

        void GenerateSceneTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Scene");
            AssetDatabase.CreateFolder(newModFolderPath, "Script");

            CreateIcon(newModFolderName);
            CreateScene(newModFolderName);
            CreateSceneInitIfNotAvailable();

            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>();
            itemInfoConfig.sceneModInfo = new SceneModInfo[1];
            itemInfoConfig.sceneModInfo[0] = sceneModInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }

        void GenerateRoleTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Audio");
            AssetDatabase.CreateFolder(newModFolderPath, "Role");

            CreateIcon(newModFolderName);
            CreateRolePrefab(newModFolderName);

            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>();  
            itemInfoConfig.roleModInfo = new RoleModInfo[1];
            itemInfoConfig.roleModInfo[0] = roleModInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }

        void GenerateSkinTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Skin");

            CreateIcon(newModFolderName);
            CreateSkinPrefab(newModFolderName);

            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>();   
            itemInfoConfig.skinInfo = new SkinInfo[1];
            itemInfoConfig.skinInfo[0] = skinInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }

        void GenerateAvatarTemplate(string newModFolderPath){ 
            AssetDatabase.CreateFolder(newModFolderPath, "ICon");
            AssetDatabase.CreateFolder(newModFolderPath, "Config");
            AssetDatabase.CreateFolder(newModFolderPath, "Avatar");

            CreateIcon(newModFolderName);
            CreateAvatarPrefab(newModFolderName);

            ItemInfoConfig itemInfoConfig = CreateInstance<ItemInfoConfig>(); 
            itemInfoConfig.avatarInfo = new AvatarInfo[1];
            itemInfoConfig.avatarInfo[0] = avatarInfo;
            AssetDatabase.CreateAsset(itemInfoConfig, $"Assets/Build/{newModFolderName}/Config/{newModFolderName}.asset");
            AssetDatabase.SaveAssets();
        }
    }
}


#endif