using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class OfflineMind : AbstractPathMind
{
    BoardInfo board = null;
    CellInfo current = null;
    CellInfo[] goal = null;

    bool goal_found  = false;
    bool first_time = false;

    void Start()
    {
        Debug.Log("Start");
    }

    public Node ChoosePath(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        // List<Locomotion.MoveDirection> reponse = new List<Locomotion.MoveDirection>();

        /*bool goal_found = false;

        float bestOption = Mathf.Infinity;
        Vector2 bestPos = Vector2.zero;

        Vector2 editablePos = Vector2.zero;

        Vector2 positionGoal = goals[0].GetPosition;

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
                    if (boardInfo.CellInfos[(int)editablePos.x + i, (int)editablePos.y].Walkable)
                    {
                        Vector2 positionToBe = boardInfo.CellInfos[(int)editablePos.x + i,(int) editablePos.y].GetPosition;

                        float option = Vector2.Distance(positionToBe, positionGoal);

                        Debug.Log("I");
                        Debug.Log("CellToBe " + boardInfo.CellInfos[(int)editablePos.x + i, (int)editablePos.y].CellId);
                        Debug.Log("option " + i + " : " + option);

                        if (option < bestOption)
                        {
                            bestOption = option;
                            Debug.Log("Best Option " + i);
                            bestPos = new Vector2(i, 0);
                        }
                        else if (option == 0)
                        {
                            Debug.Log("Found");
                            goal_found = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Wall");
                    }
                }
            }
            
            for (int j = -1; j < 2; j++)
            {
                if (editablePos.y + j > -1 && editablePos.y + j < boardInfo.NumColumns)
                {
                    if (boardInfo.CellInfos[(int)editablePos.x, (int)editablePos.y + j].Walkable)
                    {
                        Vector2 positionToBe = boardInfo.CellInfos[(int) editablePos.x, (int)editablePos.y + j].GetPosition;

                        float option = Vector2.Distance(positionToBe, positionGoal);

                        Debug.Log("J");
                        Debug.Log("CellToBe " + boardInfo.CellInfos[(int)editablePos.x, (int)editablePos.y + j].CellId);
                        Debug.Log("option " + j + " : " + option);

                        if (option < bestOption)
                        {
                            bestOption = option;
                            Debug.Log("Best Option " + j);
                            bestPos = new Vector2(0, j);
                        }
                        else if (option == 0)
                        {
                            Debug.Log("Found");
                            goal_found = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Wall");
                    }
                }
            }

            if (bestPos == new Vector2(1, 0))
            {
                // reponse.Add(Locomotion.MoveDirection.Up);
                Debug.Log("Add reponse Up");
                return Locomotion.MoveDirection.Up;
            }
            else if (bestPos == new Vector2 (-1, 0))
            {
                // reponse.Add(Locomotion.MoveDirection.Down);
                Debug.Log("Add reponse Down");
                return Locomotion.MoveDirection.Down;
            }
            else if (bestPos == new Vector2 (0, 1))
            {
                // reponse.Add(Locomotion.MoveDirection.Right);
                Debug.Log("Add reponse Right");
                return Locomotion.MoveDirection.Right;
            }
            else if (bestPos == new Vector2 (0, -1))
            {
                //reponse.Add(Locomotion.MoveDirection.Left);
                Debug.Log("Add reponse Left");
                return Locomotion.MoveDirection.Left;
            }

            // Debug
            goal_found = true;
        }*/

        // currentPos.WalkableNeighbours(boardInfo);

        List<Node> nodeResponse = new List<Node>();

        this.board = boardInfo;
        this.current = currentPos;  
        this.goal = goals;
        int j = 0;

        // nodeResponse.Add(this);
        CellInfo[] firstSteps = currentPos.WalkableNeighbours(boardInfo);
        for (int i = 0; i < firstSteps.Length; i++)
        {
            if (firstSteps[i] != null)
            {
                // El indice indica que movimiento tuvo que tomar para poder llegar a el
                /*
                 * 0 = UP
                 * 1 = RIGHT
                 * 2 = DOWN
                 * 3 = LEFT
                 */

                nodeResponse.Add(new Node(firstSteps[i], current, i));
            }
        }

        while (!goal_found && nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse[0];
            if (openNode.cellInfo != null)
            {
                if (openNode.cellInfo == goals[0])
                {
                    Debug.Log("Found");
                    goal_found = true;
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            nodeResponse.Add(new Node(nextCells[i], openNode.cellInfo, i));
                            Debug.Log("Next cell " + nextCells[i].CellId);
                        }
                    }
                    nodeResponse.Remove(openNode);
                }
                j++;
            }
            else
            {
                nodeResponse.Remove(openNode);
            }

            //Debug
            if (j > 15)
            {
                goal_found = true;
            }

            // Si no hago esto, se queda muerto el unity, en teoria ya deberia de estar pero no corre.
        }


        return null;
    }

    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        // Debug.Log(boardInfo.CellInfos[currentPos.ColumnId, currentPos.RowId].Walkable);

        // Debug.Log("goals end " + goals[0].CellId);

        // Debug.Log("current pos " + currentPos.CellId);
        // Locomotion.MoveDirection respuesta = ChoosePath(boardInfo, currentPos, goals);

        // Debug.Log("Respuesta " +  respuesta);

        // return respuesta;

        if (!first_time)
        {
            first_time = true;
            ChoosePath(boardInfo, currentPos, goals);
        }

        /*var val = Random.Range(0, 4);
        if (val == 0) return Locomotion.MoveDirection.Up;
        if (val == 1) return Locomotion.MoveDirection.Down;
        if (val == 2) return Locomotion.MoveDirection.Left;
        return Locomotion.MoveDirection.Right;*/

        // Debug.Log("GetNextMove");

        return Locomotion.MoveDirection.None;
    }
}
