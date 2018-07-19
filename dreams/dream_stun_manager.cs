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

        public CustomEnemySpeed speed;

        public int hitsToStun;
        public int hitsToFastStun;
        public float fastStunTimeoutTime;

        private int lastDamage;
        private int lastHitDamage;
        private bool didTakeDamage = false;
        private int stunCounter;
        private float timeSinceLastHit = 0f;
        private bool ruinedFastStun = false;
        

        private void Start()
        {
            log("Starting dream stun manager");
            lastDamage = speed.damageDone;
            stunCounter = 0;
            ModHooks.Instance.HitInstanceHook += playerHitTracker;
        }

        private void OnDestroy()
        {
            ModHooks.Instance.HitInstanceHook -= playerHitTracker;
        }

        private IEnumerator autoStunRoutine()
        {
            if (validStunStates == null)
            {
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
                    
                    stunFSM.SetState(stunStateName);
                    yield break;
                }

            }
            
        }

        private void Update()
        {
            if ((stunCounter >= hitsToFastStun && !ruinedFastStun) || stunCounter >= hitsToStun)
            {
                stunFSM.SetState(stunStateName);
                stunCounter = 0;
                //StartCoroutine(autoStunRoutine());
            }
            
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit > fastStunTimeoutTime)
            {
                ruinedFastStun = true;
            }
            
            if (lastDamage >= speed.damageDone) return;
            lastHitDamage = speed.damageDone - lastDamage;
            lastDamage = speed.damageDone;
            didTakeDamage = true;
        }

        private HitInstance playerHitTracker(Fsm owner, HitInstance hit)
        {
            if (!didTakeDamage) return hit;
            
            log("Hit enemy and did " + lastHitDamage + " dmg with a " + hit.AttackType.ToString());
            
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