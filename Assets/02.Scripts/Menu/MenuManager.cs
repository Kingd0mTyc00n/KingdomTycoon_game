using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    [SerializeField] List<Screen> screens;
    [SerializeField] List<HUDBar> hUDBars;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        screens = new List<Screen>(FindObjectsOfType<Screen>(true));
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

    public List<Screen> GetMenuList()
    {
        return screens;
    }

    public Screen GetScreenByName(string menuName)
    {
        foreach (Screen menu in screens)
        {
            if (menu.name == menuName)
                return menu;
        }
        return null;
    }

    public void OpenScreen(string screen)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].menuName == screen)
            {
                screens[i].Open();

            }
            if (screens[i].open && screens[i].menuName != screen)
            {
                CloseMenu(screens[i]);
            }
        }
    }

    public void OpenScreen(Screen screen)
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (screens[i].open)
            {
                CloseMenu(screens[i]);
            }
        }
        screen.Open();
    }

    public void CloseMenu(Screen screen)
    {
        screen.Close();
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
