using UnityEngine;

public class HelpScript : MonoBehaviour
{
    [SerializeField] GameObject helpMenu;
    [SerializeField] GameObject helpIcon;

    private void Start()
    {
        helpMenu.SetActive(false);
        helpIcon.SetActive(true);
    }

    public void OpenHelp()
    {
        helpMenu.SetActive(true);
        helpIcon.SetActive(false);
    }

    public void CloseHelp()
    {
        helpMenu.SetActive(false);
        helpIcon.SetActive(true);
    }
}