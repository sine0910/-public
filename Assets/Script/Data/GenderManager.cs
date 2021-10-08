using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GENDER
{
    NONE,
    MALE,
    FEMALE
}

public class GenderManager : SingletonMonobehaviour<GenderManager>
{
    byte mode;

    public GameObject set_gender_page;
    public GameObject female_select;
    public GameObject male_select;
    public Button close_button;

    public Sprite male_image;
    public Sprite female_image;

    GENDER gender;

    public void player_set_gender()
    {
        mode = 0;
        close_button.gameObject.SetActive(false);
        set_gender_page.SetActive(true);
    }

    public void player_change_gender()
    {
        mode = 1;
        close_button.gameObject.SetActive(true);
        set_gender_page.SetActive(true);
    }

    public void male()
    {
        gender = GENDER.MALE;
        male_select.SetActive(true);
        female_select.SetActive(false);
    }

    public void female()
    {
        gender = GENDER.FEMALE;
        female_select.SetActive(true);
        male_select.SetActive(false);
    }

    public void complete_select_gender()
    {
        if (gender == GENDER.NONE)
        {
            return;
        }

        DataManager.instance.save_my_gender_data(gender);

        if (mode == 0)
        {
            LoginManager.instance.check_user_set_data();
        }
        else if(mode == 1)
        {
            AccountManager.instance.success_change();
        }

        close_set_gender();
    }

    public void close_set_gender()
    {
        male_select.SetActive(false);
        female_select.SetActive(false);
        gender = GENDER.NONE;

        set_gender_page.SetActive(false);
    }

    public Sprite get_gender_sprite(GENDER gender)
    {
        switch (gender)
        {
            case GENDER.MALE:
                {
                    return male_image;
                }
            case GENDER.FEMALE:
                {
                    return female_image;
                }
            default:
                {
                    return null;
                }
        }
    }
}
