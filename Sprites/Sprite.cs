using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Baby_Thrower.Handlers;
namespace Super_Baby_Thrower.Sprites
{
    public class Sprite : Component
    {
        // Fields
        protected Texture2D _rectangleTexture;
        protected bool isSpritesheet;
        protected Rectangle spritesheetFrame;
        protected Texture2D _texture;
        protected PhysicsHandler physicsHandler;



        // Properties 
        public Vector2 Position;
        public float Mass { get; set; } = float.PositiveInfinity;
        public int Height { get; }
        public int Width { get; }
        public string id { get; set; }
        public bool GravityApplies { get; set; } = true; 
        public Rectangle SpriteRectangle
        {
            get
            {

                return this.isSpritesheet ? new Rectangle((int)Position.X, (int)Position.Y, spritesheetFrame.Width, spritesheetFrame.Height) : new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }


        }
        public bool Solid { get; set; } = true;
        public bool SolidFromTop { get; set; } = false;
        public Vector2 Speed = new Vector2(0, 0);
        public bool ShowRectangle { get; set; } = false;
        public bool OnGround { get; internal set; }
        public bool Collectible { get; internal set; }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        // Use for whole png 
        public Sprite(GraphicsDevice graphicsDevice, Texture2D texture)
          : this(texture)
        {
            isSpritesheet = false;
            SetRectangleTexture(graphicsDevice, texture);
            Height = texture.Height;
            Width = texture.Width; 
        }

        // Use for a spritesheet
        public Sprite(GraphicsDevice graphicsDevice, Texture2D texture, Rectangle spritesheetFrame)
          : this(texture)
        {
            isSpritesheet = true;
            this.spritesheetFrame = spritesheetFrame;
            SetRectangleTexture(graphicsDevice, texture); // border
            Height = spritesheetFrame.Height;
            Width = spritesheetFrame.Width;
        }

        // Issue is we're giving the entire spritesheet width not a single frame of spritesheet width 
        private void SetRectangleTexture(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            var height = isSpritesheet ? spritesheetFrame.Height : texture.Height;
            var width = isSpritesheet ? spritesheetFrame.Width : texture.Width;
            var colours = new List<Color>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0 || // On the top
                        x == 0 || // On the left
                        y == height - 1 || // on the bottom
                        x == width - 1) // on the right
                    {
                        colours.Add(new Color(255, 255, 255, 255)); // white
                    }
                    else
                    {
                        colours.Add(new Color(0, 0, 0, 0)); // transparent 
                    }
                }
            }

            _rectangleTexture = new Texture2D(graphicsDevice, width, height);
            _rectangleTexture.SetData<Color>(colours.ToArray());
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            physicsHandler = new PhysicsHandler(sprites);
            physicsHandler.Update(gameTime); // does all our physics for us 
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!isSpritesheet)
                spriteBatch.Draw(_texture, Position, Color.White);
            else
                spriteBatch.Draw(_texture, Position, spritesheetFrame, Color.White);

            if (ShowRectangle)
            {
                if (_rectangleTexture != null)
                    spriteBatch.Draw(_rectangleTexture, Position, Color.Red);

            }
        }

        #region Collision
        public bool IsTouchingLeft(Sprite sprite)
        {
            return this.SpriteRectangle.Right + this.Speed.X > sprite.SpriteRectangle.Left &&
              this.SpriteRectangle.Left < sprite.SpriteRectangle.Left &&
              this.SpriteRectangle.Bottom > sprite.SpriteRectangle.Top &&
              this.SpriteRectangle.Top < sprite.SpriteRectangle.Bottom;
        }

        public bool IsTouchingRight(Sprite sprite)
        {
            return this.SpriteRectangle.Left + this.Speed.X < sprite.SpriteRectangle.Right &&
              this.SpriteRectangle.Right > sprite.SpriteRectangle.Right &&
              this.SpriteRectangle.Bottom > sprite.SpriteRectangle.Top &&
              this.SpriteRectangle.Top < sprite.SpriteRectangle.Bottom;
        }

        public bool IsTouchingTop(Sprite sprite)
        {
            return this.SpriteRectangle.Bottom + this.Speed.Y > sprite.SpriteRectangle.Top &&
              this.SpriteRectangle.Top < sprite.SpriteRectangle.Top &&
              this.SpriteRectangle.Right > sprite.SpriteRectangle.Left &&
              this.SpriteRectangle.Left < sprite.SpriteRectangle.Right;
        }

        public bool IsTouchingBottom(Sprite sprite)
        {
            return this.SpriteRectangle.Top + this.Speed.Y < sprite.SpriteRectangle.Bottom &&
              this.SpriteRectangle.Bottom > sprite.SpriteRectangle.Bottom &&
              this.SpriteRectangle.Right > sprite.SpriteRectangle.Left &&
              this.SpriteRectangle.Left < sprite.SpriteRectangle.Right;
        }

        #endregion
        public override string ToString()
        {
            string throwerString;
            throwerString = String.Format("Speed: {0}\nPosition: {1}", Speed, Position);
            return throwerString;
        }
    }
}
