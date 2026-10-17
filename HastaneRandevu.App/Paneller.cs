using HastaneRandevu.Core.Models;

namespace HastaneRandevu.App;

// Yonetici ve sistem yoneticisi icin ekran henuz yok. (HastaPaneli ve CalisanPaneli kendi dosyalarinda.)
public class BilgiPaneli : PanelFormu
{
    public BilgiPaneli(Kullanici kullanici, UygulamaVerisi veri)
        : base(kullanici, veri, "Bilgi")
    {
        Controls.Add(new Label
        {
            Text = "Bu rol için ekran henüz yok. İşlemleri konsol demosunda (HastaneRandevu.Demo) görebilirsiniz.",
            Left = 15, Top = 60, Width = 700
        });
    }
}
