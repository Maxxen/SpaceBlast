using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Rectangle
    {

        public int Width { get { return BotRight.X - TopLeft.X; } }
        public int Height { get { return BotRight.Y - TopLeft.Y; } }
        public float Aspect { get { return Width / Height; } }


        ///These propeties correspond to the following geometrical points on a rectangle.
        /// *_______________*________________*
        /// | TopLeft   TopCenter   TopRight |
        /// |                                |
        /// * MidLeft   MidCenter   MidRight *
        /// |                                |
        /// | BotLeft   BotCenter   BotRight |
        /// *_______________*________________*

        public Point TopLeft { get; private set; }
        public Point TopCenter { get { return new Point(MidCenter.X, MidCenter.Y - (Height / 2)); } }
        public Point TopRight { get { return new Point(BotRight.X, TopLeft.Y); } }

        public Point MidLeft { get { return new Point(TopLeft.X, TopLeft.Y + Height / 2); } }
        public Point MidCenter { get { return new Point(TopLeft.X + Width / 2, TopLeft.Y + Height / 2); } }
        public Point MidRight { get { return new Point(BotRight.X, TopLeft.Y + Height / 2); } }

        public Point BotCenter { get { return new Point(MidCenter.X, MidCenter.Y + (Height / 2)); } }
        public Point BotRight { get; private set; }
        public Point BotLeft { get { return new Point(TopLeft.X, BotRight.Y); } }



        public Rectangle(Point topLeft, Point botRight)
        {
            TopLeft = topLeft;
            BotRight = botRight;
        }

        public Rectangle Inset (int margin)
        {
            if (margin < 1)
            {
                throw new ArgumentException("Inset margin can not be less than 1!");
            }

            return new Rectangle(new Point(TopLeft.X + margin, TopLeft.Y + margin), new Point(BotRight.X - margin, BotRight.Y - margin));
        }
    }
}
