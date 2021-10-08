using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TIER : int
{
    NONE,
    PRACTICE,//AI����

    GRADE_12TH,//12��
    GRADE_11TH,//11��
    GRADE_10TH,//10��
    GRADE_9TH,//9��
    GRADE_8TH,//8��
    GRADE_7TH,//7��
    GRADE_6TH,//6��
    GRADE_5TH,//5��
    GRADE_4TH,//4��
    GRADE_3TH,//3��
    GRADE_2TH,//2��
    GRADE_1TH,//1��

    SUJOL,//�ʴ�(����)
    YAKWOO,//2��(���)
    TULYEOK,//3��(����)
    SOGYO,//4��(�ұ�)
    YONGJI,//5��(����)
    TONGYU,//6��(����)
    GUCHE,//7��(��ü)
    JWAJO,//8��(����)
    RIW//9��(�Խ�)
}

public class TierManager : SingletonMonobehaviour<TierManager>
{
    public GameObject tier_update_panel;
    public Text tier_update_title_text;
    public Text tier_update_text;
    public Text tier_update_ex_text;
    public Text rating_score_text;
    public Text next_tier_text;

    public Dictionary<TIER, int> tier_rating;

    public void Start()
    {
        tier_rating = new Dictionary<TIER, int>();

        tier_rating.Add(TIER.GRADE_12TH, 10);
        tier_rating.Add(TIER.GRADE_11TH, 20);
        tier_rating.Add(TIER.GRADE_10TH, 40);
        tier_rating.Add(TIER.GRADE_9TH, 80);
        tier_rating.Add(TIER.GRADE_8TH, 150);
        tier_rating.Add(TIER.GRADE_7TH, 250);
        tier_rating.Add(TIER.GRADE_6TH, 400);
        tier_rating.Add(TIER.GRADE_5TH, 650);
        tier_rating.Add(TIER.GRADE_4TH, 700);
        tier_rating.Add(TIER.GRADE_3TH, 950);
        tier_rating.Add(TIER.GRADE_2TH, 1000);
        tier_rating.Add(TIER.GRADE_1TH, 1200);

        tier_rating.Add(TIER.SUJOL, 1400);
        tier_rating.Add(TIER.YAKWOO, 1600);
        tier_rating.Add(TIER.TULYEOK, 1800);
        tier_rating.Add(TIER.SOGYO, 2000);
        tier_rating.Add(TIER.YONGJI, 2200);
        tier_rating.Add(TIER.TONGYU, 2400);
        tier_rating.Add(TIER.GUCHE, 2600);
        tier_rating.Add(TIER.JWAJO, 3000);
        tier_rating.Add(TIER.RIW, 3500);

        tier_rating.Add(TIER.NONE, 0);
    }

    public TIER tier_check()
    {
        if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_11TH])
        {
            return TIER.GRADE_12TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_10TH])
        {
            return TIER.GRADE_11TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_9TH])
        {
            return TIER.GRADE_10TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_8TH])
        {
            return TIER.GRADE_9TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_7TH])
        {
            return TIER.GRADE_8TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_6TH])
        {
            return TIER.GRADE_7TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_5TH])
        {
            return TIER.GRADE_6TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_4TH])
        {
            return TIER.GRADE_5TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_3TH])
        {
            return TIER.GRADE_4TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_2TH])
        {
            return TIER.GRADE_3TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GRADE_1TH])
        {
            return TIER.GRADE_2TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.SUJOL])
        {
            return TIER.GRADE_1TH;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.YAKWOO])
        {
            return TIER.SUJOL;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.TULYEOK])
        {
            return TIER.YAKWOO;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.SOGYO])
        {
            return TIER.TULYEOK;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.YONGJI])
        {
            return TIER.SOGYO;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.TONGYU])
        {
            return TIER.YONGJI;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.GUCHE])
        {
            return TIER.TONGYU;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.JWAJO])
        {
            return TIER.GUCHE;
        }
        else if (DataManager.instance.rating_score < tier_rating[TIER.RIW])
        {
            return TIER.JWAJO;
        }
        else
        {
            return TIER.RIW;
        }
    }

    public void update_tier_check()
    {
        TIER before_tier = DataManager.instance.my_tier;
        TIER after_tier = tier_check();

        if (before_tier != after_tier)
        {
            DataManager.instance.save_my_tier_data(after_tier);

            if (after_tier != TIER.GRADE_12TH)
            {
                if (before_tier < after_tier)
                {
                    on_tier_up_panel();
                }
                else if (before_tier > after_tier)
                {
                    on_tier_down_panel();
                }
            }
        }
    }

    void on_tier_up_panel()
    {
        tier_update_panel.SetActive(true);
        tier_update_title_text.text = "��� ���";
        tier_update_ex_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "�� ����� ����Ͽ����ϴ�";
        rating_score_text.text = DataManager.instance.rating_score + "��";

        TIER next_tier = get_next_tier(DataManager.instance.my_tier);
        next_tier_text.text = "���� ���" + Converter.tier_to_string(next_tier) + "/��� ����" + get_tier_rating(next_tier) + "��";
    }

    void on_tier_down_panel()
    {
        tier_update_panel.SetActive(true);
        tier_update_title_text.text = "��� ����";
        tier_update_ex_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "�� ����� ����Ǿ����ϴ�";
        rating_score_text.text = DataManager.instance.rating_score + "��";

        TIER next_tier = get_next_tier(DataManager.instance.my_tier);
        next_tier_text.text = "���� ���" + Converter.tier_to_string(next_tier) + "/��� ����" + get_tier_rating(next_tier) + "��";
    }

    public void close_tier_update_panel()
    {
        tier_update_panel.SetActive(false);
    }

    public TIER get_next_tier(TIER tier)
    {
        if (tier != TIER.RIW)
        {
            return DataManager.instance.my_tier += 1;
        }
        else
        {
            return TIER.NONE;
        }
    }

    public int get_tier_rating(TIER tier)
    {
        return tier_rating[tier];
    }
}
