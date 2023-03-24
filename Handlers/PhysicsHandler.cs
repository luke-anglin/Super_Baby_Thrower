using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Baby_Thrower;
using System.Linq; 
using System.Collections.Generic;
using Super_Baby_Thrower.Sprites;
using System.Runtime.InteropServices;


namespace Super_Baby_Thrower.Handlers
{
    public class PhysicsHandler
    {
        public List<Sprite> Sprites { get; set; }


        public PhysicsHandler(List<Sprite> sprites)
        {
            Sprites = sprites; 
        }

        public void Update(GameTime gameTime)
        {
        
            ApplyForces();
            HandleCollisions();
            foreach (var sprite in Sprites)
            {
                if (sprite.GravityApplies)
                {
                    sprite.Speed = new Vector2(0, Constants.Gravity);
                }
            }
        }
        public void HandleCollisions()
        {
            List<Sprite> collected = new List<Sprite>();

            foreach (var s in Sprites)
            {
                s.OnGround = false; // figure out how to set to true
                foreach (var sprite in Sprites)
                {
                    if (sprite == s)
                        continue;

                    if (sprite.Solid)
                    {
                        if ((s.Speed.X > 0 && s.IsTouchingLeft(sprite)) ||
                            (s.Speed.X < 0 && s.IsTouchingRight(sprite)))
                        {
                            ApplyConservationOfMomentumX(sprite, s);
                            if (InBoundsX(sprite))
                            {
                                sprite.Position.X += sprite.Speed.X;
                            }

                        }

                        if ((s.Speed.Y > 0 && s.IsTouchingTop(sprite)) ||
                            (s.Speed.Y < 0 && s.IsTouchingBottom(sprite)))
                        {
                            if (s.Speed.Y > 0 && s.IsTouchingTop(sprite))
                                s.OnGround = true;
                            ApplyConservationOfMomentumY(sprite, s);
                            if (InBoundsY(sprite))
                            {
                                sprite.Position.Y += sprite.Speed.Y;
                            } 
                            
                        }
                    }
                    else if (sprite.SolidFromTop)
                    {

                        if ((s.Speed.Y > 0 && s.IsTouchingTop(sprite)))
                        {
                            ApplyConservationOfMomentumY(sprite, s);
                            if (InBoundsY(sprite))
                            {
                                sprite.Position.Y += sprite.Speed.Y;
                            }
                        }
                    }
                }


               
                if (!InBoundsY(s) && !float.IsNaN(s.Speed.Y))
                    s.OnGround = true; 

                if (InBoundsX(s))
                {
                    s.Position.X += s.Speed.X;
                }
                    
                if (InBoundsY(s))
                {
                    s.Position.Y += s.Speed.Y;
                }

            }
            foreach (var sprite in collected)
            {
                Sprites.Remove(sprite);
            }
        }

        public bool InBoundsX(Sprite s)
        {
            Vector2 newPos = s.Position + s.Speed;
            return newPos.X <= Constants.WindowWidth - s.Width && newPos.X >= 0;
        }

        public bool InBoundsY(Sprite s)
        {
            Vector2 newPos = s.Position + s.Speed;
            return newPos.Y <= Constants.WindowHeight - s.Height && newPos.Y >= 0;
        }

        public void ApplyForces()
        {
            foreach(var sprite in Sprites)
            {
                if (sprite.GravityApplies)
                    sprite.Speed.Y += Constants.Gravity;
            }
        }

        public static void ApplyConservationOfMomentumX(Sprite s1, Sprite s2)
        {
            var s1SpeedX = s1.Speed.X;
            var s2SpeedX = s2.Speed.X;

            s2.Speed.X = (2 * s1.Mass / (s1.Mass + s2.Mass)) * s1SpeedX - ((s1.Mass - s2.Mass) / (s1.Mass + s2.Mass)) * s2SpeedX;
            s1.Speed.X = ((s1.Mass - s2.Mass) / (s1.Mass + s2.Mass)) * s1SpeedX + (2 * s1.Mass / (s1.Mass + s2.Mass)) * s2SpeedX;
        }
        public static void ApplyConservationOfMomentumY(Sprite s1, Sprite s2)
        {
            var s1SpeedY = s1.Speed.Y;
            var s2SpeedY = s2.Speed.Y;

            s2.Speed.Y = (2 * s1.Mass / (s1.Mass + s2.Mass)) * s1SpeedY - ((s1.Mass - s2.Mass) / (s1.Mass + s2.Mass)) * s2SpeedY;
            s1.Speed.Y = ((s1.Mass - s2.Mass) / (s1.Mass + s2.Mass)) * s1SpeedY + (2 * s1.Mass / (s1.Mass + s2.Mass)) * s2SpeedY;

        }
    }
}
