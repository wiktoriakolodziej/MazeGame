using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using Gum.Converters;
using Gum.DataTypes;
using MazeGame.Services;


namespace MazeGame.Scenes;

public class TitleScene : Scene
{
    private const string TITLE = "Maze Ball";
    private SpriteFont _font;
    private Vector2 _titleTextPos;
    private Vector2 _titleTextOrigin;

    private Panel _titleScreenButtonsPanel;

    private int _buttonWidthPercentage = 40;
    private int _buttonHeightPercentage = 10;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();


        // Set the position and origin for the Dungeon text.
        Vector2 size = _font.MeasureString(TITLE);
        _titleTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
        _titleTextOrigin = size * 0.5f;

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Content.Load<SpriteFont>("fonts/04B1_30");
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorService.MenuBgColor);

        if (_titleScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Dungeon text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            SpriteBatch.DrawString(_font, TITLE, _titleTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _titleTextOrigin, 5.0f, SpriteEffects.None, 1.0f);

            // Draw the Dungeon text on top of that at its original position
            SpriteBatch.DrawString(_font, TITLE, _titleTextPos, Color.White, 0.0f, _titleTextOrigin, 5.0f, SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            SpriteBatch.End();
        }

        GumService.Default.Draw();
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        startButton.Visual.XUnits = GeneralUnitType.Percentage;
        startButton.Visual.YUnits = GeneralUnitType.Percentage;
        startButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        startButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        startButton.Visual.X = 50 - _buttonWidthPercentage / 2f;
        startButton.Visual.Y = 40;
        startButton.Visual.Width = _buttonWidthPercentage;
        startButton.Visual.Height = _buttonHeightPercentage;
        startButton.Text = "Levels";
        startButton.Click += HandleLevelsClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        var recordsButton = new Button();
        recordsButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        recordsButton.Visual.XUnits = GeneralUnitType.Percentage;
        recordsButton.Visual.YUnits = GeneralUnitType.Percentage;
        recordsButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        recordsButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        recordsButton.Visual.X = 50 - _buttonWidthPercentage/2f;
        recordsButton.Visual.Y = 55;
        recordsButton.Visual.Width = _buttonWidthPercentage;
        recordsButton.Visual.Height = _buttonHeightPercentage;
        recordsButton.Text = "Records";
        recordsButton.Click += HandleRecordsClicked;
        _titleScreenButtonsPanel.AddChild(recordsButton);

        var optionsButton = new Button();
        optionsButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        optionsButton.Visual.XUnits = GeneralUnitType.Percentage;
        optionsButton.Visual.YUnits = GeneralUnitType.Percentage;
        optionsButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        optionsButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        optionsButton.Visual.X = 50 - _buttonWidthPercentage / 2f;
        optionsButton.Visual.Y = 70;
        optionsButton.Visual.Width = _buttonWidthPercentage;
        optionsButton.Visual.Height = _buttonHeightPercentage;
        optionsButton.Text = "Options";
        optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(optionsButton);

    }

    private void HandleLevelsClicked(object sender, EventArgs e)
    {
        RaiseSceneChanged(ScreenType.LevelSizeSelection);
    }

    private void HandleRecordsClicked(object sender, EventArgs e)
    {
        RaiseSceneChanged(ScreenType.Records);
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        RaiseSceneChanged(ScreenType.Options);
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
    }

}

