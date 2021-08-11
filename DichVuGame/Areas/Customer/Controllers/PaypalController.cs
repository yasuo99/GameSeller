using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaypalController : Controller
    {
        private Payment payment;
        private readonly ApplicationDbContext _db;
        public PaypalController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [ActionName("TopupWithPaypal")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Charged(int topupAmount)
        {
            APIContext apiContext = Configuration.GetAPIContext();
            var user = _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            try
            {                
                string payerId = Request.Query["PayerID"].ToString();

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURL = GetRawUrl(Request) + "?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createPayment = this.CreatePayment(apiContext, topupAmount, baseURL + "guid=" + guid,user);
                    var links = createPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }
                    HttpContext.Session.SetString(guid, createPayment.id);
                    HttpContext.Session.SetString("topupAmount", topupAmount.ToString());
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Query["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, HttpContext.Session.GetString(guid));
                    var getTopupAmount = Int32.Parse(HttpContext.Session.GetString("topupAmount"));
                    TopupHistory topupHistory = new TopupHistory()
                    {
                        ApplicationUserID = user.Id,
                        TopupDate = DateTime.Now,
                        TopupAmount = getTopupAmount
                    };
                    _db.TopupHistories.Add(topupHistory);
                    user.Balance += Int32.Parse(HttpContext.Session.GetString("topupAmount"));
                    _db.ApplicationUsers.Update(user);
                    await _db.SaveChangesAsync();
                    
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }                 
                }
            }
            catch (Exception)
            {
                var getInvoiceId = Int32.Parse(HttpContext.Session.GetString("invoiceId"));
                var topupHistory = _db.TopupHistories.Where(u => u.ID == getInvoiceId).FirstOrDefault();
                _db.TopupHistories.Remove(topupHistory);
                await _db.SaveChangesAsync();
                return View("FailureView");
            }
            return View("SuccessfulView");
        }
        public static string GetRawUrl(HttpRequest request)
        {
            var httpContext = request.HttpContext;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
        }
        private Payment CreatePayment(APIContext apiContext, int topupamount, string redirectUrls, ApplicationUser user)
        {
            var topupToUSD = Math.Round((double)topupamount / 22000, MidpointRounding.AwayFromZero);
            var payer = new Payer() { payment_method = "paypal" };
            ItemList itemList = new ItemList() { items = new List<Item>() };
            itemList.items.Add(new Item
            {
                name = "Topup for Bcoin",
                currency = "USD",
                price = topupToUSD.ToString(),
                quantity = "1",
            });
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = topupToUSD.ToString()
            };
            var amount = new Amount()
            {
                currency = "USD",
                total = topupToUSD.ToString(),
                details = details
            };
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Thank you.",
                invoice_number = DateTime.Now.ToString()+user.Id,
                amount = amount,
                item_list = itemList
            });
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrls,
                return_url = redirectUrls
            };
            this.payment = new Payment()
            {
                intent = "SALE",
                transactions = transactionList,
                payer = payer,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
    }
}