using System;
using UnityEngine;

public class MiniGameKeeper : MonoBehaviour
{
    public static MiniGameKeeper currentMiniGameKeeper;
    public CanvasGroup miniGameCanvasGroup;
    // public MiniGameManager miniGameManager;

    public static event Action<MiniGameManager, bool> OnMiniGameStateChanged;

    public void OpenMiniGame()
    {
        PauseController.SetPause(true);
        currentMiniGameKeeper = this;
        // OnMiniGameStateChanged?.Invoke(miniGameManager, true);
        miniGameCanvasGroup.alpha = 1;
        miniGameCanvasGroup.blocksRaycasts = true;
        miniGameCanvasGroup.interactable = true;
    }

    public void CloseMiniGame()
    {
        PauseController.SetPause(false);
        currentMiniGameKeeper = null;
        // OnMiniGameStateChanged?.Invoke(miniGameManager, false);
        miniGameCanvasGroup.alpha = 0;
        miniGameCanvasGroup.blocksRaycasts = false;
        miniGameCanvasGroup.interactable = false;
    }
}