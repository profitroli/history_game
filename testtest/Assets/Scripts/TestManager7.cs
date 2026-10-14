using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager7 : MonoBehaviour
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
            questionText = "1. Основным направлением восстановления экономики БССР в послевоенный период стало (-а):",
            answers = new string[] { "а) сельское хозяйство", "б) легкая промышленность", "в) тяжелая промышленность", "г) сфера обслуживания" },
            correctAnswerIndex = 2 // в) тяжелая промышленность
        },
        new Question
        {
            questionText = "2. Особенностью восстановления народного хозяйства БССР в первое послевоенное десятилетие было:",
            answers = new string[] {
                "а) внедрение экономики с рыночными отношениями",
                "б) проведение коллективизации в западных областях",
                "в) внедрение территориального управления экономикой",
                "г) освоение целинных земель"
            },
            correctAnswerIndex = 1 // б) проведение коллективизации в западных областях
        },
        new Question
        {
            questionText = "3. На первой сессии Генеральной Ассамблеи ООН, начавшей свою работу в январе 1946 г., делегация БССР выступила с инициативой:",
            answers = new string[] {
                "а) пересмотреть существовавшие границы",
                "б) оказать пострадавшим в ходе войны народам помощь",
                "в) выдачи и наказания военных преступников",
                "г) предоставить всем республикам СССР членство в ООН"
            },
            correctAnswerIndex = 2 // в) выдачи и наказания военных преступников
        },
        new Question
        {
            questionText = "4. Общественно-политическая жизнь в БССР во второй половине 1960-х – первой половине 1980-х гг. характеризовалась:",
            answers = new string[] {
                "а) провозглашением гласности",
                "б) проведением политики перестройки",
                "в) формированием многопартийности",
                "г) существованием однопартийной системы"
            },
            correctAnswerIndex = 3 // г) существованием однопартийной системы
        },
        new Question
        {
            questionText = "5. Общественно-политическая жизнь в БССР во второй половине 1950-х – первой половине 1960-х гг. характеризовалась:",
            answers = new string[] {
                "а) частичной реабилитацией репрессированных",
                "б) проведением политики перестройки",
                "в) формированием многопартийности",
                "г) возникновением парламентской оппозиции"
            },
            correctAnswerIndex = 0 // а) частичной реабилитацией репрессированных
        },
        new Question
        {
            questionText = "6. Политико-воспитательную работу среди молодежи в БССР во второй половине 1950-х – первой половине 1980-х гг. проводил (-ли):",
            answers = new string[] {
                "а) комсомол",
                "б) профессиональные союзы",
                "в) Верховный Совет БССР",
                "г) Советы депутатов трудящихся"
            },
            correctAnswerIndex = 0 // а) комсомол
        },
        new Question
        {
            questionText = "7. Период в истории БССР второй половины 1950-х – первой половины 1960-х гг. называется периодом:",
            answers = new string[] { "а) оттепели", "б) застоя", "в) перестройки", "г) суверенитета" },
            correctAnswerIndex = 0 // а) оттепели
        },
        new Question
        {
            questionText = "8. В 1956–1965 гг. Коммунистическую партию Беларуси возглавлял:",
            answers = new string[] { "а) П. К. Пономаренко", "б) Н. И. Гусаров", "в) К. Т. Мазуров", "г) П. М. Машеров" },
            correctAnswerIndex = 2 // в) К. Т. Мазуров
        },
        new Question
        {
            questionText = "9. В конце 1970-х – первой половине 1980-х гг. негативные явления в экономике получили определение:",
            answers = new string[] { "а) «достойные»", "б) «застойные»", "в) «чуждые»", "г) «существенные»" },
            correctAnswerIndex = 1 // б) «застойные»
        },
        new Question
        {
            questionText = "10. В середине 1980-х гг. БССР, как и СССР, вступила в период радикальных преобразований, которые вошли в историю как:",
            answers = new string[] { "а) «передвижка»", "б) «модернизация»", "в) «перестройка»", "г) «реформа»" },
            correctAnswerIndex = 2 // в) «перестройка»
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