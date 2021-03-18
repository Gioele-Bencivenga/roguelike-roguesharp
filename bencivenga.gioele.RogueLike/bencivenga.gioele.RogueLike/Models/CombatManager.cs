using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;
using System.Diagnostics;

namespace bencivenga.gioele.RogueLike
{
    public class CombatManager
    {
        private readonly Player _player;
        private readonly List<AggressiveEnemy> _aggressiveEnemies;

        public CombatManager(Player player, List<AggressiveEnemy> aggressiveEnemies)
        {
            _player = player;
            _aggressiveEnemies = aggressiveEnemies;
        }

        public void Attack(Figure attacker, Figure defender)
        {
            var attackDie = Dice.Parse("d20");

            if ( attackDie.Roll().Value + attacker.MODAR >= defender.ArmorClass)
            {
                int damage = attacker.Damage.Roll().Value;
                int actualDamage = 0;

                if(damage < defender.MODCON)    //damage reduction
                {
                    actualDamage = 1;
                }
                else
                {
                    actualDamage = damage - defender.MODCON;
                }

                defender.HitPoints -= actualDamage;

                if(defender.HitPoints < defender.MaxHitPoints)
                {
                    if(defender is AggressiveEnemy)
                    {
                        defender.Status = "Lightly hurt";
                    }
                    if(defender is Player)
                    {
                        _player.TurnHit = _player.TurnsPassed; 
                    }
                }
                if(defender.HitPoints <= defender.MaxHitPoints/2)
                {
                    if(defender is AggressiveEnemy)
                    {
                        defender.Status = "Hurt";
                    }
                }
                if(defender.HitPoints<=defender.MaxHitPoints/4)
                {
                    if(defender is AggressiveEnemy)
                    {
                        defender.Status = "Badly hurt";
                    }
                }
                if(defender.HitPoints == defender.MaxHitPoints - defender.MaxHitPoints + 2)
                {
                    if(defender is AggressiveEnemy)
                    {
                        defender.Status = "Dying";
                    }
                }
                if (defender.HitPoints <= 0)
                {
                    if (defender is AggressiveEnemy)
                    {
                        var enemy = defender as AggressiveEnemy;
                        _aggressiveEnemies.Remove(enemy);
                        _player.AddXP(_player.Level);
                    }
                }
            }
            else
            {
                
            }
        }

        public Figure FigureAt(int x, int y)
        {
            if (IsPlayerAt(x, y))
            {
                return _player;
            }
            return EnemyAt(x, y);
        }

        public bool IsPlayerAt(int x, int y)
        {
            return (_player.X == x && _player.Y == y);
        }

        public AggressiveEnemy EnemyAt(int x, int y)
        {
            foreach (var enemy in _aggressiveEnemies)
            {
                if (enemy.X == x && enemy.Y == y)
                {
                    return enemy;
                }
            }
            return null;
        }

        public bool IsEnemyAt(int x, int y)
        {
            return EnemyAt(x, y) != null;
        }
    }
}
