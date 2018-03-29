using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;
using System.IO;

namespace COGNITIVE
{
    static class Program
    {
        static void Main(string[] args)
        {
            List<Documents> documents = new List<Documents>();
            string sentiment = "";
            string keyphrases = "";
            string language = "";
            string translation = "";
            string translatefromlanguage = "en";
            string translationlanguage = "en";

            // Download the documents from SharePoint
            //DownloadDocuments();

            // Retrieve body of the documents and store in List
            string Source = Helper.filePath;
            foreach (string d in Directory.GetFiles(Source))
            {
                var fi = new FileInfo(d);
                var body = OpenXml.GetDocumentBodyAsPlainText(d);

                documents.Add(new Documents { id = System.Guid.NewGuid().ToString(), text = body });
            }

            // Generate JSON Document to Post to Services
            var value = JSON.GenerateJSONDocument(documents).ToString();

            // Empty the documents List
            documents.Clear();

            // Retrieve Sentiment Values and Update Document Property
            Task.Run(async () =>
            {
                sentiment = await Helper.GetSentiment(value);
            }).Wait();

            Console.WriteLine("Sentiment Value:");
            Console.WriteLine(sentiment);

            // Retrieve Key Phrase Values and Update Document Property
            Task.Run(async () =>
            {
                keyphrases = await Helper.GetKeyPhrases(value);
            }).Wait();

            Console.WriteLine("Key Phrase Values:");
            Console.WriteLine(keyphrases);

            // Retrieve Key Phrase Values and Update Document Property
            Task.Run(async () =>
            {
                language = await Helper.GetLanguage(value);
            }).Wait();

            Console.WriteLine("Language Value:");
            Console.WriteLine(language);

            translation = Helper.DoTranslate(value, translatefromlanguage, translationlanguage);

            Console.WriteLine("Translation Value:");
            Console.WriteLine(translation);

            // Pause
            Console.ReadLine();

            // Create 

            //UploadDocuments();
        }

        static void DownloadDocuments()
        {
            string Destination = Helper.filePath;

            var site = new ClientContext(Helper.siteUrl);
            var web = site.Web;
            site.Load(web);
            site.ExecuteQuery();

            List list = web.Lists.GetByTitle(Helper.documentLibrary);
            site.Load(list);
            site.ExecuteQuery();
            site.Load(list.RootFolder);
            site.ExecuteQuery();
            site.Load(list.RootFolder.Folders);
            site.ExecuteQuery();

            Folder folder = web.GetFolderByServerRelativeUrl(list.RootFolder.ServerRelativeUrl);
            site.Load(folder);
            site.ExecuteQuery();
            site.Load(folder.Files);
            site.ExecuteQuery();

            foreach (Microsoft.SharePoint.Client.File file in folder.Files)
            {
                Stream fs = Microsoft.SharePoint.Client.File.OpenBinaryDirect(site, file.ServerRelativeUrl).Stream;
                byte[] binary = Helper.GetBinary(fs);

                FileStream stream = new FileStream(Destination + "/" + file.Name, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(binary);
                writer.Close();
            }
        }

        static void UploadDocuments()
        {
            string Source = Helper.filePath;

            var site = new ClientContext(Helper.siteUrl);
            var web = site.Web;
            site.Load(web);
            site.ExecuteQuery();

            foreach (string d in Directory.GetFiles(Source))
            {
                var fs = new FileStream(d, FileMode.Open);
                var fi = new FileInfo(d);
                var list = site.Web.Lists.GetByTitle(Helper.documentLibrary);
                site.Load(list.RootFolder);
                site.ExecuteQuery();

                var fileUrl = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fi.Name);
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(site, fileUrl, fs, true);
            }
        }
    }
}
