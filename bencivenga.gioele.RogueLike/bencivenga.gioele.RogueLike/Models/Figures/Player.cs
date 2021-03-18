using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;

namespace bencivenga.gioele.RogueLike
{ 
    public class Player : Figure
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }

        public Vector2 direction { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepht.Figures/*profondità del livello, dev'essere maggiore perchè l'immagine sia vista*/);
        }

        public bool HandleInput(InputState inputState, IMap map)
        {
            if (inputState.IsLeft(PlayerIndex.One))
            {
                int tempX = X - 1;

                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);

                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            if (inputState.IsRight(PlayerIndex.One))
            {
                int tempX = X + 1;

                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);
                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            if (inputState.IsUp(PlayerIndex.One))
            {
                int tempY = Y - 1;

                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            if (inputState.IsDown(PlayerIndex.One))
            {
                int tempY = Y + 1;

                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            if(inputState.IsSpace(PlayerIndex.One))
            {
                return true;
            }
            return false;
        }

        public void AddXP(int numberOfTimes)
        {
            for (int i = 0; i < numberOfTimes; i++)
            {
                XP += Dice.Parse("1d4").Roll().Value;

                if (XP >= MaxXP)
                {
                    LevelUp();
                }
            }
        }

        public void LevelUp()
        {
            XP = 0;
            MaxXP = MaxXP * 2;
            Level += 1;

            Random rnd = new Random();
            int k = rnd.Next(1, 6);

            #region Attribute UP
            if(k==1)
            {
                Strenght += 2;
            }
            if (k == 2)
            {
                Dexterity += 2;
            }
            if (k == 3)
            {
                Constitution += 2;
            }
            if (k == 4)
            {
                Intelligence += 2;
            }
            if (k == 5)
            {
                Wisdom += 2;
            }
            if (k == 6)
            {
                Charisma += 2;
            }
            #endregion

            #region Mods calculations
            MODSTR = (Strenght - 10) / 2;
            MODDEX = (Dexterity - 10) / 2;
            MODCON = (Constitution - 10) / 2;
            MODINT = (Intelligence - 10) / 2;
            MODWIS = (Wisdom - 10) / 2;
            MODCHA = (Charisma - 10) / 2;

            ArmorClass = 10 + MODDEX + MODCON;

            try
            {
                Damage = Dice.Parse(DamageDice + MODSTR);
            }
            catch { }

            int additionalHealth = 0;
            additionalHealth = Dice.Parse(HitDice.ToString()).Roll().Value + MODCON;

            if (MaxHitPoints + additionalHealth > 0)
            {
                MaxHitPoints = MaxHitPoints + additionalHealth;
            }
            else
            {
                MaxHitPoints += 1;
            }

            AttackBonus = Level;

            MODAR = AttackBonus + MODSTR + MODDEX;
            #endregion
        }

        public void Heal(int healAmount)
        {
            HitPoints += healAmount;
            if (HitPoints > MaxHitPoints)
                HitPoints = MaxHitPoints;
        }
        
        public void CheckRegeneration()
        {
            if(HitPoints<MaxHitPoints)
            {
                if(TurnsPassed == TurnHit + 10)
                {
                    Heal(1);
                    TurnHit = TurnsPassed;
                }
            }
            else
            {
                TurnHit = 0;
            }
        }

        public void EndTurn(Player _player, Lantern _lantern, Nothing _nothing, List<AggressiveEnemy> _aggressiveEnemies)
        {
            Actions = MaxActions;
            TurnsPassed++;

            if (_player.Light != _nothing)
            {
                _player.Light.durability--;
            }

            if (_aggressiveEnemies.Count == 0)
            {
                Global.GameState = GameStates.PlayerTurn;
            }
            else
            {
                Global.GameState = GameStates.EnemyTurn;
            }
        }
    }
}
