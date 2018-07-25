using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    struct MarchingSquareTile
    {
        public int Rotation { get; private set; }
        public MarchingSquareTileType Type { get; private set; }

        public MarchingSquareTile(int rotation, MarchingSquareTileType type)
        {
            Rotation = rotation;
            Type = type;
        }

    }

    public enum MarchingSquareTileType { Empty, Single, Double, Triple, Quad}
}
