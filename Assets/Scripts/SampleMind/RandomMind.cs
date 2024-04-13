using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.SampleMind
{
    public class RandomMind : AbstractPathMind {
       
        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            CellInfo[] cellInfos = currentPos.WalkableNeighbours(boardInfo);
            var val = Random.Range(0, 4);
            bool found_move = false;
            while(!found_move)
            {
                if (cellInfos[val] != null) 
                {
                    found_move = true;
                    if (val == 0) return Locomotion.MoveDirection.Up;
                    if (val == 1) return Locomotion.MoveDirection.Right;
                    if (val == 2) return Locomotion.MoveDirection.Down;
                    return Locomotion.MoveDirection.Left;
                }
                else
                {
                    val = Random.Range(0, 4);
                }
            }
            return Locomotion.MoveDirection.None;
        }
    }
}
