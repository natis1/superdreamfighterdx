using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
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

        private const int NORMAL_HITS_TO_STUN = 12;
        private const int NORMAL_HITS_TO_FAST_STUN = 7;
        private const float STUN_TIMEOUT = 2.0f;

        public GameObject kinGuy;
        public CustomEnemySpeed kinController;
        public dream_stun_manager kinStunner;
        public PlayMakerFSM kinPrimaryFSM;
        
        #region SPEED DATA

        private static readonly CustomEnemySpeed.AnimationData[] ANIMATION_DATAS = new[]
        {
            new CustomEnemySpeed.AnimationData(2.0f, "Roar Loop"),
            new CustomEnemySpeed.AnimationData(2.0f, "Roar End"),
            new CustomEnemySpeed.AnimationData(2.0f, "Jump"),
            new CustomEnemySpeed.AnimationData(2.0f, "Jump Antic"),
            new CustomEnemySpeed.AnimationData(2.0f, "Land"),
            new CustomEnemySpeed.AnimationData(2.0f, "Downstab Antic Quick"),
            new CustomEnemySpeed.AnimationData(2.0f, "Downstab"),
            new CustomEnemySpeed.AnimationData(2.0f, "Downstab Land"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Antic 1"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Attack 1"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Recover"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Attack 2"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Attack 3"),
            new CustomEnemySpeed.AnimationData(2.0f, "Idle"),
            new CustomEnemySpeed.AnimationData(2.0f, "Overhead Antic"),
            new CustomEnemySpeed.AnimationData(2.0f, "Overhead Slashing"),
            new CustomEnemySpeed.AnimationData(2.0f, "Overhead Slash"),
            new CustomEnemySpeed.AnimationData(2.0f, "Evade Antic"),
            new CustomEnemySpeed.AnimationData(2.0f, "Evade"),
            new CustomEnemySpeed.AnimationData(2.0f, "Evade Recover"),
            new CustomEnemySpeed.AnimationData(2.0f, "Shake Antic"),
            new CustomEnemySpeed.AnimationData(2.0f, "Shake Loop"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Antic 2"),
            new CustomEnemySpeed.AnimationData(2.0f, "Dash Antic 3")
        };

        private static readonly CustomEnemySpeed.WaitData[] WAIT_DATAS = new[]
        {
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Shake End"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Roar"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Ohead Antic"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Evade Dash"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Shaking Start"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Dash Antic 2"),
            new CustomEnemySpeed.WaitData(2.0f, "IK Control", "Shake End")
        };


        private static readonly CustomEnemySpeed.SetVelocity2DData[] VELOCITY_DATAS = new[]
        {
            // Lost lord used 1.5 so I should too for memes
            new CustomEnemySpeed.SetVelocity2DData(1.5f, "IK Control", "Dstab Fall"),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 6),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 8),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 10),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 12),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 14),
            new CustomEnemySpeed.SetVelocity2DData(2.0f, "IK Control", "Dstab Land", 16),
            new CustomEnemySpeed.SetVelocity2DData(0.7f, "IK Control", "Jump 2")

        };
        
        
        #endregion


        public kin_dream(GameObject lostKin, int level)
        {
            kinGuy = lostKin;
            
            PlayMakerFSM stunFSM = this.kinGuy.LocateMyFSM("Stun Control");
            stunFSM.GetState("Idle").Transitions = new FsmTransition[0];
            stunFSM.SetState("Idle");
            calculateDifficultyFromLevel(level);
            // Infinite Kin confirmed.
            kinPrimaryFSM = kinGuy.LocateMyFSM("IK Control");
            
            
            
            kinController = kinGuy.GetOrAddComponent<CustomEnemySpeed>();
            kinController.SetEnemyMaxHealth(maxHealth);
            kinController.UpdateDanceSpeed(danceSpeed);
            
            //kinController.UpdateDanceSpeed(3.0);
            
            
            foreach (CustomEnemySpeed.AnimationData a in ANIMATION_DATAS)
            {
                kinController.AddAnimationData(a);
            }

            foreach (CustomEnemySpeed.SetVelocity2DData v in VELOCITY_DATAS)
            {
                kinController.AddSetVelocity2DData(v);
            }
            
            foreach (CustomEnemySpeed.WaitData w in WAIT_DATAS)
            {
                kinController.AddWaitData(w);
            }
            

            kinStunner = kinGuy.GetOrAddComponent<dream_stun_manager>();
            kinStunner.fastStunTimeoutTime = STUN_TIMEOUT;
            kinStunner.hitsToFastStun = hitsToFastStun;
            kinStunner.hitsToStun = hitsToStun;
            kinStunner.speed = kinController;
            kinStunner.stunFSM = stunFSM;
            kinStunner.stunStateChecker = kinPrimaryFSM;
            kinStunner.stunStateName = "Stun";
            kinStunner.validStunStates = null;
            
            
            
            kinGuy.GetComponent<Recoil>().enabled = false;
            
            //TODO: Move to custom enemy speed v3

            kinPrimaryFSM.GetAction<FloatMultiply>("Aim Dstab", 3).multiplyBy = (float) (danceSpeed * 2.0);
            kinPrimaryFSM.GetAction<FloatMultiply>("Aim Jump", 3).multiplyBy = (float) danceSpeed;
            
            // hacky fix for kin resetting speed provided by 56.
            kinPrimaryFSM.GetAction<Wait>("Dstab Recover", 0).time = 0f;
            
            
            kinController.StartSpeedMod();

            if (level >= 5)
            {
                difficultyBuffOne();
            }

            if (level >= 8)
            {
                difficultyBuffTwo();
            }
            
            log("Load modified kin complete without error");

            

        }

        public void restoreOrigValues()
        {
            kinController.RestoreOriginalSpeed();
            kinController.SetActive(false);
        }


        private void difficultyBuffOne()
        {
            kinPrimaryFSM.GetAction<WaitRandom>("Idle", 5).timeMax = 0.01f;
            kinPrimaryFSM.GetAction<WaitRandom>("Idle", 5).timeMin = 0.001f;
        }

        private void difficultyBuffTwo()
        {
            kinPrimaryFSM.GetAction<SetDamageHeroAmount>("Roar End", 3).damageDealt.Value = 2;
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