using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayer
{
    public delegate void SendFn(List<string> msg);
    SendFn send_function;

    public byte player_index { get; private set; }

    public MultiPlayer(byte player_index, SendFn send_function)
    {
        this.player_index = player_index;

        switch (player_index)
        {
            case 0:
                this.send_function = send_function;
                break;

            case 1:
                this.send_function = send_function;
                break;
        }
    }

    public void send(List<string> msg)
    {
        List<string> clone = msg.ToList();
        this.send_function(clone);
    }
}
