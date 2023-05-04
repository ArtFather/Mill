using AirtableApiClient;
using Microsoft.AspNetCore.Mvc;
using Mill.Models;
using Newtonsoft.Json;
using System.Data;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Mill.Controllers
{
    public class LineController : Controller
    {
        List<Records> recordsList = new List<Records>();
        List<AirtableRecord> reecords = new List<AirtableRecord>();
        DataTable dt = new DataTable();
        public IEnumerable<string> fields = new string[] { "Order #", "Part # +", "QTY", "NAME", "DATE", "Run Number (from Master List)", "MATERIAL/COLOUR", "REASON FOR REORDER", "Mill Routing Code", "COMMENT", "WC #", "Optimized", "Optimized Date", "Rush", "Cut @ Router Time" };
        private readonly IConfiguration _configuration;

        public LineController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Line1()
        {
            ViewBag.Columns = fields;
            return View();
        }

        public async Task<JsonResult> GetDataLine1()
        {
            string appKey = _configuration.GetSection("AirtableApiLogin").GetValue<string>("APPKEY");
            string baseId = _configuration.GetSection("AirtableApiLogin").GetValue<string>("BASEID");
            string table = _configuration.GetSection("AirtableApiLogin").GetValue<string>("TABLE");
            string tableview = "Master List";
            string offset = null;
            string errorMessage = null;
            string timeZone = "America/North_Dakota/Center";
            string cellFormat = "string";
            string userLocale = "en-ca";
            string filterByFormulaLine1 = "FIND('1', {Mill Routing Code}) > 0";

            try
            {
                using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
                {
                    do
                    {
                        Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                        table,
                               offset,
                               fields,
                               filterByFormulaLine1,
                               null,
                               null,
                               null,
                               tableview,
                               cellFormat,
                               timeZone,
                               userLocale,
                               false
                           );

                        AirtableListRecordsResponse response = await task;
                        if (response.Success)
                        {
                            reecords.AddRange(response.Records.ToList());
                            offset = response.Offset;
                        }
                        else if (response.AirtableApiError is AirtableApiException)
                        {
                            errorMessage = response.AirtableApiError.ErrorMessage;
                            if (response.AirtableApiError is AirtableInvalidRequestException)
                            {
                                errorMessage += "\nDetailed error message: ";
                                errorMessage += response.AirtableApiError.DetailedErrorMessage;
                                ViewBag.Error = errorMessage;
                            }
                            break;
                        }
                        else
                        {
                            ViewBag.Error = "Unknown error";
                            break;
                        }
                    } while (offset != null);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.Error = errorMessage;
            }

            else
            {
                foreach (string field in fields)
                {
                    dt.Columns.Add(field);
                }

                int i = 0;
                foreach (var record in reecords)
                {

                    DataRow dr = dt.NewRow();
                    
                    foreach (var columns in record.Fields)
                    {
                        dr[columns.Key.ToString()] = columns.Value.ToString();
                    }
                        recordsList.Add(new Records {
                            Id = i++,
                            Order = dr["Order #"].ToString(),
                            Part = dr["Part # +"].ToString(),
                            NAME = dr["NAME"].ToString(),
                            QTY = dr["QTY"].ToString(),
                            DATE = dr["DATE"].ToString(),
                            Run = dr["Run Number (from Master List)"].ToString(),
                            MATERIAL = dr["MATERIAL/COLOUR"].ToString(),
                            REASON = dr["REASON FOR REORDER"].ToString(),
                            RoutingCode = dr["Mill Routing Code"].ToString(),
                            COMMENT = dr["COMMENT"].ToString(),
                            WC = dr["WC #"].ToString(),
                            Optimized = dr["Optimized"].ToString(),
                            OptimizedDate = dr["Optimized Date"].ToString(),
                            Rush = dr["Rush"].ToString(),
                            CutAt = dr["Cut @ Router Time"].ToString(),
                        });;
                }
            }
            return Json(recordsList);
        }
    }
}
