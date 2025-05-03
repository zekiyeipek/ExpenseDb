# ExpenseDb

**Masraf Yönetim Sistemi API**  
Patika.dev & Papara Kadın Yazılımcı Bootcamp bitirme projesi olarak geliştirilmiştir.

## Proje Özeti

Bu API, bir şirkette sahada çalışan personelin masraf taleplerini girmesi ve yöneticilerin bu talepleri yönetmesi amacıyla geliştirilmiştir. Personel, anında fiş/fatura yükleyebilir; yöneticiler talepleri onaylayabilir veya reddedebilir. Onaylanan talepler için ödeme simülasyonu da bulunmaktadır.

## Kullanıcı Rolleri

- **Admin (Yönetici)**

  - Tüm masrafları görüntüleyebilir
  - Talepleri onaylayabilir / reddedebilir
  - Kategori tanımları yapabilir
  - Raporlama işlemleri yapabilir

- **Personel (Saha Çalışanı)**
  - Kendi adına masraf girişi yapabilir
  - Taleplerinin durumunu takip edebilir
  - Red edilen masraflar için açıklamaları görebilir

## Teknolojiler ve Araçlar

- ASP.NET Core Web API
- Entity Framework Core (Code First)
- PostgreSQL
- JWT ile Kimlik Doğrulama
- Dapper (Raporlama için)
- Swagger (API Dokümantasyonu)
- Role-Based Authorization
- CSV Export
- IFormFile ile belge yükleme
- Initial migration ile kullanıcı seedleme

## API Özellikleri

- ✅ Kayıt ve Giriş (JWT Token)
- ✅ Masraf oluşturma, güncelleme, silme
- ✅ Onaylama ve red etme mekanizması
- ✅ Raporlama (günlük, haftalık, aylık, kullanıcı ve kategori bazlı)
- ✅ CSV olarak veri dışa aktarma
- ✅ Belge (fiş/fatura) yükleme desteği
- ✅ Dashboard verileri

## Kurulum

1. Repository'yi klonla:

   ```bash
   git clone https://github.com/zekiyeipek/ExpenseDb.git
   cd ExpenseDb.API
   ```
