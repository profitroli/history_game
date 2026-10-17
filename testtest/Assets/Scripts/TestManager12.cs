using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager12 : MonoBehaviour
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
            questionText = "1. Высший орган законодательной власти в ВКЛ – это…",
            answers = new string[] { "а) Вальный сейм", "б) Рада", "в) Магистрат", "г) Вече" },
            correctAnswerIndex = 0 // а) Вальный сейм
        },
        new Question
        {
            questionText = "2. В результате Манифеста от 17 октября 1905 г. в Российской империи законодательным органом власти стал (-а):",
            answers = new string[] { "а) Комитет министров", "б) Сенат", "в) Синод", "г) Государственная дума" },
            correctAnswerIndex = 3 // г) Государственная дума
        },
        new Question
        {
            questionText = "3. Согласно Судебнику Казимира 1468 г.:",
            answers = new string[] {
                "а) вводилась кровная месть за преступления",
                "б) вводились единые во всем княжестве виды наказаний",
                "в) упразднялось крепостное право",
                "г) создавалась Постоянная Рада"
            },
            correctAnswerIndex = 1 // б) вводились единые во всем княжестве виды наказаний
        },
        new Question
        {
            questionText = "4. Уголовные дела в городах ВКЛ, получивших магдебургское право, рассматривали:",
            answers = new string[] { "а) лавники", "б) радцы", "в) бургомистры", "г) воеводы" },
            correctAnswerIndex = 0 // а) лавники
        },
        new Question
        {
            questionText = "5. Подкоморский суд ВКЛ рассматривал дела о:",
            answers = new string[] {
                "а) налоговых правонарушениях и преступлениях",
                "б) земельной собственности",
                "в) воровстве и грабежах",
                "г) семейных спорах и конфликтах"
            },
            correctAnswerIndex = 1 // б) земельной собственности
        },
        new Question
        {
            questionText = "6. Копный суд в ВКЛ рассматривал судебные дела в отношении:",
            answers = new string[] { "а) крестьян", "б) мещан", "в) духовенство", "г) знати" },
            correctAnswerIndex = 0 // а) крестьян
        },
        new Question
        {
            questionText = "7. Особенность судебной реформы на территории Беларуси второй половины XIX в. в том, что…",
            answers = new string[] {
                "а) судьи и присяжные не избирались, а назначались властью.",
                "б) создали департамент расследований",
                "в) судебные процессы стали закрытыми и непубличными",
                "г) организовывались исключительно сословные суды"
            },
            correctAnswerIndex = 0 // а) мировые судьи и присяжные не выбирались местными жителями, а назначались властями
        },
        new Question
        {
            questionText = "8. В соответствии с Законом СССР от 16.08.1938 «О судоустройстве СССР, союзных и автономных республик» гражданские и уголовные дела по искам были отнесены к компетенции:",
            answers = new string[] { "а) военных трибуналов", "б) Кассационного трибунала", "в) народных судов", "г) областных судов" },
            correctAnswerIndex = 2 // в) народных судов
        },
        new Question
        {
            questionText = "9. Парламент в Республике Беларусь состоит из:",
            answers = new string[] {
                "а) Национального собрания и Совета Республики",
                "б) Палаты представителей и Сената",
                "в) Совета депутатов и Государственной думы",
                "г) Палаты представителей и Совета Республики"
            },
            correctAnswerIndex = 3 // г) Палаты представителей и Совета Республики
        },
        new Question
        {
            questionText = "10. Систему судов общей юрисдикции в Республике Беларусь возглавляет (-ют):",
            answers = new string[] { "а) Верховный Суд", "б) Конституционный Суд", "в) Минский городской суд", "г) областные суды" },
            correctAnswerIndex = 0 // а) Верховный Суд
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