using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Baby_Thrower.Sprites
{
    public class Thrower : Sprite
    {
        // Fields
        private KeyboardState ks;
        private AnimationManager animationManager;
        private SpriteEffects prevFlip = SpriteEffects.None;
        public static Sprite Arrow; // MUST set this static variable in LoadContent in Game1.cs

        // Properties
        public Point ThrowPoint; 
        public bool DesiringThrow, Throw;  

        public SpriteEffects Flip;
        public Dictionary<string, Animation> Animations;

        public Thrower(GraphicsDevice graphicsDevice, Texture2D texture, Rectangle spritesheetFrame, Dictionary<string, Animation> animationDict) : base(graphicsDevice, texture, spritesheetFrame)
        {
            // set other attributes too;
            Speed = new Vector2(0, 0);
            Animations = animationDict;
            if (!Animations.ContainsKey("throwerIdle"))
                throw new Exception("Add a thrower idle animation");
            animationManager = new AnimationManager(Animations["throwerIdle"]);
            animationManager.Play(Animations["throwerIdle"]);
        }


        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            GetInput();
            base.Update(gameTime, sprites);
            SetFlip();
            animationManager.Play(Animations["throwerIdle"]);
            animationManager.Update(gameTime, Position, Flip);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw arrow based on input
            if (DesiringThrow) {
                System.Diagnostics.Debug.WriteLine("Reached, about to draw arrow.");
                Arrow.Position = new Vector2(Position.X + this.Width, Position.Y - 30);
                Arrow.Draw(gameTime, spriteBatch);
            }
            animationManager.Draw(spriteBatch);
        }
        private void GetInput()
        {
            ks = Keyboard.GetState();
            // Speed of Thrower
            Speed.X = ks.IsKeyDown(Keys.A) ? -Constants.BaseSpeed : Speed.X;
            Speed.X = ks.IsKeyDown(Keys.D) ? Constants.BaseSpeed : Speed.X;
            Speed.X = (!ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.D)) ? 0 : Speed.X;
            // Actions of Thrower
            MouseState ms = Mouse.GetState();
            // Possibilities:
            // Mouse click held down:
            //      DesiringThrow is true
            //      Throw is false
            // Mouse click dropped after being held down
            //      Throw 
            if (ms.LeftButton == ButtonState.Pressed) {
                DesiringThrow = true;
                Throw = false; 
                ThrowPoint = ms.Position;
            }
            else {
                Throw = DesiringThrow; 
                DesiringThrow = false;
                ThrowPoint = new Point(0,0); // for debugging, an unlikely point to throw 
            }
            Throw = !ks.IsKeyDown(Keys.Space) && DesiringThrow;
            DesiringThrow = ks.IsKeyDown(Keys.Space);
            Arrow.Scale = DesiringThrow ? Arrow.Scale + 0.1f : 1f;


        }
        private void SetFlip()
        {
            if (Speed.X > 0)
            {
                Flip = SpriteEffects.None;
                prevFlip = Flip; 
            }
            if (Speed.X < 0)
            {
                Flip = SpriteEffects.FlipHorizontally;
                prevFlip = Flip; 
            }
            if (Speed.X == 0)
            {
                Flip = prevFlip; 
            }
        }

    }
}
