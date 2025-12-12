using System;
using System.Linq;
using System.Windows.Forms;
using InventorySystem.Data;
using InventorySystem.Models;

namespace InventorySystem
{
    public class SimpleEntryForm : Form
    {
        private ComboBox cbProducts;
        private TextBox txtQty;
        private Button btnOk, btnCancel;
        private AppDbContext _db;

        public SimpleEntryForm(AppDbContext db)
        {
            _db = db;
            Width = 400; Height = 200;
            Text = "Registrar Entrada";

            var lblProd = new Label(){Left=10, Top=20, Text="Producto", Width=100};
            cbProducts = new ComboBox(){Left=120, Top=20, Width=240, DropDownStyle = ComboBoxStyle.DropDownList};
            var lblQty = new Label(){Left=10, Top=60, Text="Cantidad", Width=100};
            txtQty = new TextBox(){Left=120, Top=60, Width=240};

            btnOk = new Button(){Text="Guardar", Left=120, Top=100, DialogResult = DialogResult.OK};
            btnCancel = new Button(){Text="Cancelar", Left=220, Top=100, DialogResult = DialogResult.Cancel};

            Controls.AddRange(new Control[]{lblProd, cbProducts, lblQty, txtQty, btnOk, btnCancel});

            Load += (s,e)=> {
                var products = _db.Products.ToList();
                if(products.Count == 0)
                {
                    cbProducts.Items.Add("-- No hay productos --");
                    cbProducts.SelectedIndex = 0;
                    cbProducts.Enabled = false;
                }
                else
                {
                    cbProducts.DataSource = products;
                    cbProducts.DisplayMember = "Name";
                    cbProducts.ValueMember = "ProductId";
                }
            };

            btnOk.Click += (s,e)=> {
                if(cbProducts.SelectedValue==null || !cbProducts.Enabled) { MessageBox.Show("Selecciona producto"); return; }
                int pid = (int)cbProducts.SelectedValue;
                int qty = 0; int.TryParse(txtQty.Text, out qty);
                if(qty <= 0) { MessageBox.Show("Cantidad inválida"); return; }

                var entry = new Entry{
                    ProductId = pid,
                    Quantity = qty,
                    EntryDate = DateTime.Now
                };
                var prod = _db.Products.Find(pid);
                if(prod!=null) prod.Stock += qty;

                _db.Entries.Add(entry);
                _db.SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
