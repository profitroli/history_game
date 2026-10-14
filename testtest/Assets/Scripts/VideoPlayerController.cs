using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class VideoPlayerController : MonoBehaviour
{
    [Header("Настройки видео")]
    public VideoClip videoClip;
    public RawImage videoDisplay;
    public Button playButton;

    [Header("Кнопка остановки")]
    public Button stopButton;
    public bool showStopButtonOnVideo = true;

    [Header("Слайдер прогресса")]
    public Slider progressSlider;
    public Text timeText;
    public bool allowSeeking = true;

    [Header("Сохранение прогресса")]
    [Tooltip("Автоматически сохранять прогресс при выходе со страницы")]
    public bool autoSaveProgress = true;
    [Tooltip("Сохранять прогресс только при нажатии паузы")]
    public bool saveOnlyOnPause = true;

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private RenderTexture renderTexture;
    private bool isPaused = false;
    private bool isDraggingSlider = false;
    private double savedTimeWhenPaused = 0;

    // Ключ для сохранения прогресса
    private string saveKey;

    // Флаги для отслеживания состояния
    private bool wasPausedOnExit = false;
    private bool hasManuallyPaused = false;
    private bool isInitialized = false;
    private bool shouldRestoreFromSave = false;

    void Start()
    {
        InitializeVideoController();
    }

    void InitializeVideoController()
    {
        // Генерируем уникальный ключ для сохранения
        saveKey = $"VideoProgress_{SceneManager.GetActiveScene().name}_{gameObject.name}_{videoClip?.name}";

        SetupVideoPlayer();
        SetupButtons();
        SetupProgressSlider();

        videoDisplay.gameObject.SetActive(false);

        if (stopButton != null)
            stopButton.gameObject.SetActive(false);

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);

        // Загружаем сохраненный прогресс
        LoadVideoProgress();

        isInitialized = true;
    }

    void SetupVideoPlayer()
    {
        renderTexture = new RenderTexture(1920, 1080, 24);
        videoDisplay.texture = renderTexture;

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        if (videoClip != null)
            videoPlayer.clip = videoClip;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audioSource = gameObject.AddComponent<AudioSource>();
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    // Загружаем сохраненный прогресс видео
    void LoadVideoProgress()
    {
        if (!autoSaveProgress) return;

        // Проверяем, есть ли сохраненные данные для этого видео
        if (PlayerPrefs.HasKey(saveKey))
        {
            savedTimeWhenPaused = PlayerPrefs.GetFloat(saveKey, 0f);
            wasPausedOnExit = PlayerPrefs.GetInt($"{saveKey}_paused", 0) == 1;
            hasManuallyPaused = PlayerPrefs.GetInt($"{saveKey}_manualPause", 0) == 1;

            Debug.Log($"Загружено сохраненное время: {savedTimeWhenPaused:F2}с, " +
                     $"было на паузе: {wasPausedOnExit}, ручная пауза: {hasManuallyPaused}");

            // Проверяем, нужно ли восстанавливать с сохраненной позиции
            if (saveOnlyOnPause)
            {
                // Восстанавливаем только если было на паузе И пользователь сам нажал паузу
                shouldRestoreFromSave = wasPausedOnExit && hasManuallyPaused && savedTimeWhenPaused > 0;
            }
            else
            {
                // Восстанавливаем всегда если было сохранено
                shouldRestoreFromSave = wasPausedOnExit && savedTimeWhenPaused > 0;
            }

            if (shouldRestoreFromSave)
            {
                PrepareVideoFromSavedTime();
            }
        }
        else
        {
            Debug.Log("Сохраненный прогресс не найден, начнем с начала");
        }
    }

    void PrepareVideoFromSavedTime()
    {
        if (videoClip == null) return;

        videoPlayer.clip = videoClip;

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPreparedWithSavedTime;
        }
        else
        {
            SetupUIForPausedVideo();
        }
    }

    void OnVideoPreparedWithSavedTime(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OnVideoPreparedWithSavedTime;
        SetupUIForPausedVideo();
    }

    void SetupUIForPausedVideo()
    {
        if (videoPlayer.length <= 0) return;

        videoDisplay.gameObject.SetActive(true);

        // Устанавливаем сохраненное время
        videoPlayer.time = Math.Min(savedTimeWhenPaused, videoPlayer.length - 0.1);
        videoPlayer.Pause();
        isPaused = true;

        if (playButton != null)
            playButton.gameObject.SetActive(false);

        if (stopButton != null)
        {
            stopButton.gameObject.SetActive(true);
            UpdateStopButtonText();
        }

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            progressSlider.value = (float)(savedTimeWhenPaused / videoPlayer.length);
        }

        if (timeText != null)
        {
            UpdateTimeText((float)savedTimeWhenPaused, (float)videoPlayer.length);
        }

        Debug.Log($"Видео восстановлено с позиции: {savedTimeWhenPaused:F2} секунд");
    }

    void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayVideo);
        }

        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(TogglePause);
            UpdateStopButtonText();
        }
    }

    void UpdateStopButtonText()
    {
        if (stopButton == null) return;

        Text stopButtonText = stopButton.GetComponentInChildren<Text>();
        if (stopButtonText != null)
        {
            stopButtonText.text = isPaused ? "▶ Продолжить" : "⏸ Пауза";
        }
    }

    void SetupProgressSlider()
    {
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = 1;
            progressSlider.value = 0;

            AddSliderEventTriggers();
            progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (timeText != null)
            timeText.text = "00:00 / --:--";
    }

    void AddSliderEventTriggers()
    {
        if (progressSlider == null) return;

        EventTrigger trigger = progressSlider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = progressSlider.gameObject.AddComponent<EventTrigger>();
        }

        // Событие начала перетаскивания
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnSliderPointerDown(); });
        trigger.triggers.Add(pointerDownEntry);

        // Событие окончания перетаскивания
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnSliderPointerUp(); });
        trigger.triggers.Add(pointerUpEntry);
    }

    // Сохраняем прогресс видео
    void SaveVideoProgress()
    {
        if (!autoSaveProgress || !isInitialized) return;

        // Определяем, нужно ли сохранять
        bool shouldSave = false;

        if (saveOnlyOnPause)
        {
            // Сохраняем только если видео было на паузе И пользователь сам нажал паузу
            shouldSave = isPaused && hasManuallyPaused;
        }
        else
        {
            // Сохраняем если видео на паузе (неважно как)
            shouldSave = isPaused;
        }

        if (shouldSave && videoPlayer != null && videoPlayer.length > 0)
        {
            float timeToSave = (float)(isPaused ? savedTimeWhenPaused : videoPlayer.time);

            // Не сохраняем если видео почти закончилось (последние 2 секунды)
            if (timeToSave >= videoPlayer.length - 2.0)
            {
                ClearSavedProgress();
                return;
            }

            PlayerPrefs.SetFloat(saveKey, timeToSave);
            PlayerPrefs.SetInt($"{saveKey}_paused", isPaused ? 1 : 0);
            PlayerPrefs.SetInt($"{saveKey}_manualPause", hasManuallyPaused ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log($"Прогресс сохранен: {timeToSave:F2}с, пауза: {isPaused}, ручная: {hasManuallyPaused}");
        }
        else if (!isPaused)
        {
            // Если видео не на паузе, очищаем сохранение
            ClearSavedProgress();
        }
    }

    void ClearSavedProgress()
    {
        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.DeleteKey($"{saveKey}_paused");
        PlayerPrefs.DeleteKey($"{saveKey}_manualPause");
        PlayerPrefs.Save();
        Debug.Log("Сохраненный прогресс очищен");
    }

    // Методы для слайдера
    public void OnSliderPointerDown()
    {
        if (allowSeeking && videoPlayer != null && videoPlayer.length > 0)
        {
            isDraggingSlider = true;
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (allowSeeking && videoPlayer != null && videoPlayer.length > 0)
        {
            double targetTime = value * videoPlayer.length;

            if (isPaused)
            {
                UpdateTimeText((float)savedTimeWhenPaused, (float)videoPlayer.length);
            }
            else
            {
                UpdateTimeText((float)targetTime, (float)videoPlayer.length);
            }

            if (isDraggingSlider)
            {
                bool wasPlaying = videoPlayer.isPlaying && !isPaused;
                videoPlayer.time = targetTime;

                if (isPaused)
                {
                    savedTimeWhenPaused = targetTime;
                }

                if (!wasPlaying)
                {
                    videoPlayer.Pause();
                }
            }
        }
    }

    public void OnSliderPointerUp()
    {
        if (allowSeeking && videoPlayer != null && videoPlayer.length > 0)
        {
            isDraggingSlider = false;

            if (isPaused && videoPlayer.isPrepared)
            {
                videoPlayer.Pause();
                savedTimeWhenPaused = videoPlayer.time;
                UpdateTimeText((float)savedTimeWhenPaused, (float)videoPlayer.length);
            }
        }
    }

    public void SeekToTime(float normalizedTime)
    {
        if (videoPlayer != null && videoPlayer.length > 0)
        {
            double targetTime = normalizedTime * videoPlayer.length;
            videoPlayer.time = targetTime;

            if (isPaused)
            {
                savedTimeWhenPaused = targetTime;
            }

            if (progressSlider != null)
                progressSlider.value = normalizedTime;
        }
    }

    // Основные методы управления видео
    public void PlayVideo()
    {
        if (videoPlayer == null || videoClip == null)
        {
            Debug.LogError("Видео или VideoPlayer не настроены!");
            return;
        }

        if (isPaused)
        {
            ResumeVideo();
            return;
        }

        StartVideoPlayback();
    }

    void StartVideoPlayback()
    {
        // Если есть сохранение и мы его не использовали, спрашиваем пользователя
        if (shouldRestoreFromSave && !hasManuallyPaused)
        {
            // Здесь можно показать диалоговое окно с вопросом
            // "Продолжить с момента паузы или начать сначала?"
            // Для простоты автоматически продолжаем
            ContinueFromSavedPosition();
            return;
        }

        // Иначе начинаем с начала
        ActuallyStartPlayback(0);
    }

    void ContinueFromSavedPosition()
    {
        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPreparedForContinue;
        }
        else
        {
            ActuallyStartPlayback(savedTimeWhenPaused);
        }

        shouldRestoreFromSave = false;
    }

    void OnVideoPreparedForContinue(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OnVideoPreparedForContinue;
        ActuallyStartPlayback(savedTimeWhenPaused);
    }

    void ActuallyStartPlayback(double startTime)
    {
        videoDisplay.gameObject.SetActive(true);

        if (playButton != null)
            playButton.gameObject.SetActive(false);

        if (stopButton != null)
        {
            stopButton.gameObject.SetActive(true);
            UpdateStopButtonText();
        }

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(true);

        // Устанавливаем начальное время
        videoPlayer.time = startTime;

        // Обновляем UI
        if (videoPlayer.length > 0)
        {
            UpdateTimeText((float)startTime, (float)videoPlayer.length);
            if (progressSlider != null)
                progressSlider.value = (float)(startTime / videoPlayer.length);
        }

        videoPlayer.Play();
        isPaused = false;
        hasManuallyPaused = false;

        Debug.Log($"Воспроизведение видео с позиции: {startTime:F2} секунд");
    }

    public void TogglePause()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying && !isPaused)
        {
            PauseVideo();
        }
        else if (isPaused)
        {
            ResumeVideo();
        }
    }

    void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            isPaused = true;
            hasManuallyPaused = true; // Отметим, что пауза была ручной
            savedTimeWhenPaused = videoPlayer.time;

            UpdateStopButtonText();

            Debug.Log("Видео приостановлено. Сохраненное время: " + savedTimeWhenPaused);

            // Автосохранение при паузе
            SaveVideoProgress();
        }
    }

    void ResumeVideo()
    {
        if (isPaused)
        {
            videoPlayer.time = savedTimeWhenPaused;
        }

        videoPlayer.Play();
        isPaused = false;

        UpdateStopButtonText();

        Debug.Log("Воспроизведение видео продолжено с времени: " + savedTimeWhenPaused);
    }

    public void StopVideo()
    {
        if (videoPlayer == null) return;

        videoPlayer.Stop();
        isPaused = false;
        hasManuallyPaused = false;
        savedTimeWhenPaused = 0;
        shouldRestoreFromSave = false;

        // Очищаем сохранение при остановке
        ClearSavedProgress();

        videoDisplay.gameObject.SetActive(false);

        if (playButton != null)
            playButton.gameObject.SetActive(true);

        if (stopButton != null)
            stopButton.gameObject.SetActive(false);

        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            progressSlider.value = 0;
        }

        if (timeText != null)
            timeText.text = "00:00 / --:--";

        Debug.Log("Воспроизведение видео остановлено и прогресс сброшен");
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        ProgressService.Instance?.ReportLectureViewed(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        videoDisplay.gameObject.SetActive(false);

        if (playButton != null)
            playButton.gameObject.SetActive(true);

        if (stopButton != null)
            stopButton.gameObject.SetActive(false);

        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);

        if (progressSlider != null)
            progressSlider.value = 1f;

        if (timeText != null && videoPlayer != null)
        {
            UpdateTimeText((float)videoPlayer.length, (float)videoPlayer.length);
        }

        isPaused = false;
        hasManuallyPaused = false;
        savedTimeWhenPaused = 0;
        shouldRestoreFromSave = false;

        // Очищаем сохранение при завершении видео
        ClearSavedProgress();

        Debug.Log("Видео завершено, прогресс сброшен");
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Видео подготовлено. Длина: " + vp.length + " секунд");

        if (timeText != null && !videoPlayer.isPlaying)
        {
            timeText.text = "00:00 / " + FormatTime((float)videoPlayer.length);
        }
    }

    void Update()
    {
        // Управление с клавиатуры
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopVideo();
        }

        // Обновляем слайдер и время только если не перетаскиваем его
        if (!isDraggingSlider && videoPlayer != null && videoPlayer.isPrepared)
        {
            UpdateProgressSlider();
        }
    }

    void UpdateProgressSlider()
    {
        if (videoPlayer == null || progressSlider == null || videoPlayer.length <= 0) return;

        if (videoPlayer.isPlaying && videoPlayer.frameCount > 0)
        {
            float progress = (float)(videoPlayer.time / videoPlayer.length);
            progressSlider.value = progress;

            if (timeText != null)
            {
                UpdateTimeText((float)videoPlayer.time, (float)videoPlayer.length);
            }
        }
        else if (isPaused)
        {
            float progress = (float)(savedTimeWhenPaused / videoPlayer.length);
            progressSlider.value = progress;

            if (timeText != null)
            {
                UpdateTimeText((float)savedTimeWhenPaused, (float)videoPlayer.length);
            }
        }
    }

    void UpdateTimeText(float currentTime, float totalTime)
    {
        if (timeText != null)
        {
            string currentTimeStr = FormatTime(currentTime);
            string totalTimeStr = FormatTime(totalTime);
            timeText.text = $"{currentTimeStr} / {totalTimeStr}";
        }
    }

    string FormatTime(float timeInSeconds)
    {
        if (float.IsNaN(timeInSeconds) || timeInSeconds < 0)
        {
            return "--:--";
        }

        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        return $"{minutes:00}:{seconds:00}";
    }

    // Методы для сохранения при выходе
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveVideoProgress();
        }
    }

    void OnApplicationQuit()
    {
        SaveVideoProgress();
    }

    void OnDisable()
    {
        SaveVideoProgress();
    }

    void OnDestroy()
    {
        SaveVideoProgress();

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (progressSlider != null)
        {
            progressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    // Публичные методы для ручного управления сохранением
    public void ForceSaveProgress()
    {
        SaveVideoProgress();
    }

    public void ClearProgress()
    {
        ClearSavedProgress();
    }

    public void ExitPage()
    {
        SaveVideoProgress();
        // Здесь добавьте ваш код перехода на другую страницу
        Debug.Log("Выход со страницы видео, прогресс сохранен");
    }
}