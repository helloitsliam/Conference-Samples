using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.IO.Packaging;

namespace COGNITIVE
{
    static class OpenXml
    {
        public static string GetDocumentBodyAsPlainText(string document)
        {
            string content = "";

            using (WordprocessingDocument file = WordprocessingDocument.Open(document, true))
            {
                    DocumentFormat.OpenXml.Wordprocessing.Body body = file.MainDocumentPart.Document.Body;
                    content = body.InnerText;
            }

            return content;
        }
    }
}