using System;
using System.Linq;
using System.Windows.Forms;
using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem
{
    public class MainForm : Form
    {
        private TabControl tab;
        private DataGridView dgvProducts;
        private DataGridView dgvSuppliers;
        private DataGridView dgvEntries;
        private Button btnAddProduct, btnEditProduct, btnDelProduct, btnRefresh;
        private Button btnAddSupplier, btnEditSupplier, btnDelSupplier;
        private Button btnAddEntry, btnDelEntry;
        private Button btnLowStockReport;
        private AppDbContext _db;

        public MainForm()
        {
            Text = "Sistema de Control de Inventario - POO2";
            Width = 1000; Height = 600;
            _db = new AppDbContext();

            tab = new TabControl() { Dock = DockStyle.Fill };
            var tabProducts = new TabPage("Productos");
            var tabSuppliers = new TabPage("Proveedores");
            var tabEntries = new TabPage("Entradas");
            tab.TabPages.AddRange(new[] { tabProducts, tabSuppliers, tabEntries });

            // Products grid
            dgvProducts = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AutoGenerateColumns = true };
            btnAddProduct = new Button { Text = "Agregar", Left = 10, Top = 360 };
            btnEditProduct = new Button { Text = "Editar", Left = 100, Top = 360 };
            btnDelProduct = new Button { Text = "Eliminar", Left = 190, Top = 360 };
            btnRefresh = new Button { Text = "Refrescar", Left = 280, Top = 360 };
            btnLowStockReport = new Button { Text = "Reporte Stock Bajo", Left = 380, Top = 360 };

            btnAddProduct.Click += (s,e)=> AddProduct();
            btnEditProduct.Click += (s,e)=> EditProduct();
            btnDelProduct.Click += (s,e)=> DeleteProduct();
            btnRefresh.Click += (s,e)=> LoadProducts();
            btnLowStockReport.Click += (s,e)=> ShowLowStock();

            tabProducts.Controls.AddRange(new Control[]{dgvProducts, btnAddProduct, btnEditProduct, btnDelProduct, btnRefresh, btnLowStockReport});

            // Suppliers grid
            dgvSuppliers = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AutoGenerateColumns = true };
            btnAddSupplier = new Button { Text = "Agregar", Left = 10, Top = 360 };
            btnEditSupplier = new Button { Text = "Editar", Left = 100, Top = 360 };
            btnDelSupplier = new Button { Text = "Eliminar", Left = 190, Top = 360 };
            btnAddSupplier.Click += (s,e)=> AddSupplier();
            btnEditSupplier.Click += (s,e)=> EditSupplier();
            btnDelSupplier.Click += (s,e)=> DeleteSupplier();
            tabSuppliers.Controls.AddRange(new Control[]{dgvSuppliers, btnAddSupplier, btnEditSupplier, btnDelSupplier});

            // Entries grid
            dgvEntries = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AutoGenerateColumns = true };
            btnAddEntry = new Button { Text = "Registrar Entrada", Left = 10, Top = 360 };
            btnDelEntry = new Button { Text = "Eliminar Entrada", Left = 140, Top = 360 };
            btnAddEntry.Click += (s,e)=> AddEntry();
            btnDelEntry.Click += (s,e)=> DeleteEntry();
            tabEntries.Controls.AddRange(new Control[]{dgvEntries, btnAddEntry, btnDelEntry});

            Controls.Add(tab);

            Load += (s,e)=> {
                // Try to ensure DB is accessible
                try
                {
                    // Use EnsureCreated to avoid migration requirement on the reviewer's environment
                    _db.Database.EnsureCreated();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("No se pudo crear o conectar la BD. Asegúrate que la BD existe y la cadena de conexión es correcta.\n" + ex.Message);
                }
                LoadProducts();
                LoadSuppliers();
                LoadEntries();
            };
        }

        private void LoadProducts()
        {
            dgvProducts.DataSource = _db.Products.Include(p=>p.Supplier).Select(p=> new {
                p.ProductId, p.Name, p.Description, Supplier = p.Supplier != null ? p.Supplier.Name : "", p.Stock, p.Price
            }).ToList();
        }

        private void LoadSuppliers()
        {
            dgvSuppliers.DataSource = _db.Suppliers.ToList();
        }

        private void LoadEntries()
        {
            dgvEntries.DataSource = _db.Entries.Include(e=>e.Product).Select(e=> new {
                e.EntryId, Product = e.Product != null ? e.Product.Name : "", e.Quantity, e.EntryDate, e.Notes
            }).ToList();
        }

        // Product CRUD dialogs (simple input boxes)
        private void AddProduct()
        {
            using(var f = new SimpleProductForm(_db))
            {
                if(f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadProducts();
                    LoadEntries();
                }
            }
        }
        private void EditProduct()
        {
            if(dgvProducts.CurrentRow == null) return;
            var id = (int)dgvProducts.CurrentRow.Cells["ProductId"].Value;
            var prod = _db.Products.Find(id);
            if(prod == null) return;
            using(var f = new SimpleProductForm(_db, prod))
            {
                if(f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadProducts();
                    LoadEntries();
                }
            }
        }
        private void DeleteProduct()
        {
            if(dgvProducts.CurrentRow == null) return;
            var id = (int)dgvProducts.CurrentRow.Cells["ProductId"].Value;
            var prod = _db.Products.Find(id);
            if(prod == null) return;
            if(MessageBox.Show("Eliminar producto?","Confirmar",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                _db.Products.Remove(prod);
                _db.SaveChanges();
                LoadProducts();
            }
        }

        // Suppliers
        private void AddSupplier()
        {
            string name = Prompt.ShowDialog("Nombre del proveedor:", "Agregar proveedor");
            if(string.IsNullOrWhiteSpace(name)) return;
            var s = new Supplier { Name = name };
            _db.Suppliers.Add(s);
            _db.SaveChanges();
            LoadSuppliers();
        }
        private void EditSupplier()
        {
            if(dgvSuppliers.CurrentRow == null) return;
            var id = (int)dgvSuppliers.CurrentRow.Cells["SupplierId"].Value;
            var sup = _db.Suppliers.Find(id);
            if(sup==null) return;
            string name = Prompt.ShowDialog("Nombre del proveedor:", "Editar proveedor", sup.Name);
            if(string.IsNullOrWhiteSpace(name)) return;
            sup.Name = name;
            _db.SaveChanges();
            LoadSuppliers();
        }
        private void DeleteSupplier()
        {
            if(dgvSuppliers.CurrentRow == null) return;
            var id = (int)dgvSuppliers.CurrentRow.Cells["SupplierId"].Value;
            var sup = _db.Suppliers.Find(id);
            if(sup==null) return;
            if(MessageBox.Show("Eliminar proveedor?","Confirmar",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                _db.Suppliers.Remove(sup);
                _db.SaveChanges();
                LoadSuppliers();
                LoadProducts(); // refresh products to show supplier removed (set to null)
            }
        }

        // Entries
        private void AddEntry()
        {
            using(var f = new SimpleEntryForm(_db))
            {
                if(f.ShowDialog() == DialogResult.OK)
                {
                    LoadEntries();
                    LoadProducts();
                }
            }
        }
        private void DeleteEntry()
        {
            if(dgvEntries.CurrentRow==null) return;
            var id = (int)dgvEntries.CurrentRow.Cells["EntryId"].Value;
            var en = _db.Entries.Find(id);
            if(en==null) return;
            if(MessageBox.Show("Eliminar entrada?","Confirmar",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                // adjust stock
                var prod = _db.Products.Find(en.ProductId);
                if(prod!=null) prod.Stock -= en.Quantity;
                _db.Entries.Remove(en);
                _db.SaveChanges();
                LoadEntries();
                LoadProducts();
            }
        }

        private void ShowLowStock()
        {
            int threshold = int.Parse(Program.Configuration["LowStockThreshold"] ?? "5");
            var low = _db.Products.Where(p=>p.Stock < threshold).Select(p=> new {p.ProductId, p.Name, p.Stock}).ToList();
            string msg = low.Count==0? "No hay productos con stock bajo." : string.Join("\n", low.Select(l=>$"{l.ProductId} - {l.Name}: {l.Stock}")); 
            MessageBox.Show(msg, "Reporte - Stock bajo");
        }
    }

    // Simple prompt utility
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultText = "")
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                Text = caption
            };
            Label textLabel = new Label() { Left = 50, Top=20, Text = text, Width=400 };
            TextBox textBox = new TextBox() { Left = 50, Top=50, Width=400, Text=defaultText };
            Button confirmation = new Button() { Text = "Ok", Left=350, Width=100, Top=80, DialogResult = DialogResult.OK };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
