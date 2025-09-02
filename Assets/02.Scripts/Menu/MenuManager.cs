using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] List<ScreenUI> screens;
    [SerializeField] List<HUDBar> hUDBars;

    public GameObject darkPanel;
    private CharacterController currentHunter;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        screens = new List<ScreenUI>(FindObjectsOfType<ScreenUI>(true));
        hUDBars = new List<HUDBar>(FindObjectsOfType<HUDBar>(true));
    }

    public void OpenHUDBar(HUDBar hUD)
    {
        for (int i = 0; i < hUDBars.Count; i++)
        {
            if (hUDBars[i].open)
            {
                CloseMenu(hUDBars[i]);
            }
        }
        hUD.Open();
    }

    public void CloseAllHUDBar()
    {
        foreach (var hUD in hUDBars)
        {
            hUD.Close();
        }
    }

    public void CloseMenu(HUDBar hUD)
    {
        hUD.Close();
    }


    public bool AllMenuIsClose()
    {
        foreach(var menu in screens)
        {
            if (menu.open)
            {
                return false;
            }
        }
        return true;
    }

    public List<ScreenUI> GetMenuList()
    {
        return screens;
    }

    public ScreenUI GetScreenByName(string menuName)
    {
        foreach (ScreenUI menu in screens)
        {
            if (menu.name == menuName)
                return menu;
        }
        return null;
    }
    // Open Diologue screen with specific hunter data
    public void OpenScreen(CharacterController hunter)
    {
        if (currentHunter == hunter) return;
        currentHunter = hunter;
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].open)
            {
                CloseMenu(screens[i]);
            }
        }
        OpenScreen("Diologue");
    }

    public void OpenScreen(string screen)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].menuName == screen)
            {
                screens[i].Open();
                darkPanel.SetActive(false);

            }
            if (screens[i].open && screens[i].menuName != screen)
            {
                CloseMenu(screens[i]);
            }
        }
    }

    public void OpenScreen(ScreenUI screen)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].open)
            {
                CloseMenu(screens[i]);
            }
        }
        screen.Open();
        darkPanel.SetActive(true);
    }

    public void CloseMenu(ScreenUI screen)
    {
        screen.Close();
        darkPanel.SetActive(false);
    }

    public void CloseScreen(string screen)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].menuName == screen)
            {
                screens[i].Close();

            }
        }

        darkPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
