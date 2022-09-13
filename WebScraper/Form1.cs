﻿using System;
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
using MySqlX.XDevAPI.Relational;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WebScraper
{
    public partial class WebScraper : Form
    {
        public WebScraper()
        {
            InitializeComponent();
            MonthsTabControlUpdate();
            DateTabControlUpdate(false);
            BrandCategoryUpdate();
            DataGridUpdate();

        }
        public static string MainUrl = "https://bt.rozetka.com.ua/ua/grills/c81235/";
        public List<string> TableNames = GetTables();
        public char interfaceStatement = 'd';
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
            MySqlCommand cmd = new MySqlCommand("SELECT Brand, COUNT(*) FROM `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "` GROUP BY Brand", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt1);
            string counter;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                counter = string.Join(", ", dt1.Rows[i].ItemArray);
                brandCheckedListBox.Items.Add(counter);
            }
            cmd = new MySqlCommand("SELECT COUNT(*) FROM `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`", db.getConnection());
            da.SelectCommand = cmd;
            dt1.Clear();
            da.Fill(dt1);
            counter = string.Join(", ", dt1.Rows[0].ItemArray);
            checkBox1.Text = "All" + counter;
            checkBox2.Text = "Grills" + counter;

            cmd = new MySqlCommand("SELECT Category, COUNT(*) FROM `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "` GROUP BY Category", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                counter = string.Join(", ", dt2.Rows[i].ItemArray);
                categoryCheckedListBox.Items.Add(counter);
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
                        allColumns += "WHERE (`" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`.Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                    }
                    else
                    {
                        allColumns += " OR `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`.Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
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
                        allColumns += "WHERE (`" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                        isSecond = false;
                    }
                    else if (isSecond)
                    {
                        allColumns += " AND (`" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isSecond = false;
                    }
                    else
                    {
                        allColumns += " OR `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                    }
                }
                if (!isFirst && !isSecond)
                {
                    allColumns += ")";
                }
            }
            ShowData(allColumns);
        }
        private void MonthsTabControlUpdate()
        {
            TableNames = GetTables();
            while (tabControl2.TabPages.Count != 0)
            {
                tabControl2.TabPages.Remove(tabControl2.TabPages[0]);
            }
            for (int i = 0; i < TableNames.Count; i++)
            {
                string months = Regex.Match(TableNames[i], "[.](.?)+").Value;
                months = months.Remove(0, 1);
                for (int j = 0; j < tabControl2.TabPages.Count; j++)
                {
                    if (tabControl2.TabPages[j].Text == months)
                    {
                        goto M2;
                    }
                }
                tabControl2.TabPages.Add(months);
            M2:
                continue;
            }
            tabControl2.SelectedIndex = tabControl2.TabPages.Count - 1;
        }
        private void DateTabControlUpdate(bool idSave)
        {
            TableNames = GetTables();
            int id = allDatabasesTabControl.SelectedIndex;
            while (allDatabasesTabControl.TabPages.Count != 0)
            {
                allDatabasesTabControl.TabPages.Remove(allDatabasesTabControl.TabPages[0]);
            }
            for (int i = 0; i < TableNames.Count; i++)
            {
                if (TableNames[i].IndexOf(tabControl2.TabPages[tabControl2.SelectedIndex].Text) != -1)
                {
                    allDatabasesTabControl.TabPages.Add(TableNames[i]);
                }
            }
            if (idSave)
            {
                allDatabasesTabControl.SelectedIndex = id;
            }
            else
            {
                allDatabasesTabControl.SelectedIndex = allDatabasesTabControl.TabPages.Count - 1;
            }
        }

        private void ShowData(string allColumns)
        {
            if (interfaceStatement == 'd')
            {
                mainDataGridView.DataSource = null;
                mainDataGridView.Rows.Clear();
                mainDataGridView.Refresh();
                DB db = new DB();
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `" + allDatabasesTabControl.TabPages[allDatabasesTabControl.SelectedIndex].Text + "`" + allColumns, db.getConnection());
                da.SelectCommand = cmd;
                da.Fill(dt);
                mainDataGridView.DataSource = dt;
            }
            else if (interfaceStatement == 'm')
            {
                mainDataGridView.DataSource = null;
                mainDataGridView.Rows.Clear();
                mainDataGridView.Refresh();
                DB db = new DB();
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter();
                string select = "";
                string from = "";
                string join = "";
                string firstTableName = "";
                bool isFirst = true;
                int counter = 0;
                for (int i = 0; i < TableNames.Count; i++)
                {
                    if (TableNames[i].IndexOf(tabControl2.TabPages[tabControl2.SelectedIndex].Text) != -1)
                    {
                        counter++;
                        if (isFirst)
                        {
                            firstTableName = TableNames[i];
                            select = "SELECT `" + TableNames[i] + "`.Link, `" + TableNames[i] + "`.ProductCondition,`" + TableNames[i] + "`.Segment, `" + TableNames[i] + "`.Brand, `" + TableNames[i] + "`.Model, `" + TableNames[i] + "`.Category, FLOOR ((`" + TableNames[i] + "`.RegularPrice";
                            from = " FROM `" + TableNames[i] + "` ";
                            isFirst = false;
                        }
                        else
                        {
                            join += "INNER JOIN " + "`" + TableNames[i] + "` ON `" + firstTableName + "`.Link = `" + TableNames[i] + "`.Link ";
                            if (i != TableNames.Count - 1)
                            {
                                if (TableNames[i + 1].IndexOf(tabControl2.TabPages[tabControl2.SelectedIndex].Text) == -1)
                                {
                                    select += "+`" + TableNames[i] + "`.RegularPrice)/" + counter + ") AS `Average months price`, `" + firstTableName + "`.NumberOfRatings AS `Number of ratings at the beginning of the month`, `" + TableNames[i] + "`.NumberOfRatings AS `Number of ratings at the end of the month`";
                                }
                                else
                                {
                                    select += "+`" + TableNames[i] + "`.RegularPrice ";
                                }
                            }
                            else
                            {
                                select += "+`" + TableNames[i] + "`.RegularPrice)/" + counter + ") AS `Average months price`, `" + firstTableName + "`.NumberOfRatings AS `Number of ratings at the beginning of the month`, `" + TableNames[i] + "`.NumberOfRatings AS `Number of ratings at the end of the month`";
                            }

                        }
                    }
                }
                MySqlCommand cmd = new MySqlCommand(select + from + join + allColumns, db.getConnection());
                da.SelectCommand = cmd;
                da.FillAsync(dt);
                mainDataGridView.DataSource = dt;
            }
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
            while (Unsorted(TableNames))
            {
                for (int i = 0; i < TableNames.Count - 1; i++)
                {
                    if (Convert.ToInt32(TableNames[i].Remove(0, 3).Remove(2, 5)) > Convert.ToInt32(TableNames[i + 1].Remove(0, 3).Remove(2, 5)))
                    {
                        var temp = TableNames[i];
                        TableNames[i] = TableNames[i + 1];
                        TableNames[i + 1] = temp;
                    }
                }
            }  
            return TableNames;
        }
        private static bool Unsorted(List<string> table)
        {
            for (int i = 0; i < table.Count - 1; i++)
            {
                if (Convert.ToInt32(table[i].Remove(0, 3).Remove(2, 5)) > Convert.ToInt32(table[i + 1].Remove(0, 3).Remove(2, 5)))
                {
                    return true;
                }
            }
            return false;
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
            string curName = DateTime.UtcNow.ToString("dd.MM.yyyy");
            foreach (string table in TableNames)
            {
                if (table == curName)
                {
                    MessageBox.Show(curName + " table already exists.", "Error");
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
            string name = DateTime.UtcNow.ToString("dd.MM.yyyy");
            MySqlCommand cmd = new MySqlCommand("CREATE TABLE " + "`" + name + "`" + " ( `Link` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `ProductCondition` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Segment` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Brand` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Model` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL, `Category` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL  , `RegularPrice` DOUBLE UNSIGNED NOT NULL , `PromoPrice` DOUBLE UNSIGNED NOT NULL , `NumberOfRatings` INT(10) UNSIGNED NOT NULL , `NumberOfQuestions` INT(10) UNSIGNED NOT NULL ) ENGINE = MyISAM CHARSET=utf8 COLLATE utf8_general_ci;", db.getConnection());

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
            foreach (Product product in products)
            {
                InsertProduct(product, name);
            }


            cmd = new MySqlCommand("ALTER TABLE `" + name + "` ADD INDEX(`Link`)", db.getConnection());

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
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

        private static HtmlAgilityPack.HtmlDocument GetDocument(string Url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument Doc = web.Load(Url);
            return Doc;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MonthsTabControlUpdate();
            DateTabControlUpdate(true);
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
                    HtmlNode brand = doc.DocumentNode.SelectSingleNode("//meta[contains(@property, 'og:product:brand')]");
                    //Model
                    product.Model = FullName;
                    if (brand != null)
                    {
                        product.Brand = brand.Attributes["content"].Value;
                    }
                    else if (product.Model.ToLower().IndexOf("bredeco") != -1)
                    {
                        product.Brand = "Bredeco";
                    }
                    else if (product.Model.ToLower().IndexOf("hausberg") != -1)
                    {
                        product.Brand = "Hausberg";
                    }
                    else if (product.Model.ToLower().IndexOf("black+decker") != -1)
                    {
                        product.Brand = "Black+Decker";
                    }
                    else if (product.Model.ToLower().IndexOf("maxxcuisine") != -1)
                    {
                        product.Brand = "Maxx cuisine";
                    }
                    else if (product.Model.ToLower().IndexOf("holmer") != -1)
                    {
                        product.Brand = "Hölmer";
                    }
                    else if (product.Model.ToLower().IndexOf("silver crest") != -1)
                    {
                        product.Brand = "SilverCrest";
                    }
                    else if (product.Model.ToLower().IndexOf("kingsberg") != -1)
                    {
                        product.Brand = "Kingberg";
                    }
                    else if (product.Model.ToLower().IndexOf("tefal") != -1)
                    {
                        product.Brand = "Tefal";
                    }
                    else if (product.Model.ToLower().IndexOf("babale") != -1)
                    {
                        product.Brand = "BaBaLe";
                    }
                    else if (product.Model.ToLower().IndexOf("sybo") != -1)
                    {
                        product.Brand = "SYBO";
                    }
                    else if (product.Model.ToLower().IndexOf("slich") != -1)
                    {
                        product.Brand = "Slich";
                    }
                    else if (product.Model.ToLower().IndexOf("florabest") != -1)
                    {
                        product.Brand = "Florabest";
                    }
                    else if (product.Model.ToLower().IndexOf("kebabs") != -1)
                    {
                        product.Brand = "Kebabs Machine";
                    }
                    else if (product.Model.ToLower().IndexOf("airhot") != -1)
                    {
                        product.Brand = "Airhot";
                    }
                    else if (product.Model.ToLower().IndexOf("ersoz") != -1)
                    {
                        product.Brand = "Ersoz";
                    }
                    else if (product.Model.ToLower().IndexOf("uret") != -1)
                    {
                        product.Brand = "Uret";
                    }
                    else if (product.Model.ToLower().IndexOf("wimpex") != -1)
                    {
                        product.Brand = "Wimpex";
                    }
                    else if (product.Model.ToLower().IndexOf("boska") != -1)
                    {
                        product.Brand = "Boska";
                    }
                    else if (product.Model.ToLower().IndexOf("allbee") != -1)
                    {
                        product.Brand = "Allbee";
                    }
                    else if (product.Model.ToLower().IndexOf("euro star") != -1)
                    {
                        product.Brand = "Euro Star";
                    }
                    else if (product.Model.ToLower().IndexOf("grant") != -1)
                    {
                        product.Brand = "Grant";
                    }
                    else
                    {
                        product.Brand = "Iнше";
                    }
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
                HtmlNode cat = doc.DocumentNode.SelectSingleNode("(//span[text()='Тип']/../..//a)[1]");
                if (cat != null)
                {
                    product.Category = cat.InnerText;
                }
                else
                {
                    if (product.Model.ToLower().IndexOf("вапо") != -1)
                    {
                        product.Category = "Вапо-грилі";
                    }
                    else if (product.Model.ToLower().IndexOf("ракл") != -1)
                    {
                        product.Category = "Раклетниця";
                    }
                    else if (product.Model.ToLower().IndexOf("шашл") != -1)
                    {
                        product.Category = "Електрошашличниця";
                    }
                    else if (product.Model.ToLower().IndexOf("газ") != -1)
                    {
                        product.Category = "Гриль газовий";
                    }
                    else if (product.Model.ToLower().IndexOf("сендв") != -1 || product.Model.ToLower().IndexOf("cен") != -1 || product.Model.ToLower().IndexOf("бутер") != -1 || product.Model.ToLower().IndexOf("ваф") != -1)
                    {
                        product.Category = "Сендвiчниця (вафельниця)";
                    }
                    else if (product.Model.ToLower().IndexOf("вуг") != -1 || product.Model.ToLower().IndexOf("дро") != -1)
                    {
                        product.Category = "Вугiльний(на дровах)";
                    }
                    else if (product.Model.ToLower().IndexOf("сков") != -1)
                    {
                        product.Category = "Сковорода-гриль";
                    }
                    else if (product.Model.ToLower().IndexOf("сос") != -1 || product.Model.ToLower().IndexOf("хот") != -1)
                    {
                        product.Category = "Гриль для сосисок";
                    }
                    else if (product.Model.ToLower().IndexOf("мул") != -1 || product.Model.ToLower().IndexOf("багат") != -1)
                    {
                        product.Category = "Мультимейкер";
                    }
                    else if (product.Model.ToLower().IndexOf("рол") != -1)
                    {
                        product.Category = "Роликовий";
                    }
                    else if (product.Model.ToLower().IndexOf("терм") != -1)
                    {
                        product.Category = "Гриль з терморегулятором";
                    }
                    else if (product.Model.ToLower().IndexOf("аеро") != -1)
                    {
                        product.Category = "Аерогриль";
                    }
                    else if (product.Model.ToLower().IndexOf("шау") != -1)
                    {
                        product.Category = "Апарат для шаурми";
                    }
                    else if (product.Model.ToLower().IndexOf("тост") != -1)
                    {
                        product.Category = "Тостер гриль";
                    }
                    else if (product.Model.ToLower().IndexOf("котл") != -1 || product.Model.ToLower().IndexOf("бург") != -1)
                    {
                        product.Category = "Котлетниця";
                    }
                    else if (product.Model.ToLower().IndexOf("барб") != -1)
                    {
                        product.Category = "Гриль-барбекю";
                    }
                    else if (product.Model.ToLower().IndexOf("фрит") != -1)
                    {
                        product.Category = "Фритюрниця";
                    }
                    else if (product.Model.ToLower().IndexOf("анти") != -1)
                    {
                        product.Category = "З антипригарним покриттям";
                    }
                    else if (product.Model.ToLower().IndexOf("контакт") != -1 || product.Model.ToLower().IndexOf("притис") != -1 || product.Model.ToLower().IndexOf("прижим") != -1)
                    {
                        product.Category = "Контактний (Прижимний)";
                    }
                    else if (product.Model.ToLower().IndexOf("електр") != -1 || product.Model.ToLower().IndexOf("электр") != -1)
                    {
                        product.Category = "Електричний";
                    }
                    else
                    {
                        product.Category = "Iнше";
                    }
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
            MonthsTabControlUpdate();
            DateTabControlUpdate(false);
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

            MonthsTabControlUpdate();
            DateTabControlUpdate(false);
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

        private void tabControl2_MouseUp(object sender, MouseEventArgs e)
        {
            DateTabControlUpdate(false);
            BrandCategoryUpdate();
            DataGridUpdate();
        }

        private void checkBox3_MouseUp(object sender, MouseEventArgs e)
        {
            interfaceStatement = 'd';
            checkBox3.Checked = true;
            checkBox4.Checked = false;
            allDatabasesTabControl.Visible = true;
            BrandCategoryUpdate();
            DataGridUpdate();
        }

        private void checkBox4_MouseUp(object sender, MouseEventArgs e)
        {
            interfaceStatement = 'm';
            checkBox4.Checked = true;
            checkBox3.Checked = false;
            allDatabasesTabControl.Visible = false;
            BrandCategoryUpdate();
            DataGridUpdate();
        }
        private static int GetWeekNumberISO(DateTime date)
        {
            int GetWeekday;
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                GetWeekday = 7;
            }
            else
            {
                GetWeekday = (int)date.DayOfWeek;
            }
            return (date.DayOfYear - GetWeekday + 10) / 7;
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
