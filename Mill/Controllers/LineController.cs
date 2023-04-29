using AirtableApiClient;
using Microsoft.AspNetCore.Mvc;
using Mill.Models;
using System.Reflection.Emit;

namespace Mill.Controllers
{
    public class LineController : Controller
    {
        List<AirtableRecord> records = new List<AirtableRecord>();
        public IEnumerable<string> fields = new string[] { "Order #", "Part # +", "QTY", "NAME", "DATE", "Run Number (from Master List)", "MATERIAL/COLOUR", "REASON FOR REORDER", "Mill Routing Code", "COMMENT", "WC #", "Optimized", "Optimized Date", "Rush", "Cut @ Router Time" };


        private readonly IConfiguration _configuration;

        public LineController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async void GetAirtableData()
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
                            records.AddRange(response.Records.ToList());
                            offset = response.Offset;
                        }
                        else if (response.AirtableApiError is AirtableApiException)
                        {
                            errorMessage = response.AirtableApiError.ErrorMessage;
                            if (response.AirtableApiError is AirtableInvalidRequestException)
                            {
                                errorMessage += "\nDetailed error message: ";
                                errorMessage += response.AirtableApiError.DetailedErrorMessage;
                               // MessageBox.Show(errorMessage);
                            }
                            break;
                        }
                        else
                        {
                            errorMessage = "Unknown error";
                            //MessageBox.Show(errorMessage);
                            break;
                        }
                    } while (offset != null);
                }

            }
            catch (Exception ex)
            {
               // label4.Text = ex.Message;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
              //  MessageBox.Show(errorMessage);
            }

            else
            {
              // updateTable();
               // label7.Visible = false;

            }

        }

        public IActionResult Line1()
        {
            return View();
        }

    }
}
