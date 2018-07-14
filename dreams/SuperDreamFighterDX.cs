using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ModCommon;
using Modding;
using On.TeamCherry;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable MemberCanBePrivate.Global because these could be used by other programs.
// ReSharper disable UnusedMember.Global because used implicitly but importing resharper stuff is for dungos.
// ReSharper disable ClassNeverInstantiated.Global because used implicitly bigmeme


namespace dreams
{
    // ReSharper disable once InconsistentNaming because mod api
    public class SuperDreamFighterDX : Mod <dream_save_data, dream_settings>, ITogglableMod
    {
        // Real arcade lovers have both machines.
        public bool igAvailable;
        public override string GetVersion()
        {
            string ver = global_vars.VERSION;
            int minAPI = 40;
            bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
            bool noModCommon = true;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly.GetTypes().All(type => type.Namespace != "ModCommon")) continue;
                    noModCommon = false;
                    break;
                }
                catch
                {
                    Log(assembly.FullName + " failed to load.");
                }
            }
            
            if (apiTooLow) ver += " (Error: ModAPI too old)";
            if (noModCommon) ver += " (Error: Super Dream Fighter DX requires ModCommon)";

            if (igAvailable && !apiTooLow && !noModCommon)
            {
                ver = " GOTY " + ver;
            }
            Log("Version is" + ver);
            
            return ver;
        }
        
        public override void Initialize()
        {
            igAvailable = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "infinitegrimm" select type).Any();
            setupSettings();
            ModHooks.Instance.AfterSavegameLoadHook += saveGame;
            ModHooks.Instance.NewGameHook += addComponent;
            ModHooks.Instance.ApplicationQuitHook += SaveGlobalSettings;
            ModHooks.Instance.SavegameSaveHook += saveLocalData;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += resetModSaveData;
        }

        private void saveLocalData(int id)
        {
            Settings.falseDreamFails = global_vars.falseDreamFails;
            Settings.falseDreamLevel = global_vars.falseDreamLevel;
            Settings.kinDreamFails = global_vars.kinDreamFails;
            Settings.kinDreamLevel = global_vars.kinDreamLevel;
            Settings.soulDreamFails = global_vars.soulDreamFails;
            Settings.soulDreamLevel = global_vars.soulDreamLevel;
            //Settings.soulDreamLevel = global_vars;
        }

        private void setupSettings()
        {
            string settingsFilePath = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            bool forceReloadGlobalSettings = (GlobalSettings != null && GlobalSettings.settingsVersion != version_info.SETTINGS_VER);

            if (forceReloadGlobalSettings || !File.Exists(settingsFilePath))
            {
                if (forceReloadGlobalSettings)
                {
                    Log("Settings outdated! Rebuilding.");
                }
                else
                {
                    Log("Settings not found, rebuilding... File will be saved to: " + settingsFilePath);
                }

                GlobalSettings.reset();
            }
            SaveGlobalSettings();
        }
        // quick hack to fix problem in modding api... no seriously.
        // local data doesn't reset properly.
        private void resetModSaveData(Scene arg0, Scene arg1)
        {
            if (arg1.name != "Menu_Title") return;

            Settings.falseDreamFails = 0;
            Settings.falseDreamLevel = 1;
            Settings.kinDreamFails = 0;
            Settings.kinDreamLevel = 1;
            Settings.soulDreamFails = 0;
            Settings.soulDreamLevel = 1;
        }

        private void saveGame(SaveGameData data)
        {
            global_vars.falseDreamFails = Settings.falseDreamFails;
            global_vars.falseDreamLevel = Settings.falseDreamLevel;
            global_vars.kinDreamFails = Settings.kinDreamFails;
            global_vars.kinDreamLevel = Settings.kinDreamLevel;
            global_vars.soulDreamFails = Settings.soulDreamFails;
            global_vars.soulDreamLevel = Settings.soulDreamLevel;
            addComponent();
        }

        private void addComponent()
        {
            GameManager.instance.gameObject.AddComponent<unending_dreams>();
            GameManager.instance.gameObject.AddComponent<dream_manager>();
            // add components here
        }

        public override int LoadPriority()
        {
            return global_vars.LOAD_ORDER;
        }

        public void Unload()
        {
            Log("Disabling! If you see any more non-settings messages by this mod please report as an issue.");
            ModHooks.Instance.AfterSavegameLoadHook -= saveGame;
            ModHooks.Instance.NewGameHook -= addComponent;
            
            ModHooks.Instance.ApplicationQuitHook -= SaveGlobalSettings;
            ModHooks.Instance.SavegameSaveHook -= saveLocalData;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= resetModSaveData;
            
        }

    }
}