using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;

public class MultiPlayManager : SingletonMonobehaviour<MultiPlayManager>
{
    ListenerRegistration listener;

    GameManager game_manager;
    MultiSendManager sendManager;

    public bool host = false;

    string game_room_id;
    string other_id;
    string my_id;

    IEnumerator check_coroutine;

    List<PROTOCOL> game_room_protocol_list;
    List<int> game_play_protocol_list;

    public GameObject other_out_popup;
    public GameObject other_pocus_out_popup;
    public GameObject abstension_popup;
    public GameObject other_abstension_popup;

    bool d = true;

    // Start is called before the first frame update
    void Start()
    {
        game_manager = GameManager.instance;
        sendManager = MultiSendManager.instance;

        my_chat_prefab = Resources.Load<GameObject>("Prefab/Chatting/MyChat");
        other_chat_prefab = Resources.Load<GameObject>("Prefab/Chatting/OtherChat");
        preview_chat_prefab = Resources.Load<GameObject>("Prefab/Chatting/PreviewChat");

        game_room_id = game_manager.game_room_id;
        other_id = game_manager.other_account_id;
        my_id = DataManager.instance.accountID;

        host = game_manager.host;

        Debug.Log("MultiPlayManager game room: " + game_room_id + "\nhost: " + host);

        game_room_protocol_list = new List<PROTOCOL>();
        add_game_room_protocol_list();

        game_play_protocol_list = new List<int>();
        add_game_play_protocol_list();

        if (host)
        {
            FirestoreHandleValueChangedForHost();
        }
        else
        {
            FirestoreHandleValueChangedForGuest();
        }

        StartCoroutine(FirestoreHandleValueChangedForChatting());

        MultiSendManager.instance.on_awake(host);
        UIManager.instance.ui_start();

        MultiFriendManager.instance.SetPlayerData(other_id);
    }

