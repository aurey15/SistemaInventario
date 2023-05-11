using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        //Metodos Asincronos Task<>
        Task<T> Obtener(int id);
        //Funcionara Para El ID
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T, bool>> filtro = null,    //Filtro
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,//orenar la lista
            string incluirPropiedades = null,//se encarga de hacer los enlaces con otros objetos(Realcionados)
            bool isTracking = true  //Cuando queramos accedera un objeto o lista y al mismo tiempo la queramos modificar
            );

     /*   PagedList<T> ObtenerTodosPaginado(Parametros parametros, Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string incluirPropiedades = null,
            bool isTracking = true);*/

     //Nos devuelve un solo registro del objeto
        Task<T> ObtenerPrimero(
            Expression<Func<T, bool>> filtro = null, 
            string incluirPropiedades = null,
            bool isTracking = true
            );

        Task Agregar(T entidad);

        void Remover(T entidad);

        void RemoverRango(IEnumerable<T> entidad);

    }
}
