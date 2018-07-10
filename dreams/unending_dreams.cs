using System.Linq;
using Modding;
using UnityEngine;

namespace dreams
{
    
    // Totally original mod idea, do not steal.
    public class unending_dreams : MonoBehaviour
    {
        private static readonly string[] FALSE_DREAM_BOOLS = new[]
        {
            "falseKnightDreamDefeated",
            "infectedKnightDreamDefeated",
            "mageLordDreamDefeated"
        };


        private void OnDestroy()
        {
            ModHooks.Instance.GetPlayerBoolHook -= fakeNoDefeatBosses;
        }

        private void Start()
        {
            ModHooks.Instance.GetPlayerBoolHook += fakeNoDefeatBosses;
        }
        
        

        private static bool fakeNoDefeatBosses(string originalset)
        {
            if (FALSE_DREAM_BOOLS.Any(s => originalset == s))
            {
                return false;
            }
            
            return PlayerData.instance.GetBoolInternal(originalset);
        }

        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }

    }
}