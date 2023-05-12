using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using System.Data;

namespace SistemaInventario.Areas.Admin1.Controllers
{
    [Area("Admin1")]
    public class BodegaController : Controller
    {

        //referenciameos nuestra unidad de trabajo, para poderlo utilizar lo tenemos que instanciar
        private readonly IUnidadTrabajo _unidadTrabajo;

        //Lo inicializamos en el constructor
        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo= unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Bodega bodega = new Bodega();

            if (id == null)
            {
                // Crear una nueva Bodega
                bodega.Estado = true;
                return View(bodega);
            }
            // Actualizamos Bodega
            //La funcion Obtener es envase a su id
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]//Esto sirve para las falsificaciones de solicitudes de un sitio cargado normalmente de otra pagina que puede intentar cargar datos en nuestra pagina.

        //Desde le vista le vamos a pasar todo el modelo bodega, va recibir una variable de tipo Bodega bodega
        public async Task<IActionResult> Upsert(Bodega bodega)
        {
            //Esto valida que el modelo qeu estoy recibiendo sea valido, es decir que todo este correcto dentro de cada una de sus propiedades
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)//Si el Id es igual a cero significa que es un nuevo registro
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);//Simplemente llamamos a la unidad de trabajo bodega y agregar(bodega )
                    TempData[DS.Exitosa] = "Bodega Creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                   TempData[DS.Exitosa] = "Bodega Actualizada Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));//redireccionamos a otra vista
            }
            TempData[DS.Error] = "Error al Grabar Bodega";
            return View(bodega);
        }

        

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Bodega.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Bodega" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();//Asignamos todas las bodegas  a una variable
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
