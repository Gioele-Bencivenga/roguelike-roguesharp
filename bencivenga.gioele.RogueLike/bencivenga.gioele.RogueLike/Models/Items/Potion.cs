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
    public class Potion : Item
    {
        private readonly IMap _map;

        public Potion(IMap map)
        {
            _map = map;
        }

        public void Draw(SpriteBatch spriteBatch, Player _player)
        {
            if (IsUsed == true)
            {
                spriteBatch.Draw(Sprite2, new Vector2(X * Sprite2.Width, Y * Sprite2.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepht.Items);
            }
            if (IsUsed == false)
            {
                spriteBatch.Draw(Sprite, new Vector2(X * Sprite.Width, Y * Sprite.Height), null, null, null, 0.0f, Vector2.One, Color.White, SpriteEffects.None, LayerDepht.Items);
            }

            if (_player.X == X && _player.Y == Y)
            {
                if (IsUsed == false)
                    spriteBatch.DrawString(font, message, new Vector2((X * Sprite.Width), (Y * Sprite.Height) - 30), Color.WhiteSmoke);

                if (IsUsed == true)
                    spriteBatch.DrawString(font, message2, new Vector2((X * Sprite.Width), (Y * Sprite.Height) - 30), Color.WhiteSmoke);
            }
        }

        public void Update(Player _player, InputState _inputState)
        {
            if (_player.X == X && _player.Y == Y)
            {
                if (IsUsed == false)
                {
                    if (_inputState.IsActivateItem(PlayerIndex.One))
                    {
                        _player.Heal(Dice.Parse("1d6+3").Roll().Value + _player.MODINT);
                        _player.AddXP(1);

                        IsUsed = true;
                        _player.Actions--;

                        if(_player.Actions==0)
                        {
                            _player.Actions = _player.MaxActions;
                            _player.TurnsPassed++;

                            Global.GameState = GameStates.EnemyTurn;
                        }
                    }
                }
            }
        }
    }
}
