namespace Randevu.Core.Models;

// Doktor disindaki saglik calisani (hemsire, teknisyen...). Ek ozelligi: unvan.
public class SaglikPersoneli : Calisan
{
    private string _unvan = "";

    public string Unvan
    {
        get => _unvan;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Unvan bos olamaz.", nameof(Unvan));
            _unvan = value.Trim();
        }
    }

    public SaglikPersoneli(int id, string ad, string soyad, string email, string sifre, Bolum bolum, string unvan)
        : base(id, ad, soyad, email, sifre, bolum)
    {
        Unvan = unvan;
    }

    public override string RolAdi => "Saglik Personeli";

    public override string ToString() => $"{Unvan} {TamAd} ({Bolum.Ad})";
}
