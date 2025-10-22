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
    [SerializeField] private CanvasGroup groupName;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;
    private CanvasGroupController mainMenuButtons;
    private CanvasGroupController logoCG;
    private CanvasGroupController groupNameCG;
    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        if (mainButtons != null)
            mainMenuButtons = new CanvasGroupController(this, mainButtons);

        if (logo != null)
            logoCG = new CanvasGroupController(this, logo);

        if (groupName != null)
            groupNameCG = new CanvasGroupController(this, groupName);

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
        Hide();

        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            OpenRoot();
    }

    private void Hide()
    {
        if (mainButtons != null)
        {
            if (mainMenuButtons.isVisible)
                mainMenuButtons.Hide();
        }

        if (logo != null)
        {
            if (logoCG.isVisible)
                logoCG.Hide();
        }

        if (groupName != null)
        {
            if (groupNameCG.isVisible)
                groupNameCG.Hide();
        }
    }

    private void Show()
    {
        mainMenuButtons?.Show();

        logoCG?.Show();

        groupNameCG?.Show();
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
        Show();

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
