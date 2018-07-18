using Modding;

namespace dreams
{
    
    public static class version_info
    {
        public const int SETTINGS_VER = 4;
        public const int SAVE_VER = 2;
    }
    
    public class dream_settings : IModSettings
    {
        public void reset()
        {
            BoolValues.Clear();
            IntValues.Clear();
            FloatValues.Clear();
            StringValues.Clear();
            
            settingsVersion = version_info.SETTINGS_VER;
        }
        
        public int settingsVersion { get => GetInt(); set => SetInt(value); }
    }
    
    public class dream_save_data : IModSettings
    {
        public void reset()
        {
            BoolValues.Clear();
            IntValues.Clear();
            FloatValues.Clear();
            StringValues.Clear();

            soulDreamFails = 0;
            falseDreamFails = 0;
            kinDreamFails = 0;

            soulDreamLevel = 1;
            falseDreamLevel = 1;
            kinDreamLevel = 1;
            
            settingsVersion = version_info.SAVE_VER;
        }

        public int soulDreamLevel { get => GetInt(); set => SetInt(value); }
        public int kinDreamLevel { get => GetInt(); set => SetInt(value); }
        public int falseDreamLevel { get => GetInt(); set => SetInt(value); }
        
        public int soulDreamFails { get => GetInt(); set => SetInt(value); }
        public int falseDreamFails { get => GetInt(); set => SetInt(value); }
        public int kinDreamFails { get => GetInt(); set => SetInt(value); }

        public int settingsVersion { get => GetInt(); private set => SetInt(value); }
    }
}