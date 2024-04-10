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

    bool first = true;

    double tiempo_inicio = 0;

    public List<EnemyBehaviour> Enemies
        {
            get { return GameObject.FindObjectsOfType<EnemyBehaviour>().ToList(); }
            set { }
        }

    public EnemyBehaviour closestEnemy;

    [SerializeField]
    AlgorithmOnlineOption algorithmOnlineOption;

    void Start()
    {
        // Debug.Log("Start");
        finalNode = null;
        tiempo_inicio = Time.time;
    }

    public EnemyBehaviour GetClosestEnemy(List<EnemyBehaviour> Enemies){
        Vector3 shorterPosition = Enemies[0].transform.position;

        EnemyBehaviour bestEnemy = null;
    
        foreach(EnemyBehaviour enemy in Enemies){
            Vector3 enemyPosition = enemy.transform.position;

            if (Vector3.Distance(enemyPosition, this.transform.position) < Vector3.Distance(shorterPosition, this.transform.position)) bestEnemy = enemy;
        }
        return bestEnemy; 
    }

    public Node ChoosePath_1(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){

        /*  1º Hacer un array con todos los enemigos //* DONE

            2º   Cual es el mas cercano al jugador //* DONE

            3º   LOOP:  Busca la ruta óptima    //? DONE pero peta
                        Se mueve una casilla

            4º   Alcanza a un enemigo y lo elimina. Si quedan enemigos, vuelta al paso 2. //* DONE

            5º  Calcula la ruta hacia el goal y se mueve. //* DONE
        */

        Debug.Log("Busqueda General Online");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        List<Node> nodeResponse = new List<Node>();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);


        if (first){
            closestEnemy = GetClosestEnemy(Enemies);
            first = false;
        }

        Debug.Log(closestEnemy.name);

        while (nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse[0];
            tiempo_total += Time.deltaTime;
            if (openNode.cellInfo != null && Enemies.Count > 0)
            {
                if (openNode.cellInfo == Enemies[0].CurrentPosition())
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
            else if (openNode.cellInfo != null && Enemies.Count <= 0)
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

    public Node ChoosePath_HillClimbing(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals){
        /*  1º Hacer un array con todos los enemigos //*DONE

            2º   Cual es el mas cercano al jugador //*DONE

            3º   LOOP:  Busca el nodo adyacente más cercano al objetivo, no la ruta completa y se mueve a el
                        
            4º   Alcanza a un enemigo y lo elimina. Si quedan enemigos, vuelta al paso 2. //*DONE

            5º  Calcula la ruta hacia el goal y se mueve. //*DONE
        */

        Debug.Log("Busqueda General Online");
        double tiempo_total = 0;
        tiempo_total += Time.deltaTime;
        Node nodeResponse = new Node();

        Node startNode = new Node(currentPos, null, -1);
        nodeResponse.Add(startNode);


        if (first){
            closestEnemy = GetClosestEnemy(Enemies);
            first = false;
        }

        Debug.Log(closestEnemy.name);

        while (nodeResponse.Count > 0)
        {
            Node openNode = nodeResponse[0];
            tiempo_total += Time.deltaTime;

            if (openNode.cellInfo != null && Enemies.Count > 0){

                CellInfo[] nextCells = openNode.cellInfo.WalkableNeighbours(boardInfo);
                Vector2 closestDistance = Vector2.Distance(nextCells[0].GetPosition, closestEnemy.GetPosition);
                CellInfo bestCell;

                for (int i = 0; i < nextCells.length; i++){
                   if (Vector2.Distance(nextCells[i].GetPosition, closestEnemy.GetPosition) < closestDistance){
                    closestDistance = Vector2.Distance(nextCells[i].GetPosition, closestEnemy.GetPosition);
                    bestCell = nextCells[i];
                   }
                }
                //* Aqui ya tendriamos la celda a la que movernos
            }
            else if (openNode.cellInfo != null && Enemies.Count <= 0){
                
            }
            else{
                nodeResponse.Remove(openNode);
            }
        }
        return null;
    }


    public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
    {
        if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_1){
            finalNode = ChoosePath_1(boardInfo, currentPos, goals);
        }    
        else if (algorithmOnlineOption == AlgorithmOnlineOption.ALGORITHM_2){
            finalNode = ChoosePath_HillClimbing(boardInfo, currentPos, goals);
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
