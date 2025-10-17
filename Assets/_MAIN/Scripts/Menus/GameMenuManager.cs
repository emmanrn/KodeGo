using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;
    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);

    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    public void OpenCharacterPage()
    {
        var page = GetPage(MenuPage.PageType.CharacterSelection);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page)
    {
        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    public void OpenRoot()
    {
        rootCG.Show();
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void CloseRoot()
    {
        rootCG.Hide();
        rootCG.SetInteractableState(false);

        if (activePage != null)
            activePage.Close();

        activePage = null;

        isOpen = false;
    }

    public void ClickPlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScreen");
    }

    public void ClickQuit()
    {
        uiChoiceMenu.Show("Quit to desktop?", new UIConfirmationMenu.ConfirmationButton("Yes", () => Application.Quit()), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

}
