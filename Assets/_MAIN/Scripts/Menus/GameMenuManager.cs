using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private CanvasGroup mainButtons;
    [SerializeField] private CanvasGroup logo;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;
    private CanvasGroupController mainMenuButtons;
    private CanvasGroupController logoCG;
    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        if (logo != null && mainButtons != null)
        {
            mainMenuButtons = new CanvasGroupController(this, mainButtons);
            logoCG = new CanvasGroupController(this, logo);
        }

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

    public void OpenConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void OpenHelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page)
    {
        HideLogoAndButtons();

        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    private void HideLogoAndButtons()
    {
        if (mainMenuButtons == null && logo == null)
            return;

        if (mainMenuButtons.isVisible && logoCG.isVisible)
        {
            mainMenuButtons.Hide();
            logoCG.Hide();
        }
    }

    private void ShowLogoAndButtons()
    {
        if (mainMenuButtons == null && logo == null)
            return;

        mainMenuButtons.Show();
        logoCG.Show();
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
        ShowLogoAndButtons();

        rootCG.SetInteractableState(false);

        if (activePage != null)
            activePage.Close();

        activePage = null;

        isOpen = false;
    }

    public void ClickPlay()
    {
        Game_Configuration.activeConfig.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScreen");
    }

    public void ClickLevelMenu()
    {
        Game_Configuration.activeConfig.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }

    public void ClickQuit()
    {
        uiChoiceMenu.Show("Quit to desktop?", new UIConfirmationMenu.ConfirmationButton("Yes", () => Application.Quit()), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

}
