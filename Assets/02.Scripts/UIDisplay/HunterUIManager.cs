using System.Collections.Generic;
using UnityEngine;

public class HunterUIManager : MonoBehaviour
{
    [SerializeField] private HunterUI hunterUIPrefab;
    [SerializeField] private RectTransform content;

    private List<HunterUI> huntersUI = new List<HunterUI>();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        huntersUI.Clear();
        InitializeListHunterUI(UserData.UserDeepData.HuntersData);
    }

    public void InitializeListHunterUI(List<HunterData> huntersData)
    {
        for (int i = 0; i < huntersData.Count; i++)
        {
            HunterUI hunterUI = Instantiate(hunterUIPrefab);
            hunterUI.transform.SetParent(content);
            hunterUI.transform.localScale = Vector3.one;
            hunterUI.transform.localPosition = Vector3.zero;
            hunterUI.name = huntersData[i].Name;
            hunterUI.SetData(huntersData[i]);
        }
    }
}
