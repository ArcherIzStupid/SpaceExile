using UnityEngine;

public class TabButton : MonoBehaviour
{
    public EditorTab tab;

    public void ChangeTab()
    {
        EditorManager.instance.ChangeTab(tab);
    }
}
