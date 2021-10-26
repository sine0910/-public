using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;

public enum EVENT_TYPE : byte
{
    SERVER,
    PERSONAL
}

public enum EVENT_ITEM
{
    HEART,
    OPEN_BOX
}


public class EventManager : SingletonMonobehaviour<EventManager>
{
    bool select = false;

    public GameObject event_page;
    public GameObject event_slot_panel;
    public GameObject  contect_panel;
    public GameObject event_empty_panel;

    Queue<Event> event_list = new Queue<Event>();
    List<string> event_key_list = new List<string>();
    public List<string> server_event_key_list = new List<string>();

    GameObject event_slot;
    List<EventSlot> event_slot_list;

    public GameObject confirm_result_page;
    public Image confirm_result_image;
    public Text confirm_result_text;

    public GameObject ready_all_confirm;
    public GameObject all_confirm_result_page;

    public GameObject all_confirm_heart_slot;
    public Text all_confirm_heart_slot_text;

    public Image alarm;

    int event_slot_index;

    void Start()
    {
        event_slot = Resources.Load("Prefab/EventSlot") as GameObject;

        event_slot_list = new List<EventSlot>();

        server_event_key_list = DataManager.instance.load_event_data();

        FirebaseManager.instance.get_event_list(event_list, event_key_list);
        FirebaseManager.instance.get_server_event_list(event_list, event_key_list, server_event_key_list);

        listening_event_list();
        listening_server_event_list();

        StartCoroutine(set_event_page());
    }

    public void on_event_page()
    {
        panel_contect();

        event_page.SetActive(true);

        if (event_slot_list.Count != 0)
        {
            event_slot_panel.SetActive(true);
            event_empty_panel.SetActive(false);
        }
        else
        {
            event_slot_panel.SetActive(false);
            event_empty_panel.SetActive(true);
        }

        alarm.gameObject.SetActive(false);
    }

    public void close_event_page()
    {
        event_page.SetActive(false);
        HomeManager.instance.Home();
    }

    public void listening_event_list()
    {
        FirebaseManager.instance.firestore.Collection("Users").Document(DataManager.instance.accountID).Collection("Event").Listen(snapshot =>
        {
            if (!snapshot.Metadata.IsFromCache)
            {
                foreach (DocumentChange change in snapshot.GetChanges())
                {
                    if (change.ChangeType.ToString() == "Added")
                    {
                        if (!event_key_list.Contains(change.Document.Id))
                        {
                            event_key_list.Add(change.Document.Id);

                            Dictionary<string, object> data = change.Document.ToDictionary();

                            string main = data["Main"].ToString();
                            EVENT_ITEM reward = Converter.to_item(data["Reward"].ToString());
                            int reward_count = Converter.to_int(data["RewardCount"].ToString());
                            DateTime event_deadline = ((Timestamp)data["Deadline"]).ToDateTime().Add(TimeStamp.time_span);

                            DateTime now_date = DateTime.Now;
                            int compare_val = DateTime.Compare(event_deadline, now_date);
                            if (compare_val > 0)
                            {
                                event_list.Enqueue(new Event(EVENT_TYPE.PERSONAL, change.Document.Id, reward, reward_count, main, event_deadline));
                            }
                            else
                            {
                                FirebaseManager.instance.delete_time_out_event(change.Document.Id, main, reward, reward_count);
                            }
                        }
                    }
                }
            }
        });
    }

