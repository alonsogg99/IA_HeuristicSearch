using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{
    public class Node : ICloneable
    {
        public CellInfo cellInfo { get; }
        public Node parentNode { get; }
        public int direction { get; }

        public Node(CellInfo cell, Node parent, int move)
        {
            this.cellInfo = cell;
            this.parentNode= parent;
            this.direction = move;
        }

        public object Clone()
        {
            var result = new Node(this.cellInfo, this.parentNode, this.direction);

            return result;
        }

        public override string ToString()
        {
            return "currentInfo: " + cellInfo.CellId + " parentInfo: " + parentNode.cellInfo.CellId + " direction: " + direction;
        }
    }
}