using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Find.xaml
    /// </summary>
    public partial class Find : Window
    {
        public Find()
        {
            InitializeComponent();
        }

        private void btnClose_click(object sender, RoutedEventArgs e)
        {

          
          //  TextRange text = new TextRange(MainWindow.sRtb.Document.ContentStart, MainWindow.sRtb.Document.ContentEnd);
          //  text.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

            this.Close();
        }

        private void btnOK_click(object sender, RoutedEventArgs e)
        {

            if (tbFind.Text != "")
            {

                TextRange textRange = new TextRange(MainWindow.sRtb.Document.ContentStart, MainWindow.sRtb.Document.ContentEnd);
                string tempString;

                using (var memoryStream = new MemoryStream())
                {
                    textRange.Save(memoryStream, System.Windows.Forms.DataFormats.Rtf);
                    tempString = ASCIIEncoding.Default.GetString(memoryStream.ToArray());
                }

                

                tempString = tempString.Replace(tbFind.Text, tbReplace.Text);

                MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(tempString));

                MainWindow.sRtb.SelectAll();

                

                MainWindow.sRtb.Selection.Load(stream, System.Windows.Forms.DataFormats.Rtf);
            }
            else
            {
                MessageBox.Show("Data not correct!", "Error", MessageBoxButton.OK);
            }

        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {

            string keyword = tbFind.Text;
            string newString = tbReplace.Text;
            TextRange text = new TextRange(MainWindow.sRtb.Document.ContentStart, MainWindow.sRtb.Document.ContentEnd);
            TextPointer current = text.Start.GetInsertionPosition(LogicalDirection.Forward);
            while (current != null)
            {
                string textInRun = current.GetTextInRun(LogicalDirection.Forward);

                if (!string.IsNullOrWhiteSpace(textInRun) && textInRun != newString)
                {
                    int index = textInRun.IndexOf(keyword);
                    if (index != -1)
                    {
                        TextPointer selectionStart = current.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);

                        TextRange selection1 = new TextRange(selectionStart, selectionEnd);
                        selection1.Text = keyword;
                        selection1.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Silver);
                        MainWindow.sRtb.Selection.Select(selection1.Start, selection1.End);
                        MainWindow.sRtb.Focus();
                    }
                   /* else if(index == -1)
                    {
                        MessageBox.Show("0 results found!","Results", MessageBoxButton.OK, MessageBoxImage.Information);
                    }*/
                }
               else if(keyword=="")
               {
                   lblError.Content = "Text to find cannot be emty!";
                   lblError.BorderBrush = Brushes.Red;
                   tbFind.BorderBrush = Brushes.Red;
               }
               current = current.GetNextContextPosition(LogicalDirection.Forward);
            }

        }
    }
}
