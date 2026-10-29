using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class PerformanceMonitor : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private int pooledObjectCount = 300;
    [SerializeField] private float refreshInterval = 0.25f;

    private readonly List<Transform> pooledObjects = new List<Transform>();
    private readonly List<Vector3> basePositions = new List<Vector3>();

    private Texture2D runtimeTexture;
    private Sprite runtimeSprite;
    private bool stressTestEnabled;
    private float sampleTime;
    private int sampleFrames;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        CreateObjectPool();
    }

    private void Update()
    {
        sampleTime += Time.unscaledDeltaTime;
        sampleFrames++;

        if (stressTestEnabled)
        {
            float currentTime = Time.time;

            for (int i = 0; i < pooledObjects.Count; i++)
            {
                Vector3 position = basePositions[i];
                position.y += Mathf.Sin(currentTime * 2f + i * 0.3f) * 0.08f;
                pooledObjects[i].position = position;
            }
        }

        if (sampleTime >= refreshInterval)
        {
            float fps = sampleFrames / sampleTime;
            float frameMilliseconds = 1000f / Mathf.Max(fps, 0.01f);
            float memoryMB =
                Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);

            if (fpsText != null)
            {
                fpsText.text = $"FPS: {fps:0}";
            }

            if (statsText != null)
            {
                int activeObjects = stressTestEnabled ? pooledObjectCount : 0;

                statsText.text =
                    $"Frame: {frameMilliseconds:0.0} ms | " +
                    $"Memory: {memoryMB:0.0} MB | " +
                    $"Objects: {activeObjects}";
            }

            sampleTime = 0f;
            sampleFrames = 0;
        }
    }

    public void ToggleStressTest()
    {
        stressTestEnabled = !stressTestEnabled;

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledObjects[i].gameObject.SetActive(stressTestEnabled);
        }
    }

    private void CreateObjectPool()
    {
        runtimeTexture = new Texture2D(1, 1);
        runtimeTexture.SetPixel(0, 0, Color.white);
        runtimeTexture.Apply();

        runtimeSprite = Sprite.Create(
            runtimeTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        const int columns = 20;

        for (int i = 0; i < pooledObjectCount; i++)
        {
            GameObject item = new GameObject($"PooledSquare_{i}");
            item.transform.SetParent(transform);

            SpriteRenderer spriteRenderer =
                item.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = runtimeSprite;
            spriteRenderer.color = Color.HSVToRGB(
                i / (float)pooledObjectCount,
                0.7f,
                1f
            );

            int column = i % columns;
            int row = i / columns;

            Vector3 position = new Vector3(
                -7.6f + column * 0.8f,
                -3.8f + row * 0.55f,
                0f
            );

            item.transform.position = position;
            item.transform.localScale = Vector3.one * 0.3f;

            pooledObjects.Add(item.transform);
            basePositions.Add(position);

            item.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (runtimeSprite != null)
        {
            Destroy(runtimeSprite);
        }

        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }
    }
}