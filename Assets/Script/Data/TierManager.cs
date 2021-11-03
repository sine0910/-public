using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TIER : int
{
    NONE,
    PRACTICE,//AI전용

    GRADE_12TH,//12급
    GRADE_11TH,//11급
    GRADE_10TH,//10급
    GRADE_9TH,//9급
    GRADE_8TH,//8급
    GRADE_7TH,//7급
    GRADE_6TH,//6급
    GRADE_5TH,//5급
    GRADE_4TH,//4급
    GRADE_3TH,//3급
    GRADE_2TH,//2급
    GRADE_1TH,//1급

    SUJOL,//초단(수졸)
    YAKWOO,//2단(약우)
    TULYEOK,//3단(투력)
    SOGYO,//4단(소교)
    YONGJI,//5단(용지)
    TONGYU,//6단(통유)
    GUCHE,//7단(구체)
    JWAJO,//8단(좌조)
    RIW//9단(입신)
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

        switch (DataManager.instance.language)
        {
            case 0:
                {
                    tier_update_title_text.text = "등급 상승";
                    tier_update_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "로 등급이 상습하였습니다";
                    rating_score_text.text = DataManager.instance.rating_score + "점";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "다음 등급" + Converter.tier_to_string(next_tier) + "/레이팅 점수" + get_tier_rating(next_tier) + "점";
                }
                break;

            case 1:
                {
                    tier_update_title_text.text = "評価上昇";
                    tier_update_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "で評価が上昇しました";
                    rating_score_text.text = DataManager.instance.rating_score + "点";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "次の評価: " + Converter.tier_to_string(next_tier) + "/レーティングスコア" + get_tier_rating(next_tier) + "点";
                }
                break;

            case 2:
                {
                    tier_update_title_text.text = "Rank up";
                    tier_update_text.text = "Ranked up to " + Converter.tier_to_string(DataManager.instance.my_tier);
                    rating_score_text.text = DataManager.instance.rating_score + "p";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "Next Rank" + Converter.tier_to_string(next_tier) + "/rating score" + get_tier_rating(next_tier) + "p";
                }
                break;

            case 3:
                {
                    tier_update_title_text.text = "升级";
                    tier_update_text.text = "排名高达 " + Converter.tier_to_string(DataManager.instance.my_tier);
                    rating_score_text.text = DataManager.instance.rating_score + "分";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "下一个年级: " + Converter.tier_to_string(next_tier) + "/评分" + get_tier_rating(next_tier) + "分";
                }
                break;
        }

        FirebaseManager.instance.update_my_tier();
    }

    void on_tier_down_panel()
    {
        tier_update_panel.SetActive(true);

        switch (DataManager.instance.language)
        {
            case 0:
                {
                    tier_update_title_text.text = "등급 강등";
                    tier_update_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "로 등급이 강등되었습니다";
                    rating_score_text.text = DataManager.instance.rating_score + "점";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "다음 등급" + Converter.tier_to_string(next_tier) + "/등급 점수" + get_tier_rating(next_tier) + "점";
                }
                break;

            case 1:
                {
                    tier_update_title_text.text = "格下げ";
                    tier_update_text.text = Converter.tier_to_string(DataManager.instance.my_tier) + "で評価が降格されました";
                    rating_score_text.text = DataManager.instance.rating_score + "点";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "次の評価: " + Converter.tier_to_string(next_tier) + "/レーティングスコア" + get_tier_rating(next_tier) + "点";
                }
                break;

            case 2:
                {
                    tier_update_title_text.text = "Rank Relegation";
                    tier_update_text.text = "Rank relegation to  " + Converter.tier_to_string(DataManager.instance.my_tier);
                    rating_score_text.text = DataManager.instance.rating_score + "p";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "Next Rank" + Converter.tier_to_string(next_tier) + "/rating score" + get_tier_rating(next_tier) + "p";
                }
                break;

            case 3:
                {
                    tier_update_title_text.text = "降级";
                    tier_update_text.text = "降级 " + Converter.tier_to_string(DataManager.instance.my_tier);
                    rating_score_text.text = DataManager.instance.rating_score + "分";

                    TIER next_tier = get_next_tier(DataManager.instance.my_tier);
                    next_tier_text.text = "下一个年级: " + Converter.tier_to_string(next_tier) + "/评分" + get_tier_rating(next_tier) + "分";
                }
                break;
        }
    }

    public void close_tier_update_panel()
    {
        tier_update_panel.SetActive(false);
    }

    public TIER get_next_tier(TIER tier)
    {
        if (tier != TIER.RIW)
        {
            return tier += 1;
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
