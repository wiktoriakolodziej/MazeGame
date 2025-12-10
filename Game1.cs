using MazeGame.Graphics;
using MazeGame.Maze;
using MazeGame.Scenes;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Devices.Sensors;
using System;
using System.Collections.Generic;

namespace MazeGame
{

    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        

        private Scene _activeScene;
        private Scene _nextScene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);    
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            ChangeScene(new GameScene(Content, GraphicsDevice, _spriteBatch));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // if there is a next scene waiting to be switch to, then transition
            // to that scene.
            if (_nextScene != null)
            {
                TransitionScene();
            }

            // If there is an active scene, update it.
            if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            // If there is an active scene, draw it.
            if (_activeScene != null)
            {
                _activeScene.Draw(gameTime);
            }


            base.Draw(gameTime);
        }

        public void ChangeScene(Scene next)
        {
            // Only set the next scene value if it is not the same
            // instance as the currently active scene.
            if (_activeScene != next)
            {
                _nextScene = next;
            }
        }

        private void TransitionScene()
        {
            // If there is an active scene, dispose of it.
            if (_activeScene != null)
            {
                _activeScene.Dispose();
            }

            // Force the garbage collector to collect to ensure memory is cleared.
            GC.Collect();

            // Change the currently active scene to the new scene.
            _activeScene = _nextScene;

            // Null out the next scene value so it does not trigger a change over and over.
            _nextScene = null;

            // If the active scene now is not null, initialize it.
            // Remember, just like with Game, the Initialize call also calls the
            // Scene.LoadContent
            if (_activeScene != null)
            {
                _activeScene.Initialize();
            }
        }
    }
}
