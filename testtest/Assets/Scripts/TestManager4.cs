using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager4 : MonoBehaviour
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
            questionText = "1. В 1817 г. студентами Виленского университета было создано:",
            answers = new string[] { "а) общество филаретов", "б) общество «Военных друзей»", "в) общество филоматов", "г) «Демократическое общество»" },
            correctAnswerIndex = 2 // в) общество филоматов
        },
        new Question
        {
            questionText = "2. В 1831 г. для разработки и осуществления мероприятий по унификации западных губерний с внутренними губерниями России был создан:",
            answers = new string[] { "а) институт земств", "б) Комитет западных губерний", "в) Совет рабочих, крестьянских и солдатских депутатов", "г) Литовский провинциальный комитет" },
            correctAnswerIndex = 1 // б) Комитет западных губерний
        },
        new Question
        {
            questionText = "3. Политика российского правительства по исключению мелкой шляхты из дворянства и переводу ее в податные сословия носила название:",
            answers = new string[] { "а) «разбор» шляхты", "б) реквизиция", "в) санация", "г) национализация" },
            correctAnswerIndex = 0 // а) «разбор» шляхты
        },
        new Question
        {
            questionText = "4. В 1832 г. российским правительством было закрыто следующее учебное заведение:",
            answers = new string[] { "а) Горы-Горецкая земледельческая школа", "б) Полоцкая иезуитская академия", "в) Белорусская учительская семинария в Витебске", "г) Виленский университет" },
            correctAnswerIndex = 3 // г) Виленский университет
        },
        new Question
        {
            questionText = "5. В 1884 г. в Петербурге студенты – уроженцы Беларуси А. Марченко и Х. Ратнер создали:",
            answers = new string[] { "а) «Землю и волю»", "б) «Черный передел»", "в) «Народнаю волю»", "г) группу «Гомон»" },
            correctAnswerIndex = 3 // г) группу «Гомон»
        },
        new Question
        {
            questionText = "6. Первая белорусская политическая партия носила название:",
            answers = new string[] { "а) Белорусская народная партия социалистов", "б) Белорусская социалистическая громада", "в) Литовская социал-демократическая партия", "г) Белорусская христианская демократия" },
            correctAnswerIndex = 1 // б) Белорусская социалистическая громада
        },
        new Question
        {
            questionText = "7. Одной из особенностей развития сельского хозяйства в Беларуси в первой половине XIX в. было господство землевладения:",
            answers = new string[] { "а) государственного", "б) церковного", "в) помещичьего", "г) крестьянского" },
            correctAnswerIndex = 2 // в) помещичьего
        },
        new Question
        {
            questionText = "8. Большинство крестьян-депутатов от Беларуси в I Государственной думе в решении национального вопроса придерживались позиции:",
            answers = new string[] {
                "а) построения независимого белорусского государства",
                "б) отказа от автономии края в составе Российской империи",
                "в) автономизация белорусского края в Российской империи",
                "г) создания Союзного государства Беларуси и России"
            },
            correctAnswerIndex = 2 // в) придания статуса автономии белорусскому краю в составе Российской империи
        },
        new Question
        {
            questionText = "9. С 1914 г. редактором газеты «Наша Нива» являлся:",
            answers = new string[] { "а) Я. Купала", "б) Я. Колас", "в) В. Ластовский", "г) В. Игнатовский" },
            correctAnswerIndex = 0 // а) Я. Купала
        },
        new Question
        {
            questionText = "10. Буржуазные реформы 1860–1870-х гг. в Беларуси проводились с опозданием и со значительными ограничениями по причине:",
            answers = new string[] {
                "а) удаленности белорусских губерний от губерний России",
                "б) восстания 1863–1864 гг.",
                "в) превращения Беларуси в автономный регион России",
                "г) отсутствия поддержки со стороны местной элиты"
            },
            correctAnswerIndex = 1 // б) восстания 1863–1864 гг.
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