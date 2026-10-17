using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Iki serviste de (RandevuServisi, KurumYonetimServisi) ayni yetki sorusu soruldugu icin tek yerde toplandi.
// COK BICIMLILIK: kullanici Hasta mi Doktor mu bilinmez; kendi YetkiVarMi'si cevap verir.
internal static class Yetkilendirme
{
    public static void Kontrol(Kullanici kullanici, Yetki yetki)
    {
        ArgumentNullException.ThrowIfNull(kullanici);

        if (!kullanici.YetkiVarMi(yetki))
            throw new UnauthorizedAccessException($"{kullanici.RolAdi} rolunun bu islem icin yetkisi yok.");
    }
}
