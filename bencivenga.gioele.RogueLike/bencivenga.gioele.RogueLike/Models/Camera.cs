using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RogueSharp;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace bencivenga.gioele.RogueLike
{
    public class Camera
    {
        //Crea la classe camera senza scaling e con zoom standard
        public Camera()
        {
            Zoom = 0.8f;
        }

        //Posizione centrata camera in pixel
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; }
        public float Rotation { get; private set; }

        //Larghezza e altezza dell'area visibile quando il player ridimensiona la finestra di gioco
        public int ViewPortWidth { get; set; }
        public int ViewPortHeight { get; set; }

        //Il centro dell'area visibile non viene contato per lo scaling
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewPortWidth * 0.5f, ViewPortHeight * 0.5f);
            }
        }


        // create a matrix for the camera to offset everything we draw, the map and our objects. since the
        // camera coordinates are where the camera is, we offset everything by the negative of that to simulate
        // a camera moving. we also cast to integers to avoid filtering artifacts
        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        // Call this method with negative values to zoom out
        // or positive values to zoom in. It looks at the current zoom
        // and adjusts it by the specified amount. If we were at a 1.0f
        // zoom level and specified -0.5f amount it would leave us with
        // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
        public void AdjustZoom(float amount)
        {
            Zoom += amount;

            if(Zoom < 0.25f)
            {
                Zoom = 0.25f;
            }
        }

        // Move the camera in an X and Y amount based on the cameraMovement param.
        // if clampToMap is true the camera will try not to pan outside of the
        // bounds of the map.
        public void MoveCamera(Vector2 cameraMovement, bool clampToMap = false)
        {
            Vector2 newPosition = Position + cameraMovement;

            if(clampToMap)
            {
                Position = MapClampedPosition(newPosition);
            }
            else
            {
                Position = newPosition;
            }
        }

        public Rectangle ViewPortWorldBoundry()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner = ScreenToWorld(new Vector2(ViewPortWidth, ViewPortHeight));

            return new Rectangle((int)viewPortCorner.X, (int)viewPortCorner.Y, (int)(viewPortBottomCorner.X - viewPortCorner.X), (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        //camera centrata su di un pixel specifico
        public void CenterOn(Vector2 position)
        {
            Position = position;
        }

        //camera centrata su di una specifica cella della mappa
        public void CenterOn(Cell cell)
        {
            Position = CenteredPosition(cell, true);
        }

        private Vector2 CenteredPosition(Cell cell, bool clampToMap = false)
        {
            var cameraPosition = new Vector2(cell.X * Global.SpriteWidth, cell.Y * Global.SpriteHeight);
            var cameraCenteredOnTilePosition = new Vector2(cameraPosition.X + Global.SpriteWidth / 2, cameraPosition.Y + Global.SpriteHeight / 2);

            if(clampToMap)
            {
                return MapClampedPosition(cameraCenteredOnTilePosition);
            }

            return MapClampedPosition(cameraCenteredOnTilePosition);
        }

        //blocca(clamp) la camera in modo che non esca dall'area di mappa visibile
        private Vector2 MapClampedPosition(Vector2 position)
        {
            var cameraMax = new Vector2(Global.MapWidth * Global.SpriteWidth - (ViewPortWidth / Zoom / 2), Global.MapHeight * Global.SpriteHeight - (ViewPortHeight / Zoom / 2));

            return Vector2.Clamp(position, new Vector2(ViewPortWidth / Zoom / 2, ViewPortHeight / Zoom / 2), cameraMax);
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        //Muove la camera in base all'input
        public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer)
        {
            Vector2 cameraMovement = Vector2.Zero;

            if(inputState.IsScrollLeft(controllingPlayer))
            {
                cameraMovement.X = -1;
            }
            else if (inputState.IsScrollRight(controllingPlayer))
            {
                cameraMovement.X = 1;
            }
            if (inputState.IsScrollUp(controllingPlayer))
            {
                cameraMovement.Y = -1;
            }
            else if (inputState.IsScrollDown(controllingPlayer))
            {
                cameraMovement.Y = 1;
            }
            if (inputState.IsZoomIn(controllingPlayer))
            {
                AdjustZoom(0.25f);
            }
            else if (inputState.IsZoomOut(controllingPlayer))
            {
                AdjustZoom(-0.25f);
            }

            //Se si usa un controller questo normalizza i vettori quando l'input è diagonale
            if(cameraMovement!= Vector2.Zero)
            {
                cameraMovement.Normalize();
            }

            //Muove 25 pixel per secondo
            cameraMovement *= 25f;

            MoveCamera(cameraMovement, true);
        }
    }
}
