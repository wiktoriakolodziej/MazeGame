using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using MazeGame.Graphics;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Diagnostics;
using static Android.Icu.Text.CaseMap;

namespace MazeGame.Scenes
{
    public class PauseScene(Sprite ball, string levelName, Stopwatch watch) : Scene
    {

        private readonly Sprite _ball = ball;
        private readonly string _levelName = levelName;
        private readonly Stopwatch _watch = watch;
        private SpriteFont _font;
        private string _timeText;
        private Vector2 _timeTextPos;
        private Vector2 _timeTextOrig;
        private Panel _pausePanel;

        public override void Initialize()
        {
            base.Initialize();
            GumService.Default.Root.Children.Clear();
            _timeText = $"Current time: {watch.ElapsedMilliseconds / 1000}s";
            var size = _font.MeasureString(_timeText);
            _timeTextOrig = size * 0.5f;
            _timeTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);


            CreatePausePanel();
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ColorService.MenuBgColor);
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.DrawString(_font, _timeText, _timeTextPos, ColorService.MenuTextColor, 0.0f, _timeTextOrig, 1.0f * ColorService.FontSizeMultiplier, SpriteEffects.None, 1.0f);
            SpriteBatch.End();
            GumService.Default.Draw();
        }
        public override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("fonts/Roboto");
        }

        public override void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
        }
        private void CreatePausePanel()
        {
            _pausePanel = new Panel();
            _pausePanel.Dock(Gum.Wireframe.Dock.Fill);
            _pausePanel.AddToRoot();

            var resumeButton = new Button();    
            resumeButton.Text = "Resume";
            resumeButton.Anchor(Gum.Wireframe.Anchor.Top);
            resumeButton.Visual.XUnits = GeneralUnitType.Percentage;
            resumeButton.Visual.YUnits = GeneralUnitType.Percentage;
            resumeButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
            resumeButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
            resumeButton.Visual.X = 50;
            resumeButton.Visual.Y = 40;
            resumeButton.Visual.Width = 40f;
            resumeButton.Visual.Height = 10f;
            resumeButton.Click += OnResumeButtonClicked;
            _pausePanel.AddChild(resumeButton);

            var mainMenuButton = new Button();
            mainMenuButton.Text = "Main Menu";
            mainMenuButton.Anchor(Gum.Wireframe.Anchor.Top);
            mainMenuButton.Visual.XUnits = GeneralUnitType.Percentage;
            mainMenuButton.Visual.YUnits = GeneralUnitType.Percentage;
            mainMenuButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
            mainMenuButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
            mainMenuButton.Visual.X = 50;
            mainMenuButton.Visual.Y = 60;
            mainMenuButton.Visual.Width = 40f;
            mainMenuButton.Visual.Height = 10f;
            mainMenuButton.Click += HandleBackClicked;
            _pausePanel.AddChild(mainMenuButton);

        }

        private void OnResumeButtonClicked(object sender, EventArgs e)
        {
            RaiseSceneChanged(ScreenType.Gameplay, new() { { "ball", _ball }, { "levelName", _levelName }, { "watch", _watch } });
        }

        private void HandleBackClicked(object sender, EventArgs e)
        {
            RaiseSceneChanged(ScreenType.Title);
        }
    }
}
