# ExpenseDb

**Masraf Yönetim Sistemi API**  
Patika.dev & Papara Kadın Yazılımcı Bootcamp bitirme projesi olarak geliştirilmiştir.

## Proje Özeti

Bu API, bir şirkette sahada çalışan personelin masraf taleplerini girmesi ve yöneticilerin bu talepleri yönetmesi amacıyla geliştirilmiştir.

- Personel, anında fiş/fatura yükleyebilir.
- Yöneticiler talepleri onaylayabilir veya reddedebilir.
- Onaylanan talepler için ödeme simülasyonu da bulunmaktadır.

## Kullanıcı Rolleri

### Admin (Yönetici)

- Tüm masrafları görüntüleyebilir
- Talepleri onaylayabilir / reddedebilir
- Kategori tanımları yapabilir
- Raporlama işlemleri yapabilir

### Personel (Saha Çalışanı)

- Kendi adına masraf girişi yapabilir
- Taleplerinin durumunu takip edebilir
- Red edilen masraflar için açıklamaları görebilir

## Kullanılan Teknolojiler

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

## Kurulum

1. Repository'yi klonlayın:

   ```bash
   git clone https://github.com/zekiyeipek/ExpenseDb.git
   cd ExpenseDb.API
   ```

2. `appsettings.json` dosyasını kendi bağlantı bilgilerinize göre yapılandırın.

3. Veritabanı kurulumunu gerçekleştirin:

   ```bash
   dotnet ef database update
   ```

4. Uygulamayı başlatın:
   ```bash
   dotnet run
   ```

## 📸 Ekran Görüntüleri

### 🔐 Auth İşlemleri

![Auth](Screenshots/Personel/authorization_token.png)
![Register](Screenshots/Personel/register.png)
![Login](Screenshots/Personel/login.png)

### 👤 Personel İşlemleri

![Create Expense](Screenshots/Personel/Create%20Expense.png)
![Edit Expense](Screenshots/Personel/edit%20expense.png)
![Delete Expense](Screenshots/Personel/delete%20expense.png)
![Upload Document](Screenshots/Personel/Expense%20upload.png)
![Get Expense](Screenshots/Personel/Get%20Expense.png)
![Get By ID](Screenshots/Personel/Get%20Expense%20by%20id.png)
![Reports](Screenshots/Personel/my-reports.png)
![Reports by Category](Screenshots/Personel/my-reports-by-category.png)
![Export CSV](Screenshots/Personel/reports-export.png)

### 🛠️ Admin İşlemleri

![Auth](Screenshots/Admin/admin-auth-all.png)
![Personel Create](Screenshots/Admin/admin-personel-create.png)
![Add Category](Screenshots/Admin/add-expense-category.png)
![Edit Category](Screenshots/Admin/edit-expensecategory.png)
![Delete Category](Screenshots/Admin/delete-expense-category.png)
![Approve Expense](Screenshots/Admin/approve-expenses.png)
![Reject Expense](Screenshots/Admin/reject-expense.png)
![Simulate Payment](Screenshots/Admin/expense-simulate-payment.png)
![Get All Expenses](Screenshots/Admin/get-all-expenses.png)
![Get Category By ID](Screenshots/Admin/get-expensecategory-by-id.png)

### 📊 Admin Raporlama

![Dashboard](Screenshots/Admin/reports-dashboard.png)
![Trend](Screenshots/Admin/reports-trend.png)
![Summary By Personnel](Screenshots/Admin/reports-summary-by-personel.png)
![Person Summary](Screenshots/Admin/reports-person-summary.png)
![Category Summary](Screenshots/Admin/reports-category-summary.png)
![Category Totals](Screenshots/Admin/reports-category-totals.png)
![Status Summary](Screenshots/Admin/reports-status-summary.png)
![Monthly Summary](Screenshots/Admin/reports-monthly-summary.png)
![Weekly Summary](Screenshots/Admin/reports-weekly-summary.png)
![Export All CSV](Screenshots/Admin/reports-all-export.png)
![CSV Dosyası](Screenshots/Admin/tum-masraflar.csv)
