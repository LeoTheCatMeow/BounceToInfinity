using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public Collider targetCollider;
    [Tooltip("Movement upward every physics update")]
    public float upwardDelta;
    [Tooltip("Allow the target to move upward for certain distance before resume tracking")]
    public float additionalHeadSpace;
    [Tooltip("Increase min-height quickly if the target has moved too far above")]
    public float rubberbandThreshold;
    public Vector3 titleViewPosition;
    public Vector3 gameViewPosition;
    public float titleViewAngle;
    public float gameViewAngle;

    public static CameraControl instance;
    
    public Camera cam { get; private set; }
    public float minHeight { get; private set; }
    public float localBestScore { get; private set; }
    public float bonusPoints { get; private set; }

    private Transform focus;
    private bool trackPosition;
    private bool recordBroken;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        } else
        { 
            Destroy(gameObject);
        }

        GameEvents._GameReset += OnGameReset;
        GameEvents._GameStart += OnGameStart;
        GameEvents._ReturnToMenu += OnReturnToMenu;
    }

    void OnDestroy()
    {
        if (instance)
        {
            instance = null;
        }

        GameEvents._GameReset -= OnGameReset;
        GameEvents._GameReset -= OnGameStart;
        GameEvents._ReturnToMenu -= OnReturnToMenu;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        focus = transform.parent;
        trackPosition = true;
        minHeight = 0f;
        StartCoroutine(TitleScreenExhibition());
    }

    private IEnumerator TitleScreenExhibition()
    {
        while (true)
        {
            focus.eulerAngles += new Vector3(0f, 0.1f, 0f);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator TransitionToGameView()
    {
        for (int i = 0; i < 50; i++)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, gameViewPosition, 0.1f);
            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, gameViewAngle, 0.1f), 0f, 0f);
            focus.eulerAngles = new Vector3(0f, Mathf.LerpAngle(focus.eulerAngles.y, 0f, 0.1f), 0f);
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = gameViewPosition;
        transform.localEulerAngles = new Vector3(gameViewAngle, 0f, 0f);
    }

    public IEnumerator TransitionToTitleView()
    {
        for (int i = 0; i < 50; i++)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, titleViewPosition, 0.1f);
            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, titleViewAngle, 0.1f), 0f, 0f);
            focus.eulerAngles = new Vector3(0f, Mathf.LerpAngle(focus.eulerAngles.y, 0f, 0.1f), 0f);
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = titleViewPosition;
        transform.localEulerAngles = new Vector3(titleViewAngle, 0f, 0f);
        StartCoroutine(TitleScreenExhibition());
    }

    private void OnGameStart()
    {
        StopCoroutine(TitleScreenExhibition());
        SetBonusPoints();
        StartCoroutine(TransitionToGameView());
        StartCoroutine(TrackTarget());
        StartCoroutine(FollowTarget());
    }

    private IEnumerator TrackTarget()
    {
        GameEvents.UpdateScore(0f + bonusPoints);

        while (true)
        {
            focus.eulerAngles = new Vector3(0f, Mathf.LerpAngle(focus.eulerAngles.y, target.eulerAngles.y, 0.3f), 0f);

            Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            bool inView = GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, targetCollider.bounds);

            if (inView && !trackPosition)
            {
                trackPosition = true;
            }

            if (target.position.y > localBestScore)
            {
                localBestScore = target.position.y;
                GameEvents.UpdateScore(localBestScore + bonusPoints);

                if (localBestScore + bonusPoints > PlayerData.bestScore && !recordBroken)
                {   
                    GameEvents.DisplayPopUp("New Best Score!", 2.5f);
                    GetComponent<AudioSource>().Play();
                    recordBroken = true;
                }
            }

            if (!inView && trackPosition)
            {
                trackPosition = false;

                PlayerData.totalDistance += localBestScore;
                PlayerData.numberOfAttempts++;
                GameEvents.GameOver();

                if (localBestScore + bonusPoints > PlayerData.bestScore)
                {
                    PlayerData.bestScore = localBestScore + bonusPoints;
                }
            }
           
            yield return new WaitForFixedUpdate();
        }
    }
    
    private IEnumerator FollowTarget()
    {
        while (true)
        {
            if (PlayerData.relicActivated != 4)
            {
                minHeight = Mathf.Max(minHeight + upwardDelta * (1 + localBestScore / (localBestScore + 50f)), target.position.y - rubberbandThreshold);
            }
            
            if (trackPosition)
            {
                float focusY = Mathf.Max(target.position.y - additionalHeadSpace, minHeight);
                focus.position = new Vector3(target.position.x, focusY, target.position.z);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnGameReset()
    {
        minHeight = 0f;
        focus.position = Vector3.zero;
        focus.rotation = Quaternion.identity;

        localBestScore = 0f;
        recordBroken = false;
        GameEvents.UpdateScore(0f + bonusPoints);
    }

    private void OnReturnToMenu()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionToTitleView());
    }

    private void SetBonusPoints()
    {
        bonusPoints = PlayerData.relicActivated == 0 ? 20f : PlayerData.relicActivated == 2 ? 100f : 0f;
    }
}
