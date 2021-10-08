using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum COUNTRY
{
    NONE,
    KOREA,
    JAPAN,
    CHINA
}

public class CountryManager : SingletonMonobehaviour<CountryManager>
{
    byte mode;

    public GameObject set_country_page;
    public Button close_button;

    COUNTRY select_my_country;

    public List<COUNTRY> country_list;

    public GameObject country_list_panel;

    public GameObject country_button;

    CountryButton select_country_button;

    Sprite korea;
    Sprite japan;
    Sprite china;

    public void Start()
    {
        korea = Resources.Load<Sprite>("Image/Korea");
        japan = Resources.Load<Sprite>("Image/Nipon");
        china = Resources.Load<Sprite>("Image/China");
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
    }

    public Sprite get_country_sprite(COUNTRY country)
    {
        switch (country)
        {
            case COUNTRY.KOREA:
                {
                    return korea;
                }
            case COUNTRY.JAPAN:
                {
                    return japan;
                }
            case COUNTRY.CHINA:
                {
                    return china;
                }
            default:
                {
                    return null;
                }
        }
    }
}
