using Microsoft.Win32;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PDFMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FolderPath { get; set; }

        public MainWindow()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            InitializeComponent();
            FolderPath = "";
        }

        private string[] ExtractTextAsArray()
        {
            //get the text from the textbox
            TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string text = textRange.Text;
            string[] lines = text.Split('\n');
            //Delete the last element of the array
            Array.Resize(ref lines, lines.Length - 1);
            return lines;
        }

        private string ExtractTextAsText()
        {
            //get the text from the textbox
            TextRange textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string text = textRange.Text;
            return text;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Close App
            Application.Current.Shutdown();
        }

        // Preview PDF File
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            var lines = ExtractTextAsArray();
            //Preview PDF File
            Document document = GeneratePdf(lines);
            document.GeneratePdfAndShow();
        }

        // Open File
        private void Oeffnen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string text = File.ReadAllText(openFileDialog.FileName);
                textBox.Document.Blocks.Clear();
                textBox.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
        }

        private void OeffnenFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Select Folder";
            if (openFolderDialog.ShowDialog() == true)
            {
                FolderPath = openFolderDialog.FolderName;
                FolderPath = FolderPath + @"\";
                FolderPathText.Text = FolderPath;
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This is a simple text editor with the ability to save the text as a PDF file. \n\nAuthor: Bastian Seidl",
                "About the Creator", MessageBoxButton.OK);
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Info info = new Info();
            info.Show();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Settings are not available yet.");
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        public List<LineSegment> ParseText(string line)
        {
            List<LineSegment> segments = new();
            int position = 0;
            while (position < line.Length)
            {
                if (line[position] == '*')
                {
                    // Bold
                    StringBuilder text = new StringBuilder();
                    position++;
                    while (position < line.Length && line[position] != '*')
                    {
                        text.Append(line[position]);
                        position++;
                    }
                    position++;
                    segments.Add(new LineSegment { Text = text.ToString(), TextStyle = TextStyle.Bold });
                }
                else if (line[position] == '+')
                {
                    // Italic
                    StringBuilder text = new StringBuilder();
                    position++;
                    while (position < line.Length && line[position] != '+')
                    {
                        text.Append(line[position]);
                        position++;
                    }
                    position++;
                    segments.Add(new LineSegment { Text = text.ToString(), TextStyle = TextStyle.Italic });
                }
                else if (line[position] == '_')
                {
                    // Underline
                    StringBuilder text = new StringBuilder();
                    position++;
                    while (position < line.Length && line[position] != '_')
                    {
                        text.Append(line[position]);
                        position++;
                    }
                    position++;
                    segments.Add(new LineSegment { Text = text.ToString(), TextStyle = TextStyle.Underline });
                }
                else
                {
                    // Normal
                    StringBuilder text = new StringBuilder();
                    char[] chars = { '*', '+', '_' };
                    while (position < line.Length && !chars.Contains(line[position]))
                    {
                        text.Append(line[position]);
                        position++;
                    }
                    segments.Add(new LineSegment { Text = text.ToString(), TextStyle = TextStyle.Normal });
                }
            }
            return segments;
        }

        // Save File
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            var lines = ExtractTextAsArray();
            var text = ExtractTextAsText();

            //Save File
            FileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "PDF files (*.pdf)|*.pdf|Text Files | *.txt";
            fileDialog.FileName = "document";
            if (fileDialog.ShowDialog() == true)
            {
                if (fileDialog.FileName.EndsWith(".txt"))
                {
                    //Text Save
                    File.WriteAllText(fileDialog.FileName, text);
                    MessageBox.Show("Text saved successfully.", "Success", MessageBoxButton.OK);
                    return;
                }
                if (fileDialog.FileName.EndsWith(".pdf"))
                {
                    //PDF Save
                    Document document = GeneratePdf(lines);
                    document.GeneratePdf(fileDialog.FileName);
                    MessageBox.Show("PDF saved successfully.", "Success", MessageBoxButton.OK);
                    return;
                }
                MessageBox.Show("File konnte nicht gespeichert werden.", "Error", MessageBoxButton.OK);
            }
        }

        public Document GeneratePdf(string[] lines)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));
                    page.Content().Column(x =>
                    {
                        foreach (var line in lines)
                        {
                            if (line.StartsWith("[image]"))
                            {
                                var file = line.Substring(8).Replace(")", "").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                                var path = System.IO.Path.Combine(FolderPath, file);
                                try
                                {
                                    x.Item().Image(path).FitArea();
                                }
                                catch (DocumentComposeException)
                                {
                                    MessageBox.Show("File not found: " + path, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else if (line.StartsWith("###"))
                                x.Item().Text(line.Substring(3)).AlignCenter().FontSize(16);
                            else if (line.StartsWith("##"))
                                x.Item().Text(line.Substring(2)).AlignCenter().FontSize(18);
                            else if (line.StartsWith("#"))
                            {
                                x.Item().Text(line.Substring(1)).AlignCenter().FontSize(20);
                            }
                            else
                            {
                                var segments = ParseText(line);
                                x.Item().Text(text =>
                                {
                                    foreach (var segment in segments)
                                    {
                                        switch (segment.TextStyle)
                                        {
                                            case TextStyle.Bold:
                                                text.Span(segment.Text).Bold();
                                                break;
                                            case TextStyle.Italic:
                                                text.Span(segment.Text).Italic();
                                                break;
                                            case TextStyle.Underline:
                                                text.Span(segment.Text).Underline();
                                                break;
                                            case TextStyle.Normal:
                                                text.Span(segment.Text);
                                                break;
                                        }
                                    }
                                });
                            }
                        }
                    });
                });
            });
            return document;
        }

        private void TextUpdate(object sender, RoutedEventArgs e)
        {

        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }
    }

    public class LineSegment
    {
        public string Text { get; set; }
        public TextStyle TextStyle { get; set; }
    }

    public enum TextStyle
    {
        Bold,
        Italic,
        Normal,
        Underline
    }
}