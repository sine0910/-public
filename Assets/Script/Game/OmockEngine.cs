using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokEngine : MonoBehaviour
{
    public List<Point> point_history;
    private Point current_point_being_checked;

    private Point[,] board;

    public Point select_point;

    Dictionary<byte, PLAYER_TYPE> players;

    byte current_player_index;

    public bool game_over;
    public byte winner;

    public OmokEngine()
    {
        Debug.Log("OmokEngineStart");
        set_user_data();
        point_history = new List<Point>();
    }

    public void reset()
    {
        game_over = false;

        point_history.Clear();

        board = new Point[15, 15];

        for (byte i = 0; i < 15; i++)
        {
            for (byte j = 0; j < 15; j++)
            {
                Point point = new Point(i, j);
                board[i, j] = point;
            }
        }

        current_player_index = get_black_player();
    }

    public void set_user_data()
    {
        Debug.Log("set_user_data");
        this.players = new Dictionary<byte, PLAYER_TYPE>();
        this.players[GameManager.instance.black_index] = PLAYER_TYPE.BLACK;
        this.players[GameManager.instance.white_index] = PLAYER_TYPE.WHITE;
    }

    public DataController player_select_point(byte player_index, byte x, byte y)
    {
        if (board[x, y] == null || board[x, y].state != STATE.None)
        {
            Debug.Log("player_select_point is cannot use point!\n" +
                "x: " + x + " y: " + y);
            return DataController.Error;
        }
        else
        {
            switch (this.players[player_index])
            {
                case PLAYER_TYPE.BLACK:
                    {
                        board[x, y].set_state(STATE.BLACK);

                        if (product_five_win(board[x, y], STATE.BLACK))
                        {
                            set_winner(player_index);
                        }
                    }
                    break;
                case PLAYER_TYPE.WHITE:
                    {
                        board[x, y].set_state(STATE.WHITE);

                        if (product_five_win(board[x, y], STATE.WHITE))
                        {
                            set_winner(player_index);
                        }
                    }
                    break;
            }
            select_point = board[x, y];
            point_history.Add(new Point(x, y));
            return DataController.None;
        }
    }

    public void set_winner(byte player_index)
    {
        game_over = true;
        winner = player_index;
    }

    public List<IllegalMove> calculate_Illegal_move()
    {
        List<IllegalMove> illegal_points = new List<IllegalMove>();
        bool[,] potential_moves_already_checked = new bool[GameManager.BOARD_SIZE, GameManager.BOARD_SIZE];

        foreach (Point point in point_history)
        {
            byte startingX = (byte)Math.Max(0, point.x - 2);
            byte endingX = (byte)Math.Min(GameManager.BOARD_SIZE - 1, point.x + 2);
            byte startingY = (byte)Math.Max(0, point.y - 2);
            byte endingY = (byte)Math.Min(GameManager.BOARD_SIZE - 1, point.y + 2);

            for (byte x = startingX; x <= endingX; x++)
            {
                for (byte y = startingY; y <= endingY; y++)
                {
                    current_point_being_checked = board[x, y];

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

                    if (product_five_win(current_point_being_checked, STATE.BLACK))
                    {
                        //만약 오목을 완성할 수 있는 구간은 금지구역으로 지정하지 않는다
                    }
                    else if (produce_over_line(current_point_being_checked))
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Overline));
                    }
                    else if (count_open_threes(current_point_being_checked) >= 2)
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Double3));
                    }
                    else if (count_open_fours(current_point_being_checked) >= 2)
                    {
                        illegal_points.Add(new IllegalMove(current_point_being_checked, ILLEGAL_MOVE.Double4));
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

            if (get_point_state(oneBefore) == STATE.BLACK &&
                get_point_state(oneAfter) == STATE.BLACK)
            {
                if (get_point_state(fourBefore) == STATE.BLACK &&
                    get_point_state(threeBefore) == STATE.BLACK &&
                    get_point_state(twoBefore) == STATE.BLACK)
                {
                    return true;
                }

                if (get_point_state(threeBefore) == STATE.BLACK &&
                    get_point_state(twoBefore) == STATE.BLACK &&
                    get_point_state(twoAfter) == STATE.BLACK)
                {
                    return true;
                }

                if (get_point_state(twoBefore) == STATE.BLACK &&
                    get_point_state(twoAfter) == STATE.BLACK &&
                    get_point_state(threeAfter) == STATE.BLACK)
                {
                    return true;
                }

                if (get_point_state(twoAfter) == STATE.BLACK &&
                    get_point_state(threeAfter) == STATE.BLACK &&
                    get_point_state(fourAfter) == STATE.BLACK)
                {
                    return true;
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

        return board[point.x + steps * deltaX, point.y + steps * deltaY];
    }

    public STATE get_point_state(Point point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= GameManager.BOARD_SIZE || point.y >= GameManager.BOARD_SIZE)
        {
            return STATE.OutsideOfBoard;
        }
        return board[point.x, point.y].state;
    }

    public void set_point_state(Point point, STATE state)
    {
        board[point.x, point.y].state = state;
    }

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
        else if(playerColourOccupancyState == STATE.WHITE)
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

    #region 3x3 LOGIC
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
           (get_point_state(point) == STATE.BLACK || current_point_being_checked.Equals(point)) &&
           (get_point_state(oneAfter) == STATE.BLACK || current_point_being_checked.Equals(oneAfter)) &&
            get_point_state(twoAfter) == STATE.None &&
           (get_point_state(threeAfter) == STATE.BLACK || current_point_being_checked.Equals(threeAfter)) &&
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
        Point fourAfter = get_point_steps_after(point,4, dir);

        if (get_point_state(oneBefore) == STATE.None &&
           (get_point_state(point) == STATE.BLACK || current_point_being_checked.Equals(point)) &&
            get_point_state(oneAfter) == STATE.None &&
           (get_point_state(twoAfter) == STATE.BLACK || current_point_being_checked.Equals(twoAfter)) &&
           (get_point_state(threeAfter) == STATE.BLACK || current_point_being_checked.Equals(threeAfter)) &&
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
           (get_point_state(point) == STATE.BLACK || current_point_being_checked.Equals(point)) &&
           (get_point_state(oneAfter) == STATE.BLACK || current_point_being_checked.Equals(oneAfter)) &&
           (get_point_state(twoAfter) == STATE.BLACK || current_point_being_checked.Equals(twoAfter)) &&
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

        if (get_point_state(twoBefore) == STATE.BLACK ||
            get_point_state(fiveAfter) == STATE.BLACK)
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

        if ((get_point_state(point) == STATE.BLACK || current_point_being_checked.Equals(point)) &&
            (get_point_state(oneAfter) == STATE.BLACK || current_point_being_checked.Equals(oneAfter)) &&
            (get_point_state(twoAfter) == STATE.BLACK || current_point_being_checked.Equals(twoAfter)) &&
            (get_point_state(threeAfter) == STATE.BLACK || current_point_being_checked.Equals(threeAfter)))
        {
            if (get_point_state(oneBefore) == STATE.None) //check if open 4 can be made by assuming black piece appended to tail of open 3
            {
                return get_point_state(twoBefore) != STATE.BLACK;
            }

            if (get_point_state(fourAfter) == STATE.None) //check if open 4 can be made by assuming black piece appended to head of open 3
            {
                return get_point_state(fiveAfter) != STATE.BLACK;
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

        if (get_point_state(oneBefore) == STATE.BLACK ||
            get_point_state(fiveAfter) == STATE.BLACK)
        {
            return false; //cannot produce 5 in a row due to overline hazard
        }

        if (get_point_state(oneAfter) == STATE.WHITE ||
            get_point_state(twoAfter) == STATE.WHITE ||
            get_point_state(threeAfter) == STATE.WHITE)
        {
            //ensure no white pieces in the middle
            return false;
        }

        if (get_point_state(point) != STATE.BLACK && !current_point_being_checked.Equals(point) ||
            get_point_state(fourAfter) != STATE.BLACK && !current_point_being_checked.Equals(fourAfter))
        {
            //ensure leftmost and rightmost pieces are black
            return false;
        }

        if (get_point_state(oneAfter) == STATE.BLACK || current_point_being_checked.Equals(oneAfter))
        {
            numBlackPiecesFound++;
        }
        if (get_point_state(twoAfter) == STATE.BLACK || current_point_being_checked.Equals(twoAfter))
        {
            numBlackPiecesFound++;
        }
        if (get_point_state(threeAfter) == STATE.BLACK || current_point_being_checked.Equals(threeAfter))
        {
            numBlackPiecesFound++;
        }

        return numBlackPiecesFound == 2;
    }
    #endregion

    public byte get_black_player()
    {
        for (byte i = 0; i < this.players.Count; i++)
        {
            if (this.players[i] == PLAYER_TYPE.BLACK)
            {
                Debug.Log("get_black_player: " + i);
                return i;
            }
        }
        return GameManager.instance.black_index;
    }

    public byte get_white_player()
    {
        for (byte i = 0; i < this.players.Count; i++)
        {
            if (this.players[i] == PLAYER_TYPE.WHITE)
            {
                return i;
            }
        }
        return 0;
    }

    public PLAYER_TYPE get_player_type(byte i)
    {
        return this.players[i];
    }

    public byte get_current_player()
    {
        return this.current_player_index;
    }

    public byte get_next_player()
    {
        if (this.current_player_index < 1)
        {
            this.current_player_index++;
        }
        else
        {
            this.current_player_index = 0;
        }

        return this.current_player_index;
    }
}
