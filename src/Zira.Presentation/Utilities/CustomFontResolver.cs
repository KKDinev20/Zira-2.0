using System;
using System.IO;
using PdfSharp.Fonts;

namespace Zira.Presentation.Utilities
{
    public class CustomFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            string fontPath = faceName switch
            {
                "Roboto" => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/Roboto-Regular.ttf"),
                "Roboto-Bold" => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/Roboto-Bold.ttf"),
                "Roboto-Black" => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/Roboto-Black.ttf"),
                _ => throw new InvalidOperationException($"Font '{faceName}' not found!"),
            };

            return File.ReadAllBytes(fontPath);
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Roboto", StringComparison.OrdinalIgnoreCase))
            {
                if (isBold)
                {
                    return new FontResolverInfo("Roboto-Bold");
                }

                return new FontResolverInfo("Roboto");
            }

            if (familyName.Equals("Roboto-Black", StringComparison.OrdinalIgnoreCase))
            {
                return new FontResolverInfo("Roboto-Black");
            }

            return null;
        }
    }
}