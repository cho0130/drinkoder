using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;

   using Microsoft.Win32;
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Controls.Primitives;
    using System.Windows.Data;
   using System.Windows.Documents;
  using System.Windows.Media;
  namespace WpfApp2 
{
    public partial class MainWindow : Window
    {
        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        string takeout = "";
        public MainWindow()
        {
            InitializeComponent();
            AddNewDrink(drinks);
            DisplayDrinkMenu(drinks);
        }
        private void DisplayDrinkMenu(Dictionary<string, int> myDrinks)
        {
            foreach (var drink in myDrinks)
            {
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };
                var cb = new CheckBox
                {
                    Content = $"{drink.Key} : {drink.Value}元",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18,
                    Foreground = Brushes.Blue,
                    Width = 200,
                    //cb.Height = 60;
                    Margin = new Thickness(5)
                };
                var sl = new Slider
                {
                    Width = 100,
                    //sl.Height = 60;
                    Value = 0,
                    Minimum = 0,
                    Maximum = 10,
                    IsSnapToTickEnabled = true,
                    TickPlacement = TickPlacement.BottomRight,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var lb = new Label
                {
                    Width = 50,
                    //lb.Height = 60;
                    Content = "0",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 18
                };
                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);
                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);
                stackpanel_DrinkMenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(Dictionary<string, int> myDrinks)
        {
            //myDrinks.Add("紅茶大杯", 60);
            //myDrinks.Add("紅茶小杯", 40);
            //myDrinks.Add("綠茶大杯", 60);
            //myDrinks.Add("綠茶小杯", 40);
            //myDrinks.Add("咖啡大杯", 80);
            //myDrinks.Add("咖啡小杯", 60);
            //myDrinks.Add("可樂大杯", 30);
            //myDrinks.Add("可樂小杯", 20);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV檔案|*.csv|文字檔案|*.txt|所有檔案|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = Convert.ToInt32(tokens[1]);
                    myDrinks.Add(drinkName, price);
                }
            }
        }
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            //將訂購的飲料加入訂單orders
            PlaceOrder(orders);

            //顯示訂單細項
            DisplayOrderDetail(orders);
        }
        private void DisplayOrderDetail(Dictionary<string, int> myOrders)
        {
            displayTextBlock.Inlines.Clear();
            Run titleString = new Run
            {
                Text = "您所訂購的飲品：",
                Foreground = Brushes.Blue,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            Run takeoutString = new Run
            {
                Text = $"{takeout}",
                Background = Brushes.Aqua,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            displayTextBlock.Inlines.Add(titleString);
            displayTextBlock.Inlines.Add(takeoutString);
            displayTextBlock.Inlines.Add(new Run("，訂購名下如下：\n"));
            string discountMessage = "";
            double total = 0.0;
            double sellPrice = 0.0;
            int i = 1;
            foreach (var item in myOrders)
            {
                string drinkName = item.Key;
                int quantity = myOrders[drinkName];
                int price = drinks[drinkName];

                total += quantity * price;
                Run detailString = new Run($"訂購品項{i}： {drinkName} X {quantity}杯，每杯{price}元，小計{price * quantity}元\n");
                displayTextBlock.Inlines.Add(detailString);
                i++;
            }
            if (total >= 500)
            {
                discountMessage = "訂購滿500元以上者打8折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                discountMessage = "訂購滿300元以上者打85折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                discountMessage = "訂購滿200元以上者打9折";
                sellPrice = total * 0.9;
            }
            else
            {
                discountMessage = "訂購未滿200元以上者不打折";
                sellPrice = total;
            }

            Italic summaryString = new Italic(new Run
            {
                Text = $"本次訂購總共{myOrders.Count}項，總共{total}元，{discountMessage}，售價{sellPrice}元."
            });

            if (sellPrice >= 500)
            {
                summaryString.Foreground = Brushes.Red;
                displayTextBlock.Inlines.Add(summaryString);
            }
            else if (sellPrice >= 300)
            {
                summaryString.Foreground = Brushes.Purple;
                displayTextBlock.Inlines.Add(summaryString);
            }
            else if (sellPrice >= 200)
            {
                summaryString.Foreground = Brushes.Blue;
                displayTextBlock.Inlines.Add(summaryString);
            }
            else
            {
                summaryString.Foreground = Brushes.Black;
                displayTextBlock.Inlines.Add(summaryString);
            }
        }
        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i = 0; i < stackpanel_DrinkMenu.Children.Count; i++)
            {
                var sp = stackpanel_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[1] as Slider;
                String drinkName = cb.Content.ToString().Substring(0, 4);
                int quantity = Convert.ToInt32(sl.Value);

                if (cb.IsChecked == true && quantity != 0)
                {
                    myOrders.Add(drinkName, quantity);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                takeout = rb.Content.ToString();
                //MessageBox.Show(takeout);
            }
        }
    }
}