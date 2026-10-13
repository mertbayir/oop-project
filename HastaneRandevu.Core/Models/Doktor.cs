namespace HastaneRandevu.Core.Models;

// Calisan'dan turer. Ek ozelligi: uzmanlik alani (odev: "her doktorun uzmanlik alani tanimlanabilecek").
public class Doktor : Calisan
{
    private string _uzmanlikAlani = "";

    public string UzmanlikAlani
    {
        get => _uzmanlikAlani;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Uzmanlik alani bos olamaz.", nameof(UzmanlikAlani));
            _uzmanlikAlani = value.Trim();
        }
    }

    public Doktor(int id, string ad, string soyad, string email, string sifre, Bolum bolum, string uzmanlikAlani)
        : base(id, ad, soyad, email, sifre, bolum)
    {
        UzmanlikAlani = uzmanlikAlani;
    }

    // Calisan'in birakip biraktigi abstract uyeyi burada dolduruyoruz.
    public override string RolAdi => "Doktor";

    public override string ToString() => $"Dr. {TamAd} - {UzmanlikAlani} ({Bolum.Ad})";
}
