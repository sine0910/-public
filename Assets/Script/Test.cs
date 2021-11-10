using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    GameObject matchng_effect;

    void Start()
    {
        Time.timeScale = 1;
        this.gameObject.GetComponent<Button>().onClick.AddListener(TestOn);
        matchng_effect = Resources.Load<GameObject>("Prefab/MatchingEffect");
    }

    void TestOn()
    {
        MatchngEffect effect = Instantiate(matchng_effect).GetComponent<MatchngEffect>();
        StartCoroutine(effect.on_effect("A", TIER.GRADE_12TH, COUNTRY.KOREA, "B", TIER.GRADE_12TH, COUNTRY.KOREA));
    }
}
