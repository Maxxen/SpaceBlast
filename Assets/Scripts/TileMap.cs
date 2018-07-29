using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class TileMap
    {
        private int width;
        private int height;
        private Random random;
        private Tile[,] tiles;

        public List<Room> Rooms { get; private set; }

        public TileMap(int tileWidth, int tileHeight, Random random)
        {
            tiles = new Tile[tileWidth,tileHeight];
            this.width = tileWidth;
            this.height = tileHeight;
            this.random = random;

        }

        public TileMap()
        {
            tiles = new Tile[20, 20];
            this.width = 20;
            this.height = 20;
            this.random = new Random(10);
            Room r = new Room(new Rectangle(new Point(0, 0), new Point(20, 20)));
            Rooms = new List<Room>();
            Rooms.Add(r);
            Fill(r.Dimensions, Tile.Wall);
            Fill(r.Dimensions.Inset(2), Tile.Floor);
            Fill(r.Dimensions.Inset(8), Tile.Wall);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x >= width || y >= height || x <= 0 || y <= 0)
            {
                //We cant set a tile outside of bounds.
                return;
            }
            else
            {
                tiles[x, y] = tile; 
            }
        }

        public Tile GetTile(int x, int y)
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

        public void Fill(Rectangle r, Tile tile)
        {
            for(int y = 0; y < r.Height; y++)
            {
                for(int x = 0; x < r.Width; x++)
                {
                    SetTile(r.TopLeft.X + x, r.TopLeft.Y + y, tile);
                }
            }
        }

        public MarchingSquareTile SampleTiles(int x, int y)
        {
            Tile sw = GetTile(x, y);
            Tile se = GetTile(x + 1, y);
            Tile ne = GetTile(x + 1, y + 1);
            Tile nw = GetTile(x, y + 1);

            int msTile = (int)nw * 1 + (int)ne * 2 + (int)se * 4 + (int)sw * 8;

            return new MarchingSquareTile(msTile);
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

            roomTree.Split(5, random);

            Rooms = new List<Room>();
            roomTree.GetLeaves(Rooms);

            //Go through all rooms and fill their corresponding tiles with floor.
            foreach (Room r in Rooms)
            {
                Fill(r.Dimensions.Inset(1), Tile.Floor);
            }

            ConnectRooms(roomTree);
        }

        //Recursively create hallways between every pair of split rooms.
        public void ConnectRooms(Room room)
        {
            if (!room.IsLeaf)
            {
                var l = room.Left;
                var r = room.Right;

                CreateHall(l, r);

                ConnectRooms(l);
                ConnectRooms(r);
            }
        }

        public void CreateHall(Room l, Room r)
        {
            var start = l.Dimensions.MidCenter - new Point(1, 1);
            var end = r.Dimensions.MidCenter + new Point(1, 1);
            var corridor = new Rectangle(start, end);

            Fill(corridor, Tile.Floor);
        }
    }
}
