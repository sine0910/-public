using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteFriendPopup : MonoBehaviour
{
    string key;
    string f_accountID;
    string f_name;
    COUNTRY f_country;
    TIER f_tier;
    OLD f_old;
    GENDER f_gender;

    Text main_text;
    Text country_text;
    Image country_image;
    Text tier_text;
    Image tier_image;
    Text old_text;
    Text gender_text;
    Image gender_image;
    Text map_text;

    bool ing = false;

    void SetUI()
    {
        this.main_text = this.transform.Find("MainText").GetComponent<Text>();
        this.country_text = this.transform.Find("MainText/CountryValueText").GetComponent<Text>();
        this.country_image = this.transform.Find("MainText/CountryImage").GetComponent<Image>();
        this.tier_text = this.transform.Find("MainText/TierValueText").GetComponent<Text>();
        //this.tier_image = this.transform.Find("MainText/TierImage").GetComponent<Image>();
        this.old_text = this.transform.Find("MainText/OldValueText").GetComponent<Text>();
        this.gender_text = this.transform.Find("MainText/GenderValueText").GetComponent<Text>();
        this.gender_image = this.transform.Find("MainText/GenderImage").GetComponent<Image>();
        //this.map_text = this.transform.Find("MainText/MapValueText").GetComponent<Text>();
    }

    public void set(string key, string account_id, string name, COUNTRY country, TIER tier, OLD old, GENDER gender)
    {
        SetUI();

        this.key = key;
        this.f_accountID = account_id;
        this.f_name = name;
        this.f_country = country;
        this.f_tier = tier;
        this.f_old = old;
        this.f_gender = gender;

        switch (DataManager.instance.language)
        {
            case 0:
                {
                    this.main_text.text = this.f_name + " 님에게\n친구신청을 받았습니다";
                }
                break;
            case 1:
                {
                    this.main_text.text = this.f_name + "さんに友達申請を受けた";
                }
                break;
            case 2:
                {
                    this.main_text.text = "I received a friend request from " + this.f_name;
                }
                break;
            case 3:
                {
                    this.main_text.text = "我收到了来自" + this.f_name + "的好友请求";
                }
                break;
        }

        this.country_text.text = Converter.country_to_string(this.f_country);
        this.country_image.sprite = CountryManager.instance.get_country_sprite(this.f_country);
        this.tier_text.text = Converter.tier_to_string(this.f_tier);
        //this.tier_image.sprite = TierManager.instance.get_tier_sprite(this.f_tier);
        this.old_text.text = Converter.old_to_string(this.f_old);
        this.gender_text.text = Converter.gender_to_string(this.f_gender);
        this.gender_image.sprite = GenderManager.instance.get_gender_sprite(this.f_gender);
    }

    public void accept_friend()
    {
        FriendManager.instance.accept_friend(this.key, this.f_accountID, close_invite_friend);
    }

    public void reject_friend()
    {
        FriendManager.instance.reject_friend(this.key, this.f_accountID, close_invite_friend);
    }

    void close_invite_friend(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    Destroy(this.gameObject);
                }
                break;
            case 3:
                {
                    NetworkManager.Network_Error();
                }
                break;
        }
    }
}
