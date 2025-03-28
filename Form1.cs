using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using OfficeOpenXml; // EPPlus kütüphanesi
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ProcessExcelFile(filePath);
            }
        }

        private void ProcessExcelFile(string path)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.End.Row;

                var products = new List<Product>();
                var barcodes = new List<Barcode>();
                var prices = new List<Price>();

                var seenProductCodes = new HashSet<string>();

                for (int row = 2; row <= rowCount; row++)
                {
                    string code = worksheet.Cells[row, 1].Text; // Artikel
                    string barcode = worksheet.Cells[row, 2].Text; // Barkod
                    string name = worksheet.Cells[row, 3].Text; // Taným
                    string priceStr = worksheet.Cells[row, 4].Text; // Stþ.Fyt.

                    if (!decimal.TryParse(priceStr, out decimal price)) continue;

                    if (!seenProductCodes.Contains(code))
                    {
                        products.Add(new Product
                        {
                            id = products.Count,
                            code = code,
                            name = name,
                            shortName = name.Length > 25 ? name.Substring(0, 25) : name
                        });
                        prices.Add(new Price
                        {
                            code = code,
                            storeCode = "1057",
                            price = price
                        });
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
                    prices = prices
                };
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                string json = JsonSerializer.Serialize(result, options);
                string outputDirectory = "C:\\JsonConverter";
                Directory.CreateDirectory(outputDirectory);
                string outputPath = Path.Combine(outputDirectory, "converted_output.json");
                File.WriteAllText(outputPath, json);
                MessageBox.Show($"JSON dosyasý baþarýyla oluþturuldu! \n\n{outputPath}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
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
}
