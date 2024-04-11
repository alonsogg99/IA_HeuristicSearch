using Assets.Scripts.DataStructures;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using Microsoft.Win32.SafeHandles;

public enum AlgorithmOnlineOption{
    ALGORITHM_General,
    ALGORITHM_HillClimb
}

public class OnlineMind : AbstractPathMind
{
    Node finalNode = null;
    Node parentNode = null;

    bool goal_final = false;

    double tiempo_inicio = 0;

    public List<EnemyBehaviour> Enemies
        {
            get { return GameObject.FindObjectsOfType<EnemyBehaviour>().ToList(); }
            set { }
        }

    protected EnemyBehaviour closestEnemy = null;

    [SerializeField]
    AlgorithmOnlineOption algorithmOnlineOption;

    void Start()
    {
        // Debug.Log("Start");
        finalNode = null;
        tiempo_inicio = Time.time;
    }

    public EnemyBehaviour GetClosestEnemy(List<EnemyBehaviour> Enemies){
        Vector3 shorterPosition = Vector3.positiveInfinity;

        EnemyBehaviour bestEnemy = null;
    
        foreach(EnemyBehaviour enemy in Enemies){
            Vector3 enemyPosition = enemy.transform.position;

            if (Vector3.Distance(enemyPosition, this.transform.position) < Vector3.Distance(shorterPosition, this.transform.position))
            {   
                shorterPosition = enemy.transform.position;
                bestEnemy = enemy;
            }
        }
        return bestEnemy; 
    }

    public Node ChoosePath_A_Offline(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
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

    public Node ChoosePath_General_Online(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){

        /*  1º Hacer un array con todos los enemigos //* DONE

            2º   Cual es el mas cercano al jugador //* DONE

            3º   LOOP:  Busca la ruta óptima    //? DONE pero peta
                        Se mueve una casilla

            4º   Alcanza a un enemigo y lo elimina. Si quedan enemigos, vuelta al paso 2. //* DONE

            5º  Calcula la ruta hacia el goal y se mueve. //* DONE
        */

        Debug.Log("Busqueda General Online");
        int horizonte = 0;
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        List<Node> nodeResponse = new List<Node>();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);


        if (closestEnemy == null){
            closestEnemy = GetClosestEnemy(Enemies);
        }

        Debug.Log(closestEnemy.name);
        float bestOpt = Mathf.Infinity;
        while (nodeResponse.Count > 0)
        {
            if (closestEnemy == null)
            {
                break;
            }
            Debug.LogWarning("Horizonte: " + horizonte);
            Node openNode = nodeResponse[0];
            tiempo_total += Time.deltaTime;
            if (openNode.cellInfo != null && Enemies.Count > 0)
            {
                Debug.LogWarning("Enemigos hay");
                if (openNode.cellInfo.CellId == closestEnemy.CurrentPosition().CellId)
                {
                    Debug.LogWarning("Encontrado final");
                    Debug.Log("Nodos abiertos " + nodeResponse.Count);
                    Debug.Log("Tiempo transcurrido " + tiempo_total);
                    return openNode;
                }
                else if (horizonte >= 3)
                {
                    Debug.LogWarning("Final horizonte");
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            float option = Vector2.Distance(nextCells[i].GetPosition, closestEnemy.transform.position);
                            if (option < bestOpt)
                            {
                                Debug.LogWarning("Best option");
                                bestOpt = option;
                                nodeResponse.Insert(0, new Node(nextCells[i], openNode, i));
                            }
                            else if (!nodeResponse.Any(node => node.cellInfo == nextCells[i]))
                            {
                                Debug.LogWarning("Add");
                                nodeResponse.Add(new Node(nextCells[i], openNode, i));
                            }
                        }
                    }
                    nodeResponse.Remove(openNode);
                    Debug.LogWarning("Remove");
                }
            }
            else if (openNode.cellInfo != null && Enemies.Count <= 0)
            {
                Debug.LogWarning("No enemigos");
                if (openNode.cellInfo == goals[0])
                {
                    Debug.Log("Nodos abiertos " + nodeResponse.Count);
                    Debug.Log("Tiempo transcurrido " + tiempo_total);
                    goal_final = true;
                    return openNode;
                }
                else
                {
                    CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                    for (int i = 0; i < nextCells.Length; i++)
                    {
                        if (nextCells[i] != null)
                        {
                            if (!nodeResponse.Any(node => node.cellInfo == nextCells[i])) nodeResponse.Add(new Node(nextCells[i], openNode, i));
                        }
                    }
                    nodeResponse.Remove(openNode);
                }
            }
            else
            {
                nodeResponse.Remove(openNode);
            }
            horizonte++;
        }
        Debug.LogWarning("Devuelve nulo");
        return null;
    }

    public Node ChoosePath_HillClimbing(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){
        //Debug.Log("Busqueda Hill Climbing");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        // List<Node> nodeResponse = new List<Node>();


        Node startNode = new Node(currentPos, null, -1);
        // nodeResponse.Add(startNode);

        if (closestEnemy == null)
        {
            closestEnemy = GetClosestEnemy(Enemies);
            //Debug.Log("Distancia al enemigo: " + Vector2.Distance(closestEnemy.transform.position, this.transform.position));
        }

        // Debug.Log("closestEnemy " + (closestEnemy == null));
        // Debug.Log("closestEnemy position " + closestEnemy.transform.position);

        // Node openNode = nodeResponse[0];
        tiempo_total += Time.deltaTime;

        // Debug.Log("Enemies Count " + Enemies.Count);
        if (Enemies.Count > 0)
        {
            CellInfo[] nextCells = startNode.cellInfo.WalkableNeighbours(boardInfo);
            CellInfo bestCell = nextCells[0];
            float closestDistance = Mathf.Infinity;
            int bestIndex = 0;
            for (int i = 0; i < nextCells.Length; i++)
            {
                if (nextCells[i] != null){
                    //*Compruebo la distancia que hay hasta el enemigo desde cada celda valida
                    float distanceToEnemy = Vector2.Distance(nextCells[i].GetPosition, closestEnemy.transform.position);

                    if (distanceToEnemy < closestDistance)
                    {
                        //*Si la que estoy comprobando esta mas cercana que la que tenia guardada anteriormente, la guardo
                        closestDistance = distanceToEnemy;
                        bestCell = nextCells[i];
                        bestIndex = i;
                    }
                }
            } //TODO Lo que pasa es que siempre nos esta devolviendo bestindex = 0, por tanto siempre se mueve hacia arriba.

            // Debug.Log("Best index " + bestIndex);

            //* Aqui ya tendriamos la celda a la que movernos asique creamos y devolvemos un nodo con la informacion de dicha celda
            // nodeResponse.Remove(openNode);
            // nodeResponse.Add(new Node(bestCell, openNode, bestIndex));
            return new Node(bestCell, startNode, bestIndex);
        }
        else if (Enemies.Count <= 0)
        {
            // Debug.Log("Final de enemigos");
            goal_final = true;
            return ChoosePath_A_Offline(boardInfo, currentPos, goals);
        }
        else
        {
            // nodeResponse.Remove(openNode);
        }
        return null;
    }


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (!goal_final)
        {
            if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_General){
                finalNode = ChoosePath_General_Online(boardInfo, currentPos, goals);
            }    
            else if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_HillClimb){
                finalNode = ChoosePath_HillClimbing(boardInfo, currentPos, goals);
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
