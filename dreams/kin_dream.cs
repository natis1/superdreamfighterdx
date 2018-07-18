using System;
using HutongGames.PlayMaker;
using ModCommon;
using UnityEngine;
using ModCommon.Util;

namespace dreams
{
    public class kin_dream
    {
        private double danceSpeed;
        private int maxHealth;

        private int hitsToFastStun;
        private int hitsToStun;

        private const int NORMAL_HITS_TO_STUN = 18;
        private const int NORMAL_HITS_TO_FAST_STUN = 10;

        public GameObject kinGuy;
        public CustomEnemySpeed kinController;
        public PlayMakerFSM kinPrimaryFSM;

        
        public kin_dream(GameObject lostKin, int level)
        {
            kinGuy = lostKin;
            
            PlayMakerFSM stunFSM = this.kinGuy.LocateMyFSM("Stun");
            stunFSM.GetState("Idle").Transitions = new FsmTransition[0];
            calculateDifficultyFromLevel(level);
            // Infinite Kin confirmed.
            kinPrimaryFSM = kinGuy.LocateMyFSM("IK Control");



        }
        
        
        
        private void calculateDifficultyFromLevel(int level)
        {
            danceSpeed = Math.Pow((double) level, 0.25) + 0.2;
            double healthMod = Math.Pow((double) level, 0.6) + 0.2;
            double stunMod = Math.Pow((double) level, 0.4) + 0.2;
            
            log("health mod is " + healthMod + " and stun mod is " + stunMod + " because level is " + level);

            hitsToFastStun = (int)(NORMAL_HITS_TO_FAST_STUN * stunMod);
            hitsToStun = (int)(NORMAL_HITS_TO_STUN * stunMod);

            maxHealth = (int) (kinGuy.GetComponent<HealthManager>().hp * healthMod);
        }
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}