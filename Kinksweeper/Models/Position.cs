namespace Kinksweeper.Models;

public enum PositionState
{
    CLOSED,
    FLAGGED,
    OPEN
}

public class Position
{
    public PositionState state;
    public bool hasMine;
    public int minesAround;

    public Position()
    {
        state = PositionState.CLOSED;
        hasMine = false;
        minesAround = 0;
    }
}