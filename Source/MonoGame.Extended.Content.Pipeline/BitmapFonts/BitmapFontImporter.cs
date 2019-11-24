using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor",
         DisplayName = "BMFont Importer - MonoGame.Extended")]
    public class BitmapFontImporter : ContentImporter<BitmapFontFile>
    {
        public override BitmapFontFile Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing XML file: {0}", filename);

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var deserializer = new XmlSerializer(typeof(BitmapFontFile));
                    return (BitmapFontFile)deserializer.Deserialize(streamReader);
                }

            }
        }
    }
}