using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionGuard : MonoBehaviour
{
    void Start()
    {
        if (SessionManager.Instance == null || !SessionManager.Instance.IsLoggedIn)
        {
            Debug.LogWarning("[SessionGuard] Нет активной сессии, перенаправление на LoginScene");
            SceneManager.LoadScene("LoginScene");
        }
    }
}
