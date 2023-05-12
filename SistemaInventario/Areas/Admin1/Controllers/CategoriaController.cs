using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Data;

namespace SistemaInventario.Areas.Admin1.Controllers
{
    [Area("Admin1")]
    public class CategoriaController : Controller
    {

        //referenciameos nuestra unidad de trabajo, para poderlo utilizar lo tenemos que instanciar
        private readonly IUnidadTrabajo _unidadTrabajo;

        //Lo inicializamos en el constructor
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if (id == null)
            {
                // Crear una nueva Categoria
                categoria.Estado = true;
                return View(categoria);
            }
            // Actualizamos Categoria
            //La funcion Obtener es envase a su id
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]//Esto sirve para las falsificaciones de solicitudes de un sitio cargado normalmente de otra pagina que puede intentar cargar datos en nuestra pagina.

        //Desde le vista le vamos a pasar todo el modelo Categoria, va recibir una variable de tipo Bodega bodega
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            //Esto valida que el modelo qeu estoy recibiendo sea valido, es decir que todo este correcto dentro de cada una de sus propiedades
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)//Si el Id es igual a cero significa que es un nuevo registro
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);//Simplemente llamamos a la unidad de trabajo Categoria y agregar(categoria )
                    TempData[DS.Exitosa] = "Categoria Creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                   TempData[DS.Exitosa] = "Categoria Actualizada Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));//redireccionamos a otra vista
            }
            TempData[DS.Error] = "Error al Grabar Categoria";
            return View(categoria);
        }

        

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDb = await _unidadTrabajo.Categoria.Obtener(id);
            if (categoriaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoria" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();//Asignamos todas las Categoria  a una variable
            if (id == 0)
            {
                //Valor me captura si existe una bodega con el mismo nombre. Any Recorremos toda la lista de bodegas, convertimos a minuscula para poder hacer la comparacion y le quitamos los espacios
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });

        }

        #endregion
    }
}
