using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryButton : MonoBehaviour
{
    GameObject panel;
    Text text;
    Image image;
    COUNTRY country;

    public void set_country(COUNTRY c)
    {
        this.panel = this.gameObject.transform.Find("Panel").gameObject;
        this.text = this.gameObject.transform.Find("Text").GetComponent<Text>();
        this.image = this.gameObject.transform.Find("Image").GetComponent<Image>();

        this.country = c;

        this.text.text = Converter.country_to_string(this.country);
        this.image.sprite = CountryManager.instance.get_country_sprite(this.country);
        this.gameObject.transform.Find("CountryButton").GetComponent<Button>().onClick.AddListener(this.on_click);

        this.set_panel(false);
    }

    void on_click()
    {
        CountryManager.instance.selete_country(this);
    }

    public COUNTRY get_country()
    {
        return this.country;
    }

    public void set_panel(bool b)
    {
        this.panel.SetActive(b);
    }
}
