using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlitchManager : MonoBehaviour
{
    public static GlitchManager Instance;

    [Header("Glitch Spawn Settings")]
    public float minSpawnTime = 10f;          // Минимальное время между появлением помех
    public float maxSpawnTime = 30f;          // Максимальное время между появлением помех
    public float activeGlitchLimit = 3;       // Максимальное количество активных помех

    [Header("Glitch Prefab")]
    public GameObject glitchPrefab;           // Префаб помехи

    private List<GlitchEffect> activeGlitches = new List<GlitchEffect>(); // Список активных помех
    private Coroutine spawnCoroutine;         // Корутина спавна

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Находим префаб помехи, если он не назначен вручную
        if (glitchPrefab == null)
        {
            // Попробуем найти его в Resources
            glitchPrefab = Resources.Load<GameObject>("GlitchEffect");

            // Если не нашли, выводим предупреждение
            if (glitchPrefab == null)
            {
                Debug.LogError("GlitchEffect prefab не найден! Убедитесь, что префаб находится в папке Resources/");
            }
        }

        // Запускаем процесс спавна помех
        StartSpawning();
    }

    /// <summary>
    /// Запускает процесс спавна помех
    /// </summary>
    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Останавливает процесс спавна помех
    /// </summary>
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    /// <summary>
    /// Корутина спавна помех
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Ждем случайное время до следующего спавна
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Проверяем лимит активных помех
            if (activeGlitches.Count >= activeGlitchLimit)
            {
                continue; // Пропускаем спавн, если достигнут лимит
            }

            // Пытаемся создать новую помеху
            AttemptToSpawnGlitch();
        }
    }

    /// <summary>
    /// Пытается создать помеху на случайном ClickableObject
    /// </summary>
    private void AttemptToSpawnGlitch()
    {
        // Находим все доступные ClickableObject в сцене
        ClickableObject[] clickableObjects = FindObjectsOfType<ClickableObject>();

        if (clickableObjects.Length == 0)
        {
            Debug.LogWarning("Не найдено ни одного ClickableObject для спавна помехи");
            return;
        }

        // Фильтруем объекты, на которых уже есть активные помехи
        List<ClickableObject> availableObjects = new List<ClickableObject>();
        foreach (var obj in clickableObjects)
        {
            GlitchEffect existingGlitch = obj.GetComponentInChildren<GlitchEffect>();
            if (existingGlitch == null || !existingGlitch.IsActive())
            {
                availableObjects.Add(obj);
            }
        }

        if (availableObjects.Count == 0)
        {
            Debug.Log("Нет доступных объектов для спавна помехи");
            return;
        }

        // Выбираем случайный доступный объект
        ClickableObject targetObject = availableObjects[Random.Range(0, availableObjects.Count)];

        // Создаем помеху на выбранном объекте
        SpawnGlitchOnObject(targetObject);
    }

    /// <summary>
    /// Создает помеху на указанном объекте
    /// </summary>
    private void SpawnGlitchOnObject(ClickableObject targetObject)
    {
        if (glitchPrefab == null)
        {
            Debug.LogError("Glitch prefab не назначен!");
            return;
        }

        // Создаем экземпляр помехи
        GameObject glitchInstance = Instantiate(glitchPrefab, targetObject.transform);

        // Настраиваем RectTransform
        RectTransform glitchRect = glitchInstance.GetComponent<RectTransform>();
        if (glitchRect != null)
        {
            glitchRect.anchorMin = Vector2.zero;
            glitchRect.anchorMax = Vector2.one;
            glitchRect.offsetMin = Vector2.zero;
            glitchRect.offsetMax = Vector2.zero;
        }

        // Получаем компонент GlitchEffect
        GlitchEffect glitchEffect = glitchInstance.GetComponent<GlitchEffect>();
        if (glitchEffect == null)
        {
            glitchEffect = glitchInstance.AddComponent<GlitchEffect>();
        }

        // Инициализируем компонент
        glitchEffect.Initialize(targetObject);

        // Активируем помеху
        glitchEffect.ActivateGlitch();

        // Добавляем в список активных помех
        activeGlitches.Add(glitchEffect);
    }

    /// <summary>
    /// Удаляет все активные помехи
    /// </summary>
    public void ClearAllGlitches()
    {
        for (int i = activeGlitches.Count - 1; i >= 0; i--)
        {
            if (activeGlitches[i] != null)
            {
                activeGlitches[i].ForceResolve();
            }
        }
        activeGlitches.Clear();
    }

    private void Update()
    {
        // Проверяем завершенные помехи
        for (int i = activeGlitches.Count - 1; i >= 0; i--)
        {
            if (activeGlitches[i] == null || !activeGlitches[i].IsActive())
            {
                activeGlitches.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        ClearAllGlitches();
        StopSpawning();
    }
}
