using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;


namespace negocio
{
    public class ArticuloNegocio
    {
        
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "select A.Id, A.Codigo, Nombre, A.Descripcion, IdMarca, M.Descripcion Marca, IdCategoria, C.Descripcion AS Categoria, ImagenUrl, Precio from ARTICULOS A, MARCAS M, CATEGORIAS C Where A.IdMarca = M.Id and A.IdCategoria = C.Id";
                comando.Connection = conexion;
                conexion.Open();
                lector = comando.ExecuteReader();

                while(lector.Read())
                {
                    Articulo aux = new Articulo();
                    if(!(lector["Id"] is DBNull))
                        aux.Id =(int)lector["Id"];
                    if (!(lector["Codigo"] is DBNull))
                        aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    aux.Marca = new Tipo();
                    aux.Marca.Id = (int)lector["IdMarca"];
                    aux.Marca.Descripcion = (string)lector["Marca"];

                    aux.Categoria = new Tipo();
                    aux.Categoria.Id = (int)lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)lector["Categoria"];

                    if(!(lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)lector["ImagenUrl"];

                    if(!(lector["Precio"] is DBNull))
                        aux.Precio = (decimal)lector["Precio"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
               
                conexion.Close();
            }

            
        }

        // Listar2 es igual a listar, solo que hace uso de la clase AccesoDatos.
        public List<Articulo> listar2()
        {
            List<Articulo> lista = new List<Articulo>();

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("select A.Id, A.Codigo, Nombre, A.Descripcion, IdMarca, M.Descripcion Marca, IdCategoria, C.Descripcion AS Categoria, ImagenUrl, Precio from ARTICULOS A, MARCAS M, CATEGORIAS C Where A.IdMarca = M.Id and A.IdCategoria = C.Id");
                datos.ejecutarLectura();

                while(datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    if(!(datos.Lector["Codigo"] is DBNull))
                        aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Marca = new Tipo();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    aux.Categoria = new Tipo();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    if(!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);

                }
                datos.cerrarConexion();
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }

        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("insert into ARTICULOS (Nombre, Descripcion, Precio, IdMarca, IdCategoria) values ('"+nuevo.Nombre+"','"+nuevo.Descripcion+"', "+nuevo.Precio+" , @idMarca, @idCategoria )");
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("@idCategoria", nuevo.Categoria.Id);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo seleccionado)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, ImagenUrl = @imagenUrl, Precio = @precio where Id = @id");
                datos.setearParametro("@codigo", seleccionado.Codigo);
                datos.setearParametro("@nombre", seleccionado.Nombre);
                datos.setearParametro("@descripcion", seleccionado.Descripcion);
                datos.setearParametro("@idMarca", seleccionado.Marca.Id);
                datos.setearParametro("idCategoria", seleccionado.Categoria.Id);
                datos.setearParametro("imagenUrl", seleccionado.UrlImagen);
                datos.setearParametro("@precio", seleccionado.Precio);
                datos.setearParametro("@id",seleccionado.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("delete From ARTICULOS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Articulo>FiltroAvanzadoSql(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            string consulta = "select A.Id, A.Codigo, Nombre, A.Descripcion, IdMarca, M.Descripcion Marca, IdCategoria, C.Descripcion AS Categoria, ImagenUrl, Precio from ARTICULOS A, MARCAS M, CATEGORIAS C Where A.IdMarca = M.Id and A.IdCategoria = C.Id And ";

            try
            {
                if(campo == "Codigo")
                {
                    switch(criterio)
                    {
                        case "Comienza con": 
                            consulta += "Codigo Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Codigo Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Codigo Like '%" + filtro + "%' ";
                            break;

                    }
                }else if(campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Nombre Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Nombre Like '%" + filtro + "%' ";
                            break;

                    }

                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Descripcion Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Descripcion Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Descripcion Like '%" + filtro + "%' ";
                            break;

                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Codigo"] is DBNull))
                        aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Marca = new Tipo();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    aux.Categoria = new Tipo();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);

                }
                datos.cerrarConexion();


                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public List<Articulo> FiltroAvanzadoSql(string campo, string criterio, string filtro, string marca, string categoria, string precio, string filtroPrecio)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            string consulta = "select A.Id, A.Codigo, Nombre, A.Descripcion, IdMarca, M.Descripcion Marca, IdCategoria, C.Descripcion AS Categoria, ImagenUrl, Precio from ARTICULOS A, MARCAS M, CATEGORIAS C Where A.IdMarca = M.Id and A.IdCategoria = C.Id And ";

            try
            {
                if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Codigo Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Codigo Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Codigo Like '%" + filtro + "%' ";
                            break;

                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Nombre Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Nombre Like '%" + filtro + "%' ";
                            break;

                    }

                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Descripcion Like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Descripcion Like '%" + filtro + "' ";
                            break;
                        default:
                            consulta += "Descripcion Like '%" + filtro + "%' ";
                            break;

                    }
                }
                // parte extra
                if(marca != null)
                    consulta += " And M.Descripcion = '" + marca + "' ";

                if (categoria != null)
                    consulta += " And C.Descripcion = '" + categoria + "' ";

                if(!string.IsNullOrEmpty(precio))
                {
                    switch(precio)
                    {
                        case "Mayor":
                            consulta += " And Precio > " + filtroPrecio;
                            break;
                        case "Menor":
                            consulta += " And Precio < " + filtroPrecio;
                            break;

                        default:
                            consulta += " And Precio = " + filtroPrecio;
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Codigo"] is DBNull))
                        aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Marca = new Tipo();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                    aux.Categoria = new Tipo();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);

                }
                datos.cerrarConexion();


                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



    }
}
