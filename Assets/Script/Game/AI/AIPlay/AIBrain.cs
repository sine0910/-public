using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Threading;

public class AIBrain : MonoBehaviour
{
    delegate void PacketFn(List<string> msg);
    Dictionary<PROTOCOL, PacketFn> packet_handler;

    AIGameRoom game_room;

    int[,] board;
    Point[,] ai_board;
    private Point current_point_being_checked;

    Point best_move;

    public byte player_me_index;
    public PLAYER_TYPE my_player_type;
    public List<Point> point_history;

    public AIBrain(AIGameRoom room)
    {
        this.game_room = room;
        this.packet_handler = new Dictionary<PROTOCOL, PacketFn>();
        this.packet_handler.Add(PROTOCOL.BEGIN_START, BEGIN_START);
        this.packet_handler.Add(PROTOCOL.READY_TO_GAME_START, GAME_START);
        this.packet_handler.Add(PROTOCOL.START_TURN, START_TURN);
        this.packet_handler.Add(PROTOCOL.SELECT_SLOT_RESULT, SELECT_SLOT_RESULT);
        this.packet_handler.Add(PROTOCOL.GAME_RESULT, GAME_RESULT);
        point_history = new List<Point>();
    }

    #region AI 데이터 처리 로직
    public void on_receive(List<string> msg)//데이터 받고
    {
        PROTOCOL protocol = Converter.to_protocol(PopAt(msg));

        if (!this.packet_handler.ContainsKey(protocol))
        {
            return;
        }
        this.packet_handler[protocol](msg);
    }

    void send_message(List<string> msg)
    {
        AISendManager.send_from_ai(msg, player_me_index);
    }

