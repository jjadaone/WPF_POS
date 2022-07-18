using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_POS
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        public ReceiptWindow(String str)
        {
            InitializeComponent();

            //Paragraph myParagraph = new Paragraph();


            // Add some Bold text to the paragraph
            //myParagraph.Inlines.Add(new Bold(new Run("Some bold text in the paragraph.")));

            // Add some plain text to the paragraph
            //myParagraph.Inlines.Add(new Run(str));

            //// Create a List and populate with three list items.
            //List myList = new List();

            //// First create paragraphs to go into the list item.
            //Paragraph paragraphListItem1 = new Paragraph(new Run(str));
            //Paragraph paragraphListItem2 = new Paragraph(new Run("ListItem 2"));
            //Paragraph paragraphListItem3 = new Paragraph(new Run("ListItem 3"));

            //// Add ListItems with paragraphs in them.
            //myList.ListItems.Add(new ListItem(paragraphListItem1));
            //myList.ListItems.Add(new ListItem(paragraphListItem2));
            //myList.ListItems.Add(new ListItem(paragraphListItem3));

            // Create a FlowDocument with the paragraph and list.
            //FlowDocument myFlowDocument = new FlowDocument();
            //myFlowDocument.Blocks.Add(myParagraph);
            //myFlowDocument.Blocks.Add(myList);

            // Add the FlowDocument to a FlowDocumentReader Control
            //FlowDocumentReader myFlowDocumentReader = new FlowDocumentReader();
            //FlowDocumentScrollViewer myFlowDocumentReader = new FlowDocumentScrollViewer();

            //myFlowDocumentReader.Document = myFlowDocument;

            //this.Content = myFlowDocumentReader;

            //Image img = new Image();
            //Uri imgUri = new Uri("https://www.nicepng.com/png/detail/671-6718562_compare-pos-icon.png", UriKind.Relative);
            //BitmapImage bitmap = new BitmapImage(imgUri);
            //img.Source = bitmap;

            //ImageSource imguri = new ImageSourceConverter().ConvertFromString("https://www.nicepng.com/png/detail/671-6718562_compare-pos-icon.png") as ImageSource;
            //img.Source = imguri;

            Run run = new Run();
            run.Text = "POS SYSTEM";
            Paragraph header = new Paragraph();
            header.FontSize = 50;
            header.TextAlignment = TextAlignment.Center;
            header.Inlines.Add(run);

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(str));
            paragraph.FontFamily = new FontFamily("Calibri");
            paragraph.TextAlignment = TextAlignment.Center;
            //paragraph.Inlines.Add(imguri);

            FlowDocument doc = new FlowDocument();
            doc.Blocks.Add(header);
            doc.Blocks.Add(paragraph);

            FlowDocumentScrollViewer dreader = new FlowDocumentScrollViewer();
            dreader.Document = doc;
            this.Content = dreader;
            

        }

    }
}
