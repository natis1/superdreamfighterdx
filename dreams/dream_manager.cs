using System.Collections;
using ModCommon;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace dreams
{
    public class dream_manager : MonoBehaviour
    {
        private const string FALSE_DREAM = "Dream_01_False_Knight";
        private const string SOUL_DREAM = "Dream_02_Mage_Lord";
        private const string KIN_DREAM = "Dream_03_Infected_Knight";

        private soul_dream soulDream = null;
        private false_dream falseDream = null;

        private const int FC_DREAMS_PER_LEVEL = 10;

        private int damage;

        private int currentDream = 0;
        
        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= dreamCheck;
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += dreamCheck;

            ModHooks.Instance.BeforePlayerDeadHook += playerDies;
        }

        private void playerDies()
        {
            if (currentDream != 0)
            {
                log("You died in the dream so you died in real life. Dream death is " + currentDream);
                if ((currentDream & 1) != 0)
                {
                    global_vars.falseDreamFails++;
                    if (global_vars.falseDreamFails > 2)
                    {
                        global_vars.falseDreamFails = 0;
                        if (global_vars.falseDreamLevel > 1)
                        {
                            global_vars.falseDreamLevel--;
                        }
                    }
                } else if ((currentDream & 2) != 0)
                {
                    global_vars.soulDreamFails++;
                    if (global_vars.soulDreamFails > 2)
                    {
                        global_vars.soulDreamFails = 0;
                        if (global_vars.soulDreamLevel > 1)
                        {
                            global_vars.soulDreamLevel--;
                        }
                    }
                } else if ((currentDream & 4) != 0)
                {
                    global_vars.kinDreamFails++;
                    if (global_vars.kinDreamFails > 2)
                    {
                        global_vars.kinDreamFails = 0;
                        if (global_vars.kinDreamLevel > 1)
                        {
                            global_vars.kinDreamLevel--;
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

        private static IEnumerator addDreamsDelay(int multiplier, int level)
        {
            yield return new WaitForSeconds(2f);
            
            PlayerData.instance.dreamOrbs += (level * multiplier);
            PlayerData.instance.dreamOrbsSpent -= (level * multiplier);
            if (global_vars.falseDreamLevel > 0)
                EventRegister.SendEvent("DREAM ORB COLLECT");
        }

        private void kinUpdate()
        {
            
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
            falseDream = new false_dream(falseKnight, global_vars.falseDreamLevel);
            StartCoroutine(displayEnemyLevel(global_vars.falseDreamLevel, Color.green));
            
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
            
            soulDream = new soul_dream(soulMage, global_vars.soulDreamLevel);            
            StartCoroutine(displayEnemyLevel(global_vars.soulDreamLevel, Color.blue));
            currentDream = 2;
        }
        
        private IEnumerator loadKinDream()
        {
            GameObject lostKin = GameObject.Find("Infected Knight Dream");
            while (lostKin == null)
            {
                yield return null;
                lostKin = GameObject.Find("Infected Knight Dream");
            }

            yield return null;
            lostKin.PrintSceneHierarchyTree("lostkin.txt");

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
            log("Good job on beating level " + global_vars.falseDreamLevel);
            StartCoroutine(addDreamsDelay(global_vars.falseDreamLevel, FC_DREAMS_PER_LEVEL));
            global_vars.falseDreamLevel++;
            global_vars.falseDreamFails = 0;
        }
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}