    public void listening_server_event_list()
    {
        FirebaseManager.instance.firestore.Collection("ServerEvent").Listen(snapshot =>
        {
            if (!snapshot.Metadata.IsFromCache)
            {
                foreach (DocumentChange change in snapshot.GetChanges())
                {
                    if (change.ChangeType.ToString() == "Added")
                    {
                        if (!server_event_key_list.Contains(change.Document.Id) && !event_key_list.Contains(change.Document.Id))
                        {
                            Dictionary<string, object> data = change.Document.ToDictionary();

                            event_key_list.Add(change.Document.Id);

                            string main = data["Main"].ToString();
                            EVENT_ITEM reward = Converter.to_item(data["Reward"].ToString());
                            int reward_count = Converter.to_int(data["RewardCount"].ToString());
                            DateTime event_deadline = ((Timestamp)data["Deadline"]).ToDateTime().Add(TimeStamp.time_span);

                            DateTime now_date = DateTime.Now;
                            int compare_val = DateTime.Compare(event_deadline, now_date);
                            if (compare_val > 0)
                            {
                                event_list.Enqueue(new Event(EVENT_TYPE.SERVER, change.Document.Id, reward, reward_count, main, event_deadline));
                            }
                            else
                            {
                                FirebaseManager.instance.delete_time_out_event(change.Document.Id, main, reward, reward_count);
                            }
                        }
                    }
                }
            }
        });
    }

