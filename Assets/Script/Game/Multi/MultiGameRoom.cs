using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameRoom
{
    Dictionary<byte, PROTOCOL> received_protocol;

    public OmokEngine engine;
    public List<MultiPlayer> players;

    public MultiGameRoom()
    {
        this.engine = new OmokEngine();
        this.players = new List<MultiPlayer>();
        for (byte i = 0; i < 2; ++i)
        {
            switch (i)
            {
                case 0:
                    this.players.Add(new MultiPlayer(i, MultiSendManager.instance.send_to_host));
                    break;
                case 1:
                    this.players.Add(new MultiPlayer(i, MultiSendManager.instance.send_to_guest));
                    break;
            }
        }
        this.received_protocol = new Dictionary<byte, PROTOCOL>();
    }

    public void on_ready_to_start()
    {
        for (int i = 0; i < this.players.Count; i++)
        {
            List<string> msg = new List<string>();
            msg.Add((byte)PROTOCOL.BEGIN_START + "");
            msg.Add(i + "");
            msg.Add((byte)this.engine.get_player_type((byte)i) + "");
            this.players[i].send(msg);
        }
    }

    bool is_received(byte player_index, PROTOCOL protocol)
    {
        if (!this.received_protocol.ContainsKey(player_index))
        {
            return false;
        }
        return this.received_protocol[player_index] == protocol;
    }

    void checked_protocol(byte player_index, PROTOCOL protocol)
    {
        if (this.received_protocol.ContainsKey(player_index))
        {
            return;
        }

        if (this.received_protocol.ContainsKey(player_index))
        {
            if (this.received_protocol[player_index] == protocol)
            {
                Debug.Log("this.received_protocol");
                return;
            }
        }
        this.received_protocol.Add(player_index, protocol);
    }

    bool all_received(PROTOCOL protocol)
    {
        if (this.received_protocol.Count < this.players.Count)
        {
            Debug.Log("not all received" + this.received_protocol.Count + " < " + this.players.Count);
            return false;
        }

        foreach (KeyValuePair<byte, PROTOCOL> kvp in this.received_protocol)
        {
            if (kvp.Value != protocol)
            {
                Debug.Log("not all received" + kvp.Value + " != " + protocol);
                return false;
            }
        }
        clear_received_protocol();
        return true;
    }

    void clear_received_protocol()
    {
        this.received_protocol.Clear();
    }

    public void on_receive(byte player_index, List<string> msg_list)
    {
        PROTOCOL protocol = (PROTOCOL)Converter.to_int(PopAt(msg_list));
        Debug.Log("MultiGameRoom on_receive protocol: " + protocol + "\nfrom player: " + player_index);
        if (is_received(player_index, protocol))
        {
            Debug.Log("MultiGameRoom is_received protocol: " + protocol + "\nfrom player: " + player_index);
            return;
        }

        checked_protocol(player_index, protocol);

        switch (protocol)
        {
            case PROTOCOL.READY_TO_START:
                {
                    if (all_received(protocol))
                    {
                        clear_received_protocol();

                        for (byte i = 0; i < players.Count; i++)
                        {
                            send_begin_start(i);
                        }
                    }
                }
                break;

            case PROTOCOL.GAME_START:
                {
                    Debug.Log("GAME_START");
                    if (all_received(protocol))
                    {
                        clear_received_protocol();

                        engine.reset();

                        byte current_player = this.engine.get_current_player();
                        List<string> send_msg = new List<string> { (byte)PROTOCOL.START_TURN + "" };
                        players[current_player].send(send_msg);
                    }
                }
                break;

            case PROTOCOL.SELECT_SLOT:
                {
                    get_player_select_point(player_index, msg_list);
                }
                break;

            case PROTOCOL.TURN_END:
                {
                    clear_received_protocol();

                    if (this.engine.game_over)
                    {
                        clear_received_protocol();

                        List<string> send_msg = new List<string>();
                        send_msg.Add((byte)PROTOCOL.GAME_RESULT + "");
                        send_msg.Add(this.engine.winner + "");

                        for (int i = 0; i < players.Count; i++)
                        {
                            this.players[i].send(send_msg);
                        }
                    }
                    else
                    {
                        byte next_player = this.engine.get_next_player();
                        List<string> send_msg = new List<string> { (byte)PROTOCOL.START_TURN + "" };
                        players[next_player].send(send_msg);
                    }
                }
                break;

            case PROTOCOL.TIME_OUT:
                {
                    clear_received_protocol();

                    this.engine.winner = this.engine.get_next_player();

                    List<string> send_msg = new List<string>();
                    send_msg.Add((byte)PROTOCOL.GAME_RESULT + "");
                    send_msg.Add(this.engine.winner + "");

                    for (int i = 0; i < players.Count; i++)
                    {
                        this.players[i].send(send_msg);
                    }
                }
                break;
        }
    }

    void send_begin_start(byte player_index)
    {
        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.READY_TO_GAME_START + "");
        players[player_index].send(send_msg);
    }

    void get_player_select_point(byte player_index, List<string> msg)
    {
        byte x = Converter.to_byte(PopAt(msg));
        byte y = Converter.to_byte(PopAt(msg));
        DataController data = this.engine.player_select_point(player_index, x, y);

        if (data == DataController.Error)
        {
            //플레이어가 보낸 데이터과 엔진에서 처리하는 데이터가 동일하지 않음
            Debug.Log("get_player_select_point Error!");

            //TODO 
            //플레이어에게 데이터 동기화 패킷을 보내고 재선택하게 한다
        }
        else
        {
            //UI변경을 위해 처리한 데이터를 보내준다
            for (byte i = 0; i < this.players.Count; i++)
            {
                List<string> send_msg = new List<string>();
                send_msg.Add((byte)PROTOCOL.SELECT_SLOT_RESULT + "");
                send_msg.Add(player_index + "");
                send_msg.Add((byte)this.engine.select_point.state + "");
                send_msg.Add(this.engine.select_point.x + "");
                send_msg.Add(this.engine.select_point.y + "");

                if (this.engine.get_player_type(i) == PLAYER_TYPE.BLACK)
                {
                    //검정 플레이어에게는 금수구역을 전해준다
                    send_illegal_move_point(send_msg);
                }

                this.players[i].send(send_msg);
            }
        }
    }

    void send_illegal_move_point(List<string> msg)
    {
        List<IllegalMove> illegal_move_list = engine.calculate_Illegal_move();
        msg.Add(illegal_move_list.Count + "");

        for (int i = 0; i < illegal_move_list.Count; i++)
        {
            msg.Add((byte)illegal_move_list[i].reason + "");
            msg.Add(illegal_move_list[i].point.x + "");
            msg.Add(illegal_move_list[i].point.y + "");
        }
    }

    public string PopAt(List<string> list)
    {
        string r = list[0];
        list.RemoveAt(0);
        return r;
    }
}