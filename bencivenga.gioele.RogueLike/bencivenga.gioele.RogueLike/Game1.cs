using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using System.Collections.Generic;
using RogueSharp.DiceNotation;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace bencivenga.gioele.RogueLike
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect backgroundSound;

        MainMenu main = new MainMenu();

        private Texture2D _floor;
        private Texture2D _wall;

        private IMap _map;

        public Player _player;
        public AggressiveEnemy _aggressiveEnemy;
        public FloorRock _floorRock;
        public Potion _potion;
        public Lantern _lantern;
        public Nothing _nothing;
        public Weapon _sword;

        public List<AggressiveEnemy> _aggressiveEnemies = new List<AggressiveEnemy>();
        private List<FloorRock> _floorRocks = new List<FloorRock>();
        private List<Potion> _potions = new List<Potion>();
        private List<Lantern> _lanterns = new List<Lantern>();
        private List<Weapon> _swords = new List<Weapon>();

        private InputState _inputState;

        private SpriteFont HUDfont;

        public HUD _hud;

        Random rnd;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _inputState = new InputState();
            rnd = new Random();

            _nothing = new Nothing()
            {
                durability = 0,
                DamageDice = "1d4+"
            };

            _potion = new Potion(_map);
            _lantern = new Lantern(_map);
            _sword = new Weapon(_map);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(Global.MapWidth, Global.MapHeight, Global.MaxRooms, Global.MaxRoomBigness, Global.MinRoomBigness);
            _map = Map.Create(mapCreationStrategy);

            //matches the window to the screen resolution and put it in fullscreen
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            //graphics.ToggleFullScreen();
            graphics.ApplyChanges();

            //centres the camera
            Global.Camera.ViewPortWidth = graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewPortHeight = graphics.GraphicsDevice.Viewport.Height;

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            backgroundSound = Content.Load<SoundEffect>("Audio/Environment/waterDripping");

            main.LoadContent(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            _floor = Content.Load<Texture2D>("Images/Map/floor");

            _wall = Content.Load<Texture2D>("Images/Map/wall");

            HUDfont = Content.Load<SpriteFont>("Fonts/HUDFont");

            Cell playerStartingCell = GetRandomEmptyCell();

            _nothing.Name = "Nothing";

            _player = new Player
            {
                Name = main.myName,

                X = playerStartingCell.X,
                Y = playerStartingCell.Y,
                Sprite = Content.Load<Texture2D>("Images/Player/player"),

                Strenght = Dice.Parse("3d6").Roll().Value,
                Dexterity = Dice.Parse("3d6").Roll().Value,
                Constitution = Dice.Parse("3d6").Roll().Value,
                Intelligence = Dice.Parse("3d6").Roll().Value,
                Wisdom = Dice.Parse("3d6").Roll().Value,
                Charisma = Dice.Parse("3d6").Roll().Value,

                Level = 1,
                XP = 0,
                MaxXP = 5,
                TurnsPassed = 0,
                Actions = 2,
                MaxActions = 2,
                Fov = 3,

                HitDice = Dice.Parse("1d12"),

                Weapon = _nothing,
                Armor=_nothing,
                Light = _nothing
            };

            #region Mods calculations
            _player.MODSTR = (_player.Strenght - 10) / 2;
            _player.MODDEX = (_player.Dexterity - 10) / 2;
            _player.MODCON = (_player.Constitution - 10) / 2;
            _player.MODINT = (_player.Intelligence - 10) / 2;
            _player.MODWIS = (_player.Wisdom - 10) / 2;
            _player.MODCHA = (_player.Charisma - 10) / 2;

            _player.ArmorClass = 10 + _player.MODDEX + _player.MODCON;

            _player.Damage = Dice.Parse("1d4+" + _player.MODSTR);

            _player.HitPoints = _player.HitDice.MaxRoll().Value + _player.MODCON;
            _player.MaxHitPoints = _player.HitDice.MaxRoll().Value + _player.MODCON;

            _player.AttackBonus = _player.Level;

            _player.MODAR = _player.AttackBonus + _player.MODSTR + _player.MODDEX;
            #endregion

            Global.Camera.CenterOn(playerStartingCell);

            AddAggressiveEnemies(1);
            AddFloorRocks(Dice.Parse("2d100").Roll().Value);
            AddPotions(0);
            AddLanterns(Dice.Parse("2d8").Roll().Value);
            AddWeapons(Dice.Parse("3d8").Roll().Value);

            Global.CombatManager = new CombatManager(_player, _aggressiveEnemies);

            UpdatePlayerFieldOfView();

            Global.GameState = GameStates.MainMenu;
        }

        //plays the music once the game started
        protected override void BeginRun()
        {
            var backSound = backgroundSound.CreateInstance();
            backSound.IsLooped = true;
            backSound.Play();

            base.BeginRun();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (Global.GameState)
            {
                case GameStates.MainMenu:
                    UpdateMainMenu(gameTime);
                    break;

                case GameStates.Pause:
                    UpdatePause(gameTime);
                    break;

                case GameStates.PlayerTurn:
                    UpdatePlayerTurn(gameTime);
                    break;

                case GameStates.EnemyTurn:
                    UpdateEnemyTurn(gameTime);
                    break;

                case GameStates.Debugging:
                    UpdateDebugging(gameTime);
                    break;

                case GameStates.EndGame:
                    UpdateEndGame(gameTime);
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            switch (Global.GameState)
            {
                case GameStates.MainMenu:
                    DrawMainMenu(gameTime);
                    break;
                case GameStates.Pause:
                    DrawPause(gameTime);
                    break;
                case GameStates.PlayerTurn:
                    DrawGameplay(gameTime);
                    break;
                case GameStates.EnemyTurn:
                    DrawGameplay(gameTime);
                    break;
                case GameStates.Debugging:
                    DrawGameplay(gameTime);
                    break;
                case GameStates.EndGame:
                    DrawEndGame(gameTime);
                    break;
            }
        }

        private Cell GetRandomEmptyCell()
        {
            while (true)
            {
                int x = Global.Random.Next(_map.Width - 1);
                int y = Global.Random.Next(_map.Height - 1);

                if (_map.IsWalkable(x, y))
                {
                    return _map.GetCell(x, y);
                }
            }
        }

        private void UpdatePlayerFieldOfView()
        {
            _map.ComputeFov(_player.X, _player.Y/*where to start calculating*/, _player.Fov/*distance of view*/, true/*walls or not walls*/);

            foreach (Cell cell in _map.GetAllCells())
            {
                if (_map.IsInFov(cell.X, cell.Y))
                {
                    _map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);  //quando setta true significa che la cella è stata vista
                }
            }
        }

        private void AddFloorRocks(int numberOfRocks)
        {
            for (int i = 0; i < numberOfRocks; i++)
            {
                Cell rockyCell = GetRandomEmptyCell();

                var rock = new FloorRock(_map)
                {
                    Sprite = Content.Load<Texture2D>("Images/Map/rocks"),
                    X = rockyCell.X,
                    Y = rockyCell.Y
                };

                _floorRocks.Add(rock);
            }
        }

        private void AddPotions(int numberOfPotions)
        {
            numberOfPotions += ((Global.MapWidth + Global.MapHeight) / 2) / 10;

            for (int i=0;i<numberOfPotions;i++)
            {
                Cell potionCell = GetRandomEmptyCell();

                Potion potion = new Potion(_map)
                {
                    Sprite = Content.Load<Texture2D>("Images/Items/Potion/potion"),
                    Sprite2 = Content.Load<Texture2D>("Images/Items/Potion/potion2"),
                    X = potionCell.X,
                    Y = potionCell.Y,
                    IsUsed = false,
                    font = Content.Load<SpriteFont>("Fonts/HUDFont")
                };

                _potions.Add(potion);
            }
        }

        private void AddLanterns(int numberOfLanterns)
        {
            for (int i = 0; i < numberOfLanterns; i++)
            {
                Cell lanternCell = GetRandomEmptyCell();

                Lantern lantern = new Lantern(_map)
                {
                    Sprite = Content.Load<Texture2D>("Images/Items/Lantern/lantern"),
                    X = lanternCell.X,
                    Y = lanternCell.Y,
                    IsUsed = false,
                    font = Content.Load<SpriteFont>("Fonts/HUDFont"),
                    durability = rnd.Next(200, 700),
                    Brightness = rnd.Next(5, 9)
                };
                if (lantern.Brightness <= 5)
                    lantern.Name = "Dim lantern";
                if (lantern.Brightness > 5 && lantern.Brightness <= 7)
                    lantern.Name = "Lantern";
                if (lantern.Brightness > 7)
                    lantern.Name = "Bright lantern";

                _lanterns.Add(lantern);
            }
        }

        private void AddWeapons(int numberOfWeapons)
        {
            for(int i=0;i<numberOfWeapons;i++)
            {
                Cell swordCell = GetRandomEmptyCell();
                int spriteNum = rnd.Next(1, 4);
                Weapon weapon = new Weapon(_map)
                {
                    Sprite = Content.Load<Texture2D>("Images/Items/Sword/sword" + spriteNum.ToString()),
                    X = swordCell.X,
                    Y = swordCell.Y,
                    IsUsed = false,
                    font = Content.Load<SpriteFont>("Fonts/HUDFont"),
                    DamageDice = rnd.Next(1,3) + "d" + rnd.Next(3,10) + "+" + rnd.Next(-1, 1) + "+"
                };
                if(spriteNum<=4)
                {
                    weapon.Name = "Sword";
                }
                _swords.Add(weapon);
            }
        }

        private void AddAggressiveEnemies(int numberOfEnemies)
        {
            numberOfEnemies += ((Global.MapWidth + Global.MapHeight) / 2) / 5;

            for (int i = 0; i < numberOfEnemies; i++)
            {
                Cell enemyCell = GetRandomEmptyCell();
                var pathFromAggressiveEnemy = new PathToPlayer(_player, _map, Content.Load<Texture2D>("Images/Debug/white"));

                var enemy = new AggressiveEnemy(_map, pathFromAggressiveEnemy)
                {
                    Name = "Hound",
                    font = Content.Load<SpriteFont>("Fonts/HUDFont"),
                    X = enemyCell.X,
                    Y = enemyCell.Y,

                    Strenght = Dice.Parse("2d8").Roll().Value,
                    Dexterity = Dice.Parse("2d8").Roll().Value,
                    Constitution = Dice.Parse("2d8").Roll().Value,
                    Intelligence = Dice.Parse("2d8").Roll().Value,
                    Wisdom = Dice.Parse("2d8").Roll().Value,
                    Charisma = Dice.Parse("2d8").Roll().Value,

                    Status = "Unhurt",
                    Actions = 2,

                    AttackBonus = 0,
                    HitDice = Dice.Parse("1d12")
                };

                if(enemy.Constitution>= 15)
                {
                    enemy.Sprite = Content.Load<Texture2D>("Images/Enemies/Hound/houndBig");
                }
                if(enemy.Constitution<=8)
                {
                    enemy.Sprite = Content.Load<Texture2D>("Images/Enemies/Hound/houndSmall");
                }
                else
                {
                    enemy.Sprite = Content.Load<Texture2D>("Images/Enemies/Hound/hound");
                }

                if(enemy.Charisma>=15)  //charisma control is done after since if the hound has cha he is good at hiding carateristics
                {
                    enemy.Sprite = Content.Load<Texture2D>("Images/Enemies/Hound/houndCha");
                }

                #region Mods calculations
                enemy.MODSTR = (enemy.Strenght - 10) / 2;
                enemy.MODDEX = (enemy.Dexterity - 10) / 2;
                enemy.MODCON = (enemy.Constitution - 10) / 2;
                enemy.MODINT = (enemy.Intelligence - 10) / 2;
                enemy.MODWIS = (enemy.Wisdom - 10) / 2;
                enemy.MODCHA = (enemy.Charisma - 10) / 2;

                enemy.ArmorClass = 10 + enemy.MODDEX + enemy.MODCON;

                enemy.Damage = Dice.Parse("1d4+" + enemy.MODSTR);

                enemy.HitPoints = enemy.HitDice.MaxRoll().Value + enemy.MODCON;

                enemy.MODAR = enemy.AttackBonus + enemy.MODSTR + enemy.MODDEX;
                #endregion

                _aggressiveEnemies.Add(enemy);
            }
        }

        void UpdateMainMenu(GameTime deltaTime)
        {
            main.Update();
            _player.Name = main.myName;
        }
        
        void UpdatePause(GameTime deltaTime)
        {
            //check the pressing of buttons
        }

        void UpdatePlayerTurn(GameTime gameTime)
        {
            foreach (var _potion in _potions)
            {
                _potion.Update(_player, _inputState);
            }

            foreach(var _torch in _lanterns)
            {
                _torch.Update(_player, _inputState, _nothing, _lantern, _aggressiveEnemies);
            }

            foreach(var _sword in _swords)
            {
                _sword.Update(_player, _inputState, _nothing, _lantern, _aggressiveEnemies);
            }

            if (_player.HitPoints <= 0)
            {
                Global.GameState = GameStates.EndGame;
            }

            if (_inputState.IsExitGame(PlayerIndex.One))    //pressinc esc closes the game
            {
                Global.GameState = GameStates.EndGame;
            }

            if (_inputState.IsRightControl(PlayerIndex.One))   //pressing right control changes between debug and game mode
            {
                Global.GameState = GameStates.Debugging;
            }

            if (_player.HandleInput(_inputState, _map))    //if a player input is pressed and it's his turn then he moves
            {
                UpdatePlayerFieldOfView();

                Global.Camera.CenterOn(_map.GetCell(_player.X, _player.Y));

                _player.Actions--;    //decreases counter for actions in one turn

                if (_player.Actions == 0)
                {
                    _player.EndTurn(_player, _lantern,_nothing, _aggressiveEnemies);
                }
            }

            _player.CheckRegeneration();

            _inputState.Update();

            Global.Camera.HandleInput(_inputState, PlayerIndex.One);
        }

        void UpdateEnemyTurn(GameTime gameTime)
        {
            foreach (var enemy in _aggressiveEnemies)
            {
                if (_aggressiveEnemies.Count() >= 1)
                {
                    enemy.Update(_player);
                    enemy.Actions--;

                    if (enemy.Actions == 0)
                    {
                        enemy.Actions = 2;
                        Global.GameState = GameStates.PlayerTurn;
                    }
                }
                else
                {
                    Global.GameState = GameStates.PlayerTurn;
                }
            }
        }

        void UpdateDebugging(GameTime gameTime)
        {
            if (_inputState.IsRightControl(PlayerIndex.One))   //pressing right control changes between debug and game mode
            {
                Global.GameState = GameStates.PlayerTurn;
            }

            _inputState.Update();

            Global.Camera.HandleInput(_inputState, PlayerIndex.One);
        }

        void UpdateEndGame(GameTime gameTime)
        {
            this.Exit();
        }

        void DrawMainMenu(GameTime gameTime)
        {
            spriteBatch.Begin();

            main.Draw(spriteBatch);

            spriteBatch.End();
        }

        void DrawPause(GameTime gameTime)
        {

        }

        void DrawGameplay(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            #region Map and figures

            //draws elements that will be scalated by the camera
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            foreach (Cell cell in _map.GetAllCells())
            {
                var position = new Vector2(cell.X * Global.SpriteWidth, cell.Y * Global.SpriteHeight);

                if (!cell.IsExplored && Global.GameState != GameStates.Debugging)
                {
                    continue;
                }
                Color tint = Color.White;
                if (!cell.IsInFov && Global.GameState != GameStates.Debugging)
                {
                    tint = Color.Gray;
                }
                if (cell.IsWalkable)
                {
                    spriteBatch.Draw(_floor, position, null, null, null, 0.0f, Vector2.One, tint, SpriteEffects.None, LayerDepht.Cells);
                }
                else
                {
                    spriteBatch.Draw(_wall, position, null, null, null, 0.0f, Vector2.One, tint, SpriteEffects.None, LayerDepht.Cells);
                }
            }

            _player.Draw(spriteBatch);

            foreach (var rock in _floorRocks)
            {
                if (Global.GameState == GameStates.Debugging || _map.IsInFov(rock.X, rock.Y))
                {
                    rock.Draw(spriteBatch);
                }
            }

            foreach (var potion in _potions)
            {
                if (Global.GameState == GameStates.Debugging || _map.IsInFov(potion.X, potion.Y))
                {
                    potion.Draw(spriteBatch, _player);
                }
            }

            foreach (var enemy in _aggressiveEnemies)
            {
                if (Global.GameState == GameStates.Debugging || _map.IsInFov(enemy.X, enemy.Y))
                {
                    enemy.Draw(spriteBatch);
                }
            }

            foreach(var _torch in _lanterns)
            {
                if (Global.GameState == GameStates.Debugging || _map.IsInFov(_torch.X, _torch.Y))
                {
                    _torch.Draw(spriteBatch, _player);
                }
            }

            foreach (var _sword in _swords)
            {
                if (Global.GameState == GameStates.Debugging || _map.IsInFov(_sword.X, _sword.Y))
                {
                    _sword.Draw(spriteBatch, _player);
                }
            }
            
            spriteBatch.End();

            #endregion

            #region HUD

            //draws non-scalated elements on the screen
            spriteBatch.Begin(SpriteSortMode.Immediate, null);

            _hud = new HUD(Vector2.Zero, spriteBatch, HUDfont, GraphicsDevice);
            _hud.Enable(true);

            string playerStatus = "Name: " + _player.Name + "\n" +
                "\n" +
                "HP: " + _player.HitPoints.ToString() + " / " + _player.MaxHitPoints + "\n" +
                "AC: " + _player.ArmorClass + "\n" +
                "\n" +
                "STR: " + _player.Strenght + " | " + _player.MODSTR + "\n" +
                "DEX: " + _player.Dexterity + " | " + _player.MODDEX + "\n" +
                "CON: " + _player.Constitution + " | " + _player.MODCON + "\n" +
                "INT: " + _player.Intelligence + " | " + _player.MODINT + "\n" +
                "WIS: " + _player.Wisdom + " | " + _player.MODWIS + "\n" +
                "CHA: " + _player.Charisma + " | " + _player.MODCHA + "\n" +
                "\n" +
                "Actions: " + _player.Actions.ToString() + "\n" +
                "Turns passed: " + _player.TurnsPassed.ToString() + "\n" +
                "\n" +
                "Level: " + _player.Level.ToString() + "\n" +
                "XP: " + _player.XP + " / " + _player.MaxXP + "\n" + 
                "\n" + 
                "Weapon: " + _player.Weapon.Name + "(" + _player.Weapon.DamageDice.ToString() + ")" + "\n" + 
                "Armor: " + _player.Armor.Name + "\n" + 
                "Light: " + _player.Light.Name + "(" + _player.Light.durability.ToString() + ")" + "\n"
                ;

            _hud.Update(playerStatus, Color.White);
            _hud.Draw();

            spriteBatch.End();

            #endregion
        }

        void DrawEndGame(GameTime gameTime)
        {

        }
    }
}
