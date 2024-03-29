﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Baby_Thrower.Sprites
{
    public class AnimationManager
    {
        private Animation _animation;

        private float _timer;
        private bool _updated;
        private SpriteEffects se;

        public int FrameWidth
        {
            get
            {
                return _animation.FrameWidth;
            }
        }

        public int FrameHeight
        {
            get
            {
                return _animation.FrameHeight;
            }
        }

        public Vector2 Position { get; set; }

        public float Layer { get; set; }

        

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_updated)
                throw new Exception("Need to call 'Update' first");

            _updated = false;


            spriteBatch.Draw(_animation.Texture,
                             Position,
                             new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                                           0,
                                           _animation.FrameWidth,
                                           _animation.FrameHeight),
                             Color.White,
                             0f,
                             new Vector2(0, 0),
                             1f,
                             se,
                             Layer);
        }

        public void Play(Animation animation)
        {
            if (_animation == animation)
                return;

            _animation = animation;

            _animation.CurrentFrame = 0;

            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0f;

            _animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime, Vector2 Pos, SpriteEffects _se)
        {
            _updated = true;
            se = _se;
            Position = Pos; 
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }
    }
}
