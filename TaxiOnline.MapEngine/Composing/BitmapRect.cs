using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Composing
{
    public struct BitmapRect
    {
        private int _left;
        private int _top;
        private int _width;
        private int _height;

        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public BitmapRect(int left, int top, int width, int height)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        public BitmapRect Move(int xOffset, int yOffset)
        {
            return new BitmapRect(_left + xOffset, _top + yOffset, _width, _height);
        }
    }
}
