namespace Necta.API
{
    /*
     *  Exact Keys returened by the API should be defined here.
     *  Not case sensitive but the Key should be exactly the same word.
     *  eg. API returns "printerName" -> we can define it here with "PrinterName"
     *  this is only valid because we deserialize the object with "caseInsesitive" option
     */

    public class Receipt
    {
        public string ID { get; set; } //receipt ID

        public string Device { get; set; } 

        public string Printer { get; set; } //Printer ID

        public string HTML { get; set; }

        public string PrinterName { get; set; }
    }
}
