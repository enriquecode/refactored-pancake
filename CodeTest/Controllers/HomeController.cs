using System;
using System.Linq;
using System.Web.Mvc;
using Quote.Contracts;
using Quote.Models;
using System.Net;
using System.Net.Http;
using System.Text;
//using System.Json;
using System.Threading.Tasks;

namespace PruebaIngreso.Controllers
{
    public class HomeController : Controller
    {
        private const string URL = "https://refactored-pancake.free.beeceptor.com/margin/";

        private readonly IQuoteEngine quote;

        public HomeController(IQuoteEngine quote)
        {
            this.quote = quote;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            var request = new TourQuoteRequest
            {
                adults = 1,
                ArrivalDate = DateTime.Now.AddDays(1),
                DepartingDate = DateTime.Now.AddDays(2),
                getAllRates = true,
                GetQuotes = true,
                //GetQuotes = false, //no es necesario pasarlo a falso
                RetrieveOptions = new TourQuoteRequestOptions
                {
                    GetContracts = true,
                    //GetCalculatedQuote = true,
                    //esta linea si fue necesario pasarlo a falso,
                    //para que que no calcule las quotas
                    GetCalculatedQuote = false,
                },
                TourCode = "E-U10-PRVPARKTRF",
                Language = Language.Spanish
                ,selectedServices = "102955431"
                //con la linea de arriba se intenta
                //llenar las quotas de este servicio
                //para que no falle la instrucción de LINQ
                //del otro proyecto que intenta recuperar
                //el primer elemento de una lista de quotas
                //pero como no hay elementos, falla la
                //instrucción .FIRST(), y se tuvo que cambiar
                //por FIRTSDEFAUKT()

            };

            var result = this.quote.Quote(request);
            var tour = result.Tours.FirstOrDefault();
            ViewBag.Message = "Test 1 Correcto";
            return View(tour);
        }

        public ActionResult Test2()
        {
            ViewBag.Message = "Test 2 Correcto";
            return View();
        }

        public async Task<ActionResult> Test3()
        {
            //Esta primera aproximación si funcionaba, solo habría que agregarle la linea:
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //pero mejor se separa las llamadas a la API, de manera generica en una clase
            //para que cualquiera lo pueda usar

            //System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            //client.BaseAddress = new System.Uri(URL);
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //System.Net.Http.HttpContent content = new StringContent(DATA, UTF8Encoding.UTF8, "application/json");
            //HttpResponseMessage message = client.PostAsync(URL, content).Result;
            //string description = string.Empty;
            //if (message.IsSuccessStatusCode)
            //{
            //    string result = message.Content.ReadAsStringAsync().Result;
            //    description = result;
            //}

            var callApi = new callAPI(URL);
            var client = callApi.getClient();
            string action = "E-U10-PRVPARKTRF";
            //action = "E-U10-UNILATIN";
            //action = "E-U10-DSCVCOVE";
            //action = "E-E10-PF2SHOW";
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpResponseMessage response = await client.GetAsync(action);
                if (response.IsSuccessStatusCode)
                {
                    //return Ok(await response.Content.ReadAsStringAsync());
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string margin = await response.Content.ReadAsStringAsync();
                        ViewBag.Message = margin;
                    }
                    else
                    {
                        ViewBag.Message = "\"margin: 0.0\"";
                        //ViewBag.Message = "{ margin: 0.0 }";
                    }

                }
                else
                {
                    ViewBag.Message = "\"margin: 0.0\"";
                }
            }
            catch (Exception ex)
            {

            }

            return View();

        }

        public ActionResult Test4()
        {
            var request = new TourQuoteRequest
            {
                adults = 1,
                ArrivalDate = DateTime.Now.AddDays(1),
                DepartingDate = DateTime.Now.AddDays(2),
                getAllRates = true,
                GetQuotes = true,
                RetrieveOptions = new TourQuoteRequestOptions
                {
                    GetContracts = true,
                    GetCalculatedQuote = true,
                },
                Language = Language.Spanish
            };

            var result = this.quote.Quote(request);
            return View(result.TourQuotes);
        }
    }
}