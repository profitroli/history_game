using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager5 : MonoBehaviour
{
    // Структура для хранения вопроса
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers = new string[4]; // 0:A, 1:B, 2:C, 3:D
        public int correctAnswerIndex; // 0:A, 1:B, 2:C, 3:D
    }

    // Списки и данные
    public List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private int playerScore = 0;
    private List<int> playerAnswers = new List<int>(); // Индекс выбранного ответа для каждого вопроса

    // Ссылки на UI элементы (перетащите из Hierarchy)
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject testPanel;
    public GameObject resultPanel;

    [Header("Test Panel Elements")]
    public TMP_Text questionText;
    public Toggle[] answerToggles; // Массив из 4 Toggle, настройте порядок A,B,C,D
    public TMP_Text progressText;
    public Button nextButton;

    [Header("Result Panel Elements")]
    public TMP_Text scoreText;
    public TMP_Text resultListText; // Элемент Text внутри Content ScrollView
    public ScrollRect resultScrollRect; // ДОБАВЬТЕ ЭТУ ССЫЛКУ
    public Button restartButton;

    void Start()
    {
        // Инициализация вопросов
        InitializeQuestions();

        // Настройка кнопок
        SetupButtonListeners();

        // Изначально показываем только стартовую панель
        ShowPanel(startPanel);
    }

    void SetupButtonListeners()
    {
        // Находим кнопку на стартовой панели
        Button startBtn = startPanel.transform.Find("Button_Start").GetComponent<Button>();
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(StartTest);
        }
        else
        {
            // Альтернативный способ найти кнопку
            startBtn = startPanel.GetComponentInChildren<Button>();
            if (startBtn != null)
                startBtn.onClick.AddListener(StartTest);
        }

        // Кнопка "Далее" на панели теста
        nextButton.onClick.AddListener(NextQuestion);

        // Кнопка "Повторить" на панели результатов
        restartButton.onClick.AddListener(RestartTest);
    }

    public void StartTest()
    {
        Debug.Log("Тест начат!");

        currentQuestionIndex = 0;
        playerScore = 0;
        playerAnswers.Clear();
        ShowPanel(testPanel);
        LoadQuestion(currentQuestionIndex);
    }

    void InitializeQuestions()
    {
        questions = new List<Question>
    {
        new Question
        {
            questionText = "1. Первая мировая война началась:",
            answers = new string[] { "а) 7 июля 1907 г.", "б) 1 августа 1914 г.", "в) 25 октября 1917 г.", "г) 1 сентября 1939 г." },
            correctAnswerIndex = 1 // б) 1 августа 1914 г.
        },
        new Question
        {
            questionText = "2. Итогом Февральской революции был (-о):",
            answers = new string[] { "а) переход власти только к Временному правительству", "б) переход всей власти к Советам рабочих и крестьян", "в) установление двоевластия", "г) сохранение монархии" },
            correctAnswerIndex = 2 // в) установление двоевластия
        },
        new Question
        {
            questionText = "3. Брестский мирный договор был заключен:",
            answers = new string[] { "а) 3 марта 1918 г.", "б) 21 марта 1918 г.", "в) 18 марта 1921 г.", "г) 21 марта 1921 г." },
            correctAnswerIndex = 0 // а) 3 марта 1918 г.
        },
        new Question
        {
            questionText = "4. Октябрьская революция началась:",
            answers = new string[] { "а) 23 февраля 1917 г.", "б) 15 октября 1917 г.", "в) 25 октября 1917 г.", "г) 25 ноября 1917 г." },
            correctAnswerIndex = 2 // в) 25 октября 1917 г.
        },
        new Question
        {
            questionText = "5. Облисполкомзап являлся:",
            answers = new string[] {
                "а) временным органом управления в период перехода к Советам",
                "б) первым высшим законодательным органом БССР (с 1917 г.)",
                "в) первым высшим исполнительным органом БССР (с 1917 г.)",
                "г) органом советской власти на Западном фронте"
            },
            correctAnswerIndex = 1 // б) первым высшим законодательным органом советской власти в Беларуси с ноября 1917 г.
        },
        new Question
        {
            questionText = "6. Первое провозглашение ССРБ состоялось:",
            answers = new string[] { "а) 1 января 1919 г.", "б) 2 февраля 1919 г.", "в) 31 июля 1920 г.", "г) 30 декабря 1922 г." },
            correctAnswerIndex = 0 // а) 1 января 1919 г.
        },
        new Question
        {
            questionText = "7. Первое правительство ССРБ возглавил:",
            answers = new string[] { "а) А. Мясников", "б) А. Червяков", "в) Д. Жилунович", "г) В. Игнатовский" },
            correctAnswerIndex = 2 // в) Д. Жилунович
        },
        new Question
        {
            questionText = "8. Коллективизация – это политика советских властей по созданию хозяйств:",
            answers = new string[] { "а) хуторских", "б) коллективных", "в) кооперативных", "г) фермерских" },
            correctAnswerIndex = 1 // б) коллективных
        },
        new Question
        {
            questionText = "9. Особенностью проведения довоенной индустриализации в БССР было развитие промышленности:",
            answers = new string[] { "а) легкой", "б) химической", "в) машиностроительной", "г) тяжелой" },
            correctAnswerIndex = 0 // а) легкой
        },
        new Question
        {
            questionText = "10. Первое укрупнение территории БССР произошло в:",
            answers = new string[] { "а) 1923 г.", "б) 1924 г.", "в) 1925 г.", "г) 1926 г." },
            correctAnswerIndex = 1 // б) 1924 г.
        }
    };

        Debug.Log($"Инициализировано {questions.Count} вопросов");
    }

    void LoadQuestion(int index)
    {
        // Сбросить выбор переключателей
        foreach (Toggle toggle in answerToggles)
        {
            toggle.isOn = false;
        }

        // Проверим, есть ли вопросы
        if (questions.Count == 0)
        {
            Debug.LogError("Нет вопросов для загрузки!");
            return;
        }

        // Загрузить данные вопроса
        Question q = questions[index];
        questionText.text = q.questionText;

        for (int i = 0; i < answerToggles.Length; i++)
        {
            if (i < q.answers.Length)
            {
                answerToggles[i].GetComponentInChildren<TMP_Text>().text = q.answers[i];
            }
        }

        // Обновить прогресс
        progressText.text = $"Вопрос {index + 1} из {questions.Count}";

        // Кнопка "Далее" неактивна, пока не выбран ответ
        nextButton.interactable = false;

        Debug.Log($"Загружен вопрос {index + 1}");
    }

    // Этот метод нужно назначить на событие ValueChanged каждого Toggle в инспекторе
    public void OnAnswerSelected(bool isOn)
    {
        // Проверяем, был ли какой-либо Toggle активирован (isOn == true)
        bool anySelected = false;
        foreach (Toggle toggle in answerToggles)
        {
            if (toggle.isOn)
            {
                anySelected = true;
                break;
            }
        }
        nextButton.interactable = anySelected;

        Debug.Log($"Ответ выбран: {anySelected}");
    }

    void NextQuestion()
    {
        // Сохранить ответ игрока
        int selectedIndex = -1;
        for (int i = 0; i < answerToggles.Length; i++)
        {
            if (answerToggles[i].isOn)
            {
                selectedIndex = i;
                break;
            }
        }

        if (selectedIndex != -1)
        {
            playerAnswers.Add(selectedIndex);

            // Проверить ответ
            if (selectedIndex == questions[currentQuestionIndex].correctAnswerIndex)
            {
                playerScore++;
                Debug.Log($"Правильный ответ! Счет: {playerScore}");
            }
            else
            {
                Debug.Log($"Неправильный ответ. Правильный: {questions[currentQuestionIndex].correctAnswerIndex}");
            }
        }
        else
        {
            Debug.Log("Ответ не выбран");
            playerAnswers.Add(-1); // Добавляем -1 как "нет ответа"
        }

        // Перейти к следующему вопросу или завершить тест
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion(currentQuestionIndex);
        }
        else
        {
            FinishTest();
        }
    }

    void FinishTest()
    {
        Debug.Log($"Тест завершен! Счет: {playerScore}/{questions.Count}");

        ShowPanel(resultPanel);

        // Вывести итоговый счет
        scoreText.text = $"Правильных ответов: {playerScore} из {questions.Count}";

        // Построить детализированный список
        StringBuilder resultListBuilder = new StringBuilder();

        for (int i = 0; i < questions.Count; i++)
        {
            Question q = questions[i];
            int playerAnswerIndex = (i < playerAnswers.Count) ? playerAnswers[i] : -1;
            string playerAnswerText = (playerAnswerIndex >= 0 && playerAnswerIndex < q.answers.Length) ?
                q.answers[playerAnswerIndex] : "Нет ответа";
            string correctAnswerText = q.answers[q.correctAnswerIndex];

            // Добавляем номер вопроса
            resultListBuilder.Append($"<size=22><b>Вопрос {i + 1}:</b></size>\n");
            resultListBuilder.Append($"<size=18>{q.questionText}</size>\n");

            // Проверяем ответ
            if (playerAnswerIndex == q.correctAnswerIndex)
            {
                resultListBuilder.Append($"<color=#27ae60><size=18>✓ Ваш ответ: {playerAnswerText}</size></color>\n");
            }
            else
            {
                resultListBuilder.Append($"<color=#e74c3c><size=18>✗ Ваш ответ: {playerAnswerText}</size></color>\n");
                resultListBuilder.Append($"<color=#27ae60><size=18>Правильный ответ: {correctAnswerText}</size></color>\n");
            }

            // Добавляем разделитель между вопросами (кроме последнего)
            if (i < questions.Count - 1)
            {
                resultListBuilder.Append("\n<color=#cccccc>────────────────────</color>\n\n");
            }
        }

        // Устанавливаем текст
        resultListText.text = resultListBuilder.ToString();

        // СБРОСИМ СКРОЛЛ В НАЧАЛО
        if (resultScrollRect != null)
        {
            resultScrollRect.verticalNormalizedPosition = 1f; // 1 = верх, 0 = низ
            Debug.Log("Скролл сброшен в начало");
        }

        // Принудительно обновляем layout
        Canvas.ForceUpdateCanvases();

        // Если используется Content Size Fitter, сбросим его
        ContentSizeFitter sizeFitter = resultListText.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(resultListText.rectTransform);
        }

        Debug.Log($"Текст результатов установлен. Длина: {resultListBuilder.Length} символов");
    }

    public void RestartTest()
    {
        Debug.Log("Перезапуск теста");
        ShowPanel(startPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        startPanel.SetActive(false);
        testPanel.SetActive(false);
        resultPanel.SetActive(false);

        panelToShow.SetActive(true);

        Debug.Log($"Показана панель: {panelToShow.name}");
    }
}