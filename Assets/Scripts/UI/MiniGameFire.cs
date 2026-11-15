
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameFire : MonoBehaviour
{
    public static MiniGameFire Instance { get; private set; }

    [Header("Mission Timer")]
    public LunarTimerUI lunarTimer;

    [Header("Panel")]
    public GameObject panel;
    public TextMeshProUGUI infoText;

    [Header("Fire")]
    public GameObject fireIconPrefab;
    public Transform fireParent;

    [Header("Extinguisher")]
    public Image IconFireExtinguisher;

    [Header("Extinguisher Animation")]
    public float extinguisherMoveAmount = 12f;
    public float extinguisherRotateAngle = 8f;

    private Vector3 extinguisherStartPos;
    private Quaternion extinguisherStartRotation;
    private bool extinguisherAnimating = false;

    [Header("Settings")]
    public float duration = 8f;
    public int firesToSpawn = 6;
    public float spawnInterval = 0.7f;

    private int firesLeft;
    private float timeLeft;

    private bool isPlaying = false;
    private bool isClosing = false;
    private bool resumeTimerAfterGame = false;
    private bool lastSuccess = false;

    private System.Action<bool> onComplete;
    private Coroutine spawnCoroutine;

    private List<GameObject> activeFires =
        new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLanguage;
            LocalizationManager.Instance.OnLanguageChanged += RefreshLanguage;
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLanguage;
    }

    public void StartFireGame(System.Action<bool> callback)
    {
        if (isPlaying || isClosing)
            return;

        if (panel == null ||
            fireParent == null ||
            fireIconPrefab == null)
        {
            Debug.LogError(
                "MiniGameFire: Brak referencji do panelu, " +
                "fireParent lub fireIconPrefab!"
            );
            return;
        }

        if (fireIconPrefab.GetComponent<Button>() == null)
        {
            Debug.LogError(
                "MiniGameFire: Prefab ognia musi mieć Button!"
            );
            return;
        }

        if (lunarTimer == null)
            lunarTimer = FindObjectOfType<LunarTimerUI>();

        resumeTimerAfterGame =
            lunarTimer != null && !lunarTimer.IsPaused;

        if (lunarTimer != null)
        {
            lunarTimer.PauseTimer();
            Debug.Log("MiniGameFire: Timer misji zatrzymany");
        }
        else
        {
            Debug.LogWarning(
                "MiniGameFire: Nie znaleziono LunarTimerUI!"
            );
        }

        onComplete = callback;
        firesLeft = Mathf.Max(1, firesToSpawn);
        timeLeft = Mathf.Max(0.1f, duration);

        isPlaying = true;
        isClosing = false;

        ClearFires();

        panel.SetActive(true);

        if (IconFireExtinguisher != null)
        {
            IconFireExtinguisher.enabled = true;

            extinguisherStartPos =
                IconFireExtinguisher.rectTransform.anchoredPosition;

            extinguisherStartRotation =
                IconFireExtinguisher.rectTransform.localRotation;

            extinguisherAnimating = true;
        }

        LocalizationManager loc = LocalizationManager.EnsureInstance();
        if (loc != null)
        {
            loc.OnLanguageChanged -= RefreshLanguage;
            loc.OnLanguageChanged += RefreshLanguage;
        }

        RefreshInfoText();

        spawnCoroutine = StartCoroutine(SpawnFires());
    }


    private IEnumerator SpawnFires()
    {
        for (int i = 0; i < firesToSpawn; i++)
        {
            if (!isPlaying)
                yield break;

            SpawnSingleFire(i);

            if (i < firesToSpawn - 1)
                yield return new WaitForSecondsRealtime(spawnInterval);
        }

        spawnCoroutine = null;
    }



    private void SpawnSingleFire(int index)
    {
        if (fireIconPrefab == null || fireParent == null)
            return;

        GameObject fire = Instantiate(
            fireIconPrefab,
            fireParent,
            false
        );

        fire.SetActive(true);

        RectTransform rt = fire.GetComponent<RectTransform>();
        RectTransform parentRect = fireParent as RectTransform;

        if (rt == null || parentRect == null)
        {
            Debug.LogError("Fire: Brak RectTransform!");
            Destroy(fire);
            return;
        }

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(70f, 90f);
        rt.localScale = Vector3.one;

        int columns = Mathf.CeilToInt(
            Mathf.Sqrt(Mathf.Max(1, firesToSpawn))
        );

        int rows = Mathf.CeilToInt(
            (float)firesToSpawn / columns
        );

        int col = index % columns;
        int row = index / columns;

        float marginX = rt.rect.width / 2f + 20f;
        float marginY = rt.rect.height / 2f + 20f;

        float availableWidth = Mathf.Max(
            0f,
            parentRect.rect.width - marginX * 2f
        );

        float availableHeight = Mathf.Max(
            0f,
            parentRect.rect.height - marginY * 2f
        );

        float x = -availableWidth / 2f +
            availableWidth * (col + 0.5f) / columns;

        float y = -availableHeight / 2f +
            availableHeight * (row + 0.5f) / rows;

        float jitterX = Mathf.Min(
            8f, availableWidth / columns * 0.1f
        );

        float jitterY = Mathf.Min(
            8f, availableHeight / rows * 0.1f
        );

        x += Random.Range(-jitterX, jitterX);
        y += Random.Range(-jitterY, jitterY);

        rt.anchoredPosition = new Vector2(x, y);

        activeFires.Add(fire);

        Button btn = fire.GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                OnFireClicked(fire, btn);
            });
        }
        else
        {
            Debug.LogError("Prefab ognia nie ma Button!");
        }

        Debug.Log(
            $"Fire {index + 1}: {rt.anchoredPosition}, " +
            $"obszar: {parentRect.rect.size}"
        );
    }


    private void OnFireClicked(GameObject fire, Button btn)
    {
        if (!isPlaying || fire == null)
            return;

        if (!activeFires.Contains(fire))
            return;

        btn.interactable = false;

        activeFires.Remove(fire);
        firesLeft--;

        StartCoroutine(ShrinkAndDestroy(fire));

        RefreshInfoText();

        if (firesLeft <= 0)
        {
            EndGame(true);
        }
    }

    private IEnumerator ShrinkAndDestroy(GameObject fire)
    {
        if (fire == null)
            yield break;

        RectTransform rt =
            fire.GetComponent<RectTransform>();

        if (rt == null)
        {
            Destroy(fire);
            yield break;
        }

        Vector3 startScale = rt.localScale;
        float elapsed = 0f;

        while (elapsed < 0.25f)
        {
            if (fire == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(elapsed / 0.25f);

            rt.localScale = Vector3.Lerp(
                startScale,
                Vector3.zero,
                progress
            );

            yield return null;
        }

        if (fire != null)
            Destroy(fire);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        timeLeft -= Time.unscaledDeltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(false);
            return;
        }

        RefreshInfoText();

        foreach (GameObject fire in activeFires)
        {
            if (fire == null)
                continue;

            float scale =
                1f + Mathf.Sin(Time.unscaledTime * 6f) * 0.08f;

            fire.transform.localScale =
                new Vector3(scale, scale, 1f);
        }

        if (extinguisherAnimating &&
            IconFireExtinguisher != null)
        {
            float offsetY =
                Mathf.Sin(Time.unscaledTime * 5f) *
                extinguisherMoveAmount;

            float angle =
                Mathf.Sin(Time.unscaledTime * 3f) *
                extinguisherRotateAngle;

            IconFireExtinguisher.rectTransform.anchoredPosition =
                extinguisherStartPos +
                new Vector3(0f, offsetY, 0f);

            IconFireExtinguisher.rectTransform.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void RefreshInfoText()
    {
        if (infoText == null)
            return;

        string format = LocalizationManager.GetText(
            "minigame_fire_status",
            "Ogniska: {0} | Czas: {1}s"
        );
        infoText.text = string.Format(
            format,
            firesLeft,
            timeLeft.ToString("F1")
        );
    }

    private void RefreshResultText(bool success)
    {
        if (infoText == null)
            return;

        infoText.text = success
            ? LocalizationManager.GetText("minigame_fire_success", "Pożar ugaszony!")
            : LocalizationManager.GetText("minigame_fire_fail", "Nie udało się ugasić pożaru!");
    }

    private void RefreshLanguage()
    {
        if (isPlaying)
            RefreshInfoText();
        else if (isClosing)
            RefreshResultText(lastSuccess);
    }

    private void EndGame(bool success)
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        isClosing = true;
        lastSuccess = success;
        extinguisherAnimating = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        ClearFires();

        RefreshResultText(success);

        StartCoroutine(CloseAfterDelay(success));
    }

    private IEnumerator CloseAfterDelay(bool success)
    {
        yield return new WaitForSecondsRealtime(1.2f);

        if (IconFireExtinguisher != null)
        {
            IconFireExtinguisher.enabled = false;

            IconFireExtinguisher.rectTransform.anchoredPosition =
                extinguisherStartPos;

            IconFireExtinguisher.rectTransform.localRotation =
                extinguisherStartRotation;
        }

        if (panel != null)
            panel.SetActive(false);

        if (resumeTimerAfterGame && lunarTimer != null)
        {
            lunarTimer.ResumeTimer();
            Debug.Log("MiniGameFire: Timer misji wznowiony");
        }

        resumeTimerAfterGame = false;

        System.Action<bool> callback = onComplete;
        onComplete = null;

        isClosing = false;

        callback?.Invoke(success);
    }

    private void ClearFires()
    {
        foreach (GameObject fire in activeFires)
        {
            if (fire != null)
                Destroy(fire);
        }

        activeFires.Clear();
    }

    private void OnDestroy()
    {
        if (resumeTimerAfterGame && lunarTimer != null)
            lunarTimer.ResumeTimer();

        if (Instance == this)
            Instance = null;
    }
}
