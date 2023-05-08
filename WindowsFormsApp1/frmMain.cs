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

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {
        private List<Articulo> lista;
        public frmMain()
        {
            InitializeComponent();
        }
        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                lista = negocio.listar2();
                dgvArticulos.DataSource = lista;
                ocultarColumnas();
                cargarImagen(lista[0].UrlImagen);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ArticuloNegocio negocio = new ArticuloNegocio();
            //lista = negocio.listar();
            //dgvArticulos.DataSource = lista;
            //dgvArticulos.Columns["UrlImagen"].Visible = false;
            //cargar(lista[0].UrlImagen);

            this.Text = "Software Ecommerce";
            cargar();
            cboCampo.Items.Add("Codigo");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");

            cboPrecio.Items.Add("Menor");
            cboPrecio.Items.Add("Mayor");
            cboPrecio.Items.Add("Igual");

            cboMarca.Items.Add("Samsung");
            cboMarca.Items.Add("Apple");
            cboMarca.Items.Add("Motorola");
            cboMarca.SelectedIndex = -1;

            cboCategoria.Items.Add("Celulares");
            cboCategoria.Items.Add("Televisores");
            cboCategoria.SelectedIndex = -1;

            txtPrecio.Enabled = false;

            cboMarca.Enabled = false;
            cboCategoria.Enabled = false;
            cboPrecio.Enabled = false;

        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);

            }
        }
        private void cargarImagen(string url)
        {
            try
            {
                pbxArticulo.Load(url);
            }
            catch (Exception)
            {

                pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.Text = "Agregar Articulo";
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            Articulo seleccionado;
            if(dgvArticulos.CurrentRow != null)
            {
                seleccionado = (Articulo) dgvArticulos.CurrentRow.DataBoundItem;   
                frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
                modificar.Text = "Modificar Articulo";
                modificar.ShowDialog();
                cargar();

            }
            else
            {
                MessageBox.Show("Haga click en una fila para seleccionar");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            eliminar(); 
        }

        // Encapsulamos el metodo eliminar por si en el futuro se modifica para hacer una eliminacion logica.
        private void eliminar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;

            try
            {
                DialogResult res = MessageBox.Show("Esta seguro de eliminar", "Eliminando", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.OK)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                }
                cargar();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvArticulos.Columns["Id"].Visible = false;
            dgvArticulos.Columns["UrlImagen"].Visible = false;
            dgvArticulos.Columns["Precio"].DefaultCellStyle.Format = "0.00";
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltro.Text;
            
            if( filtro.Length >= 3)
            {
                listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().Contains(filtro.ToUpper()) || x.Nombre != null && x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Descripcion!= null && x.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Marca.Descripcion != null && x.Marca.Descripcion.ToUpper().Contains(filtro.ToUpper()) || x.Categoria.Descripcion != null && x.Categoria.Descripcion.ToUpper().Contains(filtro.ToUpper()));
                
            }
            else
            {
                listaFiltrada = lista;
            }
            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion;
            if (cboCampo.SelectedItem == null)
                return;

            opcion = cboCampo.SelectedItem.ToString();
            if(opcion == "Codigo" || opcion == "Nombre" || opcion == "Descripcion")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
                
            }
                
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltro2.Text;
                string marca=null, categoria=null, precio=null, filtroPrecio=null;

                if(ckbOpcional.Checked)
                {
                    if(cboMarca.SelectedIndex != -1)
                        marca = cboMarca.SelectedItem.ToString();
                                         
                    if (cboCategoria.SelectedIndex != -1)
                        categoria = cboCategoria.SelectedItem.ToString();

                    if (cboPrecio.SelectedIndex != -1)
                    {
                        precio = cboPrecio.SelectedItem.ToString();
                        filtroPrecio = txtPrecio.Text;
                        if(string.IsNullOrEmpty(filtroPrecio))
                        {
                            MessageBox.Show("Debes ingresar el filtro para numericos...");
                            return;
                        }
                        if(!soloNumeros(filtroPrecio))
                        {
                            MessageBox.Show("Solo numeros por favor");
                            return;
                        }
                    }

                    dgvArticulos.DataSource = null;
                    // filtro por consulta sql
                    dgvArticulos.DataSource = negocio.FiltroAvanzadoSql(campo, criterio, filtro, marca, categoria, precio, filtroPrecio);

                    // filtro en memoria de la lista usando findAll
                    //dgvArticulos.DataSource = filtroAvanzado(campo, criterio, filtro, marca, categoria, precio, filtroPrecio);
                }
                else
                {
                    dgvArticulos.DataSource = null;
                    //dgvArticulos.DataSource = filtroAvanzado(campo, criterio, filtro);
                    // probamos filtro sql
                    
                    dgvArticulos.DataSource = negocio.FiltroAvanzadoSql(campo, criterio, filtro);
                }

                ocultarColumnas();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor ingrese campo");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor ingrese el criterio");
                return true;
            }
            
            
            if(cboCampo.SelectedItem.ToString() == "Numero")
            {
                //if(string.IsNullOrEmpty(txtFiltro2.Text))
                //{
                //    MessageBox.Show("Por favor ingrese algo para buscar");
                //    return true;
                //}

                if(!soloNumeros(txtFiltro2.Text))
                {
                    MessageBox.Show("Solo numeros por favor");
                    return true;
                }    
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach ( var caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        private List<Articulo>filtroAvanzado(string campo, string criterio, string filtro)
        {
            List<Articulo> listaFiltrada = new List<Articulo>();

            try
            {
                if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().StartsWith(filtro.ToUpper()));
                            //listaFiltrada = lista.FindAll(x => x.Codigo.ToUpper().StartsWith(filtro.ToUpper()));
                            break;

                        case "Termina con":
                            listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().EndsWith(filtro.ToUpper()));
                            break;

                        case "Contiene":
                            listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().Contains(filtro.ToUpper()));
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            listaFiltrada = lista.FindAll(x => x.Nombre.ToUpper().StartsWith(filtro.ToUpper()));
                            break;

                        case "Termina con":
                            listaFiltrada = lista.FindAll(x => x.Nombre.ToUpper().EndsWith(filtro.ToUpper()));
                            break;

                        case "Contiene":
                            listaFiltrada = lista.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()));
                            break;
                    }
                }
                else
                {

                    switch (criterio)
                    {
                        case "Comienza con":
                            listaFiltrada = lista.FindAll(x => x.Descripcion.ToUpper().StartsWith(filtro.ToUpper()));
                            break;

                        case "Termina con":
                            listaFiltrada = lista.FindAll(x => x.Descripcion.ToUpper().EndsWith(filtro.ToUpper()));
                            break;

                        case "Contiene":
                            listaFiltrada = lista.FindAll(x => x.Descripcion.ToUpper().Contains(filtro.ToUpper()));
                            break;
                    }
                }

                return listaFiltrada;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private List<Articulo> filtroAvanzado(string campo, string criterio, string filtro, string marca, string categoria, string precio, string filtroPrecio)
        {
            List<Articulo> listaFiltrada = new List<Articulo>();

            if (campo == "Codigo")
            {
                switch (criterio)
                {
                    case "Comienza con":
                        listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().StartsWith(filtro.ToUpper()));
                        break;

                    case "Termina con":
                        listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().EndsWith(filtro.ToUpper()));
                        break;

                    case "Contiene":
                        listaFiltrada = lista.FindAll(x => x.Codigo != null && x.Codigo.ToUpper().Contains(filtro.ToUpper()));
                        break;
                }
            }
            else if (campo == "Nombre")
            {
                switch (criterio)
                {
                    case "Comienza con":
                        listaFiltrada = lista.FindAll(x => x.Nombre != null && x.Nombre.ToUpper().StartsWith(filtro.ToUpper()));
                        break;

                    case "Termina con":
                        listaFiltrada = lista.FindAll(x => x.Nombre != null && x.Nombre.ToUpper().EndsWith(filtro.ToUpper()));
                        break;

                    case "Contiene":
                        listaFiltrada = lista.FindAll(x => x.Nombre != null && x.Nombre.ToUpper().Contains(filtro.ToUpper()));
                        break;
                }
            }
            else
            {

                switch (criterio)
                {
                    case "Comienza con":
                        listaFiltrada = lista.FindAll(x =>x.Descripcion != null && x.Descripcion.ToUpper().StartsWith(filtro.ToUpper()));
                        break;

                    case "Termina con":
                        listaFiltrada = lista.FindAll(x => x.Descripcion != null && x.Descripcion.ToUpper().EndsWith(filtro.ToUpper()));
                        break;

                    case "Contiene":
                        listaFiltrada = lista.FindAll(x => x.Descripcion != null && x.Descripcion.ToUpper().Contains(filtro.ToUpper()));
                        break;
                }
            }


            /* filtro avanzado */
            if (marca != null)
                listaFiltrada = listaFiltrada.FindAll(x => x.Marca.Descripcion == marca);

            if (categoria != null)
                listaFiltrada = listaFiltrada.FindAll(x => x.Categoria.Descripcion == categoria);

            if (precio != null)
            {
                if(precio == "Mayor")
                {
                    listaFiltrada = listaFiltrada.FindAll(x => x.Precio > decimal.Parse(filtroPrecio));
                }
                else if(precio == "Menor")
                {
                    listaFiltrada = listaFiltrada.FindAll(x => x.Precio < decimal.Parse(filtroPrecio));
                }
                else
                {
                    listaFiltrada = listaFiltrada.FindAll(x => x.Precio == decimal.Parse(filtroPrecio));
                }
            }

            return listaFiltrada;
        }

        

        private void cboPrecio_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPrecio.Enabled = true;
        }

        private void rbtOpcional_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void ckbOpcional_CheckedChanged(object sender, EventArgs e)
        {
            if(ckbOpcional.Checked)
            {
                cboMarca.Enabled = true;
                cboCategoria.Enabled = true;
                cboPrecio.Enabled = true;
            }    
            else
            {
                cboMarca.Enabled = false;
                cboCategoria.Enabled = false;
                cboPrecio.Enabled = false;
                cboMarca.SelectedIndex = -1;
                cboCategoria.SelectedIndex = -1;
                cboPrecio.SelectedIndex = -1;
                txtPrecio.Enabled = false;
                txtPrecio.Text = "";
            }
        }

        private void btnResetear_Click(object sender, EventArgs e)
        {
            cboCampo.SelectedIndex = -1;
            cboCriterio.SelectedIndex = -1;
            txtFiltro2.Text = "";

            if(ckbOpcional.Checked)
            {
                cboMarca.SelectedIndex = -1;
                cboCategoria.SelectedIndex = -1;
                cboPrecio.SelectedIndex = -1;
                txtPrecio.Text = "";
            }
        }

        private void articuloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            foreach (var item in Application.OpenForms)
            {
                if (item.GetType() == typeof(frmAltaArticulo))
                    return;
            }
            
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.Text = "Agregar Articulo";
            alta.Show();
            cargar();

        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if(dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                frmAltaArticulo detalle = new frmAltaArticulo(seleccionado, true);
                detalle.ShowDialog();

            }
        }
    }
}