    void BEGIN_START(List<string> msg)
    {
        player_me_index = Converter.to_byte(PopAt(msg));
        my_player_type = Converter.to_player_type(PopAt(msg));

        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.READY_TO_START + "");
        send_message(send_msg);
    }

    void GAME_START(List<string> msg)
    {
        reset();

        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.GAME_START + "");
        send_message(send_msg);
    }

    public ArrayList WHITE_illegal_move_list = new ArrayList();

    void START_TURN(List<string> msg)//ai 턴 시작
    {
        //Thread.Sleep(1000);

        Debug.Log("AiBrain_START_TURN");
        List<IllegalMove> illegal_move_list = calculate_Illegal_move();

        WHITE_illegal_move_list = new ArrayList();
        foreach (IllegalMove illegalMove in illegal_move_list)
        {
            WHITE_illegal_move_list.Add(new Coordinate(illegalMove.point.x, illegalMove.point.y));
            Debug.Log("WHITE_illegal_move_list.Add");
        }

        best_move = get_allow_moves();

        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.SELECT_SLOT + "");
        send_msg.Add(best_move.x + "");
        send_msg.Add(best_move.y + "");
        send_message(send_msg);
    }

    void SELECT_SLOT_RESULT(List<string> msg)
    {
        Debug.Log("AiBrain_SELECT_SLOT_RESULT");
        byte player_index = Converter.to_byte(PopAt(msg));

        STATE state = Converter.to_state(PopAt(msg));
        byte select_x = Converter.to_byte(PopAt(msg));
        byte select_y = Converter.to_byte(PopAt(msg));

        ai_board[select_x, select_y].set_state(state);
        point_history.Add(new Point(select_x, select_y));//백돌의 금수 확인 리스트에 수 기록

        if (my_player_type == PLAYER_TYPE.BLACK)
        {
            //AI가 검정일 경우 AI전용 보드에 금수구역을 지정후 추후 슬롯을 선택할 때 고려하도록 한다
            byte count = Converter.to_byte(PopAt(msg));

            if (count != 0)
            {
                for (byte i = 0; i < count; i++)
                {
                    ILLEGAL_MOVE move_not_reason = Converter.to_illegal_move(PopAt(msg));
                    byte move_not_x = Converter.to_byte(PopAt(msg));
                    byte move_not_y = Converter.to_byte(PopAt(msg));

                    ai_board[move_not_x, move_not_y].set_state(STATE.IllegalMove);
                }
            }
        }

        Coordinate coordinate = new Coordinate(select_x, select_y);
        if (state.ToString() == my_player_type.ToString())
        {
            RegOwnMove(coordinate);
        }
        else
        {
            RegOppMove(coordinate);
        }

        if (player_index == player_me_index)
        {
            List<string> send_msg = new List<string>();
            send_msg.Add((byte)PROTOCOL.TURN_END + "");
            send_message(send_msg);
        }
    }

    void GAME_RESULT(List<string> msg)
    {
        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.READY_TO_START + "");
        send_message(send_msg);
    }

    public string PopAt(List<string> list)
    {
        string r = list[0];
        list.RemoveAt(0);
        return r;
    }
    #endregion

    public void RegOwnMove(Coordinate move)
    {
        board[move.X, move.Y] = 1;
        fieldagent.UpdateInterestingFieldArray(board, move);
        fieldagent.UpdateThreatLists(board, move, 1);
        boardvalue += eval.statVal(board, move, 1, WHITE_illegal_move_list);
    }

    public void RegOppMove(Coordinate move)
    {
        board[move.X, move.Y] = -1;
        fieldagent.UpdateInterestingFieldArray(board, move);
        fieldagent.UpdateThreatLists(board, move, -1);
        move.Val = eval.statVal(board, move, -1, WHITE_illegal_move_list);
        boardvalue += move.Val;
        lastoppmove = move;
    }

    #region AI 선택 로직
    private enum MoveState
    {
        Normal,
        Attack,
    }

    private MoveState state = MoveState.Normal;

    int boardvalue;

    public int maximumsearchdepth = 16;//16

    Coordinate lastoppmove = new Coordinate(-1, -1);
    Coordinate best_field;
    Coordinate tmpbestfield;
    GBThreatSequence threatsequence = null;
    GBThreatSequence lastAppliedThreatSequence = null;

    StatValEvaluator eval;
    InterestingFieldAgent fieldagent;
    ThreatSearcher searcher;

    public StringBuilder statusStr = new StringBuilder();

    Point best_point;

    public static ArrayList non_allow_moves = new ArrayList();

    private Point get_allow_moves()
    {
        //흑돌의 금수를 받아옴
        non_allow_moves = new ArrayList();
        foreach (Point point in ai_board)
        {
            if (point.state == STATE.IllegalMove)
            {
                non_allow_moves.Add(new Coordinate(point.x, point.y));
            }
        }

        best_field = get_move();

        best_point = new Point((byte)best_field.X, (byte)best_field.Y);

        return best_point;
    }

    public Coordinate get_move()
    {
        statusStr = new StringBuilder();

        try
        {
            return (get_move_status());
        }
        catch (Exception ex)
        {
            try
            {
                state = MoveState.Normal;
                best_field = AlphaBeta_move();
            }
            catch (Exception ex2)
            {

            }

            return (best_field);
        }
    }

    //백돌 금수 calculate

    public List<IllegalMove> calculate_Illegal_move()
    {
        List<IllegalMove> illegal_points = new List<IllegalMove>();
        bool[,] potential_moves_already_checked = new bool[15, 15];

        foreach (Point point in point_history)
        {
            byte startingX = (byte)Math.Max(0, point.x - 2);
            byte endingX = (byte)Math.Min(15 - 1, point.x + 2);
            byte startingY = (byte)Math.Max(0, point.y - 2);
            byte endingY = (byte)Math.Min(15 - 1, point.y + 2);

            for (byte x = startingX; x <= endingX; x++)
            {
                for (byte y = startingY; y <= endingY; y++)
                {
                    current_point_being_checked = ai_board[x, y];

                    //이미 확인된 point는 스킵한다
                    if (potential_moves_already_checked[x, y])
                    {
                        continue;
                    }
                    //이미 오목알이 놓여져 있는 point는 스킵한다
                    if (get_point_state(current_point_being_checked) == STATE.BLACK ||
                        get_point_state(current_point_being_checked) == STATE.WHITE)
                    {
                        continue;
                    }

                    if (product_five_win(current_point_being_checked, STATE.WHITE))
                    {
                        //만약 오목을 완성할 수 있는 구간은 금지구역으로 지정하지 않는다
                    }
                    else if (produce_over_line(current_point_being_checked))
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Overline));
                        potential_moves_already_checked[x, y] = true;
                    }
                    else if (count_open_threes(current_point_being_checked) >= 2)
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Double3));
                        potential_moves_already_checked[x, y] = true;
                    }
                    else if (count_open_fours(current_point_being_checked) >= 2)
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Double4));
                        potential_moves_already_checked[x, y] = true;
                    }
                    else
                    {
                        potential_moves_already_checked[x, y] = true;
                    }
                }
            }
        }
        return illegal_points;
    }

    #region Overline
    private bool produce_over_line(Point point)
    {
        foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
        {
            Point fourBefore = get_point_steps_after(point, -4, dir);
            Point threeBefore = get_point_steps_after(point, -3, dir);
            Point twoBefore = get_point_steps_after(point, -2, dir);
            Point oneBefore = get_point_steps_after(point, -1, dir);
            Point oneAfter = get_point_steps_after(point, 1, dir);
            Point twoAfter = get_point_steps_after(point, 2, dir);
            Point threeAfter = get_point_steps_after(point, 3, dir);
            Point fourAfter = get_point_steps_after(point, 4, dir);

            if (get_point_state(oneBefore) == STATE.WHITE &&
                get_point_state(oneAfter) == STATE.WHITE)
            {
                if (get_point_state(fourBefore) == STATE.WHITE &&
                    get_point_state(threeBefore) == STATE.WHITE &&
                    get_point_state(twoBefore) == STATE.WHITE)
                {
                    return true;
                }

                if (get_point_state(threeBefore) == STATE.WHITE &&
                    get_point_state(twoBefore) == STATE.WHITE &&
                    get_point_state(twoAfter) == STATE.WHITE)
                {
                    return true;
                }

                if (get_point_state(twoBefore) == STATE.WHITE &&
                    get_point_state(twoAfter) == STATE.WHITE &&
                    get_point_state(threeAfter) == STATE.WHITE)
                {
                    return true;
                }

                if (get_point_state(twoAfter) == STATE.WHITE &&
                    get_point_state(threeAfter) == STATE.WHITE &&
                    get_point_state(fourAfter) == STATE.WHITE)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 3*3 Logic
    private int count_open_threes(Point point)
    {
        int numOpenThrees = 0;

        foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
        {
            Point threeBefore = get_point_steps_after(point, -3, dir);
            Point twoBefore = get_point_steps_after(point, -2, dir);
            Point oneBefore = get_point_steps_after(point, -1, dir);

            if (isOpenBBNBstartingAt(threeBefore, dir) ||
                isOpenBNBBstartingAt(threeBefore, dir) ||
                isOpenBBBstartingAt(twoBefore, dir) ||
                isOpenBNBBstartingAt(twoBefore, dir) ||

                isOpenBBBstartingAt(oneBefore, dir) ||

                isOpenBBNBstartingAt(oneBefore, dir) ||
                isOpenBBBstartingAt(point, dir) ||
                isOpenBBNBstartingAt(point, dir) ||
                isOpenBNBBstartingAt(point, dir))
            {
                numOpenThrees++;
            }
        }

        return numOpenThrees;
    }

    private bool isOpenBBNBstartingAt(Point point, DIRECTION dir)
    {
        Point oneBefore = get_point_steps_after(point, -1, dir);
        Point oneAfter = get_point_steps_after(point, 1, dir);
        Point twoAfter = get_point_steps_after(point, 2, dir);
        Point threeAfter = get_point_steps_after(point, 3, dir);
        Point fourAfter = get_point_steps_after(point, 4, dir);

        if (get_point_state(oneBefore) == STATE.None &&
           (get_point_state(point) == STATE.WHITE || current_point_being_checked.Equals(point)) &&
           (get_point_state(oneAfter) == STATE.WHITE || current_point_being_checked.Equals(oneAfter)) &&
            get_point_state(twoAfter) == STATE.None &&
           (get_point_state(threeAfter) == STATE.WHITE || current_point_being_checked.Equals(threeAfter)) &&
            get_point_state(fourAfter) == STATE.None)
        {
            return isNoOverlineHazardSurroundingNBBBBNwithFirstBstartingAt(point, dir);
        }

        return false;
    }

    private bool isOpenBNBBstartingAt(Point point, DIRECTION dir)
    {
        Point oneBefore = get_point_steps_after(point, -1, dir);
        Point oneAfter = get_point_steps_after(point, 1, dir);
        Point twoAfter = get_point_steps_after(point, 2, dir);
        Point threeAfter = get_point_steps_after(point, 3, dir);
        Point fourAfter = get_point_steps_after(point, 4, dir);

        if (get_point_state(oneBefore) == STATE.None &&
           (get_point_state(point) == STATE.WHITE || current_point_being_checked.Equals(point)) &&
            get_point_state(oneAfter) == STATE.None &&
           (get_point_state(twoAfter) == STATE.WHITE || current_point_being_checked.Equals(twoAfter)) &&
           (get_point_state(threeAfter) == STATE.WHITE || current_point_being_checked.Equals(threeAfter)) &&
            get_point_state(fourAfter) == STATE.None)
        {
            return isNoOverlineHazardSurroundingNBBBBNwithFirstBstartingAt(point, dir);
        }

        return false;
    }

    private bool isOpenBBBstartingAt(Point point, DIRECTION dir)
    {
        Point oneBefore = get_point_steps_after(point, -1, dir);
        Point twoBefore = get_point_steps_after(point, -2, dir);
        Point oneAfter = get_point_steps_after(point, 1, dir);
        Point twoAfter = get_point_steps_after(point, 2, dir);
        Point threeAfter = get_point_steps_after(point, 3, dir);
        Point fourAfter = get_point_steps_after(point, 4, dir);

        if (get_point_state(oneBefore) == STATE.None &&
           (get_point_state(point) == STATE.WHITE || current_point_being_checked.Equals(point)) &&
           (get_point_state(oneAfter) == STATE.WHITE || current_point_being_checked.Equals(oneAfter)) &&
           (get_point_state(twoAfter) == STATE.WHITE || current_point_being_checked.Equals(twoAfter)) &&
            get_point_state(threeAfter) == STATE.None)
        {
            if (get_point_state(twoBefore) == STATE.None)
            {
                return isNoOverlineHazardSurroundingNBBBBNwithFirstBstartingAt(oneBefore, dir);
            }

            if (get_point_state(fourAfter) == STATE.None)
            {
                return isNoOverlineHazardSurroundingNBBBBNwithFirstBstartingAt(point, dir);
            }

        }

        return false;
    }

    private bool isNoOverlineHazardSurroundingNBBBBNwithFirstBstartingAt(Point point, DIRECTION dir)
    {
        Point twoBefore = get_point_steps_after(point, -2, dir);
        Point fiveAfter = get_point_steps_after(point, 5, dir);

        if (get_point_state(twoBefore) == STATE.WHITE ||
            get_point_state(fiveAfter) == STATE.WHITE)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region 4x4 LOGIC
    private int count_open_fours(Point point)
    {
        int numOpenFours = 0;

        foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
        {
            //may have two in the same direction!!
            for (int i = -4; i <= 0; i++)
            {
                Point iBefore = get_point_steps_after(point, i, dir);
                if (isNotClosedBBBNBorBBNBBorBNBBBstartingAt(iBefore, dir))
                {
                    numOpenFours++;
                }
            }

            //may only have one BBBB in a given direction
            Point threeBefore = get_point_steps_after(point, -3, dir);
            Point twoBefore = get_point_steps_after(point, -2, dir);
            Point oneBefore = get_point_steps_after(point, -1, dir);
            if (isNotClosedBBBBstartingAt(threeBefore, dir) ||
                isNotClosedBBBBstartingAt(twoBefore, dir) ||
                isNotClosedBBBBstartingAt(oneBefore, dir) ||
                isNotClosedBBBBstartingAt(point, dir))
            {
                numOpenFours++;
            }
        }

        return numOpenFours;
    }

    private bool isNotClosedBBBBstartingAt(Point point, DIRECTION dir)
    {
        Point oneBefore = get_point_steps_after(point, -1, dir);
        Point twoBefore = get_point_steps_after(point, -2, dir);
        Point oneAfter = get_point_steps_after(point, 1, dir);
        Point twoAfter = get_point_steps_after(point, 2, dir);
        Point threeAfter = get_point_steps_after(point, 3, dir);
        Point fourAfter = get_point_steps_after(point, 4, dir);
        Point fiveAfter = get_point_steps_after(point, 5, dir);

        if ((get_point_state(point) == STATE.WHITE || current_point_being_checked.Equals(point)) &&
            (get_point_state(oneAfter) == STATE.WHITE || current_point_being_checked.Equals(oneAfter)) &&
            (get_point_state(twoAfter) == STATE.WHITE || current_point_being_checked.Equals(twoAfter)) &&
            (get_point_state(threeAfter) == STATE.WHITE || current_point_being_checked.Equals(threeAfter)))
        {
            if (get_point_state(oneBefore) == STATE.None) //check if open 4 can be made by assuming black piece appended to tail of open 3
            {
                return get_point_state(twoBefore) != STATE.WHITE;
            }

            if (get_point_state(fourAfter) == STATE.None) //check if open 4 can be made by assuming black piece appended to head of open 3
            {
                return get_point_state(fiveAfter) != STATE.WHITE;
            }
        }

        return false;
    }

    private bool isNotClosedBBBNBorBBNBBorBNBBBstartingAt(Point point, DIRECTION dir)
    {
        int numBlackPiecesFound = 0;

        Point oneBefore = get_point_steps_after(point, -1, dir);
        Point oneAfter = get_point_steps_after(point, 1, dir);
        Point twoAfter = get_point_steps_after(point, 2, dir);
        Point threeAfter = get_point_steps_after(point, 3, dir);
        Point fourAfter = get_point_steps_after(point, 4, dir);
        Point fiveAfter = get_point_steps_after(point, 5, dir);

        if (get_point_state(oneBefore) == STATE.WHITE ||
            get_point_state(fiveAfter) == STATE.WHITE)
        {
            return false; //cannot produce 5 in a row due to overline hazard
        }

        if (get_point_state(oneAfter) == STATE.BLACK ||
            get_point_state(twoAfter) == STATE.BLACK ||
            get_point_state(threeAfter) == STATE.BLACK)
        {
            //ensure no white pieces in the middle
            return false;
        }

        if (get_point_state(point) != STATE.WHITE && !current_point_being_checked.Equals(point) ||
            get_point_state(fourAfter) != STATE.WHITE && !current_point_being_checked.Equals(fourAfter))
        {
            //ensure leftmost and rightmost pieces are black
            return false;
        }

        if (get_point_state(oneAfter) == STATE.WHITE || current_point_being_checked.Equals(oneAfter))
        {
            numBlackPiecesFound++;
        }
        if (get_point_state(twoAfter) == STATE.WHITE || current_point_being_checked.Equals(twoAfter))
        {
            numBlackPiecesFound++;
        }
        if (get_point_state(threeAfter) == STATE.WHITE || current_point_being_checked.Equals(threeAfter))
        {
            numBlackPiecesFound++;
        }

        return numBlackPiecesFound == 2;
    }
    #endregion

    public bool product_five_win(Point point, STATE playerColourOccupancyState)
    {
        if (playerColourOccupancyState == STATE.BLACK)
        {
            foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
            {
                Point fiveBefore = get_point_steps_after(point, -5, dir);
                Point fourBefore = get_point_steps_after(point, -4, dir);
                Point threeBefore = get_point_steps_after(point, -3, dir);
                Point twoBefore = get_point_steps_after(point, -2, dir);
                Point oneBefore = get_point_steps_after(point, -1, dir);
                Point oneAfter = get_point_steps_after(point, 1, dir);
                Point twoAfter = get_point_steps_after(point, 2, dir);
                Point threeAfter = get_point_steps_after(point, 3, dir);
                Point fourAfter = get_point_steps_after(point, 4, dir);
                Point fiveAfter = get_point_steps_after(point, 5, dir);

                if (get_point_state(fiveBefore) != playerColourOccupancyState &&
                    get_point_state(fourBefore) == playerColourOccupancyState &&
                    get_point_state(threeBefore) == playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(fourBefore) != playerColourOccupancyState &&
                    get_point_state(threeBefore) == playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(threeBefore) != playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(twoBefore) != playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) == playerColourOccupancyState &&
                    get_point_state(fourAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(oneBefore) != playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) == playerColourOccupancyState &&
                    get_point_state(fourAfter) == playerColourOccupancyState &&
                    get_point_state(fiveAfter) != playerColourOccupancyState)
                {
                    return true;
                }
            }
        }
        else if (playerColourOccupancyState == STATE.WHITE)
        {
            foreach (DIRECTION dir in Enum.GetValues(typeof(DIRECTION)))
            {
                Point fiveBefore = get_point_steps_after(point, -5, dir);
                Point fourBefore = get_point_steps_after(point, -4, dir);
                Point threeBefore = get_point_steps_after(point, -3, dir);
                Point twoBefore = get_point_steps_after(point, -2, dir);
                Point oneBefore = get_point_steps_after(point, -1, dir);
                Point oneAfter = get_point_steps_after(point, 1, dir);
                Point twoAfter = get_point_steps_after(point, 2, dir);
                Point threeAfter = get_point_steps_after(point, 3, dir);
                Point fourAfter = get_point_steps_after(point, 4, dir);
                Point fiveAfter = get_point_steps_after(point, 5, dir);

                if (get_point_state(fiveBefore) != playerColourOccupancyState &&
                    get_point_state(fourBefore) == playerColourOccupancyState &&
                    get_point_state(threeBefore) == playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(fourBefore) != playerColourOccupancyState &&
                    get_point_state(threeBefore) == playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(threeBefore) != playerColourOccupancyState &&
                    get_point_state(twoBefore) == playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(twoBefore) != playerColourOccupancyState &&
                    get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) == playerColourOccupancyState &&
                    get_point_state(fourAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                if (get_point_state(oneBefore) != playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState &&
                    get_point_state(twoAfter) == playerColourOccupancyState &&
                    get_point_state(threeAfter) == playerColourOccupancyState &&
                    get_point_state(fourAfter) == playerColourOccupancyState &&
                    get_point_state(fiveAfter) != playerColourOccupancyState)
                {
                    return true;
                }

                //백돌은 6목으로 승리가 가능하다
                if (get_point_state(oneBefore) == playerColourOccupancyState &&
                    get_point_state(oneAfter) == playerColourOccupancyState)
                {
                    if (get_point_state(fourBefore) == playerColourOccupancyState &&
                        get_point_state(threeBefore) == playerColourOccupancyState &&
                        get_point_state(twoBefore) == playerColourOccupancyState)
                    {
                        return true;
                    }

                    if (get_point_state(threeBefore) == playerColourOccupancyState &&
                        get_point_state(twoBefore) == playerColourOccupancyState &&
                        get_point_state(twoAfter) == playerColourOccupancyState)
                    {
                        return true;
                    }

                    if (get_point_state(twoBefore) == playerColourOccupancyState &&
                        get_point_state(twoAfter) == playerColourOccupancyState &&
                        get_point_state(threeAfter) == playerColourOccupancyState)
                    {
                        return true;
                    }

                    if (get_point_state(twoAfter) == playerColourOccupancyState &&
                        get_point_state(threeAfter) == playerColourOccupancyState &&
                        get_point_state(fourAfter) == playerColourOccupancyState)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public Point get_point_steps_after(Point point, int steps, DIRECTION dir)
    {
        int deltaX = 0;
        int deltaY = 0;

        switch (dir)
        {
            case DIRECTION.S_N:
                deltaX = 0;
                deltaY = 1;
                break;
            case DIRECTION.SW_NE:
                deltaX = 1;
                deltaY = 1;
                break;
            case DIRECTION.W_E:
                deltaX = 1;
                deltaY = 0;
                break;
            case DIRECTION.NW_SE:
                deltaX = 1;
                deltaY = -1;
                break;
        }

        if (point.x + steps * deltaX < 0 || point.y + steps * deltaY < 0 || point.x + steps * deltaX > 14 || point.y + steps * deltaY > 14)
        {
            return new Point((byte)(point.x + steps * deltaX), (byte)(point.y + steps * deltaY));
        }

        return ai_board[point.x + steps * deltaX, point.y + steps * deltaY];
    }

    public Coordinate get_move_status()
    {
        bool getState = true;

        while (getState)
        {
            switch (state)
            {
                case MoveState.Normal:
                    statusStr.Append("Normal, ");
                    getState = get_move_normal();
                    break;
                case MoveState.Attack:
                    statusStr.Append("Attack, ");
                    getState = get_move_attack();
                    break;
            }
        }

        statusStr.AppendFormat("Move ({0},{1})", "abcdefghijklmnopqrstuvwxyz"[best_field.X], best_field.Y);

        return (best_field);
    }

    public STATE get_point_state(Point point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= 15 || point.y >= 15)
        {
            return STATE.OutsideOfBoard;
        }
        return ai_board[point.x, point.y].state;
    }

    //백돌의 금수 확인 end

    private bool get_move_normal()
    {
        Debug.Log("GetMoveNormal");
        Debug.Assert(threatsequence == null);

        // If only one field, skip sequence search
        ArrayList intFields = fieldagent.ReallyInterestingFields(board, 1, non_allow_moves);
        if (intFields.Count > 1)
        {
            // Search for winning threat sequence
            threatsequence = FindWinning(board);
            lastAppliedThreatSequence = null;
            if (threatsequence != null && threatsequence.attacker == null)
                threatsequence = null;

            if (threatsequence != null)
            {
                state = MoveState.Attack;
                statusStr.Append("ATTACK!, ");

                return (true);
            }
        }
        best_field = AlphaBeta_move();
        return (false);
    }

    private ArrayList firstMoveFields = null;

    private Coordinate AlphaBeta_move()
    {
        ArrayList intFields = fieldagent.ReallyInterestingFields(board, 1, non_allow_moves);

        ArrayList defFields = null;

        //처음에는 멍청하게
        if (point_history.Count < 6)
        {
            maximumsearchdepth = 6;
        }
        else
        {
            maximumsearchdepth = 10;
        }

        // Only get the defensive fields if we have a choice
        if (intFields.Count > 1)
        {
            defFields = ThreatDefensiveInterestingFields(board, 10);

            if (defFields != null)
            {
                firstMoveFields = new ArrayList();

                foreach (Coordinate c in defFields)
                {
                    if (intFields.Contains(c))
                        firstMoveFields.Add(c);
                }
            }
            else
                firstMoveFields = intFields;
        }
        else
        {
            firstMoveFields = intFields;
        }

        // Now fields contains the list of interesting moves.

        // Set bestfield and tmpbestfield to dummy value
        if (intFields.Count > 0)
            best_field = (Coordinate)firstMoveFields[0];
        if (intFields.Count > 0)
            tmpbestfield = (Coordinate)firstMoveFields[0];

        if (lastoppmove.X == -1 && lastoppmove.Y == -1)
        {
            best_field = new Coordinate(board.GetLength(0) / 2, board.GetLength(0) / 2);
        }
        else
        {
            // bestfield is already set to the first element, so if there is
            // only one field interesting, we already selected the correct one
            // (the first, the only one)
            if (firstMoveFields.Count > 1)
                iterative_AlphaBeta(lastoppmove, maximumsearchdepth);
        }
        return (best_field);
    }

    private int iterative_AlphaBeta(Coordinate field, int maxDepth)
    {
        int[,] boardsave = (int[,])board.Clone();
        InterestingFieldAgent fieldagentbackup = (InterestingFieldAgent)fieldagent.Clone();

        bool timeout = false;
        int bestalpha = 0;
        int curdepth = 0;
        int founddepth = 0;

        TimeSpan resttime = TimeSpan.Zero;
        TimeSpan usedtime = TimeSpan.Zero;

        statusStr.AppendFormat("AB ({0}): ", maxDepth);
        while (curdepth < maxDepth && bestalpha < StatValEvaluator.WINBORDER && bestalpha > -StatValEvaluator.WINBORDER)
        {
            DateTime starttime = DateTime.Now;
            try
            {
                curdepth += 2;
                statusStr.AppendFormat("{0},", curdepth);

                int alpha = -StatValEvaluator.WIN;
                int beta = StatValEvaluator.WIN;
                Console.WriteLine("Starting alphabeta, ply: {0}", curdepth / 2);
                board = (int[,])boardsave.Clone();

                Debug.Log("AI_IQ=> " + DataManager.instance.AI_IQ+ ", curdepth=> "+ curdepth + ", maxDepth=> " + maxDepth);
                if (DataManager.instance.AI_IQ < 3)
                {
                    alpha = smAlphaBeta(lastoppmove, 0, alpha, beta, curdepth, 0);
                }
                else
                {
                    alpha = AlphaBeta(lastoppmove, 0, alpha, beta, curdepth, 0);
                }
                best_field = tmpbestfield;
                bestalpha = alpha;
                founddepth = curdepth;

                usedtime = DateTime.Now.Subtract(starttime);
                resttime = expiretime.Subtract(DateTime.Now);
            }
            catch (ABSearchTimeoutException)
            {
                resttime = TimeSpan.Zero;
                statusStr.Append("timeout!, ");

                Console.WriteLine("Alpha-Beta stopped by timeout.");
                timeout = true;
            }
        }

        board = boardsave;
        fieldagent = fieldagentbackup;

        return bestalpha;
    }

    private int smAlphaBeta(Coordinate field, int boardvalue, int alpha, int beta, int depthbound, int depth)
    {
        //상대방의 수만을 체크해서 방어 
        int attacker = -1;
        //공격은 상대방

        InterestingFieldAgent fieldagentbackup = (InterestingFieldAgent)fieldagent.Clone();
        //threatList, interestiongField를 받기 전에 현재 상태를 백업을 해준다.

        //내가 공격할때는 field.Val이 음수
        board[field.X, field.Y] = -1 * attacker; // remember: field comes still from the upper level. 

        fieldagent.UpdateInterestingFieldArray(board, field);
        fieldagent.UpdateThreatLists(board, field, -1 * attacker);

        boardvalue += field.Val;

        if (boardvalue > StatValEvaluator.WINBORDER || boardvalue < -StatValEvaluator.WINBORDER) //상대방의 수를 우선시 해서 방어
        {//위에서 체크한 필드 값이 900000을 넘어서면 
            board[field.X, field.Y] = 0;
            if (attacker == 1)//내 차례일때
                return boardvalue - depth * 10000;
            else //상대방 차례일때
                return boardvalue + depth * 10000;
        }

        ArrayList fields = null;
        fields = firstMoveFields;
        board[field.X, field.Y] = attacker;
        int userX = field.X;
        int userY = field.Y;
        // Calculate the static value for all fields
        for (int i = 0; i < fields.Count; ++i)
        {
            Coordinate c = (Coordinate)fields[i];
            board[c.X, c.Y] = attacker;
            InterestingFieldAgent valifabackup = (InterestingFieldAgent)fieldagent.Clone();
            fieldagent.UpdateThreatLists(board, c, attacker);
            c.Val = eval.smstatVal(board, c, attacker, userX, userY, WHITE_illegal_move_list);
            fieldagent = valifabackup;//처음상태로 복귀
            board[c.X, c.Y] = 0;
            fields[i] = c;
        }

        fields.Sort();
        //상대방의 차례 일때
        //MIN node
        //결과가 가장 낮은 순으로 정렬
        foreach (Coordinate ifield in fields)
        {
            if (depth == 0)
            {
                tmpbestfield = ifield;
            }
            board[field.X, field.Y] = 0;
            alpha = ifield.Val;
            return alpha;
        }
        fieldagent = fieldagentbackup;
        board[field.X, field.Y] = 0;
        return alpha;
    }

    private int expirenodecnt;
    private DateTime expiretime;

    public class ABSearchTimeoutException : ApplicationException
    {

    }

    private int AlphaBeta(Coordinate field, int boardvalue, int alpha, int beta, int depthbound, int depth)
    {
        ++expirenodecnt;
        if (expirenodecnt >= 3000)
        {
            expirenodecnt = 0;
            Debug.Log("Checking timeout");
            //if (DateTime.Now > expiretime)
            //{
            //Console.WriteLine("Timeout: {0}", DateTime.Now);
            //}
            throw new ABSearchTimeoutException();
        }

        int attacker = (depth % 2 == 0) ? 1 : -1;

        InterestingFieldAgent fieldagentbackup = (InterestingFieldAgent)fieldagent.Clone();

        board[field.X, field.Y] = -1 * attacker; // remember: field comes still from the upper level.
        fieldagent.UpdateInterestingFieldArray(board, field);
        fieldagent.UpdateThreatLists(board, field, -1 * attacker);

        boardvalue += field.Val;

        if (boardvalue > StatValEvaluator.WINBORDER || boardvalue < -StatValEvaluator.WINBORDER) //상대방의 수를 우선시 해서 방어
        {//위에서 체크한 필드 값이 900000을 넘어서면 
            board[field.X, field.Y] = 0;
            if (attacker == 1)//내 차례일때
                return boardvalue - depth * 10000;
            else //상대방 차례일때
                return boardvalue + depth * 10000;
        }

        if (depth == depthbound)
        {//Max
            board[field.X, field.Y] = 0;//해당 필드 값을 0으로 변경 하고
            return boardvalue;//값 반환
        }

        ArrayList fields = null;
        if (depth == 0)//재귀를 하지 않았을때
        {
            fields = firstMoveFields;
        }
        else//재귀 중일 때
        {
            fields = fieldagent.ReallyInterestingFields(board, attacker, non_allow_moves);//interesting 필드를 넣어준다.
        }

        // Calculate the static value for all fields
        for (int i = 0; i < fields.Count; ++i)
        {
            Coordinate c = (Coordinate)fields[i];
            board[c.X, c.Y] = attacker;
            InterestingFieldAgent valifabackup = (InterestingFieldAgent)fieldagent.Clone();
            fieldagent.UpdateThreatLists(board, c, attacker);
            c.Val = eval.statVal(board, c, attacker, WHITE_illegal_move_list);
            fieldagent = valifabackup;//처음상태로 복귀
            board[c.X, c.Y] = 0;
            fields[i] = c;
        }

        fields.Sort();
        if (depth % 2 == 0) //내차례 일때
        {
            //MAX node
            fields.Reverse();//점수가 높은순으로 정렬
            foreach (Coordinate ifield in fields)
            {
                int max = alpha;
                if ((max = AlphaBeta(ifield, boardvalue, alpha, beta, depthbound, depth + 1)) > alpha)
                {
                    alpha = max;
                    if (depth == 0)
                    {
                        tmpbestfield = ifield;
                    }
                }
                if (depth == 0)//처음 함수가 돌때
                {
                    //Console.WriteLine("Got max from {0}: {1}", ifield, max);//각각의 필드의 max값을 가져온다.
                }
                if (alpha >= beta)
                {
                    fieldagent = fieldagentbackup;
                    if (depth == 0)
                    {
                        tmpbestfield = ifield;
                    }
                    board[field.X, field.Y] = 0;
                    return beta;
                }
            }
            fieldagent = fieldagentbackup;
            board[field.X, field.Y] = 0;
            return alpha;
        }
        else
        {//상대방의 차례 일때
         //MIN node
         //결과가 가장 낮은 순으로 정렬
            foreach (Coordinate ifield in fields)
            {
                if (depth == 0) tmpbestfield = ifield;
                int min = beta;

                if ((min = AlphaBeta(ifield, boardvalue, alpha, beta, depthbound, depth + 1)) < beta)
                {
                    beta = min;
                    if (depth == 0)
                    {
                        tmpbestfield = ifield;
                    }
                }
                if (alpha >= beta)
                {
                    fieldagent = fieldagentbackup;
                    if (depth == 0)
                    {
                        tmpbestfield = ifield;
                    }
                    board[field.X, field.Y] = 0;
                    return alpha;
                }
            }
            fieldagent = fieldagentbackup;
            board[field.X, field.Y] = 0;
            return beta;
        }

        ////
        ///

    }

    private bool get_move_attack()
    {
        Console.WriteLine("get_move_attack");
        Debug.Assert(threatsequence != null);

        // Check the threat sequence is still valid on the current board.
        // This is the rare magic case that could defeat us (C-3, C-4)
        if (threatsequence.CheckValidRevBoard(board) == false)
        {
            threatsequence = null;
            state = MoveState.Normal;

            return (true);
        }

        // Also check if there is still attacker moves left
        if (threatsequence.attacker == null)
        {
            threatsequence = null;
            statusStr.Append("seqEnd reached");

            state = MoveState.Normal;
            return (true);
        }

        // Check the last move of the opponent is one of the expected defender
        // moves in our threat sequence.  If it is not, fall back on
        // alpha-beta search.
        if (lastAppliedThreatSequence != null &&
            lastAppliedThreatSequence.defender != null)
        {
            bool defendedThreat = false;

            foreach (GBMove gbm in lastAppliedThreatSequence.defender)
            {
                if (gbm.X == lastoppmove.X && gbm.Y == lastoppmove.Y)
                {
                    defendedThreat = true;

                    break;
                }
            }

            // If the threat was not defended against, fall back to alpha-beta
            if (defendedThreat == false)
            {
                statusStr.Append("not defended, ");
                threatsequence = null;

                state = MoveState.Normal;
                return (true);
            }
            statusStr.Append("defended, ");
        }

        // Check if the move is in the interesting fields, which basically
        // asserts us there is no opponent threat we have to respond to.
        ArrayList fields = fieldagent.ReallyInterestingFields(board, 1, non_allow_moves);
        Coordinate wantmove = new Coordinate(threatsequence.attacker.X,
            threatsequence.attacker.Y);

        if (fields.Contains(wantmove))
        {
            best_field = wantmove;
            statusStr.Append("move ({0},{1}) from seq, ", wantmove.X, wantmove.Y);
            lastAppliedThreatSequence = threatsequence;

            // Advance to next threat
            threatsequence = threatsequence.next;

            // End of sequence -> fall back to alpha beta, for the _next_
            // move.
            if (threatsequence == null)
                state = MoveState.Normal;
        }
        else
        {
            statusStr.Append("move not from seq, ");
            best_field = AlphaBeta_move();
        }
        return (false);
    }


  

    /** Check if there is a visible winning threat sequence for the opponent
	 * on the given board.
	 *
	 * This can be used to quickly "no-false-positive" check a move to be
	 * made.  If this function returns true, and we really make this move,
	 * then the opponent has a sure win.  So, if this function returns true,
	 * you should not make this move if possible.
	 *
	 * @param board The board to be checked for opponent-attackability.
	 * @param dbDefenseTimeLimit The limit in milliseconds we allow for
	 * checking the move.  Once this time is exceeded, we return.  Zero means
	 * no limit.
	 *
	 * @returns true if the board allows a winning sequence by the opponent,
	 * false has no meaning.
	 */
    public bool CheckForOpponentSequence(int[,] board, int dbDefenseTimeLimit)
    {
        return (GetOpponentSequence(board, dbDefenseTimeLimit) != null);
    }

    // TODO: docs
    public ArrayList ThreatDefensiveInterestingFields(int[,] board,
        int dbDefenseTimeLimit)
    {
        GBThreatSequence seq = GetOpponentSequence(board, dbDefenseTimeLimit);
        if (seq == null)
            return (null);

        // Only sequences which have defending moves (that is, category 1 and
        // above) do count.  The special case STRAIGHT FOUR is a GOAL and does
        // not have any defending moves.
        if (seq.defender == null || seq.defender.Length == 0)
            return (null);

        ArrayList defFields = new ArrayList();

        // Add the attack and the defending moves of the first sequence ply to
        // the "defending interesting fields".
        defFields.Add(new Coordinate(seq.attacker.X, seq.attacker.Y));

        if (seq.defender != null)
        {
            foreach (GBMove gbm in seq.defender)
                defFields.Add(new Coordinate(gbm.X, gbm.Y));
        }


        // Now only keep the most attacking ones, that is the ones that
        // creates the highest number of distinct operators.
        ArrayList fields = GetDefensePriorityFields(board, defFields);

        return (fields);
    }

    /** Try to find a winning threat sequence for the opponent.
	 *
	 * @param board The current game board to be checked.
	 * @param dbDefenseTimeLimit The limit in milliseconds to be allowed for
	 * the search.
	 *
	 * @returns The winning threat sequence found null if none has been found.
	 */
    private GBThreatSequence GetOpponentSequence(int[,] board, int dbDefenseTimeLimit)
    {
        int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
        for (int y = 0; y < boardRev.GetLength(0); ++y)
        {
            for (int x = 0; x < boardRev.GetLength(1); ++x)
            {
                // Reverse, so we check for "us", where us is, in fact, the
                // opponent.
                boardRev[y, x] = -board[x, y];
            }
        }

        Console.WriteLine("Searching for opponent attackability of future board:");
        Console.WriteLine(new GoBangBoard(boardRev));

        GBThreatSequence seq = GBThreatSearch.FindWinning(boardRev,
            dbDefenseTimeLimit * 1000, true);

        if (seq != null)
            Console.WriteLine("Opponent sequence: {0}", seq);

        return (seq);
    }

    // return type: ArrayList<Coordinate>
    private ArrayList GetDefensePriorityFields(int[,] board, ArrayList consider)
    {
        int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
        for (int y = 0; y < boardRev.GetLength(0); ++y)
        {
            for (int x = 0; x < boardRev.GetLength(1); ++x)
            {
                // Reverse, so we check for "us", where us is, in fact, the
                // opponent.
                boardRev[y, x] = -board[x, y];
            }
        }

        int maxCount;
        int[,] operatorMap = GBThreatSearch.BuildOperatorMap
            (new GoBangBoard(boardRev), out maxCount);
        Console.WriteLine("operator map:");
        for (int y = 0; y < operatorMap.GetLength(0); ++y)
        {
            for (int x = 0; x < operatorMap.GetLength(1); ++x)
            {
                Console.Write("{0} ", operatorMap[y, x]);
            }
            Console.WriteLine();
        }

        // Only keep and determine the max fields of the consider list.
        int considerMaxCount = 0;

        foreach (Coordinate c in consider)
        {
            if (operatorMap[c.Y, c.X] > considerMaxCount)
                considerMaxCount = operatorMap[c.Y, c.X];
        }

        ArrayList fields = new ArrayList();
        foreach (Coordinate c in consider)
        {
            if (operatorMap[c.Y, c.X] == considerMaxCount)
                fields.Add(new Coordinate(c.X, c.Y));
        }

        return (fields);
    }

    /** Find a winning threat sequence for us.
	 *
	 * @param board The board to be checked.
	 *
	 * @returns The threat sequence found on success, or null if no sequence
	 * has been found.
	 */
    public GBThreatSequence FindWinning(int[,] board)
    {
        int[,] boardRev = new int[board.GetLength(1), board.GetLength(0)];
        for (int y = 0; y < boardRev.GetLength(0); ++y)
        {
            for (int x = 0; x < boardRev.GetLength(1); ++x)
            {
                boardRev[y, x] = board[x, y];
            }
        }

        // A timelimit of zero means no limit.
        return GBThreatSearch.FindWinning(boardRev, 8000, false);
    }
    #endregion

    private void reset()
    {
        point_history.Clear();//백돌의 금수 확인 클리어

        ai_board = new Point[15, 15];
        for (byte i = 0; i < 15; i++)
        {
            for (byte j = 0; j < 15; j++)
            {
                Point point = new Point(i, j);
                ai_board[i, j] = point;
            }
        }

        board = new int[15, 15];
        lastoppmove = new Coordinate(-1, -1);
        searcher = new ThreatSearcher();
        fieldagent = new InterestingFieldAgent(searcher, 15);
        eval = new StatValEvaluator(fieldagent);
    }
}