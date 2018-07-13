using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Room
    {

        public Rectangle Dimensions { get; private set; }

        public bool IsLeaf { get; private set; }
        public bool IsNode { get { return !IsLeaf; } }

        public Room Left { private set; get; }
        public Room Right { private set; get; }

        public Room(Rectangle dimensions)
        {
            IsLeaf = true;
            Dimensions = dimensions;
        }

        public void Split (int minRoomSize, Random rnd)
        {
            if (IsLeaf)
            {
                //If this room can't fit atleast two rooms + padding, we cant split it further and it remains a leaf
                if ((minRoomSize + 2) * 2 > Dimensions.Height || (minRoomSize + 2) * 2 > Dimensions.Width)
                {
                    return;
                }
                else
                {
                    IsLeaf = false;

                    if (Dimensions.Width >= Dimensions.Height)
                    {
                        //Vertical split

                        //We split the room in half, with some random offset, but make sure that we dont try to split too close to the edges.
                        //We also "inset" (shrink) the two newly created rooms by 1 in order to give them some padding.
                        var splitOffset = new Point(rnd.Next((-Dimensions.Width / 2) + minRoomSize, (Dimensions.Width / 2) - minRoomSize), 0);

                        Left = new Room(new Rectangle(Dimensions.TopLeft, Dimensions.BotCenter + splitOffset));
                        Right = new Room(new Rectangle(Dimensions.TopCenter + splitOffset, Dimensions.BotRight));

                    }
                    else
                    {
                        //Horizontal split
                        var splitOffset = new Point(0, rnd.Next((-Dimensions.Height / 2) + minRoomSize, (Dimensions.Height / 2) - minRoomSize));

                        Left = new Room(new Rectangle(Dimensions.TopLeft, Dimensions.MidRight + splitOffset));
                        Right = new Room(new Rectangle(Dimensions.MidLeft + splitOffset, Dimensions.BotRight));
                    }

                    //Recursively split the child rooms too.
                    Left.Split(minRoomSize, rnd);
                    Right.Split(minRoomSize, rnd);
                }
            }
            else
            {
                return;
            }
        }

        public void GetLeaves (List<Room> roomList)
        {
            if (IsLeaf)
            {
                //This room is a leaf, add itself
                roomList.Add(this);
                return;
            }
            else
            {
                //This room is a node, ask its children
                Left.GetLeaves(roomList);
                Right.GetLeaves(roomList);
                return;
            }
        }
    }
}
