using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;

public class OfflineMind : AbstractPathMind
{
    // Start is called before the first frame update
    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        Debug.Log(boardInfo.CellInfos[currentPos.ColumnId, currentPos.RowId].Walkable);

        Debug.Log("goals end " + goals[0].CellId);

        var val = Random.Range(0, 4);
        if (val == 0) return Locomotion.MoveDirection.Up;
        if (val == 1) return Locomotion.MoveDirection.Down;
        if (val == 2) return Locomotion.MoveDirection.Left;
        return Locomotion.MoveDirection.Right;
    }
}
