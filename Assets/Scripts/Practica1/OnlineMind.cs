using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

public enum AlgorithmOnlineOption{
    ALGORITHM_1,
    ALGORITHM_2
}

public class OnlineMind : AbstractPathMind
{
    Node finalNode = null;
    Node parentNode = null;

    bool first_time = false; //?Se refiere a la primera vez que realizamos la busqueda?

    double tiempo_inicio = 0;

    [SerializeField]
    AlgorithmOnlineOption algorithmOnlineOption;

    void Start()
    {
        // Debug.Log("Start");
        finalNode = null;
        tiempo_inicio = Time.time;
    }

    public Node ChoosePath_1(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;

        List<Node> nodeResponse = new List<Node>();
        Node startNode = new Node(currentPos, null, -1); //Crea el nodo de inicio con la posicion actual, sin nodo padre y sin direccion de movimiento
        nodeResponse.Add(startNode);


        return null;
    }

    public Node ChoosePath_2(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){
        return null;
    }


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (!first_time)
        {
            first_time = true;
            if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_1)
            {
                finalNode = ChoosePath_1(boardInfo, currentPos, goals);
            }    
            else if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_2)
            {
                finalNode = ChoosePath_2(boardInfo, currentPos, goals);
            }

        }

        bool found_move = false;
        if (finalNode != null)
        {
            while (!found_move)
            {

                if (parentNode == null)
                {
                    parentNode = finalNode;
                }
                else
                {
                    parentNode = parentNode.parentNode;
                }

                if (parentNode != null && parentNode.parentNode != null && parentNode.parentNode.cellInfo != null)
                {
                    if (parentNode.parentNode.cellInfo.CellId == currentPos.CellId)
                    {
                        found_move = true;
                        if (parentNode.direction == 0) return Locomotion.MoveDirection.Up;
                        else if (parentNode.direction == 1) return Locomotion.MoveDirection.Right;
                        else if (parentNode.direction == 2) return Locomotion.MoveDirection.Down;
                        else if (parentNode.direction == 3) return Locomotion.MoveDirection.Left;
                        else Debug.LogError("Hay algo mal bro");
                    }
                }
            }
        }
        return Locomotion.MoveDirection.None;
    }
}
