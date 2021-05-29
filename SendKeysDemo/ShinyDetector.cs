using System.Drawing;

namespace SendKeysDemo
{
    public static class ShinyDetector
    {
        private static readonly Point centerOfStar = new Point(212, 130);
        private static readonly Color shinyStarColor = Color.FromArgb(255, 214, 82);

        public static bool IsShiny(Bitmap statusPageScreenshot)
        {
            return statusPageScreenshot.GetPixel(centerOfStar.X, centerOfStar.Y) == shinyStarColor;
        }
    }
}
