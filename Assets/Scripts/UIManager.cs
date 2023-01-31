using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        clientBtn.onClick.AddListener(() =>
        {
            Debug.Log("Restart");
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}