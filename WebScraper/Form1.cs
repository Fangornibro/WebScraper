using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
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
            dateTimePicker1.Value = DateTime.UtcNow.AddDays(-1);
            dateTimePicker2.Value = DateTime.UtcNow.AddDays(-1);
            BrandCategoryUpdate();
            DataGridUpdate();
        }
        public static string MainUrl = "https://bt.rozetka.com.ua/ua/grills/c81235/";
        public char interfaceStatement = 'd';

        //Data display
        private void ShowData(string allColumns)
        {
            if (dateTimePicker1.Value <= dateTimePicker2.Value)
            {
                if (interfaceStatement == 'd')
                {
                    if (dateTimePicker1.Value == dateTimePicker2.Value)
                    {
                        mainDataGridView.DataSource = null;
                        mainDataGridView.Rows.Clear();
                        mainDataGridView.Refresh();
                        DB db = new DB();
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter();
                        MySqlCommand cmd = new MySqlCommand("SELECT Link, ProductCondition, Segment, Brand, Model, Category, " + comboBox1.Text + " FROM `" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`" + allColumns, db.getConnection());
                        da.SelectCommand = cmd;
                        try
                        {
                            da.Fill(dt);
                        }
                        catch (MySqlException)
                        {
                            MessageBox.Show("There are no tables for the selected dates.");
                        }
                        mainDataGridView.DataSource = dt;
                    }
                    else
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
                        for (int i = 0; i <= (dateTimePicker2.Value - dateTimePicker1.Value).TotalDays; i++)
                        {
                            string table = dateTimePicker1.Value.AddDays(i).ToString("dd.MM.yyyy");
                            if (isFirst)
                            {
                                firstTableName = table;
                                select = "SELECT `" + table + "`.Link, `" + table + "`.ProductCondition,`" + table + "`.Segment, `" + table + "`.Brand, `" + table + "`.Model, `" + table + "`.Category, `" + table + "`." + comboBox1.Text + " AS `" + table + " " + comboBox1.Text + "`";
                                from = " FROM `" + table + "` ";
                                isFirst = false;
                            }
                            else
                            {
                                join += "INNER JOIN " + "`" + table + "` ON `" + firstTableName + "`.Link = `" + table + "`.Link ";
                                select += ",`" + table + "`." + comboBox1.Text + " AS `" + table + " " + comboBox1.Text + "`";

                            }
                        }
                        MySqlCommand cmd = new MySqlCommand(select + from + join + allColumns, db.getConnection());
                        da.SelectCommand = cmd;
                        try
                        {
                            da.Fill(dt);
                        }
                        catch (MySqlException)
                        {
                            MessageBox.Show("There are no tables for the selected dates.");
                        }
                        mainDataGridView.DataSource = dt;
                    }
                }
                else if (interfaceStatement == 'w')
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
                    List<DateTime> sat = GetSat();
                    MySqlCommand cmd = new MySqlCommand();
                    if (sat.Count == 0)
                    {
                        MessageBox.Show("You must choose at least one week.");
                        return;
                    }
                    else if (sat.Count == 1)
                    {
                        cmd = new MySqlCommand("SELECT Link, ProductCondition, Segment, Brand, Model, Category, " + comboBox1.Text + " AS `W" + GetWeekNumberISO(sat[0]) + " " + comboBox1.Text + "` FROM `" + sat[0].ToString("dd.MM.yyyy") + "`" + allColumns, db.getConnection());
                    }
                    else
                    {
                        for (int i = 0; i < sat.Count; i++)
                        {
                            string table = sat[i].ToString("dd.MM.yyyy");
                            if (isFirst)
                            {
                                firstTableName = table;
                                select = "SELECT `" + table + "`.Link, `" + table + "`.ProductCondition,`" + table + "`.Segment, `" + table + "`.Brand, `" + table + "`.Model, `" + table + "`.Category, `" + table + "`." + comboBox1.Text + " AS `W" + GetWeekNumberISO(sat[i]) + " " + comboBox1.Text + "`";
                                from = " FROM `" + table + "` ";
                                isFirst = false;
                            }
                            else
                            {
                                join += "INNER JOIN " + "`" + table + "` ON `" + firstTableName + "`.Link = `" + table + "`.Link ";
                                select += ",`" + table + "`." + comboBox1.Text + " AS `W" + GetWeekNumberISO(sat[i]) + " " + comboBox1.Text + "`";

                            }
                        }
                        cmd = new MySqlCommand(select + from + join + allColumns, db.getConnection());
                    }

                    da.SelectCommand = cmd;
                    try
                    {
                        da.Fill(dt);
                    }
                    catch (MySqlException)
                    {
                        MessageBox.Show("There are no tables for the selected dates.");
                    }
                    mainDataGridView.DataSource = dt;
                }
            }
            else
            {
                MessageBox.Show("Invalid input.");
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
                        allColumns += "WHERE (`" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`.Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                    }
                    else
                    {
                        allColumns += " OR `" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`.Brand = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
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
                        allColumns += "WHERE (`" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isFirst = false;
                        isSecond = false;
                    }
                    else if (isSecond)
                    {
                        allColumns += " AND (`" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                        isSecond = false;
                    }
                    else
                    {
                        allColumns += " OR `" + dateTimePicker1.Value.ToString("dd.MM.yyyy") + "`.Category = '" + Regex.Replace(s, "(\\,[^.]*)$", "") + "'";
                    }
                }
                if (!isFirst && !isSecond)
                {
                    allColumns += ")";
                }
            }
            ShowData(allColumns);
        }

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
            MySqlCommand cmd = new MySqlCommand("SELECT Brand, COUNT(*) FROM `" + dateTimePicker2.Value.ToString("dd.MM.yyyy") + "` GROUP BY Brand ORDER BY COUNT(*) DESC", db.getConnection());
            da.SelectCommand = cmd;
            try
            {
                da.Fill(dt1);
            }
            catch (MySqlException)
            {
                return;
            }
            string counter;
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                counter = string.Join(", ", dt1.Rows[i].ItemArray);
                brandCheckedListBox.Items.Add(counter);
            }
            cmd = new MySqlCommand("SELECT COUNT(*) FROM `" + dateTimePicker2.Value.ToString("dd.MM.yyyy") + "`", db.getConnection());
            da.SelectCommand = cmd;
            dt1.Clear();
            da.Fill(dt1);
            counter = string.Join(", ", dt1.Rows[0].ItemArray);
            checkBox1.Text = "All" + counter;
            checkBox2.Text = "Grills" + counter;

            cmd = new MySqlCommand("SELECT Category, COUNT(*) FROM `" + dateTimePicker2.Value.ToString("dd.MM.yyyy") + "` GROUP BY Category ORDER BY COUNT(*) DESC", db.getConnection());
            da.SelectCommand = cmd;
            da.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                counter = string.Join(", ", dt2.Rows[i].ItemArray);
                categoryCheckedListBox.Items.Add(counter);
            }
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

        private List<DateTime> GetSat()
        {
            List<DateTime> sat = new List<DateTime>();
            for (int i = 0; i <= (dateTimePicker2.Value - dateTimePicker1.Value).TotalDays; i++)
            {
                DateTime table = dateTimePicker1.Value.AddDays(i);
                if (table.DayOfWeek == DayOfWeek.Sunday)
                {
                    sat.Add(table);
                }
            }
            return sat;
        }

        //Data scraping
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
        private void button1_Click(object sender, EventArgs e)
        {
            string curName = DateTime.UtcNow.ToString("dd.MM.yyyy");
            List<string> TableNames = GetTables();
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
                    double starDouble = 0;
                    for (int starID = 1; starID <= 5; starID++)
                    {
                        HtmlNode star = doc.DocumentNode.SelectSingleNode("//div[2]/rz-rating-stars/ul/li[" + starID + "]//*[local-name()='stop'][1]");
                        if (star != null)
                        {
                            starDouble += Convert.ToDouble(star.Attributes["offset"].Value.Replace(".", ","));
                        }
                    }
                    starDouble = Math.Round(starDouble, 2);
                    product.Rating = starDouble;
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

                        string promoPriceString = promoPrice.InnerText;

                        promoPriceString = Regex.Replace(promoPriceString, "([^0-9.]{1,})", "");
                        product.Price = Convert.ToDouble(promoPriceString);
                        product.Promo = Convert.ToDouble(promoPriceString) - Convert.ToDouble(priceString);
                    }
                    //WithoutPromo
                    else
                    {
                        product.Price = Convert.ToDouble(priceString);
                        product.Promo = 0;
                    }
                }
                else
                {
                    product.Price = 0;
                    product.Promo = 0;
                }
                //NumberOfRatings
                HtmlNode ratings = doc.DocumentNode.SelectSingleNode("//a[text()=' Відгуки ']/span");
                if (ratings != null)
                {
                    product.ReviewCount = Convert.ToInt32(ratings.InnerText);
                }
                else
                {
                    product.ReviewCount = 0;
                }
                //NumberOfQuestions
                HtmlNode questions = doc.DocumentNode.SelectSingleNode("//a[text()=' Питання ']/span");
                if (questions != null)
                {
                    product.QuestionCount = Convert.ToInt32(questions.InnerText);
                }
                else
                {
                    product.QuestionCount = 0;
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
            progressBar1.Visible = false;
            label1.Visible = false;
            DataGridUpdate();
        }

        //Table creation
        private void InsertProduct(Product product, string name)
        {
            DB db = new DB();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " + "`" + name + "`" + " (`Link`, `ProductCondition`, `Segment`, `Brand`, `Model`, `Category`, `Price`, `Promo`, `Rating`, `ReviewCount`, `QuestionCount`) VALUES(@L, @PC, @S, @B, @M, @C, @Pri, @Pro, @R, @RC, @QC)", db.getConnection());
            cmd.Parameters.Add("@L", MySqlDbType.VarChar).Value = product.Link;
            cmd.Parameters.Add("@PC", MySqlDbType.VarChar).Value = product.ProductCondition;
            cmd.Parameters.Add("@S", MySqlDbType.VarChar).Value = product.Segment;
            cmd.Parameters.Add("@B", MySqlDbType.VarChar).Value = product.Brand;
            cmd.Parameters.Add("@M", MySqlDbType.VarChar).Value = product.Model;
            cmd.Parameters.Add("@C", MySqlDbType.VarChar).Value = product.Category;
            cmd.Parameters.Add("@Pri", MySqlDbType.Double).Value = product.Price;
            cmd.Parameters.Add("@Pro", MySqlDbType.Double).Value = product.Promo;
            cmd.Parameters.Add("@R", MySqlDbType.Double).Value = product.Rating;
            cmd.Parameters.Add("@RC", MySqlDbType.Int64).Value = product.ReviewCount;
            cmd.Parameters.Add("@QC", MySqlDbType.Int64).Value = product.QuestionCount;

            db.openConnection();

            cmd.ExecuteNonQuery();

            db.closeConnection();
        }

        private void CreateTable(List<Product> products)
        {
            DB db = new DB();
            string name = DateTime.UtcNow.ToString("dd.MM.yyyy");
            MySqlCommand cmd = new MySqlCommand("CREATE TABLE " + "`" + name + "`" + " ( `Link` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `ProductCondition` VARCHAR(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Segment` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Brand` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL , `Model` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL, `Category` VARCHAR(256) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL  , `Price` DOUBLE UNSIGNED NOT NULL , `Promo` DOUBLE UNSIGNED NOT NULL, `Rating` DOUBLE UNSIGNED NOT NULL , `ReviewCount` INT(10) UNSIGNED NOT NULL , `QuestionCount` INT(10) UNSIGNED NOT NULL ) ENGINE = MyISAM CHARSET=utf8 COLLATE utf8_general_ci;", db.getConnection());

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

        //Table deleting
        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the all selected tables?", "Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i <= (dateTimePicker2.Value - dateTimePicker1.Value).TotalDays; i++)
                {
                    DropTable(dateTimePicker1.Value.AddDays(i).ToString("dd.MM.yyyy"));
                }
            }
        }

        private void DropTable(string name)
        {
            DB db = new DB();
            MySqlCommand cmd = new MySqlCommand("DROP TABLE " + "`" + name + "`", db.getConnection());
            db.openConnection();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Table for the date " + name + " does not exist");
            }

            db.closeConnection();
        }

        //Other events
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

        private void checkBox3_MouseUp(object sender, MouseEventArgs e)
        {
            interfaceStatement = 'd';
            checkBox3.Checked = true;
            checkBox4.Checked = false;
            button1.Visible = true;
        }

        private void checkBox4_MouseUp(object sender, MouseEventArgs e)
        {
            interfaceStatement = 'w';
            checkBox4.Checked = true;
            checkBox3.Checked = false;
            button1.Visible = false;
        }

        private void DataGridUpdateEvent(object sender, EventArgs e)
        {
            BrandCategoryUpdate();
            DataGridUpdate();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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
        public double Price { get; set; }
        public double Promo { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int QuestionCount { get; set; }
    }
}
