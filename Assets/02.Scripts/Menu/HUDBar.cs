using UnityEngine;

public class HUDBar : Menu
{
    public GameObject selected;
    public override void Open()
    {
        selected.SetActive(true);
        MenuManager.instance.OpenScreen(gameObject.name);
        open = true;
    }

    public override void Close()
    {
        selected.SetActive(false);
        MenuManager.instance.CloseScreen(gameObject.name);
        open = false;
    }
}
