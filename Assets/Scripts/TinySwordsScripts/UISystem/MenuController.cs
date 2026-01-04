using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject healthUI;
    public GameObject goldUI;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf);

            if (healthUI != null)
            {
                healthUI.SetActive(!menuCanvas.activeSelf);
            }

            if (goldUI != null)
            {
                goldUI.SetActive(!menuCanvas.activeSelf);
            }

            PauseController.SetPause(menuCanvas.activeSelf);
        }
    }
}
