using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeProfile : MonoBehaviour
{
    public Text name_text;
    public Text tier_text;
    public Text country_text;
    public Text old_text;
    public Text gender_text;
    public Text heart_text;

    public void Set()
    {
        name_text.text = DataManager.instance.my_name;
        tier_text.text = Converter.tier_to_string(DataManager.instance.my_tier);
        country_text.text = Converter.country_to_string(DataManager.instance.my_country);
        old_text.text = Converter.old_to_string(DataManager.instance.my_old);
        gender_text.text = Converter.gender_to_string(DataManager.instance.my_gender);
        heart_text.text = DataManager.instance.my_heart + "";
    }
}
