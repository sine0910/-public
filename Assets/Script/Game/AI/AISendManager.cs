using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISendManager : SingletonMonobehaviour<AISendManager>
{
    static AIGameRoom gameRoom;
    static UIManager gameUI;

    public void on_awake()
    {
        Debug.Log("AISendManager Start");
        gameRoom = new AIGameRoom();
    }

    public void on_start(UIManager ui)
    {
        Debug.Log("AISendManager on start");
        gameUI = ui;
        gameUI.set_send_function(send_from_player);
        gameRoom.on_ready_to_start();
    }

    public static void send_from_ai(List<string> msg, byte player_index)
    {
        Debug.Log("send_from_ai " + msg);
        gameRoom.on_receive(1, msg);
    }

    public static void send_from_player(List<string> msg, byte player_index)
    {
        Debug.Log("send_from_player " + msg);
        gameRoom.on_receive(0, msg);
    }

    public static void send_to_ui(List<string> msg)
    {
        Debug.Log("send_to_ui " + msg);
        gameUI.on_receive(msg);
    }
}
