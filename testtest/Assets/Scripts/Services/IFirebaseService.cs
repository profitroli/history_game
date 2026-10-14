using System.Collections.Generic;
using System.Threading.Tasks;

public class AuthResult
{
    public bool success;
    public string userId;
    public string email;
    public string error;
}

public interface IFirebaseService
{
    Task<AuthResult> SignInWithEmail(string email, string password);
    Task<AuthResult> SignUpWithEmail(string email, string password);
    void SignOut();
    string GetCurrentUserId();

    Task<Dictionary<string, object>> GetDocument(string collection, string documentId);
    Task<bool> SetDocument(string collection, string documentId, Dictionary<string, object> data);
    Task<bool> UpdateDocument(string collection, string documentId, Dictionary<string, object> data);
    Task<List<Dictionary<string, object>>> QueryByField(string collection, string field, object value);
}
