using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace kpop_shows.Controls
{
    /// <summary>
    /// Interaction logic for TextLinks.xaml
    /// </summary>
    public partial class TextLinks : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextLinks), new PropertyMetadata(TextPropertyChanged));

        public ObservableCollection<UIElement> InnerControls { get; set; } = new ObservableCollection<UIElement>();

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = (TextLinks)d;
            _this.InnerControls.Clear();

            var html = new HtmlDocument();
            html.LoadHtml((string)e.NewValue);

            foreach (var node in html.DocumentNode.ChildNodes)
                if (node is HtmlNode htmlnode && htmlnode.Name == "a")
                {
                    var link = new Hyperlink();
                    link.Inlines.Add(htmlnode.InnerText);

                    var tb = new TextBlock();
                    tb.Inlines.Add(link);

                    _this.InnerControls.Add(tb);
                }
                else if (node is HtmlTextNode textnode)
                {
                    var tb = new TextBlock();
                    tb.Inlines.Add(textnode.InnerText);

                    _this.InnerControls.Add(tb);
                }
                else
                {

                }
        }

        public TextLinks()
        {
            InitializeComponent();
        }
    }
}
