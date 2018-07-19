using System.Collections;
using ModCommon;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global Everything here is used implicitly if not explicitly

namespace dreams
{
    public class dream_manager : MonoBehaviour
    {
        private const string FALSE_DREAM = "Dream_01_False_Knight";
        private const string SOUL_DREAM = "Dream_02_Mage_Lord";
        private const string KIN_DREAM = "Dream_03_Infected_Knight";

        private soul_dream soulDream = null;
        private false_dream falseDream = null;
        private kin_dream kinDream = null;

        private const int FC_DREAMS_PER_LEVEL = 10;
        private const int KIN_DREAMS_PER_LEVEL = 25;

        private int damage;

        private int currentDream = 0;
        private int defeatedDreamReward = 0;
        
        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= dreamCheck;
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += dreamCheck;
        }

        private void playerDies()
        {
            if (currentDream != 0)
            {
                log("You died in the dream so you died in real life. Dream death is " + currentDream);
                if ((currentDream & 1) != 0)
                {
                    global_vars.gameData.falseDreamFails++;
                    if (global_vars.gameData.falseDreamFails > 2)
                    {
                        global_vars.gameData.falseDreamFails = 0;
                        if (global_vars.gameData.falseDreamLevel > 1)
                        {
                            global_vars.gameData.falseDreamLevel--;
                        }
                    }
                } else if ((currentDream & 2) != 0)
                {
                    global_vars.gameData.soulDreamFails++;
                    if (global_vars.gameData.soulDreamFails > 2)
                    {
                        global_vars.gameData.soulDreamFails = 0;
                        if (global_vars.gameData.soulDreamLevel > 1)
                        {
                            global_vars.gameData.soulDreamLevel--;
                        }
                    }
                } else if ((currentDream & 4) != 0)
                {
                    global_vars.gameData.kinDreamFails++;
                    if (global_vars.gameData.kinDreamFails > 2)
                    {
                        global_vars.gameData.kinDreamFails = 0;
                        if (global_vars.gameData.kinDreamLevel > 1)
                        {
                            global_vars.gameData.kinDreamLevel--;
                        }
                    }
                }

                currentDream = 0;
            }
        }


        //TODO: figure out if you even need this lol.
        
        private void Update()
        {
            if (currentDream == 0) return;

            if (HeroController.instance.playerData.health <= 0)
            {
                playerDies();
            }
            
            if ((currentDream & 1) != 0)
            {
                //falseUpdate();
            } else if ((currentDream & 2) != 0)
            {
                //GameObject.Find("Dream Mage Lord").GetComponent<CustomEnemySpeed>().ReloadStubbornFSMActions();
                //soulDream.setDanceSpeed();
                //soulUpdate();
            } else if ((currentDream & 4) != 0)
            {
                kinUpdate();
            }

        }

        private IEnumerator addDreamsDelay()
        {
            yield return new WaitForFinishedEnteringScene();
            yield return new WaitForSeconds(0.3f);
            
            PlayerData.instance.dreamOrbs += (defeatedDreamReward);
            PlayerData.instance.dreamOrbsSpent -= (defeatedDreamReward);
            EventRegister.SendEvent("DREAM ORB COLLECT");

            defeatedDreamReward = 0;
        }

        private void kinUpdate()
        {
            if (kinDream.kinController.cachedHealthManager.hp > 0) return;
            
            currentDream = 0;
            log("Good job on beating level " + global_vars.gameData.kinDreamLevel);
            defeatedDreamReward = global_vars.gameData.kinDreamLevel * KIN_DREAMS_PER_LEVEL;
            global_vars.gameData.kinDreamLevel++;
            global_vars.gameData.kinDreamFails = 0;
        }
        

        private void dreamCheck(Scene from, Scene to)
        {
            if (to.name == FALSE_DREAM)
            {
                StartCoroutine(loadFalseDream());
                return;
            }
            else if (to.name == SOUL_DREAM)
            {
                StartCoroutine(loadSoulDream());
                return;
            }
            else if (to.name == KIN_DREAM)
            {
                StartCoroutine(loadKinDream());
                return;
            }

            if (defeatedDreamReward != 0)
            {
                StartCoroutine(addDreamsDelay());
            }
            currentDream = 0;

        }


        private IEnumerator loadFalseDream()
        {
            GameObject falseKnight = GameObject.Find("False Knight Dream");
            while (falseKnight == null)
            {
                yield return null;
                falseKnight = GameObject.Find("False Knight Dream");
            }

            yield return null;
            falseKnight.PrintSceneHierarchyTree("falseknice.txt");
            falseDream = new false_dream(falseKnight, global_vars.gameData.falseDreamLevel);
            StartCoroutine(displayEnemyLevel(global_vars.gameData.falseDreamLevel, Color.green));
            
            currentDream = 1;
        }

        private IEnumerator loadSoulDream()
        {
            yield return new WaitForSeconds(15f);
            GameObject soulMage = GameObject.Find("Dream Mage Lord");
            while (soulMage == null)
            {
                yield return null;
                soulMage = GameObject.Find("Dream Mage Lord");
            }
            
            yield return null;
            
            soulDream = new soul_dream(soulMage, global_vars.gameData.soulDreamLevel);            
            StartCoroutine(displayEnemyLevel(global_vars.gameData.soulDreamLevel, Color.blue));
            currentDream = 2;
        }
        
        private IEnumerator loadKinDream()
        {
            GameObject lostKin = GameObject.Find("Lost Kin");
            while (lostKin == null)
            {
                yield return null;
                lostKin = GameObject.Find("Infected Knight Dream");
                if (lostKin == null)
                {
                    lostKin = GameObject.Find("Lost Kin");
                }
            }

            yield return null;
            kinDream = new kin_dream(lostKin, global_vars.gameData.kinDreamLevel);
            StartCoroutine(displayEnemyLevel(global_vars.gameData.kinDreamLevel, Color.red));
            currentDream = 4;
        }

        private IEnumerator displayEnemyLevel(int level, Color c)
        {
            yield return new WaitForSeconds(2f);
            CanvasUtil.CreateFonts();
            GameObject canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            GameObject go =
                CanvasUtil.CreateTextPanel(canvas, "", 130, TextAnchor.UpperCenter,
                    new CanvasUtil.RectData(
                        new Vector2(0, 0),
                        new Vector2(0, 0),
                        new Vector2(0, 0),
                        new Vector2(1.0f, 0.7f),
                        new Vector2(0.5f, 0.5f)));
            
            
            Text textObj = go.GetComponent<Text>();
            textObj.color = c;
            textObj.font = CanvasUtil.TrajanBold;
            textObj.text = "";
            textObj.fontSize = 170;
            textObj.text = "Level " + level;
            textObj.CrossFadeAlpha(1f, 1f, false);
            
            yield return new WaitForSeconds(3f);
            
            textObj.CrossFadeAlpha(0f, 3f, false);
            
            yield return new WaitForSeconds(3.5f);
            
            Destroy(go);
            Destroy(canvas);
        }

        public void falseKill()
        {
            falseDream.restoreOrigValues();
            currentDream = 0;
            log("Good job on beating level " + global_vars.gameData.falseDreamLevel);
            defeatedDreamReward = global_vars.gameData.kinDreamLevel * KIN_DREAMS_PER_LEVEL;
            global_vars.gameData.falseDreamLevel++;
            global_vars.gameData.falseDreamFails = 0;
        }
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}