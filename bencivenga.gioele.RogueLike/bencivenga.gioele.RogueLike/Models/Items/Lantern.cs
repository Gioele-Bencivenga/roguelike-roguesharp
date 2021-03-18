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
    public class Lantern : Item
    {
        private readonly IMap _map;

        public int Brightness { get; set; }

        public Lantern(IMap map)
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
                        if (_player.Light.durability < this.durability || _player.Light == _nothing)
                        {
                            message = "E to use";
                            _player.Light = this;

                            IsUsed = true;
                            _player.Actions--;

                            if (_player.Actions == 0)
                            {
                                _player.EndTurn(_player, _lantern, _nothing, _aggressiveEnemies);
                            }
                        }
                        else
                        {
                            message = "Less durable";
                        }
                    }
                }
            }

            if (_player.Light == this)
            {
                _player.Fov = Brightness;
            }

            if(_player.Light.durability==0)
            {
                _player.Light = _nothing;
            }

            if (_player.Light == _nothing)
            {
                _player.Fov = 3;
            }

        }
    }
}
