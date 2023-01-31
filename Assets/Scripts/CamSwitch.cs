using UnityEngine;

public class CamSwitch : MonoBehaviour
{
    [SerializeField] private Camera cam1;
    [SerializeField] private Camera cam2;
    [SerializeField] private Camera cam3;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cam1.enabled = true;
            cam2.enabled = false;
            cam3.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam1.enabled = false;
            cam2.enabled = true;
            cam3.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = true;
        }
    }
}