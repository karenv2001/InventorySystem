using System;
using System.Linq;
using System.Windows.Forms;
using InventorySystem.Data;
using InventorySystem.Models;

namespace InventorySystem
{
    public class SimpleProductForm : Form
    {
        private TextBox txtName, txtDesc, txtPrice, txtStock;
        private ComboBox cbSuppliers;
        private Button btnOk, btnCancel;
        private AppDbContext _db;
        private Product? _product;

        public SimpleProductForm(AppDbContext db, Product? product = null)
        {
            _db = db;
            _product = product;
            Width = 400; Height = 300;
            Text = product == null ? "Agregar Producto" : "Editar Producto";

            var lblName = new Label(){Left=10, Top=20, Text="Nombre", Width=100};
            txtName = new TextBox(){Left=120, Top=20, Width=240};
            var lblDesc = new Label(){Left=10, Top=60, Text="Descripción", Width=100};
            txtDesc = new TextBox(){Left=120, Top=60, Width=240};
            var lblSupplier = new Label(){Left=10, Top=100, Text="Proveedor", Width=100};
            cbSuppliers = new ComboBox(){Left=120, Top=100, Width=240, DropDownStyle = ComboBoxStyle.DropDownList};
            var lblStock = new Label(){Left=10, Top=140, Text="Stock", Width=100};
            txtStock = new TextBox(){Left=120, Top=140, Width=240};
            var lblPrice = new Label(){Left=10, Top=180, Text="Precio", Width=100};
            txtPrice = new TextBox(){Left=120, Top=180, Width=240};

            btnOk = new Button(){Text="Guardar", Left=120, Top=220, DialogResult = DialogResult.OK};
            btnCancel = new Button(){Text="Cancelar", Left=220, Top=220, DialogResult = DialogResult.Cancel};

            Controls.AddRange(new Control[]{lblName, txtName, lblDesc, txtDesc, lblSupplier, cbSuppliers, lblStock, txtStock, lblPrice, txtPrice, btnOk, btnCancel});

            Load += (s,e)=> {
                var suppliers = _db.Suppliers.ToList();
                // If no suppliers, add a placeholder option
                if(suppliers.Count == 0)
                {
                    cbSuppliers.Items.Add("-- Ninguno --");
                    cbSuppliers.SelectedIndex = 0;
                    cbSuppliers.Enabled = false;
                }
                else
                {
                    cbSuppliers.DataSource = suppliers;
                    cbSuppliers.DisplayMember = "Name";
                    cbSuppliers.ValueMember = "SupplierId";
                }

                if(_product!=null)
                {
                    txtName.Text = _product.Name;
                    txtDesc.Text = _product.Description;
                    txtStock.Text = _product.Stock.ToString();
                    txtPrice.Text = _product.Price.ToString();
                    if(_product.SupplierId.HasValue && cbSuppliers.Enabled)
                        cbSuppliers.SelectedValue = _product.SupplierId.Value;
                }
            };

            btnOk.Click += (s,e)=> {
                if(string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Nombre requerido"); return; }
                int stock = 0; int.TryParse(txtStock.Text, out stock);
                decimal price = 0; decimal.TryParse(txtPrice.Text, out price);
                int? supplierId = null;
                if(cbSuppliers.Enabled && cbSuppliers.SelectedValue != null)
                    supplierId = (int)cbSuppliers.SelectedValue;

                if(_product==null)
                {
                    var p = new Product{
                        Name = txtName.Text,
                        Description = txtDesc.Text,
                        Stock = stock,
                        Price = price,
                        SupplierId = supplierId
                    };
                    _db.Products.Add(p);
                } else {
                    _product.Name = txtName.Text;
                    _product.Description = txtDesc.Text;
                    _product.Stock = stock;
                    _product.Price = price;
                    _product.SupplierId = supplierId;
                    _db.Products.Update(_product);
                }
                _db.SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
