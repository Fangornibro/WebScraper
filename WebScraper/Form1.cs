using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;

namespace WebScraper
{
    public partial class WebScraper : Form
    {
        public WebScraper()
        {
            InitializeComponent();
            TabControlUpdate(false);
            BrandCategoryUpdate();
            DataGridUpdate();

        }
        public static string MainUrl = "https://bt.rozetka.com.ua/ua/grills/c81235/";
        public List<string> TableNames = GetTables();
        public List<string> brands = GetBrands(MainUrl);
        private void BrandCategoryUpdate()
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            brandCheckedListBox.Items.Clear();
            categoryCheckedListBox.Items.Clear();

            DB db = new DB();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT Brand, COUNT(*) FROM `" + TableNames[allDatabasesTabControl.SelectedIndex] + "` GROUP BY Brand", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt1);
            string rowAsString;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                rowAsString = string.Join(", ", dt1.Rows[i].ItemArray);
                brandCheckedListBox.Items.Add(rowAsString);
            }
            cmd = new MySqlCommand("SELECT COUNT(*) FROM `" + TableNames[allDatabasesTabControl.SelectedIndex] + "`", db.getConnection());
            da.SelectCommand = cmd;
            dt1.Clear();
            da.Fill(dt1);
            rowAsString = string.Join(", ", dt1.Rows[0].ItemArray);
            checkBox1.Text = "All" + rowAsString;
            checkBox2.Text = "Grills" + rowAsString;

