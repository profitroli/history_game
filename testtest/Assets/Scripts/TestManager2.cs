using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager2 : MonoBehaviour
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
        // Вопрос 1
        questions.Add(new Question
        {
            questionText = "1. Полоцк впервые упоминается в летописи в:",
            answers = new string[] { "а) 862 г.", "б) 980 г.", "в) 985 г.", "г) 988 г." },
            correctAnswerIndex = 0 // а) 862 г.
        });

        // Вопрос 2
        questions.Add(new Question
        {
            questionText = "2. Полоцкое княжество сформировалось на территории племенного союза:",
            answers = new string[] { "а) словен", "б) уличей", "в) вятичей", "г) кривичей" },
            correctAnswerIndex = 3 // г) кривичей
        });

        // Вопрос 3
        questions.Add(new Question
        {
            questionText = "3. Важную роль в формировании Полоцкого княжества сыграл водный торговый путь:",
            answers = new string[] { "а) «из варяг в греки»", "б) янтарный", "в) Неманский", "г) Западнобугский" },
            correctAnswerIndex = 0 // а) «из варяг в греки»
        });

        // Вопрос 4
        questions.Add(new Question
        {
            questionText = "4. Самый древний город на территории Беларуси – это...",
            answers = new string[] { "а) Минск", "б) Туров", "в) Заславль", "г) Полоцк" },
            correctAnswerIndex = 3 // г) Полоцк
        });

        // Вопрос 5
        questions.Add(new Question
        {
            questionText = "5. Холопы, рядовичи, закупы – это категории:",
            answers = new string[] { "а) ремесленников", "б) крестьян", "в) купцов", "г) представителей духовенства" },
            correctAnswerIndex = 1 // б) крестьян
        });

        // Вопрос 6
        questions.Add(new Question
        {
            questionText = "6. Первая религия, которая появилась у наших предков, - это ...",
            answers = new string[] { "а) мусульманство", "б) язычество", "в) христианство", "г) иудаизм" },
            correctAnswerIndex = 1 // б) язычество
        });

        // Вопрос 7
        questions.Add(new Question
        {
            questionText = "7. Храм Святой Софии был построен в Полоцке при князе:",
            answers = new string[] { "а) Рогволоде", "б) Брячеславе", "в) Всеславе", "г) Изяславе" },
            correctAnswerIndex = 2 // в) Всеславе
        });

        // Вопрос 8
        questions.Add(new Question
        {
            questionText = "8. Крещение Руси произошло в:",
            answers = new string[] { "а) 862 г.", "б) 980 г.", "в) 985 г.", "г) 988 г." },
            correctAnswerIndex = 3 // г) 988 г.
        });

        // Вопрос 9
        questions.Add(new Question
        {
            questionText = "9. Туровское княжество сформировалось на месте расселения:",
            answers = new string[] { "а) словен", "б) дреговичей", "в) вятичей", "г) кривичей" },
            correctAnswerIndex = 1 // б) дреговичей
        });

        // Вопрос 10
        questions.Add(new Question
        {
            questionText = "10. Князь-книжник Полоцкого княжества, один из первых принявших христианство – это...",
            answers = new string[] { "а) Рогволод", "б) Брячеслав", "в) Всеслав", "г) Изяслав" },
            correctAnswerIndex = 3 // г) Изяслав
        });

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