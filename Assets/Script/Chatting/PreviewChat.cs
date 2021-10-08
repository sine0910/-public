using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewChat : MonoBehaviour
{
    Text text;

    public void set(string msg)
    {
        this.text = this.transform.Find("Text").GetComponent<Text>();

        if (msg.Length > 20)
        {
            msg = msg.Substring(0, 19);
            msg += "...";
            Debug.Log("PreviewChat msg: " + msg);
        }

        this.text.text = msg;

        StartCoroutine(destroy());
    }

    IEnumerator destroy()
    {
        yield return new WaitForSecondsRealtime(2f);
        Destroy(this.gameObject);
    }
}