            cmd = new MySqlCommand("SELECT Category, COUNT(*) FROM `" + TableNames[allDatabasesTabControl.SelectedIndex] + "` GROUP BY Category", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                rowAsString = string.Join(", ", dt2.Rows[i].ItemArray);
                categoryCheckedListBox.Items.Add(rowAsString);
            }
        }
        private void DataGridUpdate()
        {
            string allColumns = "";
            bool isFirst = true, isSecond = true;
            if (brandCheckedListBox.CheckedItems.Count > 0 || categoryCheckedListBox.CheckedItems.Count > 0)
            {
                foreach (string s in brandCheckedListBox.CheckedItems)
                {
                    if (isFirst)
                    {
                        allColumns += "WHERE (Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                    }
                    else
                    {
                        allColumns += " OR Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                    }
                }
                if (!isFirst)
                {
                    allColumns += ")";
                }
                foreach (string s in categoryCheckedListBox.CheckedItems)
                {
                    if (isFirst)
                    {
                        allColumns += "WHERE (Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                        isSecond = false;
                    }
                    else if (isSecond)
                    {
                        allColumns += " AND (Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isSecond = false;
                    }
                    else
                    {
                        allColumns += " OR Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                    }
                }
                if (!isFirst && !isSecond)
                {
                    allColumns += ")";
                }
            }
            ShowData(allColumns);
        }

        private void TabControlUpdate(bool idSave)
        {
            TableNames = GetTables();
            int id = allDatabasesTabControl.SelectedIndex;
            while (allDatabasesTabControl.TabPages.Count != 0)
            {
                allDatabasesTabControl.TabPages.Remove(allDatabasesTabControl.TabPages[0]);
            }
            for (int i = 0; i < TableNames.Count; i++)
            {
                allDatabasesTabControl.TabPages.Add(TableNames[i]);
            }
            if (idSave)
            {
                allDatabasesTabControl.SelectedIndex = id;
            }
            else
            {
                allDatabasesTabControl.SelectedIndex = TableNames.Count - 1;
            }
        }

        private void ShowData(string allColumns)
        {
            mainDataGridView.DataSource = null;
            mainDataGridView.Rows.Clear();
            mainDataGridView.Refresh();
            DB db = new DB();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM `" + TableNames[allDatabasesTabControl.SelectedIndex] + "`" + allColumns, db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt);
            mainDataGridView.DataSource = dt;
        }

        public static List<string> GetTables()
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
        private void InsertProduct(Product product, string name)
        {
            DB db = new DB();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + "`" + name + "`" + " (`Link`, `ProductCondition`, `Segment`, `Brand`, `Model`, `Category`, `RegularPrice`, `PromoPrice`, `NumberOfRatings`, `NumberOfQuestions`) VALUES(@L, @PC, @S, @B, @M, @C, @RP, @PP, @NR, @NQ)", db.getConnection());
            cmd.Parameters.Add("@L", MySqlDbType.VarChar).Value = product.Link;
            cmd.Parameters.Add("@PC", MySqlDbType.VarChar).Value = product.ProductCondition;
            cmd.Parameters.Add("@S", MySqlDbType.VarChar).Value = product.Segment;
            cmd.Parameters.Add("@B", MySqlDbType.VarChar).Value = product.Brand;
            cmd.Parameters.Add("@M", MySqlDbType.VarChar).Value = product.Model;
            cmd.Parameters.Add("@C", MySqlDbType.VarChar).Value = product.Category;
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
            foreach (string table in TableNames)
            {
                if (table == DateTime.Today.ToString())
                {
                    MessageBox.Show("Today's table already exists.", "Error");
                    return;
                }
            }
            label1.Visible = true;
            label1.Text = "";
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            int numberOfPages = GetNumberOfPages(MainUrl);
            int count = GetLinksCount(MainUrl, numberOfPages);
            progressBar1.Maximum = count + numberOfPages;
            backgroundWorker1.RunWorkerAsync();
        }

        private void CreateTable(List<Product> products)
        {
            DB db = new DB();
            string name = DateTime.Today.ToString();
            MySqlCommand cmd = new MySqlCommand("CREATE TABLE " + "`" + name + "`" + " ( `Link` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `ProductCondition` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Segment` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Brand` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Model` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL, `Category` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL  , `RegularPrice` DOUBLE UNSIGNED NOT NULL , `PromoPrice` DOUBLE UNSIGNED NOT NULL , `NumberOfRatings` INT(10) UNSIGNED NOT NULL , `NumberOfQuestions` INT(10) UNSIGNED NOT NULL ) ENGINE = MyISAM CHARSET=utf8 COLLATE utf8_general_ci;", db.getConnection());

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
            foreach (Product product in products)
            {
                InsertProduct(product, name);
            }
        }
        private int GetNumberOfPages(string Url)
        {
            HtmlAgilityPack.HtmlDocument doc = GetDocument(Url);
            HtmlNode linkNode = doc.DocumentNode.SelectSingleNode("(//a[contains(@class, 'pagination__link ng-star-inserted')])[last()]");
            string link = linkNode.Attributes["href"].Value;
            link = Regex.Match(link, "(?:.(?!\\=))+$").Value;
            link = Regex.Replace(link, "([^0-9.]{1,})", "");
            return Convert.ToInt32(link);
        }

        private List<string> GetLinks(string Url, int numberOfPages)
        {
            HtmlAgilityPack.HtmlDocument doc;
            HtmlNodeCollection linkNodes;
            string link;
            string newUrl;
            List<string> links = new List<string>();
            for (int i = 1; i <= numberOfPages; i++)
            {
                backgroundWorker1.ReportProgress(0);
                newUrl = Url + "page=" + i + "/";
                doc = GetDocument(newUrl);
                linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'goods-tile__heading')]");
                if (linkNodes != null)
                {
                    foreach (HtmlNode node in linkNodes)
                    {
                        link = node.Attributes["href"].Value;
                        links.Add(link);
                    }
                }
            }

            return links;
        }

        private int GetLinksCount(string Url, int numberOfPages)
        {
            string newUrl = Url + "page=" + numberOfPages + "/";
            HtmlAgilityPack.HtmlDocument doc = GetDocument(newUrl);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'goods-tile__heading')]");
            int count = ((numberOfPages - 1) * 60) + linkNodes.Count;

            return count;
        }

        private static List<string> GetBrands(string Url)
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

        private static HtmlAgilityPack.HtmlDocument GetDocument(string Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument Doc = web.Load(Url);
            return Doc;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TabControlUpdate(true);
            BrandCategoryUpdate();
            DataGridUpdate();
        }

        private void categoryCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (brandCheckedListBox.Items.Count != brandCheckedListBox.CheckedItems.Count)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            DataGridUpdate();
        }

        private void allDatabasesTabControl_MouseUp(object sender, MouseEventArgs e)
        {
            BrandCategoryUpdate();
            DataGridUpdate();
        }
        private void checkBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked)
            {
                for (int i = 0; i < brandCheckedListBox.Items.Count; i++)
                {
                    brandCheckedListBox.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < brandCheckedListBox.Items.Count; i++)
                {
                    brandCheckedListBox.SetItemChecked(i, false);
                }
            }
            DataGridUpdate();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            int numberOfPages = GetNumberOfPages(MainUrl);
            List<string> links = GetLinks(MainUrl, numberOfPages);
            List<Product> products = new List<Product>();
            foreach (string link in links)
            {
                backgroundWorker1.ReportProgress(0);
                HtmlAgilityPack.HtmlDocument doc = GetDocument(link);

                Product product = new Product();
                //Name
                HtmlNode name = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product__title')]");
                if (name != null)
                {
                    string FullName = name.InnerText;
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
                }
                else
                {
                    continue;
                }
                //Link
                product.Link = link;
                //Condition
                HtmlNode cond = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'status-label status-label--')]");
                if (cond != null)
                {
                    product.ProductCondition = cond.InnerText;
                }
                else
                {
                    product.ProductCondition = "Знято з виробництва";
                }
                //Category
                HtmlNode cat = doc.DocumentNode.SelectSingleNode("(//dd[contains(@class, 'characteristics-full__value')]//a[contains(@class, 'ng-star-inserted')])[1]");
                if (cat != null)
                {
                    product.Category = cat.InnerText;
                }
                else
                {
                    product.Category = "";
                }
                //Prices
                HtmlNode price = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'product-prices__big')]");
                if (price != null)
                {
                    string priceString = price.InnerText;

                    priceString = Regex.Replace(priceString, "([^0-9.]{1,})", "");
                    //WithPromo
                    HtmlNode promoPrice = doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'product-prices__small')]");
                    if (promoPrice != null)
                    {
                        product.PromoPrice = Convert.ToDouble(priceString);

                        string promoPriceString = promoPrice.InnerText;

                        promoPriceString = Regex.Replace(promoPriceString, "([^0-9.]{1,})", "");
                        product.RegularPrice = Convert.ToDouble(promoPriceString);
                    }
                    //WithoutPromo
                    else
                    {
                        product.RegularPrice = Convert.ToDouble(priceString);
                        product.PromoPrice = 0;
                    }
                }
                else
                {
                    product.RegularPrice = 0;
                    product.PromoPrice = 0;
                }
                //NumberOfRatings
                HtmlNode ratings = doc.DocumentNode.SelectSingleNode("//a[text()=' Відгуки ']/span");
                if (ratings != null)
                {
                    product.NumberOfRatings = Convert.ToInt32(ratings.InnerText);
                }
                else
                {
                    product.NumberOfRatings = 0;
                }
                //NumberOfQuestions
                HtmlNode questions = doc.DocumentNode.SelectSingleNode("//a[text()=' Питання ']/span");
                if (questions != null)
                {
                    product.NumberOfQuestions = Convert.ToInt32(questions.InnerText);
                }
                else
                {
                    product.NumberOfQuestions = 0;
                }
                products.Add(product);
            }
            CreateTable(products);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value += 1;
            label1.Text = progressBar1.Value + "/" + progressBar1.Maximum;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TabControlUpdate(false);
            BrandCategoryUpdate();
            DataGridUpdate();
            progressBar1.Visible = false;
            label1.Visible = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the " + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + " table?", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                DropTable(allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text);
            }
        }

        private void DropTable(string name)
        {
            DB db = new DB();
            MySqlCommand cmd = new MySqlCommand("DROP TABLE " + "`" + name + "`", db.getConnection());
            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();

            TabControlUpdate(false);
            BrandCategoryUpdate();
            DataGridUpdate();
        }

        private void categoryCheckedListBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (categoryCheckedListBox.Items.Count != categoryCheckedListBox.CheckedItems.Count)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
            DataGridUpdate();
        }

        private void checkBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkBox2.Checked)
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
            DataGridUpdate();
        }
    }

    class Product
    {
        public string Link { get; set; }
        public string ProductCondition { get; set; }
        public string Segment { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public double RegularPrice { get; set; }
        public double PromoPrice { get; set; }
        public int NumberOfRatings { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}
