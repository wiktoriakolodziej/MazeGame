using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGame.Scenes;

public class LevelSizeSelectionScene : Scene
{
    private const string TITLE = "Choose size";
    private SpriteFont _font;
    private Vector2 _titleTextPos;
    private Vector2 _titleTextOrigin;

    private Panel _levelSelectionScreenButtonsPanel;

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
        GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        if (_levelSelectionScreenButtonsPanel.IsVisible)
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

    private void CreateLevelSelectionPanel()
    {
        // Create a container to hold all of our buttons
        _levelSelectionScreenButtonsPanel = new Panel();
        _levelSelectionScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _levelSelectionScreenButtonsPanel.AddToRoot();

        var folders = Android.App.Application.Context.Assets.List("MazeSources");
        var buttonHeight = _buttonHeightPercentage;
        var buttonSpacing = (100 - 35 - folders.Length * buttonHeight) / folders.Length;
        for (var x = 0; x < folders.Length; x++)
        {
            var button = new Button
            {
                Text = folders[x] + " cells",
                Width = _buttonWidthPercentage,
                Height = buttonHeight,
                X = 50 - (_buttonWidthPercentage / 2),
                Y = 35 + x * buttonHeight + x * buttonSpacing
            };
            button.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.XUnits = GeneralUnitType.Percentage;
            button.Visual.YUnits = GeneralUnitType.Percentage;

            button.Click += HandleSizeClicked;
            _levelSelectionScreenButtonsPanel.AddChild(button);
        }
        var buttonBack = new Button()
        {
            Text = "<",
            Width = 30,
            Height = 5
        };
        buttonBack.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        buttonBack.X = 10;
        buttonBack.Y = -10;
        buttonBack.Click += HandleBackClicked;
        _levelSelectionScreenButtonsPanel.AddChild(buttonBack);
    }

    private void HandleSizeClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var sizeText = button.Text.Split(' ')[0];
        RaiseSceneChanged(ScreenType.LevelSelection, new() {{"levelSize", sizeText}});
    }
    private void HandleBackClicked(object sender, EventArgs e)
    {
        RaiseSceneChanged(ScreenType.Title);
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateLevelSelectionPanel();
    }
}

