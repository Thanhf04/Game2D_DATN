using UnityEngine;

public class OpenMiniGame_Input : MonoBehaviour
{
    public GameObject PanelDialogueMiniGameInput;
    public GameObject PanelMiniGame_Input;
    public static bool isMiniGameInput = false;
    public static bool isDialogue_MiniGameInput = false;
    private void OnMouseDown()
    {
        PanelManager.Instance.OpenPanel(PanelDialogueMiniGameInput);
        isDialogue_MiniGameInput = true;
    }
    public void CloseDialog()
    {
        PanelManager.Instance.ClosePanel(PanelDialogueMiniGameInput);
        isDialogue_MiniGameInput = false;
    }
    public void OpenGameInput()
    {
        PanelManager.Instance.OpenPanel(PanelMiniGame_Input);
        isMiniGameInput = true;
    }
    public void CloseGameInput()
    {
        PanelManager.Instance.ClosePanel(PanelMiniGame_Input);
        isMiniGameInput = false;
    }
}
