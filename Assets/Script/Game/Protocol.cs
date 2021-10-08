using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAY
{
    AI,
    PVP
};

public enum RULE
{
    GENERAL,
    RENJU
};

public enum PROTOCOL : byte
{
    BEGIN_START = 1,
    READY_TO_START = 2,
    READY_TO_GAME_START = 3,
    GAME_START = 4,
    START_TURN = 10,
    SELECT_SLOT = 20,
    SELECT_SLOT_RESULT = 21,
    SELECT_SLOT_RESULT_ERROR = 22,
    TURN_END = 40,
    TIME_OUT = 44,
    GAME_RESULT = 99
}

public enum DataController
{
    Error,
    None
};

public enum ILLEGAL_MOVE
{
    Double3,
    Double4,
    Overline
};

public enum STATE
{
    None,
    BLACK,
    WHITE,
    IllegalMove,
    OutsideOfBoard
}

public enum DIRECTION
{
    S_N,
    SW_NE,
    W_E,
    NW_SE
}

public enum PLAYER_TYPE
{
    NONE,
    BLACK,
    WHITE,
}

public enum EMOTICON : int
{
    HELLO = 1,
    HURRY = 2,
    ZEALOUSLY= 3,
    HELP = 4,
    WELL = 5,
}

public class IllegalMove
{
    public Point point { get; private set; }
    public ILLEGAL_MOVE reason { get; private set; }

    public IllegalMove(Point p, ILLEGAL_MOVE r)
    {
        point = p;
        reason = r;
    }
}
