using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class TileMap
    {
        int width;
        int height;

        public Tile[,] tiles;

        public TileMap(int tileWidth, int tileHeight)
        {
            tiles = new Tile[tileWidth,tileHeight];
            this.width = tileWidth;
            this.height = tileHeight;
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x >= width || y >= height || x <= 0 || y <= 0)
            {
                return;
            }
            else
            {
                tiles[x, y] = tile; 
            }
        }

        private Tile GetTile(int x, int y)
        {
            if (x >= width || y >= height || x <= 0 || y <= 0)
            {
                //If we try to get a tile outside of bounds, always return a wall tile.
                return Tile.Wall;
            }
            else
            {
                return tiles[x, y];
            }
        }

        public MarchingSquareTile SampleTiles(int x, int y)
        {
            Tile sw = GetTile(x, y);
            Tile se = GetTile(x + 1, y);
            Tile ne = GetTile(x + 1, y + 1);
            Tile nw = GetTile(x, y + 1);

            int msTile = (int)nw * 1 + (int)ne * 2 + (int)se * 4 + (int)sw * 8;

            switch (msTile)
            {
                case 0:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Empty);
                case 1:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Single);
                case 2:
                    return new MarchingSquareTile(90, MarchingSquareTileType.Single);
                case 3:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Double);
                case 4:
                    return new MarchingSquareTile(180, MarchingSquareTileType.Single);
                case 5:
                    //Diagonal
                    return new MarchingSquareTile(0, MarchingSquareTileType.Quad);
                case 6:
                    return new MarchingSquareTile(90, MarchingSquareTileType.Double);
                case 7:
                    return new MarchingSquareTile(90, MarchingSquareTileType.Triple);
                case 8:
                    return new MarchingSquareTile(270, MarchingSquareTileType.Single);
                case 9:
                    return new MarchingSquareTile(270, MarchingSquareTileType.Double);
                case 10:
                    //Diagonal
                    return new MarchingSquareTile(0, MarchingSquareTileType.Quad);
                case 11:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Triple);
                case 12:
                    return new MarchingSquareTile(180, MarchingSquareTileType.Double);
                case 13:
                    return new MarchingSquareTile(270, MarchingSquareTileType.Triple);
                case 14:
                    return new MarchingSquareTile(180, MarchingSquareTileType.Triple);
                case 15:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Quad);
                default:
                    return new MarchingSquareTile(0, MarchingSquareTileType.Empty);
            }
        }

        public void GenerateMap()
        {

            //Fill map with walls
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetTile(x, y, Tile.Wall);
                }
            }

            //Generate the room layout by recusively splitting rooms.
            var roomTree = new Room(new Rectangle(new Point(0, 0), new Point(width, height)));
            var rnd = new Random(1337);

            roomTree.Split(5, rnd);

            var rooms = new List<Room>();
            roomTree.GetLeaves(rooms);

            //Go through all rooms and fill their corresponding tiles with floor.
            foreach (Room r in rooms)
            {
                for (int y = r.Dimensions.TopLeft.Y + 1; y < r.Dimensions.BotRight.Y - 1; y++)
                {
                    for (int x = r.Dimensions.TopLeft.X + 1; x < r.Dimensions.BotRight.X - 1; x++)
                    {
                        SetTile(x, y, Tile.Floor);
                    }
                }
            }

            //Todo,
            ConnectRooms(roomTree);

            //TileLayout()

        }
        public void ConnectRooms(Room room)
        {
            if (!room.IsLeaf)
            {
                var l = room.Left;
                var r = room.Right;

                var start = l.Dimensions.MidCenter - new Point(1, 1);
                var end = r.Dimensions.MidCenter + new Point(1, 1);
            
                for (int y = start.Y; y < end.Y; y++)
                {
                    for (int x = start.X; x < end.X; x++)
                    {
                        SetTile(x, y, Tile.Floor);
                    }
                }
                ConnectRooms(l);
                ConnectRooms(r);
            }
        }
    }
}
