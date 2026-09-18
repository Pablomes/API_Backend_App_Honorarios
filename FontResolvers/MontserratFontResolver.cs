using PdfSharp.Fonts;
using System.Reflection;

namespace API_Backend_App_Industrializacion.FontResolvers
{
    public class MontserratFontResolver : IFontResolver
    {
        public static readonly MontserratFontResolver Instance = new();

        public byte[] GetFont(string faceName)
        {
            Assembly asm =  typeof(MontserratFontResolver).Assembly;

            Console.WriteLine(faceName);

            Stream? stream = faceName switch
            {
                "Montserrat#Regular" => asm.GetManifestResourceStream("API_Backend_App_Industrializacion.Materials.Fonts.Montserrat-Regular.ttf"),
                "Montserrat#Bold" => asm.GetManifestResourceStream("API_Backend_App_Industrializacion.Materials.Fonts.Montserrat-Bold.ttf"),
                "Montserrat#Italic" => asm.GetManifestResourceStream("API_Backend_App_Industrializacion.Materials.Fonts.Montserrat-Italic.ttf"),
                "Montserrat#BoldItalic" => asm.GetManifestResourceStream("API_Backend_App_Industrializacion.Materials.Fonts.Montserrat-BoldItalic.ttf"),
                _ => throw new ArgumentException($"Font not found: {faceName}")
            };

            using MemoryStream memStream = new MemoryStream();
            stream!.CopyTo(memStream);

            return memStream.ToArray();
        }

        public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
        {

            if (!familyName.Equals("Montserrat", StringComparison.OrdinalIgnoreCase))
                return null;

            string faceName = (bold, italic) switch
            {
                (true, false) => "Montserrat#Bold",
                (true, true) => "Montserrat#BoldItalic",
                (false, true) => "Montserrat#Italic",
                (false, false) => "Montserrat#Regular"
            };

            return new FontResolverInfo(faceName);
        }
    }
}
