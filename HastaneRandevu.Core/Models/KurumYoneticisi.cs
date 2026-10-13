using HastaneRandevu.Core.Enums;

namespace HastaneRandevu.Core.Models;

// Kendi kurumunu yapilandiran yonetici (odev: "Hastane / Klinik Yoneticisi").
public class KurumYoneticisi : Kullanici
{
    public Kurum Kurum { get; }

    public KurumYoneticisi(int id, string ad, string soyad, string email, string sifre, Kurum kurum)
        : base(id, ad, soyad, email, sifre)
    {
        ArgumentNullException.ThrowIfNull(kurum);
        Kurum = kurum;
    }

    public override string RolAdi => "Kurum Yoneticisi";

    public override bool YetkiVarMi(Yetki yetki)
        => yetki == Yetki.CalisanYonet || yetki == Yetki.HizmetYonet;
}
