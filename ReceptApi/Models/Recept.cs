namespace ReceptApi.Models
{
    public class Recept
    {
        public int receptid { get; set; }
        public int userid { get; set; }
        public string titel { get; set; }
        public string beskrivning { get; set; }
        public string ingredienser { get; set; }
        public int kategoriid { get; set; }
    }
}
