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

        // Properties
        public bool Attack, Jump;
        public int Counter { get; internal set; }

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
            SetAttributes();
            base.Update(gameTime, sprites);
            animationManager.Play(Animations["throwerIdle"]);
            animationManager.Update(gameTime, Position, Flip);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // check for idle key 
            animationManager.Draw(spriteBatch);
        }
        private void GetInput()
        {
            ks = Keyboard.GetState();
            Speed.X = ks.IsKeyDown(Keys.A) ? -Constants.BaseSpeed : Speed.X;
            Speed.X = ks.IsKeyDown(Keys.D) ? Constants.BaseSpeed : Speed.X;
            Speed.X = (!ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.D)) ? 0 : Speed.X;
        }
        private void SetAttributes()
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
