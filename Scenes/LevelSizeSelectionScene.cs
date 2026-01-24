using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using MazeGame.Services;
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
        base.Initialize();

        Vector2 size = _font.MeasureString(TITLE);
        _titleTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
        _titleTextOrigin = size * 0.5f;

        InitializeUI();
    }

    public override void LoadContent()
    {
        _font = Content.Load<SpriteFont>("fonts/Roboto");
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorService.MenuBgColor);

        if (_levelSelectionScreenButtonsPanel.IsVisible)
        {
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            SpriteBatch.DrawString(_font, TITLE, _titleTextPos, ColorService.MenuTextColor, 0.0f, _titleTextOrigin, 2.0f, SpriteEffects.None, 1.0f);

            SpriteBatch.End();
        }

        GumService.Default.Draw();
    }

    private void CreateLevelSelectionPanel()
    {
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
        GumService.Default.Root.Children.Clear();

        CreateLevelSelectionPanel();
    }
}

