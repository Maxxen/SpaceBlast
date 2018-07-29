using Assets.Scripts.ColliderBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    struct MarchingSquareTile
    {
        public int Rotation { get; private set; }
        public MarchingSquareTileType Type { get; private set; }

        // The tiles are decided based on the sum of the corner values
        // The corners are assumed to have the following values if they are determined to be "solid", seen from above.
        // *-----------*
        // |1         2|
        // |           |
        // |           |
        // |8         4|
        // *-----------*

        public Edge? GetEdge(Vector3 position)
        {
            Vector2 midLeft = new Vector2(position.x - 0.5f, position.z);
            Vector2 midRight = new Vector2(position.x + 0.5f, position.z);
            Vector2 topCenter = new Vector2(position.x, position.z + 0.5f);
            Vector2 botCenter = new Vector2(position.x, position.z - 0.5f);

            switch (Type)
            {
                case MarchingSquareTileType.Empty:
                    return null;
                case MarchingSquareTileType.Single:
                    switch (Rotation)
                    {
                        case 0:
                            return new Edge(midLeft, topCenter);
                        case 90:
                            return new Edge(topCenter, midRight);
                        case 180:
                            return new Edge(midRight, botCenter);
                        case 270:
                            return new Edge(botCenter, midLeft);             
                    }
                    break;
                case MarchingSquareTileType.Double:
                    switch (Rotation)
                    {
                        case 0:
                            return new Edge(midLeft, midRight);
                        case 90:
                            return new Edge(topCenter, botCenter);
                        case 180:
                            return new Edge(midRight, midLeft);
                        case 270:
                            return new Edge(botCenter, topCenter);
                    }
                    break;
                case MarchingSquareTileType.Triple:
                    switch (Rotation)
                    {
                        case 0:
                            return new Edge(botCenter, midRight);
                        case 90:
                            return new Edge(midLeft, botCenter);
                        case 180:
                            return new Edge(topCenter, midLeft);
                        case 270:
                            return new Edge(midRight, topCenter);
                    }
                    break;
                case MarchingSquareTileType.Quad:
                    return null;
                default:
                    return null;
            }
            return null;
        }

        public MarchingSquareTile(int tile)
        {
            switch (tile)
            {
                case 0:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Empty;
                    break;
                case 1:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Single;
                    break;
                case 2:
                    this.Rotation = 90;
                    this.Type = MarchingSquareTileType.Single;
                    break;
                case 3:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Double;
                    break;
                case 4:
                    this.Rotation = 180;
                    this.Type = MarchingSquareTileType.Single;
                    break;
                case 5:
                    //Diagonal
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Quad;
                    break;
                case 6:
                    this.Rotation = 90;
                    this.Type = MarchingSquareTileType.Double;
                    break;
                case 7:
                    this.Rotation = 90;
                    this.Type = MarchingSquareTileType.Triple;
                    break;
                case 8:
                    this.Rotation = 270;
                    this.Type = MarchingSquareTileType.Single;
                    break;
                case 9:
                    this.Rotation = 270;
                    this.Type = MarchingSquareTileType.Double;
                    break;
                case 10:
                    //Diagonal
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Quad;
                    break;
                case 11:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Triple;
                    break;
                case 12:
                    this.Rotation = 180;
                    this.Type = MarchingSquareTileType.Double;
                    break;
                case 13:
                    this.Rotation = 270;
                    this.Type = MarchingSquareTileType.Triple;
                    break;
                case 14:
                    this.Rotation = 180;
                    this.Type = MarchingSquareTileType.Triple;
                    break;
                case 15:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Quad;
                    break;
                default:
                    this.Rotation = 0;
                    this.Type = MarchingSquareTileType.Empty;
                    break;
            }
        }
    }

    public enum MarchingSquareTileType { Empty, Single, Double, Triple, Quad}
}
