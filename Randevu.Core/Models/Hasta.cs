using Randevu.Core.Enums;

namespace Randevu.Core.Models;

// KALITIM: Hasta bir Kullanici'dir ("is-a"). Id, Ad, Soyad, Email ve sifre islerini
// Kullanici'dan miras alir, burada tekrar yazilmaz.
public class Hasta : Kullanici
{
    private string _telefon = "";

    // Sadece hastaya ozel alan: Kullanici'da yok, Hasta'nin eki.
    public string Telefon
    {
        get => _telefon;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Telefon bos olamaz.", nameof(Telefon));
            _telefon = value.Trim();
        }
    }

    // ": base(...)" ust sinifin (Kullanici) constructor'ini calistirir.
    public Hasta(int id, string ad, string soyad, string email, string sifre, string telefon)
        : base(id, ad, soyad, email, sifre)
    {
        Telefon = telefon;
    }

    // COK BICIMLILIK: Kullanici'daki abstract uyeleri Hasta'ya ozel doldurur (override).
    public override string RolAdi => "Hasta";

    public override bool YetkiVarMi(Yetki yetki)
        => yetki == Yetki.RandevuAl || yetki == Yetki.RandevuIptalEt;
}
