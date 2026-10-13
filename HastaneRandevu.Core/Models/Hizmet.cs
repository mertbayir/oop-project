namespace HastaneRandevu.Core.Models;

// Bir bolumun sundugu saglik hizmeti (genel muayene, rontgen vb.).
// Odev: "her hizmetin suresi ve ucreti detayli sekilde tanimlanabilecek".
public class Hizmet
{
    private string _ad = "";
    private TimeSpan _sure;
    private decimal _ucret;

    public int Id { get; }

    public string Ad
    {
        get => _ad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Hizmet adi bos olamaz.", nameof(Ad));
            _ad = value.Trim();
        }
    }

    // TimeSpan: "30 dakika" gibi bir sureyi tutar; tarih/saatle dogrudan toplanabilir.
    public TimeSpan Sure
    {
        get => _sure;
        set
        {
            if (value <= TimeSpan.Zero)
                throw new ArgumentException("Sure sifirdan buyuk olmali.", nameof(Sure));
            _sure = value;
        }
    }

    // decimal: para icin double kullanilmaz (ondalik yuvarlama hatasi yapar).
    public decimal Ucret
    {
        get => _ucret;
        set
        {
            if (value < 0)
                throw new ArgumentException("Ucret negatif olamaz.", nameof(Ucret));
            _ucret = value;
        }
    }

    public Hizmet(int id, string ad, TimeSpan sure, decimal ucret)
    {
        Id = id;
        Ad = ad;
        Sure = sure;
        Ucret = ucret;
    }

    public override string ToString() => $"{Ad} ({Sure.TotalMinutes:0} dk, {Ucret:0.00} TL)";
}
