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
    public class AggressiveEnemy : Figure
    {
        private readonly PathToPlayer _path;
        private readonly IMap _map;
        private bool _isAwareOfPlayer;

        public int X { get; set; }
        public int Y { get; set; }
        public Texture2D Sprite { get; set; }
        public SpriteFont font { get; set; }

        Random rnd = new Random();

        public AggressiveEnemy(IMap map, PathToPlayer path)
        {
            _map = map;
            _path = path;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepht.Figures);
            spriteBatch.DrawString(font, "\nStatus: " + Status + "\nHP: " + HitPoints + "\nAware: " + _isAwareOfPlayer.ToString(), new Vector2((X * Sprite.Width), (Y * Sprite.Height) - 80), Color.WhiteSmoke);
            _path.Draw(spriteBatch);
        }

        public void Update(Player _player)
        {
            if (!_isAwareOfPlayer)
            {
                if (_map.IsInFov(X, Y))
                {
                    if (Dice.Parse("1d20").Roll().Value + MODWIS > Dice.Parse("1d20").Roll().Value + _player.MODDEX)
                    {
                        _isAwareOfPlayer = true;
                    }
                    else
                    {
                        _isAwareOfPlayer = false;
                    }
                }

                if (!_map.IsInFov(X, Y))
                {
                    if (Dice.Parse("1d20").Roll().Value + MODWIS > Dice.Parse("1d20").Roll().Value + _player.MODDEX + 20)
                    {
                        _isAwareOfPlayer = true;
                    }
                    else
                    {
                        _isAwareOfPlayer = false;
                    }
                }
            }

            if (_isAwareOfPlayer)
            {
                _path.CreateFrom(X, Y);

                if (Global.CombatManager.IsPlayerAt(_path.FirstCell.X, _path.FirstCell.Y))
                {
                    Global.CombatManager.Attack(this, Global.CombatManager.FigureAt(_path.FirstCell.X, _path.FirstCell.Y));
                }
                else
                {
                    if (Global.CombatManager.IsEnemyAt(_path.FirstCell.X, _path.FirstCell.Y))
                    {

                    }
                    else
                    {
                        X = _path.FirstCell.X;
                        Y = _path.FirstCell.Y;
                    }
                }

                if (!_map.IsInFov(X, Y))
                {
                    if (Dice.Parse("1d20").Roll().Value + MODWIS > Dice.Parse("1d20").Roll().Value + _player.MODDEX + 10)
                    {
                        _isAwareOfPlayer = true;
                    }
                    else
                    {
                        _isAwareOfPlayer = false;
                    }
                }

            }
        }
    }
}
