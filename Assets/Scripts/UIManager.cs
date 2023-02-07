using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        restartBtn.onClick.AddListener(() =>
        {
            Debug.Log("Restart");
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        exitBtn.onClick.AddListener(Application.Quit);
    }
}