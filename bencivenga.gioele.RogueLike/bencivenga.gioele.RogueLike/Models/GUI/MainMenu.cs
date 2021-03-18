using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bencivenga.gioele.RogueLike
{
    public class MainMenu
    {
        enum MenuStates
        {
            Main,
            Name
        }

        MenuStates menuState;

        public string myName = string.Empty;

        private SpriteFont font;

        List<GUIElement> main = new List<GUIElement>();
        List<GUIElement> name = new List<GUIElement>();

        private Keys[] lastKeys = new Keys[10];

        public MainMenu()
        {
            main.Add(new GUIElement("Images/Menu/back"));
            main.Add(new GUIElement("Images/Menu/btn_Play"));

            name.Add(new GUIElement("Images/Menu/int_Name"));
            name.Add(new GUIElement("Images/Menu/btn_Apply"));
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/HUDFont");

            foreach(GUIElement element in main)
            {
                element.LoadContent(content);
                element.CenterElement(1280, 720);
                element.clickEvent += OnClick;
            }
            main[1].MoveElement(-400, -200);//-300
            foreach (GUIElement element in name)
            {
                element.LoadContent(content);
                element.CenterElement(1280, 720);
                element.clickEvent += OnClick;
            }
            name[1].MoveElement(0, 300);//400
        }

        public void Update()
        {
            switch (menuState)
            {
                case MenuStates.Main:
                    foreach (GUIElement element in main)
                    {
                        element.Update();
                    }
                    break;

                case MenuStates.Name:
                    foreach (GUIElement element in name)
                    {
                        element.Update();
                    }
                    GetKeys();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (menuState)
            {
                case MenuStates.Main:
                    foreach (GUIElement element in main)
                    {
                        element.Draw(spriteBatch);
                    }
                    break;

                case MenuStates.Name:
                    foreach (GUIElement element in name)
                    {
                        element.Draw(spriteBatch);
                    }
                    spriteBatch.DrawString(font, myName, new Vector2(/*420*/100, /*600*/440), Color.White);
                    break;
            }            
        }

        public void OnClick(string element)
        {
            if(element=="Images/Menu/btn_Play")
            {
                menuState = MenuStates.Name;
            }
            if(element=="Images/Menu/btn_Apply")
            {
                Global.GameState = GameStates.PlayerTurn;
            }
        }

        public void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();

            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in lastKeys)
            {
                if (!pressedKeys.Contains(key))
                {
                    OnKeyUp(key);
                }
            }
            foreach (Keys key in pressedKeys)
            {
                if (!lastKeys.Contains(key))
                {
                    OnKeyDown(key);
                }
            }
            lastKeys = pressedKeys;
        }

        public void OnKeyUp(Keys key)
        {
            
        }

        public void OnKeyDown(Keys key)
        {
            if(key==Keys.Space)
            {

            }
            else if (key == Keys.Back && myName.Length >= 1)
            {
                myName = myName.Remove(myName.Length - 1);
            }
            else if(key!=Keys.Back)
            {
                myName += key.ToString();
            }
        }
    }
}
