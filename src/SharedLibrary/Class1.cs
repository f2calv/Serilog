using System;
namespace CasCap
{
    public class TestObj
    {
        public int id { get; set; } = DateTime.UtcNow.Millisecond;
        public DateTime utcNow { get; set; } = DateTime.UtcNow;
        public string strDate { get; set; } = DateTime.UtcNow.ToString();
        public Guid uniqueId { get; set; } = Guid.NewGuid();
    }
}
