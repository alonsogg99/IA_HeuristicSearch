using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{
    public class Node : ICloneable
    {
        public CellInfo cellInfo { get; }
        public Node parentNode { get; set; }
        public int direction { get; set; }
        public float G { get; set; }
        public float H { get; set; }
        public float F { get; set; }

        public Node(CellInfo cell, Node parent, int move)
        {
            this.cellInfo = cell;
            this.parentNode= parent;
            this.direction = move;
            this.G = 0f;
            this.H = 0f;
            this.F = 0f;
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