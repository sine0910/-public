using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class AIBrain : MonoBehaviour
{
    delegate void PacketFn(List<string> msg);
    Dictionary<PROTOCOL, PacketFn> packet_handler;

    AIGameRoom game_room;

    int[,] board;
    Point[,] ai_board;

    Point best_move;

    public byte player_me_index;
    public PLAYER_TYPE my_player_type;

    public AIBrain(AIGameRoom room)
    {
        this.game_room = room;
        this.packet_handler = new Dictionary<PROTOCOL, PacketFn>();
        this.packet_handler.Add(PROTOCOL.BEGIN_START, BEGIN_START);
        this.packet_handler.Add(PROTOCOL.READY_TO_GAME_START, GAME_START);
        this.packet_handler.Add(PROTOCOL.START_TURN, START_TURN);
        this.packet_handler.Add(PROTOCOL.SELECT_SLOT_RESULT, SELECT_SLOT_RESULT);
        this.packet_handler.Add(PROTOCOL.GAME_RESULT, GAME_RESULT);
    }

    #region AI 데이터 처리 로직
    public void on_receive(List<string> msg)
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

    void START_TURN(List<string> msg)
    {
        best_move = get_allow_moves();

        List<string> send_msg = new List<string>();
        send_msg.Add((byte)PROTOCOL.SELECT_SLOT + "");
        send_msg.Add(best_move.x + "");
        send_msg.Add(best_move.y + "");
        send_message(send_msg);
    }

    void SELECT_SLOT_RESULT(List<string> msg)
    {
        byte player_index = Converter.to_byte(PopAt(msg));

        STATE state = Converter.to_state(PopAt(msg));
        byte select_x = Converter.to_byte(PopAt(msg));
        byte select_y = Converter.to_byte(PopAt(msg));

        ai_board[select_x, select_y].set_state(state);

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
        boardvalue += eval.statVal(board, move, 1);
    }

    public void RegOppMove(Coordinate move)
    {
        board[move.X, move.Y] = -1;
        fieldagent.UpdateInterestingFieldArray(board, move);
        fieldagent.UpdateThreatLists(board, move, -1);
        move.Val = eval.statVal(board, move, -1);
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

    const int maximumsearchdepth = 16;//16

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

    private Point get_allow_moves()
    {
        //AI가 선택할 수 있는 슬롯을 지정한다
        ArrayList allow_moves = new ArrayList();

        foreach (Point point in ai_board)
        {
            if (point.state != STATE.None)
            {
                continue;
            }
            allow_moves.Add(new Point(point.x, point.y));
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

    private bool get_move_normal()
    {
        Debug.Log("GetMoveNormal");
        Debug.Assert(threatsequence == null);

        // If only one field, skip sequence search
        ArrayList intFields = fieldagent.ReallyInterestingFields(board, 1);
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
        ArrayList intFields = fieldagent.ReallyInterestingFields(board, 1);

        ArrayList defFields = null;

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
        ArrayList fields = fieldagent.ReallyInterestingFields(board, 1);
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
            curdepth += 2;
            statusStr.AppendFormat("{0},", curdepth);

            int alpha = -StatValEvaluator.WIN;
            int beta = StatValEvaluator.WIN;
            Console.WriteLine("Starting alphabeta, ply: {0}", curdepth / 2);
            board = (int[,])boardsave.Clone();

            alpha = AlphaBeta(lastoppmove, 0, alpha, beta, curdepth, 0);
            best_field = tmpbestfield;
            bestalpha = alpha;
            founddepth = curdepth;
        }

        board = boardsave;
        fieldagent = fieldagentbackup;

        return bestalpha;
    }

    private int AlphaBeta(Coordinate field, int boardvalue, int alpha, int beta, int depthbound, int depth)
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
            c.Val = eval.smstatVal(board, c, attacker, userX, userY);
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