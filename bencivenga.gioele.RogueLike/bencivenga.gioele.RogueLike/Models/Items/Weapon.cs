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
    public class Weapon : Item
    {
        private readonly IMap _map;

        public Weapon(IMap map)
        {
            _map = map;
        }

        public void Draw(SpriteBatch spriteBatch, Player _player)
        {
            if (IsUsed == false)
            {
                spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepht.Items);
            }

            if (_player.X == X && _player.Y == Y)
            {
                if (IsUsed == false)
                    spriteBatch.DrawString(font, message, new Vector2((X * Sprite.Width), (Y * Sprite.Height) - 30), Color.WhiteSmoke);
            }
        }

        public void Update(Player _player, InputState _inputState, Nothing _nothing, Lantern _lantern, List<AggressiveEnemy> _aggressiveEnemies)
        {
            if (_player.X == X && _player.Y == Y)
            {
                if (IsUsed == false)
                {
                    if (_inputState.IsActivateItem(PlayerIndex.One))
                    {
                        if (_player.Weapon == _nothing)
                        {
                            message = "E to use";
                            _player.Weapon = this;

                            IsUsed = true;
                            _player.Actions--;

                            if (_player.Actions == 0)
                            {
                                _player.EndTurn(_player, _lantern, _nothing, _aggressiveEnemies);
                            }
                        }
                        if(_player.Weapon != _nothing)
                        {
                            message = "E to use";

                            _player.Weapon.IsUsed = false;

                            if (_map.IsWalkable(_player.X, _player.Y + 1))
                            {
                                _player.Weapon.X = _player.X;
                                _player.Weapon.Y = _player.Y + 1;
                            }
                            else if(_map.IsWalkable(_player.X, _player.Y-1))
                            {
                                _player.Weapon.X = _player.X;
                                _player.Weapon.Y = _player.Y - 1;
                            }
                            else if (_map.IsWalkable(_player.X + 1, _player.Y))
                            {
                                _player.Weapon.X = _player.X + 1;
                                _player.Weapon.Y = _player.Y;
                            }
                            else if (_map.IsWalkable(_player.X - 1, _player.Y))
                            {
                                _player.Weapon.X = _player.X - 1;
                                _player.Weapon.Y = _player.Y;
                            }

                            _player.Weapon = this;

                            IsUsed = true;
                            _player.Actions--;

                            if (_player.Actions == 0)
                            {
                                _player.EndTurn(_player, _lantern, _nothing, _aggressiveEnemies);
                            }
                        }
                    }
                }
            }

            if (_player.Weapon == this)
            {
                _player.DamageDice = DamageDice;
            }

            if (_player.Weapon == _nothing)
            {
                _player.DamageDice = "1d4+";
            }

        }
    }
}
