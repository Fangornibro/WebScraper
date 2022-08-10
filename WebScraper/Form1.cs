using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

namespace WebScraper
{
    public partial class WebScraper : Form
    {
        public WebScraper()
        {
            InitializeComponent(); 
            List<string> brands = GetBrands("https://bt.rozetka.com.ua/ua/grills/c81235/");
            foreach (string brand in brands)
            {
                categoryCheckedListBox.Items.Add(brand);
            }
            tabControlUpdate();
        }
        private void tabControlUpdate()
        {
            int id = allDatabasesTabControl.SelectedIndex;
            List<string> TableNames = GetTables();

            string allColumns = "";
            bool isFirst = true;
            if (checkBox1.Checked)
            {
                
            }
            else
            {
                foreach (string s in categoryCheckedListBox.CheckedItems)
                {
                    if (isFirst)
                    {
                        allColumns += "WHERE Brand = '" + s + "'";
                        isFirst = false;
                    }
                    else
                    {
                        allColumns += " OR Brand = '" + s + "'";
                    }
                }
            }
            while (allDatabasesTabControl.TabPages.Count != 0)
            {
                allDatabasesTabControl.TabPages.Remove(allDatabasesTabControl.TabPages[0]);
            }
            for (int i = 0; i < TableNames.Count; i++)
            {
                allDatabasesTabControl.TabPages.Add(TableNames[i]);
            }
            allDatabasesTabControl.SelectedIndex = id;
            showData(TableNames[allDatabasesTabControl.SelectedIndex], allColumns);
        }

        private void showData(string tableName, string allColumns)
        {
            mainTable.DataSource = null;
            mainTable.Rows.Clear();
            mainTable.Refresh();
            DB db = new DB();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `" + tableName + "`" + allColumns, db.getConnection());
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
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + "`" + name + "`" + " (`Link`, `ProductCondition`, `Segment`, `Brand`, `Model`, `RegularPrice`, `PromoPrice`, `NumberOfRatings`, `NumberOfQuestions`) VALUES(@L, @PC, @S, @B, @M, @RP, @PP, @NR, @NQ)", db.getConnection());
            cmd.Parameters.Add("@L", MySqlDbType.VarChar).Value = product.Link;
            cmd.Parameters.Add("@PC", MySqlDbType.VarChar).Value = product.ProductCondition;
            cmd.Parameters.Add("@S", MySqlDbType.VarChar).Value = product.Segment;
            cmd.Parameters.Add("@B", MySqlDbType.VarChar).Value = product.Brand;
            cmd.Parameters.Add("@M", MySqlDbType.VarChar).Value = product.Model;
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
            List<string> links = GetLinks(url, Convert.ToInt32(scrapTextBox.Text));
            List<Product> products = GetProducts(links);
            CreateTable(products);
        }

        private List<Product> GetProducts(List<string> links)
        {
            List<Product> products = new List<Product>();
            List<string> brands = GetBrands("https://bt.rozetka.com.ua/ua/grills/c81235/");
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
                else if (doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--blue')]") != null)
                {
                    product.ProductCondition = "Очікується";
                }
                //Name
                string FullName = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product__title')]").InnerText;
                List<string> FullNameSplit = new List<string>();
                FullNameSplit.AddRange(FullName.Split());
                //Segment
                product.Segment = "Grill";
                //Brand
                foreach (string word in FullNameSplit)
                {
                    foreach (string brand in brands)
                    {
                        if (word.ToLower() == brand.ToLower())
                        {
                            product.Brand = word;
                            goto M1;
                        }
                        else
                        {
                            product.Brand = "Інші";
                        }
                    }
                }
                M1:
                //Model
                product.Model = FullName;
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
            MySqlCommand cmd = new MySqlCommand("CREATE TABLE " + "`" + name + "`" + " ( `Link` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `ProductCondition` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Segment` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Brand` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Model` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `RegularPrice` DOUBLE UNSIGNED NOT NULL , `PromoPrice` DOUBLE UNSIGNED NOT NULL , `NumberOfRatings` INT(10) UNSIGNED NOT NULL , `NumberOfQuestions` INT(10) UNSIGNED NOT NULL ) ENGINE = MyISAM CHARSET=utf8 COLLATE utf8_general_ci;", db.getConnection());

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
            foreach (Product product in products)
            {
                insertProduct(product, name);
            }
            tabControlUpdate();
        }

        private List<string> GetLinks(string Url, int numberOfPages)
        {
            string newUrl;
            List<string> links = new List<string>();
            for (int i = 1; i < numberOfPages; i++)
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

        private List<string> GetBrands(string Url)
        {
            List<string> brands = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = GetDocument(Url);
            HtmlNodeCollection brandNodes = doc.DocumentNode.SelectNodes("(//ul[contains(@class, 'checkbox-filter')])[3]/li/a");
            if (brandNodes != null)
            {
                foreach (HtmlNode node in brandNodes)
                {
                    string brand = node.Attributes["data-id"].Value;
                    brands.Add(brand);
                }
            }

            brandNodes = doc.DocumentNode.SelectNodes("(//ul[contains(@class, 'checkbox-filter')])[4]/li/a");
            if (brandNodes != null)
            {
                foreach (HtmlNode node in brandNodes)
                {
                    string brand = node.Attributes["data-id"].Value;
                    brands.Add(brand);
                }
            }
            brands.Add("Інші");
            return brands;
        }

        private HtmlAgilityPack.HtmlDocument GetDocument(string Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument Doc = web.Load(Url);
            return Doc;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControlUpdate();
        }

        private void scrapTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void categoryCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoryCheckedListBox.Items.Count != categoryCheckedListBox.CheckedItems.Count)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            tabControlUpdate();
        }

        private void allDatabasesTabControl_MouseUp(object sender, MouseEventArgs e)
        {
            tabControlUpdate();
        }
        private void checkBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked)
            {
                for (int i = 0; i < categoryCheckedListBox.Items.Count; i++)
                {
                    categoryCheckedListBox.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < categoryCheckedListBox.Items.Count; i++)
                {
                    categoryCheckedListBox.SetItemChecked(i, false);
                }
            }
            tabControlUpdate();
        }
    }

    class Product
    {
        public string Link { get; set; }
        public string ProductCondition { get; set; }
        public string Segment { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double RegularPrice { get; set; }
        public double PromoPrice { get; set; }
        public int NumberOfRatings { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}
