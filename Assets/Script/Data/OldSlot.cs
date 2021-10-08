using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSlot : MonoBehaviour
{
    public delegate void Call(OLD old);
    Call call;

    OLD old;

    public void set(OLD old, Call call)
    {
        this.old = old;
        this.call = call;
    }

    public void on_click()
    {
        this.call(this.old);
    }
}
