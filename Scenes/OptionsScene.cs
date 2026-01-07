using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using Gum.Converters;
using Gum.DataTypes;
using MazeGame.Services;

namespace MazeGame.Scenes;

public class OptionsScene : Scene
{
    private const string TITLE = "Options";
    private const string CHOOSE_COLORS = "Choose a color pallette";
    private const string CHOOSE_SIZE = "Choose a font size";
    private SpriteFont _font;
    private Vector2 _titleTextPos;
    private Vector2 _titleTextOrigin;
    private Vector2 _colorsTextPos;
    private Vector2 _colorsTextOrigin;
    private Vector2 _sizeTextPos;
    private Vector2 _sizeTextOrigin;
    private Color backgroundColor = ColorService.MenuBgColor;
    private Color textColor = ColorService.MenuTextColor;

    private Panel _optionsScreenButtonsPanel;

    private int _buttonWidthPercentage = 40;
    private int _buttonHeightPercentage = 10;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();


        // Set the position and origin for the Dungeon text.
        Vector2 size = _font.MeasureString(TITLE);
        _titleTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.1f);
        _titleTextOrigin = size * 0.5f;
        size = _font.MeasureString(CHOOSE_COLORS);
        _colorsTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.2f);
        _colorsTextOrigin = size * 0.5f;
        size = _font.MeasureString(CHOOSE_SIZE);
        _sizeTextPos = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth * 0.5f, GraphicsDevice.PresentationParameters.BackBufferHeight * 0.55f);
        _sizeTextOrigin = size * 0.5f;

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Content.Load<SpriteFont>("fonts/Roboto");
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorService.MenuBgColor);

        if (_optionsScreenButtonsPanel.IsVisible)
        {
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            SpriteBatch.DrawString(_font, TITLE, _titleTextPos, ColorService.MenuTextColor, 0.0f, _titleTextOrigin, 2.0f * ColorService.FontSizeMultiplier, SpriteEffects.None, 1.0f);
            SpriteBatch.DrawString(_font, CHOOSE_COLORS, _colorsTextPos, ColorService.MenuTextColor, 0.0f, _colorsTextOrigin, 1.0f * ColorService.FontSizeMultiplier, SpriteEffects.None, 1.0f);
            SpriteBatch.DrawString(_font, CHOOSE_SIZE, _sizeTextPos, ColorService.MenuTextColor, 0.0f, _sizeTextOrigin, 1.0f * ColorService.FontSizeMultiplier, SpriteEffects.None, 1.0f);

            SpriteBatch.End();
        }

        GumService.Default.Draw();
    }

    private void CreateOptionsPanel()
    {
        // Create a container to hold all of our buttons
        _optionsScreenButtonsPanel = new Panel();
        _optionsScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsScreenButtonsPanel.AddToRoot();

        var setContrastingColorsButton = new Button();
        setContrastingColorsButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        setContrastingColorsButton.Visual.XUnits = GeneralUnitType.Percentage;
        setContrastingColorsButton.Visual.YUnits = GeneralUnitType.Percentage;
        setContrastingColorsButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        setContrastingColorsButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        setContrastingColorsButton.Visual.X = 25 - _buttonWidthPercentage / 2f;
        setContrastingColorsButton.Visual.Y = 25;
        setContrastingColorsButton.Visual.Width = _buttonWidthPercentage;
        setContrastingColorsButton.Visual.Height = _buttonHeightPercentage;
        setContrastingColorsButton.Text = "High Contrast";
        setContrastingColorsButton.Click += HandleHighContrastClicked;
        _optionsScreenButtonsPanel.AddChild(setContrastingColorsButton);

        var setBluePurpleColorsButton = new Button();
        setBluePurpleColorsButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        setBluePurpleColorsButton.Visual.XUnits = GeneralUnitType.Percentage;
        setBluePurpleColorsButton.Visual.YUnits = GeneralUnitType.Percentage;
        setBluePurpleColorsButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        setBluePurpleColorsButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        setBluePurpleColorsButton.Visual.X = 75 - _buttonWidthPercentage / 2f;
        setBluePurpleColorsButton.Visual.Y = 25;
        setBluePurpleColorsButton.Visual.Width = _buttonWidthPercentage;
        setBluePurpleColorsButton.Visual.Height = _buttonHeightPercentage;
        setBluePurpleColorsButton.Text = "Blue-Purple";
        setBluePurpleColorsButton.Click += HandleBluePurpleClicked;
        _optionsScreenButtonsPanel.AddChild(setBluePurpleColorsButton);

        var setOrangeColorsButton = new Button();
        setOrangeColorsButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        setOrangeColorsButton.Visual.XUnits = GeneralUnitType.Percentage;
        setOrangeColorsButton.Visual.YUnits = GeneralUnitType.Percentage;
        setOrangeColorsButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        setOrangeColorsButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        setOrangeColorsButton.Visual.X = 25 - _buttonWidthPercentage / 2f;
        setOrangeColorsButton.Visual.Y = 40;
        setOrangeColorsButton.Visual.Width = _buttonWidthPercentage;
        setOrangeColorsButton.Visual.Height = _buttonHeightPercentage;
        setOrangeColorsButton.Text = "Orange";
        setOrangeColorsButton.Click += HandleOrangeClicked;
        _optionsScreenButtonsPanel.AddChild(setOrangeColorsButton);

        var setNormalFontSizeButton = new Button();
        setNormalFontSizeButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        setNormalFontSizeButton.Visual.XUnits = GeneralUnitType.Percentage;
        setNormalFontSizeButton.Visual.YUnits = GeneralUnitType.Percentage;
        setNormalFontSizeButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        setNormalFontSizeButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        setNormalFontSizeButton.Visual.X = 25 - _buttonWidthPercentage / 2f;
        setNormalFontSizeButton.Visual.Y = 60;
        setNormalFontSizeButton.Visual.Width = _buttonWidthPercentage;
        setNormalFontSizeButton.Visual.Height = _buttonHeightPercentage;
        setNormalFontSizeButton.Text = "Normal";
        setNormalFontSizeButton.Click += HandleNormalFontClicked;
        _optionsScreenButtonsPanel.AddChild(setNormalFontSizeButton);

        var setLargeFontSizeButton = new Button();
        setLargeFontSizeButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        setLargeFontSizeButton.Visual.XUnits = GeneralUnitType.Percentage;
        setLargeFontSizeButton.Visual.YUnits = GeneralUnitType.Percentage;
        setLargeFontSizeButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        setLargeFontSizeButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        setLargeFontSizeButton.Visual.X = 75 - _buttonWidthPercentage / 2f;
        setLargeFontSizeButton.Visual.Y = 60;
        setLargeFontSizeButton.Visual.Width = _buttonWidthPercentage;
        setLargeFontSizeButton.Visual.Height = _buttonHeightPercentage;
        setLargeFontSizeButton.Text = "Large";
        setLargeFontSizeButton.Click += HandleLargeFontClicked;
        _optionsScreenButtonsPanel.AddChild(setLargeFontSizeButton);

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
        _optionsScreenButtonsPanel.AddChild(buttonBack);

    }

    private void HandleHighContrastClicked(object sender, EventArgs e)
    {
        ColorService.SetContrastingPallette();
    }

    private void HandleBluePurpleClicked(object sender, EventArgs e)
    {
        ColorService.SetBluePurplePallette();
    }

    private void HandleOrangeClicked(object sender, EventArgs e)
    {
        ColorService.SetOrangePallette();
    }

    private void HandleNormalFontClicked(object sender, EventArgs e)
    {
        ColorService.FontSizeMultiplier = 1.0f;
    }

    private void HandleLargeFontClicked(object sender, EventArgs e)
    {
        ColorService.FontSizeMultiplier = 1.5f;
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

        CreateOptionsPanel();
    }
}
