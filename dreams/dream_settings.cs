using Modding;

namespace dreams
{
    
    public static class version_info
    {
        public const int SETTINGS_VER = 4;
        public const int SAVE_VER = 1;
    }
    
    public class dream_settings : IModSettings
    {
        public void reset()
        {
            BoolValues.Clear();
            IntValues.Clear();
            FloatValues.Clear();
            StringValues.Clear();
            
            SettingsVersion = version_info.SETTINGS_VER;
        }
        
        public int SettingsVersion { get => GetInt(); set => SetInt(value); }
    }
    
    public class dream_save_data : IModSettings
    {
        
        
        public int soulDreamLevel;
        public int kinDreamLevel;
        public int falseDreamLevel;

        public int SettingsVersion;


    }
}