using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.DataStructures
{
    public class Node : ICloneable
    {
        public CellInfo cellInfo { get; }
        public CellInfo parentCell { get; }
        public int direction { get; }

        public Node(CellInfo cell, CellInfo parent, int move)
        {
            this.cellInfo = cell;
            this.parentCell = parent;
            this.direction = move;
        }

        public object Clone()
        {
            var result = new Node(this.cellInfo, this.parentCell, this.direction);

            return result;
        }

        public override string ToString()
        {
            return "currentInfo: " + cellInfo.CellId + " parentInfo: " + parentCell.CellId + " direction: " + direction;
        }
    }
}