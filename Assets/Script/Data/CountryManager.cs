using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum COUNTRY : int
{
    NONE,
    KOREA = 1,
    JAPAN = 2,
    CHINA = 3,
    USA = 67,
    UK = 82,
    INDIA = 118,
    RUSSIA = 32,
    MEXICO = 10,
    TAIWAN = 50,
    NIGERIA = 17,
    FRANCE = 108,
    INDONESIA = 119,
    GERMANY = 109,
    COLOMBIA = 90,
    ITALY = 123,
    CANADA = 86,
    BRAZIL = 81,
    IRAN = 120,
    SPAIN = 46,
    EGYPT = 100,
    ARGENTINA = 70,
    NETHERLANDS = 114,
    PERU = 26,
    SAUDI_ARABIA = 37,
    TURKEY = 56,
    POLAND = 28,
    UAE = 58,
    AUSTRALIA = 72,
    SWEDEN = 48,
    BELGIUM = 78,
    THAILAND = 91,
    PAKISTAN = 21,
    NORWAY = 19,
    CHILE = 88,
    NEW_ZEALAND = 15,
    PHILIPPINES = 27,
    DENMARK = 95,
    MALAYSIA = 8,
    AUSTRIA = 45,
    IRELAND = 101,
    ISRAEL = 122,
    VIETNAM = 61,
    FINLAND = 107,
    HONG_KONG = 76,
    SOUTH_AFRICA = 44,
    CZECH_REPUBLIC = 94,
}

public class CountryManager : SingletonMonobehaviour<CountryManager>
{
    byte mode;

    public GameObject set_country_page;
    public Button close_button;

    COUNTRY select_my_country;

    public List<COUNTRY> country_list;

    public GameObject country_list_panel;
    public Scrollbar scrollbar;

    public GameObject country_button;

    CountryButton select_country_button;

    public static Dictionary<COUNTRY, string> country_dic;

    Sprite[] flags;

    public void Start()
    {
        flags = Resources.LoadAll<Sprite>("Image/Simple_Flags");

        country_dic = new Dictionary<COUNTRY, string>();
        country_dic.Add(COUNTRY.KOREA, "Korea");
        country_dic.Add(COUNTRY.JAPAN, "Japan");
        country_dic.Add(COUNTRY.CHINA, "China");
        country_dic.Add(COUNTRY.USA, "USA");
        country_dic.Add(COUNTRY.UK, "England");
        country_dic.Add(COUNTRY.INDIA, "England");
        country_dic.Add(COUNTRY.RUSSIA, "Russia");
        country_dic.Add(COUNTRY.MEXICO, "Mexico");
        country_dic.Add(COUNTRY.TAIWAN, "Taiwan");
        country_dic.Add(COUNTRY.NIGERIA, "Nigeria");
        country_dic.Add(COUNTRY.FRANCE, "France");
        country_dic.Add(COUNTRY.INDONESIA, "Indonesia");
        country_dic.Add(COUNTRY.GERMANY, "Germany");
        country_dic.Add(COUNTRY.COLOMBIA, "Colombia");
        country_dic.Add(COUNTRY.ITALY, "Italy");
        country_dic.Add(COUNTRY.CANADA, "Canada");
        country_dic.Add(COUNTRY.BRAZIL, "Brazil");
        country_dic.Add(COUNTRY.IRAN, "Iran");
        country_dic.Add(COUNTRY.SPAIN, "Spain");
        country_dic.Add(COUNTRY.EGYPT, "Egypt");
        country_dic.Add(COUNTRY.ARGENTINA, "Argentina");
        country_dic.Add(COUNTRY.NETHERLANDS, "Netherlands");
        country_dic.Add(COUNTRY.PERU, "Peru");
        country_dic.Add(COUNTRY.SAUDI_ARABIA, "KSA");
        country_dic.Add(COUNTRY.TURKEY, "Turkey");
        country_dic.Add(COUNTRY.POLAND, "Poland");
        country_dic.Add(COUNTRY.UAE, "UAE");
        country_dic.Add(COUNTRY.AUSTRALIA, "Australia");
        country_dic.Add(COUNTRY.SWEDEN, "Sweden");
        country_dic.Add(COUNTRY.BELGIUM, "Belgium");
        country_dic.Add(COUNTRY.THAILAND, "Thailand");
        country_dic.Add(COUNTRY.PAKISTAN, "Pakistan");
        country_dic.Add(COUNTRY.NORWAY, "Norway");
        country_dic.Add(COUNTRY.CHILE, "Chile");
        country_dic.Add(COUNTRY.NEW_ZEALAND, "New zealand");
        country_dic.Add(COUNTRY.PHILIPPINES, "Philippines");
        country_dic.Add(COUNTRY.DENMARK, "Denmark");
        country_dic.Add(COUNTRY.MALAYSIA, "Malaysia");
        country_dic.Add(COUNTRY.AUSTRIA, "Austria");
        country_dic.Add(COUNTRY.IRELAND, "Ireland");
        country_dic.Add(COUNTRY.VIETNAM, "Vietnam");
        country_dic.Add(COUNTRY.FINLAND, "Finland");
        country_dic.Add(COUNTRY.HONG_KONG, "Hong kong");
        country_dic.Add(COUNTRY.SOUTH_AFRICA, "RSA");
        country_dic.Add(COUNTRY.CZECH_REPUBLIC, "Czechia");
    }

