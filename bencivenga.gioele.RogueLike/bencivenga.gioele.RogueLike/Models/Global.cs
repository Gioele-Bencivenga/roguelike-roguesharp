using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.Random;

namespace bencivenga.gioele.RogueLike
{
    public enum GameStates
    {
        MainMenu,
        Pause,
        PlayerTurn,
        EnemyTurn,
        Debugging,
        EndGame
    }

    public class Global
    {
        public static readonly IRandom Random = new DotNetRandom();
        public static readonly Camera Camera = new Camera();

        public static GameStates GameState { get; set; }

        public static readonly int MapWidth = Random.Next(50, 150);
        public static readonly int MapHeight = Random.Next(50, 150);
        public static readonly int MaxRooms = ((MapWidth + MapHeight) / 2);
        public static readonly int MaxRoomBigness = Random.Next(5, 15);
        public static readonly int MinRoomBigness = Random.Next(2, 5);
        public static readonly int SpriteWidth = 64;
        public static readonly int SpriteHeight = 64;
        public static CombatManager CombatManager;
    }
}
