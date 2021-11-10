using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiFriendManager : SingletonMonobehaviour<MultiFriendManager>
{
    GameObject friend_request_info;
    Text friend_info_text;

    GameObject friend_reqeust_popup;

    string other_id;

    // Start is called before the first frame update
    void Start()
    {
        friend_request_info = this.transform.Find("GameCanvas/friend_request_info").gameObject;
        friend_info_text = friend_request_info.transform.Find("background/Text").GetComponent<Text>();
        friend_request_info.transform.Find("background/Button").GetComponent<Button>().onClick.AddListener(close_info_popup);

        friend_reqeust_popup = this.transform.Find("GameCanvas/friend_request_popup").gameObject;
        friend_reqeust_popup.transform.Find("background/AcceptButton").GetComponent<Button>().onClick.AddListener(AcceptFriend);
        friend_reqeust_popup.transform.Find("background/RejectButton").GetComponent<Button>().onClick.AddListener(RejectFriend);
    }

    public void SetPlayerData(string other_id)
    {
        this.other_id = other_id;
    }

    bool invite = false;

    public void InviteFriend()
    {
        if (invite)
        {
            switch (DataManager.instance.language)
            {
                case 0:
                    {
                        friend_info_text.text = "친구 신청은 1번만 할 수 있습니다.";
                    }
                    break;
                case 1:
                    {
                        friend_info_text.text = "友達の申請は1回しかできません。";
                    }
                    break;
                case 2:
                    {
                        friend_info_text.text = "You can only request a friend once.";
                    }
                    break;
                case 3:
                    {
                        friend_info_text.text = "您只能请求一个朋友一次。";
                    }
                    break;
            }
            friend_request_info.SetActive(true);
            return;
        }
        
        if (!DataManager.instance.my_friend_id_list.Contains(other_id))
        {
            if (DataManager.instance.my_friend_id_list.Count < 10)
            {
                invite = true;

                if (MultiPlayManager.instance.host)
                {
                    MultiPlayManager.instance.ProtocolToGuest("777");
                }
                else
                {
                    MultiPlayManager.instance.ProtocolToHost("777");
                }
            }
            else
            {
                over_friend();
            }
        }
        else
        {
            already_friend();
        }
    }

    public void on_received_friend_request()
    {
        friend_reqeust_popup.SetActive(true);
    }

    public void close_received_friend_request()
    {
        friend_reqeust_popup.SetActive(false);
    }

    bool select = false;
    public void AcceptFriend()
    {
        if (select)
        {
            return;
        }

        if (DataManager.instance.my_friend_id_list.Count < 10)
        {
            select = true;

            if (MultiPlayManager.instance.host)
            {
                MultiPlayManager.instance.ProtocolToGuest("778");
            }
            else
            {
                MultiPlayManager.instance.ProtocolToHost("778");
            }
            close_received_friend_request();
            FirebaseManager.instance.multi_accept_invite_friend(other_id);
        }
        else
        {
            over_friend();
            RejectFriend();
        }
    }

    public void RejectFriend()
    {
        if (select)
        {
            return;
        }

        select = false;

        if (MultiPlayManager.instance.host)
        {
            MultiPlayManager.instance.ProtocolToGuest("779");
        }
        else
        {
            MultiPlayManager.instance.ProtocolToHost("779");
        }
        close_received_friend_request();
    }

    public void over_friend()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    friend_info_text.text = "더 이상 친구를 추가할 수 없습니다.";
                }
                break;
            case 1:
                {
                    friend_info_text.text = "もう友達を追加できません。";
                }
                break;
            case 2:
                {
                    friend_info_text.text = "Can not more add friends.";
                }
                break;
            case 3:
                {
                    friend_info_text.text = "您无法再添加好友。";
                }
                break;
        }

        friend_request_info.SetActive(true);
    }

    public void already_friend()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    friend_info_text.text = "이미 친구인 상대에게는 친구신청을 할 수 없습니다";
                }
                break;
            case 1:
                {
                    friend_info_text.text = "すでに友人の相手には友達申請をすることはできません。";
                }
                break;
            case 2:
                {
                    friend_info_text.text = "You cannot request a friend from someone who is already a friend";
                }
                break;
            case 3:
                {
                    friend_info_text.text = "您不能向已经是好友的人提出好友请求。";
                }
                break;
        }

        friend_request_info.SetActive(true);
    }

    public void reject_friend_invite()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    friend_info_text.text = "친구 신청이 거절되었습니다.";
                }
                break;
            case 1:
                {
                    friend_info_text.text = "友達の申請が拒否されました。";
                }
                break;
            case 2:
                {
                    friend_info_text.text = "Your friend request has been rejected.";
                }
                break;
            case 3:
                {
                    friend_info_text.text = "您的好友请求已被拒绝。";
                }
                break;
        }
        friend_request_info.SetActive(true);
    }

    public void accept_friend_invite()
    {
        switch (DataManager.instance.language)
        {
            case 0:
                {
                    friend_info_text.text = "친구 신청이 수락되었습니다.";
                }
                break;
            case 1:
                {
                    friend_info_text.text = "友達申請が受け入れられました。";
                }
                break;
            case 2:
                {
                    friend_info_text.text = "Your friend request has been accepted.";
                }
                break;
            case 3:
                {
                    friend_info_text.text = "您的好友请求已被接受。";
                }
                break;
        }
        friend_request_info.SetActive(true);
    }

    public void close_info_popup()
    {
        friend_request_info.SetActive(false);
    }
}
