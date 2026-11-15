
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameTiming : MonoBehaviour
{
    public static MiniGameTiming Instance { get; private set; }

    [Header("References")]
    public LunarTimerUI lunarTimer;
    public GameObject miniGamePanel;
    public Slider timingSlider;
    public TextMeshProUGUI resultText;

    [Header("Tool Animation")]
    public float toolRotateAngle = 18f;
    public float toolRotateSpeed = 1f;

    private float toolAnimTime = 0f;
    private bool toolAnimating = false;

    [Header("Icons")]
    public Image pumpIcon;
    public Sprite brokenPumpSprite;
    public Sprite fixedPumpSprite;
    public Image toolIcon;

    [Header("Settings")]
    public float speed = 1.2f;
    public float targetMin = 0.4f;
    public float targetMax = 0.6f;
    public float timeLimit = 8f;

    private bool isPlaying = false;
    private bool goingRight = true;
    private bool resumeTimerAfterGame = false;

    private float currentValue = 0f;
    private float timeLeft;

    private System.Action<bool> onComplete;

    private void Awake()
    {
        Instance = this;

        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    public void StartMiniGame(System.Action<bool> callback)
    {
        if (isPlaying)
            return;

        if (miniGamePanel == null || timingSlider == null)
        {
            Debug.LogError(
                "MiniGameTiming: Brak panelu lub suwaka!"
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
        }
        else
        {
            Debug.LogWarning(
                "MiniGameTiming: Nie znaleziono LunarTimerUI!"
            );
        }

        onComplete = callback;

        timeLeft = timeLimit;
        currentValue = 0f;
        goingRight = true;
        isPlaying = true;

        toolAnimTime = 0f;
        toolAnimating = false;

        timingSlider.minValue = 0f;
        timingSlider.maxValue = 1f;
        timingSlider.value = currentValue;

        miniGamePanel.SetActive(true);

        if (resultText != null)
            resultText.text =
                "Naciśnij SPACJĘ w zielonej strefie!";

        if (pumpIcon != null)
        {
            pumpIcon.sprite = brokenPumpSprite;
            pumpIcon.enabled = brokenPumpSprite != null;
        }

        if (toolIcon != null)
        {
            toolIcon.enabled = true;
            toolIcon.rectTransform.localRotation =
                Quaternion.identity;

            toolAnimating = true;
        }
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        timeLeft -= Time.unscaledDeltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            FinishMiniGame(true);
            return;
        }

        float delta = Time.unscaledDeltaTime * speed;

        if (goingRight)
            currentValue += delta;
        else
            currentValue -= delta;

        if (currentValue >= 1f)
        {
            currentValue = 1f;
            goingRight = false;
        }
        else if (currentValue <= 0f)
        {
            currentValue = 0f;
            goingRight = true;
        }

        timingSlider.value = currentValue;

        if (resultText != null)
            resultText.text =
                $"Naciśnij SPACJĘ! ({timeLeft:F1}s)";

        if (toolAnimating && toolIcon != null)
        {
            toolAnimTime +=
                Time.unscaledDeltaTime * toolRotateSpeed;

            float cycleDuration = 2.4f;

            float t = Mathf.PingPong(
                toolAnimTime,
                cycleDuration
            ) / cycleDuration;

            float angle;

            if (t < 0.25f)
            {
                angle = Mathf.Lerp(
                    0f,
                    -toolRotateAngle,
                    t / 0.25f
                );
            }
            else if (t < 0.5f)
            {
                angle = Mathf.Lerp(
                    -toolRotateAngle,
                    0f,
                    (t - 0.25f) / 0.25f
                );
            }
            else if (t < 0.75f)
            {
                angle = Mathf.Lerp(
                    0f,
                    toolRotateAngle,
                    (t - 0.5f) / 0.25f
                );
            }
            else
            {
                angle = Mathf.Lerp(
                    toolRotateAngle,
                    0f,
                    (t - 0.75f) / 0.25f
                );
            }

            toolIcon.rectTransform.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }

        if (WasSpacePressed())
            FinishMiniGame(false);
    }

    private bool WasSpacePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return UnityEngine.InputSystem.Keyboard.current != null &&
               UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    private void FinishMiniGame(bool timedOut)
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        toolAnimating = false;

        bool success =
            !timedOut &&
            currentValue >= targetMin &&
            currentValue <= targetMax;

        if (resultText != null)
            resultText.text =
                success ? "Sukces!" : "Nie udało się...";

        if (success && pumpIcon != null &&
            fixedPumpSprite != null)
        {
            pumpIcon.sprite = fixedPumpSprite;
        }

        StartCoroutine(CloseAfterDelay(success));
    }

    private IEnumerator CloseAfterDelay(bool success)
    {
        yield return new WaitForSecondsRealtime(1.2f);

        if (toolIcon != null)
        {
            toolIcon.enabled = false;
            toolIcon.rectTransform.localRotation =
                Quaternion.identity;
        }

        if (pumpIcon != null)
            pumpIcon.enabled = false;

        if (resumeTimerAfterGame && lunarTimer != null)
            lunarTimer.ResumeTimer();

        resumeTimerAfterGame = false;

        System.Action<bool> callback = onComplete;
        onComplete = null;

        callback?.Invoke(success);

        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }
}
