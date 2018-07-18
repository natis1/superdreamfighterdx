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
            igAvailable = false;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly.GetTypes().All(type => type.Namespace != "infinitegrimm")) continue;
                    igAvailable = true;
                    break;
                }
                catch
                {
                    Log(assembly.FullName + " failed to load.");
                }
            }
            setupSettings();
            
            ModHooks.Instance.AfterSavegameLoadHook += loadSaveGame;
            ModHooks.Instance.NewGameHook += newGame;
            ModHooks.Instance.ApplicationQuitHook += SaveGlobalSettings;
        }

        private void newGame()
        {
            Settings.reset();
            global_vars.gameData = Settings;
            
            addComponent();
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
            Settings.reset();
        }

        private void loadSaveGame(SaveGameData data)
        {
            global_vars.gameData = Settings;

            bool criticalError = false;
            
            if (global_vars.gameData.falseDreamLevel < 1)
            {
                global_vars.gameData.falseDreamLevel = 1;
                Log("Critical error serializing settings.");
                criticalError = true;
            }
            if (global_vars.gameData.kinDreamLevel < 1)
            {
                global_vars.gameData.kinDreamLevel = 1;
                Log("Critical error serializing settings.");
                criticalError = true;
            }

            if (global_vars.gameData.soulDreamLevel < 1)
            {
                global_vars.gameData.soulDreamLevel = 1;
                Log("Critical error serializing settings.");
                criticalError = true;
            }

            if (criticalError)
            {
                Settings.reset();
                loadSaveGame(data);
            }
            else
            {
                addComponent();
            }
        }

        private void addComponent()
        {
            Log("Current levels are: FC " + global_vars.gameData.falseDreamLevel + " with " + global_vars.gameData.falseDreamFails + " fails");
            Log("Lost Kin: " + global_vars.gameData.kinDreamLevel + " with " + global_vars.gameData.kinDreamFails + " fails");
            Log("Soul Guy: " + global_vars.gameData.soulDreamLevel + " with " + global_vars.gameData.soulDreamFails + " fails");
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
            ModHooks.Instance.AfterSavegameLoadHook -= loadSaveGame;
            ModHooks.Instance.NewGameHook -= addComponent;
            
            ModHooks.Instance.ApplicationQuitHook -= SaveGlobalSettings;
        }

    }
}