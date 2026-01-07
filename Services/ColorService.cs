using System;
using System.Collections.Generic;
using System.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace MazeGame.Services;

public static class ColorService
{
    public static Color WallColor = Color.MonoGameOrange;
    public static Color PathColor = Color.White;
    public static Color StartColor = Color.White;
    public static Color EndColor = Color.Gold;
    public static Color BallColor = Color.Cyan;
    public static Color MenuBgColor = Color.Black;
    public static Color MenuTextColor = Color.White;
    public static float FontSizeMultiplier = 1.0f;

    public static void SetContrastingPallette()
    {
        // https://coolors.co/palette/011627-fdfffc-2ec4b6-e71d36-ff9f1c
        WallColor = new Color(255, 159, 28);
        PathColor = Color.White;
        StartColor = Color.White;
        EndColor = new Color(0, 133, 122);
        BallColor = new Color(1, 22, 39);
        MenuBgColor = new Color(1, 22, 39);
        MenuTextColor = Color.White;
    }

    public static void SetBluePurplePallette()
    {
        // https://coolors.co/palette/7400b8-6930c3-5e60ce-5390d9-4ea8de-48bfe3-56cfe1-64dfdf-72efdd-80ffdb
        WallColor = new Color(83, 144, 217);
        PathColor = new Color(150, 190, 214);
        StartColor = new Color(150, 190, 214);
        EndColor = new Color(128, 255, 219);
        BallColor = new Color(116, 0, 184);
        MenuBgColor = new Color(52, 0, 82);
        MenuTextColor = new Color(129, 219, 233);
    }

    public static void SetOrangePallette()
    {
        // https://coolors.co/palette/8ea604-f5bb00-ec9f05-d76a03-bf3100
        WallColor = new Color(215, 106, 3);
        PathColor = new Color(245, 187, 0);
        StartColor = new Color(245, 187, 0);
        EndColor = new Color(142, 166, 4);
        BallColor = new Color(191, 49, 0);
        MenuBgColor = new Color(87, 22, 0);
        MenuTextColor = new Color(245, 187, 0);
    }
}
