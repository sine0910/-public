using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayUIManager : SingletonMonobehaviour<ReplayUIManager>
{
    public Transform slot;
    public GameObject point_slot;

    public PointSlot[,] board;
    List<PointSlot> omok_slot_list = new List<PointSlot>();

    Point select_omok_slot;
    bool select;

    Queue<List<string>> waiting_packets;

    public byte player_me_index;
    public PLAYER_TYPE my_player_type;

    public GameObject mark;

    public Button setting_button;
    public Button zoomin_button;
    public Button zoomout_button;

    //검은 돌
    //public GameObject black;
    public Sprite Black;
    //흰 돌
    //public GameObject white;
    public Sprite White;

    public Sprite Double3;
    public Sprite Double4;
    public Sprite Over;

    //public Sprite Nomal;

    public bool game_playing;

    public GameObject game_result;

    public IEnumerator timer;
    public Image timer_image;

    public IEnumerator other_timer;
    public Image other_timer_image;

    public Camera camera;

    public int camera_zoom_step = 0;
    public int camera_size = 640;

    public Dictionary<int, int> camera_zoom_size;

    private float dist;

    public float move_speed;

    Vector3 prev_pos = Vector3.zero;

    public float camera_max_distance_x;
    public float camera_max_distance_y;
    public float camera_min_distance_x;
    public float camera_min_distance_y;

    public int win_count;
    public int lose_count;
    public int tie_count;

    private void Awake()
    {
        Time.timeScale = 1;

        set_camera();
    }

    // Start is called before the first frame update
    void Start()
    {
        waiting_packets = new Queue<List<string>>();

        StartCoroutine(PacketHandler());
    }

    #region DATA RECEIVE/SEND
    public void on_receive(List<string> msg)
    {
        this.waiting_packets.Enqueue(msg);
    }

    IEnumerator PacketHandler()
    {
        while (true)
        {
            if (this.waiting_packets.Count <= 0)
            {
                yield return 0;
                continue;
            }
            List<string> msg = waiting_packets.Dequeue();
            PROTOCOL protocol = Converter.to_protocol(PopAt(msg));
            switch (protocol)
            {
                case PROTOCOL.BEGIN_START:
                    {
                        Debug.Log("GamePlayUI BEGIN_START");

                        player_me_index = Converter.to_byte(PopAt(msg));
                        my_player_type = Converter.to_player_type(PopAt(msg));
                    }
                    break;

                case PROTOCOL.READY_TO_GAME_START:
                    {
                        Debug.Log("GamePlayUI READY_TO_GAME_START");

                        reset();
                    }
                    break;

                case PROTOCOL.START_TURN:
                    {
                        Debug.Log("GamePlayUI START_TURN");

                        select = false;
                        select_omok_slot = null;

                        timer = select_timer();
                        StartCoroutine(timer);
                    }
                    break;

                case PROTOCOL.SELECT_SLOT_RESULT:
                    {
                        Debug.Log("GamePlayUI SELECT_SLOT_RESULT");

                        byte player_index = Converter.to_byte(PopAt(msg));

                        STATE state = Converter.to_state(PopAt(msg));
                        byte select_x = Converter.to_byte(PopAt(msg));
                        byte select_y = Converter.to_byte(PopAt(msg));

                        if (my_player_type == PLAYER_TYPE.BLACK)
                        {
                            set_illegal_move_mark(msg);
                        }

                        set_ston(player_index, state, select_x, select_y);
                    }
                    break;

                case PROTOCOL.SELECT_SLOT_RESULT_ERROR:
                    {
                        Debug.Log("GamePlayUI SELECT_SLOT_RESULT_ERROR");
                    }
                    break;

                case PROTOCOL.GAME_RESULT:
                    {
                        Debug.Log("GamePlayUI GAME_RESULT");

                        game_playing = false;

                        byte winner = Converter.to_byte(PopAt(msg));

                        byte win = byte.MaxValue;

                        if (winner == byte.MaxValue)
                        {
                            win = byte.MaxValue;
                        }
                        else if (winner == player_me_index)
                        {
                            win = 0;
                        }
                        else
                        {
                            win = 1;
                        }

                        game_result.SetActive(true);
                        game_result.GetComponent<ReplayGameResult>().on_result(win);
                    }
                    break;
            }
        }
    }

    IEnumerator select_timer()
    {
        Debug.Log("start_timer");

        float time = 30;
        timer_image.fillAmount = 1;

        while (true)
        {
            time -= Time.deltaTime;
            timer_image.fillAmount = time / 30;

            if (time < 0)
            {
                Debug.Log("TimeOver");
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    void reset()
    {
        game_playing = true;

        destroy_slot_point();
        make_slot_point();

        select = true;

        set_ston_index = 0;

        mark.SetActive(false);
    }

    //플레이어가 선택할 슬롯을 생성한다.
    void make_slot_point()
    {
        board = new PointSlot[15, 15];

        for (byte i = 0; i < 15; i++)
        {
            for (byte j = 0; j < 15; j++)
            {
                PointSlot slot_obj = Instantiate(point_slot).GetComponent<PointSlot>();
                slot_obj.set_point(new Point(i, j));
                slot_obj.transform.parent = slot;
                slot_obj.transform.localScale = new Vector3(1, 1);//오튜브 슬롯 크기 축소 
                slot_obj.transform.localPosition = get_slot_positions(i, j);
                board[i, j] = slot_obj;
                omok_slot_list.Add(slot_obj);
            }
        }
    }

    void destroy_slot_point()
    {
        foreach (PointSlot slot in omok_slot_list)
        {
            Destroy(slot.gameObject);
        }
        omok_slot_list.Clear();
    }

    #region ARRANGMENT STON
    int set_ston_index = 0;
    void set_ston(byte player_index, STATE state, byte x, byte y)
    {
        if (GameSoundManager.instance != null)
        {
            GameSoundManager.instance.put_ston_effect();
        }

        PointSlot point_slot = board[x, y];

        switch (state)
        {
            case STATE.BLACK:
                {
                    point_slot.get_sprite_image(Black);
                    point_slot.set_state(STATE.BLACK);
                }
                break;
            case STATE.WHITE:
                {
                    point_slot.get_sprite_image(White);
                    point_slot.set_state(STATE.WHITE);
                }
                break;
        }

        set_ston_index++;
        point_slot.set_index(set_ston_index);

        if (player_index == player_me_index)
        {
            other_timer = other_select_timer();
            StartCoroutine(other_timer);
        }
        else
        {
            if (other_timer != null)
            {
                StopCoroutine(other_timer);
                other_timer = null;
            }
        }
    }

    IEnumerator other_select_timer()
    {
        Debug.Log("start_other_select_timer");

        float time = 30;
        other_timer_image.fillAmount = 1;

        while (true)
        {
            time -= Time.deltaTime;
            other_timer_image.fillAmount = time / 30;

            if (time < 0)
            {
                Debug.Log("OtherTimeOver");
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region ILLEGAL MOVE MARK
    List<PointSlot> illegal_point_list = new List<PointSlot>();

    void set_illegal_move_mark(List<string> msg)
    {
        reset_illegal_move_mark();

        byte count = Converter.to_byte(PopAt(msg));

        for (byte i = 0; i < count; i++)
        {
            ILLEGAL_MOVE reason = Converter.to_illegal_move(PopAt(msg));
            byte x = Converter.to_byte(PopAt(msg));
            byte y = Converter.to_byte(PopAt(msg));

            PointSlot point_slot = board[x, y];
            point_slot.set_state(STATE.IllegalMove);
            illegal_point_list.Add(point_slot);
            switch (reason)
            {
                case ILLEGAL_MOVE.Double3:
                    {
                        point_slot.get_sprite_image(Double3);
                    }
                    break;
                case ILLEGAL_MOVE.Double4:
                    {
                        point_slot.get_sprite_image(Double4);
                    }
                    break;
                case ILLEGAL_MOVE.Overline:
                    {
                        point_slot.get_sprite_image(Over);
                    }
                    break;
            }
        }
    }

    void reset_illegal_move_mark()
    {
        for (int i = 0; i < illegal_point_list.Count; i++)
        {
            illegal_point_list[i].set_state(STATE.None);
            illegal_point_list[i].get_sprite_image(null);
        }
        illegal_point_list.Clear();
    }
    #endregion

    Vector3 get_slot_positions(int i, int j)
    {
        return new Vector3((i * 46f), (j * 46f));
    }

    public void set_camera()
    {
        camera_zoom_size = new Dictionary<int, int>();
        camera_zoom_size.Add(0, 700);
        camera_zoom_size.Add(1, 640);
        camera_zoom_size.Add(2, 580);
        camera_zoom_size.Add(3, 520);
        camera_zoom_size.Add(4, 460);
        camera_zoom_size.Add(5, 400);

        camera_zoom_step = 4;

        camera_max_distance_x = 350;
        camera_min_distance_x = -350;
        camera_max_distance_y = 350;
        camera_min_distance_y = -350;

        camera_zoom_set();
        camera.transform.position = new Vector3(0, 50, -5);
    }

    public void zoom_in()
    {
        if (camera_zoom_step < 5)
        {
            camera_zoom_step++;
            camera_zoom_set();
        }
    }

    public void zoom_out()
    {
        if (camera_zoom_step > 0)
        {
            camera_zoom_step--;
            camera_zoom_set();
        }
    }

    public void camera_zoom_set()
    {
        camera.orthographicSize = camera_zoom_size[camera_zoom_step];
    }

    public string PopAt(List<string> list)
    {
        string r = list[0];
        list.RemoveAt(0);
        return r;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prev_pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            prev_pos = Camera.main.ScreenToWorldPoint(prev_pos);
            prev_pos.z = camera.transform.position.z;

        }
        else if (Input.GetMouseButton(0))
        {
            var mouse_move = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            mouse_move = Camera.main.ScreenToWorldPoint(mouse_move);
            mouse_move.z = camera.transform.position.z;
            if (camera_max_distance_x > (camera.transform.position - (mouse_move - prev_pos)).x && camera_min_distance_x < (camera.transform.position - (mouse_move - prev_pos)).x
                && camera_max_distance_y > (camera.transform.position - (mouse_move - prev_pos)).y && camera_min_distance_y < (camera.transform.position - (mouse_move - prev_pos)).y)
            {
                camera.transform.position = camera.transform.position - (mouse_move - prev_pos);
            }
        }
    }

    public void on_history_view()
    {
        for (int i = 0; i < omok_slot_list.Count; i++)
        {
            omok_slot_list[i].set_view_text(true);
        }
    }

    public void close_history_view()
    {
        for (int i = 0; i < omok_slot_list.Count; i++)
        {
            omok_slot_list[i].set_view_text(false);
        }
    }
}
