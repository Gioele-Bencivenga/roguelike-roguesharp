using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bencivenga.gioele.RogueLike
{
    public static class LayerDepht
    {
        public static readonly float Cells = 0.8f;  //ground layer
        public static readonly float Items = 0.7f;  //layer for chests, items and other things on the ground
        public static readonly float Paths = 0.6f;  //layer for paths only visible while debugging
        public static readonly float Figures = 0.5f;    //layer for the characters and other moving things
    }
}
