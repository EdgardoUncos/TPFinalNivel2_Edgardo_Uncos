using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class UtilidadesVarias
    {
        public void cargarImagen(PictureBox pbxArticulo, string url)
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

        public bool validarFiltro(ComboBox cboCampo, ComboBox cboCriterio, TextBox txtFiltro2)
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor ingrese campo");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor ingrese el criterio");
                return true;
            }


            if (cboCampo.SelectedItem.ToString() == "Numero")
            {
                //if(string.IsNullOrEmpty(txtFiltro2.Text))
                //{
                //    MessageBox.Show("Por favor ingrese algo para buscar");
                //    return true;
                //}

                if (!soloNumeros(txtFiltro2.Text))
                {
                    MessageBox.Show("Solo numeros por favor");
                    return true;
                }
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (var caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

    }
}
