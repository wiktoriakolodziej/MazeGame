using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Collections.Generic;

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
    private int indexFrom = 0;
    private int indexTo = 4;

    private List<Button> buttons = [];
    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();


        // Set the position and origin for the Dungeon text.
        Vector2 size = _font.MeasureString(TITLE);
        _titleTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f,
            GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
        _titleTextOrigin = size * 0.5f;
        InitializeButtonList();

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

    private void InitializeButtonList()
    {
        var folders = Android.App.Application.Context.Assets.List($"MazeSources/{_size}");
        foreach(var item in folders)
        {
            var button = new Button()
            {
                Text = item.Replace(".txt", ""),
                Width = _buttonWidthPercentage,
                Height = _buttonHeightPercentage
            };
            button.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
            button.Visual.XUnits = GeneralUnitType.Percentage;
            button.Visual.YUnits = GeneralUnitType.Percentage;
            buttons.Add(button);
        }
    }

    private void CreateLevelSelectionPanel()
    {
        // Create a container to hold all of our buttons
        _levelSelectionScreenButtonsPanel = new Panel();
        _levelSelectionScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _levelSelectionScreenButtonsPanel.AddToRoot();
        var row = 0;

        for(var x = indexFrom; x < indexTo && x < buttons.Count; x++)
        {
            var button = buttons[x];
            button.Y = 35 + row * button.Height + row * 2;
            button.X = 50 - (button.Width / 2);
            button.Click += HandleLevelClicked;
            _levelSelectionScreenButtonsPanel.AddChild(button);
            row++;
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

        if (buttons.Count < 5)
            return;

        var buttonDown = new Button()
        {
            Text = "v",
            Width = 30,
            Height = 5
        };
        buttonDown.Anchor(Gum.Wireframe.Anchor.BottomRight);
        buttonDown.X = -10;
        buttonDown.Y = -10;
        buttonDown.IsEnabled = indexTo < buttons.Count;
        buttonDown.Click += HandleScrollClicked;
        _levelSelectionScreenButtonsPanel.AddChild(buttonDown);

        var buttonUp = new Button()
        {
            Text = "^",
            Width = 30,
            Height = 5
        };
        buttonUp.Anchor(Gum.Wireframe.Anchor.BottomRight);
        buttonUp.X = - 10;
        buttonUp.Y = -50;
        buttonUp.IsEnabled = indexFrom > 0;
        buttonUp.Click += HandleScrollClicked;
        _levelSelectionScreenButtonsPanel.AddChild(buttonUp);

       
    }

    private void HandleBackClicked(object sender, EventArgs e)
    {
        RaiseSceneChanged(ScreenType.LevelSizeSelection);
    }

    private void HandleScrollClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if(button.Text == "v")
        {
            if(indexTo < buttons.Count)
            {
                indexFrom++;
                indexTo++;
                InitializeUI();
            }
        }
        else if(button.Text == "^")
        {
            if(indexFrom > 0)
            {
                indexFrom--;
                indexTo--;
                InitializeUI();
            }
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

