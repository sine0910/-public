using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public byte x { get; private set; }
    public byte y { get; private set; }

    public STATE state;

    public Point(byte x, byte y)
    {
        this.x = x;
        this.y = y;

        state = STATE.None;
    }

    public bool is_same_point(byte x, byte y)
    {
        return this.x == x && this.y == y;
    }

    public void set_state(STATE state)
    {
        this.state = state;
    }
}
