using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager10 : MonoBehaviour
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
            questionText = "1. Во главе древних белорусских княжеств в IX–XIII вв. стоял:",
            answers = new string[] { "а) гетман", "б) король", "в) царь", "г) князь" },
            correctAnswerIndex = 3 // г) князь
        },
        new Question
        {
            questionText = "2. Первым известным князем на территории Беларуси был:",
            answers = new string[] { "а) Ягайло", "б) Владимир", "в) Рогволод", "г) Всеслав (Чародей)" },
            correctAnswerIndex = 2 // в) Рогволод
        },
        new Question
        {
            questionText = "3. Великий князь литовский, который ввел институт наместничества в ВКЛ, централизировав власть, – это …",
            answers = new string[] { "а) Миндовг", "б) Витовт", "в) Ольгерд", "г) Ягайло" },
            correctAnswerIndex = 1 // б) Витовт
        },
        new Question
        {
            questionText = "4. Король Речи Посполитой во второй половине XVI – первой половине XVIII вв.:",
            answers = new string[] {
                "а) обязательно передавал свой трон по наследству",
                "б) выбирался шляхтой на Вальном Сейме",
                "в) был полностью независим в принятии решений",
                "г) выполнял единолично законодательные функции в государстве"
            },
            correctAnswerIndex = 1 // б) выбирался шляхтой на Вальном Сейме
        },
        new Question
        {
            questionText = "5. Высшее должностное лицо в БССР по Конституции 1978 г. – это …",
            answers = new string[] {
                "а) Президент БССР",
                "б) Народный комиссар БССР",
                "в) Председатель Верховного Совета БССР",
                "г) Генеральный Секретарь Верховного Совета БССР"
            },
            correctAnswerIndex = 2 // в) Председатель Верховного Совета БССР
        },
        new Question
        {
            questionText = "6. Должность Президента в Республике Беларусь была введена согласно:",
            answers = new string[] {
                "а) Декларации о государственном суверенитете БССР",
                "б) Конституции Республики Беларусь 1994 г.",
                "в) Закону о государственном суверенитете",
                "г) Закону о разделении властей"
            },
            correctAnswerIndex = 1 // б) Конституции Республики Беларусь 1994 г.
        },
        new Question
        {
            questionText = "7. Согласно Конституции Республики Беларусь 1994 г. (с изменениями 2004, 2022 гг.), возрастной порог для кандидата в Президенты страны начинается с:",
            answers = new string[] { "а) 30 лет", "б) 40 лет", "в) 50 лет", "г) 60 лет" },
            correctAnswerIndex = 1 // б) 40 лет
        },
        new Question
        {
            questionText = "8. Согласно Конституции Республики Беларусь 1994 г. (с изменениями 2004, 2022 гг.), Президент в случае нарушения Конституции, может быть отстранен от должности:",
            answers = new string[] {
                "а) Конституционным судом",
                "б) Палатой представителей",
                "в) Советом Республики",
                "г) Всебелорусским народным собранием"
            },
            correctAnswerIndex = 3 // г) Всебелорусским народным собранием
        },
        new Question
        {
            questionText = "9. Согласно Конституции Республики Беларусь 1994 г. (с изменениями 2004, 2022 гг.), в случае невозможности исполнения обязанностей Президента его полномочия переходят к:",
            answers = new string[] {
                "а) Премьер-министру Республики Беларусь",
                "б) Председателю Совета Республики",
                "в) Председателю Палаты представителей",
                "г) Председателю Конституционного Суда"
            },
            correctAnswerIndex = 1 // б) Председателю Совета Республики
        },
        new Question
        {
            questionText = "10. Согласно Конституции Республики Беларусь 1994 г. (с изменениями 2004, 2022 гг.), полномочия руководителя государства в Республике Беларусь ограничиваются:",
            answers = new string[] { "а) 2 сроками по 5 лет", "б) 1 сроком в 5 лет", "в) 3 сроками по 3 года", "г) сроки не ограничены" },
            correctAnswerIndex = 0 // а) 2 сроками по 5 лет
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