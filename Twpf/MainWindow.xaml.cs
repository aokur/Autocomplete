using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Twpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool lockPopup;
        private BackgroundWorker work;
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
            //Focus on the first element.
            Loaded += (sender, e) =>
    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void TextBox1TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lockPopup)
            {
                lockPopup = false;
                return;
            }
            pComplete.IsOpen = false;
            if (textBox1.Text.Length > 0)
            {
                var find = textBox1.Text.Split().Last((s) => s != string.Empty).ToLower();
                var source = textBox2.Text;
                runBackgroundWorker(find, source);
            }
        }

        private void runBackgroundWorker(string find, string source)
        {//data for Autocomplete will be loaded asynchronously, that's why we need a BackgroundWorker

            //if it's already looking for data, let's cancel it to start again with the new text
            if (work != null && work.IsBusy)
                work.CancelAsync();

            work = new BackgroundWorker();
            work.DoWork += (o, args) =>
            { //Here we look for data to populate our auto complete list, It could be from a Collection or a database, but for now we'll just search words in textBox2 using FindText

                var task = Task.Factory.StartNew<IList<string>>(() => this.FindText(find, source));
                var l = task.Result;
                args.Result = l;
            };
            work.RunWorkerCompleted += (o, args) =>
            {//Here we update the view once all data has been gathered
                listBox1.ItemsSource = args.Result as IList<string>;
                if (listBox1.Items.Count > 0)
                    pComplete.IsOpen = true;
            };

            //start!
            work.RunWorkerAsync();
        }
        /// <summary>
        /// The find text inside textBox2.
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
                        //When user presses (releases) Enter key, the selected word will appear in textBox1, we also use a flag called 'lockPopup' to avoid firing TextChanged when we do this
                        if (listBox1.SelectedItem != null)
                        {
                            pComplete.IsOpen = false;
                            lockPopup = true;
                            textBox1.Text = listBox1.SelectedItem.ToString();
                        }
                        break;
                    }
                case Key.Down:
                    { //Change selection
                        listBox1.SelectedIndex++;
                        break;
                    }
                case Key.Up:
                    { //Change selection
                        listBox1.SelectedIndex--;
                        break;
                    }
            }
            if (listBox1.SelectedItem == null)
                listBox1.SelectedIndex = 0;
        }

        private void Window_togglePopup(object sender, EventArgs e)
        {
            //If user minimizes we need to hide the popup, on restore, show it again
            if (this.WindowState == System.Windows.WindowState.Minimized)
                pComplete.IsOpen = false;
            else if (this.WindowState == System.Windows.WindowState.Normal && textBox1.Text != string.Empty)
                pComplete.IsOpen = true;
        }

        void App_Activated(object sender, EventArgs e)
        {
            // Application activated, popup should be shown now, but I can't get it to work right. Let's ignore this for now. 
            if (listBox1.Items.Count > 0)
                pComplete.IsOpen = true;
        }

        void App_Deactivated(object sender, EventArgs e)
        {
            // Application deactivated, if user clicks away from window we must hide popup  
            pComplete.IsOpen = false;
        }
    }
}
