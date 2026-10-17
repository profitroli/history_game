using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseREST : IFirebaseService
{
    private readonly string apiKey;
    private readonly string projectId;
    private string idToken;
    private string refreshToken;
    private string currentUserId;
    private readonly MonoBehaviour coroutineRunner;

    private string AuthUrl => $"https://identitytoolkit.googleapis.com/v1/accounts:{{0}}?key={apiKey}";
    private string FirestoreUrl => $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";

    public FirebaseREST(string apiKey, string projectId, MonoBehaviour runner)
    {
        this.apiKey = apiKey;
        this.projectId = projectId;
        this.coroutineRunner = runner;
    }

    public Task<AuthResult> SignInWithEmail(string email, string password)
    {
        var tcs = new TaskCompletionSource<AuthResult>();
        coroutineRunner.StartCoroutine(AuthRequest("signInWithPassword", email, password, tcs));
        return tcs.Task;
    }

    public Task<AuthResult> SignUpWithEmail(string email, string password)
    {
        var tcs = new TaskCompletionSource<AuthResult>();
        coroutineRunner.StartCoroutine(AuthRequest("signUp", email, password, tcs));
        return tcs.Task;
    }

    public void SignOut()
    {
        idToken = null;
        refreshToken = null;
        currentUserId = null;
        tokenAge = 0;
    }

    public string GetCurrentUserId() => currentUserId;

    public Task<Dictionary<string, object>> GetDocument(string collection, string documentId)
    {
        var tcs = new TaskCompletionSource<Dictionary<string, object>>();
        coroutineRunner.StartCoroutine(GetDocumentCoroutine(collection, documentId, tcs));
        return tcs.Task;
    }

    public Task<bool> SetDocument(string collection, string documentId, Dictionary<string, object> data)
    {
        var tcs = new TaskCompletionSource<bool>();
        coroutineRunner.StartCoroutine(SetDocumentCoroutine(collection, documentId, data, false, tcs));
        return tcs.Task;
    }

    public Task<bool> UpdateDocument(string collection, string documentId, Dictionary<string, object> data)
    {
        var tcs = new TaskCompletionSource<bool>();
        coroutineRunner.StartCoroutine(SetDocumentCoroutine(collection, documentId, data, true, tcs));
        return tcs.Task;
    }

    public Task<List<Dictionary<string, object>>> QueryByField(string collection, string field, object value)
    {
        var tcs = new TaskCompletionSource<List<Dictionary<string, object>>>();
        coroutineRunner.StartCoroutine(QueryCoroutine(collection, field, value, tcs));
        return tcs.Task;
    }

    private IEnumerator AuthRequest(string method, string email, string password, TaskCompletionSource<AuthResult> tcs)
    {
        string url = string.Format(AuthUrl, method);
        string json = JsonUtility.ToJson(new AuthPayload { email = email, password = password, returnSecureToken = true });

        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var errorResp = JsonUtility.FromJson<AuthErrorResponse>(request.downloadHandler.text);
                string msg = errorResp?.error?.message ?? request.error;
                tcs.SetResult(new AuthResult { success = false, error = TranslateAuthError(msg) });
                yield break;
            }

            var resp = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            idToken = resp.idToken;
            refreshToken = resp.refreshToken;
            currentUserId = resp.localId;
            tcs.SetResult(new AuthResult { success = true, userId = resp.localId, email = resp.email });
        }
    }

    private IEnumerator GetDocumentCoroutine(string collection, string docId, TaskCompletionSource<Dictionary<string, object>> tcs)
    {
        string url = $"{FirestoreUrl}/{collection}/{docId}";
        using (var request = UnityWebRequest.Get(url))
        {
            if (!string.IsNullOrEmpty(idToken))
                request.SetRequestHeader("Authorization", $"Bearer {idToken}");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[FirebaseREST] GetDocument failed: {request.error}");
                tcs.SetResult(null);
                yield break;
            }

            var doc = FirestoreSerializer.DeserializeDocument(request.downloadHandler.text);
            tcs.SetResult(doc);
        }
    }

    private IEnumerator SetDocumentCoroutine(string collection, string docId, Dictionary<string, object> data, bool merge, TaskCompletionSource<bool> tcs)
    {
        string url = $"{FirestoreUrl}/{collection}/{docId}";
        string json = FirestoreSerializer.SerializeFields(data);

        string method = merge ? "PATCH" : "PATCH";
        using (var request = new UnityWebRequest(url, method))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(idToken))
                request.SetRequestHeader("Authorization", $"Bearer {idToken}");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[FirebaseREST] SetDocument failed: {request.error}");
                tcs.SetResult(false);
                yield break;
            }
            tcs.SetResult(true);
        }
    }

    private IEnumerator QueryCoroutine(string collection, string field, object value, TaskCompletionSource<List<Dictionary<string, object>>> tcs)
    {
        string url = $"{FirestoreUrl}:runQuery";
        string valueJson;
        if (value is string s)
            valueJson = $"\"stringValue\": \"{s}\"";
        else if (value is int i)
            valueJson = $"\"integerValue\": \"{i}\"";
        else
            valueJson = $"\"stringValue\": \"{value}\"";

        string queryJson = $@"{{
            ""structuredQuery"": {{
                ""from"": [{{ ""collectionId"": ""{collection}"" }}],
                ""where"": {{
                    ""fieldFilter"": {{
                        ""field"": {{ ""fieldPath"": ""{field}"" }},
                        ""op"": ""EQUAL"",
                        ""value"": {{ {valueJson} }}
                    }}
                }}
            }}
        }}";

        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(queryJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(idToken))
                request.SetRequestHeader("Authorization", $"Bearer {idToken}");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[FirebaseREST] Query failed: {request.error}");
                tcs.SetResult(new List<Dictionary<string, object>>());
                yield break;
            }

            var results = FirestoreSerializer.DeserializeQueryResults(request.downloadHandler.text);
            tcs.SetResult(results);
        }
    }

    public Task<bool> SendPasswordReset(string email)
    {
        var tcs = new TaskCompletionSource<bool>();
        coroutineRunner.StartCoroutine(PasswordResetCoroutine(email, tcs));
        return tcs.Task;
    }

    private IEnumerator PasswordResetCoroutine(string email, TaskCompletionSource<bool> tcs)
    {
        string url = string.Format(AuthUrl, "sendOobCode");
        string json = $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{email}\"}}";
        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            tcs.SetResult(request.result == UnityWebRequest.Result.Success);
        }
    }

    private IEnumerator RefreshTokenCoroutine()
    {
        if (string.IsNullOrEmpty(refreshToken)) yield break;
        string url = $"https://securetoken.googleapis.com/v1/token?key={apiKey}";
        string body = $"grant_type=refresh_token&refresh_token={refreshToken}";
        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<RefreshResponse>(request.downloadHandler.text);
                idToken = resp.id_token;
                refreshToken = resp.refresh_token;
                Debug.Log("[FirebaseREST] Token refreshed");
            }
        }
    }

    private float tokenAge;
    public void Update(float deltaTime)
    {
        if (string.IsNullOrEmpty(idToken)) return;
        tokenAge += deltaTime;
        if (tokenAge > 2700f)
        {
            tokenAge = 0;
            coroutineRunner.StartCoroutine(RefreshTokenCoroutine());
        }
    }

    private string TranslateAuthError(string error)
    {
        switch (error)
        {
            case "EMAIL_NOT_FOUND": return "Пользователь с таким email не найден";
            case "INVALID_PASSWORD": return "Неверный пароль";
            case "EMAIL_EXISTS": return "Пользователь с таким email уже зарегистрирован";
            case "WEAK_PASSWORD": return "Пароль должен быть не менее 6 символов";
            case "INVALID_EMAIL": return "Некорректный email";
            case "TOO_MANY_ATTEMPTS_TRY_LATER": return "Слишком много попыток, попробуйте позже";
            default: return $"Ошибка: {error}";
        }
    }

    [Serializable] private class AuthPayload { public string email; public string password; public bool returnSecureToken; }
    [Serializable] private class AuthResponse { public string idToken; public string refreshToken; public string localId; public string email; }
    [Serializable] private class AuthErrorResponse { public AuthError error; }
    [Serializable] private class AuthError { public string message; }
    [Serializable] private class RefreshResponse { public string id_token; public string refresh_token; }
}
