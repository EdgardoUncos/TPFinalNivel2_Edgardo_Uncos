﻿using System;
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
    public partial class FrmAlta : Form
    {
        public FrmAlta()
        {
            InitializeComponent();
        }

        private void FrmAlta_Load(object sender, EventArgs e)
        {
            TipoNegocio marcas = new TipoNegocio();
            dgv1.DataSource = marcas.listarMarcas();
            dgv2.DataSource = marcas.listarCategoria();
            
        }
    }
}
