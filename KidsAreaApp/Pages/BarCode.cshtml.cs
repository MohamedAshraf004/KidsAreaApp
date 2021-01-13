using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Net.ConnectCode.Barcode;

namespace KidsAreaApp.Pages
{
    public class BarCodeModel : PageModel
    {
        [ViewData]
        public string Barcode { get; set; }

        [ViewData]
        public string BarcodeText { get; set; }

        public void OnGet()
        {
            BarcodeFonts bec = new BarcodeFonts();
            bec.BarcodeType = BarcodeFonts.BarcodeEnum.Code128Auto;
            bec.Data = "12345678";
            bec.encode();
            Barcode = bec.EncodedData;
            BarcodeText = bec.HumanText;
        }
    }
}
