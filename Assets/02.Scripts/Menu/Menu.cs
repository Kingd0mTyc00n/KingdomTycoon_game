using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    [SerializeField] public string menuName;
    public bool open;

    public virtual void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
