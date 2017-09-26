using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private XmlWriter myXmlWriter;

        public MainWindow()
        {
            InitializeComponent();

            btnChoose.ToolTip = "Choose location for xml file";
            btnChoseFile.ToolTip = "Chose folder";
            btnStart.ToolTip = "Start the app";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFilePath.Text = ofd.SelectedPath;

                btnChoose.IsEnabled = true;
            }
        }

        public void createXml()
        {
            if (Directory.Exists(tbFilePath.Text))
            {
                DirectoryInfo diTop = new DirectoryInfo(tbFilePath.Text);

                myXmlWriter = XmlWriter.Create(tbXMLPath.Text + "FilesAndFolders.xml");
                myXmlWriter.WriteStartElement("FilesAndFolders");

                myXmlWriter.WriteString("\n");

                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = tbFilePath.Text;
                treeView1.Items.Add(tvi);

                getFilesAndFolders(tbFilePath.Text, tvi);
                myXmlWriter.Close();
            }
        }

        private void getFilesAndFolders(string folderPath, TreeViewItem item)
        {
            DirectoryInfo diTop = new DirectoryInfo(folderPath);

            TreeViewItem tvi;
            //get all the files of the selected folder
            foreach (var fi in diTop.EnumerateFiles())
            {
                myXmlWriter.WriteStartElement("File");
                myXmlWriter.WriteAttributeString("Length", fi.Length.ToString());
                myXmlWriter.WriteAttributeString("CreationTime", fi.CreationTime.ToString());
                myXmlWriter.WriteAttributeString("LastAccessTime", fi.LastAccessTime.ToString());
                myXmlWriter.WriteAttributeString("LastModified", fi.LastWriteTime.ToString());
                myXmlWriter.WriteString(fi.Name);

                myXmlWriter.WriteEndElement();
                myXmlWriter.WriteString("\n");

                tvi = new TreeViewItem();
                tvi.Header = fi.Name;
                item.Items.Add(tvi);
            }

            TreeViewItem tviFolder;
            //get all the folders of the selected folder
            foreach (var di in diTop.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
            {
                myXmlWriter.WriteStartElement("Folder");
                myXmlWriter.WriteAttributeString("NumberOfFiles", di.GetFiles("*").Length.ToString());
                myXmlWriter.WriteString(di.Name);

                tviFolder = new TreeViewItem();
                tviFolder.Header = di.Name;
                item.Items.Add(tviFolder);

                myXmlWriter.WriteString("\n");

                getFilesAndFolders(di.FullName, tviFolder);

                myXmlWriter.WriteEndElement();
                myXmlWriter.WriteString("\n");
            }
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog newOfd = new FolderBrowserDialog();
            if (newOfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbXMLPath.Text = newOfd.SelectedPath;
                btnStart.IsEnabled = true;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            createXml();
            System.Windows.Forms.MessageBox.Show("XML file was created.");
        }
    }
}
