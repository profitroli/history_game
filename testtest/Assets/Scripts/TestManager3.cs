using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager3 : MonoBehaviour
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
            questionText = "1. Первым князем ВКЛ был:",
            answers = new string[] { "а) Витень", "б) Витовт", "в) Гедимин", "г) Миндовг" },
            correctAnswerIndex = 3 // г) Миндовг
        });

        // Вопрос 2
        questions.Add(new Question
        {
            questionText = "2. Великий князь литовский, при котором территория ВКЛ начала простираться от Балтийского до Черного моря, – это…",
            answers = new string[] { "а) Ольгерд", "б) Витовт", "в) Гедимин", "г) Миндовг" },
            correctAnswerIndex = 1 // б) Витовт
        });

        // Вопрос 3
        questions.Add(new Question
        {
            questionText = "3. В 1385 г. была заключена уния:",
            answers = new string[] { "а) Городельская", "б) Кревская", "в) Виленско-Радамская", "г) Люблинская" },
            correctAnswerIndex = 1 // б) Кревская
        });

        // Вопрос 4
        questions.Add(new Question
        {
            questionText = "4. Согласно привилею Александра 1492 г. власть великого князя ограничивалась:",
            answers = new string[] { "а) сеймом", "б) Радой", "в) Сенатом", "г) Главным Трибуналом" },
            correctAnswerIndex = 1 // б) Радой
        });

        // Вопрос 5
        questions.Add(new Question
        {
            questionText = "5. Подскарбий в ВКЛ – это…",
            answers = new string[] { "а) глава великокняжеской канцелярии", "б) председательствующий на заседаниях Рады и Сейма", "в) начальник армии", "г) распорядитель государственной казны" },
            correctAnswerIndex = 3 // г) распорядитель государственной казны
        });

        // Вопрос 6
        questions.Add(new Question
        {
            questionText = "6. Согласно Конституции Речи Посполитой 3 мая 1791 г. провозглашалось о:",
            answers = new string[] {
            "а) введении ограничений на права «либерум вето»",
            "б) ликвидации права «либерум вето»",
            "в) каждый третий сейм Речи Посполитой созывался в Гродно",
            "г) проведении сеймов в Речи Посполитой только в Польше"
        },
            correctAnswerIndex = 1 // б) ликвидации права «либерум вето»
        });

        // Вопрос 7
        questions.Add(new Question
        {
            questionText = "7. Гетман ВКЛ, который вел борьбу с 80-тысячной армией Великого княжества Московского в битве под Оршей в 1514 г., – это:",
            answers = new string[] { "а) К. Острожский", "б) М. Глинский", "в) Я. Радзивилл", "г) М. Радзивилл" },
            correctAnswerIndex = 0 // а) К. Острожский
        });

        // Вопрос 8
        questions.Add(new Question
        {
            questionText = "8. Битва на реке Лань под Клецком в августе 1506 г. произошла между войсками ВКЛ и:",
            answers = new string[] { "а) крестоносцами", "б) крымскими татарами", "в) Королевством Польским", "г) Великим княжеством Московским" },
            correctAnswerIndex = 1 // б) крымскими татарами
        });

        // Вопрос 9
        questions.Add(new Question
        {
            questionText = "9. В 1764 г. великим князем Литовским и королем польским был избран:",
            answers = new string[] { "а) Станислав Лещинский", "б) Август II Сильный", "в) Станислав Август Понятовский", "г) Август III" },
            correctAnswerIndex = 2 // в) Станислав Август Понятовский
        });

        // Вопрос 10
        questions.Add(new Question
        {
            questionText = "10. Восстание 1794 г. на территории Беларуси возглавлял:",
            answers = new string[] { "а) Я. Ясинский", "б) К. Калиновский", "в) С. Грабовский", "г) М. Огинский" },
            correctAnswerIndex = 0 // а) Я. Ясинский
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