    public void ProtocolToGameRoom(string msg)//호스트에게 보내는 메세지
    {
        Debug.Log(game_room_id + " To GameRoom: " + msg + " time: " + DateTime.Now.ToString());
        msg += "/" + TimeStamp.GetUnixTimeStamp();
        DocumentReference docRef = FirebaseManager.instance.firestore.Collection("MultiRoom").Document(game_room_id).Collection("host").Document("protocol");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
                { "value",  msg }
        };
        docRef.SetAsync(user);
    }

    public void ProtocolToHost(string msg)//호스트UI에게 보내는 메세지
    {
        Debug.Log(game_room_id + " To Host: " + msg + " time: " + DateTime.Now.ToString());
        msg += "/" + TimeStamp.GetUnixTimeStamp();
        DocumentReference docRef = FirebaseManager.instance.firestore.Collection("MultiRoom").Document(game_room_id).Collection("host").Document("protocol");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "value",  msg }
        };
        docRef.SetAsync(user);
    }

    public void ProtocolToGuest(string msg)//게스트UI에게 보내는 메세지
    {
        Debug.Log(game_room_id + " To Guest: " + msg + " time: " + DateTime.Now.ToString());
        msg += "/" + TimeStamp.GetUnixTimeStamp();
        DocumentReference docRef = FirebaseManager.instance.firestore.Collection("MultiRoom").Document(game_room_id).Collection("guest").Document("protocol");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "value",  msg }
        };
        docRef.SetAsync(user);
    }

    void FirestoreHandleValueChangedForHost()
    {
        Debug.Log("FirestoreHandleValueChangedForHost");
        listener = FirebaseManager.instance.firestore.Collection("MultiRoom").Document(game_room_id).Collection("host").Document("protocol").Listen(MetadataChanges.Include, snapshot =>
        {
            if (d)
            {
                Debug.Log("FirestoreHandleValueChangedForHost Event");
            }
            if (!snapshot.Metadata.IsFromCache)
            {
                if (snapshot.Exists)
                {
                    //if (check_coroutine != null)
                    //{
                    //    StopCoroutine(check_coroutine);
                    //    check_coroutine = null;
                    //}
                    //check_coroutine = CheckingMessage();
                    //StartCoroutine(check_coroutine);

                    Dictionary<string, object> city = snapshot.ToDictionary();
                    if (d)
                    {
                        Debug.Log("FirestoreHandleValueChangedForHost city.count: " + city.Count);
                    }
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        if (pair.Value != null)
                        {
                            string value = pair.Value.ToString();
                            if (!is_received(value))
                            {
                                List<string> protocol_list = value.Split(new string[] { "/" }, StringSplitOptions.None).ToList();
                                Debug.Log("Receive Packet " + value + "\ncity count: " + city.Count + "\npair key: " + pair.Key);
                                PopAt(protocol_list);
                                if (game_play_protocol_list.Contains(Convert.ToInt32(protocol_list[0])))
                                {
                                    play_packet_handler(protocol_list);
                                }
                                else if (game_room_protocol_list.Contains((PROTOCOL)Convert.ToInt32(protocol_list[0])))
                                {
                                    byte player_index = Convert.ToByte(PopAt(protocol_list));

                                    sendManager.receive_game_room(player_index, protocol_list);
                                }
                                else
                                {
                                    sendManager.receive_ui(protocol_list);
                                }
                            }
                        }
                        else
                        {
                            if (d)
                            {
                                Debug.Log("FirestoreHandleValueChangedForHost pair.Value is null!");
                            }
                        }
                    }
                }
                else
                {
                    if (d)
                    {
                        Debug.Log("FirestoreHandleValueChangedForHost Snapshot is null!");
                    }
                }
            }
            else
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> city = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        if (pair.Value != null)
                        {
                            string value = pair.Value.ToString();
                            Debug.Log("FirestoreHandleValueChangedForHost from Cache! " + value + " time: " + DateTime.Now.ToString());
                        }
                    }
                }
            }
        });
    }

    void FirestoreHandleValueChangedForGuest()
    {
        Debug.Log("FirestoreHandleValueChangedForGuest");
        listener = FirebaseManager.instance.firestore.Collection("MultiRoom").Document(game_room_id).Collection("guest").Document("protocol").Listen(MetadataChanges.Include, snapshot =>
        {
            if (d)
            {
                Debug.Log("FirestoreHandleValueChangedForGuest Event");
            }
            if (!snapshot.Metadata.IsFromCache)
            {
                if (snapshot.Exists)
                {
                    //if (check_coroutine != null)
                    //{
                    //    StopCoroutine(check_coroutine);
                    //    check_coroutine = null;
                    //}
                    //check_coroutine = CheckingMessage();
                    //StartCoroutine(check_coroutine);

                    Dictionary<string, object> city = snapshot.ToDictionary();
                    if (d)
                    {
                        Debug.Log("FirestoreHandleValueChangedForHost city.count: " + city.Count);
                    }
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        if (pair.Value != null)
                        {
                            string value = pair.Value.ToString();

                            if(!is_received(value))
                            {
                                List<string> protocol_list = value.Split(new string[] { "/" }, StringSplitOptions.None).ToList();
                                Debug.Log("Receive Packet " + value + "\ncity count: " + city.Count + "\npair key: " + pair.Key);
                                PopAt(protocol_list);
                                if (game_play_protocol_list.Contains(Convert.ToInt32(protocol_list[0])))
                                {
                                    play_packet_handler(protocol_list);
                                }
                                else
                                {
                                    sendManager.receive_ui(protocol_list);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> city = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        if (pair.Value != null)
                        {
                            string value = pair.Value.ToString();
                            Debug.Log("FirestoreHandleValueChangedForGuest from Cache! " + value + " time: " + DateTime.Now.ToString());
                        }
                    }
                }
            }
        });
    }

    void play_packet_handler(List<string> msg)
    {
        int protocol = Convert.ToInt32(msg[0]);
        switch(protocol)
        {
            case 990:
                {
                    other_pocus_out();
                }
                break;

            case 991:
                {
                    other_pocus_on();
                }
                break;

            case 999:
                {
                    on_other_out_popup();
                }
                break;

            case 777:
                {
                    MultiFriendManager.instance.on_received_friend_request();
                }
                break;

            case 778:
                {
                    MultiFriendManager.instance.accept_friend_invite();
                }
                break;

            case 779:
                {
                    MultiFriendManager.instance.reject_friend_invite();
                }
                break;
        }
    }

    bool error = false;
    public IEnumerator CheckingMessage()
    {
        error = true;
        yield return new WaitForSecondsRealtime(15f);
        //2021-05-29 09:58 15초 동안 아무런 서버 메세지가 없는 경우 에러로 판단
        if (error)
        {
            on_error();
        }
    }

    //사용자 대전 중 에러가 생겼을 경우 호출
    public void on_error()
    {
        //Time.timeScale = 0;

    }

    bool other_out = false;
    IEnumerator checking_pucus_out;
    public void other_pocus_out()
    {
        other_out = false;
        Time.timeScale = 0;
        other_pocus_out_popup.SetActive(true);
        checking_pucus_out = CheckingPocusOutTime();
        StartCoroutine(checking_pucus_out);
    }

    public IEnumerator CheckingPocusOutTime()
    {
        yield return new WaitForSecondsRealtime(10f);
        other_out = true;
        other_pocus_out_popup.SetActive(false);
        on_other_out_popup();
    }

    public void other_pocus_on()
    {
        if (!other_out)
        {
            Time.timeScale = 1;
            StopCoroutine(checking_pucus_out);
            other_pocus_out_popup.SetActive(false);
        }
    }

    public void on_other_out_popup()
    {
        if (UIManager.instance.game_playing)
        {
            on_other_abstension();
        }
        else
        {
            Time.timeScale = 0;
            other_out_popup.SetActive(true);
        }
    }

    public void on_abstension_popup()
    {
        abstension_popup.SetActive(true);
    }

    public void close_abstension_popup()
    {
        abstension_popup.SetActive(false);
    }

    public void on_other_abstension()
    {
        other_abstension_popup.SetActive(true);

        UIManager.instance.abstension_win();
    }

    public void close_other_abstension()
    {
        other_abstension_popup.SetActive(false);
    }

    public void Abstension()
    {
        UIManager.instance.lose_count++;
        DataManager.instance.lose(UIManager.instance.my_player_type);

        QuitGame();
    }

    public void QuitGame()
    {
        Time.timeScale = 1;

        if (host)
        {
            ProtocolToGuest("999");
        }
        else
        {
            ProtocolToHost("999");
        }

        UIManager.instance.save_record();

        SceneManager.LoadScene("HomeScene");
    }

    void add_game_room_protocol_list()
    {
        game_room_protocol_list.Add(PROTOCOL.READY_TO_START);
        game_room_protocol_list.Add(PROTOCOL.GAME_START);
        game_room_protocol_list.Add(PROTOCOL.SELECT_SLOT);
        game_room_protocol_list.Add(PROTOCOL.TURN_END);
        game_room_protocol_list.Add(PROTOCOL.TIME_OUT);
    }

    void add_game_play_protocol_list()
    {
        game_play_protocol_list.Add(990);
        game_play_protocol_list.Add(991);
        game_play_protocol_list.Add(999);

        game_play_protocol_list.Add(777);
        game_play_protocol_list.Add(778);
        game_play_protocol_list.Add(779);
    }

    List<string> before_packet = new List<string>();
    bool is_received(string packet)
    {
        if (!before_packet.Contains(packet))
        {
            Debug.Log("is not received packet! " + packet);
            before_packet.Add(packet);
            return false;
        }
        else
        {
            Debug.Log("is_received packet! " + packet);
            return true;
        }
    }

    public string PopAt(List<string> list)
    {
        string r = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return r;
    }

    #region CHATTING
    public ChatManager chatManager;

    public InputField chatting_input;

    public GameObject chat_panel;

    public Scrollbar scroll_bar;

    GameObject my_chat_prefab;
    GameObject other_chat_prefab;
    GameObject preview_chat_prefab;

    public GameObject preview_panel;
    public Text preview_set_button_text;

    public GameObject chat_alarm;
    public bool on_chat;

    public Queue<Chat> chat_list = new Queue<Chat>();

    public List<string> chat_id_list = new List<string>();

    public void add_emoticon_message(string msg)
    {
        chatting_input.text += msg;
    }

    public void emoticon1()
    {
        add_emoticon_message(Converter.emoticon_to_string((EMOTICON)1));
    }
    public void emoticon2()
    {
        add_emoticon_message(Converter.emoticon_to_string((EMOTICON)2));
    }
    public void emoticon3()
    {
        add_emoticon_message(Converter.emoticon_to_string((EMOTICON)3));
    }
    public void emoticon4()
    {
        add_emoticon_message(Converter.emoticon_to_string((EMOTICON)4));
    }
    public void emoticon5()
    {
        add_emoticon_message(Converter.emoticon_to_string((EMOTICON)5));
    }

    public void send_chattng_message()
    {
        string msg = chatting_input.text;

        if (msg.Trim() != "")
        {
            chatting_input.text = "";

            send_to_chatting(msg);
        }
    }

    public void send_to_chatting(string msg)
    {
        WriteBatch batch = FirebaseManager.instance.firestore.StartBatch();

        DocumentReference my_ref = FirebaseManager.instance.firestore.Collection("MultiChatRoom").Document(my_id + other_id).Collection("chatting").Document(TimeStamp.GetUnixTimeStamp());
        Dictionary<string, object> my_msg = new Dictionary<string, object>
        {
            { "msg",  msg },
            { "player", 0 },
            { "time", TimeStamp.GetUnixTimeStamp() }
        }; 
        batch.Set(my_ref, my_msg);

        DocumentReference other_ref = FirebaseManager.instance.firestore.Collection("MultiChatRoom").Document(other_id + my_id).Collection("chatting").Document(TimeStamp.GetUnixTimeStamp());
        Dictionary<string, object> other_msg = new Dictionary<string, object>
        {
            { "msg",  msg },
            { "player", 1 },
            { "time", TimeStamp.GetUnixTimeStamp() }
        };
        batch.Set(other_ref, other_msg);

        batch.CommitAsync();
    }

    IEnumerator FirestoreHandleValueChangedForChatting()
    {
        Debug.Log("FirestoreHandleValueChangedForChatting");

        bool ready = false;

        FirebaseManager.instance.firestore.Collection("MultiChatRoom").Document(my_id + other_id).Collection("chatting").OrderBy("time").GetSnapshotAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsFaulted || task.IsCanceled)
            {

            }
            else
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (!chat_id_list.Contains(document.Id))
                    {
                        chat_id_list.Add(document.Id);

                        Dictionary<string, object> pairs = document.ToDictionary();

                        byte player_index = Convert.ToByte(pairs["player"].ToString());
                        string msg = pairs["msg"].ToString();

                        chat_list.Enqueue(new Chat(player_index, msg));
                    }
                }
                set_chatting_page();
                ready = true;
            }
        });

        Debug.Log("FirestoreHandleValueChangedForChatting listening ready");
        yield return new WaitUntil(() => ready);
        Debug.Log("FirestoreHandleValueChangedForChatting listening ready true");

        FirebaseManager.instance.firestore.Collection("MultiChatRoom").Document(my_id + other_id).Collection("chatting").Listen(snapshot =>
        {
            if (!snapshot.Metadata.IsFromCache)
            {
                foreach (DocumentChange change in snapshot.GetChanges())
                {
                    if (change.ChangeType == DocumentChange.Type.Added)
                    {
                        if (!chat_id_list.Contains(change.Document.Id))
                        {
                            chat_id_list.Add(change.Document.Id);

                            Dictionary<string, object> pairs = change.Document.ToDictionary();

                            if (pairs.ContainsKey("player") && pairs.ContainsKey("msg"))
                            {
                                byte player_index = Convert.ToByte(pairs["player"].ToString());
                                string msg = pairs["msg"].ToString();

                                chat_list.Enqueue(new Chat(player_index, msg));

                                set_chatting_page();

                                if (!on_chat)
                                {
                                    chat_alarm.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    public void set_chatting_page()
    {
        try
        {
            if (chat_list.Count != 0)
            {
                Chat chat_data = chat_list.Dequeue();

                if (chat_data.player_index == 0)
                {
                    ChatArea chat = Instantiate(my_chat_prefab).GetComponent<ChatArea>();
                    chat.transform.parent = chat_panel.transform.Find("Scroll View/Viewport/Content");
                    chat.transform.localScale = new Vector3(1, 1, 1);
                    chatManager.chat_List.Add(chat);
                    chat.set_msg(chat_data.msg);
                }
                else
                {
                    ChatArea chat = Instantiate(other_chat_prefab).GetComponent<ChatArea>();
                    chat.transform.parent = chat_panel.transform.Find("Scroll View/Viewport/Content");
                    chat.transform.localScale = new Vector3(1, 1, 1);
                    chatManager.chat_List.Add(chat);
                    chat.set_msg(chat_data.msg);

                    if (!on_chat && DataManager.instance.preview == 1)
                    {
                        PreviewChat preview = Instantiate(preview_chat_prefab).GetComponent<PreviewChat>();
                        preview.transform.parent = preview_panel.transform;
                        preview.transform.localScale = new Vector3(1, 1, 1);
                        preview.transform.localPosition = new Vector3(0, 0, 0);
                        preview.set(chat_data.msg);
                    }
                }
                chatManager.ChatSort();
            }

            scroll_to_bottom();
        }
        catch (Exception e)
        {
            Debug.Log("set_chatting_page error: " + e);
        }
    }

    public void scroll_to_bottom()
    {
        Canvas.ForceUpdateCanvases();
        chat_panel.transform.Find("Scroll View/Viewport/Content").GetComponent<VerticalLayoutGroup>().enabled = false;
        chat_panel.transform.Find("Scroll View/Viewport/Content").GetComponent<VerticalLayoutGroup>().enabled = true;

        scroll_bar.value = 0;
    }

    public void on_chatting_page()
    {
        if (chat_panel.activeSelf)
        {
            close_chatting_page();
        }
        else
        {
            on_chat = true;
            chat_alarm.SetActive(false);
            chat_panel.SetActive(true);

            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        if (DataManager.instance.preview == 1)
                        {
                            preview_set_button_text.text = "채팅 미리보기 해제";
                        }
                        else if (DataManager.instance.preview == 2)
                        {
                            preview_set_button_text.text = "채팅 미리보기";
                        }
                    }
                    break;

                case 1:
                    {
                        if (DataManager.instance.preview == 1)
                        {
                            preview_set_button_text.text = "通知を無効に";
                        }
                        else if (DataManager.instance.preview == 2)
                        {
                            preview_set_button_text.text = "チャット通知";
                        }
                    }
                    break;

                case 2:
                    {
                        if (DataManager.instance.preview == 1)
                        {
                            preview_set_button_text.text = "Disable chat popup";
                        }
                        else if (DataManager.instance.preview == 2)
                        {
                            preview_set_button_text.text = "Enable chat popup";
                        }
                    }
                    break;

                case 3:
                    {
                        if (DataManager.instance.preview == 1)
                        {
                            preview_set_button_text.text = "禁用聊天通知";
                        }
                        else if (DataManager.instance.preview == 2)
                        {
                            preview_set_button_text.text = "聊天通知";
                        }
                    }
                    break;
            }
        }
    }

    public void close_chatting_page()
    {
        on_chat = false;
        chatting_input.text = "";
        chat_panel.SetActive(false);
    }

    public void select_preview()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    if (DataManager.instance.preview == 1)
                    {
                        preview_set_button_text.text = "채팅 미리보기 해제";
                    }
                    else if (DataManager.instance.preview == 2)
                    {
                        preview_set_button_text.text = "채팅 미리보기";
                    }
                }
                break;

            case 1:
                {
                    if (DataManager.instance.preview == 1)
                    {
                        preview_set_button_text.text = "通知を無効に";
                    }
                    else if (DataManager.instance.preview == 2)
                    {
                        preview_set_button_text.text = "チャット通知";
                    }
                }
                break;

            case 2:
                {
                    if (DataManager.instance.preview == 1)
                    {
                        preview_set_button_text.text = "Disable chat popup";
                    }
                    else if (DataManager.instance.preview == 2)
                    {
                        preview_set_button_text.text = "Enable chat popup";
                    }
                }
                break;

            case 3:
                {
                    if (DataManager.instance.preview == 1)
                    {
                        preview_set_button_text.text = "禁用聊天通知";
                    }
                    else if (DataManager.instance.preview == 2)
                    {
                        preview_set_button_text.text = "聊天通知";
                    }
                }
                break;
        }

        DataManager.instance.save_preview_data();
    }

    public class Chat
    {
        public byte player_index;
        public string msg;

        public Chat(byte player_index, string msg)
        {
            this.player_index = player_index;
            this.msg = msg;
        }
    }
    #endregion

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (host)
            {
                ProtocolToGuest("990");
            }
            else
            {
                ProtocolToHost("990");
            }
        }
        else
        {
            if (host)
            {
                ProtocolToGuest("991");
            }
            else
            {
                ProtocolToHost("991");
            }
        }
    }

    public void OnApplicationQuit()
    {
        QuitGame();
    }

    public void OnDestroy()
    {
        if (listener != null)
        {
            listener.Stop();
        }
        else
        {
            Debug.Log("MultiPlayManager listener is null");
        }
        Debug.Log("MultiPlayManager Destrory");
    }
}
