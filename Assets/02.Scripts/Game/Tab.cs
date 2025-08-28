using UnityEngine;

public class Tab : MonoBehaviour
{
    private GameObject activate;
    private GameObject deactivate;
    public GameObject popup;
    public bool isActivate = false;

    private void Start()
    {
        activate = transform.GetChild(0).gameObject;
        deactivate = transform.GetChild(1).gameObject;
    }

    public void ActivateTab()
    {
        activate.SetActive(true);
        deactivate.SetActive(false);
        popup.SetActive(true);
        isActivate = true;
    }

    public void DeactivateTab()
    {
        deactivate.SetActive(true);
        activate.SetActive(false);
        popup.SetActive(false);
        isActivate = false;
    }
}
