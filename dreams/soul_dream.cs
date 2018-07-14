using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using ModCommon;
using ModCommon.Util;

namespace dreams
{
    public class soul_dream
    {
        private readonly GameObject soulGuy;
        //private GameObject 

        private double danceSpeed;
        private int maxHealth;

        private const int NORMAL_HITS_TO_STUN = 18;
        private const int NORMAL_HITS_TO_FAST_STUN = 10;
        
        public int hitsToStun;
        // lol yeah right you ain't getting one of these off.
        public int hitsToFastStun;

        public CustomEnemySpeed soulController;

        #region Speed Data
        
        private static readonly CustomEnemySpeed.AnimationData[] ANIMATION_DATAS = new[]
        {
            new CustomEnemySpeed.AnimationData(1.5f, "Tele In Quick"),
            new CustomEnemySpeed.AnimationData(2f, "Dash Antic"),
            new CustomEnemySpeed.AnimationData(2f, "SummonToIdle"),
            new CustomEnemySpeed.AnimationData(2f, "Summon"),
            new CustomEnemySpeed.AnimationData(2f, "Dash"),
            new CustomEnemySpeed.AnimationData(2f, "TurnToIdle"),
            new CustomEnemySpeed.AnimationData(2f, "Quake Antic Quick"),
            new CustomEnemySpeed.AnimationData(2f, "Tele In")
        };

        private CustomEnemySpeed.WaitData[] waitDatas;

        private CustomEnemySpeed.SetVelocity2DData[] velocityDatas;

        #endregion

        public soul_dream(GameObject soulGuy, int level)
        {
            this.soulGuy = soulGuy;

            PlayMakerFSM stunFSM = this.soulGuy.LocateMyFSM("Stun");
            stunFSM.GetState("Idle").Transitions = new FsmTransition[0];
            calculateDifficultyFromLevel(level);
            addFSMDatas();

            log("hits to stun: " + hitsToStun + " hits to fast stun: " + hitsToFastStun + " health: " + maxHealth +
                " dance speed: " + danceSpeed);

            soulController = this.soulGuy.GetOrAddComponent<CustomEnemySpeed>();
            
            soulController.SetEnemyMaxHealth(maxHealth);
            soulController.UpdateDanceSpeed(danceSpeed);
            soulController.UpdateDanceSpeed(3.0f);

            foreach (CustomEnemySpeed.WaitData w in waitDatas)
            {
                soulController.AddWaitData(w);
            }

            foreach (CustomEnemySpeed.AnimationData a in ANIMATION_DATAS)
            {
                soulController.AddAnimationData(a);
            }

            foreach (CustomEnemySpeed.SetVelocity2DData v in velocityDatas)
            {
                soulController.AddSetVelocity2DData(v);
            }
            
            soulController.StartSpeedMod();
        }
        
        private void addFSMDatas()
        {
            GameObject afterQuake = soulGuy.FindGameObjectInChildren("Quake Hit");
            
            // I have no idea which one does what.
            GameObject quakePillarAttack = soulGuy.FindGameObjectInChildren("Quake Pillar");
            
            //afterQuake.PrintSceneHierarchyTree("q1.txt");
            //quakePillarAttack.PrintSceneHierarchyTree("q2.txt");

            if (afterQuake == null || quakePillarAttack == null)
            {
                log("SUPER HUGE ERROR THIS WILL BREAK THE GAME OBJECTS ARE NULL!!!!");
            }            
            waitDatas = new[]
            {
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "HS Summon"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "HS Orb"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "HS Stop"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "HS Ret Left"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "Hs Ret Right"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "HS Dissipate"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "Shot Antic"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "Shot"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "Re Pos"),
                new CustomEnemySpeed.WaitData(2.0f, "Mage Lord", "Quake Waves"),
                new CustomEnemySpeed.WaitData(1.5f, "Mage Lord", "Fake Down"),
                new CustomEnemySpeed.WaitData(2.0f, "deactivate", "Wait", -1, afterQuake)
                //new CustomEnemySpeed.WaitData(1.5f, ), 

            };


            velocityDatas = new[]
            {
                new CustomEnemySpeed.SetVelocity2DData(2.0f, "Mage Lord", "HS Right"),
                new CustomEnemySpeed.SetVelocity2DData(2.0f, "Mage Lord", "Charge Left"),
                new CustomEnemySpeed.SetVelocity2DData(2.0f, "Mage Lord", "Charge Right"),
                new CustomEnemySpeed.SetVelocity2DData(2.0f, "Mage Lord", "Quake Down"),
                new CustomEnemySpeed.SetVelocity2DData(2.0f, "Mage Lord", "Fake Down")
            };
        }


        private void calculateDifficultyFromLevel(int level)
        {
            danceSpeed = Math.Pow((double) level, 0.25) + 0.2;
            double healthMod = Math.Pow((double) level, 0.6) + 0.2;
            double stunMod = Math.Pow((double) level, 0.4) + 0.2;
            
            log("health mod is " + healthMod + " and stun mod is " + stunMod + " because level is " + level);

            hitsToFastStun = (int)(NORMAL_HITS_TO_FAST_STUN * stunMod);
            hitsToStun = (int)(NORMAL_HITS_TO_STUN * stunMod);

            maxHealth = (int) (soulGuy.GetComponent<HealthManager>().hp * healthMod);



        }
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}