using System;
using System.Collections.Generic;

[Serializable]
public class AssignmentData
{
    public string assignmentId;
    public string name;
    public string groupId;
    public string teacherUid;
    public string teacherName;
    public List<QuestionData> questions = new List<QuestionData>();
    public bool active;
    public long createdAt;

    public Dictionary<string, object> ToFirestore()
    {
        var questionsMap = new Dictionary<string, object>();
        for (int i = 0; i < questions.Count; i++)
        {
            var qDict = new Dictionary<string, object>
            {
                { "type", questions[i].questionType },
                { "text", questions[i].text },
                { "correct", questions[i].correctIndex }
            };
            if (questions[i].questionType == QuestionData.TypeMultipleChoice)
            {
                qDict["optA"] = questions[i].optionA;
                qDict["optB"] = questions[i].optionB;
                qDict["optC"] = questions[i].optionC;
                qDict["optD"] = questions[i].optionD;
            }
            if (!string.IsNullOrEmpty(questions[i].correctTextAnswer))
                qDict["correctText"] = questions[i].correctTextAnswer;
            questionsMap[i.ToString()] = qDict;
        }

        return new Dictionary<string, object>
        {
            { "name", name },
            { "groupId", groupId },
            { "teacherUid", teacherUid },
            { "teacherName", teacherName },
            { "questions", questionsMap },
            { "questionCount", questions.Count },
            { "active", active },
            { "createdAt", createdAt }
        };
    }

    public static AssignmentData FromFirestore(Dictionary<string, object> doc, string docId)
    {
        var a = new AssignmentData
        {
            assignmentId = docId,
            name = Str(doc, "name"),
            groupId = Str(doc, "groupId"),
            teacherUid = Str(doc, "teacherUid"),
            teacherName = Str(doc, "teacherName"),
            active = doc.ContainsKey("active") && doc["active"] is bool b && b,
            createdAt = Lng(doc, "createdAt")
        };

        int count = (int)Lng(doc, "questionCount");
        if (doc.ContainsKey("questions") && doc["questions"] is Dictionary<string, object> qMap)
        {
            if (count == 0) count = qMap.Count;
            for (int i = 0; i < count; i++)
            {
                string key = i.ToString();
                if (qMap.ContainsKey(key) && qMap[key] is Dictionary<string, object> q)
                {
                    a.questions.Add(new QuestionData
                    {
                        questionType = (int)Lng(q, "type"),
                        text = Str(q, "text"),
                        optionA = Str(q, "optA"),
                        optionB = Str(q, "optB"),
                        optionC = Str(q, "optC"),
                        optionD = Str(q, "optD"),
                        correctIndex = (int)Lng(q, "correct"),
                        correctTextAnswer = Str(q, "correctText")
                    });
                }
            }
        }
        return a;
    }

    static string Str(Dictionary<string, object> d, string k) =>
        d.ContainsKey(k) ? d[k]?.ToString() ?? "" : "";

    static long Lng(Dictionary<string, object> d, string k)
    {
        if (!d.ContainsKey(k)) return 0;
        var v = d[k];
        if (v is long l) return l;
        if (v is int i) return i;
        if (v is string s && long.TryParse(s, out long p)) return p;
        return 0;
    }
}
