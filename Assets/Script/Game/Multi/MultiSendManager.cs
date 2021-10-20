using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSendManager : SingletonMonobehaviour<MultiSendManager>
{
    MultiPlayManager playManager;
    MultiGameRoom gameRoom;
    UIManager gameUI;

    bool host = false;

    public void on_awake(bool host_value)
    {
        if (host_value)
        {
            host = true;
            gameRoom = new MultiGameRoom();
        }

        playManager = MultiPlayManager.instance;
    }

    public void on_start(UIManager ui)
    {
        Debug.Log("AISendManager on start");
        gameUI = ui;
        gameUI.set_send_function(send_to_game_room);

        if (GameManager.instance.host)
        {
            host = true;
            gameRoom.on_ready_to_start();
        }
    }

    public void send_to_host(List<string> msg)
    {
        Debug.Log("send_to_host_ui " + (PROTOCOL)Convert.ToInt32(msg[0]));
        //string s_msg = "";
        //for (int i = 0; i < msg.Count; i++)
        //{
        //    s_msg += msg[i];
        //    if (i < msg.Count - 1)
        //    {
        //        s_msg += "/";
        //    }
        //}
        //playManager.ProtocolToHost(s_msg);

        List<string> clone = msg.ToList();
        receive_ui(clone);
    }

    public void send_to_guest(List<string> msg)
    {
        Debug.Log("send_to_guest_ui " + (PROTOCOL)Convert.ToInt32(msg[0]));
        string s_msg = "";
        for (int i = 0; i < msg.Count; i++)
        {
            s_msg += msg[i];
            if (i < msg.Count - 1)
            {
                s_msg += "/";
            }
        }
        playManager.ProtocolToGuest(s_msg);
    }

    public void send_to_game_room(List<string> msg, byte player_index)
    {
        if (host)
        {
            List<string> clone = msg.ToList();
            receive_game_room(player_index, clone);
        }
        else
        {

            Debug.Log("send_to_host " + msg);
            string s_msg = "";
            for (int i = 0; i < msg.Count; i++)
            {
                s_msg += msg[i] + "/";
            }
            s_msg += player_index;
            playManager.ProtocolToGameRoom(s_msg);
        }
    }

    public void receive_game_room(byte player, List<string> msg)
    {
        //Debug.Log("receive_game_room " + (PROTOCOL)Convert.ToInt32(msg[0]));
        List<string> clone = msg.ToList();
        gameRoom.on_receive(player, clone);
    }

    public void receive_ui(List<string> msg)
    {
        //Debug.Log("receive_ui " + (PROTOCOL)Convert.ToInt32(msg[0]));
        //RecordManager.instance.save_record(msg);
        Recorder.instance.save_record(msg);

        gameUI.on_receive(msg);
    }

    private void OnDestroy()
    {
        gameRoom = null;
        Debug.Log("MultiSendManager Destroy!");
    }
}
