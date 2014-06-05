using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Composing
{
    public struct BitmapSize
    {
        private int _width;
        private int _height;

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

        public BitmapSize(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}
