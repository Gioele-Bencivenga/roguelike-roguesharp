using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;

namespace bencivenga.gioele.RogueLike
{
    public class Item
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public Texture2D Sprite2 { get; set; }
        public SpriteFont font { get; set; }

        public string message = "E to use";
        public string message2 = "Used";

        public bool IsUsed { get; set; }

        public int durability { get; set; }
        public string DamageDice { get; set; }
        public string Name { get; set; }
    }
}
