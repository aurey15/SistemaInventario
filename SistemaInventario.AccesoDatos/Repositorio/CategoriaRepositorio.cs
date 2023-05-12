using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {

        private readonly ApplicationDbContext _db;
        //Le pasamos el dbcontext  al padre
        public CategoriaRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(Categoria categoria) //Actualiza el registron con el que estamos trabajando
        {
            //Capturamos el registro en base al ID antes de actualizarlo
            var categoriaBD = _db.Categorias.FirstOrDefault(b => b.Id == categoria.Id);
            if (categoriaBD != null)//Verificamos si encontro el registro y si, si lo actualizamos
            {
                categoriaBD.Nombre = categoria.Nombre;
                categoriaBD.Descripcion = categoria.Descripcion;
                categoriaBD.Estado = categoria.Estado;
                _db.SaveChanges();
            }
        }
    }
}