    public void player_set_country()
    {
        set_country_list();

        mode = 0;
        close_button.gameObject.SetActive(false);
        set_country_page_setting();
    }

    public void player_change_country()
    {
        set_country_list();

        mode = 1;
        close_button.gameObject.SetActive(true);
        set_country_page_setting();
    }

    void set_country_page_setting()
    {
        reset();
        set_country_page.SetActive(true);

        Canvas.ForceUpdateCanvases();
        country_list_panel.transform.GetComponent<GridLayoutGroup>().enabled = false;
        country_list_panel.transform.GetComponent<GridLayoutGroup>().enabled = true;

        scrollbar.value = 1;
    }

    bool ready = true;
    public void complete_set_country()
    {
        if (select_my_country == COUNTRY.NONE)
        {
            return;
        }

        DataManager.instance.save_my_country_data(select_my_country);

        if (mode == 0)
        {
            if (ready)
            {
                ready = false;
                FirebaseManager.instance.upload_first_login(upload_my_first_login_result);
            }
        }
        else
        {
            AccountManager.instance.success_change();
            close_set_country();
        }
    }

    public void upload_my_first_login_result(byte result)
    {
        if (result == 1)
        {
            LoginManager.instance.check_user_set_data();
            close_set_country();
        }
        else
        {
            ready = true;
            NetworkManager.Network_Error();
        }
    }

    public void close_set_country()
    {
        set_country_page.SetActive(false);
    }

    void reset()
    {
        select_country_button = null;

        List<CountryButton> countries = country_list_panel.GetComponentsInChildren<CountryButton>().ToList();
        for (int i = 0; i < countries.Count; i++)
        {
            Destroy(countries[i].gameObject);
        }

        for(int i= 0; i< country_list.Count; i++)
        {
            CountryButton country = Instantiate(country_button).GetComponent<CountryButton>();
            country.transform.parent = country_list_panel.transform;
            country.set_country(country_list[i]);
        }
    }

    public void selete_country(CountryButton country)
    {
        if(select_country_button != null)
        {
            select_country_button.set_panel(false);
        }
        select_country_button = country;
        select_country_button.set_panel(true);
        select_my_country = select_country_button.get_country();
    }

    void set_country_list()
    {
        country_list = new List<COUNTRY>();
        country_list.Add(COUNTRY.KOREA);
        country_list.Add(COUNTRY.JAPAN);
        country_list.Add(COUNTRY.CHINA);
        country_list.Add(COUNTRY.USA);
        country_list.Add(COUNTRY.UK);
        country_list.Add(COUNTRY.INDIA);
        country_list.Add(COUNTRY.RUSSIA);
        country_list.Add(COUNTRY.MEXICO);
        country_list.Add(COUNTRY.TAIWAN);
        country_list.Add(COUNTRY.NIGERIA);
        country_list.Add(COUNTRY.FRANCE);
        country_list.Add(COUNTRY.INDONESIA);
        country_list.Add(COUNTRY.GERMANY);
        country_list.Add(COUNTRY.COLOMBIA);
        country_list.Add(COUNTRY.ITALY);
        country_list.Add(COUNTRY.CANADA);
        country_list.Add(COUNTRY.BRAZIL);
        country_list.Add(COUNTRY.IRAN);
        country_list.Add(COUNTRY.SPAIN);
        country_list.Add(COUNTRY.EGYPT);
        country_list.Add(COUNTRY.ARGENTINA);
        country_list.Add(COUNTRY.NETHERLANDS);
        country_list.Add(COUNTRY.PERU);
        country_list.Add(COUNTRY.SAUDI_ARABIA);
        country_list.Add(COUNTRY.TURKEY);
        country_list.Add(COUNTRY.POLAND);
        country_list.Add(COUNTRY.UAE);
        country_list.Add(COUNTRY.AUSTRALIA);
        country_list.Add(COUNTRY.SWEDEN);
        country_list.Add(COUNTRY.BELGIUM);
        country_list.Add(COUNTRY.THAILAND);
        country_list.Add(COUNTRY.PAKISTAN);
        country_list.Add(COUNTRY.NORWAY);
        country_list.Add(COUNTRY.CHILE);
        country_list.Add(COUNTRY.NEW_ZEALAND);
        country_list.Add(COUNTRY.PHILIPPINES);
        country_list.Add(COUNTRY.DENMARK);
        country_list.Add(COUNTRY.MALAYSIA);
        country_list.Add(COUNTRY.AUSTRIA);
        country_list.Add(COUNTRY.IRELAND);
        country_list.Add(COUNTRY.VIETNAM);
        country_list.Add(COUNTRY.FINLAND);
        country_list.Add(COUNTRY.HONG_KONG);
        country_list.Add(COUNTRY.SOUTH_AFRICA);
        country_list.Add(COUNTRY.CZECH_REPUBLIC);
    }

    public Sprite get_country_sprite(COUNTRY country)
    {
        try
        {
            return flags[(int)country];
        }
        catch (System.Exception e)
        {
            Debug.Log("get_country_sprite error => " + e);
            return flags[0];
        }
    }

    public static string country_to_string(COUNTRY country)
    {
        try
        {
            return country_dic[country];
        }
        catch (System.Exception e)
        {
            Debug.Log("country_to_string error => " + e);
            return "";
        }
    }
}
