using System.Collections;
using System.Linq;
using HutongGames.PlayMaker;
using ModCommon;
using ModCommon.Util;
using Modding;
using UnityEngine;

namespace dreams
{
    public class dream_stun_manager : MonoBehaviour
    {
        public PlayMakerFSM stunStateChecker;
        public string[] validStunStates;
        public PlayMakerFSM stunFSM;
        public string stunStateName;
        public HealthManager enemyHM;

        public CustomEnemySpeed speed;

        public int hitsToStun;
        public int hitsToFastStun;
        public float fastStunTimeoutTime;

        private int lastHP;
        private int lastHitDamage;
        private bool didTakeDamage = false;
        private int stunCounter;
        private float timeSinceLastHit = 0f;
        private bool ruinedFastStun = false;
        

        private void Start()
        {
            log("Starting dream stun manager");
            lastHP = enemyHM.hp;
            stunCounter = 0;
            ModHooks.Instance.HitInstanceHook += playerHitTracker;
            
            log("Started dream stun manager without any errors... Yeah right");
        }

        private void OnDestroy()
        {
            ModHooks.Instance.HitInstanceHook -= playerHitTracker;
        }

        private IEnumerator autoStunRoutine()
        {
            
            if (validStunStates == null)
            {
                ruinedFastStun = false;
                timeSinceLastHit = 0f;
                stunFSM.SetState(stunStateName);
            }
            else
            {
                while (true)
                {
                    if (stunStateChecker == null)
                    {
                        yield break;
                    }
                    string s = stunStateChecker.ActiveStateName;
                    if (validStunStates.All(t => s != t)) yield return null;
                    
                    ruinedFastStun = false;
                    timeSinceLastHit = 0f;
                    stunFSM.SetState(stunStateName);
                    yield break;
                }

            }
            
        }

        private void Update()
        {
            if ((stunCounter >= hitsToFastStun && !ruinedFastStun) || stunCounter >= hitsToStun)
            {
                //stunFSM.SetState(stunStateName);
                stunCounter = 0;
                StartCoroutine(autoStunRoutine());
            }
            
            if (stunCounter > 0)
                timeSinceLastHit += Time.deltaTime;
            
            if (timeSinceLastHit > fastStunTimeoutTime)
            {
                ruinedFastStun = true;
            }
            
            if (lastHP <= enemyHM.hp) return;
            
            
            lastHitDamage = lastHP - enemyHM.hp;
            lastHP = enemyHM.hp;
            didTakeDamage = true;
        }

        private HitInstance playerHitTracker(Fsm owner, HitInstance hit)
        {
            //log("kin hp is " + enemyHM.hp);
            
            if (!didTakeDamage) return hit;

            timeSinceLastHit = 0;
            
            //log("Hit enemy and did " + lastHitDamage + " dmg with a " + hit.AttackType.ToString());
            
            didTakeDamage = false;
            
            if (hit.DamageDealt != lastHitDamage)
            {    
                return hit;
            }
            
            AttackTypes a = hit.AttackType;
            if (a == AttackTypes.Nail || a == AttackTypes.Spell || a == AttackTypes.SharpShadow)
                stunCounter++;
            
            return hit;
        }
        
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}