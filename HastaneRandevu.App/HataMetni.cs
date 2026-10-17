namespace HastaneRandevu.App;

// Servislerden gelen hata mesajlarini ekranda gosterilecek hale getirir.
public static class HataMetni
{
    public static string Al(Exception hata)
    {
        // ArgumentException mesajinin sonuna " (Parameter 'Ad')" gibi teknik bir ek yapar; ekranda gostermeyiz.
        int ek = hata.Message.IndexOf(" (Parameter", StringComparison.Ordinal);
        return ek >= 0 ? hata.Message[..ek] : hata.Message;
    }
}
