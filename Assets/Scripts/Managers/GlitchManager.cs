using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GlitchManager : MonoBehaviour
{
    public static GlitchManager Instance;

    [Header("Spawning Settings")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 15f;
    
    [Header("Difficulty")]
    public int minClicks = 3;
    public int maxClicks = 8;
    public float glitchDuration = 3f;

    private List<GlitchInstance> allGlitches = new List<GlitchInstance>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshGlitchTargets();

        StartCoroutine(SpawnGlitchRoutine());
    }

    public void RefreshGlitchTargets()
    {
        allGlitches = FindObjectsOfType<GlitchInstance>(true).ToList();
    }

    IEnumerator SpawnGlitchRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (GameManager.Instance.currentState == GameManager.GameState.Play)
            {
                TrySpawnGlitch();
            }
        }
    }

    void TrySpawnGlitch()
    {
        if (allGlitches.Count == 0) return;

        var available = allGlitches.Where(g => !g.IsActive).ToList();

        if (available.Count > 0)
        {
            GlitchInstance victim = available[Random.Range(0, available.Count)];
            
            int clicks = Random.Range(minClicks, maxClicks + 1);
            
            victim.ActivateGlitch(clicks, glitchDuration);
            
            Debug.Log($"Glitch spawned on {victim.name}. Need {clicks} clicks.");
        }
    }
}