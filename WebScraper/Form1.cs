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
using MySql.Data.MySqlClient;

namespace WebScraper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tabControlUpdate();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> TableNames = GetTables();
            if (tabControl1.SelectedIndex >= 0)
            {
                showData(TableNames[tabControl1.SelectedIndex]);
            }
        }
        private void tabControlUpdate()
        {
            List<string> TableNames = GetTables();
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages.Remove(tabControl1.TabPages[i]);
            }
            for (int i = 0; i < TableNames.Count; i++)
            {
                tabControl1.TabPages.Add(TableNames[i]);
            }            
            showData(TableNames[tabControl1.SelectedIndex]);
        }
        private void showData(string tableName)
        {

            DB db = new DB();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `"+ tableName + "`", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt);
            mainTable.DataSource = dt;
        }

        public List<string> GetTables()
        {
            DB db = new DB();
            List<string> TableNames = new List<string>();
            MySqlCommand cmd = new MySqlCommand("show tables", db.getConnection());
            db.openConnection();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TableNames.Add(reader.GetString(0));
            }
            reader.Close();
            db.closeConnection();
            return TableNames;
        }
        private void insertProduct(Product product, string name)
        {
            DB db = new DB();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + "`" + name + "`" + " (`Link`, `ProductCondition`, `Name`, `RegularPrice`, `PromoPrice`, `NumberOfRatings`, `NumberOfQuestions`) VALUES(@L, @PC, @N, @RP, @PP, @NR, @NQ)", db.getConnection());
            cmd.Parameters.Add("@L", MySqlDbType.VarChar).Value = product.Link;
            cmd.Parameters.Add("@PC", MySqlDbType.VarChar).Value = product.ProductCondition;
            cmd.Parameters.Add("@N", MySqlDbType.VarChar).Value = product.Name;
            cmd.Parameters.Add("@RP", MySqlDbType.Double).Value = product.RegularPrice;
            cmd.Parameters.Add("@PP", MySqlDbType.Double).Value = product.PromoPrice;
            cmd.Parameters.Add("@NR", MySqlDbType.Int64).Value = product.NumberOfRatings;
            cmd.Parameters.Add("@NQ", MySqlDbType.Int64).Value = product.NumberOfQuestions;

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://bt.rozetka.com.ua/ua/grills/c81235/";
            List<string> links = GetLinks(url);
            List<Product> products = GetProducts(links);
            CreateTable(products);
            //showData();
        }

        private List<Product> GetProducts(List<string> links)
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
                    product.ProductCondition = "Є в наявності";
                }
                else if (doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--orange')]") != null)
                {
                    product.ProductCondition = "Закінчується";
                }
                else if(doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--gray')]") != null)
                {
                    product.ProductCondition = "Немає в наявності";
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

        private void CreateTable(List<Product> products)
        {
            DB db = new DB();
            string name = DateTime.Today.ToString();
            MySqlCommand cmd = new MySqlCommand("CREATE TABLE " + "`" + name + "`" + " ( `Link` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `ProductCondition` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Name` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `RegularPrice` DOUBLE UNSIGNED NOT NULL , `PromoPrice` DOUBLE UNSIGNED NOT NULL , `NumberOfRatings` INT(10) UNSIGNED NOT NULL , `NumberOfQuestions` INT(10) UNSIGNED NOT NULL ) ENGINE = MyISAM CHARSET=utf8 COLLATE utf8_general_ci;", db.getConnection());

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
            foreach (Product product in products)
            {
                insertProduct(product, name);
            }
            tabControlUpdate();
        }

        private List<string> GetLinks(string Url)
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

        private HtmlAgilityPack.HtmlDocument GetDocument(string Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument Doc = web.Load(Url);
            return Doc;
        }
    }

    class Product
    {
        public string Link { get; set; }
        public string ProductCondition { get; set; }
        public string Name { get; set; }
        public double RegularPrice { get; set; }
        public double PromoPrice { get; set; }
        public int NumberOfRatings { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}
