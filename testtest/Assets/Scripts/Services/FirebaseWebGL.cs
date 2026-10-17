using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseWebGL : IFirebaseService
{
    private readonly MonoBehaviour runner;

    public FirebaseWebGL(MonoBehaviour runner)
    {
        this.runner = runner;
        Debug.Log("[FirebaseWebGL] WebGL-реализация инициализирована (заглушка — будет реализована при WebGL-билде)");
    }

    // TODO: реализовать через .jslib когда дойдём до WebGL-деплоя
    // Пока используем REST как фоллбэк

    private FirebaseREST restFallback;
    private FirebaseREST REST
    {
        get
        {
            if (restFallback == null)
                restFallback = new FirebaseREST(
                    FirebaseManager.Instance.ApiKey,
                    FirebaseManager.Instance.ProjectId,
                    runner);
            return restFallback;
        }
    }

    public Task<AuthResult> SignInWithEmail(string email, string password) => REST.SignInWithEmail(email, password);
    public Task<AuthResult> SignUpWithEmail(string email, string password) => REST.SignUpWithEmail(email, password);
    public void SignOut() => REST.SignOut();
    public string GetCurrentUserId() => REST.GetCurrentUserId();
    public Task<Dictionary<string, object>> GetDocument(string collection, string documentId) => REST.GetDocument(collection, documentId);
    public Task<bool> SetDocument(string collection, string documentId, Dictionary<string, object> data) => REST.SetDocument(collection, documentId, data);
    public Task<bool> UpdateDocument(string collection, string documentId, Dictionary<string, object> data) => REST.UpdateDocument(collection, documentId, data);
    public Task<List<Dictionary<string, object>>> QueryByField(string collection, string field, object value) => REST.QueryByField(collection, field, value);
    public Task<bool> SendPasswordReset(string email) => REST.SendPasswordReset(email);
}
