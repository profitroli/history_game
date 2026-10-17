using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager14 : MonoBehaviour
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
            questionText = "1. Какие общества (товарищества) не существовали в Беларуси в первой половине XIX в.:",
            answers = new string[] { "а) филоматы", "б) филареты", "в) эсеры", "г) декабристы" },
            correctAnswerIndex = 2 // в) эсеры
        },
        new Question
        {
            questionText = "2. В 1898 г. на съезде в Минске была образована:",
            answers = new string[] {
                "а) Партия социалистов революционеров",
                "б) Российская социал-демократическая рабочая партия",
                "в) Кадетская партия",
                "г) Социал-демократическая партия народного согласия"
            },
            correctAnswerIndex = 1 // б) Российская социал-демократическая рабочая партия
        },
        new Question
        {
            questionText = "3. Среди национальных социал-демократических партий рубежа XIX–XX вв. интересы еврейского пролетариата представлял (а):",
            answers = new string[] { "а) Бунд", "б) РСДРП", "в) БСГ", "г) ПСР" },
            correctAnswerIndex = 0 // а) Бунд
        },
        new Question
        {
            questionText = "4. К числу правых (монархических) партий начала XX в. относился:",
            answers = new string[] {
                "а) «Союз коммунистов»",
                "б) Всеобщий еврейский рабочий союз",
                "в) Союз русского народа",
                "г) Коммунистический союз молодежи Беларуси"
            },
            correctAnswerIndex = 2 // в) Союз русского народа
        },
        new Question
        {
            questionText = "5. Либерально-буржуазное направление в начале XX в. представляли:",
            answers = new string[] { "а) бундовцы", "б) кадеты", "в) черносотенцы", "г) эсеры" },
            correctAnswerIndex = 1 // б) кадеты
        },
        new Question
        {
            questionText = "6. Коммунистическая партия (большевиков) Беларуси (КП(б)Б) была создана в:",
            answers = new string[] { "а) 1898 г.", "б) 1908 г.", "в) 1918 г.", "г) 1998 г." },
            correctAnswerIndex = 2 // в) 1918 г.
        },
        new Question
        {
            questionText = "7. «Руководящей и направляющей силой советского общества» Коммунистическая партия Советского Союза была признана Конституцией СССР:",
            answers = new string[] { "а) 1918 г.", "б) 1924 г.", "в) 1936 г.", "г) 1977 г." },
            correctAnswerIndex = 3 // г) 1977 г.
        },
        new Question
        {
            questionText = "8. Закон Республики Беларусь «О политических партиях» был принят в:",
            answers = new string[] { "а) 1991 г.", "б) 1994 г.", "в) 1995 г.", "г) 2000 г." },
            correctAnswerIndex = 1 // б) 1994 г.
        },
        new Question
        {
            questionText = "9. Правоцентристская (либеральная) идеология является основой деятельности:",
            answers = new string[] {
                "а) Белорусской аграрной партии",
                "б) Коммунистическая партия Беларуси",
                "в) Объединенной гражданской партии",
                "г) Республиканской партии труда и справедливости"
            },
            correctAnswerIndex = 2 // в) Объединенной гражданской партии
        },
        new Question
        {
            questionText = "10. В 2023 г. в Республике Беларусь была создана политическая партия:",
            answers = new string[] {
                "а) «Белая Русь»",
                "б) Белорусская социал-демократическая Громада",
                "в) Белорусская социально-спортивная партия",
                "г) Социал-демократическая партия народного согласия"
            },
            correctAnswerIndex = 0 // а) «Белая Русь»
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


        ProgressService.Instance?.ReportTestCompleted(
            SceneManager.GetActiveScene().name, playerScore, questions.Count);
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