using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class MergePlatformAndCollectibles
{
    public GameObject platforms;
    public GameObject collectibles;

    public MergePlatformAndCollectibles(GameObject platforms, GameObject collectibles)
    {
        this.platforms = platforms;
        this.collectibles = collectibles;
    }

}
public class PlatformManager : MonoBehaviour
{
    public DragAndShoot player;
    public Camera mainCam;

    public GameObject[] platforms;
    public Collectibles[] collectibles;
    //public AnimationClip[] animClip;
    public float[] percentages;
    public float overlapDistance;

    public GameObject currentPlatform;
    public GameObject basePlatform;

    Vector3 screenPos;
    Vector3 nextPos;
    bool startGame;

    public List<MergePlatformAndCollectibles> merge = new List<MergePlatformAndCollectibles>();


    public void Start()
    {
        mainCam = FindObjectOfType<Camera>();
        platforms = GameObject.FindGameObjectsWithTag("Platforms");
        player = FindObjectOfType<DragAndShoot>();

        basePlatform = GameObject.FindGameObjectWithTag("CurrentPlatform");
        currentPlatform = basePlatform;

        //if()
        //{

        //}

        // int i = 0;
        for (int k = 0; k < collectibles.Length; k++)
        {
            collectibles[k].gameObject.transform.SetParent(platforms[k].transform.GetChild(0).gameObject.transform);
            collectibles[k].gameObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            //collectibles[k].gameObject.transform.localScale = 

            merge.Add(new MergePlatformAndCollectibles(platforms[k], collectibles[k].gameObject));

            //Debug.Log(merge[k].platforms.name + " and " + merge[k].collectibles.name);
        }

        foreach (GameObject plat in platforms)
        {
            plat.transform.position = GetRandomPosition1();
            RandomizePlatform(plat);
        }
        startGame = true;
    }

    public void Update()
    {
        if (startGame)
        {
            CheckIfVisible();
            startGame = false;
        }
        //CheckIfVisible();

    }
    int GetRandomPercentage()
    {
        float random = Random.Range(0f, 1f);
        float numOfAdd = 0;
        float total = 0;

        //calculate total percentage
        for(int i = 0; i < percentages.Length; i++)
        {
            total += percentages[i];
        }

        for(int i = 0; i < platforms.Length; i++)
        {
            if (percentages[i] / total + numOfAdd >= random)
            {
                return i;
            }
            else
            {
                numOfAdd += percentages[i] / total;
            }
        }
        return 0;
    }

    void RandomizePlatform(GameObject platform)
    {
        var anim = GetRandomPercentage();
        platform.GetComponent<Animator>().SetInteger("PlatformState", anim);
        platform.GetComponent<Animator>().SetTrigger(anim.ToString());
    }

    public void CheckIfVisible()
    {
        bool OnScreen;
        for (int i = 0; i < platforms.Length; i++)
        {
            //screenPos = Camera.main.WorldToScreenPoint(platforms[i].transform.position);
            //OnScreen = (screenPos.x > 0f & screenPos.x < Screen.width) & (screenPos.y > 0f & screenPos.y < Screen.height);

            screenPos = mainCam.WorldToViewportPoint(platforms[i].transform.position);
            OnScreen = screenPos.x >= 0.1f && screenPos.x <= 0.9f && screenPos.y > 0.1f/* && screenPos.y < 1*/;
            //OnScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0;

            if (!OnScreen && (currentPlatform.transform.position.y - 4f) > merge[i].platforms.transform.position.y)
            {
                merge[i].platforms.GetComponent<Animator>().speed = 1;
                RespawnPlatform(merge[i].platforms);
                if (!merge[i].collectibles.gameObject.activeInHierarchy)
                {
                    merge[i].collectibles.GetComponent<Collectibles>().randomSpawn();
                }
                //Debug.Log("Not On screen and under player platform: " + merge[i].platforms.name+"c: "+merge[i].collectibles);
            }
        }
    }

    public void RespawnPlatform(GameObject platform)
    {
        platform.transform.position = GetRandomPosition2();
        RandomizePlatform(platform);
    }

    Vector3 GetRandomPosition1()
    {
        for (int n = 0; n < 50; n++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(mainCam.WorldToViewportPoint(new Vector2(0, 0)).x +1.5f , mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x -1.5f), Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.5f)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.9f)).y), 0f);
            bool overlapping = true;
            for (int i = 0; i < merge.Count; i++)
            {
                if (Vector3.Distance(randomPosition, merge[i].platforms.transform.position) < overlapDistance)
                {
                    overlapping = false;
                    break;
                }
            }
            if (overlapping)
                return randomPosition;
        }

        return new Vector3(0f, -100f, 0f);

        //for (int n = 0; n < 50; n++)
        //{
        //    Vector3 randomPosition = new Vector3(Random.Range(mainCam.WorldToViewportPoint(new Vector2(0, 0)).x, mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x), Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.5f)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.9f)).y), 0f);
        //    bool overlapping = true;
        //    for (int i = 0; i < platforms.Length; i++)
        //    {
        //        if (Vector3.Distance(randomPosition, platforms[i].transform.position) < overlapDistance)
        //        {
        //            overlapping = false;
        //            break;
        //        }
        //    }
        //    if (overlapping)
        //        return randomPosition;
        //}

        //return new Vector3(0f, -100f, 0f);
    }

    Vector3 GetRandomPosition2()
    {
        for (int n = 0; n < 50; n++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, 0)).x +1.5f, mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x -1.5f), Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1f/*PlayerPrefs.GetFloat("platformmin")*/)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1.5f)).y), 0f);
            bool overlapping = true;
            for (int i = 0; i < merge.Count; i++)
            {
                if (Vector3.Distance(randomPosition, merge[i].platforms.transform.position) < overlapDistance)
                {
                    overlapping = false;
                    break;
                }
            }
            if (overlapping)
                return randomPosition;
        }
        return new Vector3(0f, -100f, 0f);

        //for (int n = 0; n < 50; n++)
        //{
        //    Vector3 randomPosition = new Vector3(Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, 0)).x, mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x), Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1.1f/*PlayerPrefs.GetFloat("platformmin")*/)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1.5f)).y), 0f);
        //    bool overlapping = true;
        //    for (int i = 0; i < platforms.Length; i++)
        //    {
        //        if (Vector3.Distance(randomPosition, platforms[i].transform.position) < overlapDistance)
        //        {
        //            overlapping = false;
        //            break;
        //        }
        //    }
        //    if (overlapping)
        //        return randomPosition;
        //}
        //return new Vector3(0f, -100f, 0f);
    }
}
