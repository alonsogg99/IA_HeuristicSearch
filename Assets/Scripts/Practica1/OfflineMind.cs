using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

public enum AlgorithmOption
{
    ALGORITHM_A,
    ALGORITHM_UNIFORM_COST,
    ALGORITHM_GENERAL_OPTIMIZED,
    ALGORITHM_GENERAL,
};

public class OfflineMind : AbstractPathMind
{
    Node finalNode = null;
    Node parentNode = null;

    bool first_time = false;

    double tiempo_inicio = 0;

    [SerializeField]
    AlgorithmOption algorithOption;

    void Start()
    {
        // Debug.Log("Start");
        finalNode = null;
        tiempo_inicio = Time.time;
    }

    public Node ChoosePath_General(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        // El indice indica que movimiento tuvo que tomar para poder llegar a el
        /*
         * 0 = UP
         * 1 = RIGHT
         * 2 = DOWN
         * 3 = LEFT
         */
        Debug.Log("Busqueda General");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        List<Node> nodeResponse = new List<Node>();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);

        while (nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse[0];
            tiempo_total += Time.deltaTime;
            if (openNode.cellInfo != null)
            {
                if (openNode.cellInfo == goals[0])
                {
                    Debug.Log("Nodos abiertos " + nodeResponse.Count);
                    Debug.Log("Tiempo transcurrido " + tiempo_total);
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            if (!nodeResponse.Any(node => node.cellInfo == nextCells[i]))
                            {
                                nodeResponse.Add(new Node(nextCells[i], openNode, i));
                            }
                        }
                    }
                    nodeResponse.Remove(openNode);
                }
            }
            else
            {
                nodeResponse.Remove(openNode);
            }
        }
        return null;
    }

    public Node ChoosePath_General_Optimized(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        Debug.Log("Busqueda General Optimizada");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        List<Node> nodeResponse = new List<Node>();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);

        float bestOpt = Mathf.Infinity;

        while (nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse[0];
            tiempo_total += Time.deltaTime;
            if (openNode.cellInfo != null)
            {
                if (openNode.cellInfo == goals[0])
                {
                    Debug.Log("Nodos abiertos " + nodeResponse.Count);
                    Debug.Log("Tiempo transcurrido " + tiempo_total);
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            if (!nodeResponse.Any(node => node.cellInfo == nextCells[i]))
                            {
                                float option = Vector2.Distance(nextCells[i].GetPosition, goals[0].GetPosition);
                                if (option < bestOpt)
                                {
                                    bestOpt = option;
                                    nodeResponse.Insert(0, new Node(nextCells[i], openNode, i));
                                }
                                else
                                {
                                    nodeResponse.Add(new Node(nextCells[i], openNode, i));
                                }
                            }
                        }
                    }
                    nodeResponse.Remove(openNode);
                }
            }
            else
            {
                nodeResponse.Remove(openNode);
            }
        }
        return null;
    }

    public Node ChoosePath_UCS(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        Debug.Log("Busqueda de coste uniforme");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        List<Node> nodeResponse = new List<Node>();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);

        while (nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse.OrderBy(node => node.G).First(); ;
            tiempo_total += Time.deltaTime;
            if (openNode.cellInfo != null)
            {
                if (openNode.cellInfo == goals[0])
                {
                    Debug.Log("Nodos abiertos " + nodeResponse.Count);
                    Debug.Log("Tiempo transcurrido " + tiempo_total);
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            Node neighbourNode = new Node(nextCells[i], openNode, i);

                            neighbourNode.G = openNode.G + Vector2.Distance(openNode.cellInfo.GetPosition, neighbourNode.cellInfo.GetPosition);

                            // Buscar un nodo existente en nodeResponse con la misma cellInfo
                            Node existingNode = nodeResponse.FirstOrDefault(node => node.cellInfo == neighbourNode.cellInfo);

                            if (existingNode == null)
                            {
                                // Si no existe, lo agregamos
                                nodeResponse.Add(neighbourNode);
                            }
                            else if (existingNode.G > neighbourNode.G)
                            {
                                // Si existe y su costo G es mayor, actualizamos el nodo existente
                                existingNode.G = neighbourNode.G;
                                existingNode.parentNode = neighbourNode.parentNode;
                                existingNode.direction = neighbourNode.direction;
                            }
                        }
                    }
                    nodeResponse.Remove(openNode);
                }
            }
            else
            {
                nodeResponse.Remove(openNode);
            }
        }
        return null;
    }

    public Node ChoosePath_A(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        Debug.Log("Busqueda A*");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;

        // Crear la lista abierta y cerrada
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Agregar el nodo inicial a la lista abierta
        Node startNode = new Node(currentPos, null, 0);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            tiempo_total += Time.deltaTime;

            // Obtener el nodo con el menor costo F
            Node currentNode = openList.OrderBy(node => node.F).First();

            // Si el nodo actual es el objetivo, entonces hemos terminado
            if (currentNode.cellInfo == goals[0])
            {
                Debug.Log("Nodos abiertos " + openList.Count);
                Debug.Log("Tiempo transcurrido " + tiempo_total);
                return currentNode;
            }

            // Mover el nodo actual de la lista abierta a la lista cerrada
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Explorar todos los vecinos del nodo actual
            CellInfo[] neighbourCells = currentNode.cellInfo.WalkableNeighbours(boardInfo);
            for (int i = 0; i < neighbourCells.Length; i++)
            {
                if (neighbourCells[i] != null)
                {
                    Node neighbourNode = new Node(neighbourCells[i], currentNode, i);

                    // Si el vecino ya est� en la lista cerrada, lo ignoramos
                    if (closedList.Any(node => node.cellInfo == neighbourNode.cellInfo))
                    {
                        continue;
                    }

                    // Calculamos los costos G, H y F del vecino
                    neighbourNode.G = currentNode.G + Vector2.Distance(currentNode.cellInfo.GetPosition, neighbourNode.cellInfo.GetPosition);
                    neighbourNode.H = Vector2.Distance(neighbourNode.cellInfo.GetPosition, goals[0].GetPosition);
                    neighbourNode.F = neighbourNode.G + neighbourNode.H;

                    // Si el vecino est� en la lista abierta y su nuevo costo F es m�s alto, lo ignoramos
                    if (openList.Any(node => node.cellInfo == neighbourNode.cellInfo) && neighbourNode.F > currentNode.F)
                    {
                        continue;
                    }

                    // De lo contrario, agregamos el vecino a la lista abierta
                    openList.Add(neighbourNode);
                }
            }
        }

        // Si llegamos a este punto, no se encontr� un camino
        return null;
    }


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (!first_time)
        {
            first_time = true;
            if (algorithOption == AlgorithmOption.ALGORITHM_UNIFORM_COST)
            {
                finalNode = ChoosePath_UCS(boardInfo, currentPos, goals);
            }    
            else if (algorithOption == AlgorithmOption.ALGORITHM_A)
            {
                finalNode = ChoosePath_A(boardInfo, currentPos, goals);
            }
            else if (algorithOption == AlgorithmOption.ALGORITHM_GENERAL_OPTIMIZED)
            {
                finalNode = ChoosePath_General_Optimized(boardInfo, currentPos, goals);
            }
            else if (algorithOption == AlgorithmOption.ALGORITHM_GENERAL)
            {
                finalNode = ChoosePath_General(boardInfo, currentPos, goals);
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
