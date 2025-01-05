using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragAndShoot : MonoBehaviour
{
    public static DragAndShoot ins;

    public PlatformManager platformManager;
    public GameManager gameManager;
    public GameUIManager gameUIManager;
    public BGManager bgManager;

    public Rigidbody2D hookObject;
    public Rigidbody2D playerObject;

    public GameObject collideBody;
    public GameObject currentPlatform;

    public GameObject hookHolder;
    public GameObject hookIndicator;

    public Material[] lineMaterial;

    public AudioSource myAudioSource;
    public AudioClip launchTongueSFX;
    public AudioClip pointsClip;

    public LineRenderer lr;

    Camera mainCamera;

    [HideInInspector]
    public bool hookLaunch = false;

    bool isPressed;
    public bool canShoot;

    Vector2 startPosition;

    float releaseDelay;
    public float maxDragDistance = 2f;
    public Vector2 platformPos;

    public bool canHook;
    public bool enableDrag;
    public float addScore;

    public Vector3[] tweenPoints;
    public bool dirLeft;
    public Tween t;
    Vector2 direction;


    void Awake()
    {

    }

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameUIManager = FindObjectOfType<GameUIManager>();
        bgManager = FindObjectOfType<BGManager>();
        mainCamera = GameObject.FindObjectOfType<Camera>();

        Physics2D.IgnoreCollision(playerObject.GetComponent<BoxCollider2D>(), hookObject.GetComponent<CircleCollider2D>());

        PlayerPrefs.SetInt("CurrentScore", 0);

        enableDrag = true;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        if(hookLaunch)
        {
            lr.SetPosition(0, playerObject.position);
            lr.SetPosition(1, hookObject.position);
        }
        else
        {
            lr.SetPosition(0, playerObject.position);
            lr.SetPosition(1, playerObject.position);
        }


        if (!IsPointerOverUIElement())
        {
            if(enableDrag)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    resetLine();
                    canHook = true;

                    startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    isPressed = true;
                    hookObject.isKinematic = true;
                    hookObject.GetComponent<CircleCollider2D>().enabled = false;
                }

                if(isPressed)
                {
                    if (Input.GetMouseButton(0))
                    {
                        DragRope();
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    hookIndicator.SetActive(false);
                    Utilities.PlaySFX(myAudioSource, launchTongueSFX, 1f);          //Play the "launch" sound.
                    hookObject.GetComponent<SpriteRenderer>().enabled = true;
                    hookHolder.GetComponent<SpriteRenderer>().enabled = false;

                    isPressed = false;
                    hookObject.isKinematic = false;
                    hookObject.GetComponent<CircleCollider2D>().enabled = true;

                    if (canShoot)
                    {
                        LaunchHook();
                        enableDrag = false;
                    }
                    else
                    {
                        resetLine();
                        hookObject.position = playerObject.position;
                    }
                }
            }

            if (hookLaunch)
            {
                if (!isPressed)
                {
                    lr.SetPosition(1, hookObject.position);
                }

            }
        }
    }

    public static bool IsPointerOverUIElement()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Where(r => r.gameObject.layer == 5).Count() > 0;
    }


    public void moveToPlatform()
    {
        PathType pathType = PathType.CatmullRom;

        Vector2 targetPos = new Vector2(collideBody.transform.parent.position.x - platformPos.x, collideBody.transform.parent.position.y + platformPos.y);

        //calculate score
        addScore = 1.5f * (targetPos.y - playerObject.position.y);

        CalculateScore();
        hookObject.isKinematic = true;

        float disX = targetPos.x - playerObject.position.x;

        //right
        if (disX > 1f)
        {            
            tweenPoints[0] = targetPos;
            tweenPoints[1] = targetPos;

            t = playerObject.transform.DOPath(tweenPoints, 0.4f, pathType, PathMode.Ignore, 10);
            t.SetEase(Ease.InSine)
                .OnComplete(movementComplete);
        }
        //left
        else if(disX < -1f)
        {

            tweenPoints[0] = targetPos;
            tweenPoints[1] = targetPos;

            t = playerObject.transform.DOPath(tweenPoints, 0.4f, pathType, PathMode.Ignore, 10);
            t.SetEase(Ease.InSine)
                .OnComplete(movementComplete);
        }
        else
        {
            tweenPoints[0] = targetPos;
            tweenPoints[1] = targetPos;

            t = playerObject.transform.DOPath(tweenPoints, 0.4f, pathType, PathMode.Ignore, 10);
            t.SetEase(Ease.Linear)
                .OnComplete(movementComplete);
        }
    }

    public void movementComplete()
    {
        t.Kill();
        platformManager.CheckIfVisible();
        hookLaunch = false;
        enableDrag = true;
        hookObject.position = playerObject.position;
        hookHolder.GetComponent<SpriteRenderer>().enabled = true;
        hookObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnDrawGizmos()
    {
    }

    public IEnumerator moveLine()
    {
        float elapsedTime = 0;
        bool isArrived = false;
        Vector2 targetPos = new Vector2(collideBody.transform.parent.position.x - platformPos.x, collideBody.transform.parent.position.y + platformPos.y);
        Vector2 targetHook = new Vector2(hookObject.transform.position.x, collideBody.transform.parent.position.y + platformPos.y);

        //calculate score
        addScore = 1.5f * (targetPos.y - playerObject.position.y);

        CalculateScore();
        hookObject.isKinematic = true;

        while (elapsedTime < GameManager.ins.hookReturnTime && !isArrived)
        {
            lr.SetPosition(0, playerObject.position);
            lr.SetPosition(1, hookObject.position);

            var firstPos = new Vector2(targetPos.x + 1f, playerObject.position.y);
            var second = new Vector2(targetPos.x - 1f, playerObject.position.y);

            elapsedTime += Time.deltaTime;

            playerObject.position = Vector2.Lerp(playerObject.position, targetPos, elapsedTime / GameManager.ins.hookReturnTime);

            if (Vector2.Distance(playerObject.position, targetPos) < 0.1f)
            {
                playerObject.position = targetPos;
                isArrived = true;
            }
            if (playerObject.position.y > hookObject.position.y)
            {
                StartCoroutine(returnHook());
            }
            yield return null;

        }
        enableDrag = true;
        collideBody = null;
        yield return null;
        platformManager.CheckIfVisible();

    }

    public IEnumerator returnHook()
    {
        enableDrag = true;
        collideBody = null;
        canHook = false;
        float elapsedTime = 0;
        bool isArrived = false;
        while (elapsedTime < GameManager.ins.hookReturnTime && !isArrived)
        {
            hookObject.position = Vector2.Lerp(hookObject.position, playerObject.position, elapsedTime / GameManager.ins.hookReturnTime);
            elapsedTime += Time.deltaTime;
            lr.SetPosition(1, hookObject.position);
            hookObject.GetComponent<CircleCollider2D>().enabled = false;

            if (Vector2.Distance(hookObject.position, playerObject.position) < 0.1f)
            {
                hookObject.position = playerObject.position;


                isArrived = true;
            }
            //yield return null;
        }
        resetLine();
        yield return null;
    }

    void CalculateScore()
    {
        int updatedScore;
        int currentScore = PlayerPrefs.GetInt("CurrentScore");
        string currentTheme = PlayerPrefs.GetString("Theme");
        int highScore = PlayerPrefs.GetInt("HS_" + currentTheme);
        int allhighScore = PlayerPrefs.GetInt("HighScore");

        PlayerPrefs.SetInt("CurrentScore", Mathf.RoundToInt(addScore) + currentScore);
        updatedScore = PlayerPrefs.GetInt("CurrentScore");

        if (updatedScore > highScore)
        {
            PlayerPrefs.SetInt("HS_" + currentTheme, highScore);
            PlayFabManager.Instance.UpdateCountryHighscore($"HS_{currentTheme}", highScore);
        }
        if(updatedScore > allhighScore)
        {
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayFabManager.Instance.UpdatePlayerHighScore("HighScore", highScore);
        }
        gameUIManager.StartCoroutine("updateScore");
    }

    void reachTop()
    {
        int currentScore = PlayerPrefs.GetInt("CurrentScore");

    }

    public void resetLine()
    {
        hookLaunch = false;
        hookHolder.GetComponent<SpriteRenderer>().enabled = true;
        hookObject.GetComponent<SpriteRenderer>().enabled = false;

        this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        enableDrag = true;
    }

    public IEnumerator retractLine()
    {
        canHook = false;
        float elapsedTime = 0;
        bool isArrived = false;

        while (elapsedTime < GameManager.ins.hookReturnTime && !isArrived)
        {
            hookObject.position = Vector2.Lerp(hookObject.position, playerObject.position, elapsedTime / GameManager.ins.hookReturnTime);
            elapsedTime += Time.deltaTime;

            if (Vector2.Distance(hookObject.position, playerObject.position) < 0.1f)
            {
                hookObject.position = playerObject.position;
                isArrived = true;
            }

            yield return null;
        }
        resetLine();
        hookObject.isKinematic = true;
        enableDrag = true;
        yield return null;

    }
    void LaunchHook()
    {
        hookObject.AddForce(direction * -(GameManager.ins.hookSpeed), ForceMode2D.Impulse);
        lr.material = lineMaterial[0];
        canShoot = false;
        hookLaunch = true;
        canHook = true;
    }

    public void pauseGame()
    {
        resetLine();
        PlayerPrefs.SetInt("isPaused", 1);

        TenjinManager.Ins.SendCustomEvent(false, "game_paused", 0);


    }

    public void resumeGame()
    {
        PlayerPrefs.SetInt("isPaused", 0);

        TenjinManager.Ins.SendCustomEvent(false, "game_resume", 0);
    }

    //indicator
    void DragRope()
    {
        hookObject.GetComponent<SpriteRenderer>().enabled = false;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(mousePosition, startPosition);

        Vector3 diff = startPosition - mousePosition;
        float angle = Mathf.Atan2(diff.y, diff.x);
        hookObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle);

        //lr.material = lineMaterial[1];
        hookIndicator.SetActive(true);
        hookIndicator.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle);
        var indicatorScale = hookIndicator.transform.localScale.x;

        //SetLineRendererPositions(mousePosition, distance);
        //Debug.Log(distance);

        if (distance > maxDragDistance)
        {
            direction = (mousePosition - startPosition).normalized;
            hookObject.position = playerObject.position/* + direction * -maxDragDistance*/;
            canShoot = true;
            if (indicatorScale < 1f)
            {
                indicatorScale += 0.1f;
                hookIndicator.transform.localScale = new Vector3(indicatorScale, 1f, 1f);
            }
        }
        else
        {
            direction = (mousePosition - startPosition).normalized;
            hookObject.position = playerObject.position/* + direction * -distance*/;
            canShoot = false;
            if (indicatorScale > 0f)
            {
                indicatorScale -= 0.1f;
                hookIndicator.transform.localScale = new Vector3(indicatorScale, 1f, 1f);
            }

        }
    }

    //updates the line renderer position to simulate rope stretching
    void SetLineRendererPositions(Vector2 mousePosition, float distance)
    {
        Vector3[] positions = new Vector3[2];

        positions[0] = playerObject.position;

        if (distance > maxDragDistance)
        {
            positions[positions.Length - 1] = playerObject.position + direction * -maxDragDistance;
        }
        else
        {
            positions[positions.Length - 1] = playerObject.position + direction * -distance;
        }

        lr.SetPositions(positions);
    }


}
