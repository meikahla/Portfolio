using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MergePlatformAndCollectibles
{
    public GameObject platform;
    public GameObject collectible;

    public MergePlatformAndCollectibles(GameObject platform, GameObject collectible)
    {
        this.platform = platform;
        this.collectible = collectible;
    }
}

public class PlatformManager : MonoBehaviour
{
    public DragAndShoot player;
    public Camera mainCam;
    public GameObject[] platforms;
    public Collectibles[] collectibles;
    public float[] percentages;
    public float overlapDistance;

    public GameObject currentPlatform;
    public GameObject basePlatform;

    private bool startGame;
    private List<MergePlatformAndCollectibles> merge = new List<MergePlatformAndCollectibles>();

    private void Start()
    {
        mainCam = Camera.main;
        platforms = GameObject.FindGameObjectsWithTag("Platforms");
        player = FindObjectOfType<DragAndShoot>();
        basePlatform = GameObject.FindGameObjectWithTag("CurrentPlatform");
        currentPlatform = basePlatform;

        // Merge platforms with their respective collectibles
        for (int k = 0; k < collectibles.Length; k++)
        {
            collectibles[k].transform.SetParent(platforms[k].transform.GetChild(0));
            collectibles[k].transform.localPosition = new Vector3(0f, 1.5f, 0f);
            merge.Add(new MergePlatformAndCollectibles(platforms[k], collectibles[k].gameObject));
        }

        // Randomize initial platform positions
        foreach (GameObject plat in platforms)
        {
            plat.transform.position = GetRandomPosition1();
            RandomizePlatform(plat);
        }

        startGame = true;
    }

    private void Update()
    {
        if (startGame)
        {
            CheckIfVisible();
            startGame = false;
        }
    }

    private int GetRandomPercentage()
    {
        float random = Random.Range(0f, 1f);
        float total = 0;
        foreach (float percentage in percentages) total += percentage;

        float numOfAdd = 0;
        for (int i = 0; i < platforms.Length; i++)
        {
            if (percentages[i] / total + numOfAdd >= random)
                return i;
            numOfAdd += percentages[i] / total;
        }
        return 0;
    }

    private void RandomizePlatform(GameObject platform)
    {
        var anim = GetRandomPercentage();
        Animator animator = platform.GetComponent<Animator>();
        animator.SetInteger("PlatformState", anim);
        animator.SetTrigger(anim.ToString());
    }

    private void CheckIfVisible()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            Vector3 screenPos = mainCam.WorldToViewportPoint(platforms[i].transform.position);
            bool onScreen = screenPos.x >= 0.1f && screenPos.x <= 0.9f && screenPos.y > 0.1f;

            if (!onScreen && (currentPlatform.transform.position.y - 4f) > merge[i].platform.transform.position.y)
            {
                merge[i].platform.GetComponent<Animator>().speed = 1;
                RespawnPlatform(merge[i].platform);
                if (!merge[i].collectible.activeInHierarchy)
                    merge[i].collectible.GetComponent<Collectibles>().randomSpawn();
            }
        }
    }

    private void RespawnPlatform(GameObject platform)
    {
        platform.transform.position = GetRandomPosition2();
        RandomizePlatform(platform);
    }

    private Vector3 GetRandomPosition1()
    {
        for (int n = 0; n < 50; n++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(mainCam.WorldToViewportPoint(Vector2.zero).x + 1.5f, mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - 1.5f),
                Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.5f)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 0.9f)).y),
                0f
            );

            if (IsPositionValid(randomPosition))
                return randomPosition;
        }

        return new Vector3(0f, -100f, 0f);
    }

    private Vector3 GetRandomPosition2()
    {
        for (int n = 0; n < 50; n++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(mainCam.ScreenToWorldPoint(Vector2.zero).x + 1.5f, mainCam.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - 1.5f),
                Random.Range(mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1f)).y, mainCam.ScreenToWorldPoint(new Vector2(0, Screen.height * 1.5f)).y),
                0f
            );

            if (IsPositionValid(randomPosition))
                return randomPosition;
        }

        return new Vector3(0f, -100f, 0f);
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (var platform in merge)
        {
            if (Vector3.Distance(position, platform.platform.transform.position) < overlapDistance)
                return false;
        }
        return true;
    }
}
