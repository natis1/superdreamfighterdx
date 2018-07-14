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

        private int currentDream = 0;
        
        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= dreamCheck;
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += dreamCheck;
        }
        
        
        //TODO: figure out if you even need this lol.
        
        private void Update()
        {
            if (currentDream == 0) return;
            
            if ((currentDream & 1) != 0)
            {
                falseUpdate();
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

        private void falseUpdate()
        {
            
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
            falseDream = new false_dream(falseKnight, SuperDreamFighterDX.saveData.falseDreamLevel);
            StartCoroutine(displayEnemyLevel(SuperDreamFighterDX.saveData.soulDreamLevel, Color.green));
            
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
            
            soulDream = new soul_dream(soulMage, SuperDreamFighterDX.saveData.soulDreamLevel);            
            StartCoroutine(displayEnemyLevel(SuperDreamFighterDX.saveData.soulDreamLevel, Color.blue));
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
        
        private static void log(string str)
        {
            Modding.Logger.Log("[Dreamfighter] " + str);
        }
    }
}