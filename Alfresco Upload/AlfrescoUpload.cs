using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Data;
using DotCMIS.Data.Impl;
using System.Web;
using DotCMIS.Enums;

namespace Alfresco_Upload
{
    public class AlfrescoUpload
    {
        private string user;
        private string password;
        private string directory;
        private string alfrescoServerURL;
        ISession session = null;
        public AlfrescoUpload(string user, string password, string directory, string url)
        {
            this.user = user;
            this.password = password;
            this.directory = directory;
            this.alfrescoServerURL = url;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters[DotCMIS.SessionParameter.BindingType] = BindingType.AtomPub;
            parameters[DotCMIS.SessionParameter.AtomPubUrl] = alfrescoServerURL + CMIS_V1_URL;
            parameters[DotCMIS.SessionParameter.User] = user;
            parameters[DotCMIS.SessionParameter.Password] = password;
            SessionFactory factory = SessionFactory.NewInstance();
            session = factory.GetRepositories(parameters)[0].CreateSession();
        }

        private string ALFRESCO_ROOT = "Alfresco Root";
        private string CMIS_V1_URL = "alfresco/api/-default-/public/cmis/versions/1.0/atom";

        public void clickHandler()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {

                DialogResult result = dialog.ShowDialog();
                if (result.Equals(System.Windows.Forms.DialogResult.OK))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(dialog.SelectedPath);
                        string path = createFolder(di.Name, directory);
                        uploadFilesInDirectory(di, path);
                        createDirectoriesAndFiles(di, path);
                        MessageBox.Show("Directories created successfully!");
                    }
                    catch
                    {

                    }

                }
            }
        }

        private void uploadFilesInDirectory(DirectoryInfo di, string uploadPath)
        {
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                uploadFile(file, uploadPath);
            }

        }

        private void createDirectoriesAndFiles(DirectoryInfo di, string path)
        {
            DirectoryInfo[] directories = di.GetDirectories();
            if (directories.Length > 0)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    string path2 = createFolder(directories[i].Name, path);
                    uploadFilesInDirectory(directories[i], path2);
                    createDirectoriesAndFiles(directories[i], path2);
                }
            }
        }

        private void uploadFile(FileInfo file, string uploadPath)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add(PropertyIds.Name, file.Name);
            properties.Add(PropertyIds.ObjectTypeId, "cmis:document");
            byte[] content = File.ReadAllBytes(file.FullName);
            string mimeType = MimeMapping.GetMimeMapping(file.Name);
            IContentStream contentStream = session.ObjectFactory.CreateContentStream(file.Name, content.LongLength, mimeType, new MemoryStream(content));
            IFolder root = session.GetObjectByPath(uploadPath) as IFolder;
            root.CreateDocument(properties, contentStream, VersioningState.Major);

        }

        private string createFolder(string name, string path)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add(PropertyIds.Name, name);
            properties.Add(PropertyIds.ObjectTypeId, "cmis:folder");
            IFolder root = session.GetObjectByPath(path) as IFolder;
            root.CreateFolder(properties);
            return path + "/" + name;
        }
    }
}

