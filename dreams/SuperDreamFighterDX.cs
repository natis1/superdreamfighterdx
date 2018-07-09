using System;
using System.IO;
using System.Linq;
using Modding;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global because these could be used by other programs.
// ReSharper disable UnusedMember.Global because used implicitly but importing resharper stuff is for dungos.


namespace dreams
{
    // ReSharper disable once InconsistentNaming because mod api
    public class SuperDreamFighterDX : Mod <dream_save_data, dream_settings>, ITogglableMod
    {
        public const string VERSION = "0.2.3";
        public const int LOAD_ORDER = 30;

        // Real arcade lovers have both machines.
        public bool igAvailable;

        public override string GetVersion()
        {
            string ver = VERSION;
            int minAPI = 40;
            bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
            bool noModCommon = !(from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "ModCommon" select type).Any();
            
            if (apiTooLow) ver += " (Error: ModAPI too old)";
            if (noModCommon) ver += " (Error: Super Dream Fighter DX requires ModCommon)";

            if (igAvailable && !apiTooLow && !noModCommon)
            {
                ver = " GOTY " + ver;
            }
            return ver;
        }
        
        public override void Initialize()
        {
            igAvailable = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "infinitegrimm" select type).Any();
            setupSettings();
            
            ModHooks.Instance.AfterSavegameLoadHook += saveGame;
            ModHooks.Instance.NewGameHook += addComponent;
            

            ModHooks.Instance.ApplicationQuitHook += SaveGlobalSettings;
        }

        private void setupSettings()
        {
            string settingsFilePath = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            bool forceReloadGlobalSettings = (GlobalSettings != null && GlobalSettings.SettingsVersion != version_info.SETTINGS_VER);

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

        private void saveGame(SaveGameData data)
        {
            addComponent();
        }

        private void addComponent()
        {
            // add components here
        }

        public override int LoadPriority()
        {
            return LOAD_ORDER;
        }

        public void Unload()
        {
            Log("Disabling! If you see any more non-settings messages by this mod please report as an issue.");
            ModHooks.Instance.AfterSavegameLoadHook -= saveGame;
            ModHooks.Instance.NewGameHook -= addComponent;
            
        }

    }
}