using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectedHunter : MonoBehaviour
{
    public static SelectedHunter Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public HunterController hunterController;

    public Button moveToOrcBtn;
    public Transform orcCamp;
    public Button moveToUndeadBtn;
    public Transform undeadCamp;
    public Button moveToDevilBtn;
    public Transform devilCamp;

    void Start()
    {
        moveToOrcBtn.onClick.AddListener(() =>
        {
            if (hunterController != null)
            {
                hunterController.SelectMap(orcCamp);
            }
            else
            {
                Debug.LogWarning("No hunter selected!");
            }
        });

        moveToUndeadBtn.onClick.AddListener(() =>
        {
            if (hunterController != null)
            {
                hunterController.SelectMap(undeadCamp);
            }
            else
            {
                Debug.LogWarning("No hunter selected!");
            }
        });

        moveToDevilBtn.onClick.AddListener(() =>
        {
            if (hunterController != null)
            {
                hunterController.SelectMap(devilCamp);
            }
            else
            {
                Debug.LogWarning("No hunter selected!");
            }
        });

        moveToDevilBtn.onClick.AddListener(() =>
        {
            if (hunterController != null)
            {
                hunterController.SelectMap(devilCamp);
            }
            else
            {
                Debug.LogWarning("No hunter selected!");
            }
        });
    }

    public void SelectedHunterObj(HunterController controller)
    {
        hunterController = controller;
    }




}
