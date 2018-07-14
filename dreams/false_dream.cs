using System;
using HutongGames.PlayMaker;
using ModCommon;
using UnityEngine;

namespace dreams
{
    public class false_dream
    {
        private readonly GameObject falseGuy;
        //private GameObject 

        private double danceSpeed;
        private int maxHealth;

        private const int DEFAULT_HEALTH = 360;

        private PlayMakerFSM realHPChecker;
        
        public int hitsToStun;
        // lol yeah right you ain't getting one of these off.
        public int hitsToFastStun;

        public CustomEnemySpeed falseController;

        #region Speed Data
        
        private static readonly CustomEnemySpeed.AnimationData[] ANIMATION_DATAS = new[]
        {
            new CustomEnemySpeed.AnimationData(1.5f, "Idle"),
            new CustomEnemySpeed.AnimationData(2f, "Turn"),
            new CustomEnemySpeed.AnimationData(2f, "Jump Attack Up"),
            new CustomEnemySpeed.AnimationData(2f, "Jump Antic"),
            new CustomEnemySpeed.AnimationData(2f, "Land"),
            new CustomEnemySpeed.AnimationData(2f, "Jump"),
            new CustomEnemySpeed.AnimationData(2f, "Attack Antic"),
            new CustomEnemySpeed.AnimationData(2f, "Attack Recover"),
            new CustomEnemySpeed.AnimationData(2f, "Blank"),
            new CustomEnemySpeed.AnimationData(2f, "Run Antic"),
            new CustomEnemySpeed.AnimationData(2f, "Attack")
        };
        
        /*
        private static readonly CustomEnemySpeed.WaitData[] WAIT_DATAS = new[]
        {
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Turn"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Idle"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "HS Stop"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "HS Ret Left"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Hs Ret Right"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "HS Dissipate"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Shot Antic"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Shot"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Re Pos"),
            new CustomEnemySpeed.WaitData(2.0f, "FalseyControl", "Quake Waves"),
            new CustomEnemySpeed.WaitData(1.5f, "FalseyControl", "Fake Down")
        };*/


        

        private static readonly CustomEnemySpeed.SetVelocity2DData[] VELOCITY_DATAS = new[]
        {
            new CustomEnemySpeed.SetVelocity2DData(0.8f, "FalseyControl", "Jump"),
            new CustomEnemySpeed.SetVelocity2DData(0.8f, "FalseyControl", "JA Jump"),
            new CustomEnemySpeed.SetVelocity2DData(0.8f, "FalseyControl", "S Jump"),
            new CustomEnemySpeed.SetVelocity2DData(2.5f, "FalseyControl", "Run"),
        };

        #endregion

        public false_dream(GameObject falseGuy, int level)
        {
            this.falseGuy = falseGuy;

            realHPChecker = this.falseGuy.LocateMyFSM("Check Health");
            //realHPChecker.GetState("Init").Transitions = new FsmTransition[0];
            //realHPChecker.SetState("Init");
            foreach (FsmInt i in realHPChecker.FsmVariables.IntVariables)
            {
                log("found int " + i.Name + " with value " + i.Value);
            }
            
            calculateDifficultyFromLevel(level);

            log("hits to stun: " + hitsToStun + " hits to fast stun: " + hitsToFastStun + " health: " + maxHealth +
                " dance speed: " + danceSpeed);

            falseController = this.falseGuy.GetOrAddComponent<CustomEnemySpeed>();
            
            foreach (FsmInt i in realHPChecker.FsmVariables.IntVariables)
            {
                i.Value = maxHealth;
            }
            falseController.UpdateDanceSpeed(danceSpeed);

            foreach (CustomEnemySpeed.AnimationData a in ANIMATION_DATAS)
            {
                falseController.AddAnimationData(a);
            }

            foreach (CustomEnemySpeed.SetVelocity2DData v in VELOCITY_DATAS)
            {
                falseController.AddSetVelocity2DData(v);
            }
            
            falseController.StartSpeedMod();
        }

        public void restoreOrigValues()
        {
            falseController.RestoreOriginalSpeed();
            falseController.SetActive(false);
        }


        private void calculateDifficultyFromLevel(int level)
        {
            danceSpeed = Math.Pow((double) level, 0.25) + 0.2;
            double healthMod = Math.Pow((double) level, 0.8) + 0.2;
            
            log("health mod is " + healthMod + " because level is " + level);

            maxHealth = (int) (DEFAULT_HEALTH * healthMod);
        }
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }

    }
}