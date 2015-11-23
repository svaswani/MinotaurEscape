using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinotaurEscape
{
    class MenuItem
    {
        public MenuItem(Texture2D texture)
        {
            ButtonGraphic = texture;
        }

        public MouseState stateMouse
        {
            get; set; 
        }

        public Rectangle Rectangle
        {
            get; set;
        }

        public Texture2D ButtonGraphic
        {
            get; set;
        }

        public bool IsMouseInside()
        {
            stateMouse = Mouse.GetState();
            if (stateMouse.X >= Rectangle.X && stateMouse.X <= Rectangle.X + Rectangle.Width)
            {
                if (stateMouse.Y > Rectangle.Y && stateMouse.Y <= Rectangle.Y + Rectangle.Height)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(ButtonGraphic, Rectangle, Color.White);
        }
    }
}
