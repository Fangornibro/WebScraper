using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace WebScraper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://bt.rozetka.com.ua/ua/grills/c81235/";
            List<string> links = GetLinks(url);
            List<Product> products = GetProducts(links);
            dataGridView1.DataSource = products;
        }

        private static List<Product> GetProducts(List<string> links)
        {
            List<Product> products = new List<Product>();
            foreach (string link in links)
            {
                HtmlAgilityPack.HtmlDocument doc = GetDocument(link);
                Product product = new Product();
                //Link
                product.Link = link;
                //Condition
                if (doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--green')]") != null)
                {
                    product.Condition = "Є в наявності";
                }
                else if (doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--orange')]") != null)
                {
                    product.Condition = "Закінчується";
                }
                else if(doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--gray')]") != null)
                {
                    product.Condition = "Немає в наявності";
                }
                //Name
                product.Name = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product__title')]").InnerText;
                //Prices
                string priceString = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'product-prices__big')]").InnerText;

                string priceNumber = "";

                for (int i = 0; i < priceString.Length; i++)
                {
                    if (Char.IsDigit(priceString[i]))
                        priceNumber += priceString[i];
                }
                //WithPromo
                if (doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'product-prices__small')]") != null)
                {
                    product.PromoPrice = Convert.ToDouble(priceNumber);

                    priceString = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'product-prices__small')]").InnerText;

                    priceNumber = "";

                    for (int i = 0; i < priceString.Length; i++)
                    {
                        if (Char.IsDigit(priceString[i]))
                            priceNumber += priceString[i];
                    }
                    product.RegularPrice = Convert.ToDouble(priceNumber);
                }
                //WithoutPromo
                else
                {
                    product.RegularPrice = Convert.ToDouble(priceNumber);
                    product.PromoPrice = 0;
                }
                //NumberOfRatings
                if (doc.DocumentNode.SelectSingleNode("//a[text()=' Відгуки ']/span") != null)
                {
                    product.NumberOfRatings = Convert.ToInt32(doc.DocumentNode.SelectSingleNode("//a[text()=' Відгуки ']/span").InnerText);
                }
                else
                {
                    product.NumberOfRatings = 0;
                }
                //NumberOfQuestions
                if (doc.DocumentNode.SelectSingleNode("//a[text()=' Питання ']/span") != null)
                {
                    product.NumberOfQuestions = Convert.ToInt32(doc.DocumentNode.SelectSingleNode("//a[text()=' Питання ']/span").InnerText);
                }
                else
                {
                    product.NumberOfQuestions = 0;
                }
                products.Add(product);
            }
            return products;
        }

        static List<string> GetLinks(string Url)
        {
            string newUrl;
            List<string> links = new List<string>();
            for (int i = 1; i < 2; i++)
            {
                newUrl = Url + "page=" + i + "/";
                HtmlAgilityPack.HtmlDocument doc = GetDocument(newUrl);
                HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'goods-tile__heading')]");
                if (linkNodes != null)
                {
                    foreach (HtmlNode node in linkNodes)
                    {
                        string link = node.Attributes["href"].Value;
                        links.Add(link);
                    }
                }
            }

            return links;
        }

        static HtmlAgilityPack.HtmlDocument GetDocument(string Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument Doc = web.Load(Url);
            return Doc;
        }
    }

    class Product
    {
        public string Link { get; set; }
        public string Condition { get; set; }
        public string Name { get; set; }
        public double RegularPrice { get; set; }
        public double PromoPrice { get; set; }
        public int NumberOfRatings { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}
