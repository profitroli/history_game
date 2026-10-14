using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager8 : MonoBehaviour
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
            questionText = "1. 27 июля 1990 г. Верховный Совет БССР принял:",
            answers = new string[] {
                "а) Закон «Об утверждении Государственной символики БССР»",
                "б) Декларацию о государственном суверенитете БССР",
                "в) Закон «О гражданстве Республики Беларусь»",
                "г) Закон о местном самоуправлении"
            },
            correctAnswerIndex = 1 // б) Декларацию о государственном суверенитете БССР
        },
        new Question
        {
            questionText = "2. Первый Президент Республики Беларусь А.Г. Лукашенко был избран:",
            answers = new string[] { "а) 11 января 1994 г.", "б) 10 июля 1994 г.", "в) 21 сентября 1994 г.", "г) 7 декабря 1994 г." },
            correctAnswerIndex = 1 // б) 10 июля 1994 г.
        },
        new Question
        {
            questionText = "3. Референдум – это всенародное:",
            answers = new string[] { "а) избрание", "б) баллотирование", "в) собрание", "г) голосование" },
            correctAnswerIndex = 3 // г) голосование
        },
        new Question
        {
            questionText = "4. Одним из вопросов, вынесенных на республиканский референдум 24 ноября 1996 г. в Республике Беларусь, был вопрос о:",
            answers = new string[] {
                "а) придании русскому языку равного статуса с белорусским",
                "б) развитии экономической интеграции с Российской Федерацией",
                "в) установлении новых Государственных символов",
                "г) принятии новой редакции Конституции Республики Беларусь"
            },
            correctAnswerIndex = 3 // г) принятии новой редакции Конституции Республики Беларусь с изменениями и дополнениями
        },
        new Question
        {
            questionText = "5. Основным вопросом, вынесенным на республиканский референдум 17 октября 2004 г. в Республике Беларусь, был вопрос о (об):",
            answers = new string[] {
                "а) придании русскому языку равного статуса с белорусским",
                "б) принятия новой редакции Конституции Республики Беларусь",
                "в) изменении сроков нахождения лиц на должности Президента",
                "г) установлении новых Государственных символов"
            },
            correctAnswerIndex = 2 // в) внесении изменений в Конституцию Республики Беларусь о сроке нахождения на должности Президента одного и того же лица
        },
        new Question
        {
            questionText = "6. На референдумах, проводимых в Республике Беларусь, не ставился вопрос о (об):",
            answers = new string[] {
                "а) отмене смертной казни",
                "б) о свободной купли-продаже земли",
                "в) об экономической интеграции с Российской Федерацией",
                "г) о придании белорусскому языку статусу равного с русским"
            },
            correctAnswerIndex = 3 // г) о придании белорусскому языку статусу равного с русским
        },
        new Question
        {
            questionText = "7. 8 декабря 1991 г. руководители России (Б. Ельцин), Украины (Л. Кравчук) и Беларуси (С. Шушкевич) в Беловежской пуще подписали Соглашение об образовании:",
            answers = new string[] { "а) СНГ", "б) ЕАЭС", "в) ЕЭС", "г) ОДКБ" },
            correctAnswerIndex = 0 // а) СНГ
        },
        new Question
        {
            questionText = "8. Белорусская модель социально ориентированной рыночной экономики характеризуется:",
            answers = new string[] {
                "а) существованием только частной формы собственности",
                "б) существованием только государственной собственности",
                "в) вниманием к социальным потребностям человека",
                "г) поддержкой только фермерских хозяйств"
            },
            correctAnswerIndex = 2 // в) вниманием к социальным потребностям человека
        },
        new Question
        {
            questionText = "9. Решение о переименовании Белорусской Советской Социалистической Республики в Республику Беларусь было принято:",
            answers = new string[] { "а) 27 июля 1990 г.", "б) 25 августа 1991 г.", "в) 19 сентября 1991 г.", "г) 15 марта 1994 г." },
            correctAnswerIndex = 2 // в) 19 сентября 1991 г.
        },
        new Question
        {
            questionText = "10. 2 апреля 1996 г. между Республикой Беларусь и Российской Федерацией был заключен договор о создании:",
            answers = new string[] { "а) союза", "б) союзного государства", "в) федерации", "г) сообщества" },
            correctAnswerIndex = 3 // г) сообщества
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