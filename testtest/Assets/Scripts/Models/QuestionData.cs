using System;

[Serializable]
public class QuestionData
{
    public int questionType;
    public string text;
    public string optionA;
    public string optionB;
    public string optionC;
    public string optionD;
    public int correctIndex;
    public string correctTextAnswer;

    public const int TypeMultipleChoice = 0;
    public const int TypeTrueFalse = 1;
    public const int TypeOpenText = 2;

    public string GetOption(int index)
    {
        switch (index)
        {
            case 0: return optionA;
            case 1: return optionB;
            case 2: return optionC;
            case 3: return optionD;
            default: return "";
        }
    }

    public string TypeLabel
    {
        get
        {
            switch (questionType)
            {
                case TypeTrueFalse: return "Да/Нет";
                case TypeOpenText: return "Текст";
                default: return "Тест";
            }
        }
    }
}
