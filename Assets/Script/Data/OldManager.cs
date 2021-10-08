using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OLD : int
{
    NONE,
    TEN,
    TWENTY,
    THIRTY,
    FORTY,
    FIFTY,
    SIXTY,
    SEVENTY
}

public class OldManager :  SingletonMonobehaviour<OldManager>
{
    byte mode;

    public GameObject old_set_page;
    public Button close_button;

    public GameObject old_select_box;

    OLD select_old;

    public Text old_text;

    public void Start()
    {
        OldSlot[] slots = old_select_box.transform.Find("Panel").GetComponentsInChildren<OldSlot>();
        for (int i = 0; i < slots.Length; ++i)
        {
            slots[i].set((OLD)(i + 1), select_this_old);
        }
    }

    public void player_set_old()
    {
        mode = 0;
        select_old = OLD.NONE;
        old_text.text = "연령대 선택하기";
        close_button.gameObject.SetActive(false);
        close_select_old_slot();

        old_set_page.SetActive(true);
    }

    public void player_change_old()
    {
        mode = 1;
        select_old = OLD.NONE;
        old_text.text = "연령대 선택하기";
        close_button.gameObject.SetActive(true);
        close_select_old_slot();

        old_set_page.SetActive(true);
    }

    public void close_player_change_old()
    {
        old_set_page.SetActive(false);
    }

    public void on_select_old_slot()
    {
        old_select_box.SetActive(true);
    }

    public void close_select_old_slot()
    {
        old_select_box.SetActive(false);
    }

    public void select_this_old(OLD old)
    {
        select_old = old;
        old_text.text = Converter.old_to_string(old);
    }

    public void complete_set_old()
    {
        if(select_old == OLD.NONE)
        {
            return;
        }

        DataManager.instance.save_my_old_data(select_old);

        select_old = OLD.NONE;
        old_select_box.SetActive(false);
        old_set_page.SetActive(false);

        if (mode == 0)
        {
            LoginManager.instance.check_user_set_data();
        }
        else if (mode == 1)
        {
            AccountManager.instance.success_change();
        }
    }
}

