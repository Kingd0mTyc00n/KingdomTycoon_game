using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<Tab> tabs;

    private void Awake()
    {
        tabs.Clear();
        tabs =  new List<Tab>(FindObjectsOfType<Tab>(true));
    }

    public void OpenTab(Tab tab)
    {
        foreach (Tab t in tabs)
        {
            t.DeactivateTab();
        }
        tab.ActivateTab();
    }
}
