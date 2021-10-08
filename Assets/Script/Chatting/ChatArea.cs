using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatArea : MonoBehaviour
{
    public RectTransform AreaRect, BoxRect, TextRect;
    public Text msg_text;

    public void set_msg(string _msg)
    {
        this.msg_text.text = _msg;
    }
}
