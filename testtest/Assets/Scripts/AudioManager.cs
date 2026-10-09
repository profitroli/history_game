using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Музыка")]
    public AudioClip backgroundMusic;
    private AudioSource musicSource;

    [Header("Громкость")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Header("Настройки запуска")]
    public string mainMenuSceneName = "MainMenu"; // Название сцены главного меню

    private const string VOLUME_KEY = "MusicVolume";
    private bool musicStarted = false;

    void Awake()
    {
        // Создаем единственный экземпляр
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Проверяем текущую сцену
        CheckCurrentScene();
    }

    void SetupAudio()
    {
        // Создаем источник звука
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Загружаем сохраненную громкость
        musicVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.7f);
        musicSource.volume = musicVolume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем, нужно ли включать/выключать музыку при загрузке любой сцены
        UpdateMusicForCurrentScene();
    }

    void CheckCurrentScene()
    {
        // Проверяем текущую сцену при старте
        UpdateMusicForCurrentScene();
    }

    void UpdateMusicForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Проверяем, является ли текущая сцена лекцией (сами видео-лекции)
        bool isLectureVideoScene = IsLectureVideoScene(currentScene);

        if (isLectureVideoScene)
        {
            // Если это видео-лекция - останавливаем музыку
            StopBackgroundMusic();
        }
        else
        {
            // Если это не видео-лекция - запускаем музыку
            if (!musicStarted)
            {
                StartBackgroundMusic();
            }
            else if (musicStarted && !musicSource.isPlaying)
            {
                StartBackgroundMusic();
            }
        }
    }

    bool IsLectureVideoScene(string sceneName)
    {
        // Список сцен, которые являются видео-лекциями (без музыки)
        // Lecture 1-14 (включая Lecture 11-12)
        for (int i = 1; i <= 14; i++)
        {
            if (sceneName == $"Lecture {i}")
                return true;
        }

        // Специальный случай для Lecture 11-12
        if (sceneName == "Lecture 11-12")
            return true;

        return false;
    }

    void StartBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.Play();
            musicStarted = true;
            Debug.Log("Фоновая музыка включена");
        }
    }

    void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Фоновая музыка выключена (видео-лекция)");
        }
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;

        // Сохраняем
        PlayerPrefs.SetFloat(VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}