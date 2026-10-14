using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    [Header("Firebase Config")]
    [SerializeField] private string apiKey = "AIzaSyD2PW7-21-pkiW3q9L2ZFrfmsIrluQcNh4";
    [SerializeField] private string projectId = "history-game-59b70";
    [SerializeField] private string authDomain = "history-game-59b70.firebaseapp.com";

    public IFirebaseService Service { get; private set; }

    public string ApiKey => apiKey;
    public string ProjectId => projectId;
    public string AuthDomain => authDomain;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_WEBGL && !UNITY_EDITOR
        Service = new FirebaseWebGL(this);
        Debug.Log("[FirebaseManager] WebGL режим");
#else
        Service = new FirebaseREST(apiKey, projectId, this);
        Debug.Log("[FirebaseManager] REST режим (Editor/Desktop)");
#endif
    }
}
