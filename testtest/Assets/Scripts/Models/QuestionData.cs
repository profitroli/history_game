using System;

[Serializable]
public class QuestionData
{
    public string text;
    public string optionA;
    public string optionB;
    public string optionC;
    public string optionD;
    public int correctIndex;

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
}
