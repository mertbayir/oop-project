using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

public static class PanelSecici
{
    // COK BICIMLILIK: rolun adina ("if rol == Hasta") bakmiyoruz; kullanicinin YETKISINE bakiyoruz.
    // Yarin yeni bir rol eklenirse ve RandevuAl yetkisi verilirse, bu kod degismeden ona da hasta paneli acar.
    public static Form Sec(Kullanici kullanici, UygulamaVerisi veri)
    {
        if (kullanici.YetkiVarMi(Yetki.RandevuAl))
            return new HastaPaneli(kullanici, veri);

        if (kullanici.YetkiVarMi(Yetki.RandevuOnayla))
            return new CalisanPaneli(kullanici, veri);

        return new BilgiPaneli(kullanici, veri);
    }
}
