namespace dotnetapp.Models
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int MovieID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
