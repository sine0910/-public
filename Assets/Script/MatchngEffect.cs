using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchngEffect : MonoBehaviour
{
    GameObject my_zone;
    Text my_name;
    Text my_tier;
    Image my_country;

    GameObject other_zone;
    Text other_name;
    Text other_tier;
    Image other_country;

    GameObject back;

    GameObject effect;

    public IEnumerator on_effect(string my_name, TIER my_tier, COUNTRY my_country, string other_name, TIER other_tier, COUNTRY other_country)
    {
        set_object();

        this.my_name.text = my_name;
        this.my_tier.text = Converter.tier_to_string(my_tier);
        this.my_country.sprite = CountryManager.instance.get_country_sprite(my_country);

        this.other_name.text = other_name;
        this.other_tier.text = Converter.tier_to_string(other_tier);
        this.other_country.sprite = CountryManager.instance.get_country_sprite(other_country);

       yield return StartCoroutine(Effect());
    }

    public void set_object()
    {
        DontDestroyOnLoad(this.gameObject);

        this.effect = this.transform.Find("effect").gameObject;
        this.back = this.transform.Find("effect/back").gameObject;

        this.my_zone = this.transform.Find("effect/my_zone").gameObject;
        this.my_name = this.my_zone.transform.Find("NameText").GetComponent<Text>();
        this.my_tier = this.my_zone.transform.Find("TierText").GetComponent<Text>();
        this.my_country = this.my_zone.transform.Find("CountryImage").GetComponent<Image>();

        this.other_zone = this.transform.Find("effect/other_zone").gameObject;
        this.other_name = this.other_zone.transform.Find("NameText").GetComponent<Text>();
        this.other_tier = this.other_zone.transform.Find("TierText").GetComponent<Text>();
        this.other_country = this.other_zone.transform.Find("CountryImage").GetComponent<Image>();

        this.my_zone.transform.localPosition = new Vector3(-Screen.width, 0);
        this.other_zone.transform.localPosition = new Vector3(Screen.width, 0);
    }

    IEnumerator Effect()
    {
        Debug.Log("Matching Effect On");
        yield return StartCoroutine(MoveTo(my_zone, new Vector3(0, 0, 0), 7500));
        yield return StartCoroutine(MoveTo(other_zone, new Vector3(0, 0, 0), 7500));

        yield return new WaitForSecondsRealtime(1f);

        StartCoroutine(MoveTo(my_zone, new Vector3(-Screen.width, 0, 0), 750));
        StartCoroutine(MoveTo(other_zone, new Vector3(Screen.width, 0, 0), 750));
        StartCoroutine(ScaleTo(effect));

        Destroy();
    }

    IEnumerator MoveTo(GameObject obj, Vector3 pos, int speed)
    {
        float time = 0;
        Vector3 wasPos = obj.transform.localPosition;

        while (true)
        {
            time += Time.deltaTime;
            obj.transform.localPosition = Vector3.MoveTowards(wasPos, pos, time * speed);

            if (time >= 0.75f)
            {
                obj.transform.localPosition = pos;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        yield return 0;
    }

    IEnumerator ScaleTo(GameObject a)
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            a.transform.localScale = new Vector3(1 + (1.25f * time), 1 + (1.25f * time));

            if (time >= 1f)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        yield return 0;
    }

    private void Destroy()
    {
        Destroy(this.gameObject, 1f);
    }
}
