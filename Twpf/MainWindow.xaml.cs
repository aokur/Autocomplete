namespace Twpf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Threading;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Documents;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            textBox2.Text = @"Услуга позволяет ограничивать получение нежелательных 
sms-собщений с коротких номеров контент-провайдеров и отправку sms-собщений 
на короткие номера контент-провайдеров. При этом у вас полностью сохранится 
возможность воспользоваться сервисами мобильной коммерции, сервисами социальных 
сетей, сервисами «Билайн» на коротких номерах , а также получать сообщения об 
акциях и услугах «Билайн».

Услуга очень удобна для ограничения доступа детей к платному и неподходящему 
по возрасту  контенту, а также для предотвращения случаев непреднамеренной 
отправки sms-сообщений на короткие номера контент-провайдеров.

The service allows you to restrict receiving unwanted
sms-distance traffic with short numbers of content providers and send sms-memos
short numbers of content providers. In this case, you will remain fully
opportunity to use mobile commerce services, social services
network services Beeline to short number, and receive messages
promotions and services Beeline.

The service is very easy to limit children's access to paid and improper
age content, and for the prevention of unintentional
send sms-message to the number of content providers.";
        }

        private void TextBox1TextChanged(object sender, TextChangedEventArgs e)
        {
            pComplete.IsOpen = false;
            if (textBox1.Text.Length > 0)
            {
                var find = textBox1.Text.Split().Last((s) => s != string.Empty).ToLower();
                var source = textBox2.Text;

                var work = new BackgroundWorker();
                
                work.DoWork += (o, args) =>
                    {
                        var task = Task.Factory.StartNew<IList<string>>(() => this.FindText(find, source));
                        var l = task.Result;
                        args.Result = l;
                    };
                work.RunWorkerCompleted += (o, args) =>
                    {
                        listBox1.ItemsSource = args.Result as IList<string>;
                        if (listBox1.Items.Count > 0)
                            pComplete.IsOpen = true;
                    };
                work.RunWorkerAsync();
            }
        }

        /// <summary>
        /// The find text.
        /// </summary>
        /// <param name="find">
        /// The find.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        private IList<string> FindText(string find, string source)
        {
            //Thread.Sleep(3000); //working
            var words = source.Split(source.Where(c => !char.IsLetterOrDigit(c)).ToArray());
            return words.Where(word => this.CheckWord(find, word.ToLower())).Distinct().ToList();
        }

        private bool CheckWord(string find, string word)
        {
            return find.Length <= word.Length && word.Contains(find);
        }

        private void TextBox1KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
                    {
                        textBox2.Text += textBox1.Text;
                        textBox1.Text = "";
                    }

            if (listBox1.Items == null)
                return;

            if (listBox1.Items.Count == 0)
                return;

            if (listBox1.SelectedItem == null)
                listBox1.SelectedIndex = 0;
            switch (e.Key)
            {
                case Key.Enter:
                    {

                        var find = textBox1.Text.Split().Last((s) => s != string.Empty).ToLower();
                        var i = this.textBox1.Text.ToLower().LastIndexOf(find, System.StringComparison.Ordinal);
                        var t = textBox1.Text.Substring(0, i);
                        if (listBox1.SelectedItem != null)
                        {
                            textBox1.Text = t + listBox1.SelectedItem.ToString();
                        }
                        textBox1.SelectionStart = textBox1.Text.Length;
                        break;
                    }
                case Key.Down:
                    {
                        listBox1.SelectedIndex++;
                        break;
                    }
                case Key.Up:
                    {
                        listBox1.SelectedIndex--;
                        break;
                    }
            }
            if (listBox1.SelectedItem == null)
                listBox1.SelectedIndex = 0;
        }
    }
}
