using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class OfflineMind : AbstractPathMind
{

    public Locomotion.MoveDirection[] ChoosePath(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        List<Locomotion.MoveDirection> reponse = new List<Locomotion.MoveDirection>();

        bool goal_found = false;

        float bestOption = Mathf.Infinity;
        Vector2 bestPos = Vector2.zero;

        Vector2 editablePos = Vector2.zero;

        editablePos.x = currentPos.ColumnId;
        editablePos.y = currentPos.RowId;

        while (!goal_found)
        {
            if (bestOption < Mathf.Infinity)
            {
                bestOption = Mathf.Infinity;
            }

            if (bestPos != Vector2.zero)
            {
                editablePos.x += bestPos.x;
                editablePos.y += bestPos.y;
                bestPos = Vector2.zero;
            }

            for (int i = -1; i < 2; i++)
            {
                
                if (editablePos.x + i > -1 && editablePos.x + i < boardInfo.NumColumns)
                {
                    Vector2 positionToBe = boardInfo.CellInfos[(int) editablePos.x + i,(int) editablePos.y].GetPosition;
                    Vector2 positionGoal = goals[0].GetPosition;

                    float option = Vector2.Distance(positionToBe, positionGoal);

                    Debug.Log("I");
                    Debug.Log("CellToBe " + boardInfo.CellInfos[(int)editablePos.x + i, (int)editablePos.y].CellId);
                    Debug.Log("Goal " + goals[0].CellId);

                    Debug.Log("option " + i + " : " + option);

                    if (option < bestOption)
                    {
                        bestOption = option;
                        Debug.Log("Best Option");
                        bestPos = new Vector2(i, 0);
                    }
                    else if (option == 0)
                    {
                        goal_found = true;
                    }
                }
            }
            
            for (int j = -1; j < 2; j++)
            {
                if (editablePos.y + j > -1 && editablePos.y + j < boardInfo.NumColumns)
                {
                    Vector2 positionToBe = boardInfo.CellInfos[(int)editablePos.x, (int)editablePos.y + j].GetPosition;
                    Vector2 positionGoal = goals[0].GetPosition;

                    float option = Vector2.Distance(positionToBe, positionGoal);

                    if (option < bestOption)
                    {
                        bestOption = option;
                        Debug.Log("Best Option");
                        bestPos = new Vector2(0, j);
                    }
                    else if (option == 0)
                    {
                        goal_found = true;
                    }

                    Debug.Log("J");
                    Debug.Log("CellToBe " + boardInfo.CellInfos[(int)editablePos.x, (int)editablePos.y + j].CellId);
                    Debug.Log("Goal " + goals[0].CellId);

                    Debug.Log("option " + j + " : " + option);
                }
            }

            if (bestPos == new Vector2(1, 0))
            {
                reponse.Add(Locomotion.MoveDirection.Up);
                Debug.Log("Add reponse");
            }
            else if (bestPos == new Vector2 (-1, 0))
            {
                reponse.Add(Locomotion.MoveDirection.Down);
                Debug.Log("Add reponse");
            }
            else if (bestPos == new Vector2 (0, 1))
            {
                reponse.Add(Locomotion.MoveDirection.Right);
                Debug.Log("Add reponse");
            }
            else if (bestPos == new Vector2 (0, -1))
            {
                reponse.Add(Locomotion.MoveDirection.Left);
                Debug.Log("Add reponse");
            }

            // Debug
            goal_found = true;
        }

        return reponse.ToArray();
    }

    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        // Debug.Log(boardInfo.CellInfos[currentPos.ColumnId, currentPos.RowId].Walkable);

        // Debug.Log("goals end " + goals[0].CellId);

        ChoosePath(boardInfo, currentPos, goals);

        /*var val = Random.Range(0, 4);
        if (val == 0) return Locomotion.MoveDirection.Up;
        if (val == 1) return Locomotion.MoveDirection.Down;
        if (val == 2) return Locomotion.MoveDirection.Left;
        return Locomotion.MoveDirection.Right;*/

        return Locomotion.MoveDirection.None;
    }
}