    IEnumerator set_event_page()
    {
        while (true)
        {
            if (event_list.Count > 0)
            {
                Event _event = event_list.Dequeue();

                EventSlot clone_event_slot = Instantiate(event_slot).GetComponent<EventSlot>();
                clone_event_slot.gameObject.transform.parent = event_slot_panel.transform.Find("Viewport/Content");
                clone_event_slot.gameObject.transform.localScale = new Vector3(1, 1, 1);
                clone_event_slot.set(_event.event_type, event_slot_list.Count, _event.key, _event.item, _event.count, _event.main, _event.time);
                event_slot_list.Add(clone_event_slot);
                alarm.gameObject.SetActive(true);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void all_comfirm_event()
    {
        if (event_slot_list.Count > 0)
        {
            ready_all_confirm.SetActive(true);
            StartCoroutine(get_all_event_reward());
        }
    }

    IEnumerator get_all_event_reward()
    {
        Dictionary<EVENT_ITEM, int> item_dictionary = new Dictionary<EVENT_ITEM, int>();
        item_dictionary.Add(EVENT_ITEM.HEART, 0);

        for (int i = 0; i < event_slot_list.Count; i++)
        {
            EventSlot event_slot = event_slot_list[i];

            if (event_slot.event_type == EVENT_TYPE.SERVER)
            {
                server_event_key_list.Add(event_slot.key_value);
            }

            EVENT_ITEM item = event_slot.item;
            int item_count = event_slot.item_count;

            switch (item)
            {
                case EVENT_ITEM.HEART:
                    {
                        item_dictionary[item] += item_count;
                    }
                    break;
            }
        }
        DataManager.instance.save_event_get_result_key_data(server_event_key_list);
        yield return StartCoroutine(delete_all_event());
        if (item_dictionary.ContainsKey(EVENT_ITEM.HEART))
        {
            DataManager.instance.my_heart += item_dictionary[EVENT_ITEM.HEART];
            all_confirm_heart_slot.SetActive(true);
            all_confirm_heart_slot_text.text = item_dictionary[EVENT_ITEM.HEART] + "";
        }
        DataManager.instance.save_heart();

        ready_all_confirm.SetActive(false);
        all_confirm_result_page.SetActive(true);
    }

    public void close_all_confirm_result_page()
    {
        all_confirm_heart_slot.SetActive(false);
        all_confirm_result_page.SetActive(false);
    }

    public bool delete_event = false;
    public IEnumerator delete_all_event()
    {
        for (int i = 0; i < event_slot_list.Count; i++)
        {
            delete_event = false;
            Destroy(event_slot_list[i].gameObject);
            panel_contect();

            if (event_slot_list[i].event_type == EVENT_TYPE.PERSONAL)
            {
                FirebaseManager.instance.delete_all_event(event_slot_list[i].key_value,
                   event_slot_list[i].main,
                   event_slot_list[i].item,
                   event_slot_list[i].item_count);
            }
            else if (event_slot_list[i].event_type == EVENT_TYPE.SERVER)
            {
                server_event_key_list.Add(event_slot_list[i].key_value);
                DataManager.instance.save_event_get_result_key_data(server_event_key_list);

                delete_event = true;
                //FirebaseManager.instance.send_get_event_history(event_slot_list[i].key_value,
                //   event_slot_list[i].main,
                //   event_slot_list[i].item,
                //   event_slot_list[i].item_count,
                //   "GET_EVENT_REWARD");
            }

            yield return new WaitUntil(() => delete_event);
        }
        event_slot_list.Clear();
    }

    public void select_this_event(EVENT_TYPE type, EventSlot slot_index, string key)
    {
        if (!select)
        {
            select = true;
            event_slot_index = event_slot_list.FindIndex(x => x == slot_index);
            delete_this_event();
        }
    }

    public void delete_this_event()
    {
        if (event_slot_list[event_slot_index].event_type == EVENT_TYPE.PERSONAL)
        {
            FirebaseManager.instance.delete_this_event(event_slot_list[event_slot_index].key_value,
                event_slot_list[event_slot_index].main,
                event_slot_list[event_slot_index].item,
                event_slot_list[event_slot_index].item_count,
                delete_event_result);
        }
        else if (event_slot_list[event_slot_index].event_type == EVENT_TYPE.SERVER)
        {
            server_event_key_list.Add(event_slot_list[event_slot_index].key_value);
            DataManager.instance.save_event_get_result_key_data(server_event_key_list);

            //FirebaseManager.instance.send_get_event_history(event_slot_list[event_slot_index].key_value,
            //    event_slot_list[event_slot_index].main,
            //    event_slot_list[event_slot_index].item,
            //    event_slot_list[event_slot_index].item_count,
            //    "GET_EVENT_REWARD");

            get_reward();
        }
    }

    public void delete_event_result(byte result)
    {
        switch (result)
        {
            case 1:
                {
                    get_reward();
                }
                break;
            case 3:
                {
                    select = false;
                    NetworkManager.Network_Error();
                }
                break;
        }
    }

    public void get_reward()
    {
        EVENT_ITEM item = event_slot_list[event_slot_index].item;
        int count = event_slot_list[event_slot_index].item_count;
        switch (item)
        {
            case EVENT_ITEM.HEART:
                {
                    DataManager.instance.my_heart += count;
                    confirm_result_image.sprite = Resources.Load<Sprite>("Image/heart");
                    switch (DataManager.instance.language)
                    {
                        case 0:
                            {
                                confirm_result_text.text = "하트 " + count + "개를\n획득하였습니다";
                            }
                            break;
                        case 1:
                            {
                                confirm_result_text.text = "ハート" + count + "個を獲得しました";
                            }
                            break;
                        case 2:
                            {
                                confirm_result_text.text = "You have earned " + count + "heart.";
                            }
                            break;
                        case 3:
                            {
                                confirm_result_text.text = "你赢得了" + count + "颗心。";
                            }
                            break;
                    }
                }
                break;
            //case EVENT_ITEM.KEY:
            //    {
            //        DataManager.instance.my_key += (int)count;
            //        confirm_result_image.sprite = Resources.Load<Sprite>("Image/GoldKey");
            //        confirm_result_text.text = "황금열쇠 " + MoneyManager.instance.convert_money_to_string(count) + "개를 획득하였습니다";
            //    }
            //    break;
        }
        DataManager.instance.save_heart();

        Destroy(event_slot_list[event_slot_index].gameObject);
        event_slot_list.RemoveAt(event_slot_index);

        event_slot_index = 0;

        select = true;
        confirm_result_page.SetActive(true);
    }

    public void close_confirm_result()
    {
        select = false;
        panel_contect();
        TierManager.instance.update_tier_check();
        confirm_result_page.SetActive(false);
    }

    public void panel_contect()
    {
        contect_panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 240 - ((event_slot_list.Count * 120) / 2));
    }
}

public class Event
{
    public EVENT_TYPE event_type;
    public string key;
    public EVENT_ITEM item;
    public int count;
    public string main;
    public DateTime time;

    public Event(EVENT_TYPE event_type, string key, EVENT_ITEM item, int count, string main, DateTime time)
    {
        this.event_type = event_type;
        this.key = key;
        this.item = item;
        this.count = count;
        this.main = main;
        this.time = time;
    }
}
