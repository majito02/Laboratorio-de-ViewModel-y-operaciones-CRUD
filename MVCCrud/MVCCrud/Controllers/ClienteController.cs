using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MVCCrud.Models;
using MVCCrud.Models.ViewModels;

namespace MVCCrud.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index()
        {
            List<ListClienteViewModel> listaClientes;
            using (FacturaEntities db = new FacturaEntities())
            {
                listaClientes = (from c in db.cliente select new ListClienteViewModel 
                { 
                    id_cli = c.id_cli,
                    nombre_cli = c.nombre_cli,
                    cedula_cli = c.cedula_cli,
                    correo_cli = c.correo_cli
                }).ToList();
            }
                return View(listaClientes);
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ClienteViewModel clienteModel)
        {
            try 
            {
                //Validar el Modelo
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase FileBase = Request.Files[0];

                    WebImage image = new WebImage(FileBase.InputStream);

                    clienteModel.foto = image.GetBytes();

                    //Conexion a la base de datos y paso de datos del modelo a un objeto tipo cliente
                    using (FacturaEntities db = new FacturaEntities())
                    {
                        var oCliente = new cliente();
                        oCliente.nombre_cli = clienteModel.nombre_cli;
                        oCliente.cedula_cli = clienteModel.cedula_cli;
                        oCliente.correo_cli = clienteModel.correo_cli;
                        oCliente.fechaNacimiento = clienteModel.fechaNacimiento_cli;
                        oCliente.foto = clienteModel.foto;

                        //Almacenar en la base de datos el objeto cliente
                        db.cliente.Add(oCliente);
                        db.SaveChanges();
                    }
                    return Redirect("~/Cliente");
                }
                return View(clienteModel);
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Editar(int id)
        {
            ClienteViewModel model = new ClienteViewModel();

            using (FacturaEntities db = new FacturaEntities())
            {
                var oCliente = db.cliente.Find(id);
                model.nombre_cli = oCliente.nombre_cli;
                model.cedula_cli = oCliente.cedula_cli;
                model.correo_cli = oCliente.correo_cli;
                model.fechaNacimiento_cli = (DateTime)oCliente.fechaNacimiento;
                model.id_cli = oCliente.id_cli;
                

            }
            return View(model);


        }
        [HttpPost]
        public ActionResult Editar(ClienteViewModel clienteModel)
        {
            try
            {
                //Validar el formulario
                if (ModelState.IsValid)
                {
                    using (FacturaEntities db = new FacturaEntities())
                    {
                        var oCliente = db.cliente.Find(clienteModel.id_cli) ;
                        oCliente.nombre_cli = clienteModel.nombre_cli;
                        oCliente.cedula_cli = clienteModel.cedula_cli;
                        oCliente.correo_cli = clienteModel.correo_cli;
                        oCliente.fechaNacimiento = (DateTime) clienteModel.fechaNacimiento_cli;

                        db.Entry(oCliente).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Redirect("~/Cliente/");
                }
                return View(clienteModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public ActionResult Eliminar(int id)
        {
            using(FacturaEntities db = new FacturaEntities())
            {
                var oCliente = db.cliente.Find(id);
                db.cliente.Remove(oCliente);
                db.SaveChanges();
            }
            return Redirect("~/Cliente");
        }

        public ActionResult getImage(int id)
        {
            FacturaEntities db = new FacturaEntities();
            cliente model = db.cliente.Find(id);
            byte[] byteImage = model.foto;
            MemoryStream memoryStream = new MemoryStream(byteImage);
            Image image = Image.FromStream(memoryStream);
            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return File(memoryStream, "image/jpg");
        }
    }
}
