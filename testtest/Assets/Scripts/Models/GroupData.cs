using System;

[Serializable]
public class GroupData
{
    public string groupId;
    public string name;
    public string code;
    public string program; // "school" или "university"
    public string teacherUid;
    public string teacherName;
    public long createdAt;

    public bool IsSchool => program == "school";
}
