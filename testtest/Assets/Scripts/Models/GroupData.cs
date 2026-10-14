using System;

[Serializable]
public class GroupData
{
    public string groupId;
    public string name;
    public string code;
    public string program;
    public string teacherUid;
    public string teacherName;
    public string institution;
    public bool archived;
    public long createdAt;

    public bool IsSchool => program == "school";
}
