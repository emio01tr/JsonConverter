using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using OfficeOpenXml; // EPPlus kütüphanesi
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using JsonConverter;

namespace ProductJsonExporter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked && string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Ürünlerin grup fiyatý mý yoksa maðaza fiyatý mý olduðu belirtilmelidir.!", btnSelectFile.Text);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ProcessExcelFile(filePath);
            }
        }

        private decimal ParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            decimal value;

            // Önce TR formatý
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out value))
                return value;

            // Nokta varsa virgüle çevirip tekrar dene
            string normalized = input.Replace(".", ",");
            if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out value))
                return value;

            throw new Exception($"Geçersiz fiyat formatý: {input}");
        }

        private async void ProcessExcelFile(string path)
        {
            string storeCode = textBox1.Text;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.End.Row;

                var products = new List<Product>();
                var barcodes = new List<Barcode>();
                var prices = new List<Price>();
                var groupPrices = new List<GroupPrice>();

                var seenProductCodes = new HashSet<string>();

                for (int row = 2; row <= rowCount; row++)
                {

                    string code = worksheet.Cells[row, 1].Text; // Artikel
                    string barcode = worksheet.Cells[row, 2].Text; // Barkod
                    string name = worksheet.Cells[row, 3].Text; // Taným
                    string priceStr = worksheet.Cells[row, 4].Text; // Stþ.Fyt.
                    string secondPriceStr = worksheet.Cells[row, 5].Text ?? ""; // ikinci fiyat

                    if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(barcode))
                    {
                        continue;
                    }

                    decimal price;
                    decimal secondPrice = 0;

                    try
                    {
                        price = ParseDecimal(priceStr);

                        if (!string.IsNullOrEmpty(secondPriceStr))
                            secondPrice = ParseDecimal(secondPriceStr);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Satýr {row} -> {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    if (!seenProductCodes.Contains(code))
                    {
                        products.Add(new Product
                        {
                            id = products.Count,
                            code = code,
                            name = name,
                            shortName = name.Length > 25 ? name.Substring(0, 25) : name
                        });

                        if (!string.IsNullOrEmpty(storeCode))
                        {
                            prices.Add(new Price
                            {
                                code = code,
                                storeCode = $"{storeCode}",
                                price = price,
                                price2 = secondPrice
                            });
                        }
                        if (checkBox1.Checked)
                        {
                            groupPrices.Add(new GroupPrice
                            {
                                code = code,
                                groupId = 1,
                                price = price,
                                price2 = secondPrice
                            });
                        }



                        seenProductCodes.Add(code);
                    }

                    barcodes.Add(new Barcode
                    {
                        code = code,
                        barcode = barcode
                    });
                }

                var result = new
                {
                    products = products,
                    barcodes = barcodes,
                    prices = prices,
                    groupPrices = groupPrices
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };

                string json = JsonSerializer.Serialize(result, options);

                if (!string.IsNullOrEmpty(requesturl.Text))
                {
                    using (var progress = new ProgressForm())
                    {
                        progress.Show(this);
                        await Task.Delay(2500);

                        var token = await GetToken();
                        var productResponse = await SendProducts(token, json);

                        if (productResponse)
                        {
                            progress.Close();
                            MessageBox.Show(this,$"Ürünler belirtilen adrese baþarýlý ile gönderildi. Url: {requesturl.Text}", "Baþarýlý");
                        }
                        else
                        {
                            progress.Close();
                            MessageBox.Show(this,$"Hata: Url bilgisini tekrar kontrol edin. Url: {requesturl.Text}", "Hata!");
                        }
                    }

                }


                string outputDirectory = "C:\\JsonConverter";
                Directory.CreateDirectory(outputDirectory);
                string outputPath = Path.Combine(outputDirectory, "converted_output.json");
                File.WriteAllText(outputPath, json);
            }

            MessageBox.Show($"JSON dosyasý baþarýyla oluþturuldu! \n\n", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }


        public async Task<string> GetToken()
        {

            TokenBody tokenBody = new();
            string tokenJson = JsonSerializer.Serialize(tokenBody);
            var tokenUrl = $"{requesturl.Text}";
            var tokenEndpoint = "token";
            var tokenContent = new StringContent(tokenJson, Encoding.UTF8, "application/json");

            HttpClient tokenClient = new HttpClient();
            tokenClient.BaseAddress = new Uri(tokenUrl);
            tokenClient.Timeout = TimeSpan.FromSeconds(30);

            var tokenResponse = await tokenClient.PostAsync(tokenEndpoint, tokenContent);

            if (tokenResponse != null)
            {
                var token = await tokenResponse.Content.ReadAsStringAsync();
                return token.Trim('"');
            }
            return "";
        }
        public async Task<bool> SendProducts(string token, string json)
        {
            var url = $"{requesturl.Text}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(150);
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Product/add-bulk", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public class Product
        {
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string shortName { get; set; }
            public string unitCode { get; set; } = "AD";
            public int vatPercent { get; set; } = 10;
            public int vatId { get; set; } = 5;
            public int buyingVatPercent { get; set; } = 10;
            public int buyingVatId { get; set; } = 5;
            public bool isActive { get; set; } = true;
            public bool isGivesBonus { get; set; } = true;
            public int bonusMultiplier { get; set; } = 0;
            public int priceEntryType { get; set; } = 0;
            public int returnType { get; set; } = 1;
            public int quantityType { get; set; } = 0;
            public int discountType { get; set; } = 1;
            public int scaleType { get; set; } = 0;
            public int currneysTypeId { get; set; } = 1;
            public int salesmanSetting { get; set; } = 0;
            public bool isLunchVoucher { get; set; } = true;
            public int installmentNumber { get; set; } = 0;
            public int installmentType { get; set; } = 0;
            public string description { get; set; } = "";
            public int productType { get; set; } = 0;
            public int salesInformationsId { get; set; } = 0;
            public int maxQuantity { get; set; } = 0;
        }

        public class Barcode
        {
            public string code { get; set; }
            public string barcode { get; set; }
            public string unitCode { get; set; } = "AD";
            public int priceId { get; set; } = 0;
            public int quantity { get; set; } = 1;
        }

        public class Price
        {
            public string code { get; set; }
            public string storeCode { get; set; }
            public int priceId { get; set; } = 0;
            public decimal price { get; set; }
            public decimal price2 { get; set; } = 0;
            public decimal price3 { get; set; } = 0;
            public decimal price4 { get; set; } = 0;
            public decimal price5 { get; set; } = 0;
            public bool isActive { get; set; } = true;
            public object nextPrices { get; set; } = new { };
            public int maxQuantity { get; set; } = 0;
            public bool isOnSale { get; set; } = true;
        }

        public class GroupPrice
        {
            public string code { get; set; }
            public int groupId { get; set; }
            public int priceId { get; set; } = 0;
            public decimal price { get; set; }
            public decimal price2 { get; set; } = 0;
            public decimal price3 { get; set; } = 0;
            public decimal price4 { get; set; } = 0;
            public decimal price5 { get; set; } = 0;
            public object nextPrices { get; set; } = new { };
        }

        public class TokenBody
        {
            public string grant_type { get; set; } = "password";
            public string username { get; set; } = "kasa";
            public string password { get; set; } = "81dc9bdb52d04dc20036dbd8313ed055";
        }
    }
}
