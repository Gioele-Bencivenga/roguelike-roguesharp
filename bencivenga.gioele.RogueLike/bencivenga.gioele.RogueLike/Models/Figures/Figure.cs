using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;

namespace bencivenga.gioele.RogueLike
{
    public class Figure
    {
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int AttackBonus { get; set; }
        public int ArmorClass { get; set; }
        public DiceExpression Damage { get; set; }
        public string DamageDice { get; set; }
        public DiceExpression HitDice { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
        public int MaxXP { get; set; }
        public string Status { get; set; }
        public Int64 TurnHit { get; set; }
        public Int64 TurnsPassed { get; set; }
        public Int16 Actions { get; set; }
        public Int16 MaxActions { get; set; }
        public int Fov { get; set; }

        public int Strenght { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int MODSTR { get; set; }
        public int MODDEX { get; set; }
        public int MODCON { get; set; }
        public int MODINT { get; set; }
        public int MODWIS { get; set; }
        public int MODCHA { get; set; }

        public int MODAR { get; set; }  //mod attack roll

        public Item Light { get; set; }
        public Item Weapon { get; set; }
        public Item Armor { get; set; }
    }
}
