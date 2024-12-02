using UnityEngine;

public class ClosePanelKill : MonoBehaviour
{
    public GameObject kill;
    public void Close()
    {
        kill.SetActive(false);
    }

}
