using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageManager : SingletonMonobehaviour<LanguageManager>
{
    public GameObject select_language_page;
    byte mode;

    byte select_language;

    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[DataManager.instance.language];
    }

    public void on_select_language_page()
    {
        select_language_page.SetActive(true);
        mode = 1;
    }

    public void on_select_change_language_page()
    {
        select_language_page.SetActive(true);
        mode = 2;
    }

    public void close_select_language_page()
    {
        DataManager.instance.language = select_language;
        DataManager.instance.save_language();

        switch (mode)
        {
            case 1:
                {
                    LoginManager.instance.select_language = true;
                }
                break;
            case 2:
                {
                    AccountManager.instance.success_change();
                }
                break;
        }
        select_language_page.SetActive(false); 
    }

    public void SelectKorean()
    {
        LocaleSelected(0);
    }

    public void SelectJapanese()
    {
        LocaleSelected(1);
    }

    public void SelectEnglish()
    {
        LocaleSelected(2);
    }

    public void SelectChinese()
    {
        LocaleSelected(3);
    }

    void LocaleSelected(int index)
    {
        select_language = (byte)index;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
