using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour
{
    PLAYER_TYPE player_type;
    Image player_profile;
    Sprite black;
    Sprite white;

    Text name_text;
    Image country_image;
    Text tier_text;

    GameObject heart;
    Text my_heart;

    // Start is called before the first frame update
    void Awake()
    {
        this.black = Resources.Load<Sprite>("Image/ProfileBlack");
        this.white = Resources.Load<Sprite>("Image/ProfileWhite");

        this.name_text = this.transform.Find("NameText").GetComponent<Text>();
        this.country_image = this.transform.Find("CountryImage").GetComponent<Image>();
        this.tier_text = this.transform.Find("TierText").GetComponent<Text>();

        this.player_profile = this.gameObject.GetComponent<Image>();
    }

    public void set_player_data(PLAYER_TYPE type, string name, TIER tier, COUNTRY country)
    {
        this.player_type = type;
        switch (this.player_type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    this.player_profile.sprite = this.black;
                    this.name_text.color = new Color32(255, 255, 255, 255);
                    this.tier_text.color = new Color32(255, 255, 255, 255);
                    break;
                }
            case PLAYER_TYPE.WHITE:
                {
                    this.player_profile.sprite = this.white;
                    this.name_text.color = new Color32(0, 0, 0, 255);
                    this.tier_text.color = new Color32(0, 0, 0, 255);
                    break;
                }
        }

        this.name_text.text = name;
        this.tier_text.text = Converter.tier_to_string(tier);
        this.country_image.sprite = CountryManager.instance.get_country_sprite(country);
    }

    public bool ask_get_heart_profile()
    {
        if (this.transform.Find("Heart").gameObject != null)
        {
            return true;
        }
        return false;
    }

    public void set_player_heart_text()
    {
        this.heart = this.transform.Find("Heart").gameObject;
        this.my_heart = this.transform.Find("Heart/HeartText").GetComponent<Text>();

        switch (this.player_type)
        {
            case PLAYER_TYPE.BLACK:
                {
                    this.my_heart.color = new Color32(255, 255, 255, 255);
                    break;
                }
            case PLAYER_TYPE.WHITE:
                {
                    this.my_heart.color = new Color32(0, 0, 0, 255);
                    break;
                }
        }
        on_player_heart_count();
    }

    void on_player_heart_count()
    {
        this.my_heart.text = DataManager.instance.my_heart.ToString();
    }
}
