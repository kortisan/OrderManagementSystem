# Order Management System

Aplikasi web berbasis ASP.NET Core untuk manajemen order dan item.

## Fitur Utama

1. Manajemen Order
   - Daftar Order
   - Pencarian Order berdasarkan keyword dan tanggal
   - Tambah, Edit, Hapus Order
   - Export Order ke Excel

2. Manajemen Item
   - Tambah, Edit, Hapus Item untuk setiap Order
   - Inline editing untuk Item

## Teknologi yang Digunakan

- ASP.NET Core 9.0 MVC
- Entity Framework Core
- SQL Server
- CSS
- Javascript
- EPPlus (untuk export Excel)

## Setup Database

1. Restore database dengan menjalankan script `So Schema.sql`
2. Update koneksi database di `appsettings.json`

## Menjalankan Aplikasi

1. Clone repository
2. Build dan jalankan aplikasi dengan Visual Studio atau command line