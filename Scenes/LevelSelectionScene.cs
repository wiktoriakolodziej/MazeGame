using System;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;

namespace MazeGame.Scenes;

public class LevelSelectionScene(string size) : Scene
{
    private const string TITLE = "Choose level";
    private SpriteFont _font;
    private Vector2 _titleTextPos;
    private Vector2 _titleTextOrigin;
    private Panel _levelSelectionScreenButtonsPanel;
    private int _buttonWidthPercentage = 90;
    private int _buttonHeightPercentage = 10;
    private string _size = size;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();


        // Set the position and origin for the Dungeon text.
        Vector2 size = _font.MeasureString(TITLE);
        _titleTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
            GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
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
        GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        if (_levelSelectionScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Dungeon text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            SpriteBatch.DrawString(_font, TITLE, _titleTextPos + new Vector2(10, 10), dropShadowColor, 0.0f,
                _titleTextOrigin, 5.0f, SpriteEffects.None, 1.0f);

            // Draw the Dungeon text on top of that at its original position
            SpriteBatch.DrawString(_font, TITLE, _titleTextPos, Color.White, 0.0f, _titleTextOrigin, 5.0f,
                SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            SpriteBatch.End();
        }

        GumService.Default.Draw();
    }

    private void CreateLevelSelectionPanel()
    {
        // Create a container to hold all of our buttons
        _levelSelectionScreenButtonsPanel = new Panel();
        _levelSelectionScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _levelSelectionScreenButtonsPanel.AddToRoot();

        var folders = Android.App.Application.Context.Assets.List($"MazeSources/{_size}");
        var buttonHeight = _buttonHeightPercentage;
        var buttonSpacing = (100 - 35 - folders.Length * buttonHeight) / folders.Length;
        for (var x = 0; x < folders.Length; x++)
        {
            var button = new Button
            {
                Text = folders[x].Replace(".txt", ""),
                Width = _buttonWidthPercentage,
                Height = buttonHeight,
                X = 50 - (_buttonWidthPercentage / 2),
                Y = 35 + x * buttonHeight + x * buttonSpacing
            };
            button.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.XUnits = GeneralUnitType.Percentage;
            button.Visual.YUnits = GeneralUnitType.Percentage;

            button.Click += HandleLevelClicked;
            _levelSelectionScreenButtonsPanel.AddChild(button);
        }

    }

    private void HandleLevelClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var nameText = button.Text.Split(' ')[0];
        var levelName = $"MazeSources/{_size}/{nameText}.txt";
        RaiseSceneChanged(ScreenType.Gameplay, new() { { "levelName", levelName } });
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateLevelSelectionPanel();
    }
}

