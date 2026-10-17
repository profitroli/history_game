using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager6 : MonoBehaviour
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
            questionText = "1. 22 июня 1941 г. началась:",
            answers = new string[] { "а) Вторая мировая война", "б) Первая мировая война", "в) Великая Отечественная война", "г) советско-финляндская война" },
            correctAnswerIndex = 2 // в) Великая Отечественная война
        },
        new Question
        {
            questionText = "2. 26 ноября 1943 г. был освобожден от немецко-фашистских захватчиков первый областной центр БССР:",
            answers = new string[] { "а) Гомель", "б) Брест", "в) Минск", "г) Могилёв" },
            correctAnswerIndex = 0 // а) Гомель
        },
        new Question
        {
            questionText = "3. Впервые в июне 1941 г. по немецко-фашистским захватчикам был нанесен удар батареей реактивных установок «Катюш» под:",
            answers = new string[] { "а) Сенно", "б) Лепелем", "в) Оршей", "г) Борисовом" },
            correctAnswerIndex = 2 // в) Оршей
        },
        new Question
        {
            questionText = "4. Один из первых партизанских отрядов на пятый день войны организовал в Пинске:",
            answers = new string[] { "а) М. Шмырев", "б) А. Далидович", "в) В. Корж", "г) Т. Бумажков" },
            correctAnswerIndex = 2 // в) В. Корж
        },
        new Question
        {
            questionText = "5. В сентябре 1942 г. Белорусский штаб партизанского движения возглавил:",
            answers = new string[] { "а) П. К. Пономаренко", "б) П. З. Калинин", "в) П. М. Машеров", "г) К. Т. Мазуров" },
            correctAnswerIndex = 1 // б) П. З. Калинин
        },
        new Question
        {
            questionText = "6. 30 июля 1943 г. на железнодорожном узле Осиповичи осуществил самую крупную диверсию в годы Второй мировой войны:",
            answers = new string[] { "а) И. Казинец", "б) И. Ковалёв", "в) Ф. Крылович", "г) Н. Кедышко" },
            correctAnswerIndex = 2 // в) Ф. Крылович
        },
        new Question
        {
            questionText = "7. Впервые в начале Великой Отечественной войны новые советские танки Т-34 были использованы против немецко-фашистских захватчиков под:",
            answers = new string[] { "а) Сенно", "б) Лепелем", "в) Оршей", "г) Борисовом" },
            correctAnswerIndex = 3 // г) Борисовом
        },
        new Question
        {
            questionText = "8. Самым крупным в Беларуси и СССР является созданный немецко-фашистскими захватчиками лагерь смерти:",
            answers = new string[] { "а) Тростенецкий", "б) Озаричский", "в) Колдычевский", "г) Луполовский" },
            correctAnswerIndex = 0 // а) Тростенецкий
        },
        new Question
        {
            questionText = "9. В период с 23 июня по 29 августа 1944 г. советские войска провели крупнейшую наступательную военную операцию:",
            answers = new string[] { "а) «Ленинградско-Новгородскую»", "б) «Яссы-Кишеневскую»", "в) «Багратион»", "г) «Прибалтийскую»" },
            correctAnswerIndex = 2 // в) «Багратион»
        },
        new Question
        {
            questionText = "10. Операция белорусских партизан в 1943–1944 гг. против немецко-фашистских захватчиков вошли в историю под названием:",
            answers = new string[] { "а) «Молниеносная война»", "б) «Багратион»", "в) «Рельсовая война»", "г) «Котбус»" },
            correctAnswerIndex = 2 // в) «Рельсовая война»
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