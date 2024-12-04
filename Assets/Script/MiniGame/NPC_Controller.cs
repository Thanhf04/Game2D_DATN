using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public GameObject panelDialogueNpc;
    public GameObject panelMiniGame;
    public static bool isDialogue = false;
    private void OnMouseDown()
    {
        PanelManager.Instance.OpenPanel(panelDialogueNpc);
        isDialogue = true;
    }
    public void CloseDialogue()
    {
        PanelManager.Instance.ClosePanel(panelDialogueNpc);
        isDialogue = false;
    }
    public void OpenMiniGame()
    {
        PanelManager.Instance.OpenPanel(panelMiniGame);
        isDialogue = true;
    }
}
