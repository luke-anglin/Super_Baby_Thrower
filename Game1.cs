using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Super_Baby_Thrower.Sprites;
using Sprite = Super_Baby_Thrower.Sprites.Sprite;
using TiledCS;
 
namespace Super_Baby_Thrower {
    public class SuperBabyGame : Game
    {
        // Base 
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Point GameBounds = new Point(Constants.WindowWidth, Constants.WindowHeight); // Window Resolution
        private SpriteFont font;

        // Tiled
        private TiledMap map1Map;
        private TiledTileset tileset;
        private Texture2D tilesetTexture;
        private bool map1, mapShowing, won;
        private int tileWidth, tileHeight, tilesetTilesWide, tilesetTilesHeight;

        // Sprites, Textures, and Animations
        // Sprites
        private Thrower thrower;
        private List<Sprite> sprites;
        // Textures
        private Texture2D babyFlying, babyWalking, blueTrampoline, redTrampoline, wall, stairs, throwerSheet;
        // Animations
        private AnimationManager animationManager;
        private Dictionary<string, Animation> animations;

        public SuperBabyGame()
        {
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = GameBounds.X;
            _graphics.PreferredBackBufferHeight = GameBounds.Y;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load font 
            font = Content.Load<SpriteFont>("Font");

            // Tiled Variables
            map1Map = new TiledMap(Content.RootDirectory + "/map1.tmx");
            tileset = new TiledTileset(Content.RootDirectory + "/Tileset.tsx");
            tilesetTexture = Content.Load<Texture2D>("Tileset");
            tileWidth = tileset.TileWidth;
            tileHeight = tileset.TileHeight;
            tilesetTilesWide = tileset.Columns;
            tilesetTilesHeight = tileset.TileCount / tileset.Columns;

            // Load Textures 
            babyFlying = Content.Load<Texture2D>("BabyFlying");
            babyWalking = Content.Load<Texture2D>("BabyWalking");
            blueTrampoline = Content.Load<Texture2D>("BlueTrampoline");
            redTrampoline = Content.Load<Texture2D>("RedTrampoline");
            wall = Content.Load<Texture2D>("Wall"); 
            stairs = Content.Load<Texture2D>("Stairs");
            throwerSheet = Content.Load<Texture2D>("Thrower");

            // Animations 
            // Idle 
            Animation babyFlyingAnimation = new Animation(babyFlying, 12)
            {
                FrameSpeed = 0.05f,
                FrameWidth = 28
            };
            Animation babyWalkingAnimation = new Animation(babyWalking, 12)
            {
                FrameSpeed = 0.05f,
                FrameWidth = 28
            };

            Animation throwerAnimation = new Animation(throwerSheet, 4)
            {
                FrameSpeed = 0.1f,
                FrameWidth = 32
            };


            animations = new Dictionary<string, Animation>();
            animationManager = new AnimationManager(babyWalkingAnimation);

            // Add animations here
            animations.Add("babyWalking", babyWalkingAnimation);
            animations.Add("babyFlying", babyFlyingAnimation);
            animations.Add("throwerIdle", throwerAnimation);
            //animations.Add()

            sprites = new List<Sprite>();
            thrower = new Thrower(_graphics.GraphicsDevice, babyWalking, new Rectangle(0, 0, 32, 32), animations)
            {
                Position = new Vector2(0, GameBounds.Y - 32),
                Mass = 10f,
                id = "thrower"
            };

            // Add sprites here

            sprites.Add(thrower);
            LoadMap(map1Map);

        }

        protected void LoadMap(TiledMap _map)
        {

            for (var i = 0; i < _map.Layers[0].data.Length; i++)
            {
                int gid = _map.Layers[0].data[i];

                // Empty tile, do nothing
                if (gid == 0)
                {

                }
                else
                {
                    // Tileset tile ID
                    // 0 = wall
                    // 1 = stairs
                    // 2 = blueTrampoline
                    int tileFrame = gid - 1;
                    var tile = _map.GetTiledTile(_map.Tilesets[0], tileset, gid);

                    int column = tileFrame % tilesetTilesWide;
                    int row = (int)System.Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                    float x = (i % _map.Width) * _map.TileWidth;
                    float y = (float)System.Math.Floor(i / (double)_map.Width) * _map.TileHeight;


                    Sprite s;
                    switch (tileFrame)
                    {
                        case 0:
                            s = new Sprite(_graphics.GraphicsDevice, wall)
                            {
                                Position = new Vector2(x, y)
                            };
                            break;

                        case 1:
                            s = new Sprite(_graphics.GraphicsDevice, stairs)
                            {
                                Position = new Vector2(x, y)
                            };
                            break;
                        default:
                            s = new Sprite(_graphics.GraphicsDevice, blueTrampoline)
                            {
                                Position = new Vector2(x, y),
                                GravityApplies = false
                                
                            };
                            break;
                    }

                    sprites.Add(s);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!map1) // Later, change this to !map1 && !map2 && !map3...
            {
                // Get input
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.D1) || ks.IsKeyDown(Keys.NumPad1))
                    map1 = true;
                //else if (ks.IsKeyDown(Keys.D2) || ks.IsKeyDown(Keys.NumPad2))
                //    map2 = true;
            }

            thrower.Update(gameTime, sprites); // gets movement input

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);


            // position vectors from rectangles (for non-static objects)
            //var throwerPos = new Vector2(thrower.SpriteRectangle.X, thrower.SpriteRectangle.Y);


            _spriteBatch.Begin();

            // Sprite stuff

            // No map selected, show options
            if (!map1) // Later, change this to !map1 && !map2 && !map3...
            {
                _spriteBatch.DrawString(font, "Select a menu!\nHit the 1 key for map 1\nHit the 2 key for map 2", new Vector2(50, 300), Color.Black);
            }
            else if (map1 && !mapShowing)
            {
                LoadMap(map1Map);
                mapShowing = true;
            }
            if (map1) // later change to map1 || map2 || map3...
            {
                foreach (var sprite in sprites)
                {
                    sprite.Draw(gameTime, _spriteBatch);
                }

                //_spriteBatch.DrawString(font, string.Format("Coins: {0}", thrower.Counter.ToString()), new Vector2(50, 300), Color.Black);

                // 795 253
                //if (throwerPos.X > 700 && throwerPos.Y < 254)
                //{
                //    won = true;
                //    _spriteBatch.DrawString(font, "You win!", new Vector2(0, 50), Color.Red);
                //}
                //if (!won)
                //    _spriteBatch.DrawString(font, string.Format("thrower Position: {0}", throwerPos.ToString()), new Vector2(0, 50), Color.Orange);
            }


            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }

}


