using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using System.Configuration;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class frmAltaArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        private bool detalle = false;
        public frmAltaArticulo()
        {
            InitializeComponent();
        }

        public frmAltaArticulo(Articulo seleccionado)
        {
            InitializeComponent();
            articulo = seleccionado;
        }

        public frmAltaArticulo(Articulo seleccionado, bool detalle)
        {
            InitializeComponent();
            articulo = seleccionado;
            this.detalle = true;
        }

        private void cargarImagen(string url)
        {
            try
            {
                pbxImagen.Load(url);
            }
            catch (Exception )
            {

                pbxImagen.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (articulo == null)
                    articulo = new Articulo();
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.UrlImagen = txtUrlImagen.Text;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.Marca = (Tipo)cboMarca.SelectedItem; // repasar esta parte
                articulo.Categoria = (Tipo)cboCategoria.SelectedItem; // repasar esta parte

                if(articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Articulo modificado exitosamente");

                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Articulo agregado exitosamente");
                }

                if(archivo != null && txtUrlImagen.Text.ToUpper().Contains("HTTPS"))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["carpeta"] + archivo.SafeFileName);
                }

                Close(); // para que cierre la ventana luego de agregar

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            TipoNegocio tiponegocio = new TipoNegocio();

            try
            {
                cboMarca.DataSource = tiponegocio.listarMarcas();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
                cboCategoria.DataSource = tiponegocio.listarCategoria();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                if(articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtUrlImagen.Text = articulo.UrlImagen;
                    txtPrecio.Text = articulo.Precio.ToString();
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                    cargarImagen(articulo.UrlImagen);

                }

                if(detalle)
                {
                    this.Text = "Detalle Articulo";
                    cboMarca.Enabled = false;
                    cboCategoria.Enabled = false;
                    txtCodigo.Enabled = false;
                    txtNombre.Enabled = false;
                    txtDescripcion.Enabled = false;
                    txtUrlImagen.Enabled = false;
                    txtPrecio.Enabled = false;
                    btnAgregarImg.Visible = false;
                    btnAceptar.Visible = false;
                }    

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                
            }
        }

        private void tbxUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnAgregarImg_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
            }
        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == ',')
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
