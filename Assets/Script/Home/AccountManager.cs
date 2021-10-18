using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountManager : SingletonMonobehaviour<AccountManager>
{
    DataManager data_manager;

    GameObject account_page;

    GameObject account_info_page;
    GameObject game_play_info_page;
    GameObject tier_info_page;

    Sprite on_click_button;
    Sprite non_click_button;

    Button account_info_button;
    Button game_play_info_button;
    Button tier_info_button;

    public GameObject vip_panel;

    #region Account Info Page
    GameObject player_data_success_change_panel;
    GameObject account_reset_page;
    GameObject account_out_page;

    Text name_text;
    Text rating_text;
    Text tier_text;
    Image country_image;
    Text country_text;
    Text old_text;
    Text gender_text;
    Image gender_image;

    Text heart_text;
    #endregion

    #region GamePlay Info Page
    byte time_set = 0;

    Text b_play_text;
    Text b_win_text;
    Text b_lose_text;
    Text b_tie_text;
    Text b_win_per_text;

    Text w_play_text;
    Text w_win_text;
    Text w_lose_text;
    Text w_tie_text;
    Text w_win_per_text;

    Button full_play_button;
    Button month_play_button;
    Button day_play_button;
    #endregion

    #region Tier/Rating
    Text now_tier_text;
    Text now_rating_text;

    Text more_rating_text;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        data_manager = DataManager.instance;

        on_click_button = Resources.Load<Sprite>("Image/Button_UI_07");
        non_click_button = Resources.Load<Sprite>("Image/Button_UI_06");

        account_page = transform.Find("AccountPage").gameObject;

        account_info_page = transform.Find("AccountPage/Main/MyData").gameObject;
        game_play_info_page = transform.Find("AccountPage/Main/MyPlayData").gameObject;
        tier_info_page = transform.Find("AccountPage/Main/MyTier").gameObject;

        account_info_button = transform.Find("AccountPage/Buttons/MyDataButton").GetComponent<Button>();
        game_play_info_button = transform.Find("AccountPage/Buttons/MyPlayDataButton").GetComponent<Button>();
        tier_info_button = transform.Find("AccountPage/Buttons/MyTierButton").GetComponent<Button>();

        account_info_button.onClick.AddListener(on_account_info_page);
        game_play_info_button.onClick.AddListener(on_game_play_info_page);
        tier_info_button.onClick.AddListener(on_tier_info_page);

        player_data_success_change_panel = transform.Find("SuccessChangePage").gameObject;
        account_reset_page = transform.Find("ResetPage").gameObject;
        account_out_page = transform.Find("AccountOutPage").gameObject;

        name_text = transform.Find("AccountPage/Main/MyData/name/ValueText").GetComponent<Text>();
        rating_text = transform.Find("AccountPage/Main/MyData/rating/ValueText").GetComponent<Text>();
        tier_text = transform.Find("AccountPage/Main/MyData/tier/ValueText").GetComponent<Text>();
        country_image = transform.Find("AccountPage/Main/MyData/map/ValueImage").GetComponent<Image>();
        country_text = transform.Find("AccountPage/Main/MyData/map/ValueText").GetComponent<Text>();
        old_text = transform.Find("AccountPage/Main/MyData/old/ValueText").GetComponent<Text>();
        gender_text = transform.Find("AccountPage/Main/MyData/gender/ValueText").GetComponent<Text>();
        gender_image = transform.Find("AccountPage/Main/MyData/gender/Image").GetComponent<Image>();

        heart_text = transform.Find("AccountPage/Main/MyData/money/ValueText").GetComponent<Text>();

        b_play_text = transform.Find("AccountPage/Main/MyPlayData/Black/PlayValueText").GetComponent<Text>();
        b_win_text = transform.Find("AccountPage/Main/MyPlayData/Black/WinValueText").GetComponent<Text>();
        b_lose_text = transform.Find("AccountPage/Main/MyPlayData/Black/LoseValueText").GetComponent<Text>();
        b_tie_text = transform.Find("AccountPage/Main/MyPlayData/Black/TieValueText").GetComponent<Text>();
        b_win_per_text = transform.Find("AccountPage/Main/MyPlayData/Black/WinLosePerValueText").GetComponent<Text>();

        w_play_text = transform.Find("AccountPage/Main/MyPlayData/White/PlayValueText").GetComponent<Text>();
        w_win_text = transform.Find("AccountPage/Main/MyPlayData/White/WinValueText").GetComponent<Text>();
        w_lose_text = transform.Find("AccountPage/Main/MyPlayData/White/LoseValueText").GetComponent<Text>();
        w_tie_text = transform.Find("AccountPage/Main/MyPlayData/White/TieValueText").GetComponent<Text>();
        w_win_per_text = transform.Find("AccountPage/Main/MyPlayData/White/WinLosePerValueText").GetComponent<Text>();

        full_play_button = transform.Find("AccountPage/Main/MyPlayData/FullButton").GetComponent<Button>();
        month_play_button = transform.Find("AccountPage/Main/MyPlayData/MonthButton").GetComponent<Button>();
        day_play_button = transform.Find("AccountPage/Main/MyPlayData/DayButton").GetComponent<Button>();

        full_play_button.onClick.AddListener(set_full_play_info);
        month_play_button.onClick.AddListener(set_month_play_info);
        day_play_button.onClick.AddListener(set_day_play_info);

        now_tier_text = transform.Find("AccountPage/Main/MyTier/MyTierText").GetComponent<Text>();
        now_rating_text = transform.Find("AccountPage/Main/MyTier/MyRatingText").GetComponent<Text>();

        more_rating_text = transform.Find("AccountPage/Main/MyTier/ExpBar/Exp_text").GetComponent<Text>();

        transform.Find("AccountPage/Title/CloseButton").GetComponent<Button>().onClick.AddListener(close_account_page);

        transform.Find("AccountPage/Main/MyData/AccountDeleteButton").GetComponent<Button>().onClick.AddListener(on_account_reset_page);
        transform.Find("AccountPage/Main/MyData/AccountOutButton").GetComponent<Button>().onClick.AddListener(on_out_account_page);

        transform.Find("ResetPage/ResetButton").GetComponent<Button>().onClick.AddListener(reset_account);
        transform.Find("ResetPage/CancleResetButton").GetComponent<Button>().onClick.AddListener(cancle_account_reset_page);
    }

    public void on_account_page()
    {
        account_page.SetActive(true);

        on_account_info_page();
    }

    public void on_vip_panel()
    {
        if (vip_panel.activeSelf)
        {
            vip_panel.SetActive(false);
        }
        else
        {
            vip_panel.SetActive(true);
        }
    }

    public void on_purchase()
    {
        PurchaseManager.instance.on_purchase();
        on_vip_panel();
    }

    public void on_success_purchase()
    {
        on_account_info_page();
    }

    #region Account Info
    public void on_account_info_page()
    {
        reset_page();
        account_info_page.SetActive(true);
        account_info_button.image.sprite = on_click_button;
        game_play_info_button.image.sprite = non_click_button;
        tier_info_button.image.sprite = non_click_button;

        set_account_info_page();
    }

    void set_account_info_page()
    {
        name_text.text = data_manager.my_name;
        rating_text.text = Converter.rating_to_string(data_manager.my_rating);
        tier_text.text = Converter.tier_to_string(data_manager.my_tier);
        country_image.sprite = CountryManager.instance.get_country_sprite(data_manager.my_country);
        country_text.text = Converter.country_to_string(data_manager.my_country);
        old_text.text = Converter.old_to_string(data_manager.my_old);
        gender_text.text = Converter.gender_to_string(data_manager.my_gender);
        gender_image.sprite = GenderManager.instance.get_gender_sprite(data_manager.my_gender);

        switch (data_manager.language)
        {
            case 0:
                {
                    heart_text.text = data_manager.my_heart + "개";
                }
                break;

            case 1:
                {
                    heart_text.text = data_manager.my_heart + "";
                }
                break;

            case 2:
                {
                    heart_text.text = data_manager.my_heart + "";
                }
                break;

            case 3:
                {
                    heart_text.text = data_manager.my_heart + "";
                }
                break;
        }
    }

    public void change_name()
    {
        NameManager.instance.player_change_name();
    }

    public void change_country()
    {
        CountryManager.instance.player_change_country();
    }

    public void change_old()
    {
        OldManager.instance.player_change_old();
    }

    public void change_gender()
    {
        GenderManager.instance.player_change_gender();
    }

    public void change_language()
    {
        LanguageManager.instance.on_select_change_language_page(); 
    }

    public void success_change()
    {
        on_account_info_page();
        player_data_success_change_panel.SetActive(true);
    }

    public void close_success_change()
    {
        player_data_success_change_panel.SetActive(false);
    }
    #endregion

    #region GamePlay Info
    void on_game_play_info_page()
    {
        reset_page();
        game_play_info_page.SetActive(true);

        game_play_info_button.image.sprite = on_click_button;
        account_info_button.image.sprite = non_click_button;
        tier_info_button.image.sprite = non_click_button;

        set_game_play_info_page();
    }

    void set_game_play_info_page()
    {
        switch (time_set)
        {
            case 0:
                {
                    b_play_text.text = data_manager.b_win_count + data_manager.b_lose_count + data_manager.b_tie_count + "";
                    b_win_text.text = data_manager.b_win_count + "";
                    b_lose_text.text = data_manager.b_lose_count + "";
                    b_tie_text.text = data_manager.b_tie_count + "";
                    b_win_per_text.text = get_win_percent(data_manager.b_win_count, data_manager.b_lose_count, data_manager.b_tie_count);

                    w_play_text.text = data_manager.w_win_count + data_manager.w_lose_count + data_manager.w_tie_count + "";
                    w_win_text.text = data_manager.w_win_count + "";
                    w_lose_text.text = data_manager.w_lose_count + "";
                    w_tie_text.text = data_manager.w_tie_count + "";
                    w_win_per_text.text = get_win_percent(data_manager.w_win_count, data_manager.w_lose_count, data_manager.w_tie_count);
                }
                break;

            case 1:
                {
                    b_play_text.text = data_manager.b_month_win_count + data_manager.b_month_lose_count + data_manager.b_month_tie_count + "";
                    b_win_text.text = data_manager.b_month_win_count + "";
                    b_lose_text.text = data_manager.b_month_lose_count + "";
                    b_tie_text.text = data_manager.b_month_tie_count + "";
                    b_win_per_text.text = get_win_percent(data_manager.b_month_win_count, data_manager.b_month_lose_count, data_manager.b_month_tie_count);

                    w_play_text.text = data_manager.w_month_win_count + data_manager.w_month_lose_count + data_manager.w_month_tie_count + "";
                    w_win_text.text = data_manager.w_month_win_count + "";
                    w_lose_text.text = data_manager.w_month_lose_count + "";
                    w_tie_text.text = data_manager.w_month_tie_count + "";
                    w_win_per_text.text = get_win_percent(data_manager.w_month_win_count, data_manager.w_month_lose_count, data_manager.w_month_tie_count);
                }
                break;

            case 2:
                {
                    b_play_text.text = data_manager.b_day_win_count + data_manager.b_day_lose_count + data_manager.b_day_tie_count + "";
                    b_win_text.text = data_manager.b_day_win_count + "";
                    b_lose_text.text = data_manager.b_day_lose_count + "";
                    b_tie_text.text = data_manager.b_day_tie_count + "";
                    b_win_per_text.text = get_win_percent(data_manager.b_day_win_count, data_manager.b_day_lose_count, data_manager.b_day_tie_count);

                    w_play_text.text = data_manager.w_day_win_count + data_manager.w_day_lose_count + data_manager.w_day_tie_count + "";
                    w_win_text.text = data_manager.w_day_win_count + "";
                    w_lose_text.text = data_manager.w_day_lose_count + "";
                    w_tie_text.text = data_manager.w_day_tie_count + "";
                    w_win_per_text.text = get_win_percent(data_manager.w_day_win_count, data_manager.w_day_lose_count, data_manager.w_day_tie_count);
                }
                break;
        }
    }

    void set_full_play_info()
    {
        if (time_set != 0)
        {
            time_set = 0;
            full_play_button.image.color = new Color32(150, 150, 150, 255);

            month_play_button.image.color = new Color32(255, 255, 255, 255);
            day_play_button.image.color = new Color32(255, 255, 255, 255);

            set_game_play_info_page();
        }
    }

    void set_month_play_info()
    {
        if (time_set != 1)
        {
            time_set = 1;
            month_play_button.image.color = new Color32(150, 150, 150, 255);

            full_play_button.image.color = new Color32(255, 255, 255, 255);
            day_play_button.image.color = new Color32(255, 255, 255, 255);

            set_game_play_info_page();
        }
    }

    void set_day_play_info()
    {
        if (time_set != 2)
        {
            time_set = 2;
            day_play_button.image.color = new Color32(150, 150, 150, 255);

            full_play_button.image.color = new Color32(255, 255, 255, 255);
            month_play_button.image.color = new Color32(255, 255, 255, 255);

            set_game_play_info_page();
        }
    }

    public string get_win_percent(int win, int lose, int tie)
    {
        string value = "";
        if (win + lose != 0)
        {
            float win_count = win;
            float play_count = win + lose + tie;
            float val = Convert.ToSingle(win_count / play_count * 100);
            Debug.Log("get_win_percent value: " + val);
            value = Convert.ToInt32(val) + "";
        }
        else
        {
            value = "0";
        }
        return value;
    }
    #endregion

    #region Tier/Rating Info
    void on_tier_info_page()
    {
        reset_page();
        tier_info_page.SetActive(true);

        tier_info_button.image.sprite = on_click_button;
        game_play_info_button.image.sprite = non_click_button;
        account_info_button.image.sprite = non_click_button;

        set_tier_info_page();
    }

    void set_tier_info_page()
    {
        switch (data_manager.language)
        {
            case 0:
                {
                    now_tier_text.text = "등급: " + Converter.tier_to_string(data_manager.my_tier);
                    now_rating_text.text = "레이팅 점수: " + data_manager.rating_score + "점";

                    TIER next = TierManager.instance.get_next_tier(data_manager.my_tier);
                    more_rating_text.text = "다음 등급까지 " + (TierManager.instance.get_tier_rating(next) - data_manager.rating_score) + "점";
                }
                break;

            case 1:
                {
                    now_tier_text.text = "評価：" + Converter.tier_to_string(data_manager.my_tier);
                    now_rating_text.text = "レーティングスコア：" + data_manager.rating_score + "点";

                    TIER next = TierManager.instance.get_next_tier(data_manager.my_tier);
                    more_rating_text.text = "次の評価まで、" + (TierManager.instance.get_tier_rating(next) - data_manager.rating_score) + "点";
                }
                break;

            case 2:
                {
                    now_tier_text.text = "ranking: " + Converter.tier_to_string(data_manager.my_tier);
                    now_rating_text.text = "Rating Score: " + data_manager.rating_score + "p";

                    TIER next = TierManager.instance.get_next_tier(data_manager.my_tier);
                    more_rating_text.text = (TierManager.instance.get_tier_rating(next) - data_manager.rating_score) + " point to the next level";
                }
                break;

            case 3:
                {
                    now_tier_text.text = "排行：" + Converter.tier_to_string(data_manager.my_tier);
                    now_rating_text.text = "评分：" + data_manager.rating_score + "分";

                    TIER next = TierManager.instance.get_next_tier(data_manager.my_tier);
                    more_rating_text.text = (TierManager.instance.get_tier_rating(next) - data_manager.rating_score) + "分到下一级别";
                }
                break;
        }
    }
    #endregion

    void reset_page()
    {
        account_info_page.SetActive(false);
        game_play_info_page.SetActive(false);
        tier_info_page.SetActive(false);

        account_info_button.image.sprite = non_click_button;
        game_play_info_button.image.sprite = non_click_button;
        tier_info_button.image.sprite = non_click_button;

        time_set = 0;
    }

    void on_account_reset_page()
    {
        account_reset_page.SetActive(true);
    }

    void reset_account()
    {
        data_manager.reset_my_data();
        FirebaseManager.instance.clear_my_data();
        Application.Quit();
    }

    public void on_out_account_page()
    {
        account_out_page.SetActive(true);
    }

    public void close_out_account_page()
    {
        account_out_page.SetActive(false);
    }

    public void out_account()
    {
        data_manager.account_logout();
        Application.Quit();
    }

    void cancle_account_reset_page()
    {
        account_reset_page.SetActive(false);
    }

    void close_account_page()
    {
        account_page.SetActive(false);
        HomeManager.instance.Home();
    }
}
