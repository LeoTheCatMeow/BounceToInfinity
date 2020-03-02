using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [Header("Spawn Parameters")]
    public float maxVerticalDifference;
    public float minVerticalDifference;
    public float maxHorizontalDifference;
    public float minHorizontalDifference;
    [Tooltip("Relative to the current height of camera")]
    public float spawnBufferDistance;
    [Tooltip("Distance below camera's min height before getting collected")]
    public float garbageCollectDistance;

    [Header("Platforms")]
    public Platforms platforms;
    public GameObject origin;

    [Header("Relics")]
    public List<GameObject> relics;
    public List<float> relicSpawnRequirements;
    private List<GameObject> spawnedRelics;
    private List<GameObject> collectedRelics;

    private delegate IEnumerator Generator();
    private List<(Generator, System.Func<bool>)> allGeneratorLogics;
    private Generator previousGenerator;
   
    private GameObject previousPlatform;
    private List<GameObject> platformPool;
    private System.Func<bool> readyToGenerate;
    private System.Func<float> basicPlatformPossibility;

    void Awake()
    {
        GameEvents._GameOver += OnGameOver;
        GameEvents._GameReset += OnGameReset;
        GameEvents._GameStart += OnGameStart;
        GameEvents._ReturnToMenu += OnReturnToMenu;
        GameEvents._RelicCollected += OnRelicCollected;
    }

    void OnDestroy()
    {
        GameEvents._GameOver -= OnGameOver;
        GameEvents._GameReset -= OnGameReset;
        GameEvents._GameStart -= OnGameStart;
        GameEvents._ReturnToMenu -= OnReturnToMenu;
        GameEvents._RelicCollected -= OnRelicCollected;
    }

    void Start()
    {
        platformPool = new List<GameObject>();
        spawnedRelics = new List<GameObject>();
        collectedRelics = new List<GameObject>();

        previousPlatform = origin;
        readyToGenerate = () => transform.position.y < CameraControl.instance.transform.position.y + spawnBufferDistance;
        basicPlatformPossibility = () => 0.95f - 0.4f * Mathf.Sqrt(CameraControl.instance.localBestScore) / (Mathf.Sqrt(CameraControl.instance.localBestScore) + 5f);

        allGeneratorLogics = new List<(Generator, System.Func<bool>)>
        {(GenerateOneByOneRandom, null), (GenerateSpiral, null), (GenerateBridge, BridgeRequirement)};
    }

    private void OnGameStart()
    {
        StartCoroutine(ChooseGenerator());
    }

    private IEnumerator ChooseGenerator()
    {
        //wait one extra frame in case game is transitioning to menu
        yield return null;

        while (true)
        {
            (Generator generator, System.Func<bool> requirement) generatorLogic = allGeneratorLogics[Random.Range(0, allGeneratorLogics.Count)];
            while (generatorLogic.requirement != null && !generatorLogic.requirement()) 
            { 
                generatorLogic = allGeneratorLogics[Random.Range(0, allGeneratorLogics.Count)]; 
            }

            Generator newGenerator = generatorLogic.generator;
            previousGenerator = newGenerator;
            DiactivateOutofViewPlatforms();
            DiactivateOutofViewRelics();

            yield return StartCoroutine(newGenerator());
        }
    }

    private IEnumerator GenerateOneByOneRandom()
    {
        int iterations = Random.Range(8, 16);
        for (int i = 0; i < iterations; i++)
        {
            //set generator position to last platform
            transform.position = previousPlatform.transform.position;

            //random vertical difference
            transform.position += new Vector3(0f, Random.Range(minVerticalDifference, maxVerticalDifference), 0f);

            //random rotation
            transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

            //create new platform
            GameObject newPlatform = FetchPlatform(new List<(GameObject prototype, float possibility, System.Func<bool> requirement)>
            {
                (platforms.PlatformBasic, basicPlatformPossibility(), null), 
                (platforms.PlatformTimed, 0.6f, () => PlatformTimedRequirement()),
                (platforms.PlatformShifting, 1f, () => PlatformShiftingRequirement())
            });

            //random horizontal difference
            newPlatform.transform.localPosition = new Vector3(0f, 0f, Random.Range(minHorizontalDifference, maxHorizontalDifference));

            //release platform
            ReleasePlatform(newPlatform);

            yield return new WaitUntil(readyToGenerate);
        }
    }

    private IEnumerator GenerateSpiral()
    {
        int iterations = Random.Range(15, 25);
        int direction = Random.Range(0, 2) * 2 - 1;
        float radius = Random.Range(2.5f, 3f);

        transform.position = previousPlatform.transform.position + previousPlatform.transform.right * direction * radius;
        //Debug.Log(transform.position);
        transform.rotation = Quaternion.LookRotation(previousPlatform.transform.position - transform.position, transform.up);

        for (int i = 0; i < iterations; i++)
        {
            transform.position += new Vector3(0f, minVerticalDifference, 0f);
            transform.eulerAngles += new Vector3(0f, direction * 20f, 0f);

            GameObject newPlatform = FetchPlatform(new List<(GameObject prototype, float possibility, System.Func<bool> requirement)>
            {
                (platforms.PlatformBasic, basicPlatformPossibility(), null),
                (platforms.PlatformTimed, 0.4f, () => PlatformTimedRequirement()),
                (platforms.PlatformShifting, 0.5f, () => PlatformShiftingRequirement()),
                (platforms.PlatformFallthrough, 1f, () => PlatformFallthroughRequirement(i, iterations))
            });

            newPlatform.transform.localRotation = Quaternion.identity;
            newPlatform.transform.localPosition = new Vector3(0f, 0f, radius);

            ReleasePlatform(newPlatform);

            yield return new WaitUntil(readyToGenerate);
        }
    }

    private IEnumerator GenerateBridge()
    {
        float length = Random.Range(3f, 4.5f);
        float rotation = Random.Range(0, 4) * 90f;

        transform.position = previousPlatform.transform.position;
        transform.eulerAngles = previousPlatform.transform.eulerAngles + new Vector3(0f, rotation, 0f);

        GameObject newBridge = FetchPlatform(new List<(GameObject prototype, float possibility, System.Func<bool> requirement)>
        {
            (platforms.PlatformBridge, 1f, null)
        });

        newBridge.transform.localPosition = new Vector3(0f, 0f, length / 2 + minHorizontalDifference);
        newBridge.transform.localRotation = Quaternion.identity;
        newBridge.transform.localScale = new Vector3(newBridge.transform.localScale.x, newBridge.transform.localScale.y, length);

        ReleasePlatform(newBridge);

        GameObject newPlatform = FetchPlatform(new List<(GameObject prototype, float possibility, System.Func<bool> requirement)>
        {
            (platforms.PlatformBasic, 1f, null)
        });

        newPlatform.transform.localPosition = new Vector3(0f, 0f, length + 2 * minHorizontalDifference);
        newPlatform.transform.localRotation = Quaternion.identity;

        ReleasePlatform(newPlatform);

        yield return null;
    }

    private bool PlatformTimedRequirement()
    {
        return CameraControl.instance.localBestScore > 2f;
    }

    private bool PlatformShiftingRequirement()
    {
        return previousPlatform.tag == platforms.PlatformBasic.tag && CameraControl.instance.localBestScore > 5f;
    }

    private bool PlatformFallthroughRequirement(int i, int iterations)
    {
        return previousPlatform.tag == platforms.PlatformBasic.tag && CameraControl.instance.localBestScore > 5f && i < iterations - 1;
    }

    private bool BridgeRequirement()
    {
        return (CameraControl.instance.localBestScore - CameraControl.instance.minHeight > 3f || CameraControl.instance.localBestScore > 10f) &&
               previousGenerator != GenerateBridge;
    }

    //possibility is multiplicative
    private GameObject FetchPlatform(List<(GameObject prototype, float possibility, System.Func<bool> requirement)> options)
    {
        GameObject newPlatformPrototype = null;
        while (newPlatformPrototype == null)
        {
            for(int i = 0; i < options.Count; i++)
            {
                if (Random.value < options[i].possibility && (options[i].requirement == null || options[i].requirement()))
                {
                    newPlatformPrototype = options[i].prototype;
                    break;
                }
            }
        }

        foreach (GameObject platform in platformPool)
        {
            if (!platform.activeSelf && platform.tag == newPlatformPrototype.tag)
            {
                platform.transform.SetParent(transform);
                platform.transform.position = Vector3.zero;
                platform.transform.rotation = Quaternion.identity;
                return platform;
            }
        }

        GameObject newPlatform = Instantiate(newPlatformPrototype, transform);
        newPlatform.SetActive(false);
        return newPlatform;
    }

    private void ReleasePlatform(GameObject newPlatform)
    {
        newPlatform.transform.SetParent(null);
        newPlatform.SetActive(true);
        platformPool.Add(newPlatform);
        previousPlatform = newPlatform;

        SpawnRelics(newPlatform);
    }

    private void SpawnRelics(GameObject newPlatform)
    {
        for (int i = 0; i < relicSpawnRequirements.Count; i++)
        {
            if (newPlatform.tag != platforms.PlatformBasic.tag || 
                transform.position.y + CameraControl.instance.bonusPoints < relicSpawnRequirements[i] || 
                relicSpawnRequirements[i] < CameraControl.instance.bonusPoints)
            {
                return;
            } else
            {
                if (i < relics.Count && !collectedRelics.Contains(relics[i]) && !spawnedRelics.Contains(relics[i]))
                {
                    relics[i].transform.position = newPlatform.transform.position + new Vector3(0f, 0.2f, 0f);
                    relics[i].SetActive(true);
                    spawnedRelics.Add(relics[i]);
                    return;
                }
            }
        }
    }

    private void OnRelicCollected(int i) 
    {
        collectedRelics.Add(relics[i]);
        spawnedRelics.Remove(relics[i]);

        if ((PlayerData.relicsCollected & (1 << i)) == 0) 
        {
            PlayerData.relicsCollected |= 1 << i;
        }
    }

    private void DiactivateOutofViewPlatforms()
    {
        foreach (GameObject platform in platformPool)
        {
            if (platform.transform.position.y < CameraControl.instance.minHeight - garbageCollectDistance)
            {
                platform.SetActive(false);
            }
        }

        if (origin.activeSelf && origin.transform.position.y < CameraControl.instance.minHeight - garbageCollectDistance)
        {
            origin.SetActive(false);
        }
    }

    private void DiactivateOutofViewRelics()
    {
        for (int i = spawnedRelics.Count - 1; i >= 0; i--)
        {
            if (spawnedRelics[i].transform.position.y < CameraControl.instance.minHeight - garbageCollectDistance)
            {
                spawnedRelics[i].SetActive(false);
                spawnedRelics.Remove(spawnedRelics[i]);
            }
        }
    }

    private void OnGameOver()
    {
        StopAllCoroutines();
    }

    private void OnGameReset()
    {
        foreach (GameObject platform in platformPool)
        {
            platform.SetActive(false);
        }

        origin.SetActive(true);
        previousPlatform = origin;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;    

        foreach (GameObject relic in spawnedRelics)
        {
            relic.SetActive(false);
        }
        spawnedRelics.Clear();
        collectedRelics.Clear();

        StartCoroutine(ChooseGenerator());
    }

    private void OnReturnToMenu()
    {
        StopAllCoroutines();
    }
}
