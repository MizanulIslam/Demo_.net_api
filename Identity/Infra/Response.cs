namespace Demo_Elmah.Identity.Infra
{
    public class Response<T>
    {
        public Response()
        {

        }
        /// <summary>
        /// Commpleted
        /// </summary>
        /// <param name="resultdata"></param>
        public Response(T resultdata)
        {
            statuscode = 200;
            errormsg = "";
            data = resultdata;

        }
        /// <summary>
        /// Failed Request
        /// </summary>
        /// <param name="status"></param>
        /// <param name="erMsg"></param>
        /// <param name="resultdata"></param>
        public Response(int status, string erMsg, T resultdata)
        {
            statuscode = status;
            errormsg = erMsg;
            data = resultdata;

        }


        public T data { get; set; }
        public string errormsg { get; set; }
        public int statuscode { get; set; }
        // public bool Succeeded { get; set; }
    }
}
