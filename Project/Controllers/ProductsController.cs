using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class ProductsController : Controller
    {
        private IStoreContextData Context { get; }
        private int pageSize = 10;
        public ProductsController(IStoreContextData db)
        {
            Context = db;
        }
        public IActionResult Index(int? page)
        {
            //Automapper library
            var res = MapperConfigurator.Mapper.Map<List<ProductViewModel>>(Context.Products.All());

            return View(res.ToPagedList(page ?? 1, pageSize));
        }
        [Authorize(Roles = "Seller, Warehouse")]
        public IActionResult Add(ProductViewModel model)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                var res = MapperConfigurator.Mapper.Map<Product>(model);

                this.Context.Products.Add(res);
                this.Context.SaveChanges();
                if (isAjaxCall)
                {
                    string id = res.Id;
                    model.Id = id;
                    return Json(JsonConvert.SerializeObject(model));
                }
                return View("Index");
            }

            return View(model);
        }
        [HttpPut]
        [Authorize(Roles = "Seller, Warehouse")]
        public IActionResult Edit(ProductViewModel model)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (ModelState.IsValid)
            {
                var res = MapperConfigurator.Mapper.Map<Product>(model);
                this.Context.Products.Update(res);
                int status = this.Context.SaveChanges();
                if (status == 1)
                {
                    if (isAjaxCall)
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(JsonConvert.SerializeObject(model));
                    }
                    return View();
                }
                else
                {
                    if (isAjaxCall)
                    {
                        Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return Json(JsonConvert.SerializeObject("Server error."));
                    }
                    return BadRequest();
                }
            }
            else
            {
                if (isAjaxCall)
                {
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    var errors = JsonConvert.SerializeObject(ModelState.GetErrors());

                    return Content(errors);
                }
                return View(model);
            }

        }
        public IActionResult Delete(string id)
        {
            var item = this.Context.Products.All(x => x.Id == id).SingleOrDefault();
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            this.Context.Products.Delete(item);
            var res = this.Context.SaveChanges();
            if (res == 1)
            {
                if (isAjaxCall)
                {

                    return Json(Response.StatusCode = (int)HttpStatusCode.OK);
                }
                return View();
            }
            else
            {
                if (isAjaxCall)
                {
                    return Json(Response.StatusCode = (int)HttpStatusCode.BadRequest);
                }
            }
            return StatusCode(400);
        }
        public IActionResult GetProductInfo(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (Context)
                {
                    var product = Context.Products.All(x => x.Id == id).SingleOrDefault();
                    var res = MapperConfigurator.Mapper.Map<ProductViewModel>(product);
                    return View(res);
                }
            }
            return Redirect("/Products/Index");
        }
        public IActionResult CreateOrder(OrderViewModel model)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            SaleOrder so = new SaleOrder()
            {

                DateOfSale = DateTime.Now,
                Finished = true,
                InternetOrdered = true,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Total = model.Quantity * model.Price
            };
            using (Context)
            {

                try
                {
                    var customer = this.Context.Customers.All(x => x.Id == user).SingleOrDefault();
                    so.Customer = customer;
                    this.Context.SaleOrders.Add(so);

                    var res = Context.SaveChanges();
                    if (res == 1)
                    {
                        return Redirect("/Products/Index");
                    }
                    return Redirect("/Home/Error");
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                }
            }
            return View("/Home");
        }
    }
}
